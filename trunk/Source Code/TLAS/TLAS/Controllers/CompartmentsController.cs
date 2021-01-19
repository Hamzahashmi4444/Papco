using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TLAS.Models;

namespace TLAS.Controllers
{
    [Authorize]
    public class CompartmentsController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        // GET: Compartments
        //public ActionResult Index()
        //{
        //    var compartments = db.Compartments.Include(c => c.Trailer);
        //    return View(compartments.ToList());
        //}
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewData["trailorId"] = id;
            ViewData["trailerCode"] = db.Trailers.Where(p => p.TrailerID == id).FirstOrDefault().TrailerCode;
            var compartments = db.Compartments.Include(c => c.Trailer).Where(p => p.TrailerID == id);
            return View(compartments.ToList());
        }

        // GET: Compartments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Compartment compartment = db.Compartments.Find(id);
            if (compartment == null)
            {
                return HttpNotFound();
            }
            return View(compartment);
        }

        // GET: Compartments/Create
        public ActionResult Create()
        {
            ViewBag.TrailerID = new SelectList(db.Trailers, "TrailerID", "TrailerCode");
            return View();
        }

        // POST: Compartments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompartmentID,CompartmentCode,Capactiy,TrailerID,Description,ChamberDipMM")] Compartment compartment)
        {
            if (ModelState.IsValid)
            {
                if (compartment.CompartmentID.Equals(0))
                {
                    compartment.CreatedDate = DateTime.Now;
                    compartment.CreatedBy = User.Identity.Name;
                    compartment.ModifiedDate = DateTime.Now;
                    compartment.ModifiedBy = User.Identity.Name;
                    db.Compartments.Add(compartment);
                    db.SaveChanges();
                    return RedirectToAction("Index", new
                    {
                        ID = compartment.TrailerID
                    });
                }
                else
                {
                    compartment.ModifiedDate = DateTime.Now;
                    compartment.ModifiedBy = User.Identity.Name;
                    db.Entry(compartment).State = EntityState.Modified;
                    db.Entry(compartment).Property(x => x.CreatedDate).IsModified = false;
                    db.Entry(compartment).Property(x => x.CreatedBy).IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("Index", new
                    {
                        ID = compartment.TrailerID
                    });
                }

            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            ViewBag.TrailerID = new SelectList(db.Trailers, "TrailerID", "TrailerCode", compartment.TrailerID);
            return View(compartment);
        }

        // GET: Compartments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Compartment compartment = db.Compartments.Find(id);
            if (compartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TrailerID = new SelectList(db.Trailers, "TrailerID", "TrailerCode", compartment.TrailerID);
            return View(compartment);
        }

        // POST: Compartments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompartmentID,CompartmentCode,Capactiy,TrailerID,Description,ModifiedDate,CreatedDate")] Compartment compartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(compartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TrailerID = new SelectList(db.Trailers, "TrailerID", "TrailerCode", compartment.TrailerID);
            return View(compartment);
        }

        // GET: Compartments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Compartment compartment = db.Compartments.Find(id);
            if (compartment == null)
            {
                return HttpNotFound();
            }
            return View(compartment);
        }

        // POST: Compartments/Delete/5
        [HttpPost, ActionName("Delete")]
       // [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int code, int traId)
        {
            Compartment compartment = db.Compartments.Where(c => c.TrailerID == traId && c.CompartmentCode == code).FirstOrDefault<Compartment>();
            db.Compartments.Remove(compartment);
            db.SaveChanges();
            return RedirectToAction("Index", new
            {
                ID = traId
            });
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
