using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalProject.Models;
using FinalProject.DTO;
using FinalProject.Filters;

namespace FinalProject.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        DBEntities db = new DBEntities();
        [Route("dashboard")]
        public ActionResult Index()
        {
            //try
            //{
                if (Session["UserLogin"] == null)
                {
                    return Redirect("~/auth/login");
                }
                else
                {
                    ViewBag.DataView = new Dictionary<string, string>()
                  {
                     {"title","Dashboard"}
                  };

                    List<DashboardDTO> Toppositions = new List<DashboardDTO>();//alltime result
                    List<DashboardDTO> ToppositionMonths = new List<DashboardDTO>();//month result
                    List<DashboardDTO> ToppositionYears = new List<DashboardDTO>();//year result

                    //all most

                    List<TB_CANDIDATE> candpres = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 2).ToList();
                    List<TB_CANDIDATE> apa = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 4).ToList();
                    SearchTopJob(candpres);
                    List<DashboardDTO> toppres = (List<DashboardDTO>)TempData.Peek("Top");
                    DashboardDTO toppress = toppres.Where(m => m.moststate == "Pra-Selected").OrderByDescending(n => n.mostqty).FirstOrDefault();
                    Toppositions.Add(toppress);

                    List<TB_CANDIDATE> candcall = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 5).ToList();
                    SearchTopJob(candcall);
                    List<DashboardDTO> topcalls = (List<DashboardDTO>)TempData.Peek("Top");
                    DashboardDTO topcall = topcalls.Where(m => m.moststate == "Called").OrderByDescending(n => n.mostqty).FirstOrDefault();
                    Toppositions.Add(topcall);

                    TempData["topposition"] = Toppositions;

                    //month most
                    List<TB_CANDIDATE> candpresmonth = db.TB_CANDIDATE.Where(m => m.SOURCING_DATE.Value.Year == DateTime.Now.Year).Where(m => m.SOURCING_DATE.Value.Month == DateTime.Now.Month).Where(m => m.CANDIDATE_STATE_ID == 1).ToList();
                    SearchTopJob(candpresmonth);
                    List<DashboardDTO> toppresmonth = (List<DashboardDTO>)TempData.Peek("Top");
                    DashboardDTO toppressmonth = toppresmonth.Where(m => m.moststate == "Pra-Selected").OrderByDescending(n => n.mostqty).FirstOrDefault();
                    ToppositionMonths.Add(toppressmonth);

                    List<TB_CANDIDATE> candcallmonth = db.TB_CANDIDATE.Where(m => m.SOURCING_DATE.Value.Year == DateTime.Now.Year).Where(m => m.SOURCING_DATE.Value.Month == DateTime.Now.Month).Where(m => m.CANDIDATE_STATE_ID == 8).ToList();
                    SearchTopJob(candcallmonth);
                    List<DashboardDTO> topcallsmonth = (List<DashboardDTO>)TempData.Peek("Top");
                    DashboardDTO topcallmonth = topcallsmonth.Where(m => m.moststate == "Called").OrderByDescending(n => n.mostqty).FirstOrDefault();
                    ToppositionMonths.Add(topcallmonth);

                    TempData["toppositionmonth"] = ToppositionMonths;

                    //year most
                    List<TB_CANDIDATE> candpresyear = db.TB_CANDIDATE.Where(m => m.SOURCING_DATE.Value.Year == DateTime.Now.Year).Where(m => m.CANDIDATE_STATE_ID == 2).ToList();
                    SearchTopJob(candpresyear);
                    List<DashboardDTO> toppresyear = (List<DashboardDTO>)TempData.Peek("Top");
                    DashboardDTO toppressyear = toppresyear.Where(m => m.moststate == "Pra-Selected").OrderByDescending(n => n.mostqty).FirstOrDefault();
                    ToppositionYears.Add(toppressyear);

                    List<TB_CANDIDATE> candcallyear = db.TB_CANDIDATE.Where(m => m.SOURCING_DATE.Value.Year == DateTime.Now.Year).Where(m => m.CANDIDATE_STATE_ID == 5).ToList();
                    SearchTopJob(candcallyear);
                    List<DashboardDTO> topcallsyear = (List<DashboardDTO>)TempData.Peek("Top");
                    DashboardDTO topcallyear = topcallsyear.Where(m => m.moststate == "Called").OrderByDescending(n => n.mostqty).FirstOrDefault();
                    ToppositionYears.Add(topcallyear);

                    TempData["toppositionyear"] = ToppositionYears;

                    return View("Index");
                }
            //}
            //catch
            //{
            //    return Redirect("~/auth/error");
            //}
        }


        public ActionResult GetPie()
        {
            Ratio getcount = new Ratio();
            getcount.Jobstreet = db.TB_CANDIDATE.Where(x => x.SOURCE == "Jobstreet").Count();
            getcount.JobsID = db.TB_CANDIDATE.Where(x => x.SOURCE == "JobsID").Count();
            getcount.JobsDB = db.TB_CANDIDATE.Where(x => x.SOURCE == "JobsDB").Count();
            getcount.Joblike = db.TB_CANDIDATE.Where(x => x.SOURCE == "Joblike").Count();
            getcount.TopKarir = db.TB_CANDIDATE.Where(x => x.SOURCE == "Top Karir").Count();
            getcount.KarirPad = db.TB_CANDIDATE.Where(x => x.SOURCE == "KarirPad").Count();
            getcount.Karir2 = db.TB_CANDIDATE.Where(x => x.SOURCE == "Karir2.com").Count();
            return Json(getcount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBar()
        {
            Total total = new Total();
            total.PreJava = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 2 || m.POSITION == "Java").Count();
            total.PrePHP = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 2 || m.POSITION == "PHP").Count();
            total.PreRuby = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 2 || m.POSITION == "Ruby").Count();
            total.PreVB = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 2 || m.POSITION == "VB.NET").Count();

            total.CallJava = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 5 || m.POSITION == "Java").Count();
            total.CallPHP = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 5 || m.POSITION == "PHP").Count();
            total.CallRuby = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 5 || m.POSITION == "Ruby").Count();
            total.CallVB = db.TB_CANDIDATE.Where(m => m.CANDIDATE_STATE_ID == 5 || m.POSITION == "VB.NET").Count();
            return Json(total, JsonRequestBehavior.AllowGet);
        }

        public void SearchTopJob(List<TB_CANDIDATE> candidate)
        {
            TempData["Top"] = 0;
            List<DashboardDTO> most = new List<DashboardDTO>();
            foreach (TB_CANDIDATE c in candidate)
            {
                int Totalpos = candidate.Where(m => m.CANDIDATE_STATE_ID == c.CANDIDATE_STATE_ID).Count();
                TB_JOB_POSITION namepos = db.TB_JOB_POSITION.Where(m => m.JOBPOSITION_NAME == c.POSITION).FirstOrDefault();
                TB_STATE_CANDIDATE statepos = db.TB_STATE_CANDIDATE.Where(m => m.ID == c.CANDIDATE_STATE_ID).FirstOrDefault();
                DashboardDTO mostpos = new DashboardDTO { mostqty = Totalpos, mostposition = namepos.JOBPOSITION_NAME, moststate = statepos.STATE_NAME };

                most.Add(mostpos);
            }
            TempData["Top"] = most;
        }
    }
}