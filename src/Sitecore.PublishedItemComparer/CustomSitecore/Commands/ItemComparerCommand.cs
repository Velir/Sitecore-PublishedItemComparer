using Sitecore;
using Sitecore.Data.Items;
using Sitecore.SharedSource.PublishedItemComparer.Domain;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using Velir.SitecoreLibrary.Modules.PublishedItemComparer.Util;

namespace Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomSitecore.Commands
{
	public class ItemComparerCommand : Command
	{
		/// <summary>
		/// 	Overriding the GetIcon method that Sitecore calls.
		/// 	Figures out which Icon and Tooltip Text to use and calls the 
		/// 	method to set them.
		/// </summary>
		/// <param name = "context"></param>
		/// <param name = "icon"></param>
		/// <returns></returns>
		public override string GetIcon(CommandContext context, string icon)
		{
			if (context.Items.Length == 0) return icon;

			//only use on authoring environment
			Item currentItem = context.Items[0];
			if (currentItem == null || currentItem.Database.Name.ToLower() != "master")
				return ItemComparerUtil.ErrorIcon;

			//validate the current sitecore context
			if (!ItemCompare.IsValid(currentItem))
				return ItemComparerUtil.FailIcon;

			return ItemComparerUtil.PassIcon;
		}

		/// <summary>
		/// 	Overriding the Execute method that Sitecore calls.
		/// </summary>
		/// <param name = "context"></param>
		public override void Execute(CommandContext context)
		{
			//check to see if there is an item selected)
			if (context.Items.Length != 1) return;

			//only use on authoring environment
			Item currentItem = context.Items[0];
			if (currentItem == null || currentItem.Database.Name.ToLower() != "master") return;

			//Build the url for the control            
			string controlUrl = UIUtil.GetUri("control:ItemComparerViewer");
			string id = currentItem.ID.ToString();
			string la = currentItem.Language.ToString();
			string vs = currentItem.Version.ToString();
			string[] args = {controlUrl, id, la, vs};
			string url = string.Format("{0}&id={1}&la={2}&vs={3}", args);

			//Open the dialog
			SheerResponse.CheckModified(false);
			SheerResponse.ShowModalDialog(new UrlString(url).ToString(), "500", "1000");
		}
	}
}