using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalProject.Controllers;
using FinalProject.DTO;
using FinalProject.Filters;
using FinalProject.Utils;
using FinalProject.Models;
using System.Configuration;

namespace FinalProject.Controllers
{
    [UserAccessCandidateFilter]
    public class CandidateController : Controller
    {
        DBEntities db = new DBEntities();
        [Route("candidate")]
        public ActionResult Index()
        {
            return Redirect("~/candidate/preselection");
        }
        //################################################ Sub Menu Candidate Preselection ################################# 

        //********************************************************** Manage Data Candidate **********************************************************

        //------------------------------------------------------- for view candidate preselection -----------------------------------------------
        [Route("candidate/praselection/read/{i?}")]
        public ActionResult CandidatePreselection(string i = null)
        {
            //try
            //{
            //    //---------------------------- prepare data candidate for show in view --------------
                //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
                //not  : data in this view especialy for candidate where state_id is 1 or 2 (state in step preselection)

                //formula pagination
                int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]);
                float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh => sh.VIEWS_INFORMATION == "YES" && (sh.CANDIDATE_STATE == 1 || sh.CANDIDATE_STATE == 3)).ToList().Count();
                int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));
            
                List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory()
                .Where(d => d.VIEWS_INFORMATION == "YES" &&( d.CANDIDATE_STATE == 1 || d.CANDIDATE_STATE == 3))
                .Skip(idx)
                .Take(perPage)
                .ToList();

                //---------------------------- prepare data viewbag --------------------


                //============================ process searchng ============================
                if (Request["filter"] != null)
                {
                    string Position = Request["POSITION"];
                    int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                    string Keyword = Request["Keyword"];
                    string DataPerPage = Request["DataPerPage"];
                    int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);
                    if (StateId != 0 && (Position == "all" && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).Skip(idx).Take(5).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));

                    }
                    if (Position != "all" && (StateId == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_APPLIED_POSITION == Position ||
                        d.CANDIDATE_SUITABLE_POSITION == Position &&
                        (d.VIEWS_INFORMATION == "YES" && (d.CANDIDATE_STATE == 1 || d.CANDIDATE_STATE == 3))).Skip(idx).Take(5).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Keyword != "" && (StateId == 0 && Position == "all"))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                            d.CANDIDATE_EMAIL.Contains(Keyword) ||
                            d.CANDIDATE_NAME.Contains(Keyword) ||
                            d.CANDIDATE_PHONE.Contains(Keyword) &&
                            (d.VIEWS_INFORMATION == "YES" && (d.CANDIDATE_STATE == 1 || d.CANDIDATE_STATE == 3))).Skip(idx).Take(5).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (DataPerPage != "" && (StateId == 0 && Position == "all" && Keyword == ""))
                    {
                        perPage = dt;
                        ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d => d.VIEWS_INFORMATION == "YES" && (
                    d.CANDIDATE_STATE == 1 || d.CANDIDATE_STATE == 3)).Skip(idx).Take(perPage).ToList();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                        Session.Add("DataPage", dt);
                    }
                    else
                    {
                        ListCandidate = ListCandidate.Where(d =>
                         d.CANDIDATE_APPLIED_POSITION == Position ||
                         d.CANDIDATE_SUITABLE_POSITION == Position ||
                         d.CANDIDATE_STATE == StateId ||
                         d.CANDIDATE_EMAIL.Contains(Keyword) ||
                         d.CANDIDATE_NAME.Contains(Keyword) ||
                         d.CANDIDATE_PHONE.Contains(Keyword) &&
                         (d.VIEWS_INFORMATION == "YES" && (d.CANDIDATE_STATE == 1 || d.CANDIDATE_STATE == 3))).Skip(idx).Take(dt).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                }
            //============================ end process searchng ============================
            List<string> Degree = new List<string>()
            {
                "High School", "Diploma", "Bachelor", "Magister", "Doctor"
            };

                ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Praselection"},
                    {"ListPosition",Manage_JobPositionDTO.GetData()},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 1 || d.ID == 3)},
                    {"PageCount",PageCount},
                    };
            ViewBag.Degree = new Dictionary<string, object>
            {
                { "ListDegree", new List<string>{"High School", "Diploma" }}
            };
                //return view
                return View("Preselection/Index", ListCandidate);
            //}
            //catch (Exception)
            //{
            //    return Redirect("~/auth/error");
            //}
        }

        //----------------------------------------------------------- view form add new candidate -----------------------------------------
        [Route("candidate/praselection/create/candidate")]
        public ActionResult CandidatePreselectionAdd()
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {

                    ViewBag.DataView = new Dictionary<string, object>(){
                    {"title","Praselection"}
                    };
                    return View("Preselection/AddCandidate");
                }
            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }

        //---------------------------------------------------------- View Detail candidate ------------------------------------------------
        [Route("candidate/praselection/read/detailcandidate/{id?}")]
        public ActionResult DetailCandidate(string id = null)
        {
            try
            {
                using (DBEntities db = new DBEntities())
            {
                if (id == null) return Redirect("~/candidate/praselection");

                int candidateId = Convert.ToInt16(id);

                DetailCandidateDTO DataDetail = Manage_DetailCandidate.GetData(candidateId);

                if (DataDetail == null) return Redirect("~/candidate/praselection");

                ViewBag.DataView = new Dictionary<string, object>()
                    {
                        {"title","Praselection"}
                    };

                return View("Preselection/DetailCandidate", DataDetail);
            }

            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }

        //-------------------------------------------------- PROCESS ADD NEW CANDIDATE --------------------------------------
        [Route("candidate/praselection/create/candidate/process")]
        public ActionResult CandidatePreselectionAdd(CandidateDTO DataNewCandidate, HttpPostedFileBase Pict = null, HttpPostedFileBase Cv = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //process add will return list object, [0] is return from db.SaveChanges() and [1] return candidate_id (CA******)

                    var CheckCan = db.TB_CANDIDATE.FirstOrDefault(m => m.CANDIDATE_NAME == DataNewCandidate.CANDIDATE_NAME && m.CANDIDATE_BIRTH_DATE == DataNewCandidate.CANDIDATE_BIRTH_DATE);
                    if(CheckCan != null)
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "The candidate's name and birth date cannot be the same");
                            TempData.Add("type", "danger");

                            return Redirect("~/candidate/praselection/read");
                        }
                    }

                    var ProcessAdd = Manage_CandidateDTO.AddData(DataNewCandidate, Pict, Cv);

                    if (Convert.ToInt16(ProcessAdd[0]) > 0)
                    {
                        if(TempData["message"] == null)
                        {
                        TempData.Add("message", "New Candidate added successfully");
                        TempData.Add("type", "success");

                        UserLogingUtils.SaveLoggingUserActivity("add new Candidate" + Convert.ToString(ProcessAdd[1]));
                        }
                    }
                    else
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "New Candidate failed to add");
                            TempData.Add("type", "warning");
                        }
                    }
                    return Redirect("~/candidate/praselection/read");
                }

                ViewBag.DataView = new Dictionary<string, object>(){
                    {"title","Praselection"}
                    };
                return View("Preselection/AddCandidate");
            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }

        //------------------------------------------ VIEW EDIT CANDIDATE ---------------------------------------------------
        [Route("candidate/praselection/update/candidate/{id?}")]
        public ActionResult CandidateEdit(string id = null)
        {
            try
            {
                if (id == null) return Redirect("~/candidate/praselection");

                int CandidateId = Convert.ToInt16(id);
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == CandidateId);

                if (DataCandidate == null) return Redirect("~/candidate/praselection");

                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","praselection"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 2 || d.ID == 1 || d.ID == 3 ).ToList() }
                };

                return View("Preselection/EditCandidate", DataCandidate);
            }
            catch
            {
                return Redirect("~/auth/error");
            }
        }



        //------------------------------------------ Process Edit Data Candidate -------------------------------------------------
        [Route("candidate/praselection/update/candidate/process")]
        public ActionResult CandidateEdit(CandidateDTO Data, HttpPostedFileBase Pict = null, HttpPostedFileBase Cv = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                     var Edit = Manage_CandidateDTO.EditCandidate(Data, Pict, Cv);
                        if (Edit > 0)
                        {
                            if (TempData["message"] == null)
                            {
                                TempData.Add("message", "Candidate Update successfully");
                                TempData.Add("type", "success");
                                UserLogingUtils.SaveLoggingUserActivity("Edit Candidate" + Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID));
                            }
                        }
                        else
                        {
                            if (TempData["message"] == null)
                            {
                                TempData.Add("message", "Candidate failed to Update");
                                TempData.Add("type", "warning");
                            }
                        }
                    
                     
                        
                    
                    return Redirect("~/candidate/praselection/read/praselected/");
                }
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID);
                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","praselection"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 2 || d.ID == 1 || d.ID == 3).ToList() }
                };

                return View("Preselection/EditCandidate", DataCandidate);
            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }

        //------------------------------------------- DELETE CANDIDATE ----------------------------------------------
    
        [Route("candidate/praselection/delete/{id?}")]
        //take 'id' from Table Candidate Selection History not Table Candidate
        public ActionResult DeleteCandidate(int id) {
            try
            {
                var DelCandi = Manage_CandidateDTO.DeleteDataCandidate(id);

                    if (DelCandi > 0)
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate Delete successfully");
                            TempData.Add("type", "success");
                        }
                    }
                    else
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate Delete to Update");
                            TempData.Add("type", "warning");
                        }
                    }
                
                return Redirect("~/candidate/praselection/read");
            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }


         // ======================================================================= PRA-SELECTED ==============================================================================
         // ======================================================================= PRA-SELECTED ==============================================================================
        [Route("candidate/praselection/read/praselected/{i?}")]
        public ActionResult CandidatePraselected(string i = null)
        {
            //try
            //{
            //    //---------------------------- prepare data candidate for show in view --------------
            //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
            //not  : data in this view especialy for candidate where state_id is 2 (state in step preselection)

            //formula pagination
            int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]);
            float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh => sh.VIEWS_INFORMATION == "YES" && sh.CANDIDATE_STATE == 2).ToList().Count();
            int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
            int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));

            List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory()
            .Where(d => d.VIEWS_INFORMATION == "YES" && d.CANDIDATE_STATE == 2)
            .Skip(idx)
            .Take(perPage)
            .ToList();

            //---------------------------- prepare data viewbag --------------------


            //============================ process searchng ============================
            if (Request["filter"] != null)
            {
                string Position = Request["POSITION"];
                int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                string Keyword = Request["Keyword"];
                string DataPerPage = Request["DataPerPage"];
                int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);
                if (StateId != 0 && (Position == "all" && Keyword == ""))
                {
                    ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).Skip(idx).Take(5).ToList();
                    DataCount = ListCandidate.Count();
                    PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));

                }
                if (Position != "all" && (StateId == 0 && Keyword == ""))
                {
                    ListCandidate = ListCandidate.Where(d =>
                    d.CANDIDATE_APPLIED_POSITION == Position ||
                    d.CANDIDATE_SUITABLE_POSITION == Position &&
                    (d.CANDIDATE_STATE == 2 && d.VIEWS_INFORMATION == "YES")).Skip(idx).Take(5).ToList();
                    DataCount = ListCandidate.Count();
                    PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                }
                if (Keyword != "" && (StateId == 0 && Position == "all"))
                {
                    ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_EMAIL.Contains(Keyword) ||
                        d.CANDIDATE_NAME.Contains(Keyword) ||
                        d.CANDIDATE_PHONE.Contains(Keyword) &&
                        (d.CANDIDATE_STATE == 2 && d.VIEWS_INFORMATION == "YES")).Skip(idx).Take(5).ToList();
                    DataCount = ListCandidate.Count();
                    PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                }
                if (DataPerPage != "" && (StateId == 0 && Position == "all" && Keyword == ""))
                {
                    perPage = dt;
                    ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                d.CANDIDATE_STATE == 2 && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                    PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    Session.Add("DataPage", dt);
                }
                else
                {
                    ListCandidate = ListCandidate.Where(d =>
                     d.CANDIDATE_APPLIED_POSITION == Position ||
                     d.CANDIDATE_SUITABLE_POSITION == Position ||
                     d.CANDIDATE_STATE == StateId ||
                     d.CANDIDATE_EMAIL.Contains(Keyword) ||
                     d.CANDIDATE_NAME.Contains(Keyword) ||
                     d.CANDIDATE_PHONE.Contains(Keyword) &&
                     (d.CANDIDATE_STATE == 2 && d.VIEWS_INFORMATION == "YES")).Skip(idx).Take(dt).ToList();
                    DataCount = ListCandidate.Count();
                    PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                }
            }
            //============================ end process searchng ============================
            

            ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Praselection"},
                    {"ListPosition",Manage_JobPositionDTO.GetData()},
                    {"PageCount",PageCount},
                    };
            
            //return view
            return View("Preselection/CandidatePraselected", ListCandidate);
            //}
            //catch (Exception)
            //{
            //    return Redirect("~/auth/error");
            //}
        }

        //------------------------------------------ VIEW EDIT CANDIDATE PRASELECTED ---------------------------------------------------
        [Route("candidate/praselection/update/praselected/next/{id?}")]
        public ActionResult EditPraselected(string id = null)
        {
            try
            {

                if (id == null) return Redirect("~/candidate/praselection");
                int CandidateId = Convert.ToInt32(id);
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == CandidateId);

                if (DataCandidate == null) return Redirect("~/candidate/praselection");

                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","praselection"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 2 || d.ID == 3 || d.ID == 4 ).ToList() }
                };

                return View("Preselection/EditCandidate", DataCandidate);
            }
            catch
            {
                return Redirect("~/auth/error");
            }
        }



        //------------------------------------------ Process Edit Data Praselected -------------------------------------------------
        [Route("candidate/praselection/update/praselected/next/process")]
        public ActionResult PraselectedEdit(CandidateDTO Data, HttpPostedFileBase Pict = null, HttpPostedFileBase Cv = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Edit = Manage_CandidateDTO.EditCandidate(Data, Pict, Cv);

                    if (Edit > 0)
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate Update successfully");
                            TempData.Add("type", "success");
                            UserLogingUtils.SaveLoggingUserActivity("Edit Candidate" + Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID));
                        }
                    }
                    else
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate failed to Update");
                            TempData.Add("type", "warning");
                        }
                    }




                    return Redirect("~/candidate/praselection/read/praselected");
                }
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID);
                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","praselection"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 2 || d.ID == 1 || d.ID == 3).ToList() }
                };

                return View("Preselection/EditPraselected", DataCandidate);
            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }


        //************************************************* JOB EXPERIENCE OF CANDIDATE *****************************************************


        //----------------------------------------------------- process add new job experience ------------------------------
        [Route("candidate/praselection/create/jobExp")]
        public ActionResult JobExpAdd(CandidateJobExperienceDTO NewJobExp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DBEntities db = new DBEntities())
                    {
                        var ProcessAdd = Manage_CandidateJobExperienceDTO.AddData(NewJobExp);

                        if (ProcessAdd > 0)
                        {
                            if (TempData.Peek("message") == null)
                            {
                                TempData.Remove("message");
                                TempData.Remove("type");
                            TempData.Add("message", "Candidate new job experience added successfully");
                            TempData.Add("type", "success");
                            UserLogingUtils.SaveLoggingUserActivity("add job experience Candidate " + NewJobExp.CANDIDATE_ID + " Job Experience in " + NewJobExp.COMPANY_NAME);
                            }
                        }
                        else
                        {
                            if (TempData.Peek("message") == null)
                            {
                                TempData.Remove("message");
                                TempData.Remove("type");
                            TempData.Add("message", "Candidate new job experience failed to add");
                            TempData.Add("type", "warning");
                            }
                        }
                        return Redirect("~/candidate/praselection/read/detailcandidate/" + NewJobExp.CANDIDATE_ID);
                    }
                }

                TempData.Add("message", "Candidate new job experience failed to add please complete form add");
                TempData.Add("type", "danger");
                return Redirect("~/candidate/praselection/read/detailcandidate/" + NewJobExp.CANDIDATE_ID);
            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }

        //----------------------------------------------------------- view edit job exp ------------------------------------
        [Route("candidate/praselection/update/jobExp/{id?}")]
        public ActionResult JobExp(string id = null)
        {
            try
            {
                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","Praselection"}
                };
                int JobExpId = Convert.ToInt16(id);
                CandidateJobExperienceDTO Data = Manage_CandidateJobExperienceDTO.GetData().FirstOrDefault(d => d.ID == JobExpId);
                return View("Preselection/EditJobExp", Data);
            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }

        //----------------------------------------------------------- process edit job exp ---------------------------------
        [Route("candidate/praselection/update/jobExp/process")]
        public ActionResult JobExpEdit(CandidateJobExperienceDTO NewJobExp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (DBEntities db = new DBEntities())
                    {
                        var ProcessEdit = Manage_CandidateJobExperienceDTO.EditData(NewJobExp);

                        if (ProcessEdit > 0)
                        {
                            if (TempData.Peek("message") == null)
                            {
                                TempData.Remove("message");
                                TempData.Remove("type");
                            TempData.Add("message", "Candidate job experience edited successfully");
                            TempData.Add("type", "success");
                            UserLogingUtils.SaveLoggingUserActivity("edit job experience Candidate " + NewJobExp.CANDIDATE_ID + " Job Experience in " + NewJobExp.COMPANY_NAME);
                            }
                        }
                        else
                        {
                            if (TempData.Peek("message") == null)
                            {
                                TempData.Remove("message");
                                TempData.Remove("type");
                            TempData.Add("message", "Candidate job experience failed to edit");
                            TempData.Add("type", "warning");
                            }
                        }
                        return Redirect("~/candidate/praselection/read/detailcandidate/" + NewJobExp.CANDIDATE_ID);
                    }
                }

                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","Praselection"}
                };
                CandidateJobExperienceDTO Data = Manage_CandidateJobExperienceDTO.GetData().FirstOrDefault(d => d.ID == NewJobExp.ID);
                return View("Preselection/EditJobExp", Data);

            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }











        //################################################# CANDIDATE CALL ###############################################################

        //------------------------------------------------- View for candidate call -----------------------------------------------------
        [Route("candidate/call/read/{i?}")]
        public ActionResult CandidateCall(string i = null)
        {
            try
            {
                //---------------------------- prepare data candidate for show in view --------------
                //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
                //note : data in this view especialy for candidate where state_id is 2(call) or 18(called) (state in step call)

                int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]); ;
                float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh => sh.VIEWS_INFORMATION == "YES" && sh.CANDIDATE_STATE == 4).ToList().Count();
                int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));
                List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d => d.VIEWS_INFORMATION == "YES" &&
                d.CANDIDATE_STATE == 4).Skip(idx).Take(perPage).ToList();

                //prepare vew bag
                //---------------------------- prepare data viewbag --------------------

                //============================ process searchng ============================
                if (Request["filter"] != null)
                {
                    string Position = Request["POSITION"];
                    int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                    string Keyword = Request["Keyword"];
                    string DataPerPage = Request["DataPerPage"];
                    int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);
                    if (StateId != 0 && (Position == "all" && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Position != "all" && (StateId == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_APPLIED_POSITION == Position ||
                        d.CANDIDATE_SUITABLE_POSITION == Position &&
                        (d.CANDIDATE_STATE == 4) && d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Keyword != "" && (StateId == 0 && Position == "all"))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_EMAIL.Contains(Keyword) ||
                        d.CANDIDATE_NAME.Contains(Keyword) ||
                        d.CANDIDATE_PHONE.Contains(Keyword) &&
                            (d.CANDIDATE_STATE == 4) && d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (DataPerPage != "" && (StateId == 0 && Position == "all" && Keyword == ""))
                    {
                        perPage = dt;
                        ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                d.CANDIDATE_STATE == 4 && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                        Session.Add("DataPage", dt);

                    }
                    else
                    {
                        ListCandidate = ListCandidate.Where(d =>
                         d.CANDIDATE_APPLIED_POSITION == Position ||
                         d.CANDIDATE_SUITABLE_POSITION == Position ||
                         d.CANDIDATE_STATE == StateId ||
                         d.CANDIDATE_EMAIL.Contains(Keyword) ||
                         d.CANDIDATE_NAME.Contains(Keyword) ||
                         d.CANDIDATE_PHONE.Contains(Keyword) &&
                         (d.CANDIDATE_STATE == 4 ) && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                }
                //============================ end process searchng ============================

                ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Call"},
                    {"ListPosition",Manage_JobPositionDTO.GetData()},
                    {"PageCount",PageCount}
                    };

                return View("Call/Call", ListCandidate);
            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }




        //---------------------------------------------------------- View next call to called ----------------------------------------
        [Route("candidate/call/update/next/{id?}")]
        public ActionResult CallNext(string id = null)
        {
            try
            {
                if (id == null) return Redirect("~/candidate/call");

                int CandidateId = Convert.ToInt16(id);
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == CandidateId);

                if (DataCandidate == null) return Redirect("~/candidate/praselection");

                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","Call"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 4  || d.ID == 5 || d.ID == 6 ).ToList() }
                };

                return View("Call/EditCandidateCall", DataCandidate);
            }
            catch
            {
                return Redirect("~/auth/error");
            }
        }

        //------------------------------------------ Process call next to called -------------------------------------------------
        [Route("candidate/call/update/next/process")]
        public ActionResult CandidateCallNext(CandidateDTO Data, HttpPostedFileBase Pict = null, HttpPostedFileBase Cv = null)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int ProcessEdit = Manage_CandidateDTO.EditCandidate(Data, Pict, Cv);
                if (ProcessEdit > 0)
                {
                     if(TempData["message"] == null)
                     {
                            TempData.Add("message", "Candidate Update successfully");
                            TempData.Add("type", "success");
                            UserLogingUtils.SaveLoggingUserActivity("Edit Candidate" + Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID));
                            //check state candidat      e before updated
                     }
                }
                else
                {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate failed to Update");
                            TempData.Add("type", "warning");
                        }
                }

                return Redirect("~/candidate/call/read");
            }

            CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID);

            ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","Call"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 4 ||  d.ID == 5 || d.ID == 6).ToList() }
                };
            return View("Call/EditCandidateCall", DataCandidate);
            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }

        //------------------------------------------------- View for candidate !!! CALLED !!! ---------------------------------------------
        [Route("candidate/call/read/called/{i?}")]
        public ActionResult CandidateCalled(string i = null)
        {
            try
            {
                //---------------------------- prepare data candidate for show in view --------------
                //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
                //note : data in this view especialy for candidate where state_id is 2(call) or 18(called) (state in step call)

                int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]);
                float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh =>sh.VIEWS_INFORMATION == "YES" && ( sh.CANDIDATE_STATE == 5 || sh.CANDIDATE_STATE == 6)).ToList().Count();
                int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));
                List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d => d.VIEWS_INFORMATION == "YES" &&
                (d.CANDIDATE_STATE == 5 || d.CANDIDATE_STATE == 6)).Skip(idx).Take(perPage).ToList();


                //============================ process searchng ============================
                if (Request["filter"] != null)
                {
                    string Position = Request["POSITION"];
                    int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                    string Keyword = Request["Keyword"];
                    string DataPerPage = Request["DataPerPage"];
                    int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);

                    if (StateId != 0 && (Position == "all" && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Position != "all" && (StateId == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_APPLIED_POSITION == Position ||
                        d.CANDIDATE_SUITABLE_POSITION == Position &&
                        (d.CANDIDATE_STATE == 5 || d.CANDIDATE_STATE == 6) &&
                        d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Keyword != "" && (StateId == 0 && Position == "all"))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_EMAIL.Contains(Keyword) ||
                        d.CANDIDATE_NAME.Contains(Keyword) ||
                        d.CANDIDATE_PHONE.Contains(Keyword) &&
                        (d.CANDIDATE_STATE == 5 || d.CANDIDATE_STATE == 6) &&
                        d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (DataPerPage != "" && (StateId == 0 && Position == "all" && Keyword == ""))
                    {
                        perPage = dt;
                        ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                       (d.CANDIDATE_STATE == 5 || d.CANDIDATE_STATE == 6) && 
                        d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                        Session.Add("DataPage", dt);
                    }
                    else
                    {
                        ListCandidate = ListCandidate.Where(d =>
                         d.CANDIDATE_APPLIED_POSITION == Position ||
                         d.CANDIDATE_SUITABLE_POSITION == Position ||
                         d.CANDIDATE_STATE == StateId ||
                         d.CANDIDATE_EMAIL.Contains(Keyword) ||
                         d.CANDIDATE_NAME.Contains(Keyword) ||
                         d.CANDIDATE_PHONE.Contains(Keyword) &&
                         (d.CANDIDATE_STATE == 5 || d.CANDIDATE_STATE == 6) && 
                         d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                }
                //============================ end process searchng ============================

                //prepare vew bag
                //---------------------------- prepare data viewbag --------------------
                ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Call"},
                    {"ListPosition",Manage_JobPositionDTO.GetData()},
                    {"ListState", Manage_StateCandidateDTO.GetData().Where(m => m.ID == 5 || m.ID == 6).ToList()},
                    {"PageCount",PageCount}
                    };

                return View("Call/Called", ListCandidate);

            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }

        //---------------------------------------------------------- View next called to Interview ----------------------------------------
        [Route("candidate/call/update/called/next/{id?}")]
        public ActionResult CalledNext(string id = null)
        {
            try
            {
                if (id == null) return Redirect("~/candidate/call");

                int CandidateId = Convert.ToInt16(id);
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == CandidateId);
                
                if (DataCandidate == null) return Redirect("~/candidate/praselection");

                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","Call"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 5 ||  d.ID == 6 || d.ID == 7).ToList() }
                };

                return View("Call/EditCandidateCalled", DataCandidate);
            }
            catch
            {
                return Redirect("~/auth/error");
            }
        }

        //------------------------------------------ Process called next to interview -------------------------------------------------
        [Route("candidate/call/update/called/next/process")]
        public ActionResult CandidateCalledNext(CandidateDTO Data, HttpPostedFileBase Pict = null, HttpPostedFileBase Cv = null)
        {
            try
            {
                if (ModelState.IsValid)
            {
                var ProcessEdit = Manage_CandidateDTO.EditCandidate(Data, Pict, Cv);

                if (ProcessEdit > 0)
                {
                        if(TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate Update successfully");
                            TempData.Add("type", "success");
                            UserLogingUtils.SaveLoggingUserActivity("Edit Candidate" + Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID));
                        }
                }
                else
                {
                        if(TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate failed to Update");
                            TempData.Add("type", "warning");
                        }
                }

                    return Redirect("~/candidate/call/read/called");
            }
            CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID);
            DataCandidate.CANDIDATE_INTERVIEW_DATE = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().FirstOrDefault(d => d.CANDIDATE_STATE == 8 && d.CANDIDATE_ID == Data.ID).CANDIDATE_INTERVIEW_DATE;

            ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","Call"},
                };

            return View("Call/EditCandidateCalled", DataCandidate);
            }
            catch
            {
                return Redirect("~/auth/error");
            }
        }






        //************************************************************ Candidate INTERVIEW ************************************************************

        //------------------------------------------------------------ candidate interview -----------------------------------------------------------

        [Route("candidate/interview/read/{i?}")]
        public ActionResult CandidateInterview(string i = null)
        {
            try
            {
                //---------------------------- prepare data candidate for show in view --------------
                //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
                //note : data in this view especialy for candidate where state_id is 19(interview process)


                int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]);
                float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh => sh.CANDIDATE_STATE == 7 && sh.VIEWS_INFORMATION == "YES").ToList().Count();
                int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));
                List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                d.CANDIDATE_STATE == 7 && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();


                //============================ process searchng ============================
                if (Request["filter"] != null)
                {
                    string Position = Request["POSITION"];
                    int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                    string Keyword = Request["Keyword"];
                    string DataPerPage = Request["DataPerPage"];
                    int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);

                    if (StateId != 0 && (Position == "all" && Keyword == "" && DataPerPage == ""))
                    {
                        ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Position != "all" && (StateId == 0 && Keyword == "" && DataPerPage == ""))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_APPLIED_POSITION == Position ||
                        d.CANDIDATE_SUITABLE_POSITION == Position &&
                        (d.CANDIDATE_STATE == 7) && 
                        d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Keyword != "" && (StateId == 0 && Position == "all" && DataPerPage == ""))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_EMAIL.Contains(Keyword) ||
                        d.CANDIDATE_NAME.Contains(Keyword) ||
                        d.CANDIDATE_PHONE.Contains(Keyword) &&
                       (d.CANDIDATE_STATE == 7) &&
                        d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (DataPerPage != "" && (StateId == 0 && Position == "all" && Keyword == ""))
                    {
                        perPage = dt;
                        ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                d.CANDIDATE_STATE == 17).Skip(idx).Take(perPage).ToList();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                        Session.Add("DataPage", dt);

                    }

                    else
                    {
                        ListCandidate = ListCandidate.Where(d =>
                         d.CANDIDATE_APPLIED_POSITION == Position ||
                         d.CANDIDATE_SUITABLE_POSITION == Position ||
                         d.CANDIDATE_STATE == StateId ||
                         d.CANDIDATE_EMAIL.Contains(Keyword) ||
                         d.CANDIDATE_NAME.Contains(Keyword) ||
                         d.CANDIDATE_PHONE.Contains(Keyword) &&
                         (d.CANDIDATE_STATE == 7) && 
                         d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                }
                //============================ end process searchng ============================

                //prepare vew bag
                //---------------------------- prepare data viewbag --------------------
                ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Interview"},
                    {"ListPosition",Manage_JobPositionDTO.GetData()},
                    {"PageCount",PageCount}
                    };

                return View("Interview/Interview", ListCandidate);

            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }

        //---------------------------------------------------------- View edit interview to interviewed ----------------------------------------
        [Route("candidate/interview/update/next/{id?}")]
        public ActionResult InterviewNext(string id = null)
        {
            try
            {
                if (id == null) return Redirect("~/candidate/call");

                int CandidateId = Convert.ToInt16(id);
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == CandidateId);

                if (DataCandidate == null) return Redirect("~/candidate/praselection");

                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","Interview"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where(d => d.ID == 7 || d.ID == 8 || d.ID == 9 || d.ID == 10).ToList()}
                };

                return View("Interview/EditCandidateInterview", DataCandidate);
            }
            catch
            {
                return Redirect("~/auth/error");
            }
        }

        //------------------------------------------ Process interview next to interviewed -------------------------------------------------
        [Route("candidate/interview/update/next/process")]
        public ActionResult CandidateInterviewNext(CandidateDTO Data, HttpPostedFileBase Pict = null, HttpPostedFileBase Cv = null)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    int ProcessEdit = Manage_CandidateDTO.EditCandidate(Data, Pict, Cv);

                    if (ProcessEdit > 0)
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate Update successfully");
                            TempData.Add("type", "success");
                            UserLogingUtils.SaveLoggingUserActivity("Edit Candidate" + Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID));
                            //check state candidate before updated
                        }

                    }
                    else
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate failed to Update");
                            TempData.Add("type", "warning");
                        }
                    }

                    return Redirect("~/candidate/interview/read");
                }

                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","interview"},
                };
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID);
                //TempData.Add("message", "Candidate failed to Update, please complete form edit");
                //TempData.Add("type", "danger");
                return View("Interview/EditCandidateInterview", DataCandidate);
            }


        
            catch
            {
                return Redirect("~/auth/error");
            }
        }


        //------------------------------------------------------------ view candidate interviewed -----------------------------------------

        [Route("candidate/interview/read/interviewed/{i?}")]
        public ActionResult CandidateInterviewed(string i = null)
        {
            try
            {
                //---------------------------- prepare data candidate for show in view --------------
                //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
                //note : data in this view especialy for candidate where state_id is 15(hold), 16(pass), 17(drop)

                int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]);
                float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh => (sh.CANDIDATE_STATE == 9 || sh.CANDIDATE_STATE == 10) && sh.VIEWS_INFORMATION == "YES").ToList().Count();
                int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));
                List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                ( d.CANDIDATE_STATE == 9 || d.CANDIDATE_STATE == 10) && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                //prepare vew bag


                //============================ process searchng ============================
                if (Request["filter"] != null)
                {
                    string Position = Request["POSITION"];
                    int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                    string Keyword = Request["Keyword"];
                    string DataPerPage = Request["DataPerPage"];
                    int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);

                    if (StateId != 0 && (Position == "all" && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Position != "all" && (StateId == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_APPLIED_POSITION == Position ||
                        d.CANDIDATE_SUITABLE_POSITION == Position &&
                        (d.CANDIDATE_STATE == 9 || d.CANDIDATE_STATE == 10) && 
                        d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Keyword != "" && (StateId == 0 && Position == "all"))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_EMAIL.Contains(Keyword) ||
                        d.CANDIDATE_NAME.Contains(Keyword) ||
                        d.CANDIDATE_PHONE.Contains(Keyword) &&
                       (d.CANDIDATE_STATE == 9 || d.CANDIDATE_STATE == 10) && d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (DataPerPage != "" && (StateId == 0 && Position == "all" && Keyword == ""))
                    {
                        perPage = dt;
                        ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                        (d.CANDIDATE_STATE == 9 || d.CANDIDATE_STATE == 10) 
                        && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                        Session.Add("DataPage", dt);
                    }
                    else
                    {
                        ListCandidate = ListCandidate.Where(d =>
                         d.CANDIDATE_APPLIED_POSITION == Position ||
                         d.CANDIDATE_SUITABLE_POSITION == Position ||
                         d.CANDIDATE_STATE == StateId ||
                         d.CANDIDATE_EMAIL.Contains(Keyword) ||
                         d.CANDIDATE_NAME.Contains(Keyword) ||
                         d.CANDIDATE_PHONE.Contains(Keyword) &&
                         (d.CANDIDATE_STATE == 9 || d.CANDIDATE_STATE == 10) && 
                         d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                }
                //============================ end process searchng ============================

                //---------------------------- prepare data viewbag --------------------
                ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Interview"},
                    {"ListPosition",Manage_JobPositionDTO.GetData()},
                    {"ListState", Manage_StateCandidateDTO.GetData().Where(s => s.ID == 9 || s.ID == 10)},
                    {"PageCount",PageCount}
                };

                return View("Interview/Interviewed", ListCandidate);

            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }


        //---------------------------------------------------------- View edit interviewed  ----------------------------------------
        [Route("candidate/interview/update/interviewed/next/{id?}")]
        public ActionResult InterviewedNext(string id = null)
        {
            try
            {
                if (id == null) return Redirect("~/candidate/call");

                int CandidateId = Convert.ToInt16(id);
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == CandidateId);

                if (DataCandidate == null) return Redirect("~/candidate/praselection");

                ViewBag.DataView = new Dictionary<string, object>()
                {
                    {"title","Interview"},
                    {"ListState",Manage_StateCandidateDTO.GetData().Where( d=> d.ID == 8 || d.ID == 9 || d.ID == 10 ).ToList()}
                };

                return View("Interview/EditCandidateInterviewed", DataCandidate);
            }
            catch
            {
                return Redirect("~/auth/error");
            }
        }

        //------------------------------------------ Process interviewed edit -------------------------------------------------
        [Route("candidate/interview/update/interviewed/next/process")]
        public ActionResult CandidateInterviewedNext(CandidateDTO Data, HttpPostedFileBase Pict = null, HttpPostedFileBase Cv = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int ProcessEdit = Manage_CandidateDTO.EditCandidate(Data, Pict, Cv);

                    if (ProcessEdit > 0)
                    {
                        if(TempData["message"] == null)
                        {
                        TempData.Add("message", "Candidate Update successfully");
                        TempData.Add("type", "success");
                        UserLogingUtils.SaveLoggingUserActivity("Edit Candidate" + Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID));
                        }
                    }
                    else
                    {
                        if(TempData["message"] == null)
                        {
                        TempData.Add("message", "Candidate failed to Update");
                        TempData.Add("type", "warning");
                        }
                    }

                    return Redirect("~/candidate/interview/read/interviewed");
                }
                CandidateDTO DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == Data.ID);
                TempData.Add("message", "Candidate failed to Update, please complete form edit");
                TempData.Add("type", "danger");

                return View("Interview/EditCandidateInterviewed", DataCandidate);
            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }







        //**********************************************************  SUGGEST   *******************************************************


        //---------------------------------------------------------- view for SUGGEST -------------------------------------------------
        [Route("candidate/suggestion/read/{i?}")]
        public ActionResult CandidateSuggestion(string i = null)
        {
            try
            {
                //---------------------------- prepare data candidate for show in view --------------
                //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
                //note : data in this view especialy for candidate where state_id is 15(hold), 16(pass), 17(drop)


                int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]);
                float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh =>  sh.CANDIDATE_STATE == 8 && sh.VIEWS_INFORMATION == "YES").ToList().Count();
                int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));
                List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                d.CANDIDATE_STATE == 8 && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();


                //============================ process searchng ============================
                if (Request["filter"] != null)
                {
                    string Position = Request["POSITION"];
                    int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                    string Keyword = Request["Keyword"];
                    string DataPerPage = Request["DataPerPage"];
                    int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);

                    if (StateId != 0 && (Position == "all" && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Position != "all" && (StateId == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_APPLIED_POSITION == Position ||
                        d.CANDIDATE_SUITABLE_POSITION == Position &&
                        (d.CANDIDATE_STATE == 8) && d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Keyword != "" && (StateId == 0 && Position == "all"))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_EMAIL.Contains(Keyword) ||
                        d.CANDIDATE_NAME.Contains(Keyword) ||
                        d.CANDIDATE_PHONE.Contains(Keyword) &&
                       (d.CANDIDATE_STATE == 8) && d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (DataPerPage != "")
                    {
                        perPage = dt;
                        ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d => d.CANDIDATE_STATE == 8 && 
                        d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        DataCount = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d => d.CANDIDATE_STATE == 8 && 
                        d.VIEWS_INFORMATION == "YES").ToList().Count;
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));

                        Session.Add("DataPage", dt);

                    }
                    else
                    {
                        ListCandidate = ListCandidate.Where(d =>
                         d.CANDIDATE_APPLIED_POSITION == Position ||
                         d.CANDIDATE_SUITABLE_POSITION == Position ||
                         d.CANDIDATE_STATE == StateId ||
                         d.CANDIDATE_EMAIL.Contains(Keyword) ||
                         d.CANDIDATE_NAME.Contains(Keyword) ||
                         d.CANDIDATE_PHONE.Contains(Keyword) &&
                         (d.CANDIDATE_STATE == 8) && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                }
                //============================ end process searchng ============================
                //ViewBag.DataView = new Dictionary<string, object>()
                //{
                //    {"title","Delivery" },
                //    {"PageCount", PageCount}
                //};

                //prepare vew bag
                //---------------------------- prepare data viewbag --------------------
                ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Suggestion"},
                    {"ListPosition",Manage_JobPositionDTO.GetData()},
                    {"PageCount",PageCount},
                    {"ListState", Manage_StateCandidateDTO.GetData().Where(s => s.ID == 11 || s.ID == 12) }
                    };
                return View("Suggestion/Suggestion", ListCandidate);

            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }

        //-------------------------------------------- process suggestion next -------------------------------------------------------
        [Route("candidate/suggestion/create/next")]
        public ActionResult SuggestNext(DeliveryHistoryDTO data)
        {
            try
            {
                int ProcessEdit;
                using (DBEntities db = new DBEntities())
            {
                    
                var Candidate = db.TB_CANDIDATE.FirstOrDefault(c => c.ID == data.CANDIDATE_ID);
                Candidate.CANDIDATE_STATE_ID = data.CANDIDATE_STATE;
                    
                     ProcessEdit = db.SaveChanges();
                //process add selection history
                //preparedata pic
                UserDTO DataPic = (UserDTO)Session["UserLogin"];
                
                var ProcessAddSelectionHistory = Manage_CandidateSelectionHistoryDTO.AddData(new CandidateSelectionHistoryDTO
                {
                    CANDIDATE_ID = data.CANDIDATE_ID,
                    PIC_ID = DataPic.USER_ID,
                    CANDIDATE_APPLIED_POSITION = Candidate.POSITION,
                    CANDIDATE_SUITABLE_POSITION = Candidate.SUITABLE_POSITION,
                    CANDIDATE_SOURCE = Candidate.SOURCE,
                    CANDIDATE_STATE = data.CANDIDATE_STATE,
                    CANDIDATE_EXPECTED_SALARY = Candidate.EXPECTED_SALARY,
                    PROCESS_DATE = DateTime.Now,
                    NOTES = data.NOTE,
                    CANDIDATE_INTERVIEW_DATE = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().FirstOrDefault(d => d.CANDIDATE_ID == data.CANDIDATE_ID && d.CANDIDATE_STATE == 8).CANDIDATE_INTERVIEW_DATE,
                    VIEWS_INFORMATION = "YES",
                    CANDIDATE_CLIENT = data.CLIENT_ID,
                    DELIVERY_ID = data.DELIVERY_ID

                });

                var ProcessAddDelivery = Manage_DeliveryHistoryDTO.AddData(new DeliveryHistoryDTO
                {
                    DELIVERY_ID = data.DELIVERY_ID,
                    CANDIDATE_ID = data.CANDIDATE_ID,
                    CLIENT_ID = data.CLIENT_ID,
                    LAST_PIC = DataPic.USER_ID,
                    CANDIDATE_STATE = data.CANDIDATE_STATE,
                    PROCESS_DATE = DateTime.Now,
                    NOTE = data.NOTE,
                    CANDIDATE_POSITION = Candidate.SUITABLE_POSITION
                });
                    
                    if ( ProcessAddDelivery > 0 && ProcessAddSelectionHistory > 0 || (ProcessEdit > 0))
                    {
                        if(TempData["message"] == null)
                        {
                        TempData.Add("message", "Candidate Update successfully");
                        TempData.Add("type", "success");
                        UserLogingUtils.SaveLoggingUserActivity("edit suggest state of candidate id " + data.CANDIDATE_ID);
                        }
                    }
                    else
                    {
                        if(TempData["message"] == null)
                        {
                        TempData.Add("message", "Candidate failed to Update");
                        TempData.Add("type", "warning");
                        }
                    }

                    return Redirect("~/candidate/suggestion/read/suggested");
            }
                
            }
            catch (Exception)
            {
               return Redirect("~/auth/error");
            }
        }

        //=========================================================== SUGGEST CANDIDATE ==========================================================

        [Route("candidate/suggestion/read/suggested/{i?}")]
        public ActionResult SuggestedCandidate(string i = null)
        {
            try
            {
                //---------------------------- prepare data candidate for show in view --------------
                //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
                //note : data in this view especialy for candidate where state_id is 14(offering or 6(sent to client))

                int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]);
                float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh => (sh.CANDIDATE_STATE == 11 || sh.CANDIDATE_STATE == 14 || sh.CANDIDATE_STATE == 12 || sh.CANDIDATE_STATE == 13) && sh.VIEWS_INFORMATION == "YES" ).ToList().Count();
                int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));
                List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(sh => (sh.CANDIDATE_STATE == 11 || sh.CANDIDATE_STATE == 14 || sh.CANDIDATE_STATE == 12 || sh.CANDIDATE_STATE == 13) && sh.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();


                //============================ process searchng ============================
                if (Request["filter"] != null)
                {
                    int Client = Convert.ToInt16(Request["CLIENT"]);
                    string Position = Request["CLIENT"];
                    int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                    string Keyword = Request["Keyword"];
                    string DataPerPage = Request["DataPerPage"];
                    int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);

                    if (StateId != 0 && (Client == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Client != 0 && (StateId == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_CLIENT == Client  &&
                        (d.CANDIDATE_STATE == 11 || d.CANDIDATE_STATE == 12 || d.CANDIDATE_STATE == 13 || d.CANDIDATE_STATE == 14) &&
                        d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Keyword != "" && (StateId == 0 && Client == 0))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_EMAIL.Contains(Keyword) ||
                        d.CANDIDATE_NAME.Contains(Keyword) ||
                        d.CANDIDATE_PHONE.Contains(Keyword) &&
                       (d.CANDIDATE_STATE == 11 || d.CANDIDATE_STATE == 12 || d.CANDIDATE_STATE == 13 || d.CANDIDATE_STATE == 14) &&
                        d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (DataPerPage != "")
                    {
                        perPage = dt;
                        ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                        (d.CANDIDATE_STATE == 11 || d.CANDIDATE_STATE == 12 || d.CANDIDATE_STATE == 13 || d.CANDIDATE_STATE == 14) &&
                        d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                        Session.Add("DataPage", dt);

                    }
                    else
                    {
                        ListCandidate = ListCandidate.Where(d =>
                         d.CANDIDATE_CLIENT == Client ||
                         d.CANDIDATE_STATE == StateId ||
                         d.CANDIDATE_EMAIL.Contains(Keyword) ||
                         d.CANDIDATE_NAME.Contains(Keyword) ||
                         d.CANDIDATE_PHONE.Contains(Keyword) &&
                          (d.CANDIDATE_STATE == 11 || d.CANDIDATE_STATE == 12 || d.CANDIDATE_STATE == 13 || d.CANDIDATE_STATE == 14) &&
                        d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                }
                //============================ end process searchng ============================
                //prepare vew bag
                //---------------------------- prepare data viewbag --------------------
                ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Suggestion"},
                    {"ListClient", Manage_ClientDTO.GetData()},
                    {"PageCount",PageCount}
                    };

                return View("Suggestion/Suggested", ListCandidate);
            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }

        //------------------------------------- candidate Suggested update ---------------------------------------------------
        [Route("candidate/suggestion/update/suggested")]
        public ActionResult SuggestedUpdate(DeliveryHistoryDTO data)
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {

                    

                    var Candidate = db.TB_CANDIDATE.FirstOrDefault(c => c.ID == data.CANDIDATE_ID);
                    Candidate.CANDIDATE_STATE_ID = data.CANDIDATE_STATE;
                    foreach(var item in db.TB_CANDIDATE_SELECTION_HISTORY.Where(m => m.DELIVERY_ID == data.DELIVERY_ID))
                    {
                        item.VIEWS_INFORMATION = "NO";
                    }

                    int ProcessEdit = db.SaveChanges();

                    //process add selection history
                    //preparedata pic
                    UserDTO DataPic = (UserDTO)Session["UserLogin"];

                    var ProcessAddSelectionHistory = Manage_CandidateSelectionHistoryDTO.AddData(new CandidateSelectionHistoryDTO
                    {
                        CANDIDATE_ID = data.CANDIDATE_ID,
                        PIC_ID = DataPic.USER_ID,
                        CANDIDATE_APPLIED_POSITION = Candidate.POSITION,
                        CANDIDATE_SUITABLE_POSITION = Candidate.SUITABLE_POSITION,
                        CANDIDATE_SOURCE = Candidate.SOURCE,
                        CANDIDATE_STATE = data.CANDIDATE_STATE,
                        CANDIDATE_EXPECTED_SALARY = Candidate.EXPECTED_SALARY,
                        PROCESS_DATE = DateTime.Now,
                        NOTES = data.NOTE,
                        CANDIDATE_INTERVIEW_DATE = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().FirstOrDefault(d => d.CANDIDATE_ID == data.CANDIDATE_ID && d.CANDIDATE_STATE == 8).CANDIDATE_INTERVIEW_DATE,
                        VIEWS_INFORMATION = "YES",
                        CANDIDATE_CLIENT = data.CLIENT_ID,
                        DELIVERY_ID = data.DELIVERY_ID

                    });

                    var ProcessAddDelivery = Manage_DeliveryHistoryDTO.AddData(new DeliveryHistoryDTO
                    {
                        DELIVERY_ID = data.DELIVERY_ID,
                        CANDIDATE_ID = data.CANDIDATE_ID,
                        CLIENT_ID = data.CLIENT_ID,
                        LAST_PIC = DataPic.USER_ID,
                        CANDIDATE_STATE = data.CANDIDATE_STATE,
                        PROCESS_DATE = DateTime.Now,
                        NOTE = data.NOTE,
                        CANDIDATE_POSITION = Candidate.SUITABLE_POSITION
                    });

                    if (ProcessEdit > 0 || (ProcessAddSelectionHistory > 0 && ProcessAddDelivery > 0)  )
                    {
                        if(TempData["message"] == null)
                        {
                        TempData.Add("message", "Candidate Update successfully");
                        TempData.Add("type", "success");
                        UserLogingUtils.SaveLoggingUserActivity("edit suggest state of candidate id " + data.CANDIDATE_ID);
                        }
                    }
                    else
                    {
                        if(TempData["message"] == null)
                        {
                        TempData.Add("message", "Candidate failed to Update");
                        TempData.Add("type", "warning");
                        }
                    }

                }
                    return Redirect("~/candidate/suggestion/read/suggested");
            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }


        //=========================================================== DELIVERY CANDIDATE ==========================================================
        [Route("candidate/delivery/read/{i?}")]
        public ActionResult DeliveryCandidate(string i = null)
        {
            try
            {
                //---------------------------- prepare data candidate for show in view --------------
                //note : data candidate from class Manage_CandidateSelectionHistoryDTO method GetDataSelectionHistory
                //note : data in this view especialy for candidate where state_id is 14(offering or 6(sent to client))

                int perPage = Session["DataPage"] == null ? 5 : Convert.ToInt16(Session["DataPage"]);
                float DataCount = db.TB_CANDIDATE_SELECTION_HISTORY.Where(sh => (sh.CANDIDATE_STATE == 15 || sh.CANDIDATE_STATE == 16) && sh.VIEWS_INFORMATION == "YES").ToList().Count();
                int PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                int idx = (i == null ? 0 : (perPage * int.Parse(i) - perPage));
                List<CandidateSelectionHistoryDTO> ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d => (d.CANDIDATE_STATE == 15 || d.CANDIDATE_STATE == 16)&& d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();



                //============================ process searchng ============================
                if (Request["filter"] != null)
                {
                    int ClientId = Convert.ToInt16(Request["CLIENT"]);
                    int StateId = Convert.ToInt16(Request["CANDIDATE_STATE"]);
                    string Keyword = Request["Keyword"];
                    string DataPerPage = Request["DataPerPage"];
                    int dt = DataPerPage == "" ? 5 : Convert.ToInt16(DataPerPage);

                    if (StateId != 0 && (ClientId == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(d => d.CANDIDATE_STATE == StateId).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (ClientId != 0 && (StateId == 0 && Keyword == ""))
                    {
                        ListCandidate = ListCandidate.Where(m => m.CANDIDATE_CLIENT == ClientId && (m.CANDIDATE_STATE == 15 || m.CANDIDATE_STATE == 16) && 
                        m.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (Keyword != "" && (StateId == 0 && ClientId == 0))
                    {
                        ListCandidate = ListCandidate.Where(d =>
                        d.CANDIDATE_EMAIL.Contains(Keyword) ||
                        d.CANDIDATE_NAME.Contains(Keyword) ||
                        d.CANDIDATE_PHONE.Contains(Keyword) &&
                       (d.CANDIDATE_STATE == 15 || d.CANDIDATE_STATE == 16) && d.VIEWS_INFORMATION == "YES").ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                    if (DataPerPage != "")
                    {
                        perPage = dt;
                        ListCandidate = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().Where(d =>
                          (d.CANDIDATE_STATE == 15 || d.CANDIDATE_STATE == 16) && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                        Session.Add("DataPage", dt);

                    }
                    else
                    {
                        ListCandidate = ListCandidate.Where(d =>
                         d.CANDIDATE_CLIENT ==  ClientId ||
                         d.CANDIDATE_STATE == StateId ||
                         d.CANDIDATE_EMAIL.Contains(Keyword) ||
                         d.CANDIDATE_NAME.Contains(Keyword) ||
                         d.CANDIDATE_PHONE.Contains(Keyword) &&
                          (d.CANDIDATE_STATE == 15 || d.CANDIDATE_STATE == 16) && d.VIEWS_INFORMATION == "YES").Skip(idx).Take(perPage).ToList();
                        DataCount = ListCandidate.Count();
                        PageCount = Convert.ToInt16(Math.Ceiling(DataCount / perPage));
                    }
                }
                //============================ end process searchng ============================
                //prepare vew bag
                //---------------------------- prepare data viewbag --------------------
                ViewBag.DataView = new Dictionary<string, object>{
                    {"title","Delivery"},
                    {"ListClient", Manage_ClientDTO.GetData()},
                    {"PageCount",PageCount}
                    };

                return View("Delivery/Delivery", ListCandidate);
            }
            catch (Exception e)
            {
                string msg = e.Message.Replace('\n', ' ') + e.StackTrace.Replace('\n', ' ');
                return Redirect("~/auth/error?msg=" + (ConfigurationManager.AppSettings["env"].ToString().Equals("development") ? msg : " "));
            }
        }

        //------------------------------------- candidate suggestion update ---------------------------------------------------
        [Route("candidate/delivery/update")]
        public ActionResult SuggestionUpdate(DeliveryHistoryDTO data)
        {
            try
            {
                using (DBEntities db = new DBEntities())
                {
                    var Candidate = db.TB_CANDIDATE.FirstOrDefault(c => c.ID == data.CANDIDATE_ID);
                    Candidate.CANDIDATE_STATE_ID = data.CANDIDATE_STATE;
                    var SelHis = db.TB_CANDIDATE_SELECTION_HISTORY.FirstOrDefault(m => m.DELIVERY_ID == data.DELIVERY_ID);
                    SelHis.VIEWS_INFORMATION = "NO";

                    int ProcessEdit = db.SaveChanges();

                    //process add selection history
                    //preparedata pic
                    UserDTO DataPic = (UserDTO)Session["UserLogin"];

                    var ProcessAddSelectionHistory = Manage_CandidateSelectionHistoryDTO.AddData(new CandidateSelectionHistoryDTO
                    {
                        CANDIDATE_ID = data.CANDIDATE_ID,
                        PIC_ID = DataPic.USER_ID,
                        CANDIDATE_APPLIED_POSITION = Candidate.POSITION,
                        CANDIDATE_SUITABLE_POSITION = Candidate.SUITABLE_POSITION,
                        CANDIDATE_SOURCE = Candidate.SOURCE,
                        CANDIDATE_STATE = data.CANDIDATE_STATE,
                        CANDIDATE_EXPECTED_SALARY = Candidate.EXPECTED_SALARY,
                        PROCESS_DATE = DateTime.Now,
                        NOTES = data.NOTE,
                        CANDIDATE_INTERVIEW_DATE = Manage_CandidateSelectionHistoryDTO.GetDataSelectionHistory().FirstOrDefault(d => d.CANDIDATE_ID == data.CANDIDATE_ID && d.CANDIDATE_STATE == 8).CANDIDATE_INTERVIEW_DATE,
                        VIEWS_INFORMATION = "YES",
                        CANDIDATE_CLIENT = data.CLIENT_ID,
                        DELIVERY_ID = data.DELIVERY_ID

                    });

                    var ProcessAddDelivery = Manage_DeliveryHistoryDTO.AddData(new DeliveryHistoryDTO
                    {
                        DELIVERY_ID = data.DELIVERY_ID,
                        CANDIDATE_ID = data.CANDIDATE_ID,
                        CLIENT_ID = data.CLIENT_ID,
                        LAST_PIC = DataPic.USER_ID,
                        CANDIDATE_STATE = data.CANDIDATE_STATE,
                        PROCESS_DATE = DateTime.Now,
                        NOTE = data.NOTE,
                        CANDIDATE_POSITION = Candidate.SUITABLE_POSITION
                    });

                    if (ProcessEdit > 0 || (ProcessAddSelectionHistory > 0 && ProcessAddDelivery > 0))
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate Update successfully");
                            TempData.Add("type", "success");
                            UserLogingUtils.SaveLoggingUserActivity("edit suggest state of candidate id " + data.CANDIDATE_ID);
                        }
                    }
                    else
                    {
                        if (TempData["message"] == null)
                        {
                            TempData.Add("message", "Candidate failed to Update");
                            TempData.Add("type", "warning");
                        }
                    }

                }
                return Redirect("~/candidate/suggestion/read/suggested");
            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }



































    }
}