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
    public class TrailersController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        // GET: Trailers
        public ActionResult Index(int? id)
        {
            if (TempData["CustomError"] != null)
            {
                ModelState.AddModelError("", TempData["CustomError"].ToString());
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewData["vehicleID"] = id;
            ViewData["vehicleCode"] = db.Vehicles.Where(p => p.VehicleID == id).FirstOrDefault().VehicleCode;
            SelectLoadingType();
            var trailers = db.Trailers.Include(t => t.Vehicle).Where(p => p.VehicleID == id);
            TempData["VehicleTempID"] = id;
            return View(trailers.ToList());
        }
        public void SelectLoadingType()
        {
            List<SelectListItem> items = new List<SelectListItem>();
                //items.Add(new SelectListItem { Text = "Top(Condensate)", Value = true.ToString() });
                items.Add(new SelectListItem { Text = "Top", Value = true.ToString() });
                //items.Add(new SelectListItem { Text = "Bottom(LPG)", Value = false.ToString() });

            ViewData["loadingType"] = items;

        }
        // GET: Trailers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trailer trailer = db.Trailers.Find(id);
            if (trailer == null)
            {
                return HttpNotFound();
            }
            return View(trailer);
        }

        // GET: Trailers/Create
        public ActionResult Create()
        {
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleCode");
            return View();
        }

        // POST: Trailers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TrailerID,TrailerCode,LoadingType,VehicleID,BowserCalibrationCertificate,CertificateExpiryDate")] Trailer trailer)
        {
            if (ModelState.IsValid)
            {
                if (trailer.TrailerID.Equals(0))
                {
                    trailer.CreatedDate = DateTime.Now;
                    trailer.CreatedBy = User.Identity.Name;
                    trailer.ModifiedDate = DateTime.Now;
                    trailer.ModifiedBy = User.Identity.Name;
                    db.Trailers.Add(trailer);
                    db.SaveChanges();
                    return RedirectToAction("Index", new
                    {
                        ID = trailer.VehicleID
                    });
                }
                else
                {
                    trailer.ModifiedDate = DateTime.Now;
                    trailer.ModifiedBy = User.Identity.Name;
                    db.Entry(trailer).State = EntityState.Modified;
                    db.Entry(trailer).Property(x => x.CreatedDate).IsModified = false;
                    db.Entry(trailer).Property(x => x.CreatedBy).IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("Index", new
                    {
                        ID = trailer.VehicleID
                    });
                }

            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleCode", trailer.VehicleID);
            return View(trailer);
        }

        // GET: Trailers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trailer trailer = db.Trailers.Find(id);
            if (trailer == null)
            {
                return HttpNotFound();
            }
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleCode", trailer.VehicleID);
            return View(trailer);
        }

        // POST: Trailers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TrailerID,TrailerCode,LoadingType,VehicleID,ModifiedDate,CreatedDate")] Trailer trailer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trailer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "VehicleCode", trailer.VehicleID);
            return View(trailer);
        }

        // GET: Trailers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trailer trailer = db.Trailers.Find(id);
            if (trailer == null)
            {
                return HttpNotFound();
            }
            return View(trailer);
        }

        // POST: Trailers/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int vehId)
        {
            try { 
            Trailer trailer = db.Trailers.Find(id);
            db.Trailers.Remove(trailer);
            db.SaveChanges();
            return RedirectToAction("Index", new
            {
                ID = vehId
            });
            }
            catch{
                TempData["CustomError"] = "The trailer cannot be removed";
                return RedirectToAction("Index", new
                {
                    ID = vehId
                });
            }
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
