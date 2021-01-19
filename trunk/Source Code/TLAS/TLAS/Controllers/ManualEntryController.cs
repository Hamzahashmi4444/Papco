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

namespace TLAS.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    public class ManualEntryController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //if (TempData["CustomError"] != null)
            //{
            //    ModelState.AddModelError("", TempData["CustomError"].ToString());
            //}
            // ViewData["compartments"] = db.Compartments.ToList();
            ViewBag.CurrentSort = sortOrder; // added new
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.VehCodeSortParm = sortOrder == "VehCode" ? "VehCode_desc" : "VehCode";
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name_desc" : "Name";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;



            DateTime dateTime = DateTime.Parse(DateTime.Now.AddDays(7).ToShortDateString());
            var vehicle = from s in db.Shipments.Include(s => s.AccessCard).Include(s => s.Bay).Include(s => s.Order)
                              .Include(s => s.ShipmentStatu).Include(s => s.WeighBridge).Where(s => s.IsManual == true).Where(x => x.ModifiedDate <= dateTime)
                          select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                vehicle = vehicle.Where(s => s.Order.OrderCode.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "id":
                    vehicle = vehicle.OrderBy(s => s.ID);
                    break;
                case "id_desc":
                    vehicle = vehicle.OrderByDescending(s => s.ID);
                    break;
                case "Name":
                    vehicle = vehicle.OrderBy(s => s.Order.OrderCode);
                    break;
                case "Name_desc":
                    vehicle = vehicle.OrderByDescending(s => s.Order.OrderCode);
                    break;
                case "VehCode":
                    vehicle = vehicle.OrderBy(s => s.VehicleCode);
                    break;
                case "VehCode_desc":
                    vehicle = vehicle.OrderByDescending(s => s.VehicleCode);
                    break;
                default:
                    vehicle = vehicle.OrderByDescending(s => s.ModifiedDate);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(vehicle.ToPagedList(pageNumber, pageSize));
            //return View(customers.ToList());
        }
        public JsonResult GetAutoComplete(string term)
        {
            List<String> _result;

            _result = db.Shipments.Where(x => x.Order.OrderCode.ToString().StartsWith(term) && (x.ShipmentStatusID == 4 || x.ShipmentStatusID == 5))
                              .Select(e => e.Order.OrderCode.ToString()).Distinct().ToList();

            return Json(_result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult compartmentPlanning(string vehicleId, int id)
        {
            var shipComp = from s in db.ShipmentCompartments.Where(x => x.ShipmentID == id)
                           select s;

            if (shipComp.Count() != 0)
            {
                TempData["CompCountUpdate"] = shipComp.Count();
                List<ListItem> list = new List<ListItem>();
                foreach (var item in shipComp)
                {
                    list.Add(new ListItem() { CompIdLI = item.ID.ToString(), CompCodeLI = item.CompartmentCode.ToString(), CapacityLI = item.Capacity.ToString(), OrderedQtyLI = item.OrderedQuantity.ToString(), PlannedQtyLI = item.PlannedQuantity.ToString(), ActualQtyLI = item.ActualBCUQuantity.ToString(), PreDip = item.PreDip.ToString(), PostDip = item.PostDip.ToString(), Delta = item.Delta.ToString(), isCreatedLI = true });
                }

                return this.Json(list);

            }
            else
            {
                string vehCode;
                List<SelectListItem> compData = new List<SelectListItem>();
                if (!string.IsNullOrEmpty(vehicleId))
                {
                    vehCode = vehicleId;
                    try
                    {
                        var traiId = from s in db.Trailers.Where(x => x.Vehicle.VehicleCode == vehCode).FirstOrDefault().Compartments
                                     select s;
                        TempData["CompCountNew"] = traiId.Count();
                        TempData["CompCountUpdate"] = 0;
                        foreach (var item in traiId)
                        {
                            compData.Add(new SelectListItem { Text = item.CompartmentCode.ToString(), Value = item.Capactiy.ToString() });
                        }
                    }
                    catch
                    {
                        TempData["CompCountNew"] = 0;
                        TempData["CompCountUpdate"] = 0;
                        compData = null;
                    }

                }
                return Json(compData);

            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,AccessCardID,DeletedFlag,ShipmentStatusID,OrderID,BayID,BayName,CustomerName,VehicleCode,DriverName,DriverCNIC,CarrierName,ShipmentDate,ProductID,RemarksManual")] Shipment shipment, FormCollection formCollection)
        {
            #region shipment compartment
            int compCountTempDataUpdate = Convert.ToInt32(TempData["CompCountUpdate"].ToString());
            if (compCountTempDataUpdate != 0)
            {
                for (int i = 0; i < compCountTempDataUpdate; i++)
                {
                    int compID = Convert.ToInt32(formCollection["compId" + i]);
                    ShipmentCompartment shipmentComp = db.ShipmentCompartments.AsNoTracking().Where(x => x.ID == compID).FirstOrDefault<ShipmentCompartment>();
                    //shipmentComp.ID = Convert.ToInt32(formCollection["compId" + i]);
                    shipmentComp.OrderedQuantity = Convert.ToInt32(formCollection["Order.OrderQty"]);
                    int compActualQty;
                    if (Int32.TryParse(formCollection["compActualQty" + i], out compActualQty))
                    {
                        shipmentComp.ActualBCUQuantity = compActualQty;
                    }
                    else
                    {
                        shipmentComp.ActualBCUQuantity = null;
                    }
                
                    int preDip;
                    if (Int32.TryParse(formCollection["compManualPreDip" + i], out preDip))
                        shipmentComp.PreDip = preDip;
                    else
                        shipmentComp.PreDip = null;
                    int postDip;
                    if (Int32.TryParse(formCollection["compManualPostDip" + i], out postDip))
                        shipmentComp.PostDip = postDip;
                    else
                        shipmentComp.PostDip = null;
                    int compDelta;
                    if (Int32.TryParse(formCollection["compDelta" + i], out compDelta))
                    {
                        shipmentComp.Delta = compDelta;
                    }
                    else
                    {
                        shipmentComp.Delta = null;
                    }
                    //if (Int32.TryParse(formCollection["compDelta" + i], out Delta))
                    //    shipmentComp.Delta = Delta;
                    //else
                    //    shipmentComp.Delta = null;
                    //shipmentComp.AccessCardKey = shipment.AccessCardID.ToString();
                    //shipmentComp.BayID = Convert.ToInt32(shipment.BayID);
                    shipmentComp.Product = shipment.ProductID.ToString();
                    shipmentComp.ShipmentID = shipment.ID;
                    //shipmentComp.CreatedDate = DateTime.Now;
                    shipmentComp.ModifiedDate = DateTime.Now;
                    shipmentComp.ModifiedBy = User.Identity.Name;
                    shipmentComp.CompartmentCode = Convert.ToInt32(formCollection["compCode" + i]);
                    shipmentComp.Capacity = Convert.ToInt32(formCollection["compCapacity" + i]);
                    db.ShipmentCompartments.Attach(shipmentComp);
                    db.Entry(shipmentComp).Property(x => x.OrderedQuantity).IsModified = true;
                    db.Entry(shipmentComp).Property(x => x.ActualBCUQuantity).IsModified = true;
                   
                    db.Entry(shipmentComp).Property(x => x.Product).IsModified = true;
                    db.Entry(shipmentComp).Property(x => x.ShipmentID).IsModified = true;
                    db.Entry(shipmentComp).Property(x => x.ModifiedDate).IsModified = true;
                    db.Entry(shipmentComp).Property(x => x.ModifiedBy).IsModified = true;
                    db.Entry(shipmentComp).Property(x => x.CompartmentCode).IsModified = true;
                    db.Entry(shipmentComp).Property(x => x.Capacity).IsModified = true;
                    db.Entry(shipmentComp).State = EntityState.Modified;
                    db.SaveChanges();
                    //db.Entry(shipmentComp).State = EntityState.Detached;
                }
            }
            else
            {
                int compCountTempDataNew = Convert.ToInt32(TempData["CompCountNew"].ToString());
                for (int i = 0; i < compCountTempDataNew; i++)
                {
                    try
                    {
                        //shipment.ShipmentStatusID = 2; // shipment status id is set to queued.
                        ShipmentCompartment shipmentComp = new ShipmentCompartment();
                        shipmentComp.OrderedQuantity = Convert.ToInt32(formCollection["Order.OrderQty"]);
                        int compActualQty;
                        if (Int32.TryParse(formCollection["compActualQty" + i], out compActualQty))
                        {
                            shipmentComp.ActualBCUQuantity = compActualQty;
                        }
                        else
                        {
                            shipmentComp.ActualBCUQuantity = null;
                        }
                   

                        shipmentComp.AccessCardKey = shipment.AccessCardID.ToString();
                        shipmentComp.BayID = Convert.ToInt32(shipment.BayID);
                        shipmentComp.Product = shipment.ProductID.ToString();
                        shipmentComp.ShipmentID = shipment.ID;
                        shipmentComp.CreatedDate = DateTime.Now;
                        shipmentComp.CreatedBy = User.Identity.Name;
                        shipmentComp.ModifiedDate = DateTime.Now;
                        shipmentComp.ModifiedBy = User.Identity.Name;
                        shipmentComp.CompartmentCode = Convert.ToInt32(formCollection["compCode" + i]);
                        shipmentComp.Capacity = Convert.ToInt32(formCollection["compCapacity" + i]);
                        db.ShipmentCompartments.Add(shipmentComp);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                }

            }
            #endregion
            if (shipment.ShipmentStatusID == 5)
            {
                Order odr = db.Orders.Where(x => x.OrderID == shipment.OrderID).FirstOrDefault<Order>();
                odr.OrderStatusID = 3;
                odr.OrderDeliveryDT = DateTime.Now;
                odr.ModifiedDate = DateTime.Now;
                odr.ModifiedBy = User.Identity.Name;
                db.Orders.Attach(odr);
                db.Entry(odr).Property(x => x.ModifiedDate).IsModified = true;
                db.Entry(odr).Property(x => x.ModifiedBy).IsModified = true;
                db.Entry(odr).Property(x => x.OrderStatusID).IsModified = true;
                db.Entry(odr).Property(x => x.OrderDeliveryDT).IsModified = true;
                db.SaveChanges();

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                AccessCard accessCard = db.AccessCards.Where(x => x.CardID == shipment.AccessCardID).FirstOrDefault<AccessCard>();
                accessCard.CardID = (int)shipment.AccessCardID;
                accessCard.IsAssigned = false;
                accessCard.ModifiedDate = DateTime.Now;
                accessCard.ModifiedBy = User.Identity.Name;
                db.AccessCards.Attach(accessCard);
                db.Entry(accessCard).Property(x => x.ModifiedDate).IsModified = true;
                db.Entry(accessCard).Property(x => x.ModifiedBy).IsModified = true;
                db.Entry(accessCard).Property(x => x.IsAssigned).IsModified = true;
                db.Entry(accessCard).State = EntityState.Modified; //commneted by ahad for performance 
                db.SaveChanges();
                //db.Entry(accessCard).State = EntityState.Detached;
            }
            if (ModelState.IsValid)
            {
                shipment.ModifiedDate = DateTime.Now;
                shipment.ModifiedBy = User.Identity.Name;
                shipment.RemarksManual = shipment.RemarksManual.TrimEnd();
                db.Shipments.Attach(shipment);
                db.Entry(shipment).Property(x => x.ModifiedDate).IsModified = true;
                db.Entry(shipment).Property(x => x.ModifiedBy).IsModified = true;
                db.Entry(shipment).Property(x => x.ShipmentStatusID).IsModified = true;
                db.Entry(shipment).Property(x => x.RemarksManual).IsModified = true;
                //db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccessCardID = new SelectList(db.AccessCards, "CardID", "Remarks", shipment.AccessCardID);
            ViewBag.BayID = new SelectList(db.Bays, "BayID", "Remarks", shipment.BayID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderCode", shipment.OrderID);
            ViewBag.ShipmentStatusID = new SelectList(db.ShipmentStatus, "ID", "Status", shipment.ShipmentStatusID);
            ViewBag.ID = new SelectList(db.WeighBridges, "ShipmentID", "Status", shipment.ID);
            return View(shipment);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult WeighOutValueCheck(string ShipmentID)
        {
            Int32 ShipID = Convert.ToInt32(ShipmentID);
            var LoadedWeightValue = from s in db.WeighBridges.Where(x => x.ShipmentID == ShipID)
                                    select s.LoadedWeight;

            return Json(LoadedWeightValue.FirstOrDefault().HasValue);
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
