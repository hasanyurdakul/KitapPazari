using KitapPazariDataAccess.Data;
using KitapPazariDataAccess.Repository.IRepository;
using KitapPazariModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitapPazariDataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productFromDatabase = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productFromDatabase != null)
            {
                productFromDatabase.Title = product.Title;
                productFromDatabase.Description = product.Description;
                productFromDatabase.Author = product.Author;
                productFromDatabase.ISBN = product.ISBN;
                productFromDatabase.ListPrice = product.ListPrice;
                productFromDatabase.Price = product.Price;
                productFromDatabase.Price50 = product.Price50;
                productFromDatabase.Price100 = product.Price100;
                productFromDatabase.CategoryId = product.CategoryId;
                if (productFromDatabase.ImageURL != null)
                {
                    productFromDatabase.ImageURL = product.ImageURL;
                }
            }
        }
    }
}
