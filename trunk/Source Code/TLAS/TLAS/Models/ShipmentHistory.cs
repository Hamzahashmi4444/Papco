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
    
    public partial class ShipmentHistory
    {
        public int ID { get; set; }
        public bool IsManual { get; set; }
        public Nullable<int> AccessCardID { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public Nullable<bool> DeletedFlag { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int ShipmentStatusID { get; set; }
        public int OrderID { get; set; }
        public Nullable<int> BayID { get; set; }
        public string BayName { get; set; }
        public string CustomerName { get; set; }
        public string VehicleCode { get; set; }
        public string DriverName { get; set; }
        public string DriverCNIC { get; set; }
        public string CarrierName { get; set; }
        public Nullable<System.DateTime> ShipmentDate { get; set; }
        public int ProductID { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string HistoryType { get; set; }
    }
}
