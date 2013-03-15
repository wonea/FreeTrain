using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using freetrain.util;
using freetrain.world;

namespace freetrain.views.bank
{
	/// <summary>
	/// Setup ListView
	/// </summary>
	public class BankbookListHelper
	{
		static private Set debtList = new Set();
		static private Set depositList = new Set();

		static private BankModule bank { get { return BankModule.theInstance; } }

		private BankbookListHelper()
		{
		}

		static public void buildDebtList( ListView _list )
		{
			// Create columns
			ColumnHeader column0 = new ColumnHeader();
			ColumnHeader column1 = new ColumnHeader();
			ColumnHeader column2 = new ColumnHeader();
			ColumnHeader column3 = new ColumnHeader();
			ColumnHeader column4 = new ColumnHeader();
			ColumnHeader column5 = new ColumnHeader();
			int width = _list.ClientSize.Width;
			Debug.Write(width);

			//! Translator's note: Not being familiar with banking/financial terms,
			//! I have probably gotten many of these strings wrong.

			_list.Clear();
			_list.Columns.AddRange( new ColumnHeader[]
					{column0,column1,column2,column3,column4,column5 } );
			column0.Text = "ID";
			column1.Text = "Repayment terms";
			//! column1.Text = "返済期日";
			column2.Text = "Loan";
			//! column2.Text = "借入額";
			column2.TextAlign=HorizontalAlignment.Right;
			column3.Text = "Interest rate";
			//! column3.Text = "利率";
			column3.TextAlign=HorizontalAlignment.Right;
			column4.Text = "Expiration interest";
			//! column4.Text = "満期利息";
			column4.TextAlign=HorizontalAlignment.Right;
			column5.Text = "Date of loan";
			//! column5.Text = "借入日";
			column0.Width = 0;
			if( width < 400 ) 
			{
				column1.Width = Math.Max(70,width*60/300);
				column2.Width = Math.Max(100,width*100/300);
				column3.Width = Math.Max(50,width*50/300);
				column4.Width = Math.Max(75,width*80/300);
				column5.Width = 3;
			}
			else
			{
				column1.Width = 70;
				column2.Width = 110;
				column3.Width = 50;
				column4.Width = 100;
				column5.Width = 80;
			}
			buildDebtListItems(_list);
			debtList.add(_list);
		}
		
		static public void buildDepositList( ListView _list )
		{
			BankModule bank = BankModule.theInstance;
			// Create columns
			ColumnHeader column0 = new ColumnHeader();
			ColumnHeader column1 = new ColumnHeader();
			ColumnHeader column2 = new ColumnHeader();
			ColumnHeader column3 = new ColumnHeader();
			ColumnHeader column4 = new ColumnHeader();
			ColumnHeader column5 = new ColumnHeader();
			int width = _list.ClientSize.Width;
			_list.Clear();
			_list.Columns.AddRange( new ColumnHeader[]
					{column0,column1,column2,column3,column4,column5 } );
			column0.Text = "ID";
			column1.Text = "Maturity date";
			//! column1.Text = "満期日";
			column2.Text = "Credit";
			//! column2.Text = "預金額";
			column2.TextAlign=HorizontalAlignment.Right;
			column3.Text = "Interest rate";
			//! column3.Text = "利率";
			column3.TextAlign=HorizontalAlignment.Right;
			column4.Text = "Date of credit";
			//! column4.Text = "預入日";
			column5.Text = "Remarks";
			//! column5.Text = "備考";
			column0.Width = 0;
			if( width < 400 ) 
			{
				column1.Width = Math.Max(70,width*60/300);
				column2.Width = Math.Max(100,width*100/300);
				column3.Width = Math.Max(50,width*50/300);
				column4.Width = 3;
				column5.Width = Math.Max(75,width*80/300);
			}
			else
			{
				column1.Width = 70;
				column2.Width = 110;
				column3.Width = 50;
				column4.Width = 70;
				column5.Width = 75;
			}
			buildDepositListItems(_list);
			depositList.add(_list);
		}

		#region non public methods about building list.
		static protected void buildDebtListItems(ListView _list)
		{
			IEnumerator e = bank.getDebts();
			while(e.MoveNext())	{
				ListViewItem item = new ListViewItem();
				rebuildDebtItem(item,(FloatingAsset)e.Current);
				_list.Items.Add(item);
			}
		}

		static protected void buildDepositListItems(ListView _list)
		{
			IEnumerator e = bank.getDeposits();
			while(e.MoveNext())	{
				ListViewItem item = new ListViewItem();
				rebuildDepositItem(item,(FloatingAsset)e.Current);
				_list.Items.Add(item);
			}
		}

		static public string formatDate(Time time) 
		{
			string r="";
			r = time.year.ToString() + '/' + time.month.ToString() + '/' + time.day.ToString(); 
			return r;
		}

		static public string formatRate(double rate) 
		{
			return rate.ToString("P2");
		}

