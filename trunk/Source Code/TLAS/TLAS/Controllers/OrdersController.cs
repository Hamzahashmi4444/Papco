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
using System.Data.Entity.Infrastructure;

namespace TLAS.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (TempData["CustomError"] != null)
            {
                ModelState.AddModelError("", TempData["CustomError"].ToString());
            }
            if (TempData["ErrorMessage"] != null)
            {
                ModelState.AddModelError("", TempData["ErrorMessage"].ToString());
            }
            ViewBag.CurrentSort = sortOrder; // added new
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.CustNameSortParm = sortOrder == "CustName" ? "CustName_desc" : "CustName";

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

            var orders = from s in db.Orders.Include(o => o.Carrier).Include(o => o.Customer).Include(o => o.Product)
                             .Include(o => o.OrderStatu).Include(o => o.Vehicle).Where(o => o.OrderStatusID != 3)
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(s => s.OrderCode == searchString);
            }
            switch (sortOrder)
            {
                case "id":
                    orders = orders.OrderBy(s => s.OrderID);
                    break;
                case "id_desc":
                    orders = orders.OrderByDescending(s => s.OrderID);
                    break;
                case "Name":
                    orders = orders.OrderBy(s => s.OrderCode);
                    break;
                case "Name_desc":
                    orders = orders.OrderByDescending(s => s.OrderCode);
                    break;
                case "CustName":
                    orders = orders.OrderBy(s => s.Customer.CustomerName);
                    break;
                case "CustName_desc":
                    orders = orders.OrderByDescending(s => s.Customer.CustomerName);
                    break;

                case "ModifiedDate":
                    orders = orders.OrderBy(s => s.ModifiedDate);
                    break;
                case "ModifiedDate_desc":
                    orders = orders.OrderByDescending(s => s.ModifiedDate);
                    break;

                case "CreatedDate":
                    orders = orders.OrderBy(s => s.CreatedDate);
                    break;
                case "CreatedDate_desc":
                    orders = orders.OrderByDescending(s => s.CreatedDate);
                    break;

                case "ModifiedBy":
                    orders = orders.OrderBy(s => s.ModifiedBy);
                    break;
                case "ModifiedBy_desc":
                    orders = orders.OrderByDescending(s => s.ModifiedBy);
                    break;
                case "CreatedBy":
                    orders = orders.OrderBy(s => s.CreatedBy);
                    break;
                case "CreatedBy_desc":
                    orders = orders.OrderByDescending(s => s.CreatedBy);
                    break;

                default:
                    orders = orders.OrderByDescending(s => s.ModifiedDate);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            SelectProductType();
            SelectCustomer();
            SelectCarrier();


            try
            {
                SelectVehicleIndex(orders.FirstOrDefault().CarrierID);
            }
            catch
            {
                List<SelectListItem> vehicleData = new List<SelectListItem>();
                ViewData["VehicleID"] = vehicleData;
            }
            return View(orders.ToPagedList(pageNumber, pageSize));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetAutoComplete(string term)
        {
            List<String> _result;

            _result = db.Orders.Where(x => x.OrderCode.ToString().StartsWith(term) && x.OrderStatusID != 3)
                              .Select(e => e.OrderCode.ToString()).Distinct().ToList();

            return Json(_result, JsonRequestBehavior.AllowGet);
        }
        public void SelectProductType()
        {
            var product = from s in db.Products.OrderBy(s => s.ProductName).Where(s => s.Active == true)
                          select s;
            List<SelectListItem> pro = new List<SelectListItem>();
            foreach (var item in product)
            {
                if (item.ProductID == 1)
                {
                    pro.Add(new SelectListItem { Text = "Mogas(Ltr)", Value = item.ProductID.ToString() });
                }
                else
                {
                    pro.Add(new SelectListItem { Text = "HSD(Ltr)", Value = item.ProductID.ToString() });
                }

            }
            //if(pro.Find(id).Value != null)
            //{

            //}
            //pro.FirstOrDefault().Selected = true;
            ViewData["ProductID"] = pro;

        }
        public ActionResult SelectCustomer()
        {
            var customers = from s in db.Customers.OrderBy(s => s.CustomerName)
                            select s;

            List<SelectListItem> custName = new List<SelectListItem>();
            foreach (var item in customers)
            {
                custName.Add(new SelectListItem { Text = item.CustomerName, Value = item.CustomerID.ToString() });
            }

            ViewData["CustomerID"] = custName;
            return View();
        }
        public ActionResult SelectCarrier()
        {
            var Carriers = from s in db.Carriers.OrderBy(s => s.CarrierName).Where(s => s.Active == true)
                           select s;

            List<SelectListItem> carrName = new List<SelectListItem>();
            foreach (var item in Carriers)
            {
                carrName.Add(new SelectListItem { Text = item.CarrierName, Value = item.CarrierID.ToString() });
            }

            ViewData["CarrierID"] = carrName;
            return View();
        }
        public ActionResult SelectVehicleIndex(int? carriId)
        {
            List<SelectListItem> vehicleNames = new List<SelectListItem>();
            if (carriId != null)
            {
                var vehicles = from s in db.Vehicles.Where(x => x.CarrierID == carriId && x.Active == true && x.LicenseEDate > DateTime.Now)
                               select s;
                foreach (var item in vehicles)
                {
                    vehicleNames.Add(new SelectListItem { Text = item.VehicleCode, Value = item.VehicleID.ToString() });
                }
            }
            ViewData["VehicleID"] = vehicleNames;
            return View();
        }
        public ActionResult SelectVehicle(string carriId)
        {
            int carrId;
            List<SelectListItem> vehicleNames = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(carriId))
            {
                carrId = Convert.ToInt32(carriId);
                var drivers = from s in db.Vehicles.Where(x => x.CarrierID == carrId && x.Active == true && x.LicenseEDate > DateTime.Now)
                              select s;
                foreach (var item in drivers)
                {
                    vehicleNames.Add(new SelectListItem { Text = item.VehicleCode, Value = item.VehicleID.ToString() });
                }
            }
            return Json(vehicleNames);
        }
        public ActionResult DriverNameCNIC(string vehiId)
        {
            int vehhId;
            List<ListItemVehicleChange> nameCNIC = new List<ListItemVehicleChange>();
            if (!string.IsNullOrEmpty(vehiId))
            {
                vehhId = Convert.ToInt32(vehiId);

                var Ordercountmaxcount = from s in db.Orders.OrderByDescending(x => x.CreatedDate)
                                         select new { s.OrderCode };
                string maxcount = (Int32.Parse(Ordercountmaxcount.FirstOrDefault().OrderCode) + 1).ToString();

                var vehicle = from x in db.Vehicles.Where(x => x.VehicleID == vehhId && x.Driver.Active == true && x.Driver.LicenseEDate > DateTime.Now
                                  && x.Trailers.Any())
                              select x;
                if (vehicle.Any())
                {
                    foreach (var item in vehicle)
                    {
                        nameCNIC.Add(new ListItemVehicleChange { DrvName = item.Driver.DriverName, DrvCnic = item.Driver.CNIC, PrdType = item.Trailers.FirstOrDefault().LoadingType, Ordercountmaxcount = maxcount });
                    }
                }
                else
                {
                    nameCNIC.Add(new ListItemVehicleChange { DrvName = string.Empty, DrvCnic = string.Empty, PrdType = false, Ordercountmaxcount = string.Empty });
                }


            }
            else
            {
                nameCNIC.Add(new ListItemVehicleChange { DrvName = string.Empty, DrvCnic = string.Empty, PrdType = false, Ordercountmaxcount = string.Empty });
            }
            return Json(nameCNIC);
        }
        public ActionResult UniqueOrderCode(string Code, int Id)
        {
            var order = db.Orders.Where(x => x.OrderCode == Code);
            if (order.Count() == 0)
            {
                return Json(true);
            }
            else
            {
                if (Id != 0)
                {
                    var modifyResult = order.Where(x => x.OrderID == Id).ToList();
                    if (modifyResult.Count() == 0)
                    {
                        return Json(false);
                    }
                    else
                    {
                        return Json(true);
                    }

                }
                else
                {
                    return Json(false);
                }

            }

        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,OrderDate,OrderQty,RemainQty,CustomerID,ProductID,OrderCode,DeletedFlag,CarrierID,VehicleID,OrderStatusID,OrderDeliveryDT,RowVersion")] Order order, FormCollection formCollection, byte[] RowVersion)
        {
            string[] fieldsToBind = { "OrderID", "OrderDate", "OrderQty", "RemainQty", "CustomerID", "ProductID", "OrderCode", "DeletedFlag", "CarrierID", "VehicleID", "OrderStatusID", "OrderDeliveryDT", "CustomerName", "CarrierName", "VehicleCode", "DriverName", "DriverCNIC", "ProductID" };

            try
            {
                if (ModelState.IsValid)
                {
                    if (order.OrderID.Equals(0))
                    {
                        var shipment = new Shipment()
                        {
                            ProductID = order.ProductID,
                            IsManual = false,
                            CreatedDate = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            ShipmentStatusID = 1,
                            OrderID = order.OrderID,
                            CustomerName = formCollection["Customer.CustomerName"],
                            CarrierName = formCollection["Carrier.CarrierName"],
                            VehicleCode = formCollection["Vehicle.VehicleCode"],
                            DriverName = formCollection["Vehicle.Driver.DriverName"],
                            DriverCNIC = formCollection["Vehicle.Driver.CNIC"]
                        };
                        ///////////////////////////////////////////////////////////////
                        order.CreatedDate = DateTime.Now;
                        order.CreatedBy = User.Identity.Name;
                        order.ModifiedDate = DateTime.Now;
                        order.ModifiedBy = User.Identity.Name;
                        db.Orders.Add(order);
                        //////////////////////////////////////////////////////////////
                        db.Shipments.Add(shipment);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        string UpdatedUser = string.Empty;
                        try
                        {
                            Shipment ship = db.Shipments.Where(x => x.OrderID == order.OrderID).FirstOrDefault<Shipment>();
                            //TryUpdateModel(ship, fieldsToBind);//
                            UpdatedUser = ship.ModifiedBy;//
                            ship.ModifiedDate = DateTime.Now;
                            ship.ModifiedBy = User.Identity.Name;
                            ship.CustomerName = formCollection["Customer.CustomerName"];
                            ship.CarrierName = formCollection["Carrier.CarrierName"];
                            ship.VehicleCode = formCollection["Vehicle.VehicleCode"];
                            ship.DriverName = formCollection["Vehicle.Driver.DriverName"];
                            ship.DriverCNIC = formCollection["Vehicle.Driver.CNIC"];
                            ship.ProductID = order.ProductID;
                            db.Shipments.Attach(ship);
                            db.Entry(ship).Property(x => x.ModifiedDate).IsModified = true;
                            db.Entry(ship).Property(x => x.ModifiedBy).IsModified = true;
                            db.Entry(ship).Property(x => x.CustomerName).IsModified = true;
                            db.Entry(ship).Property(x => x.CarrierName).IsModified = true;
                            db.Entry(ship).Property(x => x.VehicleCode).IsModified = true;
                            db.Entry(ship).Property(x => x.DriverName).IsModified = true;
                            db.Entry(ship).Property(x => x.DriverCNIC).IsModified = true;
                            db.Entry(ship).Property(x => x.ProductID).IsModified = true;
                            db.Entry(ship).State = EntityState.Modified;
                            /////////////////////////////////////////////////////////////////////////////////////
                            order.ModifiedDate = DateTime.Now;
                            order.ModifiedBy = User.Identity.Name;
                            db.Entry(order).State = EntityState.Modified;
                            db.Entry(order).Property(x => x.CreatedDate).IsModified = false;
                            db.Entry(order).Property(x => x.CreatedBy).IsModified = false;
                            
                            ////////////////////////////////////////////////////////////////////////////////////

                            //db.Entry(order).OriginalValues["RowVersion"] = RowVersion;
                            //Shipment ship1 = db.Shipments.Where(x => x.OrderID == order.OrderID).FirstOrDefault<Shipment>();
                            //Order order1 = db.Orders.Where(x => x.OrderID == order.OrderID).FirstOrDefault<Order>();//
                            db.Entry(order).OriginalValues["RowVersion"] = RowVersion;//


                            ///////////////

                            db.SaveChanges();

                            return RedirectToAction("Index");
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            var entry = ex.Entries.Single();
                            var clientValues = (Order)entry.Entity;
                            var databaseEntry = entry.GetDatabaseValues();
                            if (databaseEntry == null)
                            {
                                TempData["ErrorMessage"] = "Unable to save changes. The record was deleted";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Unable to save changes. The record was edited by " + UpdatedUser + ". Select Updated Record.";
                            }
                        }
                    }

                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                }
            }
            catch
            {
                TempData["CustomError"] = "Order Code is not Unique";
                return RedirectToAction("Index");
            }

            ViewBag.CarrierID = new SelectList(db.Carriers, "CarrierID", "CarrierName", order.CarrierID);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", order.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", order.ProductID);
            ViewBag.OrderStatusID = new SelectList(db.OrderStatus, "ID", "Status", order.OrderStatusID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleCode", order.VehicleID);
            return RedirectToAction("Index");
            //return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
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
