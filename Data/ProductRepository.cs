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

        public List<EmployeeDTO> GetEmployees()
        {
            var employees = new List<EmployeeDTO>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_getEmployees", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new EmployeeDTO()
                    {
                        EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                        UserId = Convert.ToInt32(reader["UserId"]),                                                                   
                        FullName = reader["FullName"].ToString(),                                                                     
                        Email = reader["Email"].ToString(),                                                                           
                        Role = reader["Role"].ToString(),                                                                             
                        Department = reader["Department"].ToString(),
                        Designation = reader["Designation"].ToString(),
                        Salary = Convert.ToDecimal(reader["Salary"]),
                        JoiningDate = Convert.ToDateTime(reader["JoiningDate"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }
            }

            // ALL()
            // All employees active?
            bool allActive = employees.All(e=>e.IsActive);

            //COUNT()
            //Count employees
            int totalemp = employees.Count();

            //AVERAGE()
            //Average salary
            decimal avgsal = employees
                .Average(e=>e.Salary);

            //MAX / MIN
            //Highest salary
            decimal maxsal = employees.Max(e=>e.Salary);
            decimal minsal = employees.Min(e=>e.Salary);

            //DISTINCT()
            //Unique roles
            var roles = employees
                .Select(e => e.Role)
                .Distinct()
                .ToList();

            //TAKE()
            //Top 5 employees
            var top5 = employees
                .Take(5)
                .ToList();

            //SKIP()
            //Pagination(?page=2&pageSize=5)
            var page2 = employees
                .Skip(5)
                .Take(5)
                .ToList();

            //GROUPBY() 🔥 VERY IMPORTANT
            //Group employees by department
            var groupbydept = employees
                .GroupBy(e => e.Department)
                .ToList();

            //Print
            //foreach (var group in groupbydept)
            //{
            //    Console.WriteLine(group.Key);

            //    foreach (var emp in group)
            //    {
            //        Console.WriteLine(emp.FullName);
            //    }
            //}


            //GROUP + COUNT
            //Employees count department - wise
            var departcounts = employees
                .GroupBy(e=>e.Department)
                .Select(g => new
                {

                    Department = g.Key,
                    Count = g.Count()
                }).ToList();

            //GROUP + SUM
            //Total salary by department
            var salarybydept = employees
                .GroupBy(e=>e.Department)
                .Select(g => new
                {
                    Department = g.Key,
                    TotalSalary = g.Sum(e=>e.Salary)
                }).ToList();

            //JOIN() 🔥 SUPER IMPORTANT
            //var result = employees.Join(
            //products,
            //e => e.UserId,
            //p => p.Id,
            //(e, p) => new
            //{
            //    Employee = e.FullName,
            //    Product = p.Name
            //}).ToList();

         
            //Pending        //SELECT MANY()

            //CONTAINS()
            //Find employee names
            var names = new List<string> { "Nishant","Rahul"};
            var result = employees
                .Where(e=>names.Contains(e.FullName))
                .ToList();

            //MULTIPLE CONDITIONS
            var result3 = employees
                .Where(e=>e.IsActive &&
                e.Salary>50000 &&
                e.Department =="IT")
                .ToList();

            //THENBY()
            var sorted = employees
                .OrderBy(e => e.Department)
                .ThenBy(e => e.FullName)
                .ToList();

            //PAGINATION API STYLE
            int page = 1;
            int pageSize = 10;
            var result4 = employees
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToList();

            //REAL INTERVIEW QUESTION
            //Second Highest Salary
            var sechighest = employees
                .OrderByDescending(e=>e.Salary)
                .Skip(1)
                .FirstOrDefault();

            //ACTIVE EMPLOYEES GROUPED BY ROLE
            var result5 = employees
                .Where(e=>e.IsActive)
                .GroupBy(e=>e.Role)
                .Select(g=>new
                {
                    Role = g.Key,
                    Employees = g.ToList()
                }).ToList();

            //EMPLOYEE DTO TRANSFORMATION
            var dto = employees
                .Select(e => new
                {
                    Name=e.FullName,
                    Department = e.Department,
                    AnnualSalary = e.Salary*12
                })
                .ToList();
            return employees;
        }
    } }
