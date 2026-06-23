using ProjectShashtra.Models;

namespace ProjectShashtra.Repositories.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetProducts();
        List<Product> GetProductsById(int id);
        int InsertProducts(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(int id);
        
    }

   
}