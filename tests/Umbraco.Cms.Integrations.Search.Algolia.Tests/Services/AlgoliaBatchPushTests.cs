using Umbraco.Cms.Integrations.Search.Algolia.Services;

using Xunit;

// Xunit.Record collides with the Algolia record model.
using Record = Umbraco.Cms.Integrations.Search.Algolia.Models.Record;

namespace Umbraco.Cms.Integrations.Search.Algolia.Tests.Services;

public class AlgoliaBatchPushTests
{
    private static IReadOnlyList<Record> Records(params int[] ids) =>
        ids.Select(id => new Record { Id = id, Name = $"Node {id}", ObjectID = id.ToString() }).ToList();

    /// <summary>
    /// Records a batch save as one call and rejects any batch containing a "rejected" record,
    /// the way Algolia rejects a whole batch when one record is over the size limit.
    /// </summary>
    private sealed class FakeIndex(params int[] rejectedIds)
    {
        private readonly HashSet<int> _rejected = [.. rejectedIds];

        public List<int> Saved { get; } = [];

        public List<int> BatchSizes { get; } = [];

        public int SingleSaveCount { get; private set; }

        public Task SaveBatch(IReadOnlyList<Record> batch)
        {
            BatchSizes.Add(batch.Count);

            if (batch.Any(r => _rejected.Contains(r.Id)))
            {
                throw new InvalidOperationException("Record too big");
            }

            Saved.AddRange(batch.Select(r => r.Id));
            return Task.CompletedTask;
        }

        public Task SaveOne(Record record)
        {
            SingleSaveCount++;

            if (_rejected.Contains(record.Id))
            {
                throw new InvalidOperationException("Record too big");
            }

            Saved.Add(record.Id);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task SaveAsync_WhenNothingIsRejected_SavesEverythingAndSkipsNothing()
    {
        var index = new FakeIndex();

        var skipped = await AlgoliaBatchPush.SaveAsync(Records(1, 2, 3), index.SaveBatch, index.SaveOne);

        Assert.Empty(skipped);
        Assert.Equal([1, 2, 3], index.Saved);
        Assert.Equal(0, index.SingleSaveCount);
    }

    [Fact]
    public async Task SaveAsync_WhenOneRecordIsRejected_SavesTheRestAndReportsIt()
    {
        var index = new FakeIndex(2);

        var skipped = await AlgoliaBatchPush.SaveAsync(Records(1, 2, 3), index.SaveBatch, index.SaveOne);

        Assert.Equal([2], skipped.Select(r => r.Id));
        Assert.Equal([1, 3], index.Saved);
    }

    [Fact]
    public async Task SaveAsync_OnlyRetriesTheRejectedBatch()
    {
        // Chunk of 2: [1,2] fails on record 2, [3,4] goes through untouched.
        var index = new FakeIndex(2);

        await AlgoliaBatchPush.SaveAsync(Records(1, 2, 3, 4), index.SaveBatch, index.SaveOne, chunkSize: 2);

        Assert.Equal([2, 2], index.BatchSizes);

        // Only the failed batch is retried record by record.
        Assert.Equal(2, index.SingleSaveCount);

        // 1 saved on retry, 2 rejected, then 3 and 4 go through as a batch.
        Assert.Equal([1, 3, 4], index.Saved);
    }

    [Fact]
    public async Task SaveAsync_WhenEveryRecordInABatchFails_RethrowsInsteadOfReportingThemAllSkipped()
    {
        // A bad API key fails every record. That is the call failing, not oversized content,
        // and it must not be reported as "the build succeeded, we just skipped everything".
        var index = new FakeIndex(1, 2, 3);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => AlgoliaBatchPush.SaveAsync(Records(1, 2, 3), index.SaveBatch, index.SaveOne));

        Assert.Equal("Record too big", ex.Message);
        Assert.Empty(index.Saved);
    }

    [Fact]
    public async Task SaveAsync_SplitsIntoChunksOfTheGivenSize()
    {
        var index = new FakeIndex();

        await AlgoliaBatchPush.SaveAsync(Records(1, 2, 3, 4, 5), index.SaveBatch, index.SaveOne, chunkSize: 2);

        Assert.Equal([2, 2, 1], index.BatchSizes);
    }

    [Fact]
    public async Task SaveAsync_WithNoRecords_DoesNothing()
    {
        var index = new FakeIndex();

        var skipped = await AlgoliaBatchPush.SaveAsync([], index.SaveBatch, index.SaveOne);

        Assert.Empty(skipped);
        Assert.Empty(index.BatchSizes);
    }
}
