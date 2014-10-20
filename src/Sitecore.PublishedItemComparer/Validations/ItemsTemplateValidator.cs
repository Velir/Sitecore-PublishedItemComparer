using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.SharedSource.PublishedItemComparer.Domain;

namespace Sitecore.SharedSource.PublishedItemComparer.Validations
{
	public class ItemsTemplateValidator : Validator
	{
		public override List<string> Validate(ItemComparerContext context)
		{
			//do not run on templates
			if (context.Item.TemplateID.ToString() == "{AB86861A-6030-46C5-B394-E8F99E8B87DB}")
				return new List<string>();

			//defaults
			List<string> outputs = new List<string>();

			//check to see if the item exists in the target database which 
			//was specified in the settings item
			Item publishedItem = context.TargetDatabase.GetItem(context.Item.ID, context.Item.Language);
			if (publishedItem == null) return outputs;

			//we are not on a template item, lets check the item's template to make 
			//sure the template is published
			if (publishedItem.Template == null)
			{
				outputs.Add("The template is not published.");
				return outputs;
			}

			//need to reset the item in the context to represent the template item
			ItemComparerContext newContext = new ItemComparerContext();
			newContext.Item = context.Item.Template;
			newContext.ItemComparerSettingsItem = context.ItemComparerSettingsItem;
			newContext.TargetDatabase = context.TargetDatabase;

			//validating the items template
			TemplateValidator templateValidator = new TemplateValidator();
			outputs.AddRange(templateValidator.Validate(newContext));
			return outputs;
		}
	}
}