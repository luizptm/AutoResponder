//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoResponder.Web.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class BR_AccessControl_Application
    {
        public BR_AccessControl_Application()
        {
            this.BR_AccessControl_Role = new HashSet<BR_AccessControl_Role>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime CREATION_DATE { get; set; }
    
        public virtual ICollection<BR_AccessControl_Role> BR_AccessControl_Role { get; set; }
    }
}
