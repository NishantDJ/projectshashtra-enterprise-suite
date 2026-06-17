using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using ProjectShashtra.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace ProjectShashtra.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBCS");
        }
        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_GetProducts", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = (int)reader["product_id"],
                        Name = reader["product_name"].ToString(),
                        Price = (decimal)reader["price"]


                    });
                }
            }
            //  WHERE — Filtering
            var expensiveprod = products
                                .Where(p=>p.Price>500)
                                .ToList();

            // SELECT — Projection
            var selproj = products
                          .Select(p => p.Name)
                          .ToList();

            // ORDERBY 
            var sort = products
                       .OrderBy(p => p.Stock)
                       .ToList();

            // FIRST / FIRSTORDEFAULT
            var firstordefault = products
                                .FirstOrDefault(p => p.Price > 500);
            var first = products
                                .First(p => p.Price > 500);

            // SINGLE / SINGLEORDEFAULT
            var singordef = products
                .SingleOrDefault(p => p.Price < 500);
            var sing = products
                .Single(p => p.Price < 500);

            // ANY()
            bool hasany = products
                .Any(p => p.Stock ==0);

            return expensiveprod;
        }

        public List<Product> GetProductsById(int id)
        {
            var products = new List<Product>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_GetProductsById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = (int)reader["product_id"],
                        Name = reader["product_name"].ToString(),
                        Price = (decimal)reader["price"]


                    });
                }
            }

            return products;
        }

        public int InsertProducts(Product product)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_insertProduct", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@product_id", product.Id);
                cmd.Parameters.AddWithValue("@product_name", product.Name);
                cmd.Parameters.AddWithValue("@category", product.Category);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@stock_quantity", product.Stock);
                con.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);

            }
        }

        public bool UpdateProduct(Product product)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_updateProduct", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@product_id", product.Id);
                cmd.Parameters.AddWithValue("@product_name", product.Name);
                cmd.Parameters.AddWithValue("@category", product.Category);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@stock_quantity", product.Stock);
                con.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;

            }
        }

        public bool DeleteProduct(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_deleteProduct", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@product_id", id);
                con.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }

        }

        
    } }
