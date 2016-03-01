using System;
using System.Collections.Generic;
using System.Text;
using AutoResponder.Library.DatumBox;
using AutoResponder.Postmark.Spamcheck;
using Newtonsoft.Json;
using AutoResponder.Library.SpamCheck;

namespace AutoResponder.Library
{
	public class SpamScore
	{
		public static string Get(string text, bool useDatumbox = true)
		{
			String resultJson = "";
			byte[] bytes = Encoding.Default.GetBytes(resultJson);
			resultJson = Encoding.UTF8.GetString(bytes);

			if (useDatumbox)
			{
				DatumboxAPI api = new DatumboxAPI();
				resultJson = api.SpamDetection(text);
				if (resultJson != "")
				{
					DatumboxResult resultSPAM = (DatumboxResult)JsonConvert.DeserializeObject(resultJson, typeof(DatumboxResult));
					return resultSPAM.output.result;
				}
				else
				{
					return "ERRO";
				}
			}
			else
			{
				SpamCheckAPI api = new SpamCheckAPI();
				resultJson = api.SpamDetection(text);
				resultJson = api.SpamDetection_ThirdParty(text);
                if (resultJson != "")
				{
                    AutoResponder.Library.SpamCheck.SpamcheckResult resultSPAM = null;
                    resultSPAM = (AutoResponder.Library.SpamCheck.SpamcheckResult)JsonConvert.DeserializeObject(resultJson, typeof(AutoResponder.Library.SpamCheck.SpamcheckResult));
					return resultSPAM.Message;
				}
				else
				{
					return "ERRO";
				}
			}
		}
	}
}