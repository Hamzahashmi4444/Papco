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
    
    public partial class AccessCardHistory
    {
        public int CardID { get; set; }
        public int BayID { get; set; }
        public bool Active { get; set; }
        public bool IsAssigned { get; set; }
        public System.DateTime LastActive { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CardKey { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string HistoryType { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
