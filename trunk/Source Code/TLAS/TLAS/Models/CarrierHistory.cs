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
    
    public partial class CarrierHistory
    {
        public int CarrierID { get; set; }
        public string CarrierName { get; set; }
        public string CarrierAddress { get; set; }
        public string ContactName { get; set; }
        public string CarrierPhone { get; set; }
        public string CarrierMobile { get; set; }
        public string CarrierEmail { get; set; }
        public bool Active { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public Nullable<System.DateTime> LastActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string HistoryType { get; set; }
    }
}
