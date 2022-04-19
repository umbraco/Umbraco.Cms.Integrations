using System.Collections.Generic;

using Newtonsoft.Json;

namespace Umbraco.Cms.Integrations.SEO.GoogleSearchConsole.URLInspectionTool.Models.Dtos
{
    public class InspectionResultDto
    {
        [JsonProperty("inspectionResultLink")]
        public string Link { get; set; }

        [JsonProperty("indexStatusResult")]
        public IndexStatusResultDto IndexingResult { get; set; }

        [JsonProperty("ampResult")]
        public AmpInspectionResultDto AmpResult { get; set; }

        [JsonProperty("mobileUsabilityResult")]
        public MobileUsabilityResultDto MobileUsabilityResult { get; set; }

        [JsonProperty("richResultsResult")]
        public RichResultsResultDto RichResultsResult { get; set; }
    }

    public class IndexStatusResultDto
    {
        [JsonProperty("verdict")]
        public string Verdict { get; set; }

        [JsonProperty("coverageState")]
        public string CoverageState { get; set; }

        [JsonProperty("robotsTxtState")] 
        public string RobotsTxtState { get; set; }

        [JsonProperty("indexingState")]
        public string IndexingState { get; set; }

        [JsonProperty("lastCrawlTime")]
        public string LastCrawlTime { get; set; }

        [JsonProperty("pageFetchState")]
        public string PageFetchState { get; set; }

        [JsonProperty("googleCanonical")]
        public string GoogleCanonical { get; set; }

        [JsonProperty("userCanonical")]
        public string UserCanonical { get; set; }

        [JsonProperty("crawledAs")]
        public string CrawledAs { get; set; }
    }

    public class AmpInspectionResultDto
    {
        [JsonProperty("issues")]
        public List<AmpIssueDto> Issues { get; set; }

        [JsonProperty("verdict")]
        public string Verdict { get; set; }

        [JsonProperty("ampUrl")]
        public string AmpUrl { get; set; }

        [JsonProperty("robotsTxtState")]
        public string RobotsTxtState { get; set; }

        [JsonProperty("indexingState")]
        public string IndexingState { get; set; }

        [JsonProperty("ampIndexStatusVerdict")]
        public string AmpIndexStatusVerdict { get; set; }

        [JsonProperty("lastCrawlTime")]
        public string LastCrawlTime { get; set; }

        [JsonProperty("pageFetchState")]
        public string PageFetchState { get; set; }
    }

    public class AmpIssueDto
    {
        [JsonProperty("issueMessage")]
        public string IssueMessage { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }
    }

    public class MobileUsabilityResultDto
    {
        [JsonProperty("verdict")]
        public string Verdict { get; set; }

        [JsonProperty("issues")]
        public List<MobileUsabilityIssueDto> Issues { get; set; }
    }

    public class MobileUsabilityIssueDto
    {
        [JsonProperty("issueType")]
        public string IssueType { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class RichResultsResultDto
    {
        [JsonProperty("detectedItems")]
        public List<DetectedItemDto> DetectedItems { get; set; }

        [JsonProperty("verdict")]
        public string Verdict { get; set; }
    }

    public class DetectedItemDto
    {
        [JsonProperty("items")]
        public List<ItemDto> Items { get; set; }

        [JsonProperty("richResultType")]
        public string RichResultType { get; set; }
    }

    public class ItemDto
    {
        [JsonProperty("issues")]
        public List<RichResultsIssueDto> Issues { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class RichResultsIssueDto
    {
        [JsonProperty("issueMessage")]
        public string IssueMessage { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }
    }
}
