using System;
using System.Security.Principal;
using System.Web.Security;

namespace AutoResponder.Library.Security
{
    /// <summary>
    /// An identity object represents the user on whose behalf the code is running.
    /// </summary>
    [Serializable]
    public class CustomIdentity : IIdentity
    {
		public IIdentity Identity { get; set; }

		/// <summary>
		/// Gets the name of the current user.
		/// </summary>
		/// <returns>
		/// The name of the user on whose behalf the code is running.
		/// </returns>
		public string Name
		{
			get { return Identity.Name; }
		}

		/// <summary>
		/// Trigrama
		/// </summary>
		public string Trigram { get; set; }

		/// <summary>
		/// Role ID
		/// </summary>
		public int RoleId { get; set; }

		/// <summary>
		/// Role Name
		/// </summary>
		public string RoleName { get; set; }

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <returns>
        /// The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType
        {
            get { return Identity.AuthenticationType; }
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>
        /// true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated { get { return Identity.IsAuthenticated; } }

        #region Constructor

        public CustomIdentity(IIdentity identity)
        {
            Identity = identity;

            var customMembershipUser = (CustomMembershipUser) Membership.GetUser(identity.Name);
            if(customMembershipUser != null)
            {
                Trigram = customMembershipUser.Trigram;
				RoleId = customMembershipUser.RoleId;
                RoleName = customMembershipUser.RoleName;
            }
        }
 
        #endregion
    }
}