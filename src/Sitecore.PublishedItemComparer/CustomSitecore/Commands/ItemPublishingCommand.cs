using System.Collections.Specialized;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;

namespace Velir.SitecoreLibrary.Modules.PublishedItemComparer.CustomSitecore.Commands
{
	public class ItemPublishingCommand : Command
	{
		// Methods
		private static bool CheckWorkflow(ClientPipelineArgs args, Item item)
		{
			Assert.ArgumentNotNull(args, "args");
			Assert.ArgumentNotNull(item, "item");
			if (args.Parameters["workflow"] == "1")
			{
				return true;
			}
			args.Parameters["workflow"] = "1";
			if (args.IsPostBack)
			{
				if (args.Result == "yes")
				{
					args.IsPostBack = false;
					return true;
				}
				args.AbortPipeline();
				return false;
			}
			IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
			if ((workflowProvider == null) || (workflowProvider.GetWorkflows().Length <= 0))
			{
				return true;
			}
			IWorkflow workflow = workflowProvider.GetWorkflow(item);
			if (workflow == null)
			{
				return true;
			}
			WorkflowState state = workflow.GetState(item);
			if (state == null)
			{
				return true;
			}
			if (state.FinalState)
			{
				return true;
			}
			args.Parameters["workflow"] = "0";
			SheerResponse.Confirm(
				Translate.Text(
					"The current item \"{0}\" is in the workflow state \"{1}\"\nand will not be published.\n\nAre you sure you want to publish?",
					new object[] {item.DisplayName, state.DisplayName}));
			args.WaitForPostBack();
			return false;
		}

		public override void Execute(CommandContext context)
		{
			if (context.Items.Length == 1)
			{
				Item item = context.Items[0];
				NameValueCollection parameters = new NameValueCollection();
				parameters["id"] = item.ID.ToString();
				parameters["language"] = item.Language.ToString();
				parameters["version"] = item.Version.ToString();
				parameters["workflow"] = "0";
				Context.ClientPage.Start(this, "Run", parameters);
			}
		}

		public override CommandState QueryState(CommandContext context)
		{
			if (context.Items.Length != 1)
			{
				return CommandState.Hidden;
			}
			return base.QueryState(context);
		}

		protected void Run(ClientPipelineArgs args)
		{
			Assert.ArgumentNotNull(args, "args");
			string str = args.Parameters["id"];
			string name = args.Parameters["language"];
			string str3 = args.Parameters["version"];
			if (SheerResponse.CheckModified())
			{
				Item item = Context.ContentDatabase.Items[str, Language.Parse(name), Sitecore.Data.Version.Parse(str3)];
				if (item == null)
				{
					SheerResponse.Alert("Item not found.", new string[0]);
				}
				else if (CheckWorkflow(args, item))
				{
					Log.Audit(this, "Publish item: {0}", new string[] {AuditFormatter.FormatItem(item)});
					Items.Publish(item);
					Sitecore.Context.ClientPage.SendMessage(this, string.Format("item:refresh(id={0})", item.ID));
					Sitecore.Context.ClientPage.SendMessage(this, string.Format("item:load(id={0})", item.ID));
				}
			}
		}
	}
}