namespace BikeStore.Models
{
    public class CartRemoveItem
    {
        public int ItemId { get; set; }
        public int Quantity {  get; set; }
        public CartRemoveItem()
        {
            
        }
        public CartRemoveItem(int ItemId, int Quantity)
        {
            this.ItemId = ItemId;
            this.Quantity = Quantity;
        }
    }
}
