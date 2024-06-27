namespace Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos
{
    public class ContentTypeDto
    {
        public int Id { get; set; }

        public string Icon { get; set; }

        public string Alias { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

        public bool AllowRemove { get; set; }

        public IEnumerable<ContentTypePropertyDto> Properties { get; set; }
    }
}
