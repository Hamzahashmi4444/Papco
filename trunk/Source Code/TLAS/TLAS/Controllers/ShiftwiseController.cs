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

namespace TLAS.Controllers
{
    [Authorize(Roles = "Admin,Supervisor")]
    public class ShiftwiseController : Controller
    {
        // GET: Report
        public ActionResult Index(string ReportDate, string ID, string DrpdwnLoadtype)
        {

            TLASPreEntities db = new TLASPreEntities();

            string Stime = "00:00 AM";
            String Etime = "08:00 AM";
            string Selectedshift = "Shift A";
            int loadtype = -1;
            string conString = ConfigurationManager.ConnectionStrings["TLASPreConnectionString"].ConnectionString;
            SqlCommand cmd = new SqlCommand("GetLoadedReport");

            if (!string.IsNullOrEmpty(DrpdwnLoadtype))
            {
                loadtype = Int32.Parse(DrpdwnLoadtype);
            }


            if (!string.IsNullOrEmpty(ID))
            {
                int _id= Convert.ToInt32(ID);
                var CurrentSelectedShift = from o in db.Shifts
                                           where o.ID == _id
                                           select o;

                Selectedshift = CurrentSelectedShift.FirstOrDefault().ShiftName;
                Stime = CurrentSelectedShift.FirstOrDefault().StartTime;
                Etime = CurrentSelectedShift.FirstOrDefault().EndTime;

            }


            DateTime StartDateTime = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + Stime);
            DateTime EndDateTime = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + Etime);

            if (!string.IsNullOrEmpty(ReportDate) && !ReportDate.Contains("Select"))
            {
                StartDateTime = Convert.ToDateTime(ReportDate + " " + Stime);
                EndDateTime = Convert.ToDateTime(ReportDate + " "+ Etime);
            }

            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    sda.SelectCommand = cmd;


                    cmd.Parameters.Add("@Startdatetime", SqlDbType.DateTime).Value = StartDateTime;
                    cmd.Parameters.Add("@Enddatetime", SqlDbType.DateTime).Value = EndDateTime;
                    cmd.Parameters.Add("@Prodtype", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@Loadtype", SqlDbType.Int).Value = loadtype;

                    DsReports.ViewLoadedAllDataTable dsRptView = new DsReports.ViewLoadedAllDataTable();

                    sda.Fill(dsRptView);

                    DsReports.ViewLoadedAllDataTable dsRptView1 = new DsReports.ViewLoadedAllDataTable();

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@Startdatetime", SqlDbType.DateTime).Value = StartDateTime;
                    cmd.Parameters.Add("@Enddatetime", SqlDbType.DateTime).Value = EndDateTime;
                    cmd.Parameters.Add("@Prodtype", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@Loadtype", SqlDbType.Int).Value = loadtype;

                    sda.Fill(dsRptView1);

                    ReportViewer reportViewer = new ReportViewer()
                    {
                        SizeToReportContent = true,
                        Width = Unit.Percentage(100),
                        Height = Unit.Percentage(100),
                    };

                    reportViewer.ProcessingMode = ProcessingMode.Local;

                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/RPTReports/Shiftwise.rdlc");
                    reportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("report", dsRptView.Rows);
                    ReportDataSource rdc1 = new ReportDataSource("DataSet1", dsRptView1.Rows);
                    
                    reportViewer.LocalReport.SetParameters(new ReportParameter("StartDate", StartDateTime.ToShortDateString()));
                    reportViewer.LocalReport.SetParameters(new ReportParameter("Time", Stime));
                    reportViewer.LocalReport.SetParameters(new ReportParameter("EndTime", Etime));
                    reportViewer.LocalReport.SetParameters(new ReportParameter("Shift", Selectedshift));
                    reportViewer.LocalReport.SetParameters(new ReportParameter("Loadtype", loadtype.ToString()));
                  
                    reportViewer.LocalReport.DataSources.Add(rdc);
                    reportViewer.LocalReport.DataSources.Add(rdc1);
                    reportViewer.LocalReport.Refresh();

                    ViewBag.ReportViewer = reportViewer;

                    return View(db.Shifts.ToList());

                }
            }


        }
    }
}