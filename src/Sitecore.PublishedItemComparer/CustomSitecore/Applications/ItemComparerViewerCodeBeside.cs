using System;
using System.Collections.Generic;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.SharedSource.PublishedItemComparer.CustomItems.Common.ItemComparer;
using Sitecore.SharedSource.PublishedItemComparer.CustomItems.System;
using Sitecore.SharedSource.PublishedItemComparer.Domain;
using Sitecore.Text;
using Sitecore.Text.Diff.View;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Version = Sitecore.Data.Version;

namespace Sitecore.SharedSource.PublishedItemComparer.CustomSitecore.Applications
{
	public class ItemComparerViewerCodeBeside : BaseForm
	{
		#region Fields

		protected Button Cancel;
		protected GridPanel Grid;
		protected Button OK;
		protected Combobox Version1;
		protected Combobox PublishingTargets;
		protected Placeholder comparerPlaceholder;
		protected Literal infoLiteral;
		protected Literal nameLiteral;
		protected Literal itemPathLiteral;
		protected Literal templatePathLiteral;
		protected Literal createdLiteral;
		protected Literal createdByLiteral;
		protected Literal lastModifiedLiteral;
		protected Literal lastModifiedByLiteral;
		protected Button publishButton;
		protected Button viewTargetItemButton;

		#endregion Fields

		#region Page Events

		protected override void OnLoad(EventArgs eventArgs)
		{
			Assert.ArgumentNotNull(eventArgs, "eventArgs");
			base.OnLoad(eventArgs);

			//If not a Client Event then attempt to load item id and language from QueryString
			if (!Context.ClientPage.IsEvent)
			{
				Context.ClientPage.ServerProperties["id"] = WebUtil.GetQueryString("id");
				Context.ClientPage.ServerProperties["language"] = WebUtil.GetQueryString("la", Language.Current.Name);
			}

			//Set the Custom Page Events
			this.Version1.OnChange += new EventHandler(this.OnUpdate);
			this.PublishingTargets.OnChange += new EventHandler(this.OnUpdate);
			this.OK.OnClick += new EventHandler(ItemComparerViewerCodeBeside.OnOK);
			this.Cancel.OnClick += new EventHandler(ItemComparerViewerCodeBeside.OnCancel);
			this.publishButton.OnClick += new EventHandler(publishButton_OnClick);
			this.publishButton.OnClick += new EventHandler(publishButton_OnClick);
			this.viewTargetItemButton.OnClick += new EventHandler(viewTargetItemButton_OnClick);
			this.viewTargetItemButton.Width = 150;
		}

		protected override void OnPreRender(EventArgs eventArgs)
		{
			comparerPlaceholder.Visible = false;
			Assert.ArgumentNotNull(eventArgs, "eventArgs");
			if (eventArgs == null)
			{
				SheerResponse.Alert("eventArgs is null");
				return;
			}

			//If this is a client page event then exit
			if (Context.ClientPage.IsEvent)
			{
				return;
			}

			string queryString = WebUtil.GetQueryString("id");
			string lang = WebUtil.GetQueryString("la", Language.Current.Name);
			string str3 = WebUtil.GetQueryString("vs", "0");
			Item item = Database.GetDatabase("master").GetItem(queryString, Language.Parse(lang), Version.Parse(str3));
			if (item == null)
			{
				SheerResponse.Alert("Item is null");
				return;
			}

			//set item validation field
			SetValidationField(item);

			//set fields
			SetItemInformation(item);

			//Compare
			CompareItem(item);
		}

		#endregion

		#region Click Events

		protected void viewTargetItemButton_OnClick(object sender, EventArgs e)
		{
			string queryString = WebUtil.GetQueryString("id");
			string language = WebUtil.GetQueryString("la", Language.Current.Name);
			string version = WebUtil.GetQueryString("vs", "0");
			Item item = Database.GetDatabase("master").GetItem(queryString, Language.Parse(language), Version.Parse(version));
			if (item == null)
			{
				SheerResponse.Alert("Item is null");
				return;
			}

			//get settings item
			ItemComparerSettingsItem settingsItem = ItemComparerSettingsItem.GetSettingsItem();
			if (settingsItem == null || settingsItem.DatabasetoCompareAgainst.TargetItem == null) return;

			PublishingTargetItem publishingTargetItem = settingsItem.DatabasetoCompareAgainst.TargetItem;
			if (publishingTargetItem.TargetDatabaseItem == null) return;

			Database targetDatabase = publishingTargetItem.TargetDatabaseItem;

			// Add parameters to the UrlString
			UrlString parameters = new UrlString();
			parameters.Add("id", item.ID.ToString());
			parameters.Add("fo", item.ID.ToString());
			parameters.Add("la", language);
			parameters.Add("vs", version);
			parameters.Add("sc_content", targetDatabase.Name);

			// Run function that initializes the Content Editor; pass in necessary parameters
			Item contentEditor = Database.GetDatabase("core").GetItem("{7EADA46B-11E2-4EC1-8C44-BE75784FF105}");
			if (contentEditor == null) return;

			Sitecore.Shell.Framework.Windows.RunApplication(contentEditor, "/~/icon/People/16x16/cubes_blue.png",
			                                                "Item Comparer (" + targetDatabase.Name + ")", parameters.ToString());
		}

