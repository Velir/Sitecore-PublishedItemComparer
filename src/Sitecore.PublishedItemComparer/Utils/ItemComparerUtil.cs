using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.SharedSource.PublishedItemComparer.CustomItems.Common.ItemComparer;
using Sitecore.SharedSource.PublishedItemComparer.CustomItems.System;
using Sitecore.SharedSource.PublishedItemComparer.CustomSitecore;

namespace Sitecore.SharedSource.PublishedItemComparer.Utils
{
	public class ItemComparerUtil
	{
		public static string PassIcon = "/sitecore/shell/Themes/Standard/Network/16x16/earth_view.png";
		public static string FailIcon = "/sitecore/shell/Themes/Standard/Network/16x16/earth_delete.png";
		public static string ErrorIcon = "/sitecore/shell/Themes/Standard/Network/16x16/earth_location.png";

		/// <summary>
		/// 	Get the Target Database that is specified in the settings item
		/// </summary>
		/// <returns></returns>
		public static Database GetTargetDatabase()
		{
			//get settings item
			ItemComparerSettingsItem settingsItem = ItemComparerSettingsItem.GetSettingsItem();
			if (settingsItem == null) return null;

			//get the publishing target that is specified in the settings item
			PublishingTargetItem publishingTargetItem = settingsItem.DatabasetoCompareAgainst.TargetItem;
			if (publishingTargetItem == null) return null;

			//pull the database from the publishing target
			Database targetDatabase = publishingTargetItem.TargetDatabaseItem;
			if (targetDatabase == null) return null;

			return targetDatabase;
		}

		/// <summary>
		/// 	Validate the fields of the item against another
		/// </summary>
		/// <param name = "currentItem"></param>
		/// <param name = "publishedItem"></param>
		/// <returns></returns>
		public static bool Validate(Item currentItem, Item publishedItem)
		{
			CustomDiffViewer diffViewer = new CustomDiffViewer();
			return diffViewer.Compare(currentItem, publishedItem);
		}
	}
}