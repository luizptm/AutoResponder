using System.Collections.Generic;

namespace AutoResponder.Library.DatumBox
{
	public class DatumboxResult
	{
		public DatumboxResult_inner output { get; set; }
	}

	public class DatumboxResult_inner
	{
		public string status { get; set; }
		public string result { get; set; }
	}

	/// <summary>
	/// http://www.datumbox.com/machine-learning-api/
	/// </summary>
	public class DatumboxAPI
	{
		private readonly string api_key = "1d566a43d35379b4533ea57fa9688c2f";

		public DatumboxAPI()
		{
			return;
		}

		public string SentimentAnalysis(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/SentimentAnalysis.json", arguments);
		}

		public string TwitterSentimentAnalysis(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/TwitterSentimentAnalysis.json", arguments);
		}

		public string SubjectivityAnalysis(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/SubjectivityAnalysis.json", arguments);
		}

		public string TopicClassification(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/TopicClassification.json", arguments);
		}

		public string SpamDetection(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/SpamDetection.json", arguments);
		}

		public string AdultContentDetection(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/AdultContentDetection.json", arguments);
		}

		public string ReadabilityAssessment(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/ReadabilityAssessment.json", arguments);
		}

		public string LanguageDetection(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/LanguageDetection.json", arguments);
		}

		public string CommercialDetection(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/CommercialDetection.json", arguments);
		}

		public string EducationalDetection(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/EducationalDetection.json", arguments);
		}

		public string GenderDetection(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/GenderDetection.json", arguments);
		}

		public string TextExtraction(string text)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/TextExtraction.json", arguments);
		}

		public string KeywordExtraction(string text, int n)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "text", text);
			Api.AddArgument(arguments, "n", n.ToString());

			return Api.SendPostRequest("http://api.datumbox.com/1.0/KeywordExtraction.json", arguments);
		}

		public string DocumentSimilarity(string original, string copy)
		{
			Dictionary<string, string> arguments = new Dictionary<string, string>();

			Api.AddArgument(arguments, "api_key", api_key);
			Api.AddArgument(arguments, "original", original);
			Api.AddArgument(arguments, "copy", copy);

			return Api.SendPostRequest("http://api.datumbox.com/1.0/DocumentSimilarity.json", arguments);
		}
	}
}