using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AutoResponder
{
	/// <summary>
	/// DataTypeAttributeAdapter
	/// </summary>
	/// <source>http://weblogs.asp.net/srkirkland/archive/2011/02/15/adding-client-validation-to-dataannotations-datatype-attribute.aspx</source>
	public class DataTypeAttributeAdapter : DataAnnotationsModelValidator<DataTypeAttribute>
	{
		/// <summary>
		/// DataTypeAttributeAdapter
		/// </summary>
		/// <param name="metadata"></param>
		/// <param name="context"></param>
		/// <param name="attribute"></param>
		public DataTypeAttributeAdapter(ModelMetadata metadata, ControllerContext context, DataTypeAttribute attribute)
			: base(metadata, context, attribute) { }

		/// <summary>
		/// GetClientValidationRules
		/// </summary>
		/// <returns></returns>
		public override System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules()
		{
			if (Attribute.DataType == System.ComponentModel.DataAnnotations.DataType.Date)
			{
				return new[] { new ModelClientValidationDateRule(Attribute.FormatErrorMessage(Metadata.GetDisplayName())) };
			}
			/*
			if (Attribute.DataType == System.ComponentModel.DataAnnotations.DataType.DateTime)
			{
				return new[] { new ModelClientValidationDateTimeRule(Attribute.FormatErrorMessage(Metadata.GetDisplayName())) };
			}
			*/
			if (Attribute.DataType == System.ComponentModel.DataAnnotations.DataType.Custom && Attribute.CustomDataType.ToLower() == "date")
			{
				return new[] { new ModelClientValidationDateRule(Attribute.FormatErrorMessage(Metadata.GetDisplayName())) };
			}
			if (Attribute.DataType == System.ComponentModel.DataAnnotations.DataType.Custom && Attribute.CustomDataType.ToLower() == "email")
			{
				return new[] { new ModelClientValidationEmailRule(Attribute.FormatErrorMessage(Metadata.GetDisplayName())) };
			}

			
			return base.GetClientValidationRules();
		}
	}

	/// ModelClientValidationDateRule
	/// </summary>
	public class ModelClientValidationDateRule : ModelClientValidationRule
	{
		/// <summary>
		/// ModelClientValidationDateRule
		/// </summary>
		/// <param name="errorMessage"></param>
		public ModelClientValidationDateRule(string errorMessage)
		{
			ErrorMessage = errorMessage;
			ValidationType = "date";
		}
	}

	/// ModelClientValidationDateTimeRule
	/// </summary>
	public class ModelClientValidationDateTimeRule : ModelClientValidationRule
	{
		/// <summary>
		/// ModelClientValidationDateTimeRule
		/// </summary>
		/// <param name="errorMessage"></param>
		public ModelClientValidationDateTimeRule(string errorMessage)
		{
			ErrorMessage = errorMessage;
			ValidationType = "datetime";
		}
	}

	/// <summary>
	/// ModelClientValidationEmailRule
	/// </summary>
	public class ModelClientValidationEmailRule : ModelClientValidationRule
	{
		/// <summary>
		/// ModelClientValidationDateRule
		/// </summary>
		/// <param name="errorMessage"></param>
		public ModelClientValidationEmailRule(string errorMessage)
		{
			ErrorMessage = errorMessage;
			ValidationType = "email";
		}
	}
}
