using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assignment3_API.Models
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _appDbContext;

        public Repository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Add<T>(T entity) where T : class
        {
            _appDbContext.Add(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _appDbContext.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                .Where(p => p.IsActive && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Brand>> GetBrandsAsync()
        {
            return await _appDbContext.Brands
                .AsNoTracking()
                .Where(b => b.IsActive && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductType>> GetProductTypesAsync()
        {
            return await _appDbContext.ProductTypes
                .AsNoTracking()
                .Where(pt => pt.IsActive && !pt.IsDeleted)
                .ToListAsync();
        }
    }
}