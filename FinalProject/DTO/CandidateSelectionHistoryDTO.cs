using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinalProject.Models;
using FinalProject.DTO;

namespace FinalProject.DTO
{
    public class CandidateSelectionHistoryDTO
    {

        public int ID { get; set; }
        
        
        public string CANDIDATE_APPLIED_POSITION { get; set; }
        public string CANDIDATE_SUITABLE_POSITION { get; set; }
        
        public int? CANDIDATE_STATE { get; set; }
        public string CANDIDATE_STATE_NAME { get; set; }
        public decimal? CANDIDATE_EXPECTED_SALARY { get; set; }
        public System.DateTime? PROCESS_DATE { get; set; }
        public string NOTES { get; set; }
        public System.DateTime? CANDIDATE_INTERVIEW_DATE { get; set; }
        public int? CANDIDATE_CLIENT { get; set; }
        public System.DateTime? LAST_UPDATE { get; set; }

        public string VIEWS_INFORMATION { get; set; }

        public string DELIVERY_ID { get; set; }

        //data candidate
        public int? CANDIDATE_ID { get; set; }
        public string CANDIDATE_NAME { get; set; }
        public System.DateTime? CANDIDATE_SOURCING_DATE { get; set; }
        public string CANDIDATE_EMAIL { get; set; }
        public string CANDIDATE_PHONE { get; set; }
        public string CANDIDATE_SOURCE { get; set; }
        //public CandidateDTO DataCandidate { get; set; }

        //data PIC
        public int? PIC_ID { get; set; }
        public string PIC_FULL_NAME { get; set; }
        //public UserDTO DataPic { get; set; } 
    }

    //--------------------------------------------------------- Add data ----------------------------------------------------------------
    public class Manage_CandidateSelectionHistoryDTO
    {

        public static int AddData(CandidateSelectionHistoryDTO data)
        {
            using(DBEntities db = new DBEntities())
            {

                db.TB_CANDIDATE_SELECTION_HISTORY.Add(new TB_CANDIDATE_SELECTION_HISTORY
                {
                    CANDIDATE_ID = data.CANDIDATE_ID,
                    PIC_ID = data.PIC_ID,
                    CANDIDATE_APPLIED_POSITION = data.CANDIDATE_APPLIED_POSITION,
                    CANDIDATE_SUITABLE_POSITION = data.CANDIDATE_SUITABLE_POSITION,
                    CANDIDATE_SOURCE = data.CANDIDATE_SOURCE,
                    CANDIDATE_STATE = data.CANDIDATE_STATE,
                    CANDIDATE_EXPECTED_SALARY = data.CANDIDATE_EXPECTED_SALARY,
                    PROCESS_DATE = DateTime.Now,
                    NOTES = data.NOTES,
                    CANDIDATE_INTERVIEW_DATE = data.CANDIDATE_INTERVIEW_DATE,
                    LAST_UPDATE = DateTime.Now,
                    VIEWS_INFORMATION = "YES",
                    CANDIDATE_CLIENT = data.CANDIDATE_CLIENT,
                    DELIVERY_ID = data.DELIVERY_ID
                    
                    
                    
                });

                return db.SaveChanges();
            }
        }

