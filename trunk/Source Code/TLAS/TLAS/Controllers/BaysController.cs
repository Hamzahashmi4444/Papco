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
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace TLAS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BaysController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        // GET: Bays
        //public ActionResult Index()
        //{
        //    var bays = db.Bays.Include(b => b.Product);
        //    return View(bays.ToList());
        //}
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (TempData["ErrorMessage"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["ErrorMessage"].ToString());
            }
            //return View(db.Products.ToList());
            ViewBag.CurrentSort = sortOrder; // added new
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.ActiveSortParm = sortOrder == "Active" ? "InActive" : "Active";
            ViewBag.PrdNameSortParm = sortOrder == "PrdName" ? "PrdName_desc" : "PrdName";
            ViewBag.FrequencySortParm = sortOrder == "Frequency" ? "Frequency_desc" : "Frequency";
            ViewBag.QueueSortParm = sortOrder == "Queue" ? "Queue_desc" : "Queue";


            //List<SelectListItem> countryList = new List<SelectListItem>();

            List<SelectListItem> productList = (from p in db.Products.AsEnumerable()
                                                 select new SelectListItem
                                                 {
                                                     Text = p.ProductName,
                                                     Value = p.ProductID.ToString()
                                                 }).ToList();
            ViewBag.ProductsDropDown = productList;
            ViewBag.ProductCount = db.Bays.Count() + 1;
            //ViewBag.ProductsDropDown = new SelectList(db.Products, "ProductID", "ProductName");

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;




            var bay = from s in db.Bays.Include(b => b.Product)
                      select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                int numValue;
                bool parsed = Int32.TryParse(searchString, out numValue);

                if (parsed)
                {
                    bay = bay.Where(s => s.BayID == numValue);
                }
                else
                {
                    bay = bay.Where(s => s.Product.ProductName.ToUpper().Contains(searchString.ToUpper()));
                }
            }
            switch (sortOrder)
            {
                case "id_desc":
                    bay = bay.OrderByDescending(s => s.BayID);
                    break;
                case "id":
                    bay = bay.OrderBy(s => s.BayID);
                    break;
                case "Active":
                    bay = bay.OrderBy(s => s.Active);
                    break;
                case "InActive":
                    bay = bay.OrderByDescending(s => s.Active);
                    break;
                case "PrdName":
                    bay = bay.OrderBy(s => s.Product.ProductName);
                    break;
                case "PrdName_desc":
                    bay = bay.OrderByDescending(s => s.Product.ProductName);
                    break;
                case "Frequency":
                    bay = bay.OrderBy(s => s.Frequency);
                    break;
                case "Frequency_desc":
                    bay = bay.OrderByDescending(s => s.Frequency);
                    break;
                case "Queue":
                    bay = bay.OrderBy(s => s.CurrQueue);
                    break;
                case "Queue_desc":
                    bay = bay.OrderByDescending(s => s.CurrQueue);
                    break;
                default:
                    bay = bay.OrderBy(s => s.BayID);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(bay.ToPagedList(pageNumber, pageSize));
        }

        // GET: Bays/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bay bay = db.Bays.Find(id);
            if (bay == null)
            {
                return HttpNotFound();
            }
            return View(bay);
        }

        // GET: Bays/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: Bays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BayID,Frequency,CurrQueue,Active,LastActive,Remarks,ModifiedDate,CreatedDate,ProductID")] Bay bay)
        {
            if (ModelState.IsValid)
            {
                db.Bays.Add(bay);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", bay.ProductID);
            return View(bay);
        }

        // GET: Bays/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bay bay = db.Bays.Find(id);
            if (bay == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", bay.ProductID);
            return View(bay);
        }

        // POST: Bays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "BayID,Frequency,CurrQueue,Active,LastActive,Remarks,ProductID")] Bay bay)
        public async Task<ActionResult> Edit(int? BayID, byte[] RowVersion, string[] ddlProduct)
        {
            if (ddlProduct == null)
            {
                TempData["ErrorMessage"] = "Please select the Product first.";
                return RedirectToAction("Index");
            }
            string[] fieldsToBind = new string[] { "BayID","Frequency","CurrQueue","Active","LastActive","Remarks","ProductID" };
            if (BayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            { 

                var bayToUpdate = await db.Bays.FindAsync(BayID);
                var productID = Request["Frequency"].ToString();
                if (productID.Equals("-1"))
                {
                    

                        Bay bayProduct = new Bay();
                    TryUpdateModel(bayProduct, fieldsToBind);

                    bayProduct.Frequency = 0;
                    bayProduct.CurrQueue = 0;
                    bayProduct.CreatedDate = DateTime.Now;
                    bayProduct.ModifiedDate = DateTime.Now;
                    bayProduct.CreatedBy = User.Identity.Name;
                    bayProduct.ModifiedBy = User.Identity.Name;
                    db.Bays.Add(bayProduct);
                    db.SaveChanges();

                }
                else if (bayToUpdate == null)
                {
                    Bay productDeleted = new Bay();
                    TryUpdateModel(productDeleted, fieldsToBind);
                    ModelState.AddModelError(string.Empty, "Unable to save changes. The product was deleted by another user.");
                    //return View(productDeleted);
                    return RedirectToAction("Index");
                }
                if (bayToUpdate !=null)
                {
                    TryUpdateModel(bayToUpdate, fieldsToBind);
                    string UpdatedUser = string.Empty;
                    try
                    {
                        //string strDDLValue = Request.Form["ddlVendor"].ToString();
                        bayToUpdate.ProductID = Convert.ToInt32(ddlProduct[0]); //Convert.ToInt32(Request["ddlProduct"].ToString());
                        UpdatedUser = bayToUpdate.ModifiedBy;
                        bayToUpdate.ModifiedDate = DateTime.Now;
                        bayToUpdate.ModifiedBy = User.Identity.Name;
                        db.Entry(bayToUpdate).Property(x => x.CreatedDate).IsModified = false;
                        db.Entry(bayToUpdate).Property(x => x.CreatedBy).IsModified = false;
                        db.Entry<Bay>(bayToUpdate).State = EntityState.Modified;
                        db.Entry(bayToUpdate).OriginalValues["RowVersion"] = RowVersion;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var entry = ex.Entries.Single();
                        var clientValues = (Bay)entry.Entity;
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
                //bay.ModifiedDate = DateTime.Now;
                //bay.ModifiedBy = User.Identity.Name;
                //db.Entry(bay).State = EntityState.Modified;
                //db.Entry(bay).Property(x => x.CreatedDate).IsModified = false;
                //db.Entry(bay).Property(x => x.CreatedBy).IsModified = false;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", bay.ProductID);
            //return View(bay);
            return RedirectToAction("Index");
        }

        // GET: Bays/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bay bay = db.Bays.Find(id);
            if (bay == null)
            {
                return HttpNotFound();
            }
            return View(bay);
        }

        // POST: Bays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bay bay = db.Bays.Find(id);
            db.Bays.Remove(bay);
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
