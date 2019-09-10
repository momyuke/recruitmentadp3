using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FinalProject.Models;

namespace FinalProject.DTO
{
    public class UserDTO
    {
        public int USER_ID { get; set; }

        [RegularExpression(".{5,30}")]
        [Required(ErrorMessage = "Username Must Be fill")]
        public string USERNAME { get; set; }

        [Required(ErrorMessage = "Password Must Be fill")]
        public string PASSWORD { get; set; }

        [Compare("PASSWORD")]
        [Required(ErrorMessage = "Password Must Be fill")]
        public string CONFIRM_PASSWORD { get; set; }

        public string EMAIL { get; set; }

        [Required]
        [RegularExpression(@"([a-z]|\W){5,30}", ErrorMessage = "only letters")]
        public string FULL_NAME { get; set; }

        [Required]
        public int ROLE_ID { get; set; }

        public string ROLE_NAME { get; set; }
    }

    public class Manage_UserDTO
    {
        public static List<UserDTO> GetDataUser()
        {
            using (DBEntities db = new DBEntities())
            {
                return db.TB_USER.Select(usr => new UserDTO
                {
                    USER_ID = usr.USER_ID,
                    USERNAME = usr.USERNAME,
                    EMAIL = usr.EMAIL,
                    FULL_NAME = usr.FULL_NAME,
                    ROLE_ID = usr.ROLE_ID,
                    ROLE_NAME = db.TB_ROLE.FirstOrDefault(r => r.ROLE_ID == usr.ROLE_ID).ROLE_NAME
                }).ToList();
            }
        }

        public static int GetUserID()
        {

            UserDTO UserId = (UserDTO)HttpContext.Current.Session["UserLogin"];
            return UserId.USER_ID;
        }
    }
}