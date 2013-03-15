using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;

namespace freetrain.controls
{
	public class MruMenu 
	{
		protected MenuItem			recentFileMenuItem;
		public String			registryKeyName;
		protected int				numEntries = 0;
		protected int				maxEntries = 4;
		protected int				maxShortenPathLength = 48;

		//
		// events
		//
		public event ClickHandler	onClicked;

		#region MruMenuItem

		// The menu may display a shortened or otherwise invalid pathname
		// This class is used to store the actual filename, preferably as
		// a fully resolved name.

		internal sealed class MruMenuItem : MenuItem
		{
			internal string fileName;

			public MruMenuItem(String _filename, String entryname, EventHandler eventHandler)
				: base(entryname, eventHandler)
			{
				fileName = _filename;
			}
			
			// when menus are merged, Windows Forms need a default constructor.
			// I have no idea why it needs it, and how it works. but this seems
			// to keep everything happy ... at least for now.
			public MruMenuItem() {}
		}
		#endregion

		/// <summary>
		/// MruMenu handles a most recently used (MRU) file list.
		/// 
		/// This class shows the MRU list in a popup menu. To display
		/// the MRU list "inline" use MruMenuInline.
		/// 
		/// The class will load the last set of files from the registry
		/// on construction and store them when instructed by the main
		/// program.
		/// 
		/// Internally, this class uses zero-based numbering for the items.
		/// The displayed numbers, however, will start with one.
		/// </summary>

		#region Construction

		protected MruMenu()	{}

		public MruMenu(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler)
			: this(_recentFileMenuItem, _clickedHandler, null, false, 4) {}

		public MruMenu(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, int _maxEntries)
			: this(_recentFileMenuItem, _clickedHandler, null, false, _maxEntries) {}

		public MruMenu(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, String _registryKeyName)
			: this(_recentFileMenuItem, _clickedHandler, _registryKeyName, true, 4) {}

		public MruMenu(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, String _registryKeyName, int _maxEntries)
			: this(_recentFileMenuItem, _clickedHandler, _registryKeyName, true, _maxEntries) {}
		
		public MruMenu(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, String _registryKeyName, bool loadFromRegistry)
			: this(_recentFileMenuItem, _clickedHandler, _registryKeyName, loadFromRegistry, 4) {}

		public MruMenu(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, String _registryKeyName, bool loadFromRegistry, int _maxEntries)
		{
			if (_recentFileMenuItem == null)
				throw new ArgumentNullException("recentFileMenuItem");

			if (_recentFileMenuItem.Parent == null)
				throw new ArgumentException("recentFileMenuItem is not part of a menu");
			
			recentFileMenuItem = _recentFileMenuItem;
			recentFileMenuItem.Checked = false;
			recentFileMenuItem.Enabled = false;
			recentFileMenuItem.DefaultItem = false;

			maxEntries = _maxEntries;
			onClicked += _clickedHandler;

			if (_registryKeyName != null)
			{
				registryKeyName = _registryKeyName;
				if (loadFromRegistry)
				{
					LoadFromRegistry();
				}
			}
		}
		#endregion

		#region Event Handling

		public delegate void ClickHandler(int number, String filename);

		protected void OnClick(object sender, System.EventArgs e)
		{
			MruMenuItem menuItem = (MruMenuItem) sender;
			if (onClicked != null) {
				try {
					onClicked(menuItem.Index - StartIndex, menuItem.fileName);
				}
				catch {
					//TODO: should we display an error message here?
				}
			}
		}
		#endregion

		#region Properties

		public virtual Menu.MenuItemCollection MenuItems
		{
			get
			{
				return recentFileMenuItem.MenuItems;
			}
		}

		public virtual int StartIndex
		{
			get
			{
				return 0;
			}
		}

		public virtual int EndIndex
		{
			get
			{
				return numEntries;
			}
		}

		public int NumEntries
		{
			get 
			{
				return numEntries; 
			}
		}

		public int MaxEntries
		{
			get 
			{
				return maxEntries; 
			}
			set 
			{
				if (value > 16)
				{
					maxEntries = 16;
				}
				else
				{
					maxEntries = value < 4 ? 4 : value;

					int index = StartIndex + maxEntries;
					while (numEntries > maxEntries)
					{
						MenuItems.RemoveAt(index);
						numEntries--;
					}
				}
			}
		}

		public int MaxShortenPathLength
		{
			get
			{
				return maxShortenPathLength;
			}
			set
			{
				maxShortenPathLength = value < 16 ? 16 : value;
			}
		}

