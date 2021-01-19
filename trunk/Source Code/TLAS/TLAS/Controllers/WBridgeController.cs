using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TLAS.Models;
using PagedList;
using TLAS.ViewModel;

namespace TLAS.Controllers
{
    [Authorize]
    public class WBridgeController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        // GET: WBridge
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder; // added new
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.VehicleSortParm = sortOrder == "Vehicle" ? "Vehicle_desc" : "Vehicle";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewBag.ModifiedDateSortParm = sortOrder == "ModifiedDate" ? "ModifiedDate_desc" : "ModifiedDate";
            ViewBag.ModifiedBySortParm = sortOrder == "ModifiedBy" ? "ModifiedBy_desc" : "ModifiedBy";

            ViewBag.CreatedDateSortParm = sortOrder == "CreatedDate" ? "CreatedDate_desc" : "CreatedDate";
            ViewBag.CreatedBySortParm = sortOrder == "CreatedBy" ? "CreatedBy_desc" : "CreatedBy";

            ViewBag.IsManualSortParm = sortOrder == "IsManual" ? "IsManual_desc" : "IsManual";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;


            var _weighbride = db.WeighBridges
                                     .Include("Shipment")
                                     .Include("Order")
                              .Select(d => new WeighbridgeViewModel()
            {
                OrderID = d.Shipment.OrderID,
                OrderCode = d.Shipment.Order.OrderCode,
                ShipmentID = d.ShipmentID,
                TareWeight = d.TareWeight,
                LoadedWeight = d.LoadedWeight,
                Ismanual = d.Ismanual,
                ActualWeight = d.ActualWeight,
                CreatedDate = d.CreatedDate,
                ModifiedDate = d.ModifiedDate,
                Status = d.Status,
                VehicleCode = d.VehicleCode,
                DriverName = d.Shipment.DriverName,
                CarrierName = d.Shipment.CarrierName,
                AccessCardID = d.Shipment.AccessCardID,
                CustomerName = d.Shipment.Order.Customer.CustomerName,
                OrderQty = d.Shipment.Order.OrderQty,
                Prodtype = d.Shipment.Order.Product.ProductName,
                CreatedBy = d.CreatedBy,
                ModifiedBy = d.ModifiedBy
            });

            if (!String.IsNullOrEmpty(searchString))
            {
                _weighbride = _weighbride.Where(s => s.ShipmentID.ToString().ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "id":
                    _weighbride = _weighbride.OrderBy(s => s.ShipmentID);
                    break;
                case "id_desc":
                    _weighbride = _weighbride.OrderByDescending(s => s.ShipmentID);
                    break;
                case "Vehicle":
                    _weighbride = _weighbride.OrderBy(s => s.VehicleCode);
                    break;
                case "Vehicle_desc":
                    _weighbride = _weighbride.OrderByDescending(s => s.VehicleCode);
                    break;
                case "Status":
                    _weighbride = _weighbride.OrderBy(s => s.Status);
                    break;
                case "Status_desc":
                    _weighbride = _weighbride.OrderByDescending(s => s.Status);
                    break;
                case "ModifiedDate":
                    _weighbride = _weighbride.OrderBy(s => s.ModifiedDate);
                    break;
                case "ModifiedDate_desc":
                    _weighbride = _weighbride.OrderByDescending(s => s.ModifiedDate);
                    break;

                case "CreatedDate":
                    _weighbride = _weighbride.OrderBy(s => s.CreatedDate);
                    break;
                case "CreatedDate_desc":
                    _weighbride = _weighbride.OrderByDescending(s => s.CreatedDate);
                    break;

                case "ModifiedBy":
                    _weighbride = _weighbride.OrderBy(s => s.ModifiedBy);
                    break;
                case "ModifiedBy_desc":
                    _weighbride = _weighbride.OrderByDescending(s => s.ModifiedBy);
                    break;

                case "CreatedBy":
                    _weighbride = _weighbride.OrderBy(s => s.CreatedBy);
                    break;
                case "CreatedBy_desc":
                    _weighbride = _weighbride.OrderByDescending(s => s.CreatedBy);
                    break;

                case "IsManual":
                    _weighbride = _weighbride.OrderBy(s => s.Ismanual);
                    break;
                case "IsManual_desc":
                    _weighbride = _weighbride.OrderByDescending(s => s.Ismanual);
                    break;
                default:
                    _weighbride = _weighbride.OrderByDescending(s => s.ModifiedDate);
                    break;

            }

            int pageSize = 13;
            int pageNumber = (page ?? 1);
            return View(_weighbride.ToPagedList(pageNumber, pageSize));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetAutoComplete(string term)
        {
            List<String> _result;

            _result = db.WeighBridges
                                     .Include("Shipment")
                                     .Include("Order")
                                     .Where(x => x.ShipmentID.ToString().StartsWith(term))
                              .Select(e => e.ShipmentID.ToString()).Distinct().ToList();

            return Json(_result, JsonRequestBehavior.AllowGet);
        }


        //[OutputCache(Duration = 0)] 
        // public ActionResult GetMessages()
        // {
        //     //WBridgeGridRepository _messageRepository = new WBridgeGridRepository();
        //     //return PartialView("_WBGrid", _messageRepository.GetAllMessages());
        // }
    }
}
