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
    
    public partial class BR_AccessControl_Role
    {
        public BR_AccessControl_Role()
        {
            this.BR_AccessControl_Action = new HashSet<BR_AccessControl_Action>();
            this.BR_AccessControl_User = new HashSet<BR_AccessControl_User>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int ApplicationId { get; set; }
    
        public virtual BR_AccessControl_Application BR_AccessControl_Application { get; set; }
        public virtual ICollection<BR_AccessControl_Action> BR_AccessControl_Action { get; set; }
        public virtual ICollection<BR_AccessControl_User> BR_AccessControl_User { get; set; }
    }
}
