using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AutoResponder.Library
{
	public static class Extensions
	{
		public static IEnumerable<T> Select<T>(
			this SqlDataReader reader, Func<SqlDataReader, T> projection)
		{
			while (reader.Read())
			{
				yield return projection(reader);
			}
		}
	}
}