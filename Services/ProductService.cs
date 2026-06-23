using ProjectShashtra.Models;
using ProjectShashtra.Repositories.Interfaces;
using ProjectShashtra.Services.Interfaces;

namespace ProjectShashtra.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public List<Product> GetProducts()
        {
            return _repository.GetProducts();
        }

        public List<Product> GetProductsById(int id)
        {
            return _repository.GetProductsById(id);
        }

        public int InsertProducts(Product product)
        {
            if (product.Price < 0)
                throw new Exception("Price cannot be negative");

            return _repository.InsertProducts(product);
        }

        public bool UpdateProduct(Product product)
        {
            if (product.Price < 0)
                throw new Exception("Price cannot be negative");

            return _repository.UpdateProduct(product);
        }

        public bool DeleteProduct(int id)
        {
            return _repository.DeleteProduct(id);
        }
    }
}