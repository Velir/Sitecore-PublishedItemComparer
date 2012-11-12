using Sitecore.Data;

namespace Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomItems.System
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