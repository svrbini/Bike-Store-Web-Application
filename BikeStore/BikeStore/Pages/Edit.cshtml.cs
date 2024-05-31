using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace BikeStore.Pages
{
    [BindProperties]
    public class EditModel : PageModel
    {
        public Customer EditCustomer { get; set; }

		public void OnGet(int id)
		{
			using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True"))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.CommandText = "select * from customers where customer_id = @id";
					SqlParameter param = new SqlParameter
					{
						ParameterName = "@id",
						Value = id,
						SqlDbType = System.Data.SqlDbType.Int
					};
					cmd.Parameters.Add(param);
					cmd.Connection = conn;

					SqlDataReader reader = cmd.ExecuteReader();

					while(reader.Read())
					{
						EditCustomer = new Customer();
						EditCustomer.Id = (int)reader["customer_id"];
						EditCustomer.FirstName = (string)reader["first_name"];
						EditCustomer.LastName = (string)reader["last_name"];
						EditCustomer.Phone = reader["phone"] == DBNull.Value ? "" : (string)reader["phone"];
						EditCustomer.Email = (string)reader["email"];
						EditCustomer.Street = reader["street"] == DBNull.Value ? "" : (string)reader["street"];
						EditCustomer.City = reader["city"] == DBNull.Value ? "" : (string)reader["city"];
						EditCustomer.State = reader["state"] == DBNull.Value ? "" : (string)reader["state"];
						EditCustomer.ZipCode = reader["zip_code"] == DBNull.Value ? "" : (string)reader["zip_code"];
					}
					reader.Close();
				}
			}
		}



		public IActionResult OnPostUpdate()
		{
			using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True"))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand())
				{
					cmd.Parameters.AddWithValue("@id", EditCustomer.Id);
					cmd.Parameters.AddWithValue("@first_name", EditCustomer.FirstName);
					cmd.Parameters.AddWithValue("@last_name", EditCustomer.LastName);
					cmd.Parameters.AddWithValue("@email", EditCustomer.Email);
					cmd.Parameters.AddWithValue("@street", EditCustomer.Street);
					cmd.Parameters.AddWithValue("@city", EditCustomer.City);
					cmd.Parameters.AddWithValue("@state", EditCustomer.State);
					cmd.Parameters.AddWithValue("@zip_code", EditCustomer.ZipCode);

					cmd.CommandText = "update customers " +
					"set first_name = @first_name, last_name = @last_name, email = @email, street = @street, city = @city, state = @state, zip_code = @zip_code ";
					if (!string.IsNullOrEmpty(EditCustomer.Phone))
					{
						cmd.CommandText += ", phone = @phone ";
						cmd.Parameters.AddWithValue("@phone", EditCustomer.Phone);
					}
					cmd.CommandText += "where customer_id = @id";

					cmd.Connection = conn;
					cmd.ExecuteNonQuery();

					return RedirectToPage("/Index");

				}
			}
		}
    }
}
