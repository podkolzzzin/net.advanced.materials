using System;
using System.Collections.Generic;

#nullable disable

namespace NetAdvcancedLiveCoding._2021._09.Model
{
    public partial class CustomerPhone
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public string PhoneNumber { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