		#endregion

		#region Helper Methods

		protected virtual void Enable()
		{
			recentFileMenuItem.Enabled = true;
		}

		protected virtual void Disable()
		{
			recentFileMenuItem.Enabled = false;
			recentFileMenuItem.MenuItems.RemoveAt(0);
		}

		protected virtual void SetFirstFile(MenuItem menuItem)
		{
		}

		public void SetFirstFile(int number)
		{
			if (number > 0 && numEntries > 1 && number < numEntries)
			{
				MenuItem menuItem = MenuItems[StartIndex + number];
				menuItem.Index = StartIndex;
				SetFirstFile(menuItem);
				FixupPrefixes(0);
			}
		}

		public static String FixupEntryname(int number, String entryname)
		{
			if (number < 9)
				return "&" + (number + 1) + "  " + entryname;
			else if (number == 9)
				return "1&0" + "  " + entryname;
			else
				return (number + 1) + "  " + entryname;
		}

		protected void FixupPrefixes(int startNumber)
		{
			if (startNumber < 0)
				startNumber = 0;

			if (startNumber < maxEntries)
			{
				for (int i = StartIndex + startNumber; i < EndIndex; i++, startNumber++)
				{
					MenuItems[i].Text = FixupEntryname(startNumber, MenuItems[i].Text.Substring(startNumber == 9 ? 5 : 4));
				}
			}
		}
		#endregion

		#region Get Methods

		public int FindFilenameNumber(String filename)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");

			if (filename.Length == 0)
				throw new ArgumentException("filename");

			if (numEntries > 0)
			{
				int number = 0;
				for (int i = StartIndex; i < EndIndex; i++, number++)
				{
					if (String.Compare(((MruMenuItem)MenuItems[i]).fileName, filename, true) == 0)
					{
						return number;
					}
				}
			}
			return -1;
		}

		public int FindFilenameMenuIndex(String filename)
		{
			int number = FindFilenameNumber(filename);
			return number < 0 ? -1 : StartIndex + number;
		}

		public int GetMenuIndex(int number)
		{
			if (number < 0 || number >= numEntries)
				throw new ArgumentOutOfRangeException("number");

			return StartIndex + number;
		}

		public String GetFileAt(int number)
		{
			if (number < 0 || number >= numEntries)
				throw new ArgumentOutOfRangeException("number");

			return ((MruMenuItem)MenuItems[StartIndex + number]).fileName;
		}

		public String[] GetFiles()
		{
			String[] filenames = new String[numEntries];

			int index = StartIndex;
			for (int i = 0; i < filenames.GetLength(0); i++, index++)
			{
				filenames[i] = ((MruMenuItem)MenuItems[index]).fileName;
			}

			return filenames;
		}

		// This is used for testing
		public String[] GetFilesFullEntryString()
		{
			String[] filenames = new String[numEntries];

			int index = StartIndex;
			for (int i = 0; i < filenames.GetLength(0); i++, index++)
			{
				filenames[i] = MenuItems[index].Text;
			}

			return filenames;
		}
		#endregion

		#region Add Methods

		public void SetFiles(String[] filenames)
		{
			RemoveAll();
			for (int i = filenames.GetLength(0) - 1; i >= 0; i--)
			{
				AddFile(filenames[i]);
			}
		}

		public void AddFiles(String[] filenames)
		{
			for (int i = filenames.GetLength(0) - 1; i >= 0; i--)
			{
				AddFile(filenames[i]);
			}
		}

		// Shortens a pathname by either removing consecutive components of a path
		// and/or by removing characters from the end of the filename and replacing
		// then with three elipses (...)
		//
		// In all cases, the root of the passed path will be preserved in it's entirety.
		//
		// If a UNC path is used or the pathname and maxLength are particularly short,
		// the resulting path may be longer than maxLength.
		//
		// This method expects fully resolved pathnames to be passed to it.
		// (Use Path.GetFullPath() to obtain this.)

