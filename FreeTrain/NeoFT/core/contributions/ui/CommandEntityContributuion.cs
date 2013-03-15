using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using nft.ui.command;
using nft.framework;
using nft.framework.plugin;
using nft.util;

namespace nft.contributions.ui
{
	/// <summary>
	/// CommandEntityContributuion の概要の説明です。
	/// </summary>
	public class CommandEntityContributuion : Contribution
	{
		protected enum MethodType { Static, Instance, Constructor };
		protected readonly string commandType;
		protected readonly ICommandEntity entity;
		protected readonly Type targetType;
		protected readonly MethodType methodType;
		protected readonly string methodName;
		private string path;

		public CommandEntityContributuion( XmlElement contrib ) : base(contrib)
		{
			path = contrib.OwnerDocument.BaseURI;
			XmlNode cls = XmlUtil.selectSingleNode(contrib,"class");
			targetType = PluginUtil.loadTypeFromManifest((XmlElement)cls);

			XmlNode mn = contrib.SelectSingleNode("method");
			XmlNode cmd = XmlUtil.selectSingleNode( contrib, "command" );
			commandType =  XmlUtil.getAttributeValue( cmd, "type", "entity" );

			if( mn!=null )
			{
				methodName = XmlUtil.getAttributeValue( mn, "name", null);
				if( bool.Parse(XmlUtil.getAttributeValue(mn,"static","false")) )
					methodType = MethodType.Static;
				else
					methodType = MethodType.Instance;

				if( methodName == null )
				{
					string templ = Core.resources["xml.attribute_not_found"].stringValue;
					throw new Exception(string.Format(templ,mn,"name",contrib.OwnerDocument.BaseURI));
				}
			}
			else
				methodType = MethodType.Constructor;

			entity = CreateEntity(contrib);
			PluginUtil.RegisterCommand( id, entity, cmd );
		}

		protected virtual ICommandEntity CreateEntity( XmlElement contrib )
		{
			if( commandType.Equals("ModalForm"))
				return new ModalFormCommand(targetType);
			if( commandType.Equals("ModelessForm"))
				return new ModelessFormCommand(targetType);
			ICommandEntity entity = CreateTarget() as ICommandEntity;
			if(entity==null)
			{
				string templ = Core.resources["plugin.invalid_command_entity"].stringValue;
				string[] args = new string[]{ commandType, targetType.Name, methodName };
				throw new PluginXmlException(contrib,string.Format(templ,args));
			}
			return entity;
		}

		public object CreateTarget()
		{
			try 
			{				
				if( methodType == MethodType.Static )
				{
					object obj = targetType.InvokeMember(methodName,
						BindingFlags.Public|BindingFlags.InvokeMethod|BindingFlags.Static,
						null,null,new object[]{id});
					return obj;
				}
				else
				{
					object obj = Activator.CreateInstance(targetType);
					if( methodType == MethodType.Instance )
						return targetType.InvokeMember(methodName,
							BindingFlags.Public|BindingFlags.InvokeMethod|BindingFlags.Instance,
							null,obj,new object[]{id});
					else 
						return obj; // MethodType.Constructor
				}
			} 
			catch( Exception e ) 
			{
				string templ = Core.resources["xml.class_load_error"].stringValue;
				throw new Exception(string.Format(templ,targetType.FullName,path),e);
			}			
		}
	}
}
