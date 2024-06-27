namespace Umbraco.Cms.Integrations.Search.Algolia.Models.ContentTypeDtos
{
    public class ContentTypePropertyDto
    {
        public int Id { get; set; }

        public string Icon { get; set; }

        public string Alias { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public bool Selected { get; set; }
    }
}
