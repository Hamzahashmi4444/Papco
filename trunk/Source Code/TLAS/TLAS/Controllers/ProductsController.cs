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
    public class ProductsController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        // GET: Products
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (TempData["ErrorMessage"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["ErrorMessage"].ToString());
            }
            //return View(db.Products.ToList());
            ViewBag.CurrentSort = sortOrder; // added new
            ViewBag.IdSortParm = sortOrder == "id" ? "id_desc" : "id";
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.ActiveSortParm = sortOrder == "Active" ? "InActive" : "Active";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;




            var products = from s in db.Products
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                int numValue;
                bool parsed = Int32.TryParse(searchString, out numValue);

                if (parsed)
                {
                    products = products.Where(s => s.ProductID == numValue);
                }
                else
                {
                    products = products.Where(s => s.ProductName.ToUpper().Contains(searchString.ToUpper()));
                }
            }
            switch (sortOrder)
            {
                case "id":
                    products = products.OrderBy(s => s.ProductID);
                    break;
                case "id_desc":
                    products = products.OrderByDescending(s => s.ProductID);
                    break;
                case "Name":
                    products = products.OrderBy(s => s.ProductName);
                    break;
                case "Name_desc":
                    products = products.OrderByDescending(s => s.ProductName);
                    break;
                case "Active":
                    products = products.OrderBy(s => s.Active);
                    break;
                case "InActive":
                    products = products.OrderByDescending(s => s.Active);
                    break;
                default:
                    products = products.OrderBy(s => s.ProductID);
                    break;
            }
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,WeighOut,Active,LastActive,Remarks,ModifiedDate,CreatedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ProductID,ProductName,WeighOut,Active,LastActive,Remarks")] Product product)
        public async Task<ActionResult> Edit(int? ProductID, byte[] RowVersion)
        {
            string[] fieldsToBind = new string[] { "ProductID", "ProductName", "WeighOut", "Active", "LastActive", "Remarks" };
            if (ProductID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                Product product = new Product();
                //customer.CustomerID.Equals(0)
                if (ProductID.Equals(0) && TryUpdateModel(product, fieldsToBind))
                {
                    product.CreatedBy = User.Identity.Name;
                    product.ModifiedBy = User.Identity.Name;
                    product.CreatedDate = DateTime.Now;
                    product.ModifiedDate= DateTime.Now;
                    db.Products.Add(product);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                var productToUpdate = await db.Products.FindAsync(ProductID);
                if (productToUpdate == null)
                {
                    Product productDeleted = new Product();
                    TryUpdateModel(productDeleted, fieldsToBind);
                    ModelState.AddModelError(string.Empty, "Unable to save changes. The product was deleted by another user.");
                    return View(productDeleted);
                }
                if (TryUpdateModel(productToUpdate, fieldsToBind))
                {
                    string UpdatedUser = string.Empty;
                    try
                    {
                        UpdatedUser = productToUpdate.ModifiedBy;
                        productToUpdate.ModifiedDate = DateTime.Now;
                        productToUpdate.ModifiedBy = User.Identity.Name;
                        db.Entry(productToUpdate).Property(x => x.CreatedDate).IsModified = false;
                        db.Entry(productToUpdate).Property(x => x.CreatedBy).IsModified = false;
                        db.Entry<Product>(productToUpdate).State = EntityState.Modified;
                        db.Entry(productToUpdate).OriginalValues["RowVersion"] = RowVersion;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var entry = ex.Entries.Single();
                        var clientValues = (Product)entry.Entity;
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
                //    product.ModifiedDate = DateTime.Now;
                //product.ModifiedBy = User.Identity.Name;
                //db.Entry(product).State = EntityState.Modified;
                //db.Entry(product).Property(x => x.CreatedDate).IsModified = false;
                //db.Entry(product).Property(x => x.CreatedBy).IsModified = false;
                //db.SaveChanges();
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            //return View(product);
            return RedirectToAction("Index");
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
