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
    
    public partial class TB_ACCESS_MENU
    {
        public int ACCESS_MENU_ID { get; set; }
        public int ROLE_ID { get; set; }
        public int MENU_ID { get; set; }
    
        public virtual TB_ACCESS_MENU TB_ACCESS_MENU1 { get; set; }
        public virtual TB_ACCESS_MENU TB_ACCESS_MENU2 { get; set; }
        public virtual TB_ROLE TB_ROLE { get; set; }
        public virtual TB_MENU TB_MENU { get; set; }
    }
}
