using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assignment3_API.Models
{
    public interface IRepository
    {
        Task<bool> SaveChangesAsync();
        void Add<T>(T entity) where T : class;

        // Product operations
        Task<IEnumerable<Product>> GetProductsAsync();

        // Brand operations
        Task<IEnumerable<Brand>> GetBrandsAsync();

        // ProductType operations
        Task<IEnumerable<ProductType>> GetProductTypesAsync();
    }
}