using System;
using System.Collections.Generic;
using System.Xml;
using log4net;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomItems.Common.ItemComparer;
using Velir.SitecoreLibrary.Modules.PublishedItemComparer.Util;

namespace Sitecore.SharedSource.PublishedItemComparer.Domain
{
	public class ItemCompare
	{
		/// <summary>
		/// Is the item compliant with the validations
		/// </summary>
		/// <param name = "item"></param>
		/// <returns></returns>
		public static bool IsValid(Item item)
		{
			return Validate(item, true).Count == 0;
		}

		/// <summary>
		/// Validate Item
		/// </summary>
		/// <param name = "item"></param>
		/// <returns></returns>
		public static List<string> Validate(Item item)
		{
			return Validate(item, false);
		}

		/// <summary>
		/// Validate Item
		/// </summary>
		/// <param name = "item"></param>
		/// <param name = "returnOnFirstFailure">return on first failed validation</param>
		/// <returns></returns>
		private static List<string> Validate(Item item, bool returnOnFirstFailure)
		{
			//validation list
			List<string> validations = new List<string>();

			XmlNode itemComparerNode = Factory.GetConfigNode("itemComparer");
			if (itemComparerNode == null)
			{
				Logger.Error("Published Item Comparer: Could not find ItemComparer Configuration Section");
				return validations;
			}

			//verify settings item exists
			ItemComparerSettingsItem settingsItem = ItemComparerSettingsItem.GetSettingsItem();
			if (settingsItem == null)
			{
				Logger.Error("Published Item Comparer: The Settings Item Could not be retrieved.");
				validations.Add("The Settings Item Could not be retrieved.");
				return validations;
			}

			//verify target database
			Database targetDatabase = ItemComparerUtil.GetTargetDatabase();
			if (targetDatabase == null)
			{
				Logger.Error("Published Item Comparer: The Target Database Could not be retrieved.");
				validations.Add("The Target Database Could not be retrieved.");
				return validations;
			}

			ItemComparerContext context = new ItemComparerContext();
			context.Item = item;
			context.ItemComparerSettingsItem = settingsItem;
			context.TargetDatabase = targetDatabase;

			//get validations
			DateTime start = DateTime.Now;
			foreach (XmlNode validationNode in itemComparerNode.ChildNodes)
			{
				ValidationItem validationItem = ValidationItem.GetItem_FromXmlNode(validationNode);
				if (validationItem == null)
				{
					Logger.Error("Published Item Comparer: Validation Node is Null");
					continue;
				}

				//validate item
				List<string> output = validationItem.Validate(context);

				//add output of validation to the master validation list
				validations.AddRange(output);

				//break on first failed validation
				if (returnOnFirstFailure && output.Count > 0)
				{
					break;
				}
			}

			TimeSpan timeSpan = DateTime.Now - start;
			Logger.Debug("PublishedItemComparer - ItemId: " + item.ID);
			Logger.Debug("PublishedItemComparer - Seconds to Validate Item: " + timeSpan.TotalSeconds);
			Logger.Debug(" ");

			//return all output validation;
			return validations;
		}

		private static ILog _logger;
		private static ILog Logger
		{
			get
			{
				if (_logger == null)
				{
					_logger = LogManager.GetLogger(typeof(ItemCompare));
				}
				return _logger;
			}
		}
	}
}