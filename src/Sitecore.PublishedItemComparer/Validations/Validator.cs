using System.Collections.Generic;
using Sitecore.SharedSource.PublishedItemComparer.Domain;

namespace Velir.SitecoreLibrary.Modules.PublishedItemComparer.Validations
{
	public abstract class Validator
	{
		public abstract List<string> Validate(ItemComparerContext context);
	}
}