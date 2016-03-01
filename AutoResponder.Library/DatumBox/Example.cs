using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoResponder.Library.DatumBox
{
	public class Example
	{
		public void run()
		{
			/*
			 * Example of API Client for Datumbox Machine Learning API.
			 * 
			 * @version 1.0
			 * @author Vasilis Vryniotis
			 * @link   http://www.datumbox.com/
			 * @copyright Copyright (c) 2013, Datumbox.com
			 */

			DatumboxAPI DatumboxAPI = new DatumboxAPI();
            
			string text = "Google Search (or Google Web Search) is a web search engine owned by Google Inc. Google Search is the most-used search engine on the World Wide Web,[4] receiving several hundred million queries each day through its various services.[5] The order of search results on Google's search-results pages is based, in part, on a priority rank called a 'PageRank'. Google Search provides many options for customized search, using Boolean operators such as: exclusion ('-xx'), alternatives ('xx OR yy'), and wildcards ('x * x').[6] The main purpose of Google Search is to hunt for text in publicly accessible documents offered by web servers, as opposed to other data, such as with Google Image Search. Google Search was originally developed by Larry Page and Sergey Brin in 1997.[7] Google Search provides at least 22 special features beyond the original word-search capability.[8] These include synonyms, weather forecasts, time zones, stock quotes, maps, earthquake data, movie showtimes, airports, home listings, and sports scores. There are special features for dates, including ranges,[9] prices, temperatures, money/unit conversions, calculations, package tracking, patents, area codes,[8] and language translation of displayed pages. In June 2011, Google introduced 'Google Voice Search' and 'Search by Image' features for allowing the users to search words by speaking and by giving images.[10] In May 2012, Google introduced a new Knowledge Graph semantic search feature to customers in the U.S";    

			Console.WriteLine(DatumboxAPI.SentimentAnalysis(text));
			Console.WriteLine(DatumboxAPI.SubjectivityAnalysis(text));
			Console.WriteLine(DatumboxAPI.TopicClassification(text));
			Console.WriteLine(DatumboxAPI.SpamDetection(text));
			Console.WriteLine(DatumboxAPI.AdultContentDetection(text));
			Console.WriteLine(DatumboxAPI.ReadabilityAssessment(text));
			Console.WriteLine(DatumboxAPI.LanguageDetection(text));
			Console.WriteLine(DatumboxAPI.CommercialDetection(text));
			Console.WriteLine(DatumboxAPI.EducationalDetection(text));
			Console.WriteLine(DatumboxAPI.GenderDetection(text));

			string tweet = "I love the new Datumbox API! :)";
			Console.WriteLine(DatumboxAPI.TwitterSentimentAnalysis(tweet));

			string html = "<html>\n<body>\nI love\n<h1>\nMachine Learning\n</h1>\n</body>\n</html>";    

			Console.WriteLine(DatumboxAPI.TextExtraction(html));
			Console.WriteLine(DatumboxAPI.KeywordExtraction(text,3));

			string original = "This book has been written against a background of both reckless optimism and reckless despair. It holds that Progress and Doom are two sides of the same medal; that both are articles of superstition, not of faith. It was written out of the conviction that it should be possible to discover the hidden mechanics by which all traditional elements of our political and spiritual world were dissolved into a conglomeration where everything seems to have lost specific value, and has become unrecognizable for human comprehension, unusable for human purpose. Hannah Arendt, The Origins of Totalitarianism (New York: Harcourt Brace Jovanovich, Inc., 1973 ed.), p.vii, Preface to the First Edition.";

			string copy="Hannah Arendt's book, The Origins of Totalitarianism, was written in the light of both excessive hope and excessive pessimism. Her thesis is that both Advancement and Ruin are merely different sides of the same coin. Her book was produced out of a belief that one can understand the method in which the more conventional aspects of politics and philosophy were mixed together so that they lose their distinctiveness and become worthless for human uses.";

			Console.WriteLine(DatumboxAPI.DocumentSimilarity(original,copy));
		}
	}
}