		protected void publishButton_OnClick(object sender, EventArgs e)
		{
			string queryString = WebUtil.GetQueryString("id");
			string lang = WebUtil.GetQueryString("la", Language.Current.Name);
			string str3 = WebUtil.GetQueryString("vs", "0");
			Item item = Database.GetDatabase("master").GetItem(queryString, Language.Parse(lang), Version.Parse(str3));
			if (item == null)
			{
				SheerResponse.Alert("Item is null");
				return;
			}

			if (item.Database.Name != "master") return;

			PublishOptions options = new PublishOptions(item.Database, Factory.GetDatabase("web"), PublishMode.SingleItem,
			                                            item.Language, DateTime.Now);
			options.RootItem = item;

			//if its a template item, publish children
			options.Deep = (item.Paths.FullPath.StartsWith("/sitecore/templates") &&
			                item.TemplateID.ToString() == "{AB86861A-6030-46C5-B394-E8F99E8B87DB}");

			//begin publishing
			new Publisher(options).Publish();

			//reload item
			item = item.Database.GetItem(item.ID);

			//set item validation field
			SetValidationField(item);

			//set fields
			SetItemInformation(item);

			//Compare
			CompareItem(item);
		}

		#endregion

		/// <summary>
		/// 	Sets field information for the current item
		/// </summary>
		/// <param name = "item"></param>
		private void SetItemInformation(Item item)
		{
			nameLiteral.Text = item.Name;
			itemPathLiteral.Text = item.Paths.FullPath;
			templatePathLiteral.Text = item.TemplateName;
			createdLiteral.Text = item.Statistics.Created.ToString();
			createdByLiteral.Text = item.Statistics.CreatedBy;
			lastModifiedLiteral.Text = item.Statistics.Updated.ToString();
			lastModifiedByLiteral.Text = item.Statistics.UpdatedBy;
		}

		/// <summary>
		/// 	Sets the bulleted list of validation outputs
		/// </summary>
		/// <param name = "item"></param>
		private void SetValidationField(Item item)
		{
			List<string> outputs = ItemCompare.Validate(item);
			if (outputs.Count == 0) return;

			infoLiteral.Text = "<ul>";
			foreach (string validation in outputs)
				infoLiteral.Text += string.Format("<li>{0}</li>", validation);

			infoLiteral.Text += "</ul>";
		}

		/// <summary>
		/// 	Compare and show results
		/// </summary>
		/// <param name = "item"></param>
		private void CompareItem(Item item)
		{
			//set placeholder's visibility to false by default
			comparerPlaceholder.Visible = false;

			//verify we have versions for this item
			Version[] versionNumbers = item.Versions.GetVersionNumbers();
			if (versionNumbers.Length == 0)
			{
				SheerResponse.Alert("There are no versions for this item");
				return;
			}

			//set the versions combobox
			SetVersionsComboBox(versionNumbers, item);

			//Set the publishing targets combobox
			IList<Item> publishingTargets = GetPublishingTargets();
			if (publishingTargets.Count == 0)
			{
				SheerResponse.Alert("There are no publishing targets.");
				return;
			}

			//Set the publishing targets combobox
			SetPublishingTargetsComboBox(publishingTargets);

			ListItem selectedItem = this.Version1.SelectedItem;
			ListItem publishingTargetSelectedItem = this.PublishingTargets.SelectedItem;
			if ((selectedItem != null) && (publishingTargetSelectedItem != null))
			{
				this.Compare(selectedItem.Value, publishingTargetSelectedItem.Value);
			}
			comparerPlaceholder.Visible = true;
		}

		/// <summary>
		/// 	Fills the Versions ComboBox.
		/// </summary>
		/// <param name = "versionNumbers"></param>
		/// <param name = "item"></param>
		private void SetVersionsComboBox(Version[] versionNumbers, Item item)
		{
			//Set number to the current version number
			int number = item.Version.Number;

			//Set the version combobox
			for (int i = versionNumbers.Length - 1; i >= 0; i--)
			{
				Version version = versionNumbers[i];
				ListItem child = new ListItem();
				this.Version1.Controls.Add(child);
				child.ID = Control.GetUniqueID("ListItem");
				child.Header = version.Number.ToString();
				child.Value = version.Number.ToString();
				if (version.Number == number)
				{
					child.Selected = true;
				}
			}
		}

		/// <summary>
		/// 	Fills the Publishing Targets ComboBox.
		/// </summary>
		/// <param name = "publishingTargets"></param>
		private void SetPublishingTargetsComboBox(IList<Item> publishingTargets)
		{
			string targetDatabaseFieldName = "Target database";

			int count = 0;
			foreach (Item publishingTarget in publishingTargets)
			{
				count++;

				//Skip this entry if Target Database field is not set
				if (String.IsNullOrEmpty(publishingTarget[targetDatabaseFieldName]))
				{
					continue;
				}

				//Add Publishing target to list
				ListItem child = new ListItem();
				child = new ListItem();
				this.PublishingTargets.Controls.Add(child);
				child.ID = Control.GetUniqueID("ListItem");
				child.Header = publishingTarget.DisplayName;
				child.Value = publishingTarget[targetDatabaseFieldName];

				//Default selection to last target in the list
				if (count == publishingTargets.Count)
				{
					child.Selected = true;
				}
			}
		}

