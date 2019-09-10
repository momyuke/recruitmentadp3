using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinalProject.Models;
using FinalProject.Controllers;
using FinalProject.Filters;
using FinalProject.Utils;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.DTO
{
    public class CandidateDTO
    {
        //this dto consist of entire data that needed for view, so the format of candidate dto is not same with tb_dto, ok

        public int ID { get; set; }
        public string CANDIDATE_ID { get; set; }

        [Required(ErrorMessage = "Please Insert Candidate Name")]
        public string CANDIDATE_NAME { get; set; }

        public int? CANDIDATE_AGE { get; set; }

        [Required(ErrorMessage = "Please Insert Candidate Birth Date")]
        public System.DateTime? CANDIDATE_BIRTH_DATE { get; set; }
        [Required(ErrorMessage = "Please Insert Candidate Place Birth")]
        public string CANDIDATE_PLACE_BIRTH { get; set; }
        public string CANDIDATE_ETNIC { get; set; }
        public string PARENT_ADDRESS { get; set; }
        public string RESIDENT_CARD_NUMBER { get; set; }
        [StringLength(14, ErrorMessage = "The Numbers Must Not Exceed 14 and Not Less Than 8 ")] 
        public string TELEPHONE_NUMBER { get; set; }
        [Required(ErrorMessage = "Please Insert Telephone Number")]
        [StringLength(14, ErrorMessage = "The Numbers Must Not Exceed 14 and Not Less Than 12 ")]
        public string CANDIDATE_PHONENUMBER { get; set; }
        [Required(ErrorMessage = "Please Insert Email")]
        [EmailAddress(ErrorMessage = "Please Insert The Correct Email")]
        public string CANDIDATE_EMAIL { get; set; }
        [Required(ErrorMessage = "Please Insert Skill")]
        public List<string> CANDIDATE_SKILL { get; set; }
        public string SUITABLE_POSITION { get; set; }
        public int? AGE { get; set; }
        public System.DateTime? CANDIDATE_INTERVIEW_DATE { get; set; }


        public System.DateTime? AVAILABLE_JOIN { get; set; }
        public string RECOMENDATION { get; set; }
        public decimal?  EXPECTED_sALARY { get; set; }
        public string  NOTES { get; set; }
        [Required(ErrorMessage = "Please Insert Position")]
        public string  POSITION { get; set; }



        [Required(ErrorMessage = "Please Insert Current Address")]
        public string CANDIDATE_CURRENT_ADDRESS { get; set; }
        [Required(ErrorMessage = "Please Insert ID Card Number")]
        public string CANDIDATE_KTP_NUMBER { get; set; }
        public string CANDDIATE_NPWP_NUMBER { get; set; }
        public string CANDIDATE_CV { get; set; }
        public string CANDIDATE_PHOTO { get; set; }

        public string SOURCE { get; set; }
        public System.DateTime? SOURCING_DATE { get; set; }


        //datagender
        public int? GENDER_ID { get; set; }
        public string GENDER_NAME { get; set; }

        //data marital status
        public int? MARITAL_STATUS_ID { get; set; }
        public string MARITAL_STATUS { get; set; }

        //data religion
        public int? RELIGION_ID { get; set; }
        public string RELIGION { get; set; }

        //data address
        [Required(ErrorMessage = "Please Insert City of Candidate")]
        public string CANDIDATE_CITY { get; set; }
        [Required(ErrorMessage = "Please Insert Province of City of Candidate")]
        public string CANDIDATE_PROVINCE { get; set; }
        [Required(ErrorMessage = "Please Insert Zipcode")]
        public int? ZIP_CODE { get; set; }


        //data education candidate
        [Required(ErrorMessage = "Please Insert Last Education of Candidate")]
        public string CANDIDATE_LAST_EDUCATON { get; set; }
        [Required(ErrorMessage = "Please Insert GPA")]
        public double? CANDIDATE_GPA { get; set; }
        [Required(ErrorMessage = "Please Insert Major")]
        public string CANDIDATE_MAJOR { get; set; }
        [Required(ErrorMessage = "Please Insert Degree")]
        public string CANDIDATE_DEGREE { get; set; }
        [Required(ErrorMessage = "Please Select Start Date")]
        public System.DateTime? EDUCATON_START_DATE { get; set; }
        [Required(ErrorMessage = "Please Select End Date")]
        public System.DateTime? EDUCATON_END_DATE { get; set; }

        //data state
        public int? CANDIDATE_STATE_ID { get; set; }
        public string CANDIDATE_STATE_NAME { get; set; }

        public List<CandidateJobExperienceDTO> DataJobExp { get; set; }

        public System.DateTime? LAST_UPDATE { get; set; }

        public string DELIVERY_ID { get; set; }
    }

    public class Manage_CandidateDTO
    {
        //----------------------------------------------------------------- add data candidate ------------------------------------------
        public static List<object> AddData(CandidateDTO DataNewCandidate, HttpPostedFileBase Pict, HttpPostedFileBase Cv)
        {
            using(DBEntities db = new DBEntities())
            {
                //generate id candidate
                string Candidate_ID = "CA" + DateTime.Now.ToString("fffff");

                //process file pict candidate
                string pict_name = "-";
                if (Pict != null)
                {
                    string pict_ext = Pict.FileName.Split('.')[1];
                    pict_name = "Pict_" + Candidate_ID + "." + pict_ext;
                    string path_pict = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Data/DataCandidate/Pict/"), pict_name);
                    Pict.SaveAs(path_pict);
                }

                //process file Cv
                string Cv_name = "-";
                if (Cv != null)
                {
                    string Cv_ext = Cv.FileName.Split('.')[1];
                    Cv_name = "Cv_" + Candidate_ID + "." + Cv_ext;
                    string path_cv = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Data/DataCandidate/Cv/"), Cv_name);
                    Cv.SaveAs(path_cv);
                }

                //process to convert datetime
                var birth_date = Convert.ToDateTime(DataNewCandidate.CANDIDATE_BIRTH_DATE);
                var edu_start_date = Convert.ToDateTime(DataNewCandidate.EDUCATON_START_DATE);
                var edu_end_date = Convert.ToDateTime(DataNewCandidate.EDUCATON_END_DATE);

                var LastUpdate = DateTime.Now;

                //process insert data
                db.TB_CANDIDATE.Add(new TB_CANDIDATE
                {
                    CANDIDATE_ID = Candidate_ID,
                    CANDIDATE_NAME = DataNewCandidate.CANDIDATE_NAME,
                    CANDIDATE_AGE = DataNewCandidate.CANDIDATE_AGE,
                    CANDIDATE_BIRTH_DATE = birth_date,
                    CANDIDATE_PLACE_BIRTH = DataNewCandidate.CANDIDATE_PLACE_BIRTH,
                    MARITAL_STATUS_ID = DataNewCandidate.MARITAL_STATUS_ID,
                    GENDER_ID = DataNewCandidate.GENDER_ID,
                    RELIGION_ID = DataNewCandidate.RELIGION_ID,
                    CANDIDATE_ETNIC = DataNewCandidate.CANDIDATE_ETNIC,
                    CANDIDATE_PHONENUMBER = DataNewCandidate.CANDIDATE_PHONENUMBER,
                    CANDIDATE_EMAIL = DataNewCandidate.CANDIDATE_EMAIL,
                    CANDIDATE_CITY = DataNewCandidate.CANDIDATE_CITY,
                    CANDIDATE_PROVINCE = DataNewCandidate.CANDIDATE_PROVINCE,
                    CANDIDATE_CURRENT_ADDRESS = DataNewCandidate.CANDIDATE_CURRENT_ADDRESS,
                    CANDIDATE_KTP_NUMBER = DataNewCandidate.CANDIDATE_KTP_NUMBER,
                    CANDDIATE_NPWP_NUMBER = DataNewCandidate.CANDDIATE_NPWP_NUMBER,
                    CANDIDATE_CV = Cv_name,
                    CANDIDATE_PHOTO = pict_name,
                    CANDIDATE_LAST_EDUCATON = DataNewCandidate.CANDIDATE_LAST_EDUCATON,
                    CANDIDATE_GPA = DataNewCandidate.CANDIDATE_GPA,
                    CANDIDATE_MAJOR = DataNewCandidate.CANDIDATE_MAJOR,
                    CANDIDATE_DEGREE = DataNewCandidate.CANDIDATE_DEGREE,
                    CANDIDATE_STATE_ID = 1,
                    SOURCE = DataNewCandidate.SOURCE,
                    SOURCING_DATE = DateTime.Now,
                    CANDIDATE_ZIPCODE = DataNewCandidate.ZIP_CODE,
                    PARENT_ADDRESS = DataNewCandidate.PARENT_ADDRESS,
                    RESIDENT_CARD_NUMBER = DataNewCandidate.RESIDENT_CARD_NUMBER,
                    TELEPHONE_NUMBER = DataNewCandidate.TELEPHONE_NUMBER,
                    AVAILABLE_JOIN = DataNewCandidate.AVAILABLE_JOIN,
                    RECOMENDATION = DataNewCandidate.RECOMENDATION,
                    EXPECTED_SALARY = DataNewCandidate.EXPECTED_sALARY,
                    NOTES = DataNewCandidate.NOTES,
                    POSITION = DataNewCandidate.POSITION,
                    EDUCATION_START_DATE = edu_start_date,
                    EDUCATION_END_DATE = edu_end_date,
                    CANDIDATE_INTERVIEW_DATE = DataNewCandidate.CANDIDATE_INTERVIEW_DATE,
                    LAST_UPDATE = LastUpdate
                });

                int res = 0;

                if (db.SaveChanges() > 0)
                {
                    //insert skill
                   if(DataNewCandidate.CANDIDATE_SKILL != null)
                    {
                        foreach (string skill in DataNewCandidate.CANDIDATE_SKILL)
                        {
                            db.TB_CANDIDATE_SKILL.Add(new TB_CANDIDATE_SKILL
                            {
                                CANDIDATE_ID = db.TB_CANDIDATE.FirstOrDefault(c => c.CANDIDATE_ID == Candidate_ID).ID,
                                SKILL = skill
                            });
                            db.SaveChanges();
                        }
                    }
                        //insert selection history
                        UserDTO UserLogin = (UserDTO)HttpContext.Current.Session["UserLogin"];
                        Manage_CandidateSelectionHistoryDTO.AddData(new CandidateSelectionHistoryDTO
                        {
                            CANDIDATE_ID = db.TB_CANDIDATE.FirstOrDefault(ca => ca.CANDIDATE_ID == Candidate_ID).ID,
                            PIC_ID = UserLogin.USER_ID,
                            CANDIDATE_APPLIED_POSITION = DataNewCandidate.POSITION,
                            CANDIDATE_SUITABLE_POSITION = "-",
                            CANDIDATE_SOURCE = DataNewCandidate.SOURCE,
                            CANDIDATE_EXPECTED_SALARY = DataNewCandidate.EXPECTED_sALARY,
                            CANDIDATE_STATE = 1,
                            NOTES = DataNewCandidate.NOTES,
                            LAST_UPDATE = LastUpdate
                        });
                    db.SaveChanges();
                    res = 1;
                }
                return new List<object>() { { res }, {Candidate_ID} };
            }
        }

        //--------------------------------------------------------- edit data candidate -------------------------------------------------
        public static int EditCandidate(CandidateDTO Data, HttpPostedFileBase Pict, HttpPostedFileBase Cv)
        {
            using(DBEntities db = new DBEntities())
            {
                TB_CANDIDATE Candidate = db.TB_CANDIDATE.FirstOrDefault(d => d.ID == Data.ID);

                

                //process file pict candidate
                string pict_name = "-";
                if (Pict != null)
                {
                    string pict_ext = Pict.FileName.Split('.')[1];
                    pict_name = "Pict_" + DateTime.Now.ToString("ffff") + "." + pict_ext;
                    string path_pict = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Data/DataCandidate/Pict/"), pict_name);
                    Pict.SaveAs(path_pict);
                }

                //process file Cv
                string Cv_name = "-";
                if (Cv != null)
                {
                    string Cv_ext = Cv.FileName.Split('.')[1];
                    Cv_name = "Cv_" + DateTime.Now.ToString("ffff") + "." + Cv_ext;
                    string path_cv = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Data/DataCandidate/Cv/"), Cv_name);
                    Cv.SaveAs(path_cv);
                }

                //process to convert datetime
                var birth_date = Convert.ToDateTime(Data.CANDIDATE_BIRTH_DATE); 
                var edu_start_date = Convert.ToDateTime(Data.EDUCATON_START_DATE);
                var edu_end_date = Convert.ToDateTime(Data.EDUCATON_END_DATE);


                var LastUpdate = DateTime.Now;

                //add selection history
                if (Candidate.CANDIDATE_STATE_ID != Data.CANDIDATE_STATE_ID)
                {
                    //insert selection history
                    UserDTO UserLogin = (UserDTO)HttpContext.Current.Session["UserLogin"];

                    //EDIT FIELD VIEWS INFORMATION TO MAKE EACH OF SELECTION HISTORY THAT SELECT WITH PROGRAM IS BEING NOT DISPLAY. BUT STILL BE ABLE TO SEE IT AT TABLE SELECTION HISTORY ON DETAILS INFORMATION OF CANDIDATE
                    foreach (var item in db.TB_CANDIDATE_SELECTION_HISTORY.Where(m => m.CANDIDATE_ID == Data.ID)) {
                        item.VIEWS_INFORMATION = "NO";
                    }

                    //THEN ADD TO SELECTION HISTORY FOR DISPLAY WITH LAST EDIT STATE

                    db.TB_CANDIDATE_SELECTION_HISTORY.Add(new TB_CANDIDATE_SELECTION_HISTORY
                    {

                        CANDIDATE_ID = Data.ID,
                        PIC_ID = UserLogin.USER_ID,
                        CANDIDATE_APPLIED_POSITION = Data.POSITION,
                        CANDIDATE_SUITABLE_POSITION = Data.SUITABLE_POSITION,
                        CANDIDATE_SOURCE = Data.SOURCE,
                        CANDIDATE_EXPECTED_SALARY = Data.EXPECTED_sALARY,
                        CANDIDATE_STATE = Data.CANDIDATE_STATE_ID,
                        NOTES = Data.NOTES,
                        PROCESS_DATE = DateTime.Now,
                        CANDIDATE_INTERVIEW_DATE = Data.CANDIDATE_INTERVIEW_DATE,
                        VIEWS_INFORMATION = "YES",
                        LAST_UPDATE = LastUpdate

                    }) ;
                    db.SaveChanges();
                }

                //process update data
                Candidate.CANDIDATE_NAME = Data.CANDIDATE_NAME;
                Candidate.CANDIDATE_AGE = Data.CANDIDATE_AGE;
                Candidate.CANDIDATE_BIRTH_DATE = birth_date;
                Candidate.CANDIDATE_PLACE_BIRTH = Data.CANDIDATE_PLACE_BIRTH;
                Candidate.MARITAL_STATUS_ID = Data.MARITAL_STATUS_ID;
                Candidate.GENDER_ID = Data.GENDER_ID;
                Candidate.RELIGION_ID = Data.RELIGION_ID;
                Candidate.CANDIDATE_ETNIC = Data.CANDIDATE_ETNIC;
                Candidate.CANDIDATE_PHONENUMBER = Data.CANDIDATE_PHONENUMBER;
                Candidate.CANDIDATE_EMAIL = Data.CANDIDATE_EMAIL;
                Candidate.CANDIDATE_CITY = Data.CANDIDATE_CITY;
                Candidate.CANDIDATE_PROVINCE = Data.CANDIDATE_PROVINCE;
                Candidate.CANDIDATE_CURRENT_ADDRESS = Data.CANDIDATE_CURRENT_ADDRESS;
                Candidate.CANDIDATE_KTP_NUMBER = Data.CANDIDATE_KTP_NUMBER;
                Candidate.CANDDIATE_NPWP_NUMBER = Data.CANDDIATE_NPWP_NUMBER;

                if(Cv != null) Candidate.CANDIDATE_CV = Cv_name;
                if(Pict != null) Candidate.CANDIDATE_PHOTO = pict_name;
                Candidate.CANDIDATE_LAST_EDUCATON = Data.CANDIDATE_LAST_EDUCATON;
                Candidate.CANDIDATE_GPA = Data.CANDIDATE_GPA;
                Candidate.CANDIDATE_MAJOR = Data.CANDIDATE_MAJOR;
                Candidate.CANDIDATE_DEGREE = Data.CANDIDATE_DEGREE;
                Candidate.CANDIDATE_STATE_ID = Data.CANDIDATE_STATE_ID;
                Candidate.SOURCE = Data.SOURCE;
                Candidate.SOURCING_DATE = DateTime.Now;
                Candidate.CANDIDATE_ZIPCODE = Data.ZIP_CODE;
                Candidate.PARENT_ADDRESS = Data.PARENT_ADDRESS;
                Candidate.RESIDENT_CARD_NUMBER = Data.RESIDENT_CARD_NUMBER;
                Candidate.TELEPHONE_NUMBER = Data.TELEPHONE_NUMBER;
                Candidate.AVAILABLE_JOIN = Data.AVAILABLE_JOIN;
                Candidate.RECOMENDATION = Data.RECOMENDATION;
                Candidate.EXPECTED_SALARY = Data.EXPECTED_sALARY;
                Candidate.NOTES = Data.NOTES;
                Candidate.POSITION = Data.POSITION;
                Candidate.SUITABLE_POSITION = Data.SUITABLE_POSITION;
                Candidate.EDUCATION_START_DATE = edu_start_date;
                Candidate.EDUCATION_END_DATE = edu_end_date;
                Candidate.CANDIDATE_INTERVIEW_DATE = Data.CANDIDATE_INTERVIEW_DATE;
                Candidate.LAST_UPDATE = LastUpdate;

                List<string> Skills = db.TB_CANDIDATE_SKILL.Where(d => d.CANDIDATE_ID == Data.ID).Select(d => d.SKILL).ToList();
                if (Skills.Count > 0)
                {
                    foreach (var d in db.TB_CANDIDATE_SKILL.Where(d => d.CANDIDATE_ID == Data.ID).ToList())
                    {
                        db.TB_CANDIDATE_SKILL.Remove(d);
                    }
                }

                foreach (var d in Data.CANDIDATE_SKILL)
                {
                    db.TB_CANDIDATE_SKILL.Add(new TB_CANDIDATE_SKILL {
                        CANDIDATE_ID = Data.ID,
                        SKILL = d
                    });
                }

                return db.SaveChanges();
            }
        }

        //---------------------------------------------------------- Get Data Candidate -----------------------------------------------
        public static List<CandidateDTO> GetDataCandidate()
        {
            using (DBEntities db = new DBEntities())
            {
                List<CandidateDTO> ListCandidateDTO = db.TB_CANDIDATE.Select(ca => new CandidateDTO
                {
                    ID = ca.ID,
                    CANDIDATE_ID = ca.CANDIDATE_ID,
                    CANDIDATE_NAME = ca.CANDIDATE_NAME,
                    CANDIDATE_AGE = ca.CANDIDATE_AGE,
                    CANDIDATE_BIRTH_DATE = ca.CANDIDATE_BIRTH_DATE,
                    CANDIDATE_PLACE_BIRTH = ca.CANDIDATE_PLACE_BIRTH,
                    MARITAL_STATUS_ID = ca.MARITAL_STATUS_ID,
                    GENDER_ID = ca.GENDER_ID,
                    RELIGION_ID = ca.RELIGION_ID,
                    CANDIDATE_ETNIC = ca.CANDIDATE_ETNIC,
                    CANDIDATE_PHONENUMBER = ca.CANDIDATE_PHONENUMBER,
                    CANDIDATE_EMAIL = ca.CANDIDATE_EMAIL,
                    CANDIDATE_CITY = ca.CANDIDATE_CITY,
                    CANDIDATE_PROVINCE = ca.CANDIDATE_PROVINCE,
                    CANDIDATE_CURRENT_ADDRESS = ca.CANDIDATE_CURRENT_ADDRESS,
                    CANDIDATE_KTP_NUMBER = ca.CANDIDATE_KTP_NUMBER,
                    CANDDIATE_NPWP_NUMBER = ca.CANDDIATE_NPWP_NUMBER,
                    CANDIDATE_CV = ca.CANDIDATE_CV,
                    CANDIDATE_PHOTO = ca.CANDIDATE_PHOTO,
                    CANDIDATE_LAST_EDUCATON = ca.CANDIDATE_LAST_EDUCATON,
                    CANDIDATE_GPA = ca.CANDIDATE_GPA,
                    CANDIDATE_MAJOR = ca.CANDIDATE_MAJOR,
                    CANDIDATE_DEGREE = ca.CANDIDATE_DEGREE,
                    CANDIDATE_STATE_ID = ca.CANDIDATE_STATE_ID,
                    SOURCE = ca.SOURCE,
                    SOURCING_DATE = ca.SOURCING_DATE,
                    ZIP_CODE = ca.CANDIDATE_ZIPCODE,
                    PARENT_ADDRESS = ca.PARENT_ADDRESS,
                    RESIDENT_CARD_NUMBER = ca.RESIDENT_CARD_NUMBER,
                    TELEPHONE_NUMBER = ca.TELEPHONE_NUMBER,
                    AVAILABLE_JOIN = ca.AVAILABLE_JOIN,
                    RECOMENDATION = ca.RECOMENDATION,
                    EXPECTED_sALARY = ca.EXPECTED_SALARY,
                    NOTES = ca.NOTES,
                    POSITION = ca.POSITION,
                    EDUCATON_START_DATE = ca.EDUCATION_START_DATE,
                    EDUCATON_END_DATE = ca.EDUCATION_END_DATE,
                    RELIGION = db.TB_RELIGION.FirstOrDefault(d => d.RELIGION_ID == ca.RELIGION_ID).RELIGION_NAME,
                    GENDER_NAME = db.TB_GENDER.FirstOrDefault(d => d.GENDER_ID == ca.GENDER_ID).GENDER_NAME,
                    SUITABLE_POSITION = ca.SUITABLE_POSITION,
                    CANDIDATE_INTERVIEW_DATE = ca.CANDIDATE_INTERVIEW_DATE,
                    LAST_UPDATE = ca.LAST_UPDATE
                }).ToList();
                return ListCandidateDTO;
            }
        }


        // =========================================================================== DELETE DATA CANDIDATE =================================================================
           public static int DeleteDataCandidate(int CandidateId)
        {
            using (DBEntities db = new DBEntities()){
                var Cand = db.TB_CANDIDATE.FirstOrDefault(m => m.ID == CandidateId);
                db.TB_CANDIDATE.Remove(Cand);


                foreach (var item in db.TB_CANDIDATE_SELECTION_HISTORY.Where(m => m.CANDIDATE_ID == CandidateId)){
                    db.TB_CANDIDATE_SELECTION_HISTORY.Remove(item);
                }

                return db.SaveChanges();
            }
            
        }
        //------------------------------------------------ get job experience
       public static List<CandidateJobExperienceDTO> GetJobExp(int CandidateId)
        {
            return Manage_CandidateJobExperienceDTO.GetData().Where(d => d.CANDIDATE_ID == CandidateId).ToList();
        }

        //----------------------------------------------- getAge ---------------------------------------------
        public static int GetCandidateAge(int CandidateId)
        {
            using(DBEntities db = new DBEntities())
            {
                return (int.Parse(DateTime.Now.ToString("yyyy")) - int.Parse(Convert.ToDateTime(db.TB_CANDIDATE.FirstOrDefault(d => d.ID == CandidateId).CANDIDATE_BIRTH_DATE).ToString("yyyy")));
            }
        }

        //--------------------------------------------- check skill chekcbox -------------------------------------
        public static string CheckBoxSkill(string Skill, int IdCandidate)
        {
            using(DBEntities db = new DBEntities())
            {
                var data = db.TB_CANDIDATE_SKILL.FirstOrDefault(d => d.SKILL.Equals(Skill) && d.CANDIDATE_ID.Equals(IdCandidate));
                return data != null ? "checked" : null; 
                
            }
        }

        //------------------------------------------------------ get skill --------------------------------------------------
        public static string GetSkill(int CandidateId)
        {
            using(DBEntities db = new DBEntities())
            {
                string skill = String.Join(",", db.TB_CANDIDATE_SKILL.Where(d => d.CANDIDATE_ID == CandidateId).Select(d => d.SKILL).ToArray());

                return skill;
            }
        }
    }
}