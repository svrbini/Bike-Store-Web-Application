using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Reflection.Metadata;

namespace BikeStore.Pages
{
    [BindProperties]
    public class CustomersModel : PageModel
    {
        //private List<Customer> _customers {  get; set; }
        public List<Customer> Customers { get; set; }
        public string SrcParametro { get; set; }

        string _connStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True";


        public void OnGet()
        {
            
            /*
            Customers.Add(new Customer(1, "Debra", "Burks", null, "debra.burks@yahoo.com", "9273 Thorne Ave.", "Orchard Park", "NY", "14127"));
            Customers.Add(new Customer(2, "Kasha", "Todd", null, "kasha.todd@yahoo.com", "910 Vine Street", "Campbell", "CA", "95008"));
            Customers.Add(new Customer(3, "Tameka", "Fisher", null, "tameka.fisher@aol.com", "769C Honey Creek St.", "Redondo Beach", "CA", "90278"));
            Customers.Add(new Customer(4, "Daryl", "Spence", null, "daryl.spence@aol.com", "988 Pearl Lane", "Uniondale", "NY", "11553"));
            Customers.Add(new Customer(5, "Charolette", "Rice", "(916) 381-6003", "charolette.rice@msn.com", "107 River Dr.", "Sacramento", "CA", "95820"));
             */
            riempiLista();
        }


        public void riempiLista()
        {
            Customers = new List<Customer>();
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "select * from customers";
                    cmd.Connection = conn;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Customer cstm = new Customer();
                        cstm.Id = (int)reader["customer_id"];
                        cstm.FirstName = (string)reader["first_name"];
                        cstm.LastName = (string)reader["last_name"];
                        //if (var == null) {var = "";} else {var = (string)reader["phone"]} 
                        cstm.Phone = reader["phone"] == DBNull.Value ? "" : (string)reader["phone"];
                        cstm.Email = (string)reader["email"];
                        cstm.Street = reader["street"] == DBNull.Value ? "" : (string)reader["street"];
                        cstm.City = reader["city"] == DBNull.Value ? "" : (string)reader["city"];
                        cstm.State = reader["state"] == DBNull.Value ? "" : (string)reader["state"];
                        cstm.ZipCode = reader["zip_code"] == DBNull.Value ? "" : (string)reader["zip_code"];
                        Customers.Add(cstm);
                    }
                }
            }
        }
        
        public IActionResult OnPostRicerca()
        {
            if (!String.IsNullOrEmpty(SrcParametro))
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.CommandText = "select * from customers where ";
                        cmd.CommandText += "first_name LIKE @src ";
                        cmd.CommandText += "or last_name LIKE @src ";
                        cmd.CommandText += "or phone LIKE @src ";
                        cmd.CommandText += "or email LIKE @src ";
                        cmd.CommandText += "or street LIKE @src ";
                        cmd.CommandText += "or city LIKE @src ";
                        cmd.CommandText += "or state LIKE @src ";
                        cmd.CommandText += "or zip_code LIKE @src";

                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@src";
                        param.Value = $"%{SrcParametro}%";
                        param.SqlDbType = System.Data.SqlDbType.NVarChar;

                        cmd.Parameters.Add(param);

                        cmd.Connection = conn;
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Customer cstm = new Customer();
                            cstm.Id = (int)reader["customer_id"];
                            cstm.FirstName = (string)reader["first_name"];
                            cstm.LastName = (string)reader["last_name"];
                            //if (var == null) {var = "";} else {var = (string)reader["phone"]} 
                            cstm.Phone = reader["phone"] == DBNull.Value ? "" : (string)reader["phone"];
                            cstm.Email = (string)reader["email"];
                            cstm.Street = (string)reader["street"];
                            cstm.City = (string)reader["city"];
                            cstm.State = (string)reader["state"];
                            cstm.ZipCode = (string)reader["zip_code"];
                            Customers.Add(cstm);
                        }
                    }
                }
            }
            else riempiLista();

            return Page();
        }


        public IActionResult OnGetDelete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.CommandText = "delete from customers where customer_id = @id";

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@id";
                    param.Value = id;
                    param.SqlDbType = System.Data.SqlDbType.Int;
                    cmd.Parameters.Add(param);

                    cmd.Connection = conn;
                    int rowsAffected = cmd.ExecuteNonQuery();

                    riempiLista();
                }
            }


            return Page();
        }

        public IActionResult OnGetEdit(int id)
        {
            return RedirectToPage("/Edit", new { id });
        }

        public IActionResult OnGetAddNew()
        {
            return RedirectToPage("/AddNewCustomer");
        }

    }
}

