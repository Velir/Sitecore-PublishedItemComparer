using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.PublishedItemComparer.CustomItems.Common.ItemComparer
{
	public partial class ItemComparerSettingsItem : CustomItem
	{
		public static readonly string TemplateId = "{34CA12A7-4CC3-4313-BEE4-95133965F176}";

		#region Boilerplate CustomItem Code

		public ItemComparerSettingsItem(Item innerItem)
			: base(innerItem)
		{
		}

		public static implicit operator ItemComparerSettingsItem(Item innerItem)
		{
			return innerItem != null ? new ItemComparerSettingsItem(innerItem) : null;
		}

		public static implicit operator Item(ItemComparerSettingsItem customItem)
		{
			return customItem != null ? customItem.InnerItem : null;
		}

		#endregion //Boilerplate CustomItem Code

		#region Field Instance Methods

		public ReferenceField DatabasetoCompareAgainst
		{
			get { return InnerItem.Fields["Database to Compare Against"]; }
		}

		#endregion //Field Instance Methods
	}
}