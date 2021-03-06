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
    
    public partial class BR_Users
    {
        public BR_Users()
        {
            this.BR_AutoResponder_Sending = new HashSet<BR_AutoResponder_Sending>();
            this.BR_AutoResponder_UserEntry = new HashSet<BR_AutoResponder_UserEntry>();
        }
    
        public int idUser { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string gender { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public Nullable<System.DateTime> UPDATE_TIME { get; set; }
        public Nullable<System.DateTime> CREATION_TIME { get; set; }
        public string zipcode { get; set; }
        public string cpf { get; set; }
        public string hometown { get; set; }
        public string navigateURL { get; set; }
        public Nullable<bool> verified { get; set; }
        public string relationshipStatus { get; set; }
        public string religion { get; set; }
        public string politicalView { get; set; }
        public string biography { get; set; }
        public string languages { get; set; }
        public Nullable<double> timezone { get; set; }
        public string location { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string userID { get; set; }
        public string rg { get; set; }
        public string telephone { get; set; }
        public string tags { get; set; }
    
        public virtual ICollection<BR_AutoResponder_Sending> BR_AutoResponder_Sending { get; set; }
        public virtual ICollection<BR_AutoResponder_UserEntry> BR_AutoResponder_UserEntry { get; set; }
    }
}
