﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalProject.Models;
using FinalProject.DTO;
using FinalProject.Filters;
using System.Security.Cryptography;
using System.Text;

namespace FinalProject.Controllers
{
    public class AuthController : Controller
    {
 //-------------------------------------------- ini untuk namplin view login :) ------------------------------------------------------
        [Route("auth/login")]
        public ActionResult Login()
        {
            try
            {
                if (Session["UserLogin"] != null)
                {
                    using (DBEntities db = new DBEntities())
                    {
                        UserDTO UserLogin = (UserDTO)Session["UserLogin"];
                        TB_ROLE Tb_Role = db.TB_ROLE.FirstOrDefault(r => r.ROLE_ID == UserLogin.ROLE_ID);
                        if (Tb_Role != null)
                        {
                            return Redirect("~/");
                        }
                        else
                        {
                            return Redirect("~/auth/login");
                        }
                    }
                }
                else
                {
                    ViewBag.DataView = new Dictionary<string, string>()
                {
                    {"title","Login"}
                };
                    return View("Login");
                }
            }
            catch(Exception)
            {
                return Redirect("~/auth/error");
            }
        }

//------------------------------------------------------------ ini peroses login :)----------------------------------------------

        [HttpPost,Route("auth/login")]
        public ActionResult Login(UserDTO UserLogin = null)
        {
            try
            {
                if (UserLogin != null)
                {
                    using (DBEntities db = new DBEntities())
                    {
                        ModelState.Remove("EMAIL");
                        ModelState.Remove("CONFIRM_PASSWORD");
                        ModelState.Remove("FULL_NAME");
                        ModelState.Remove("ROLE_ID");
                        if (ModelState.IsValid)
                        {
                            //encrypt password with sha256
                            TB_USER user = db.TB_USER.FirstOrDefault(u => u.USERNAME == UserLogin.USERNAME);
                            //if user is not already in database
                            if (user == null)
                            {
                                TempData.Add("message", "User is not valid");
                                TempData.Add("type", "warning");
                                return Redirect("~/auth/login");
                            }
                            //if user is already in database
                            else
                            {
                                if (user.PASSWORD != UserLogin.PASSWORD)
                                {
                                    TempData.Add("message", "Password Wrong");
                                    TempData.Add("type", "warning");
                                    return Redirect("~/auth/login");
                                }
                                else
                                {
                                    //make session is filed by userDTO 
                                    UserDTO userDTO = new UserDTO
                                    {
                                        ROLE_ID = user.ROLE_ID,
                                        USER_ID = user.USER_ID,
                                        USERNAME = user.USERNAME,
                                        EMAIL = user.EMAIL,
                                        FULL_NAME = user.FULL_NAME
                                    };
                                    Session.Add("UserLogin", userDTO);
                                    return Redirect("~/user");
                                }
                            }
                        }
                    }
                    return Redirect("~/auth/login");
                }
                return Redirect("~/auth/login");
            }
            catch (Exception)
            {
                return Redirect("~/auth/error");
            }
        }
 //-------------------------------------------------------- untuk menampilkan error jika tidak ada access menu-----------------
         [Route("auth/error")]
         public ActionResult Error()
        {
            return View("404");
        }
    }
}