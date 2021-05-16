using System;
using System.Collections.Generic;

#nullable disable

namespace NetAdvcancedLiveCoding._2021._09.Model
{
    public partial class Customer
    {
        public Customer()
        {
            CustomerPhones = new HashSet<CustomerPhone>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MidleName { get; set; }
        public Guid? Type { get; set; }

        public virtual ICollection<CustomerPhone> CustomerPhones { get; set; }
    }
}
