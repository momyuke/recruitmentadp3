//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FinalProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_SUBMENU
    {
        public int MENU_ID { get; set; }
        public int SUB_MENU_ID { get; set; }
        public string TITLE_SUBMENU { get; set; }
        public string LOGO_SUBMENU { get; set; }
        public string URL { get; set; }
    
        public virtual TB_MENU TB_MENU { get; set; }
    }
}