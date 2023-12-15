using System;
using System.Collections.Generic;

#nullable disable

namespace ManagerOrder.Models.Entities
{
    public partial class HistoryOrder
    {
        public long Id { get; set; }
        public string OrderCode { get; set; }
        public long? IsApproved { get; set; }
        public long? CustomerId { get; set; }
        public string CreatedDate { get; set; }
        public double? TotalIntoMoney { get; set; }
        public double? CustomerPayment { get; set; }
        public double? MoneyOwedCustomer { get; set; }
    }
}
