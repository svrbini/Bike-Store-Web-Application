using BikeStore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;

namespace BikeStore.Pages
{
    
    public class IndexModel : PageModel
    {
		[BindProperty]
		public Credentials Credentials { get; set; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPostLogin()
        {
            var claims = new List<Claim>();

            string cyphPW = "";
            try
            {

                using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True"))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select customer_id,first_name,password from customers " +
                            "where email = @mail";
                        cmd.Parameters.AddWithValue("@mail", Credentials.Mail);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            Credentials.Id = (int)reader["customer_id"];
                            Credentials.Name = (string)reader["first_name"];
                            cyphPW = (string)reader["password"];
                        }
                        
                    }
                }

                if (ComputeSHA256Hash(Credentials.Password) == cyphPW)
                {

                    //Setting  
                    claims.Add(new Claim(ClaimTypes.Email, Credentials.Mail));
                    claims.Add(new Claim(ClaimTypes.Name, Credentials.Name));
                    claims.Add(new Claim("Id", Credentials.Id.ToString()));
                    //claims.Add(new Claim(ClaimTypes.Role, Credential.Username));

                    var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimPrincipal = new ClaimsPrincipal(claimIdenties);

                    //var authenticationProperties = new AuthenticationProperties() { IsPersistent = isPersistent };

                    var authenticationManager = Request.HttpContext;

                    // Sign In.  
                    authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);


                }

                return RedirectToPage("/index");
            }
            catch (Exception ex)
            {
                return Page();
            }
        }

        static string ComputeSHA256Hash(string input)
        {
            // Convert input string to a byte array
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Create an instance of the SHA256 algorithm class
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash value of the input bytes
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }


        public IActionResult OnGetLogout()
        {
            var authenticationManager = Request.HttpContext;

            // Sign Out.  
            authenticationManager.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return this.RedirectToPage("/Index");
        }

        public IActionResult OnGetEditCustomer()
        {
            int id = 0;
            foreach (var item in User.Claims)
            {
                if (item.Type == "Id")
                {
                    id = Convert.ToInt32(item.Value);
                    break;
                }
            }
            return RedirectToPage("/Edit", new { id });
        }
    }
}