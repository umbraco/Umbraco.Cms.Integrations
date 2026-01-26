using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class InspectionResultDto
    {
        [JsonPropertyName("inspectionResultLink")]
        public string Link { get; set; }

        [JsonPropertyName("indexStatusResult")]
        public IndexStatusResultDto IndexingResult { get; set; }

        [JsonPropertyName("ampResult")]
        public AmpInspectionResultDto AmpResult { get; set; }

        [JsonPropertyName("mobileUsabilityResult")]
        public MobileUsabilityResultDto MobileUsabilityResult { get; set; }

        [JsonPropertyName("richResultsResult")]
        public RichResultsResultDto RichResultsResult { get; set; }
    }

    public class IndexStatusResultDto
    {
        [JsonPropertyName("verdict")]
        public string Verdict { get; set; }

        [JsonPropertyName("coverageState")]
        public string CoverageState { get; set; }

        [JsonPropertyName("robotsTxtState")] 
        public string RobotsTxtState { get; set; }

        [JsonPropertyName("indexingState")]
        public string IndexingState { get; set; }

        [JsonPropertyName("lastCrawlTime")]
        public string LastCrawlTime { get; set; }

        [JsonPropertyName("pageFetchState")]
        public string PageFetchState { get; set; }

        [JsonPropertyName("googleCanonical")]
        public string GoogleCanonical { get; set; }

        [JsonPropertyName("userCanonical")]
        public string UserCanonical { get; set; }

        [JsonPropertyName("crawledAs")]
        public string CrawledAs { get; set; }
    }

    public class AmpInspectionResultDto
    {
        [JsonPropertyName("issues")]
        public List<AmpIssueDto> Issues { get; set; }

        [JsonPropertyName("verdict")]
        public string Verdict { get; set; }

        [JsonPropertyName("ampUrl")]
        public string AmpUrl { get; set; }

        [JsonPropertyName("robotsTxtState")]
        public string RobotsTxtState { get; set; }

        [JsonPropertyName("indexingState")]
        public string IndexingState { get; set; }

        [JsonPropertyName("ampIndexStatusVerdict")]
        public string AmpIndexStatusVerdict { get; set; }

        [JsonPropertyName("lastCrawlTime")]
        public string LastCrawlTime { get; set; }

        [JsonPropertyName("pageFetchState")]
        public string PageFetchState { get; set; }
    }

    public class AmpIssueDto
    {
        [JsonPropertyName("issueMessage")]
        public string IssueMessage { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }
    }

    public class MobileUsabilityResultDto
    {
        [JsonPropertyName("verdict")]
        public string Verdict { get; set; }

        [JsonPropertyName("issues")]
        public List<MobileUsabilityIssueDto> Issues { get; set; }
    }

    public class MobileUsabilityIssueDto
    {
        [JsonPropertyName("issueType")]
        public string IssueType { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class RichResultsResultDto
    {
        [JsonPropertyName("detectedItems")]
        public List<DetectedItemDto> DetectedItems { get; set; }

        [JsonPropertyName("verdict")]
        public string Verdict { get; set; }
    }

    public class DetectedItemDto
    {
        [JsonPropertyName("items")]
        public List<ItemDto> Items { get; set; }

        [JsonPropertyName("richResultType")]
        public string RichResultType { get; set; }
    }

    public class ItemDto
    {
        [JsonPropertyName("issues")]
        public List<RichResultsIssueDto> Issues { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class RichResultsIssueDto
    {
        [JsonPropertyName("issueMessage")]
        public string IssueMessage { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }
    }
}
