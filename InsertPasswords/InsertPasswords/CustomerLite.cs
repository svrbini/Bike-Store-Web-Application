using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertPasswords
{
    public class CustomerLite
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
        public CustomerLite()
        {

        }

        public CustomerLite( string FirstName, string LastName, int Id)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Id = Id;
        }
    }
}
