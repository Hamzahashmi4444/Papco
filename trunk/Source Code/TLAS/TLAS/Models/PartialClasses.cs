using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TLAS.Models
{
    //[MetadataType(typeof(ProductMetadata))]
    //public partial class Product
    //{
    //}
    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {
    }
    [MetadataType(typeof(CarrierMetadata))]
    public partial class Carrier
    {
    }
    [MetadataType(typeof(DriverMetadata))]
    public partial class Driver
    {
    }
    [MetadataType(typeof(VehicleMetadata))]
    public partial class Vehicle
    {
    }
    [MetadataType(typeof(TrailerMetadata))]
    public partial class Trailer
    {
    }
    [MetadataType(typeof(CompartmentMetadata))]
    public partial class Compartment
    {
    }
    [MetadataType(typeof(OrderMetadata))]
    public partial class Order
    {
    }
    [MetadataType(typeof(ShipmentMetadata))]
    public partial class Shipment
    {
    }
}