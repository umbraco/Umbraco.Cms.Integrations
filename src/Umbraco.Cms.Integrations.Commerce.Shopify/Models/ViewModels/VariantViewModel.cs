namespace Umbraco.Cms.Integrations.Commerce.Shopify.Models.ViewModels
{
    public class VariantViewModel
    {
        public string Price { get; set; }

        public string Sku { get; set; }

        public int Position { get; set; }

        public int InventoryQuantity { get; set; }

        public bool Taxable { get; set; }

    }
}
