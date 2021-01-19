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
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace TLAS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccessCardsController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        //// GET: AccessCards
        //public ActionResult Index()
        //{
        //    var accessCards = db.AccessCards.Include(a => a.Bay);
        //    return View(accessCards.ToList());
        //}
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
            ViewBag.keySortParm = sortOrder == "key" ? "key_desc" : "key";
            ViewBag.BayIdSortParm = sortOrder == "BayId" ? "BayId_desc" : "BayId";
            ViewBag.ActiveSortParm = sortOrder == "Active" ? "InActive" : "Active";
            ViewBag.IsAssignedSortParm = sortOrder == "IsAssigned" ? "IsAssigned_desc" : "IsAssigned";

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




            var accessCard = from s in db.AccessCards
                             select new AccessCardViewModel
                             {
                                 CardKey = s.CardKey,
                                 CardID = s.CardID,
                                 BayID = s.BayID,
                                 Remarks = s.Remarks,
                                 Active = s.Active,
                                 IsAssigned = s.IsAssigned,
                                 ModifiedDate = s.ModifiedDate,
                                 ModifiedBy = s.ModifiedBy,
                                 RowVersion = s.RowVersion
                             };
            if (!String.IsNullOrEmpty(searchString))
            {
                int numValue;
                bool parsed = Int32.TryParse(searchString, out numValue);

                if (parsed)
                {
                    accessCard = accessCard.Where(s => s.CardID == numValue);
                }

            }
            switch (sortOrder)
            {
                case "id":
                    accessCard = accessCard.OrderBy(s => s.CardID);
                    break;
                case "id_desc":
                    accessCard = accessCard.OrderByDescending(s => s.CardID);
                    break;
                case "key":
                    accessCard = accessCard.OrderBy(s => s.CardKey);
                    break;
                case "key_desc":
                    accessCard = accessCard.OrderByDescending(s => s.CardKey);
                    break;
                case "BayId":
                    accessCard = accessCard.OrderBy(s => s.BayID);
                    break;
                case "BayId_desc":
                    accessCard = accessCard.OrderByDescending(s => s.BayID);
                    break;
                case "Active":
                    accessCard = accessCard.OrderBy(s => s.Active);
                    break;
                case "InActive":
                    accessCard = accessCard.OrderByDescending(s => s.Active);
                    break;
                case "IsAssigned":
                    accessCard = accessCard.OrderBy(s => s.IsAssigned);
                    break;
                case "IsAssigned_desc":
                    accessCard = accessCard.OrderByDescending(s => s.IsAssigned);
                    break;
                case "ModifiedDate":
                    accessCard = accessCard.OrderBy(s => s.ModifiedDate);
                    break;
                case "ModifiedDate_desc":
                    accessCard = accessCard.OrderByDescending(s => s.ModifiedDate);
                    break;
                case "ModifiedBy":
                    accessCard = accessCard.OrderBy(s => s.ModifiedBy);
                    break;
                case "ModifiedBy_desc":
                    accessCard = accessCard.OrderByDescending(s => s.ModifiedBy);
                    break;

                default:
                    accessCard = accessCard.OrderByDescending(s => s.ModifiedDate);
                    break;
            }
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ViewBag.Bays = new SelectList(db.Bays.ToList(), "BayID", "Remarks");
            return View(accessCard.ToPagedList(pageNumber, pageSize));
            //return View(customers.ToList());
        }
        public ActionResult SelectBay()
        {
            List<SelectListItem> bayNames = new List<SelectListItem>();
            var bay = from x in db.Bays
                      select new { x.BayID };
            foreach (var item in bay)
            {
                bayNames.Add(new SelectListItem { Text = item.BayID.ToString(), Value = item.BayID.ToString() });
            }
            ViewData["BayID"] = bayNames;
            //return View();
            return Json(bayNames);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetAutoComplete(string term)
        {
            List<String> _result;

            _result = db.AccessCards.Where(x => x.CardID.ToString().StartsWith(term))
                              .Select(e => e.CardID.ToString()).Distinct().ToList();

            return Json(_result, JsonRequestBehavior.AllowGet);
        }


        // GET: AccessCards/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessCard accessCard = db.AccessCards.Find(id);
            if (accessCard == null)
            {
                return HttpNotFound();
            }
            return View(accessCard);
        }

        // GET: AccessCards/Create
        public ActionResult Create()
        {
            ViewBag.BayID = new SelectList(db.Bays, "BayID", "Remarks");
            return View();
        }

        // POST: AccessCards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(AccessCard accessCard, byte[] RowVersion)
        {
            string[] fieldsToBind = new string[] { "CardID", "BayID", "Active", "IsAssigned", "LastActive", "ModifiedDate", "Remarks", "CreatedDate", "CardKey" };
            if (ModelState.IsValid)
            {
                try
                {
                    if (accessCard.CardID.Equals(0))
                    {
                        accessCard.ModifiedDate = accessCard.LastActive = accessCard.CreatedDate = DateTime.Now;
                        accessCard.CreatedBy = accessCard.ModifiedBy = User.Identity.Name;
                        int maxId = 0;
                        if (db.AccessCards.Count() > 0)
                        {
                            maxId = db.AccessCards.Max(x => x.CardID);
                        }
                        accessCard.CardID = (maxId > 0) ? (maxId + 1) : 1 ; 
                        db.AccessCards.Add(accessCard);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var accessCardToUpdate = db.AccessCards.Find(accessCard.CardID);
                        if (accessCardToUpdate == null)
                        {
                            AccessCard accessCardDeleted = new AccessCard();
                            TryUpdateModel(accessCardDeleted, fieldsToBind);
                            ModelState.AddModelError(string.Empty, "Unable to save changes. The Access Card was deleted by another user.");
                            return RedirectToAction("Index");
                        }
                        if (TryUpdateModel(accessCardToUpdate, fieldsToBind))
                        {
                            string UpdatedUser = string.Empty;
                            try
                            {
                                UpdatedUser = accessCardToUpdate.ModifiedBy;

                                accessCardToUpdate.ModifiedDate = DateTime.Now;
                                accessCardToUpdate.ModifiedBy = User.Identity.Name;
                            db.AccessCards.Attach(accessCardToUpdate);
                            db.Entry(accessCardToUpdate).Property(x => x.BayID).IsModified = true;
                            db.Entry(accessCardToUpdate).Property(x => x.Remarks).IsModified = true;
                            db.Entry(accessCardToUpdate).Property(x => x.Active).IsModified = true;
                            db.Entry(accessCardToUpdate).Property(x => x.IsAssigned).IsModified = true;
                            db.Entry(accessCardToUpdate).Property(x => x.ModifiedDate).IsModified = true;
                            db.Entry(accessCardToUpdate).Property(x => x.ModifiedBy).IsModified = true;


                            db.Entry<AccessCard>(accessCardToUpdate).State = EntityState.Modified;
                                db.Entry(accessCardToUpdate).OriginalValues["RowVersion"] = RowVersion;
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {
                                var entry = ex.Entries.Single();
                                var clientValues = (AccessCard)entry.Entity;
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
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    TempData["CustomError"] = "Card ID is not Unique";
                    return RedirectToAction("Index");
                }

            }

            ViewBag.BayID = new SelectList(db.Bays, "BayID", "Remarks", accessCard.BayID);
            return View("Index", accessCard);
        }

        // GET: AccessCards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessCard accessCard = db.AccessCards.Find(id);
            if (accessCard == null)
            {
                return HttpNotFound();
            }
            ViewBag.BayID = new SelectList(db.Bays, "BayID", "Remarks", accessCard.BayID);
            return View(accessCard);
        }

        // POST: AccessCards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CardID,BayID,Active,IsAssigned,LastActive,ModifiedDate,Remarks,CreatedDate,CardKey")] AccessCard accessCard, byte[] RowVersion)
        {
            string[] fieldsToBind = new string[] { "CardID", "BayID", "Active", "IsAssigned", "LastActive", "ModifiedDate", "Remarks", "CreatedDate", "CardKey" };
            if (ModelState.IsValid)
            {
                var accessCardToUpdate = await db.AccessCards.FindAsync(accessCard.CardID);
                if (accessCardToUpdate == null)
                {
                    AccessCard accessCardDeleted = new AccessCard();
                    TryUpdateModel(accessCardDeleted, fieldsToBind);
                    ModelState.AddModelError(string.Empty, "Unable to save changes. The Access Card was deleted by another user.");
                    return RedirectToAction("Index");
                }
                if (TryUpdateModel(accessCardToUpdate, fieldsToBind))
                {
                    string UpdatedUser = string.Empty;
                    try
                    {
                        UpdatedUser = accessCardToUpdate.ModifiedBy;
                        accessCardToUpdate.ModifiedDate = DateTime.Now;
                        accessCardToUpdate.ModifiedBy = User.Identity.Name;
                        db.Entry(accessCardToUpdate).Property(x => x.CreatedDate).IsModified = false;
                        db.Entry(accessCardToUpdate).Property(x => x.CreatedBy).IsModified = false;
                        db.Entry<AccessCard>(accessCardToUpdate).State = EntityState.Modified;
                        db.Entry(accessCardToUpdate).OriginalValues["RowVersion"] = RowVersion;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var entry = ex.Entries.Single();
                        var clientValues = (AccessCard)entry.Entity;
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
                return RedirectToAction("Index");

                //db.Entry(accessCard).State = EntityState.Modified;
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }
            ViewBag.BayID = new SelectList(db.Bays, "BayID", "Remarks", accessCard.BayID);
            return View(accessCard);
        }

        // GET: AccessCards/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessCard accessCard = db.AccessCards.Find(id);
            if (accessCard == null)
            {
                return HttpNotFound();
            }
            return View(accessCard);
        }

        // POST: AccessCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccessCard accessCard = db.AccessCards.Find(id);
            db.AccessCards.Remove(accessCard);
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
