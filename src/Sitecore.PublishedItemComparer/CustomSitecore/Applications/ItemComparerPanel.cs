using System.Linq;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.SharedSource.PublishedItemComparer.Domain;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;

namespace Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomSitecore.Commands
{
	public class ItemComparerPanel : RibbonPanel
	{
		public override void Render(HtmlTextWriter output, Ribbon ribbon, Item button, CommandContext context)
		{
			//check to see if there is an item selected))
			if (context.Items.Count() != 1) return;

			//only use on authoring environment
			Item currentItem = context.Items[0];
			if (currentItem == null || currentItem.Database.Name.ToLower() != "master")
			{
				RenderText(output, "Only valid on the authoring environment.");
				return;
			}

			//compare item and set the text for the panel
			string text = "The item exists in the target database<br/>and passed all validation.";
			if (!ItemCompare.IsValid(currentItem))
				text = "The item did not pass validation, open the<br/>Item Comparer to view more details.";

			RenderText(output, text);
		}
	}
}