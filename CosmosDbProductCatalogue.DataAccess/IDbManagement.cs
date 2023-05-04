using System;
using System.Linq;

namespace CosmosDbProductCatalogue.DataAccess
{
    public interface IDbManagement
    {
        Task<bool> EnsureContainersCreated();
        Task<bool> EnsureDbCreated();
    }
}
