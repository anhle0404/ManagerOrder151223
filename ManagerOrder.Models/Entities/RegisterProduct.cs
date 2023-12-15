using System;
using System.Collections.Generic;

#nullable disable

namespace ManagerOrder.Models.Entities
{
    public partial class RegisterProduct
    {
        public long Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public long? Unit { get; set; }
        public double? QtyInventory { get; set; }
        public double? QtyImport { get; set; }
        public double? QtyExport { get; set; }
        public double? ProductImportPrice { get; set; }
        public double? WholesalePrice { get; set; }
    }
}
