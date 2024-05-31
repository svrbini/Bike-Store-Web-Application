using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace BikeStore.Pages
{
    [BindProperties]
    public class AddNewCustomerModel : PageModel
    {
        public Customer NewCustomer { get; set; }

        public void OnGet()
        {
			
        }

        public IActionResult OnPostAdd()
        {

			using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True"))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.Parameters.AddWithValue("@id", NewCustomer.Id);
					cmd.Parameters.AddWithValue("@first_name", NewCustomer.FirstName);
					cmd.Parameters.AddWithValue("@last_name", NewCustomer.LastName);
					cmd.Parameters.AddWithValue("@email", NewCustomer.Email);
					cmd.Parameters.AddWithValue("@street", NewCustomer.Street);
					cmd.Parameters.AddWithValue("@city", NewCustomer.City);
					cmd.Parameters.AddWithValue("@state", NewCustomer.State);
					cmd.Parameters.AddWithValue("@zip_code", NewCustomer.ZipCode);

                    if (string.IsNullOrEmpty(NewCustomer.Phone))
					{
                        cmd.CommandText = "INSERT INTO customers(first_name, last_name, email, street, city, state, zip_code) VALUES(@first_name, @last_name, @email, @street, @city, @state, @zip_code)";
                    }
                    else
					{
						cmd.CommandText = "INSERT INTO customers(first_name, last_name, phone, email, street, city, state, zip_code) VALUES(@first_name, @last_name, @phone, @email, @street, @city, @state, @zip_code)";

                        SqlParameter param = new SqlParameter
                        {
                            ParameterName = "@phone",
                            Value = NewCustomer.Phone,
                            SqlDbType = System.Data.SqlDbType.VarChar
                        };
                        cmd.Parameters.Add(param);

                    }

					cmd.Connection = conn;
					int rowsAffected = cmd.ExecuteNonQuery();

				}
			}

			return RedirectToPage("/Customers");
        }
    }
}
