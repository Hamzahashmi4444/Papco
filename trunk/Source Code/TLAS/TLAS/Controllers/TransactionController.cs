using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TLAS.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        public ActionResult Order()
        {
            return View();
        }
        public ActionResult Shipment()
        {
            return View();
        }
        public ActionResult ManualEntry()
        {
            return View();
        }
    }
}