using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Text.Diff.View;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.WebControls;
using Control = System.Web.UI.Control;

namespace Sitecore.SharedSource.PublishedItemComparer.CustomSitecore
{
	public class CustomDiffViewer : TwoCoumnsDiffView
	{
		public bool Compare(Item item1, Item item2)
		{
			return Compare(new Control(), item1, item2, string.Empty);
		}

		public new bool Compare(Control parent, Item item1, Item item2, string click)
		{
			bool isItemTheSame = true;

			int num = 0;
			FieldCollection fields = item1.Fields;
			fields.ReadAll();
			fields.Sort();
			string section = null;
			Section child = null;
			GridPanel panel = null;
			foreach (Field field in fields)
			{
				if (!this.ShowField(field))
				{
					continue;
				}
				if (this.ShowField(field) && (section != field.Section))
				{
					child = new Section();
					parent.Controls.Add(child);
					child.Header = field.Section;
					child.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("S");
					panel = new GridPanel();
					child.Controls.Add(panel);
					panel.Columns = 2;
					panel.Width = new Unit(100.0, UnitType.Percentage);
					panel.Fixed = true;
					section = field.Section;
				}
				string str2 = base.GetValue(item1[field.Name]);
				string str3 = base.GetValue(item2[field.Name]);
				string str4 = (str2 == str3) ? "scUnchangedFieldLabel" : "scChangedFieldLabel";

				//item changed, set value
				if (str4 == "scChangedFieldLabel")
					isItemTheSame = false;

				if (field.IsBlobField)
				{
					if (str2 != str3)
					{
						str2 = "<span style=\"color:blue\">" + Translate.Text("Changed") + "</span>";
						str3 = "<span style=\"color:blue\">" + Translate.Text("Changed") + "</span>";
					}
					else
					{
						str2 = Translate.Text("Unable to compare binary fields.");
						str3 = Translate.Text("Unable to compare binary fields.");
					}
				}
				else
				{
					this.Compare(ref str2, ref str3);
				}
				str2 = (str2.Length == 0) ? "&#160;" : str2;
				str3 = (str3.Length == 0) ? "&#160;" : str3;
				Border border = new Border();
				panel.Controls.Add(border);
				panel.SetExtensibleProperty(border, "valign", "top");
				panel.SetExtensibleProperty(border, "width", "50%");
				border.Class = str4;
				border.Controls.Add(new LiteralControl(field.DisplayName + ":"));
				border = new Border();
				panel.Controls.Add(border);
				panel.SetExtensibleProperty(border, "valign", "top");
				panel.SetExtensibleProperty(border, "width", "50%");
				border.Class = str4;
				border.Controls.Add(new LiteralControl(field.DisplayName + ":"));
				border = new Border();
				panel.Controls.Add(border);
				panel.SetExtensibleProperty(border, "valign", "top");
				border.Class = "scField";
				if (field.Type == "checkbox")
				{
					str2 = "<input type=\"checkbox\" disabled" + ((str2 == "1") ? " checked" : "") + " />";
				}
				border.Controls.Add(new LiteralControl(str2));
				border = new Border();
				panel.Controls.Add(border);
				panel.SetExtensibleProperty(border, "valign", "top");
				border.Class = "scField";
				if (field.Type == "checkbox")
				{
					str3 = "<input type=\"checkbox\" disabled" + ((str3 == "1") ? " checked" : "") + " />";
				}
				border.Controls.Add(new LiteralControl(str3));
				num++;
			}

			return isItemTheSame;
		}
	}
}