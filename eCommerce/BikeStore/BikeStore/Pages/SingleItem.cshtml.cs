using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BikeStore.Pages
{
    [BindProperties]
    public class SingleItemModel : PageModel
    {
        public Cart MyCart { get; set; }
        public Product Item{ get; set; }
        string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True";

        public void OnGet(int id)
        {
            Item = getProductById(id);
        }

        public IActionResult OnPostAddToCart(int id)
        {
            Item = getProductById(id);

            MyCart = new Cart(Request, User);

            MyCart.Add(Item, 1);

            MyCart.ResponseCookie(Response, User);

            return RedirectToPage("/Cart");
        }

        private Product getProductById(int id)
        {
            Product prd = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "select p.product_id, product_name, model_year, list_price, category_name, brand_name, SUM(quantity) as qnt " +
                        "from products as p " +

                        "inner join categories on p.category_id=categories.category_id " +
                        "inner join stocks on p.product_id=stocks.product_id " +
                        "inner join brands on p.brand_id=brands.brand_id " +
                        "where p.product_id=" + id +
                        " group by p.product_id,product_name, model_year, list_price, category_name, brand_name";
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        prd = new Product();
                        prd.Id = (int)reader["product_id"];
                        prd.Name = (string)reader["product_name"];
                        prd.Year = (short)reader["model_year"];
                        prd.Price = (double)(decimal)reader["list_price"];
                        prd.Category = (string)reader["category_name"];
                        prd.Brand = (string)reader["brand_name"];
                        prd.Quantity = reader["qnt"] == DBNull.Value ? 0 : (int)reader["qnt"];
                    }
                }
            }
            return prd;
        }
    }
}
