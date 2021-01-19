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
using System.Configuration;

namespace TLAS.Controllers
{
    [Authorize]
    public class ShipmentsController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();
        public ActionResult ActiveShipments(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder; // added new
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.OdrCodeSortParm = sortOrder == "OdrCode" ? "OdrCode_desc" : "OdrCode";
            ViewBag.ProductSortParm = sortOrder == "Product" ? "Product_desc" : "Product";
            ViewBag.BaySortParm = sortOrder == "Bay" ? "Bay_desc" : "Bay";
            ViewBag.ShipStatusSortParm = sortOrder == "ShipStatus" ? "ShipStatus_desc" : "ShipStatus";
            ViewBag.WeighStatusSortParm = sortOrder == "WeighStatus" ? "WeighStatus_desc" : "WeighStatus";
            ViewBag.VehCodeSortParm = sortOrder == "VehCode" ? "VehCode_desc" : "VehCode";
           
            ViewBag.ModifiedDateSortParm = sortOrder == "ModifiedDate" ? "ModifiedDate_desc" : "ModifiedDate";
            ViewBag.ModifiedBySortParm = sortOrder == "ModifiedBy" ? "ModifiedBy_desc" : "ModifiedBy";

            ViewBag.CreatedDateSortParm = sortOrder == "CreatedDate" ? "CreatedDate_desc" : "CreatedDate";
            ViewBag.CreatedBySortParm = sortOrder == "CreatedBy" ? "CreatedBy_desc" : "CreatedBy";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;




            var ActiveShipments = from s in db.Shipments.Include(s => s.AccessCard).Include(s => s.Bay).Include(s => s.Order).Include(s => s.ShipmentStatu)
                              .Include(s => s.WeighBridge).Where(s => s.ShipmentStatusID != 5 && s.ShipmentStatusID != 1)
                                  select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                ActiveShipments = ActiveShipments.Where(s => s.Order.OrderCode.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "id":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.ID);
                    break;
                case "id_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.ID);
                    break;
                case "OdrCode":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.Order.OrderCode);
                    break;
                case "OdrCode_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.Order.OrderCode);
                    break;
                case "Product":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.Order.Product.ProductName);
                    break;
                case "Product_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.Order.Product.ProductName);
                    break;
                case "Bay":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.BayID);
                    break;
                case "Bay_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.BayID);
                    break;
                case "ShipStatus":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.ShipmentStatu.Status);
                    break;
                case "ShipStatus_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.ShipmentStatu.Status);
                    break;
                case "WeighStatus":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.WeighBridge.Status);
                    break;
                case "WeighStatus_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.WeighBridge.Status);
                    break;
                case "VehCode":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.VehicleCode);
                    break;
                case "VehCode_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.VehicleCode);
                    break;

                case "ModifiedDate":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.ModifiedDate);
                    break;
                case "ModifiedDate_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.ModifiedDate);
                    break;

                case "CreatedDate":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.CreatedDate);
                    break;
                case "CreatedDate_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.CreatedDate);
                    break;

                case "ModifiedBy":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.ModifiedBy);
                    break;
                case "ModifiedBy_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.ModifiedBy);
                    break;

                case "CreatedBy":
                    ActiveShipments = ActiveShipments.OrderBy(s => s.CreatedBy);
                    break;
                case "CreatedBy_desc":
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.CreatedBy);
                    break;

                default:
                    ActiveShipments = ActiveShipments.OrderByDescending(s => s.ModifiedDate);

                    break;
            }
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridPageSize"].ToString());
            int pageNumber = (page ?? 1);

            Response.AddHeader("Refresh", "10");
            return View(ActiveShipments.ToPagedList(pageNumber, pageSize));
            //return View(customers.ToList());
        }
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            setCurrQueue();
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


            DateTime dateTime =  DateTime.Parse(DateTime.Now.AddDays(-7).ToShortDateString());

            //var vehicle = from s in db.Shipments.Include(s => s.AccessCard).Include(s => s.Bay).Include(s => s.Order).Include(s => s.ShipmentStatu).Include(s => s.WeighBridge)
            //              .Where(x => x.ModifiedDate >= dateTime)
            //              select s;
            var vehicle = from s in db.Shipments.Include(s => s.AccessCard).Include(s => s.Bay).Include(s => s.Order).Include(s => s.ShipmentStatu).Include(s => s.WeighBridge)
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
            List<SelectListItem> cardBayEmpty = new List<SelectListItem>();
            ViewData["BayID"] = cardBayEmpty;
            ViewData["AccessCardID"] = cardBayEmpty;

            return View(vehicle.ToPagedList(pageNumber, pageSize));
            //return View(customers.ToList());
        }
        public ActionResult Home(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder; // added new
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.OdrCodeSortParm = sortOrder == "OdrCode" ? "OdrCode_desc" : "OdrCode";
            ViewBag.CustNameSortParm = sortOrder == "CustName" ? "CustName_desc" : "CustName";
            ViewBag.CarrNameSortParm = sortOrder == "CarrName" ? "CarrName_desc" : "CarrName";
            ViewBag.DrvNameSortParm = sortOrder == "DrvName" ? "DrvName_desc" : "DrvName";
            ViewBag.VehCodeSortParm = sortOrder == "VehCode" ? "VehCode_desc" : "VehCode";

            ViewBag.ModifiedDateSortParm = sortOrder == "ModifiedDate" ? "ModifiedDate_desc" : "ModifiedDate";
            ViewBag.ModifiedBySortParm = sortOrder == "ModifiedBy" ? "ModifiedBy_desc" : "ModifiedBy";

            ViewBag.CreatedDateSortParm = sortOrder == "CreatedDate" ? "CreatedDate_desc" : "CreatedDate";
            ViewBag.CreatedBySortParm = sortOrder == "CreatedBy" ? "CreatedBy_desc" : "CreatedBy";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;




            var closedShipments = from s in db.Shipments.Include(s => s.AccessCard).Include(s => s.Bay).Include(s => s.Order).Include(s => s.ShipmentStatu)
                              .Include(s => s.WeighBridge).Where(s =>s.ShipmentStatusID == 5)
                          select s;
            
            if (!String.IsNullOrEmpty(searchString))
            {

               closedShipments = closedShipments.Where(s => s.Order.OrderCode.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "id":
                    closedShipments = closedShipments.OrderBy(s => s.ID);
                    break;
                case "id_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.ID);
                    break;
                case "OdrCode":
                    closedShipments = closedShipments.OrderBy(s => s.Order.OrderCode);
                    break;
                case "OdrCode_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.Order.OrderCode);
                    break;
                case "CustName":
                    closedShipments = closedShipments.OrderBy(s => s.CustomerName);
                    break;
                case "CustName_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.CustomerName);
                    break;
                case "CarrName":
                    closedShipments = closedShipments.OrderBy(s => s.CarrierName);
                    break;
                case "CarrName_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.CarrierName);
                    break;
                case "DrvName":
                    closedShipments = closedShipments.OrderBy(s => s.DriverName);
                    break;
                case "DrvName_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.DriverName);
                    break;
                case "VehCode":
                    closedShipments = closedShipments.OrderBy(s => s.VehicleCode);
                    break;
                case "VehCode_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.VehicleCode);
                    break;
                case "ModifiedDate":
                    closedShipments = closedShipments.OrderBy(s => s.ModifiedDate);
                    break;
                case "ModifiedDate_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.ModifiedDate);
                    break;

                case "CreatedDate":
                    closedShipments = closedShipments.OrderBy(s => s.CreatedDate);
                    break;
                case "CreatedDate_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.CreatedDate);
                    break;

                case "ModifiedBy":
                    closedShipments = closedShipments.OrderBy(s => s.ModifiedBy);
                    break;
                case "ModifiedBy_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.ModifiedBy);
                    break;

                case "CreatedBy":
                    closedShipments = closedShipments.OrderBy(s => s.CreatedBy);
                    break;
                case "CreatedBy_desc":
                    closedShipments = closedShipments.OrderByDescending(s => s.CreatedBy);
                    break;


                default:
                    closedShipments = closedShipments.OrderByDescending(s => s.ID);

                    break;
            }
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["GridPageSize"].ToString());
            int pageNumber = (page ?? 1);

            return View(closedShipments.ToPagedList(pageNumber, pageSize));
            //return View(customers.ToList());
        }
        public ActionResult ClosedDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var shipmentDetails = from s in db.Shipments.Include(s => s.AccessCard).Include(s => s.Bay).Include(s => s.Order).Include(s => s.ShipmentStatu)
                              .Include(s => s.WeighBridge).Where(s => s.ID == id)
                          select s;
            
            return View(shipmentDetails.ToList());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetAutoCompleteIndex(string term)
        {
            List<String> _result;

            _result = db.Shipments.Where(x => x.Order.OrderCode.ToString().StartsWith(term))
                              .Select(e => e.Order.OrderCode.ToString()).Distinct().ToList();

            return Json(_result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetAutoComplete(string term)
        {
            List<String> _result;

            _result = db.Shipments.Where(x => x.Order.OrderCode.ToString().StartsWith(term) && x.ShipmentStatusID == 5)
                              .Select(e => e.Order.OrderCode.ToString()).Distinct().ToList();

            return Json(_result, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetAutoCompleteActive(string term)
        {
            List<String> _result;

            _result = db.Shipments.Where(x => x.Order.OrderCode.ToString().StartsWith(term) && x.ShipmentStatusID != 5)
                              .Select(e => e.Order.OrderCode.ToString()).Distinct().ToList();

            return Json(_result, JsonRequestBehavior.AllowGet);
        }
        public void bayFrequency(int bayId)
        {
            Bay bay = db.Bays.Where(x => x.BayID == bayId).FirstOrDefault<Bay>();
            bay.Frequency = bay.Frequency + 1;
            db.Bays.Attach(bay);
            db.Entry(bay).Property(x => x.Frequency).IsModified = true;
            db.SaveChanges();
        }
        public void setCurrQueue()
        {
            var queuedShiments = from x in db.Shipments
                                where x.ShipmentStatusID == 2
                                orderby x.BayID
                                group x by x.BayID into grp
                                select new { BayID = grp.Select(y => y.BayID).Distinct().FirstOrDefault().Value, count = grp.Count() };


            var queueCount = (from b in db.Bays
                              select new
                             {
                               ID = b.BayID, Queue = 0
                             }).AsEnumerable().Select(x => new Bay {
                                 BayID = x.ID,
                                 CurrQueue = x.Queue              
                             }).ToList();


            foreach (var ship in queuedShiments)
            {
                queueCount.Where(x => x.BayID == ship.BayID).FirstOrDefault().CurrQueue = ship.count;
            }
            foreach(var bayNo in queueCount)
            {
                Bay bay = db.Bays.Where(x => x.BayID == bayNo.BayID).FirstOrDefault<Bay>();
                bay.ModifiedDate = DateTime.Now;
                bay.CurrQueue = bayNo.CurrQueue;
                db.Bays.Attach(bay);
                db.Entry(bay).Property(x => x.ModifiedDate).IsModified = true;
                db.Entry(bay).Property(x => x.CurrQueue).IsModified = true;
            }
            db.SaveChanges();
                                                                    
        }
        public ActionResult SelectBay(int prdId)
        {
            List<SelectListItem> bayNames = new List<SelectListItem>();
            var bay = from x in db.Bays
                           where x.ProductID == prdId && x.Active == true
                           orderby x.CurrQueue, x.Frequency
                           select x;
                foreach (var item in bay)
                {
                    bayNames.Add(new SelectListItem { Text = item.BayID.ToString(), Value = item.BayID.ToString() });
                }
            return Json(bayNames);
        }
        public ActionResult SelectCard(string bayId)
        {
            int baysId;
            List<SelectListItem> cardNames = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(bayId))
            {
                baysId = Convert.ToInt32(bayId);
                //var access = from s in db.AccessCards.Where(x => x.BayID == baysId && x.Active == true)
                //          select s;
                //foreach (var item in access)
                //{
                //    cardNames.Add(new SelectListItem { Text = item.CardKey.ToString(), Value = item.CardID.ToString() });
                //}
                var access = db.AccessCards.Where(x => x.BayID == baysId && x.Active == true).ToList().OrderBy(o => int.Parse(o.CardKey));
                foreach (var item in access)
                {
                    cardNames.Add(new SelectListItem { Text = item.CardKey.ToString(), Value = item.CardID.ToString() });
                }
            }
            return Json(cardNames);
        }
        public ActionResult cardAssigned(string cardId)
        {
            int cardNumber;
            if (!string.IsNullOrEmpty(cardId))
            {
                cardNumber = Convert.ToInt32(cardId);


                var access = from s in db.AccessCards.Where(x => x.CardID == cardNumber)
                                     select s;
                    if(access.FirstOrDefault().IsAssigned)
                    {
                        var shipment = from s in db.Shipments.Where(x => x.AccessCardID == cardNumber && x.ShipmentStatusID != 5)
                                     select s;
                        return Json(shipment.FirstOrDefault().ID.ToString());
                    }
                        return Json(false);
            }
            return Json(false);
        }
        public ActionResult compartmentPlanning(string vehicleId, int id)
        {
            var shipComp = from s in db.ShipmentCompartments.Where(x => x.ShipmentID == id)
                           select s;
            
            if(shipComp.Count() != 0)
            {
                TempData["CompCountUpdate"] = shipComp.Count();
                List<ListItem> list = new List<ListItem>(); 
                foreach (var item in shipComp)
                {
                    list.Add(new ListItem() { CompIdLI = item.ID.ToString(), CompCodeLI = item.CompartmentCode.ToString(), CapacityLI = item.Capacity.ToString(), OrderedQtyLI = item.OrderedQuantity.ToString(), PlannedQtyLI = item.PlannedQuantity.ToString(), ActualQtyLI = item.ActualBCUQuantity.ToString(),  isCreatedLI = true, PreDip = item.PreDip.ToString(), PostDip = item.PostDip.ToString(), Delta = item.Delta.ToString() });
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
                        return Json(false);
                    }
                    
                }
                return Json(compData);

            }   
        }


        // GET: Shipments/Create
        public ActionResult Create()
        {
            ViewBag.AccessCardID = new SelectList(db.AccessCards, "CardID", "Remarks");
            ViewBag.BayID = new SelectList(db.Bays, "BayID", "Remarks");
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderCode");
            ViewBag.ShipmentStatusID = new SelectList(db.ShipmentStatus, "ID", "Status");
            ViewBag.ID = new SelectList(db.WeighBridges, "ShipmentID", "Status");
            return View();
        }

        // POST: Shipments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,IsManual,AccessCardID,DeletedFlag,ShipmentStatusID,OrderID,BayID,BayName,CustomerName,VehicleCode,DriverName,DriverCNIC,CarrierName,ProductID")] Shipment shipment, FormCollection formCollection)
        {
            #region update bays queue
            #endregion
            #region access card update
            if (shipment.AccessCardID != null && shipment.ShipmentStatusID == 1)
            {
                ///////////////////////////////////////////////////update order status///////////////////////////////////////////////////
                Order odr = db.Orders.Where(x => x.OrderID == shipment.OrderID).FirstOrDefault<Order>();
                odr.OrderStatusID = 3;
                odr.ModifiedDate = DateTime.Now;
                odr.ModifiedBy = User.Identity.Name;
                db.Orders.Attach(odr);
                db.Entry(odr).Property(x => x.ModifiedDate).IsModified = true;
                db.Entry(odr).Property(x => x.ModifiedBy).IsModified = true;
                db.Entry(odr).Property(x => x.OrderStatusID).IsModified = true;
                db.SaveChanges();
                /////////////////////////////////////////////////assign access card///////////////////////////////////////////////
                AccessCard accessCard = db.AccessCards.Where(x => x.CardID == shipment.AccessCardID).FirstOrDefault<AccessCard>();
                accessCard.IsAssigned = true;
                accessCard.ModifiedDate = DateTime.Now;
                accessCard.ModifiedBy = User.Identity.Name;
                db.AccessCards.Attach(accessCard);
                db.Entry(accessCard).Property(x => x.ModifiedDate).IsModified = true;
                db.Entry(accessCard).Property(x => x.ModifiedBy).IsModified = true;
                db.Entry(accessCard).Property(x => x.IsAssigned).IsModified = true;
                db.Entry(accessCard).State = EntityState.Modified;
                db.SaveChanges();
                //db.Entry(accessCard).State = EntityState.Detached;

            }
            else if (shipment.AccessCardID != null && shipment.ShipmentStatusID != 1 && shipment.ShipmentStatusID != 5)
            {                
                ////////////////////////////////////unassign card if any////////////////////////
                int? expectedID = db.Shipments.AsNoTracking().Where(x => x.ID == shipment.ID).FirstOrDefault().AccessCardID;
                if(expectedID != null && expectedID != shipment.AccessCardID)
                {
                    /////////////////////////////////////////access card old///////////////////////
                    AccessCard accessCardOld = db.AccessCards.Where(x => x.CardID == expectedID).FirstOrDefault<AccessCard>();
                    accessCardOld.IsAssigned = false;
                    accessCardOld.ModifiedDate = DateTime.Now;
                    accessCardOld.ModifiedBy = User.Identity.Name;
                    db.AccessCards.Attach(accessCardOld);
                    db.Entry(accessCardOld).Property(x => x.ModifiedDate).IsModified = true;
                    db.Entry(accessCardOld).Property(x => x.ModifiedBy).IsModified = true;
                    db.Entry(accessCardOld).Property(x => x.IsAssigned).IsModified = true;
                    //////////////////////////access card New///////////////////////////////////////
                    AccessCard accessCardNew = db.AccessCards.Where(x => x.CardID == shipment.AccessCardID).FirstOrDefault<AccessCard>();
                    accessCardNew.CardID = (int)shipment.AccessCardID;
                    accessCardNew.IsAssigned = true;
                    accessCardNew.ModifiedDate = DateTime.Now;
                    accessCardNew.ModifiedBy = User.Identity.Name;
                    db.AccessCards.Attach(accessCardNew);
                    db.Entry(accessCardNew).Property(x => x.ModifiedDate).IsModified = true;
                    db.Entry(accessCardNew).Property(x => x.ModifiedBy).IsModified = true;
                    db.Entry(accessCardNew).Property(x => x.IsAssigned).IsModified = true;
                    ///////////////////////////finally save data/////////////////////////////////////

                    db.Entry(accessCardOld).State = EntityState.Modified;
                    db.Entry(accessCardNew).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    ///////////////////////////////////////////////////update order status///////////////////////////////////////////////////
                    Order odr = db.Orders.Where(x => x.OrderID == shipment.OrderID).FirstOrDefault<Order>();
                    odr.OrderStatusID = 3;
                    odr.ModifiedDate = DateTime.Now;
                    odr.ModifiedBy = User.Identity.Name;
                    db.Orders.Attach(odr);
                    db.Entry(odr).Property(x => x.ModifiedDate).IsModified = true;
                    db.Entry(odr).Property(x => x.ModifiedBy).IsModified = true;
                    db.Entry(odr).Property(x => x.OrderStatusID).IsModified = true;
                    db.SaveChanges();
                    ////////////////////////////////////////////////assign Access card///////////////////////////////////////////////////////
                    AccessCard accessCard = db.AccessCards.Where(x => x.CardID == shipment.AccessCardID).FirstOrDefault<AccessCard>();
                    accessCard.CardID = (int)shipment.AccessCardID;
                    accessCard.IsAssigned = true;
                    accessCard.ModifiedDate = DateTime.Now;
                    accessCard.ModifiedBy = User.Identity.Name;
                    db.AccessCards.Attach(accessCard);
                    db.Entry(accessCard).Property(x => x.ModifiedDate).IsModified = true;
                    db.Entry(accessCard).Property(x => x.ModifiedBy).IsModified = true;
                    db.Entry(accessCard).Property(x => x.IsAssigned).IsModified = true;
                    db.Entry(accessCard).State = EntityState.Modified; //commneted by ahad for performance 
                    db.SaveChanges();
                }
            }
            else if (shipment.ShipmentStatusID == 5)
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
                //////////////////////////////
                int bayidfrmDb = db.Shipments.AsNoTracking().Where(x => x.ID == shipment.ID).FirstOrDefault().BayID.Value;
                bayFrequency(bayidfrmDb);

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                int accessCardidfrmDb = db.Shipments.AsNoTracking().Where(x => x.ID == shipment.ID).FirstOrDefault().AccessCardID.Value;
                AccessCard accessCard = db.AccessCards.Where(x => x.CardID == accessCardidfrmDb).FirstOrDefault<AccessCard>();
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
            #endregion 
            #region shipment compartment
            int compCountTempDataUpdate = Convert.ToInt32(TempData["CompCountUpdate"].ToString());
            if (compCountTempDataUpdate != 0)
            {
                for (int i = 0; i < compCountTempDataUpdate; i++)
                {
                    int compIDConverted = Convert.ToInt32(formCollection["compId" + i]);
                    ShipmentCompartment shipmentComp = db.ShipmentCompartments.Where(x => x.ID == compIDConverted).FirstOrDefault<ShipmentCompartment>();
                    //shipmentComp.ID = Convert.ToInt32(formCollection["compId" + i]);
                    shipmentComp.OrderedQuantity = Convert.ToInt32(formCollection["Order.OrderQty"]);
                    int compPlannedQty;
                    if (Int32.TryParse(formCollection["compPlannedQty" + i], out compPlannedQty))
                    {
                        shipmentComp.PlannedQuantity = compPlannedQty;
                    }
                    else
                    {
                        shipmentComp.PlannedQuantity = null;
                    }
                    if (shipment.ShipmentStatusID == 3 || shipment.ShipmentStatusID == 5) // Either Close or checkedIn
                    {
                        int compManualPreDip;
                        if (Int32.TryParse(formCollection["compManualPreDip" + i], out compManualPreDip))
                        {
                            shipmentComp.PreDip = compManualPreDip;
                        }
                        else
                        {
                            shipmentComp.PreDip = null;
                        }

                        int compManualPostDip;
                        if (Int32.TryParse(formCollection["compManualPostDip" + i], out compManualPostDip))
                        {
                            shipmentComp.PostDip = compManualPostDip;
                        }
                        else
                        {
                            shipmentComp.PostDip = null;
                        }

                        int compDelta;
                        if (Int32.TryParse(formCollection["compDelta" + i], out compDelta))
                        {
                            shipmentComp.Delta = compDelta;
                        }
                        else
                        {
                            shipmentComp.Delta = null;
                        }

                       
                    }

                    shipmentComp.AccessCardKey = shipment.AccessCardID.ToString();
                    shipmentComp.BayID = Convert.ToInt32(shipment.BayID);
                    shipmentComp.Product = shipment.ProductID.ToString();
                    shipmentComp.ShipmentID = shipment.ID;
                    //shipmentComp.CreatedDate = DateTime.Now;
                    shipmentComp.ModifiedDate = DateTime.Now;
                    shipmentComp.ModifiedBy = User.Identity.Name;
                    shipmentComp.CompartmentCode = Convert.ToInt32(formCollection["compCode" + i]);
                    shipmentComp.Capacity = Convert.ToInt32(formCollection["compCapacity" + i]);
                    db.ShipmentCompartments.Attach(shipmentComp);
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
                        if (shipment.ShipmentStatusID != 4)
                        {
                            shipment.ShipmentStatusID = 2; // shipment status id is set to queued.
                        }
                        
                        ShipmentCompartment shipmentComp = new ShipmentCompartment();
                        shipmentComp.OrderedQuantity = Convert.ToInt32(formCollection["Order.OrderQty"]);
                        int compPlannedQty;
                        if(Int32.TryParse(formCollection["compPlannedQty" + i], out compPlannedQty))
                        {
                            shipmentComp.PlannedQuantity = compPlannedQty;
                        }
                        else
                        {
                            shipmentComp.PlannedQuantity = null;
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
                    catch(Exception e)
                    {
                        e.ToString();
                    }
                }

            }
            #endregion

            if (ModelState.IsValid)
            {
                //Shipment ship = db.Shipments.Where(x => x.ID == shipment.ID).FirstOrDefault<Shipment>();
                shipment.ShipmentDate = DateTime.Now;
                shipment.ModifiedDate = DateTime.Now;
                shipment.ModifiedBy = User.Identity.Name;
                db.Entry(shipment).State = EntityState.Modified;
                db.Entry(shipment).Property(x => x.CreatedDate).IsModified = false;
                db.Entry(shipment).Property(x => x.CreatedBy).IsModified = false;
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
        

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shipment shipment = db.Shipments.Find(id);
            db.Shipments.Remove(shipment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult WeighOutValue(string ShipmentID)
        {
            Int32 ShipID = Convert.ToInt32(ShipmentID);
            var ActualWeightValue = (from s in db.WeighBridges.Where(x => x.ShipmentID == ShipID)
                                     select s).FirstOrDefault();


            string value = string.Empty;
            if (ActualWeightValue != null)
            {
                if (!string.IsNullOrEmpty(ActualWeightValue.ActualWeight.ToString()))
                    value = ActualWeightValue.ActualWeight.ToString();
            }
            return this.Json(value);


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
