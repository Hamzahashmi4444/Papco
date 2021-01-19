using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TLAS.ViewModel
{

    public class WeighbridgeViewModel
    {
        //order fields
        [Required(ErrorMessage = "Order Code is required")]
        public int OrderID { get; set; }
        [Required(ErrorMessage = "Order Code is required")]
        public string OrderCode { get; set; }

        //shipment fields
        [Required(ErrorMessage = "Tare Weight is required")]
        [RegularExpression(@"^(\d{3,7})$", ErrorMessage = " Invalid Number should be between 3-7 digits")]
        public Nullable<int> TareWeight { get; set; }
       
        [RegularExpression(@"^(\d{3,7})$", ErrorMessage = " Invalid Number should be between 3-7 digits")]
        public Nullable<int> LoadedWeight { get; set; }
        
        public string Status { get; set; }
        public Nullable<int> ActualWeight { get; set; }

        public System.String CreatedBy { get; set; }
        public System.String ModifiedBy { get; set; }

        public System.DateTime ModifiedDate { get; set; }

        public int ShipmentID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string VehicleCode { get; set; }
        public bool Ismanual { get; set; }

        //shipments fields
        public string DriverName { get; set; }
        public Nullable<int> AccessCardID { get; set; }
        public string CarrierName { get; set; }

        public string CustomerName { get; set; }
        public double OrderQty { get; set; }

        public string Prodtype { get; set; }

    }
}