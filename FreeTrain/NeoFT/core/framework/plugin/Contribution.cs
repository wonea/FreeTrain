using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using nft.util;

namespace nft.framework.plugin
{
	
	/// <summary>
	/// Common base class of contributions.
	/// 
	/// A contribution is a functionality provided by a plug-in.
	/// </summary>
	[Serializable]
	public abstract class Contribution : ISerializable, IUserExtension
	{
		static string GenerateID( XmlElement contrib )
		{
			string short_id = XmlUtil.getAttributeValue( contrib, "id", null);
			if( short_id == null )
			{
				string templ = Core.resources["xml.attribute_not_found"].stringValue;
				throw new PluginXmlException(contrib,string.Format(
					templ,contrib.Name,"name",contrib.OwnerDocument.BaseURI));
			}

			string pname = PluginUtil.GetPruginDirName(contrib);
			return pname + ":" + short_id;
		}
		
		protected Contribution( XmlElement contrib )
		{
			this.name = XmlUtil.getSingleNodeText(contrib,"name","<unknown>");

			id = GenerateID( contrib );
			try
			{
				type = contrib.Attributes["type"].Value;
			}
			catch
			{
				string templ = Core.resources["xml.attribute_not_found"].stringValue;
				throw new PluginXmlException(contrib,string.Format(
					templ,contrib.Name,"type",contrib.OwnerDocument.BaseURI));
			}
		}

		protected Contribution( string _type, string _id, string _name ) {
			this.type = _type;
			this.id = _id;
			this.name = _name;
		}

		/// <summary>
		/// This method is a backdoor to configure a contribution.
		/// 
		/// We could just pass this argument through a constructor,
		/// but Contribution will be inherited multiple times, so it would be
		/// little awkward to pass a lot of parameters around.
		/// </summary>
		/// <param name="_baseUri"></param>
		internal void init( Plugin _parent, Uri _baseUri ) {
			this._parent = _parent;
			this._baseUri = _baseUri;
		}

		#region IUserExtension メンバ
		/// <summary>
		/// Name of this contribution.
		/// </summary>
		public readonly string name;
		
		internal protected ModuleState _state = ModuleState.Uninitialized;
		public ModuleState state
		{
			get{ return _state;	}
		}
		
		private bool _userAvailable = true;
		public virtual bool UserAvailable
		{
			get{ return _userAvailable;	}
			set{ _userAvailable = value; }
		}
		private bool _comAvailable = true;
		public virtual bool ComAvailable
		{
			get{ return _comAvailable;	}
			set{ _comAvailable = value; }
		}
		#endregion

		/// <summary>
		/// Notifies the end of the initialization.
		/// 
		/// This method is called after all the contributions are loaded
		/// into memory. This is a good chance to run additional tasks
		/// that need to access other contributions.
		/// </summary>
		protected internal virtual void onInitComplete() 
		{
		}

		/// <summary>
		/// Type of this contribution.
		/// This is the value of the type attribute.
		/// </summary>
		public readonly string type;

		/// <summary>
		/// Unique ID of this contribution.
		/// 
		/// Either GUID or URI, but can be anything as long
		/// as it's unique.
		/// </summary>
		public readonly string id;

		internal protected bool _hideFromCom;
		internal protected bool _hideFromPlayer;
		public bool hideFromCom{ get{ return _hideFromCom;} }
		public bool hideFromPlayer{ get{ return _hideFromPlayer;} }

			/// <summary>
			/// Base URI for this contribution (which is the same
			/// as the base URI for the plug-in.)
			/// 
			/// This poinst to the plug-in directory.
			/// </summary>
			public Uri baseUri { get { return _baseUri; } }
		private  Uri _baseUri;
		
		/// <summary>
		/// Returns the Plugin object that contains this contribution.
		/// </summary>
		public Plugin parent { get { return _parent; } }
		private Plugin _parent;


		/// <summary>
		/// If a plug-in is implemented by using an assembly,
		/// it should override property and return the Assembly
		/// object, so that obejcts from this assembly can be
		/// de-serialized.
		/// 
		/// Returns null if this contribution doesn't rely on
		/// any assembly.
		/// </summary>
		public virtual Assembly assembly { get { return this.GetType().Assembly; } }


		#region utility methods
		/*
		protected Picture loadPicture( string name ) {
			return new Picture( this.id+":"+name, new Uri( baseUri,name).LocalPath );
		}

		/// <summary>
		/// Locate the Picture from which sprites should be loaded.
		/// </summary>
		/// <param name="sprite">&lt;sprite> element in the manifest.</param>
		/// <returns>non-null valid object.</returns>
		protected Picture getPicture( XmlElement sprite ) {
			XmlElement pic = (XmlElement)XmlUtil.selectSingleNode(sprite,"picture");
			
			XmlAttribute r = pic.Attributes["ref"];
			if(r!=null)
				// reference to externally defined pictures.
				return PictureManager.get(r.Value);

			// otherwise look for local picture definition
			return new Picture(pic,
				sprite.SelectSingleNode("ancestor-or-self::contribution/@id").InnerText);
		}
		*/
		#endregion

		// serialize this object by reference
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context) {
			info.SetType(typeof(ReferenceImpl));
			info.AddValue("id",id);
		}
		
		[Serializable]
		internal sealed class ReferenceImpl : IObjectReference {
			private string id=null;
			public object GetRealObject(StreamingContext context) {
				object o = Core.plugins.getContribution(id);
				if(o==null)
					throw new SerializationException(
						"Plugin that contains Contribution\""+id+"\" could not be found");
						//! "コントリビューション¥""+id+"¥"を含むプラグインが見つかりません");
				return o;
			}
		}
	}
}
