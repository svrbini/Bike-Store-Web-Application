using System.Security.Claims;
using System.Text.Json;

namespace BikeStore.Models
{
    public class Cart
    {
        //se no non funziona
        public Cart() { }

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public Cart(HttpRequest Request, ClaimsPrincipal User)
        {
            if (!string.IsNullOrEmpty(Request.Cookies[User.Identity.Name + "MyCart"]))
            {
                this.SetCart(Request.Cookies[User.Identity.Name + "MyCart"]);
            }
        }

        public string GetJson()
        {
            return JsonSerializer.Serialize(this.Items);
        }

        public void SetCart(string cart)
        {
            this.Items = JsonSerializer.Deserialize<List<CartItem>>(cart);
        }

        public void Add(Product p, int quantita)
        {

            bool exists = false;

            foreach (var c in Items)
            {
                if (c.Id == p.Id)
                {
                    c.Quantity += quantita;
                    exists = true;
                    break;
                }

            }

            //lo inserisco nel carrello solo se non esiste

            if (!exists)
            {

                CartItem x = new CartItem()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = quantita
                };

                this.Items.Add(x);
            }
        }

        public void Remove(int Id, int qnt)
        {
            foreach (var c in Items)
            {
                if (c.Id == Id)
                {
                    if ((c.Quantity - qnt) >= 1)  
                        c.Quantity -= qnt;
                    else
                    {
                        if ((c.Quantity - qnt) < 0)
                            throw new ApplicationException("the user is trying to buy more than whats in the cart");
                        Delete(Id);
                    }
                    
                    break;
                }
            }
        }

        public void Delete(int Id)
        {

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Id == Id)
                {
                    Items.RemoveAt(i);
                    break;
                }
            }
        }

        public void Update(int[] Quantita)
        {
            //Ipotesi: indice del vettore delle quantità corrispoden all'indice del vettore degli items

            for (int i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].Quantity = Quantita[i];
            }
        }

        public void ResponseCookie(HttpResponse Response, ClaimsPrincipal User)
        {
            string mycart_serialized = GetJson();

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Append(User.Identity.Name + "MyCart", mycart_serialized, cookieOptions);
        }
    }
}