        //---------------------------------------------------------- for get data ----------------------------------------------------
        public static List<CandidateSelectionHistoryDTO> GetDataSelectionHistory()
        {
            using (DBEntities db = new DBEntities())
            {
                List<CandidateSelectionHistoryDTO> Data = db.TB_CANDIDATE_SELECTION_HISTORY.
                    Select(sh => new CandidateSelectionHistoryDTO
                    {
                        ID = sh.ID,
                        CANDIDATE_ID = sh.CANDIDATE_ID,
                        CANDIDATE_APPLIED_POSITION = db.TB_CANDIDATE.FirstOrDefault(d => d.ID == sh.CANDIDATE_ID).POSITION,
                        CANDIDATE_SUITABLE_POSITION = sh.CANDIDATE_SUITABLE_POSITION,
                        CANDIDATE_SOURCE = db.TB_CANDIDATE.FirstOrDefault(d => d.ID == sh.CANDIDATE_ID).SOURCE,
                        CANDIDATE_EXPECTED_SALARY = db.TB_CANDIDATE.FirstOrDefault(d => d.ID == sh.CANDIDATE_ID).EXPECTED_SALARY,
                        PROCESS_DATE = sh.PROCESS_DATE,
                        NOTES = sh.NOTES,
                        CANDIDATE_STATE = sh.CANDIDATE_STATE,
                        CANDIDATE_STATE_NAME = db.TB_STATE_CANDIDATE.FirstOrDefault(s => s.ID == sh.CANDIDATE_STATE).STATE_NAME,
                        //DataCandidate = Manage_CandidateDTO.GetDataCandidate().FirstOrDefault(d => d.ID == sh.CANDIDATE_ID),
                        CANDIDATE_EMAIL = db.TB_CANDIDATE.FirstOrDefault(c => c.ID == sh.CANDIDATE_ID).CANDIDATE_EMAIL,
                        CANDIDATE_SOURCING_DATE = db.TB_CANDIDATE.FirstOrDefault(c => c.ID == sh.CANDIDATE_ID).SOURCING_DATE,
                        CANDIDATE_PHONE = db.TB_CANDIDATE.FirstOrDefault(c => c.ID == sh.CANDIDATE_ID).CANDIDATE_PHONENUMBER,
                        CANDIDATE_NAME = db.TB_CANDIDATE.FirstOrDefault(c => c.ID == sh.CANDIDATE_ID).CANDIDATE_NAME,
                        PIC_ID = sh.PIC_ID,
                        //DataPic = Manage_UserDTO.GetDataUser().FirstOrDefault(d => d.USER_ID == sh.PIC_ID)
                        PIC_FULL_NAME = db.TB_USER.FirstOrDefault(u => u.USER_ID == sh.PIC_ID).FULL_NAME,
                        CANDIDATE_INTERVIEW_DATE = sh.CANDIDATE_INTERVIEW_DATE,
                        CANDIDATE_CLIENT = sh.CANDIDATE_CLIENT,
                        VIEWS_INFORMATION = sh.VIEWS_INFORMATION,
                        DELIVERY_ID = sh.DELIVERY_ID,
                        
                    }
                ).ToList();
                return Data;
            }
        }

        public static int EditData(CandidateSelectionHistoryDTO Data)
        {
            using(DBEntities db = new DBEntities())
            {
                //prepare data pic
                UserDTO DataPIC = (UserDTO)HttpContext.Current.Session["UserLogin"]; 

                var DataSelectHistory = db.TB_CANDIDATE_SELECTION_HISTORY.FirstOrDefault(sh => sh.CANDIDATE_ID == Data.CANDIDATE_ID && sh.CANDIDATE_STATE == Data.CANDIDATE_STATE);

                DataSelectHistory.PIC_ID = DataPIC.USER_ID;
                DataSelectHistory.CANDIDATE_STATE = Data.CANDIDATE_STATE;
                DataSelectHistory.NOTES = Data.NOTES;
                DataSelectHistory.PROCESS_DATE = DateTime.Now;
                DataSelectHistory.CANDIDATE_EXPECTED_SALARY = Data.CANDIDATE_EXPECTED_SALARY;
                DataSelectHistory.CANDIDATE_INTERVIEW_DATE = Data.CANDIDATE_INTERVIEW_DATE;
                DataSelectHistory.CANDIDATE_APPLIED_POSITION = Data.CANDIDATE_APPLIED_POSITION;
                DataSelectHistory.CANDIDATE_SUITABLE_POSITION = Data.CANDIDATE_SUITABLE_POSITION;
                DataSelectHistory.CANDIDATE_SOURCE = Data.CANDIDATE_SOURCE;
                DataSelectHistory.CANDIDATE_CLIENT = Data.CANDIDATE_CLIENT;
                DataSelectHistory.VIEWS_INFORMATION = Data.VIEWS_INFORMATION;
                DataSelectHistory.DELIVERY_ID = Data.DELIVERY_ID;
                return db.SaveChanges();
            }
        }

        

        

    }
}