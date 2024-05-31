using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BikeStore.Pages
{
    [BindProperties]
    public class ProductsPageModel : PageModel
    {
		public List<Product> Products { get; set; }
        public string SrcParameter { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Brands { get; set; }
        public List<string> Stores { get; set; }
        public string OrderCriteria { get; set; }
        public string WhereClause { get; set; }
        public double Price { get; set; }
        string _connStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True";

        public void OnGet()
        {
            Products = new List<Product>();
			using (SqlConnection conn = new SqlConnection(_connStr))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandText = "select p.product_id, product_name, model_year, list_price, category_name, brand_name, SUM(quantity) as qnt " +
						"from products as p " +
						"inner join categories on p.category_id=categories.category_id " +
						"inner join stocks on p.product_id=stocks.product_id " +
						"inner join brands on p.brand_id=brands.brand_id " +
						"group by p.product_id,product_name, model_year, list_price, category_name, brand_name";
					cmd.Connection = conn;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product prd = new Product();
                            prd.Id = (int)reader["product_id"];
                            prd.Name = (string)reader["product_name"];
                            prd.Year = (short)reader["model_year"];
                            prd.Price = (double)(decimal)reader["list_price"];
                            prd.Category = (string)reader["category_name"];
                            prd.Brand = (string)reader["brand_name"];
                            prd.Quantity = reader["qnt"] == DBNull.Value ? 0 : (int)reader["qnt"];
                            Products.Add(prd);
                        }
                    }

                    Categories = new List<string>();
                    cmd.CommandText = "select * from categories";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) { Categories.Add((string)reader["category_name"]);}
                    }
                    Brands = new List<string>();
                    cmd.CommandText = "select * from brands";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) { Brands.Add((string)reader["brand_name"]); }
                    }
                    Stores = new List<string>();
                    cmd.CommandText = "select * from stores";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) { Stores.Add((string)reader["city"] + ", " + (string)reader["state"]); }
                    }
                }

			}
		}

        public IActionResult OnPostSearch()
        {
            Products = new List<Product>();
            //Products = GetProducts($"where product_name LIKE %{ SrcParameter}%");
            if (!String.IsNullOrEmpty(SrcParameter))
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = "select p.product_id, product_name, model_year, list_price, category_name, brand_name, SUM(quantity) as qnt " +
                        "from products as p " +
                        "inner join categories on p.category_id=categories.category_id " +
                        "inner join stocks on p.product_id=stocks.product_id " +
                        "inner join brands on p.brand_id=brands.brand_id " +
                        "where product_name LIKE @src " +
                        "group by p.product_id,product_name, model_year, list_price, category_name, brand_name";

                        cmd.Parameters.AddWithValue("@src", $"%{SrcParameter}%");

                        cmd.Connection = conn;
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Product prd = new Product();
                            prd.Id = (int)reader["product_id"];
                            prd.Name = (string)reader["product_name"];
                            prd.Year = (short)reader["model_year"];
                            prd.Price = (double)(decimal)reader["list_price"];
                            prd.Category = (string)reader["category_name"];
                            prd.Brand = (string)reader["brand_name"];
                            prd.Quantity = reader["qnt"] == DBNull.Value ? 0 : (int)reader["qnt"];
                            Products.Add(prd);
                        }
                    }
                }
            }

            return Page();
        }


        public IActionResult OnPostSelect()
        {
            Products = GetProducts("");
            return Page();
        }


        public List<Product> GetProducts(string whereClause)
        {
            if (!string.IsNullOrEmpty(SrcParameter)) { whereClause += $"where product_name LIKE %{SrcParameter}%"; } 
            Products = new List<Product>();
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "select p.product_id, product_name, model_year, list_price, category_name, brand_name, SUM(quantity) as qnt " +
                        "from products as p " +
                        "inner join categories on p.category_id=categories.category_id " +
                        "inner join stocks on p.product_id=stocks.product_id " +
                        "inner join brands on p.brand_id=brands.brand_id " +
                        "@where " +
                        "group by p.product_id,product_name, model_year, list_price, category_name, brand_name " +
                        "@order_by";
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@where", whereClause);
                    cmd.Parameters.AddWithValue("@order_by", OrderCriteria);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product prd = new Product();
                            prd.Id = (int)reader["product_id"];
                            prd.Name = (string)reader["product_name"];
                            prd.Year = (short)reader["model_year"];
                            prd.Price = (double)(decimal)reader["list_price"];
                            prd.Category = (string)reader["category_name"];
                            prd.Brand = (string)reader["brand_name"];
                            prd.Quantity = reader["qnt"] == DBNull.Value ? 0 : (int)reader["qnt"];
                            Products.Add(prd);
                        }
                    }
                }
            }
            return Products;
        }

        

        public IActionResult OnGetBuy(int id)
        {
            return RedirectToPage("/SingleItem", new { id });
        }
    }
}