		static public String ShortenPathname(String pathname, int maxLength)
		{
			if (pathname.Length <= maxLength)
				return pathname;

			String root = Path.GetPathRoot(pathname);
			if (root.Length > 3)
				root += Path.DirectorySeparatorChar;

			String[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			int filenameIndex = elements.GetLength(0) - 1;

			if (elements.GetLength(0) == 1) // pathname is just a root and filename
			{
				if (elements[0].Length > 5) // long enough to shorten
				{
					// if path is a UNC path, root may be rather long
					if (root.Length + 6 >= maxLength)
					{
						return root + elements[0].Substring(0, 3) + "...";
					}
					else
					{
						return pathname.Substring(0, maxLength - 3) + "...";
					}
				}
			}
			else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
			{
				root += "...\\";

				int len = elements[filenameIndex].Length;
				if (len < 6)
					return root + elements[filenameIndex];

				if ((root.Length + 6) >= maxLength)
				{
					len = 3;
				}
				else
				{
					len = maxLength - root.Length - 3;
				}
				return root + elements[filenameIndex].Substring(0, len) + "...";
			}
			else if (elements.GetLength(0) == 2)
			{
				return root + "...\\" + elements[1];
			}
			else
			{
				int len = 0;
				int begin = 0;

				for (int i = 0; i < filenameIndex; i++)
				{
					if (elements[i].Length > len)
					{
						begin = i;
						len = elements[i].Length;
					}
				}

				int totalLength = pathname.Length - len + 3;
				int end = begin + 1;

				while (totalLength > maxLength)
				{
					if (begin > 0)
						totalLength -= elements[--begin].Length - 1;

					if (totalLength <= maxLength)
						break;

					if (end < filenameIndex)
						totalLength -= elements[++end].Length - 1;

					if (begin == 0 && end == filenameIndex)
						break;
				}

				// assemble final string

				for (int i = 0; i < begin; i++)
				{
					root += elements[i] + '\\';
				}

				root += "...\\";

				for (int i = end; i < filenameIndex; i++)
				{
					root += elements[i] + '\\';
				}

				return root + elements[filenameIndex];
			}
			return pathname;
		}

		public void AddFile(String filename)
		{
			String pathname = Path.GetFullPath(filename);
			AddFile(pathname, ShortenPathname(pathname, MaxShortenPathLength));
		}

		public void AddFile(FileInfo file)
		{
			AddFile(file.FullName);
		}

		public void AddFile(String filename, String entryname)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");

			if (filename.Length == 0)
				throw new ArgumentException("filename");

			if (numEntries > 0)
			{
				int index = FindFilenameMenuIndex(filename);
				if (index >= 0)
				{
					SetFirstFile(index - StartIndex);
					return;
				}
			}

			if (numEntries < maxEntries)
			{
				MruMenuItem menuItem = new MruMenuItem(filename, FixupEntryname(0, entryname), new System.EventHandler(OnClick));
				MenuItems.Add(StartIndex, menuItem);
				SetFirstFile(menuItem);

				if (numEntries++ == 0)
				{
					Enable();
				}
				else
				{
					FixupPrefixes(1);
				}
			}
			else if (numEntries > 1)
			{
				MruMenuItem menuItem = (MruMenuItem) MenuItems[StartIndex + numEntries - 1];
				menuItem.Text = FixupEntryname(0, entryname);
				menuItem.fileName = filename;
				menuItem.Index = StartIndex;
				SetFirstFile(menuItem);
				FixupPrefixes(1);
			}
		}

		#endregion

		#region Remove Methods

		public void RemoveFile(int number)
		{
			if (number >= 0 && number < numEntries)
			{
				if (--numEntries == 0)
				{
					Disable();
				}
				else
				{
					int startIndex = StartIndex;
					MenuItems.RemoveAt(startIndex + number);
					if (number == 0)
					{
						SetFirstFile(MenuItems[startIndex]);
					}
					if (number < numEntries)
					{
						FixupPrefixes(number);
					}
				}
			}
		}

		public void RemoveFile(String filename)
		{
			if (numEntries > 0)
			{
				RemoveFile(FindFilenameNumber(filename));
			}
		}

		public void RemoveAll()
		{
			if (numEntries > 0)
			{
				for (int index = EndIndex - 1; index > StartIndex; index--)
				{
					MenuItems.RemoveAt(index);
				}
				Disable();
				numEntries = 0;
			}
		}

		#endregion

		#region Registry Methods