		static protected void rebuildDebtItem(ListViewItem item, FloatingAsset asset)
		{
			IEnumerator e = debtList.GetEnumerator();
			e.MoveNext();
			item.Tag = asset;
			//item.Text = asset.GetHashCode().ToString();
			item.SubItems.Clear();
			item.SubItems.AddRange(new string[]{ 
												   formatDate(asset.due), 
												   asset.corpus.ToString(),
												   formatRate(asset.interestRate),
												   asset.interestDue.ToString(),
												   formatDate(asset.begining)});
		}

		static protected void rebuildDepositItem(ListViewItem item, FloatingAsset asset)
		{
			IEnumerator e = depositList.GetEnumerator();
			e.MoveNext();
			item.Tag = asset;
			//item.Text = asset.GetHashCode().ToString();
			item.SubItems.Clear();
			item.SubItems.AddRange(new string[]{ 
												   formatDate(asset.due), 
												   asset.corpus.ToString(),
												   formatRate(asset.interestRate),
												   formatDate(asset.begining),
												   ((FixedDeposit)asset).isContinue?"Continue interest":"Cancel maturity"});
//! ((FixedDeposit)asset).isContinue?"元利継続":"満期日解約"});
//! Translator's note: I have most probably gotten these two wrong!
		}
		#endregion

		static public void RemoveList(ListView _list)
		{
			debtList.remove(_list);
			depositList.remove(_list);
		}

		// Do actions required when debt added.
		internal static void addNewDebt(FloatingAsset sender)
		{
			IEnumerator e = debtList.GetEnumerator();
			if(debtList.count>0)
			{
				ListViewItem item = new ListViewItem();
				rebuildDebtItem(item,sender);
				while(e.MoveNext())	{
					((ListView)e.Current).Items.Add((ListViewItem)item.Clone());
				}
			}
			sender.onStatusChanging += new AssetChangeListener(onDebtStatusChanged);
		}
		
		// Do actions required when deposit added.
		internal static void addNewDeposit(FloatingAsset sender)
		{
			IEnumerator e = depositList.GetEnumerator();
			if(depositList.count>0)
			{
				ListViewItem item = new ListViewItem();
				rebuildDepositItem(item,sender);
				while(e.MoveNext())	{
					((ListView)e.Current).Items.Add((ListViewItem)item.Clone());
				}
			}
			sender.onStatusChanging += new AssetChangeListener(onDepositStatusChanged);
		}

		static public ListViewItem findItem( ListView list, FloatingAsset asset )
		{
			IEnumerator e = list.Items.GetEnumerator();
			while(e.MoveNext())	
			{
				ListViewItem current = (ListViewItem)e.Current;
				if(current.Tag.Equals(asset)) return current;
			}
			return null;
		}

		// Called when asset status changed.
		static public void onDebtStatusChanged(FloatingAsset sender, AssetStatus status, long param)
		{
			IEnumerator e = debtList.GetEnumerator();
			while(e.MoveNext())	{
				ListView list = (ListView)e.Current;
				ListViewItem item = findItem(list,sender);
				if( item!=null ) {
					switch(status) {
						case AssetStatus.CANCELING:
							list.Items.Remove(item);							
							break;
						default:
							rebuildDebtItem(item,sender);
							break;
					}
				}
			}
		}

		// Called when asset status changed.
		static public void onDepositStatusChanged(FloatingAsset sender, AssetStatus status, long param)
		{
			IEnumerator e = depositList.GetEnumerator();
			while(e.MoveNext())	
			{
				ListView list = (ListView)e.Current;
				ListViewItem item = findItem(list,sender);
				if( item!=null ) 
				{
					switch(status) 
					{
						case AssetStatus.CANCELING:
							list.Items.Remove(item);
							break;
						default:
							rebuildDepositItem(item,sender);
							break;
					}
				}
			}
		}

		// Restore data and delegate registrations (called on new world).
		static public void restoreData()
		{
			IEnumerator e;
//			e = bank.getDebts();
//			while(e.MoveNext())	
//			{
//				FloatingAsset asset = (FloatingAsset)e.Current;
//				asset.onStatusChanging += new AssetChangeListener(onDebtStatusChanged);
//			}			
//			e = bank.getDeposits();
//			while(e.MoveNext())	
//			{
//				FloatingAsset asset = (FloatingAsset)e.Current;
//				asset.onStatusChanging += new AssetChangeListener(onDepositStatusChanged);
//			}			
			e = debtList.GetEnumerator();
			while(e.MoveNext())	
			{
				ListView list = (ListView)e.Current;
				list.Items.Clear();
				buildDebtListItems(list);
			}
			e = depositList.GetEnumerator();
			while(e.MoveNext())	
			{
				ListView list = (ListView)e.Current;
				list.Items.Clear();
				buildDepositList(list);
			}
		}
	}
}
