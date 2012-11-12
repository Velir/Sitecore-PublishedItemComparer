using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.SharedSource.PublishedItemComparer.Domain;

namespace Velir.SitecoreLibrary.Modules.PublishedItemComparer.Validations
{
	public class PresentationDetailValidator : Validator
	{
		public override List<string> Validate(ItemComparerContext context)
		{
			//defaults
			List<string> outputs = new List<string>();

			//check to see if the item exists in the target database which 
			//was specified in the settings item
			Item publishedItem = context.TargetDatabase.GetItem(context.Item.ID);
			if (publishedItem == null) return outputs;

			//check presentation details for each device
			//if there is no device folder, bypass check
			Item devicesFolder = context.Item.Database.GetItem("/sitecore/layout/Devices");
			if (devicesFolder == null) return outputs;

			foreach (Item descendant in devicesFolder.Axes.GetDescendants())
			{
				//verify we have a device item
				if (descendant.TemplateID.ToString() != "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}") continue;

				//check against layout
				DeviceItem deviceItem = descendant;
				if (context.Item.Visualization.GetLayoutID(deviceItem) != publishedItem.Visualization.GetLayoutID(deviceItem))
					outputs.Add(string.Format("The presentation layout for the {0} device does not match", deviceItem.InnerItem.Name));

				//get list of renderings in target db
				List<string> renderingsInTargetDb = new List<string>();
				foreach (RenderingReference renderingReference in publishedItem.Visualization.GetRenderings(deviceItem, false))
				{
					if (renderingReference.RenderingItem == null || string.IsNullOrEmpty(renderingReference.RenderingID.ToString()))
						continue;
					renderingsInTargetDb.Add(renderingReference.RenderingID.ToString());
				}

				//verify rendering exist and are set for this device in the target db
				List<string> renderingsInAuthoringDb = new List<string>();
				foreach (RenderingReference renderingReference in context.Item.Visualization.GetRenderings(deviceItem, false))
				{
					if (renderingReference.RenderingItem == null || string.IsNullOrEmpty(renderingReference.RenderingID.ToString()))
						continue;

					//add to list
					renderingsInAuthoringDb.Add(renderingReference.RenderingID.ToString());

					//check against target database
					if (!renderingsInTargetDb.Contains(renderingReference.RenderingID.ToString()))
						outputs.Add(
							string.Format("The rendering ({0}) is not set or does not exist in the {1} device in the target database",
							              renderingReference.RenderingItem.Name, deviceItem.InnerItem.Name));
				}

				//verify rendering exist and are set for this device in the authoring db - reverse validation
				foreach (RenderingReference renderingReference in publishedItem.Visualization.GetRenderings(deviceItem, false))
				{
					if (renderingReference.RenderingItem == null || string.IsNullOrEmpty(renderingReference.RenderingID.ToString()))
						continue;

					//check against authoring database
					if (!renderingsInAuthoringDb.Contains(renderingReference.RenderingID.ToString()))
						outputs.Add(
							string.Format("The rendering ({0}) is not set or does not exist in the {1} device in the authoring database",
							              renderingReference.RenderingItem.Name, deviceItem.InnerItem.Name));
				}
			}

			return outputs;
		}
	}
}