		/// <summary>
		/// 	Executes the compare of two items.
		/// </summary>
		/// <param name = "version1"></param>
		/// <param name = "publishingTargetDatabase"></param>
		private void Compare(string version1, string publishingTargetDatabase)
		{
			//Define the DiffView as a two column 
			DiffView view;
			view = new TwoCoumnsDiffView();

			//check that version 1 and publishignTargetDatabase values are not null
			Assert.ArgumentNotNull(version1, "version1");
			Assert.ArgumentNotNull(publishingTargetDatabase, "publishingTargetDatabase");

			//Retrieve the Current Items Item ID and Language
			string id = Context.ClientPage.ServerProperties["id"] as string;
			string lang = Context.ClientPage.ServerProperties["language"] as string;

			//Retrieve the items from the current database
			Item item = Context.ContentDatabase.Items[id, Language.Parse(lang), Version.Parse(version1)];
			if (item == null)
			{
				SheerResponse.Alert(string.Format("Item is null: {0}", id));
				return;
			}

			Database database = Factory.GetDatabase(publishingTargetDatabase);
			if (database == null)
			{
				SheerResponse.Alert(string.Format("Database is null: {0}.  Check the settings item.", publishingTargetDatabase));
				return;
			}

			//the item might be null because of publishing constraints such as workflow
			//or the item just isn't published yet.
			Item item2 = database.Items[id, Language.Parse(lang)];
			if (item2 == null)
			{
				return;
			}

			//clear the comparer grid
			this.Grid.Controls.Clear();

			//compare the items
			string results = view.Compare(this.Grid, item, item2, string.Empty).ToString();
			if (this.Grid == null)
			{
				return;
			}

			//reset the contents of the comparer grid
			Context.ClientPage.ClientResponse.SetOuterHtml("Grid", this.Grid);
		}

		/// <summary>
		/// 	Process currently selected item versions and refresh the dialog window
		/// </summary>
		private void Refresh()
		{
			ListItem itemOne = this.Version1.SelectedItem;
			ListItem itemTwo = this.PublishingTargets.SelectedItem;
			if ((itemOne != null) && (itemTwo != null))
			{
				this.Grid.Controls.Clear();
				this.Compare(itemOne.Value, itemTwo.Value);
				Context.ClientPage.ClientResponse.SetOuterHtml("Grid", this.Grid);
			}
		}

		/// <summary>
		/// 	Returns a list of publishing target items
		/// </summary>
		/// <returns></returns>
		private IList<Item> GetPublishingTargets()
		{
			List<Item> publishingTargets = new List<Item>();

			Guid publishingTargetItemGuid = new Guid("E130C748-C13B-40D5-B6C6-4B150DC3FAB3");
			string publishingTargetsPath = "/sitecore/system/publishing targets";

			//Retrieve list of publishing target items from the Publishing Targets folder in 
			//Sitecore
			Item publishingTargetsFolder = Context.ContentDatabase.GetItem(publishingTargetsPath);
			if (publishingTargetsFolder == null)
			{
				return new List<Item>();
			}

			ChildList targets = publishingTargetsFolder.GetChildren();
			foreach (Item target in targets)
			{
				//Check that this is a valid publishing target item
				if (target.Template.ID.Guid.Equals(publishingTargetItemGuid))
				{
					publishingTargets.Add(target);
				}
			}

			return publishingTargets;
		}

		/// <summary>
		/// 	Closes the dialog window
		/// </summary>
		/// <param name = "sender"></param>
		/// <param name = "eventArgs"></param>
		private static void OnCancel(object sender, EventArgs eventArgs)
		{
			Assert.ArgumentNotNull(sender, "sender");
			Assert.ArgumentNotNull(eventArgs, "eventArgs");
			Context.ClientPage.ClientResponse.CloseWindow();
		}

		/// <summary>
		/// 	Closes the dialog window
		/// </summary>
		/// <param name = "sender"></param>
		/// <param name = "eventArgs"></param>
		private static void OnOK(object sender, EventArgs eventArgs)
		{
			Assert.ArgumentNotNull(sender, "sender");
			Assert.ArgumentNotNull(eventArgs, "eventArgs");
			Context.ClientPage.ClientResponse.CloseWindow();
		}

		/// <summary>
		/// 	Refresh the dialog window
		/// </summary>
		/// <param name = "sender"></param>
		/// <param name = "eventArgs"></param>
		private void OnUpdate(object sender, EventArgs eventArgs)
		{
			Assert.ArgumentNotNull(sender, "sender");
			Assert.ArgumentNotNull(eventArgs, "eventArgs");
			this.Refresh();
		}
	}
}