using Sitecore.Data;
using Sitecore.Data.Items;
using Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomItems.Common.ItemComparer;

namespace Sitecore.SharedSource.PublishedItemComparer.Domain
{
	public class ItemComparerContext
	{
		public Item Item { get; set; }
		public ItemComparerSettingsItem ItemComparerSettingsItem { get; set; }
		public Database TargetDatabase { get; set; }
	}
}