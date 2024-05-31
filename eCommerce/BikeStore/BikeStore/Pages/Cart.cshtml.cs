 using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static System.Formats.Asn1.AsnWriter;
using System.Security.Principal;

namespace BikeStore.Pages
{
    [BindProperties]
    public class CartModel : PageModel
    {
        public Cart MyCart { get; set; }
        string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BikeStores;Integrated Security=True";
        public List<string> AllStores;
        public string DefaultStore { get; set; }


        public void OnGet()
        {
            MyCart = new Cart(Request, User);
            AllStores = loadStores();
        }

        private List<string> loadStores()
        {
            List<string> result = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select city, state from stores";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(((string)reader["city"]) + ", " + ((string)reader["state"]));
                        }
                    }
                }
            }
            return result;
        }

        public IActionResult OnPostRemoveOneItem(int id)
        {
            MyCart = new Cart(Request, User);

            MyCart.Remove(id,1);

            MyCart.ResponseCookie(Response, User);

            return this.RedirectToPage("/Cart");
        }

        public IActionResult OnPostRemoveAllOfOneItem(int id)
        {
            MyCart = new Cart(Request, User);

            MyCart.Delete(id);

            MyCart.ResponseCookie(Response, User);

            return this.RedirectToPage("/Cart");
        }




        public IActionResult OnPostCheckout()
        {
            MyCart = new Cart(Request, User);

            if (MyCart.Items.Count > 0)
            {
                string UserId = "";
                foreach (var item in User.Claims)
                {
                    if (item.Type == "Id")
                    {
                        UserId = item.Value;
                        break;
                    }
                }

                List<CartRemoveItem> cartItemsToRemove = new List<CartRemoveItem>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlTransaction trns = null;
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            trns = conn.BeginTransaction();
                            cmd.Connection = conn;
                            cmd.Transaction = trns;

                            cmd.CommandText = "select COUNT(*) from stores";
                            int stores = (int)cmd.ExecuteScalar();
                            Order[] orders = new Order[stores];
                            for (int i = 0; i < orders.Length; i++)
                            {
                                orders[i] = new Order();
                            }


                            DefaultStore = "Santa Cruz";

                            //select default store id
                            cmd.CommandText = "select store_id " +
                                "from stores " +
                                "where city = @city";
                            cmd.Parameters.AddWithValue("@city", DefaultStore);
                            int store = (int)cmd.ExecuteScalar();
                            orders[0].StoreId = store;

                            //fill order array
                            cmd.CommandText = "select store_id " +
                                "from stores " +
                                "where city <> @city";
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                int i = 1;
                                while (reader.Read())
                                {
                                    orders[i].Id = (int)reader["store_id"];
                                    i++;
                                }
                            }


                            //imposta un modo per vedere se è disponibile o gestire in front end

                            cmd.Parameters.AddWithValue("@id", UserId);
                            //order_items
                            cmd.Parameters.AddWithValue("@ord_id", System.Data.SqlDbType.Int);
                            cmd.Parameters.AddWithValue("@i", 0);
                            cmd.Parameters.Add("@prod_id", System.Data.SqlDbType.Int);
                            cmd.Parameters.AddWithValue("@qnt", 0);
                            cmd.Parameters.AddWithValue("@price", 0);
                            cmd.Parameters.Add("@store", System.Data.SqlDbType.Int);

                            foreach (var item in MyCart.Items)
                            {
                                //ciclo per ognuno degli stores
                                for (int i = 0; i < orders.Length && item.Quantity > 0; i++)
                                {
                                    cmd.Parameters["@store"].Value = orders[i].StoreId;
                                    cmd.Parameters["@prod_id"].Value = item.Id;
                                    cmd.CommandText = "SELECT quantity from stocks where product_id = @prod_id and store_id = @store";
                                    int availableItems = (int)cmd.ExecuteScalar();
                                    if (availableItems > 0)
                                    {
                                        if (orders[i].Id == 0)
                                        {
                                            cmd.CommandText = "INSERT INTO orders (order_id, customer_id, order_status,order_date, required_date, store_id, staff_id) " +
                                            "VALUES ( @ord_id @id, 2, @ord_date, @req_date, @store, 1)";
                                            cmd.Parameters.AddWithValue("@ord_date", DateTime.Now);
                                            cmd.Parameters.AddWithValue("@req_date", DateTime.Now.AddDays(3));
                                            //query sull'ultimo id + 1

                                            cmd.ExecuteNonQuery();
                                            //prendi id dell'ordine --> SELECT SCOPE_IDENTITY(), non funziona con transazioni usa sequenze
                                            //utilizzo una lista di id ma non è corretta, solo per verificare il funzionamento
                                            cmd.CommandText = "SELECT SCOPE_IDENTITY()";
                                            orders[i].Id = (int)cmd.ExecuteScalar();
                                        }

                                        orders[i].CountProduct++;

                                        //order_item
                                        cmd.Parameters["@i"].Value = orders[i].CountProduct;
                                        cmd.Parameters["@price"].Value = item.Price;
                                        //quantità item - store
                                        int orderQnt;
                                        if(item.Quantity <= availableItems)
                                            orderQnt = item.Quantity;
                                        else
                                            orderQnt = availableItems;

                                        cmd.Parameters["@qnt"].Value = orderQnt;


                                        cmd.CommandText = "INSERT INTO order_items(order_id, item_id, product_id, quantity, list_price, discount)" +
                                        "VALUES(@ord_id, @i, @prod_id, @qnt, @price, 0)";

                                        cmd.ExecuteNonQuery();

                                        cmd.CommandText = "UPDATE stocks " +
                                            "SET quantity = quantity - @qnt " +
                                            "WHERE product_id = @prod_id";

                                        //rimuovi item dal cart --> lista di Id e togli poi
                                        foreach (var cartItem in cartItemsToRemove)
                                        {
                                            //finisci
                                        }
                                    }

                                }
                            }
                            

                            trns.Commit();

                            foreach (var item in cartItemsToRemove)
                            {
                                try
                                {
                                    MyCart.Remove(item.ItemId, item.Quantity);
                                }
                                catch (ApplicationException)
                                {

                                }

                            }
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }
                }


                /*
                 * select store_id, quantity
                    from stocks
                    where product_id = 6 AND quantity =(
                        select MAX(quantity)
                        from stocks
                        where product_id = 6
                    )
                 */

                MyCart.Items.Clear();
                MyCart.ResponseCookie(Response, User);
            }
            return this.RedirectToPage("/Index");
        }
    }
}
