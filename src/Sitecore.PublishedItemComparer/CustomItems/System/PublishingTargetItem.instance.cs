using Sitecore.Data;

namespace Sitecore.SharedSource.PublishedItemComparer.CustomItems.System
{
	public partial class PublishingTargetItem
	{
		public Database TargetDatabaseItem
		{
			get
			{
				if (string.IsNullOrEmpty(TargetDatabase)) return null;

				return Database.GetDatabase(TargetDatabase);
			}
		}
	}
}