		public void LoadFromRegistry()
		{
			Debug.Assert( registryKeyName!=null );

			RemoveAll();

			RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryKeyName);
			if (regKey != null)
			{
				maxEntries = (int)regKey.GetValue("max", maxEntries);

				for (int number = maxEntries; number > 0; number--)
				{
					String filename = (String)regKey.GetValue("File" + number.ToString());
					if (filename != null)
						AddFile(filename);
				}

				regKey.Close();
			}
		}

		public void SaveToRegistry()
		{
			Debug.Assert( registryKeyName!=null );

			RegistryKey regKey = Registry.CurrentUser.CreateSubKey(registryKeyName);
			if (regKey != null)
			{
				regKey.SetValue("max", maxEntries);

				int number = 1;
				int i = StartIndex;
				for (; i < EndIndex; i++, number++)
				{
					regKey.SetValue("File" + number.ToString(), ((MruMenuItem)MenuItems[i]).fileName);
				}

				for (; number <= 16; number++)
				{
					regKey.DeleteValue("File" + number.ToString(), false);
				}

				regKey.Close();
			}
		}

		#endregion

		#region XML save/load
		public void LoadFromFile( FileInfo file ) {
			RemoveAll();

			try {
				XmlDocument dom = new XmlDocument();
				dom.Load(file.FullName);
				
				maxEntries = int.Parse(dom.SelectSingleNode("/mru/@max").Value);
                foreach (XmlElement e in dom.SelectNodes("/mru/entry"))
                    AddFile(e.InnerText);
			} catch( Exception e ) {
				Debug.WriteLine("failed to load "+file);
				Debug.WriteLine(e);
			}
		}

		public void SaveToFile( FileInfo file ) {
			XmlTextWriter w = new XmlTextWriter(file.FullName,Encoding.Default);
			w.Formatting = Formatting.Indented;
			w.WriteStartDocument(true);
			w.WriteComment("stores most recently used files.");
			w.WriteStartElement("mru");
			w.WriteAttributeString("max",maxEntries.ToString());

			// write in the reverse order to make loading easy
			for( int i=EndIndex-1; i>=StartIndex; i-- )
				w.WriteElementString("entry", ((MruMenuItem)MenuItems[i]).fileName);

			w.WriteEndElement();
			w.WriteEndDocument();
			w.Close();
		}
		#endregion
	}


	public class MruMenuInline : MruMenu 
	{
		protected MenuItem firstMenuItem;

		/// <summary>
		/// MruMenuInline shows the MRU list inline (without a popup)
		/// </summary>

		#region Construction

		public MruMenuInline(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler)
			: base(_recentFileMenuItem, _clickedHandler, null, false, 4)
		{
			maxShortenPathLength = 128;
			firstMenuItem = _recentFileMenuItem;
		}

		public MruMenuInline(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, int _maxEntries)
			: base(_recentFileMenuItem, _clickedHandler, null, false, _maxEntries)
		{
			maxShortenPathLength = 128;
			firstMenuItem = _recentFileMenuItem;
		}

		public MruMenuInline(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, String _registryKeyName)
			: base(_recentFileMenuItem, _clickedHandler, _registryKeyName, true, 4)
		{
			maxShortenPathLength = 128;
			firstMenuItem = _recentFileMenuItem;
		}

		public MruMenuInline(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, String _registryKeyName, int _maxEntries)
			: base(_recentFileMenuItem, _clickedHandler, _registryKeyName, true, _maxEntries)
		{
			maxShortenPathLength = 128;
			firstMenuItem = _recentFileMenuItem;
		}
		
		public MruMenuInline(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, String _registryKeyName, bool loadFromRegistry)
			: base(_recentFileMenuItem, _clickedHandler, _registryKeyName,loadFromRegistry, 4)
		{
			maxShortenPathLength = 128;
			firstMenuItem = _recentFileMenuItem;
		}

		public MruMenuInline(MenuItem _recentFileMenuItem, ClickHandler _clickedHandler, String _registryKeyName, bool loadFromRegistry, int _maxEntries)
			: base(_recentFileMenuItem, _clickedHandler, _registryKeyName,loadFromRegistry, _maxEntries)
		{
			maxShortenPathLength = 128;
			firstMenuItem = _recentFileMenuItem;
		}
		#endregion

		#region Overridden Properties

		public override Menu.MenuItemCollection MenuItems
		{
			get
			{
				return firstMenuItem.Parent.MenuItems;
			}
		}

		public override int StartIndex
		{
			get
			{
				return firstMenuItem.Index;
			}
		}

		public override int EndIndex
		{
			get
			{
				return StartIndex + numEntries;
			}
		}
		#endregion

		#region Overridden Methods

		protected override void Enable()
		{
			MenuItems.Remove(recentFileMenuItem);
		}

		protected override void SetFirstFile(MenuItem menuItem)
		{
			firstMenuItem = menuItem;
		}

		protected override void Disable()
		{
			MenuItems.Add(firstMenuItem.Index, recentFileMenuItem);
			MenuItems.Remove(firstMenuItem);
			firstMenuItem = recentFileMenuItem;
		}
		#endregion
	}
}