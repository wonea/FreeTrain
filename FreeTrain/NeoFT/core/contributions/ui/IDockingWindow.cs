using System;
using nft.ui.docking;

namespace nft.contributions.ui	
{
	/// <summary>
	/// <c>Control</c> object specified in the <c>DockingContribution</c>
	/// can implement this interface to receive the reference to the docking site.
	/// 
	/// Any reference to the site should be released when the control is disposed,
	/// since the same site could be reused for another control.
	/// </summary>
	public interface IDockingWindow
	{
		void setSite( ContentEx site );
	}
}
