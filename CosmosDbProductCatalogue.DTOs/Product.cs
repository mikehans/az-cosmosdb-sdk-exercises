using System;
using System.Linq;

namespace CosmosDbProductCatalogue.DTOs
{
    public class Product
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductKey { get; set; }
        public string Type { get; set; }
        public ICollection<NestedCategoryDTO> Categories { get; set; }
        public ICollection<ProductVariant> Variants { get; set; }
        public ProductVariant MasterVariant { get; set; }
    }
}
