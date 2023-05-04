using System;
using System.Linq;

namespace CosmosDbProductCatalogue.DTOs
{
    public class ProductVariant
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string ProductId { get; set; }
        public bool IsMasterVariant { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
    }
}
