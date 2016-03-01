using System;
using System.Web.Security;
using AutoResponder.Models.Entity;

namespace AutoResponder.Library.Security
{
    public class CustomMembershipUser : MembershipUser
    {
        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string Trigram { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

		public CustomMembershipUser(BR_AccessControl_User user)
			: base("CustomMembershipProvider", user.Name, user.Trigram, "", 
				string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
		{
            Id = user.Id;
			Name = user.Name;
			Trigram = user.Trigram;
			RoleId = user.RoleId;
			RoleName = user.BR_AccessControl_Role.Name;
		}

        #endregion
    }
}