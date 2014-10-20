using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.SharedSource.PublishedItemComparer.Domain;
using Sitecore.SharedSource.PublishedItemComparer.Utils;

namespace Sitecore.SharedSource.PublishedItemComparer.Validations
{
	public class AncestorValidator : Validator
	{
		public override List<string> Validate(ItemComparerContext context)
		{
			//defaults
			List<string> outputs = new List<string>();

			//check ancestors to see if they exist in the target database
			foreach (Item ancestorItem in context.Item.Axes.GetAncestors())
			{
				Item targetAncestorItem = context.TargetDatabase.GetItem(ancestorItem.ID, context.Item.Language);
				if (targetAncestorItem == null)
				{
					outputs.Add(string.Format("The ancestor ({0}) does not exist in the target database", ancestorItem.Name));
				}
				else
				{
					//item exists in the target database, comparing fields
					if (!ItemComparerUtil.Validate(ancestorItem, targetAncestorItem))
					{
						//there are differences between the two items
						outputs.Add(string.Format("The ancestor ({0}) exists in the target database but the fields are different.",
						                          ancestorItem.Name));
					}
				}
			}

			return outputs;
		}
	}
}