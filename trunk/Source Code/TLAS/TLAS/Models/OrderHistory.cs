//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TLAS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderHistory
    {
        public int OrderID { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public double OrderQty { get; set; }
        public Nullable<double> RemainQty { get; set; }
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        public string OrderCode { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> CarrierID { get; set; }
        public Nullable<int> VehicleID { get; set; }
        public int OrderStatusID { get; set; }
        public Nullable<System.DateTime> OrderDeliveryDT { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string HistoryType { get; set; }
    }
}
