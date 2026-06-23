using ProjectShashtra.Data;
using ProjectShashtra.Models;

namespace ProjectShashtra.Services.Interfaces
{
    public interface IProductService
    {
        List<Product> GetProducts();
        List<Product> GetProductsById(int id);
        int InsertProducts(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(int id);

    }
}
