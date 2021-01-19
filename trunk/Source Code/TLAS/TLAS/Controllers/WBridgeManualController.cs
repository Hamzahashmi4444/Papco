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
    [Authorize(Roles = "Admin,Supervisor")]
    public class WBridgeManualController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        // GET: WBridgeManual


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


            var ordercode = from o in db.Shipments
                            join p in db.Products on o.ProductID equals p.ProductID
                            where (o.ShipmentStatusID == 2 || o.ShipmentStatusID == 3 || o.ShipmentStatusID == 4)
                             && p.ProductName == "LPG"

                            select o;



            List<SelectListItem> listorders = new List<SelectListItem>();
            foreach (var order in ordercode)
            {
                listorders.Add(new SelectListItem { Text = order.Order.OrderCode.ToString(), Value = order.Order.OrderCode.ToString() });

            }

            ViewData["OrderCode"] = listorders;


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
                               CreatedBy=d.CreatedBy,
                               ModifiedBy=d.ModifiedBy

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

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(_weighbride.ToPagedList(pageNumber, pageSize));

        }

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

        public ActionResult GetAllOrderCode()
        {

            //var ordercode = from s in db.Shipments
            //                join p in db.Products on s.ProductID equals p.ProductID
            //                join b in db.WeighBridges on s.ID equals b.ShipmentID
            //                where (s.ShipmentStatusID == 2 || s.ShipmentStatusID == 3 || s.ShipmentStatusID == 4)
            //                 && p.ProductName == "LPG" &&
            //                    (b.TareWeight == null || b.LoadedWeight == null)

            //                select s;
            //if (ordercode.Count() == 0)
            //{
            var ordercode = from s in db.Shipments
                            join p in db.Products on s.ProductID equals p.ProductID
                            where (s.ShipmentStatusID == 2 || s.ShipmentStatusID == 3 || s.ShipmentStatusID == 4)
                         && p.ProductName == "LPG"
                            select s;
            //}


            List<SelectListItem> listorders = new List<SelectListItem>();
            int i = 0;
            foreach (var order in ordercode)
            {
                listorders.Add(new SelectListItem { Text = i++.ToString(), Value = order.Order.OrderCode.ToString() });

            }

            return Json(listorders);
        }

        public ActionResult GETShipdataByOrderID(string ordercode)
        {

            List<SelectListItem> ListShipment = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(ordercode))
            {


                var _shipments = from a in db.Orders
                                 join u in db.Shipments on a.OrderID equals u.OrderID
                                 where a.OrderCode == ordercode
                                 select new
                                 {
                                     u.CarrierName,
                                     u.DriverName,
                                     u.VehicleCode,
                                     u.AccessCardID,
                                     u.ID

                                 };

                foreach (var item in _shipments)
                {

                    if (item.CarrierName != null && !string.IsNullOrEmpty(item.CarrierName.ToString()))
                        ListShipment.Add(new SelectListItem { Text = item.CarrierName, Value = item.CarrierName.ToString() });

                    if (item.DriverName != null && !string.IsNullOrEmpty(item.DriverName.ToString()))
                        ListShipment.Add(new SelectListItem { Text = item.DriverName, Value = item.DriverName.ToString() });

                    if (item.VehicleCode != null && !string.IsNullOrEmpty(item.VehicleCode.ToString()))
                        ListShipment.Add(new SelectListItem { Text = item.VehicleCode, Value = item.VehicleCode.ToString() });

                    if (item.AccessCardID != null && !string.IsNullOrEmpty(item.AccessCardID.ToString()))
                        ListShipment.Add(new SelectListItem { Text = item.AccessCardID.ToString(), Value = item.AccessCardID.ToString() });

                    if (!string.IsNullOrEmpty(item.ID.ToString()))
                        ListShipment.Add(new SelectListItem { Text = item.ID.ToString(), Value = item.ID.ToString() });

                }
            }

            return Json(ListShipment);
        }

        public ActionResult GetWBEntryByShipID(string Shipid)
        {

            List<SelectListItem> ListWB = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(Shipid))
            {

                int _shipid = Convert.ToInt32(Shipid);

                var _Wblist = from w in db.WeighBridges
                              join s in db.Shipments on w.ShipmentID equals s.ID
                              where s.ID == _shipid
                              select new
                              {
                                  w.TareWeight,
                                  w.LoadedWeight,
                                  w.Status,
                                  w.ActualWeight
                              };

                foreach (var item in _Wblist)
                {

                    if (item.TareWeight != null && !string.IsNullOrEmpty(item.TareWeight.ToString()))
                        ListWB.Add(new SelectListItem { Text = "_TW", Value = item.TareWeight.ToString() });
                    else
                    {
                        ListWB.Add(new SelectListItem { Text = "_TW", Value = "" });
                    }

                    if (item.LoadedWeight != null && !string.IsNullOrEmpty(item.LoadedWeight.ToString()))
                        ListWB.Add(new SelectListItem { Text = "_LW", Value = item.LoadedWeight.ToString() });
                    else
                    {
                        ListWB.Add(new SelectListItem { Text = "_LW", Value = "" });
                    }

                    if (item.Status != null && !string.IsNullOrEmpty(item.Status.ToString()))
                        ListWB.Add(new SelectListItem { Text = "_S", Value = item.Status.ToString() });
                    else
                    {
                        ListWB.Add(new SelectListItem { Text = "_S", Value = "" });
                    }

                    if (item.ActualWeight != null && !string.IsNullOrEmpty(item.ActualWeight.ToString()))
                        ListWB.Add(new SelectListItem { Text = "_AW", Value = item.ActualWeight.ToString() });
                    else
                    {
                        ListWB.Add(new SelectListItem { Text = "_AW", Value = "" });
                    }
                }
            }

            return Json(ListWB);
        }

        // POST: WBridgeManual/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //   [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TareWeight,LoadedWeight,Status,ActualWeight,ModifiedDate,ShipmentID,CreatedDate,VehicleCode,Ismanual")] WeighBridge weighBridge)
        {


            if (ModelState.IsValid)
            {
                WeighBridge _weighBridge = db.WeighBridges.Find(weighBridge.ShipmentID);
                Shipment _Ship = db.Shipments.Find(weighBridge.ShipmentID);
                if (_Ship.ShipmentStatusID == 2)
                {
                    _Ship.ShipmentStatusID = 3;
                    db.Entry(_Ship).State = EntityState.Modified;
                }
                if (_Ship.ShipmentStatusID == 5)
                {

                    return RedirectToAction("Index");
                }

                WeighBridge WB = new WeighBridge();

                if (_weighBridge != null)
                {
                    _weighBridge.Status = "Manual";
                    _weighBridge.LoadedWeight = weighBridge.LoadedWeight;
                    _weighBridge.TareWeight = weighBridge.TareWeight;
                    _weighBridge.ActualWeight = weighBridge.ActualWeight;
                    _weighBridge.Ismanual = true;
                    _weighBridge.VehicleCode = weighBridge.VehicleCode;
                    _weighBridge.ModifiedDate = DateTime.Now;
                    _weighBridge.ModifiedBy = User.Identity.Name;

                    if (ModelState.IsValid)
                    {
                        db.Entry(_weighBridge).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                else
                {

                    WB.Status = "Manual";
                    WB.LoadedWeight = weighBridge.LoadedWeight;
                    WB.TareWeight = weighBridge.TareWeight;
                    WB.ActualWeight = weighBridge.ActualWeight;
                    WB.Ismanual = true;
                    WB.VehicleCode = weighBridge.VehicleCode;
                    WB.ShipmentID = weighBridge.ShipmentID;
                    WB.CreatedDate = DateTime.Now;
                    WB.CreatedBy = User.Identity.Name;
                    WB.ModifiedDate = DateTime.Now;
                    WB.ModifiedBy = User.Identity.Name;
                    db.WeighBridges.Add(WB);

                    db.SaveChanges();

                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
