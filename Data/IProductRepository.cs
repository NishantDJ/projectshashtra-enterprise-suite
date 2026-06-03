using ProjectShashtra.Models;

namespace ProjectShashtra.Data
{
    public interface IProductRepository
    {
        List<Product> GetProducts();
        List<Product> GetProductsById(int id);
        int InsertProducts(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(int id);


        List<Employee> GetEmployees();
    }

   
}