
namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.ViewModels
{
    public class ProductViewModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Image => ProductImage.Src;

        public ProductImageViewModel ProductImage { get; set; }

        public string ProductType { get; set; }

        public string Tags { get; set; }

        public string Status { get; set; }

        public string PublishedScope { get; set; }

        public string Handle { get; set; }

        public IEnumerable<VariantViewModel> Variants { get; set; }
    }
}
