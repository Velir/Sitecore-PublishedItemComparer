using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomItems.Common.ItemComparer;
using Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomItems.System;

namespace Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomSitecore.Commands
{
	public class TargetDatabaseCommand : Command
	{
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

			//get settings item
			ItemComparerSettingsItem settingsItem = ItemComparerSettingsItem.GetSettingsItem();
			if (settingsItem == null || settingsItem.DatabasetoCompareAgainst.TargetItem == null) return;

			PublishingTargetItem publishingTargetItem = settingsItem.DatabasetoCompareAgainst.TargetItem;
			if (publishingTargetItem.TargetDatabaseItem == null) return;

			Database targetDatabase = publishingTargetItem.TargetDatabaseItem;

			//Build the url for the control            
			string language = currentItem.Language.ToString();
			string version = currentItem.Version.ToString();

			// Add parameters to the UrlString
			UrlString parameters = new UrlString();
			parameters.Add("id", currentItem.ID.ToString());
			parameters.Add("fo", currentItem.ID.ToString());
			parameters.Add("la", language);
			parameters.Add("vs", version);
			parameters.Add("sc_content", targetDatabase.Name);

			// Run function that initializes the Content Editor; pass in necessary parameters
			Item contentEditor = Database.GetDatabase("core").GetItem("{7EADA46B-11E2-4EC1-8C44-BE75784FF105}");
			if (contentEditor == null) return;

			Sitecore.Shell.Framework.Windows.RunApplication(contentEditor, "/~/icon/People/16x16/cubes_blue.png",
			                                                "Item Comparer (" + targetDatabase.Name + ")", parameters.ToString());
		}
	}
}