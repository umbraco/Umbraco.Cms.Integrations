using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    /// <summary>
    /// Pushes records to Algolia in chunks, isolating the ones Algolia refuses so a single bad
    /// record cannot abandon the rest of the index build.
    /// </summary>
    /// <remarks>
    /// Algolia rejects a whole batch when any record in it exceeds the account's maximum record
    /// size, so a single oversized page used to stop the build dead. Nothing here knows what that
    /// limit is - it lets Algolia decide, then retries the rejected batch one record at a time.
    /// <para>
    /// Kept free of the Algolia client so the retry logic can be tested without an account.
    /// </para>
    /// </remarks>
    public static class AlgoliaBatchPush
    {
        /// <summary>
        /// Matches the batch size the Algolia client uses internally, so a rejection maps to one
        /// request rather than being split differently on the way out.
        /// </summary>
        public const int DefaultChunkSize = 1000;

        /// <summary>
        /// Saves every record, returning the ones Algolia refused individually.
        /// </summary>
        /// <param name="records">The records to save.</param>
        /// <param name="saveBatch">Saves a batch of records. Expected to throw when Algolia rejects it.</param>
        /// <param name="saveOne">Saves a single record. Expected to throw when Algolia rejects it.</param>
        /// <param name="chunkSize">Records per batch.</param>
        /// <returns>The records that could not be saved, in the order they were attempted.</returns>
        /// <exception cref="Exception">
        /// Rethrows the batch failure when every record in a rejected batch also fails on its own.
        /// That is not one oversized record - it is the whole call failing, an expired key or an
        /// unreachable index - and the caller needs to see it rather than be told everything was
        /// silently skipped.
        /// </exception>
        public static async Task<IReadOnlyList<Record>> SaveAsync(
            IReadOnlyList<Record> records,
            Func<IReadOnlyList<Record>, Task> saveBatch,
            Func<Record, Task> saveOne,
            int chunkSize = DefaultChunkSize)
        {
            ArgumentNullException.ThrowIfNull(records);
            ArgumentNullException.ThrowIfNull(saveBatch);
            ArgumentNullException.ThrowIfNull(saveOne);

            if (chunkSize < 1) throw new ArgumentOutOfRangeException(nameof(chunkSize));

            var skipped = new List<Record>();

            for (var offset = 0; offset < records.Count; offset += chunkSize)
            {
                var chunk = records.Skip(offset).Take(chunkSize).ToList();

                try
                {
                    await saveBatch(chunk);
                }
                catch (Exception batchException)
                {
                    var chunkSkipped = await SaveIndividuallyAsync(chunk, saveOne);

                    // Every record failing is not a size problem, it is the call itself failing.
                    if (chunkSkipped.Count == chunk.Count) throw batchException;

                    skipped.AddRange(chunkSkipped);
                }
            }

            return skipped;
        }

        private static async Task<List<Record>> SaveIndividuallyAsync(
            IReadOnlyList<Record> chunk,
            Func<Record, Task> saveOne)
        {
            var skipped = new List<Record>();

            foreach (var record in chunk)
            {
                try
                {
                    await saveOne(record);
                }
                catch
                {
                    skipped.Add(record);
                }
            }

            return skipped;
        }
    }
}
