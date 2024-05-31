namespace BikeStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int CountProduct {  get; set; }
        public Order()
        {
            Id = 0;
            CountProduct = 0;
        }
        public Order(int id, int storeId)
        {
            Id = id;
            StoreId = storeId;
            CountProduct = 0;
        }
    }
}
