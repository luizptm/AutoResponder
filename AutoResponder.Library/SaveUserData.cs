using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using AutoResponder.Models.Entity;


namespace AutoResponder.Library
{
	public class UserVM
	{
		public string Nome { get; set; }

		public string Email { get; set; }

		public string Tags { get; set; }

		public string SendingListId { get; set; }

		public string Application { get; set; }
	}

	public class SaveUserData
	{
		public readonly static string ConnectionString = ConfigurationManager.ConnectionStrings["PROTESTE.BRAZIL_SMALL"].ToString();

		/// <summary>
		/// Save
		/// </summary>
		/// <param name="vm"></param>
		/// <returns></returns>
		public static String Save(UserVM vm)
		{
			//string result = "";
			int returnSave1 = 0;
			int returnSave2 = 0;
			int returnSave3 = 0;
			using (SqlConnection cnn = new SqlConnection(ConnectionString))
			{
				cnn.Open();
				try
				{
					if (!String.IsNullOrEmpty(vm.Nome) && !String.IsNullOrEmpty(vm.Email))
					{
						returnSave1 = SaveUserAndUserApp(vm, cnn);
						//if (returnSave1 >= 0) result += "STEP 01 - executado com sucesso.<br/>";
						//else result += "STEP 01 - erro ao executar (" + returnSave1 + ").<br/>";
					}
					if (!String.IsNullOrEmpty(vm.Email) && !String.IsNullOrEmpty(vm.Tags))
					{
						string[] separator = { " " };
						string[] vetTags = vm.Tags.Split(separator, StringSplitOptions.RemoveEmptyEntries);
						foreach (String tag in vetTags)
						{
							returnSave2 = SaveUserTag(vm.Email, tag, cnn);
							//if (returnSave2 >= 0) result += "STEP 02 - executado com sucesso para a tag '" + tag + "'.";
							//else result += "STEP 02 - erro ao executar para a tag '" + tag + "' (" + returnSave2 + ").";
						}
					}
					if (!String.IsNullOrEmpty(vm.Email) && !String.IsNullOrEmpty(vm.SendingListId))
					{
						returnSave3 = SaveUserList(vm, cnn);
						//if (returnSave3 >= 0) result += "STEP 03 - executado com sucesso.";
						//else result += "STEP 03 - erro ao executar (" + returnSave3 + ").";
					}
					return "OK";
				}
				catch (Exception ex)
				{
					return "ERROR**<br/>" + ex.Message + "<br/>" + ex.StackTrace;
				}
				finally
				{
					cnn.Close();
					cnn.Dispose();
				}
			}
		}

		/// <summary>
		/// SaveUserAndUserApp
		/// </summary>
		/// <param name="vm"></param>
		/// <param name="cnn"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		private static int SaveUserAndUserApp(UserVM vm, SqlConnection cnn, SqlTransaction transaction = null)
		{
			if (!String.IsNullOrEmpty(vm.Nome) && !String.IsNullOrEmpty(vm.Email))
			{
				SqlCommand cmd = new SqlCommand("BR_Insert_User_User_APP", cnn);
				cmd.Parameters.Add(new SqlParameter("param_firstname", vm.Nome));
				cmd.Parameters.Add(new SqlParameter("param_email", vm.Email));
				cmd.Parameters.Add(new SqlParameter("param_AppSource", vm.Application));
				cmd.Parameters.Add(new SqlParameter("param_Tags", vm.Tags));

				cmd.CommandType = CommandType.StoredProcedure;
				if (transaction != null)
				{
					cmd.Transaction = transaction;
				}
				int change = cmd.ExecuteNonQuery();
				return change;
			}
			return 0;
		}

		/// <summary>
		/// SaveUserTag
		/// </summary>
		/// <param name="vm"></param>
		/// <param name="cnn"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		private static int SaveUserTag(String email, String tag, SqlConnection cnn, SqlTransaction transaction = null)
		{
			if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(tag))
			{
				SqlCommand cmd = new SqlCommand("BR_Insert_User_TAG", cnn);
				cmd.Parameters.Add(new SqlParameter("param_Email", email));
				cmd.Parameters.Add(new SqlParameter("param_Tag", tag));

				cmd.CommandType = CommandType.StoredProcedure;
				if (transaction != null)
				{
					cmd.Transaction = transaction;
				}
				int change = cmd.ExecuteNonQuery();
				return change;
			}
			return 0;
		}

		/// <summary>
		/// SaveUserList
		/// </summary>
		/// <param name="vm"></param>
		/// <param name="cnn"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		private static int SaveUserList(UserVM vm, SqlConnection cnn, SqlTransaction transaction = null)
		{
			if (!String.IsNullOrEmpty(vm.Email) && !String.IsNullOrEmpty(vm.SendingListId))
			{
				SqlCommand cmd = new SqlCommand("BR_Insert_User_SendingList", cnn);
				cmd.Parameters.Add(new SqlParameter("param_Email", vm.Email));
				cmd.Parameters.Add(new SqlParameter("param_SendingListId", vm.SendingListId));
				cmd.Parameters.Add(new SqlParameter("param_SendingList", ""));

				cmd.CommandType = CommandType.StoredProcedure;
				if (transaction != null)
				{
					cmd.Transaction = transaction;
				}
				int change = cmd.ExecuteNonQuery();
				return change;
			}
			return 0;
		}
	}
}