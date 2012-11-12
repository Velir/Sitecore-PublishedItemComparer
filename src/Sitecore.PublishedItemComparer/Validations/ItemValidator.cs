using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.SharedSource.PublishedItemComparer.Domain;
using Velir.SitecoreLibrary.Modules.PublishedItemComparer.Util;

namespace Velir.SitecoreLibrary.Modules.PublishedItemComparer.Validations
{
	public class ItemValidator : Validator
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
			Item publishedItem = context.TargetDatabase.GetItem(context.Item.ID);
			if (publishedItem == null)
			{
				outputs.Add("The item does not exist in the target database.");
				return outputs;
			}

			//we are not on a template, compare only the current item
			//if the item is the same, pass validation
			if (!ItemComparerUtil.Validate(context.Item, publishedItem))
			{
				//there are differences between the two items
				outputs.Add("The item exists in the target database but the fields are different.");
			}

			//check to see if they live in different locations
			if (context.Item.Paths.FullPath != publishedItem.Paths.FullPath)
			{
				outputs.Add("The item's path is different than the path in the target database.");
			}

			return outputs;
		}
	}
}