// See https://aka.ms/new-console-template for more information
using InsertPasswords;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Press any key to generate a password for every user");
Console.ReadLine();

List<CustomerLite> customers = new List<CustomerLite>(); 

List<string> pws = new List<string>();

using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True"))
{
    conn.Open();
	using (SqlCommand cmd = new SqlCommand("select * from customers", conn))
	{
		SqlDataReader reader = cmd.ExecuteReader();
		while (reader.Read())
		{
			CustomerLite cstm = new CustomerLite((string)reader["first_name"], (string)reader["last_name"], (int)reader["customer_id"]);
			customers.Add(cstm);
		}
		reader.Close();
	}

	Console.WriteLine("caricato lista");

	using (SqlCommand cmd2 = new SqlCommand("update customers set password=@pw where customer_id = @id", conn))
	{
		cmd2.Parameters.Add(new SqlParameter("@pw", SqlDbType.NVarChar));
		cmd2.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
		FileStream fs = File.Create("..\\..\\..\\..\\..\\pw\\pwfile.txt");
		StreamWriter wr = new StreamWriter(fs);
		foreach (CustomerLite cstm in customers)
		{
			Random rnd = new Random();
			int n = rnd.Next(2, 4);
			string pw = "";

			try { pw = cstm.FirstName.Substring(0, n); }
			catch (Exception) { pw = cstm.FirstName; }

			try { pw += cstm.LastName.Substring(0, 6 - n); }
			catch (Exception) { pw += cstm.LastName; }

			pw += cstm.Id.ToString();
            wr.WriteLine($"{cstm.FirstName} {cstm.LastName}: {pw.ToString()}");

            cmd2.Parameters["@pw"].Value = ComputeSHA256Hash(pw);
			cmd2.Parameters["@id"].Value = cstm.Id;
			cmd2.ExecuteNonQuery();

			
		}
		wr.Close();
	}
		
}

Console.ReadLine();


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
