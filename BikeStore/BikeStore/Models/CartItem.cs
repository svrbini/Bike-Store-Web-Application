namespace BikeStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public CartItem() { }
    }
}
