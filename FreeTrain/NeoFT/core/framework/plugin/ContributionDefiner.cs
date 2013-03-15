using System;
using System.Xml;

namespace nft.framework.plugin
{
	/// <summary>
	/// ContributionDefiner defines new Contribution class.
	/// (for the needs to distinguish other stereotyped contributions.)
	/// </summary>
	public class ContributionDefiner : Contribution, IUserExtension
	{
		public ContributionDefiner(XmlElement e) : base(e)
		{
		}

		#region IUserExtension メンバ

		public override bool UserAvailable
		{
			get{ return true; }
			set{}
		}

		public override bool ComAvailable
		{
			get{ return true; }
			set{}
		}

		#endregion
	}
}
