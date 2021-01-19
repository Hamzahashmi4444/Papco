using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TLAS.Models
{
    //public class ProductMetadata
    //{
    //    public System.DateTime CreatedDate;
    //}
    public class CustomerMetadata
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(50, MinimumLength=3 ,ErrorMessage="Customer Name should be more than 2 and less than 51 characters")]
        public string CustomerName;
        [StringLength(200, ErrorMessage = "Address max lenght 200 Characters")]
        public string CustomerAddress;
        [StringLength(50, ErrorMessage = "Name max lenght 50 Characters")]
        public string ContactName;
        //[Required(ErrorMessage = "Phone number is required")]
        //[RegularExpression(@"^(\d{10}|\d{11}|\d{12})$", ErrorMessage = " Invalid Number, Allowed Format e.g(03001234567,0421234567,042111222333)")]
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(\d{10,16})$", ErrorMessage = " Phone Number should be between 10-16 digits")]
        public string CustomerPhone;
        [RegularExpression(@"(^$)|^(\d{11})$", ErrorMessage = " Invalid Number, Allowed Format e.g(03001234567)")]
        public string CustomerMobile;
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string CustomerEmail;
        [StringLength(13, MinimumLength = 13, ErrorMessage = "CNIC is not completed.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "CNIC must be numeric.")]
        [Display(Name = "CNIC #")]
        public string CNIC;
        [Display(Name = "Select Image")]
        public string SignatureImagePath { get; set; }


    }
    public class CarrierMetadata
    {
        [Required(ErrorMessage = "Carrier name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Carrier Name should be more than 2 and less than 51 characters")]
        public string CarrierName;
        [StringLength(200, ErrorMessage = "Address max lenght 200 Characters")]
        public string CarrierAddress;
        [StringLength(50, ErrorMessage = "Name max lenght 50 Characters")]
        public string ContactName;
        //[Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(\d{10,16})$", ErrorMessage = " Phone Number should be between 10-16 digits")]
        public string CarrierPhone;
        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"(^$)|^(\d{11})$", ErrorMessage = " Invalid Number, Allowed Format e.g(03001234567)")]
        public string CarrierMobile;
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string CarrierEmail { get; set; }
    }
    public class DriverMetadata
    {
        [Required(ErrorMessage = "Carrier Name is required")]
        public int CarrierID { get; set; }
        [Required(ErrorMessage = "Driver name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Driver Name should be more than 2 and less than 51 characters")]
        public string DriverName { get; set; }
        [Required(ErrorMessage = "License number is required")]
        public string LicenseNo { get; set; }
        [Required(ErrorMessage = "License issue date is required")]
        public Nullable<System.DateTime> LicenseIDate { get; set; }
        [Required(ErrorMessage = "License expiry date is required")]
        public Nullable<System.DateTime> LicenseEDate { get; set; }
        [Required(ErrorMessage = "CNIC is required")]
        [RegularExpression(@"^(\d{13})$", ErrorMessage = " CNIC should be 13 of digits")]
        public string CNIC { get; set; }
        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"(^$)|^(\d{11})$", ErrorMessage = " Invalid Number, Allowed Format e.g(03001234567)")]
        public string Mobile { get; set; }
        public Nullable<System.DateTime> LastActive { get; set; }
        [StringLength(200, ErrorMessage = "Remarks max lenght 200 Characters")]
        public string Remarks { get; set; }
    }
    public class VehicleMetadata
    {
        [Required(ErrorMessage = "Vehicle Code is required")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "vehicle Code Should be between 3-10 characters")]
        public string VehicleCode { get; set; }
        [Required(ErrorMessage = "License Number is required")]
        public string LicenseNo { get; set; }
        [Required(ErrorMessage = "Carrier Name is required")]
        public int CarrierID { get; set; }
        [Required(ErrorMessage = "Driver Name is required")]
        public int DriverID { get; set; }
        [Required(ErrorMessage = "License issue date is required")]
        public Nullable<System.DateTime> LicenseIDate { get; set; }
        [Required(ErrorMessage = "License expiry date is required")]
        public Nullable<System.DateTime> LicenseEDate { get; set; }
    }
    public class TrailerMetadata
    {
        [Required(ErrorMessage = "Trailer Code is required")]
        public string TrailerCode { get; set; }

        //[Required(ErrorMessage = "Bowser certificate number is required")]
        //public string BowserCalibrationCertificate { get; set; }
        //[Required(ErrorMessage = "Compartment Count is required")]
        //public int CompCount { get; set; }
        //[Required(ErrorMessage = "Compartment Capacity is required")]
        //public int Capacity { get; set; }
    }
    public class CompartmentMetadata
    {
        [Required(ErrorMessage = "Compartment Code is required")]
        public string CompartmentCode { get; set; }
        [RegularExpression(@"^(\d{1,13})$", ErrorMessage = "Only Digits allow")]
        public int Capactiy { get; set; }

        //[RegularExpression(@"^(\d{1,13})$", ErrorMessage = "Only Digits allow")]
        

        [Display(Name = "Chamber Dip [MM]")]
        [Required(ErrorMessage = "Chamber Dip is required.")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
        public float ChamberDipMM { get; set; }
    }
    public class OrderMetadata
    {
        [Required(ErrorMessage = "OrderDate is required")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> OrderDate { get; set; }
        [Required(ErrorMessage = "Product Quantity is required")]
        [RegularExpression(@"^(\d{1,13})$", ErrorMessage = "Only Digits allow")]
        public double OrderQty { get; set; }
        [Required(ErrorMessage = "Customer Name is required")]
        public int CustomerID { get; set; }
        [Required(ErrorMessage = "Product Name is required")]
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Order Code is required")]
        public string OrderCode { get; set; }
        [Required(ErrorMessage = "Carrier Name is required")]
        public Nullable<int> CarrierID { get; set; }
        [Required(ErrorMessage = "Vehicle Code is required")]
        public Nullable<int> VehicleID { get; set; }
    }
    public class ShipmentMetadata
    {
        //[Required(ErrorMessage = "Access Card ID is required")]
        //public Nullable<int> AccessCardID;
        //[Required(ErrorMessage = "Bay ID is required")]
        //public Nullable<int> BayID;
        //[Required(ErrorMessage = "Shipment Date is required")]
        //public Nullable<System.DateTime> ShipmentDate;
    }
}