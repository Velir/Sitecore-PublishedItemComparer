using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Xml;
using Velir.SitecoreLibrary.Modules.PublishedItemComparer.Validations;

namespace Sitecore.SharedSource.PublishedItemComparer.Domain
{
	public class ValidationItem
	{
		public string Type { get; set; }
		public string Namespace { get; set; }
		public string AssemblyName { get; set; }

		public string CacheName
		{
			get
			{
				string nameSpace = string.Empty;
				if (!string.IsNullOrEmpty(Namespace))
				{
					nameSpace = Namespace;
				}

				return string.Format("PublishedItemComparer.Validator.{0}", nameSpace);
			}
		}

		public List<string> Validate(ItemComparerContext context)
		{
			//set parameter to be passed to validation method
			Object[] args = new object[1];
			args[0] = context;

			Validator validator = null;
			if (HttpContext.Current.Cache[CacheName] != null)
			{
				validator = (Validator) HttpContext.Current.Cache[CacheName];
			}
			else
			{
				// load the assemly
				Assembly assembly = GetAssembly(this.AssemblyName);

				// Walk through each type in the assembly looking for our class
				Type type = assembly.GetType(this.Namespace);
				if (type == null || !type.IsClass) return new List<string>();

				//cast to validator class
				validator = (Validator)Activator.CreateInstance(type);
				HttpContext.Current.Cache[CacheName] = validator;
			}

			if (validator == null)
			{
				return new List<string>();
			}

			//validate
			return validator.Validate(context);
		}

		private static Assembly GetAssembly(string AssemblyName)
		{
			//try and find it in the currently loaded assemblies
			AppDomain appDomain = AppDomain.CurrentDomain;
			foreach (Assembly assembly in appDomain.GetAssemblies())
			{
				if (assembly.FullName == AssemblyName)
					return assembly;
			}

			//load assembly
			return appDomain.Load(AssemblyName);
		}

		public static ValidationItem GetItem_FromXmlNode(XmlNode validationNode)
		{
			if (validationNode.Name != "validation") return null;

			ValidationItem validationItem = new ValidationItem();
			validationItem.Type = validationNode.Attributes["type"].Value;

			//check to verify that xml was not malformed
			if (string.IsNullOrEmpty(validationItem.Type)) return null;

			//verify we can break up the type string into a namespace and assembly name
			string[] split = validationItem.Type.Split(',');
			if (split.Length == 0) return null;

			validationItem.Namespace = split[0];
			validationItem.AssemblyName = split[1];

			return validationItem;
		}
	}
}