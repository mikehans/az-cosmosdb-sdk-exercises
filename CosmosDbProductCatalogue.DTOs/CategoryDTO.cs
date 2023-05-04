using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbProductCatalogue.DTOs;

public class CategoryDTO
{
    public string id { get; set; }
    //public string PartitionKey { get; set; }
    public string name { get; set; }
    public NestedCategoryDTO parent { get; set; }
    public ICollection<NestedCategoryDTO> ancestors { get; set; }
}
