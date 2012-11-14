using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SharedSource.PublishedItemComparer.Domain;
using Sitecore.SharedSource.PublishedItemComparer.Utils;

namespace Sitecore.SharedSource.PublishedItemComparer.Validations
{
	public class TemplateValidator : Validator
	{
		public override List<string> Validate(ItemComparerContext context)
		{
			//only run on templates
			if (context.Item.TemplateID.ToString() != "{AB86861A-6030-46C5-B394-E8F99E8B87DB}") return new List<string>();

			//defaults
			List<string> outputs = new List<string>();

			//check to see if the item exists in the target database which 
			//was specified in the settings item
			Item publishedItem = context.TargetDatabase.GetItem(context.Item.ID);
			if (publishedItem == null) return outputs;

			if (!ItemComparerUtil.Validate(context.Item, publishedItem))
				outputs.Add(string.Format("The template item ({0}) exists in the target database but is different.",
				                          context.Item.Name));

			//check base templates
			TemplateItem currentTemplateItem = context.Item;

			//get list of base templates from current template item
			List<string> currentBaseTemplates = new List<string>();
			foreach (Item baseItem in currentTemplateItem.BaseTemplates)
				currentBaseTemplates.Add(baseItem.ID.ToString());

			//get list of base templates from published template item
			List<string> publishedBaseTemplates = new List<string>();
			foreach (Item baseItem in ((TemplateItem) publishedItem).BaseTemplates)
				publishedBaseTemplates.Add(baseItem.ID.ToString());

			//check to see if all base templates exist on the template in the target database
			foreach (string baseTemplateId in currentBaseTemplates)
			{
				if (!publishedBaseTemplates.Contains(baseTemplateId))
				{
					Item baseTemplateItem = context.TargetDatabase.GetItem(baseTemplateId);
					if (baseTemplateItem == null)
					{
						baseTemplateItem = Database.GetDatabase("master").GetItem(baseTemplateId);
						outputs.Add(string.Format("The base template ({0}) does not exist in the target database.", baseTemplateItem.Name));
					}
					else
						outputs.Add(string.Format("The base template ({0}) is not selected in the target template item.",
						                          baseTemplateItem.Name));
				}
			}

			//check to see if all target base templates exist on the template in the current database
			foreach (string baseTemplateId in publishedBaseTemplates)
			{
				if (!publishedBaseTemplates.Contains(baseTemplateId))
				{
					Item baseTemplateItem = context.TargetDatabase.GetItem(baseTemplateId);
					outputs.Add(string.Format("The target base template ({0}) is not selected in the current template item.",
					                          baseTemplateItem.Name));
				}
			}

			foreach (Item descendant in context.Item.Axes.GetDescendants())
			{
				//check children of template, verify descendant exists in target database
				Item publishedDescendantItem = context.TargetDatabase.GetItem(descendant.ID);
				if (publishedDescendantItem == null)
					outputs.Add(string.Format("The template descendant ({0}) does not exist in the target database.", descendant.Name));
				else
				{
					//check name
					if (descendant.Name != publishedDescendantItem.Name)
						outputs.Add(string.Format("The template descendant's name ({0}) is different from the target item's name ({1}).",
						                          descendant.Name, publishedDescendantItem.Name));

					//if the item is the same, pass validation
					if (!ItemComparerUtil.Validate(descendant, publishedDescendantItem))
						outputs.Add(string.Format("The template descendant ({0}) is not published.", descendant.Name));
				}
			}

			return outputs;
		}
	}
}