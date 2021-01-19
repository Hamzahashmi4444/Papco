using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using TLAS.RPTDatasets;
using System.Web.UI.WebControls;
using TLAS.Models;
using TLAS.ViewModel;

namespace TLAS.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    public class WBReportController : Controller
    {
        private TLASPreEntities db = new TLASPreEntities();

        // GET: Report
        public ActionResult Index(string SDate, string EDate)
        {

            DateTime StartDateTime = Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToShortDateString() + " 00:00 AM");
            DateTime EndDateTime = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 11:59 PM");

            if (!string.IsNullOrEmpty(SDate) && !SDate.Contains("Select"))
            {
                StartDateTime = Convert.ToDateTime(SDate + " 00:00 AM");
            }


            if (!string.IsNullOrEmpty(EDate) && !EDate.Contains("Select"))
            {
                EndDateTime = Convert.ToDateTime(EDate + " 11:59 PM");
            }



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
                                CustomerName = d.Shipment.Order.Customer.CustomerName,
                                OrderQty = d.Shipment.Order.OrderQty,
                                Prodtype = d.Shipment.Order.Product.ProductName
                            }).Where(x => x.ModifiedDate >= StartDateTime && x.ModifiedDate <= EndDateTime);





      //      DsReports.tbl_WBReportDataTable dsRptView = new DsReports.tbl_WBReportDataTable();

        //    sda.Fill(dsRptView);


            ReportViewer reportViewer = new ReportViewer()
            {
                SizeToReportContent = true,
                Width = Unit.Percentage(100),
                Height = Unit.Percentage(100),
            };

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/RPTReports/WB.rdlc");
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsWB", _weighbride.ToList<WeighbridgeViewModel>());
            reportViewer.LocalReport.DataSources.Add(rdc);
            reportViewer.LocalReport.SetParameters(new ReportParameter("StartDate", StartDateTime.ToShortDateString()));
            reportViewer.LocalReport.SetParameters(new ReportParameter("EndDate", EndDateTime.ToShortDateString()));
            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            return View();



        }
    }
}