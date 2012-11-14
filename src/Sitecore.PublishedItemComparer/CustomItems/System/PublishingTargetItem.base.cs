using Sitecore.Data.Items;

namespace Sitecore.SharedSource.PublishedItemComparer.CustomItems.System
{
	public partial class PublishingTargetItem : CustomItem
	{
		public static readonly string TemplateId = "{E130C748-C13B-40D5-B6C6-4B150DC3FAB3}";

		#region Boilerplate CustomItem Code

		public PublishingTargetItem(Item innerItem) : base(innerItem)
		{
		}

		public static implicit operator PublishingTargetItem(Item innerItem)
		{
			return innerItem != null ? new PublishingTargetItem(innerItem) : null;
		}

		public static implicit operator Item(PublishingTargetItem customItem)
		{
			return customItem != null ? customItem.InnerItem : null;
		}

		#endregion //Boilerplate CustomItem Code

		#region Field Instance Methods

		public string TargetDatabase
		{
			get { return InnerItem["Target Database"]; }
		}

		#endregion //Field Instance Methods
	}
}