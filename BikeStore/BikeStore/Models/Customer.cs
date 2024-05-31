namespace BikeStore.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public Customer()
        {

        }

        public Customer(int Id, string FirstName, string LastName, string Phone, string Email, string Street, string City, string State, string ZipCode)
        {
            this.Id = Id;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Phone = Phone;
            this.Email = Email;
            this.Street = Street;
            this.City = City;
            this.State = State;
            this.ZipCode = ZipCode;
        }
    }
}
