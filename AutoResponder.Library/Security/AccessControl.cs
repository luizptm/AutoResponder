using System;
using System.Web;
using System.Web.Security;
using AutoResponder.ViewModels;

namespace AutoResponder.Library.Security
{
	public class AccessControl
	{
		public MembershipUser GetUser(string allowedRoles = "*")
		{
			Login login = HttpContext.Current.Session["login"] == null ? null : (Login)HttpContext.Current.Session["login"];
			if (login == null)
			{
				return null;
			}

			CustomMembershipProvider membershipProvider = new CustomMembershipProvider();
			MembershipUser user = membershipProvider.GetUser(login.UserName, false);

			if (allowedRoles != "*")
			{
				allowedRoles = allowedRoles.Replace(" ", "");
				allowedRoles = allowedRoles.ToLower();
				string[] separator = { "," };
				string[] vet_allowedRoles = allowedRoles.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				CustomRoleProvider customRoleProvider = new CustomRoleProvider();
				string[] vet_rolesUser = customRoleProvider.GetRolesForUser(login.UserName);
				foreach (string role in vet_allowedRoles)
				{
					foreach (string roleUser in vet_rolesUser)
					{
						if (role == roleUser)
							return user;
					}
				}
				return null;
			}
			return user;
		}

		public string HasAccess(string allowedRoles = "*")
		{
			Login login = HttpContext.Current.Session["login"] == null ? null : (Login)HttpContext.Current.Session["login"];
			if (login == null)
			{
				return "-1";
			}

			CustomMembershipProvider membershipProvider = new CustomMembershipProvider();
			MembershipUser user = membershipProvider.GetUser(login.UserName, false);

			if (allowedRoles != "*")
			{
				allowedRoles = allowedRoles.Replace(" ", "");
				allowedRoles = allowedRoles.ToLower();
				string[] separator = { "," };
				string[] vet_allowedRoles = allowedRoles.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				CustomRoleProvider customRoleProvider = new CustomRoleProvider();
				string[] vet_rolesUser = customRoleProvider.GetRolesForUser(login.UserName);
				foreach (string role in vet_allowedRoles)
				{
					foreach (string roleUser in vet_rolesUser)
					{
						if (role == roleUser)
							return "1";
					}
				}
				return "-2";
			}
			return "0";
		}

		public string[] GetCurrentUserRoles()
		{
			Login login = HttpContext.Current.Session["login"] == null ? null : (Login)HttpContext.Current.Session["login"];
			if (login == null)
			{
				return null;
			}

			CustomRoleProvider customRoleProvider = new CustomRoleProvider();
			string[] roles = customRoleProvider.GetRolesForUser(login.UserName);
			return roles;
		}
	}
}