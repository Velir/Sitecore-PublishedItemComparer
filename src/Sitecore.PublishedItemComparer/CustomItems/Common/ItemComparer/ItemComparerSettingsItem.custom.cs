using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.PublishedItemComparer.CustomItems.Common.ItemComparer
{
	public partial class ItemComparerSettingsItem
	{
		/// <summary>
		/// 	Returns the Custom Item for the Settings Item
		/// </summary>
		/// <returns>PublishedItemComparerSettingsItem</returns>
		public static ItemComparerSettingsItem GetSettingsItem()
		{
			Database database = Client.ContentDatabase;
			if (database == null) return null;

			Item item = database.GetItem("/sitecore/system/Modules/Item Comparer Settings");
			if (item == null) return null;

			return item;
		}
	}
}