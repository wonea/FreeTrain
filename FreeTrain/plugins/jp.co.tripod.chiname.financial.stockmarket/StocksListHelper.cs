using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using freetrain.util;
using freetrain.world;

namespace freetrain.finance.stock
{
	/// <summary>
	/// Setup ListView
	/// </summary>
	public class StocksListHelper
	{
		static private Set marketList = new Set();
		static private Set retainList = new Set();

		static private StockCompanyModule stockCom { get { return StockCompanyModule.theInstance; } }
		static private int unit { get { return StockCompanyModule.numUNIT; } }

		private StocksListHelper()
		{
		}

		static public void buildStockMarketList( ListView _list )
		{
			_list.Sorting = SortOrder.Ascending;
			// Create columns
			ColumnHeader column0 = new ColumnHeader();
			ColumnHeader column1 = new ColumnHeader();
			ColumnHeader column2 = new ColumnHeader();
			ColumnHeader column3 = new ColumnHeader();
			ColumnHeader column3x = new ColumnHeader();
			ColumnHeader column4 = new ColumnHeader();
			ColumnHeader column5 = new ColumnHeader();
			ColumnHeader column6 = new ColumnHeader();
			ColumnHeader column7 = new ColumnHeader();
			ColumnHeader column8 = new ColumnHeader();
			int width = _list.ClientSize.Width;
			_list.Clear();
			_list.Columns.AddRange( new ColumnHeader[]
					{column0,column1,column2,column3,column3x,column4,column5,column6,column7,column8 } );
			column0.Text = "ID";
			column1.Text = "Stock";
			//! column1.Text = "銘柄";
			column2.Text = "Category";
			//! column2.Text = "業種";
			column3.Text = "Current share price";
			//! column3.Text = "現在株価";
			column3.TextAlign=HorizontalAlignment.Right;
			column3x.Text = "Change since yesterday";
			//! column3x.Text = "前日比";
			column3x.TextAlign=HorizontalAlignment.Right;
			column4.Text = "Shares on the market";
			//! column4.Text = "市場株数";
			column4.TextAlign=HorizontalAlignment.Right;
			column5.Text = "Issued shares";
			//! column5.Text = "発行株数";
			column5.TextAlign=HorizontalAlignment.Right;
			column6.Text = "Dividend yield";
			//! column6.Text = "配当利回";
			column6.TextAlign=HorizontalAlignment.Right;
			column7.Text = "1 share dividend";
			//! column7.Text = "１株配当";
			column7.TextAlign=HorizontalAlignment.Right;
			column8.Text = "Held shares";
			//! column8.Text = "保有株数";
			column8.TextAlign=HorizontalAlignment.Right;
			column0.Width = 0;
			column1.Width = 100;
			column2.Width = 50;
			column3.Width = 60;
			column3x.Width = 50;
			column4.Width = 50;
			column5.Width = 3;
			column6.Width = 50;
			column7.Width = 3;
			column8.Width = 50;
			#region debug mode
			if( StockMarketConfig.debug )
			{
				ColumnHeader column10 = new ColumnHeader();
				ColumnHeader column11 = new ColumnHeader();
				ColumnHeader column12 = new ColumnHeader();
				ColumnHeader column13 = new ColumnHeader();
				ColumnHeader column14 = new ColumnHeader();
				_list.Columns.AddRange( new ColumnHeader[]
					{column10,column11,column12,column13,column14 } );

				column10.Text = "Score";
				//! column10.Text = "スコア";
				column11.Text = "Sales";
				//! column11.Text = "売上";
				column12.Text = "Profit";
				//! column12.Text = "利益";
				column13.Text = "Future score";
				//! column13.Text = "将来スコア";
				column14.Text = "Fair share price";
				//! column14.Text = "適正株価";
				column10.TextAlign=HorizontalAlignment.Right;
				column11.TextAlign=HorizontalAlignment.Right;
				column12.TextAlign=HorizontalAlignment.Right;
				column13.TextAlign=HorizontalAlignment.Right;
				column14.TextAlign=HorizontalAlignment.Right;
				column11.Width = 100;
				column12.Width = 100;
				column13.Width = 100;
			}
			#endregion
			buildMarketListItems(_list);
			setSorter(_list);
			marketList.add(_list);
			Debug.WriteLine("$$$$$$$$$$");
		}
		
		static public void buildRetainingList( ListView _list )
		{
			_list.Sorting = SortOrder.Ascending;
			// Create columns
			ColumnHeader column0 = new ColumnHeader();
			ColumnHeader column1 = new ColumnHeader();
			ColumnHeader column2 = new ColumnHeader();
			ColumnHeader column3 = new ColumnHeader();
			ColumnHeader column3x = new ColumnHeader();
			ColumnHeader column4 = new ColumnHeader();
			ColumnHeader column5 = new ColumnHeader();
			ColumnHeader column6 = new ColumnHeader();
			ColumnHeader column7 = new ColumnHeader();
			ColumnHeader column8 = new ColumnHeader();
			int width = _list.ClientSize.Width;
			_list.Clear();
			_list.Columns.AddRange( new ColumnHeader[]
					{column0,column1,column2,column3,column3x,column4,column5,column6,column7,column8 } );
			column0.Text = "ID";
			column1.Text = "Stock";
			//! column1.Text = "銘柄";
			column2.Text = "Category";
			//! column2.Text = "業種";
			column3.Text = "Current share price";
			//! column3.Text = "現在株価";
			column3.TextAlign=HorizontalAlignment.Right;
			column3x.Text = "Change since yesterday";
			//! column3x.Text = "前日比";
			column3x.TextAlign=HorizontalAlignment.Right;
			column4.Text = "Purchase price (average)";
			//! column4.Text = "購入価格(平均)";
			column4.TextAlign=HorizontalAlignment.Right;
			column5.Text = "Dividend yield";
			//! column5.Text = "配当利回";
			column5.TextAlign=HorizontalAlignment.Right;
			column6.Text = "1 share dividend";
			//! column6.Text = "１株配当";
			column6.TextAlign=HorizontalAlignment.Right;
			column7.Text = "Held shares";
			//! column7.Text = "保有株数";
			column7.TextAlign=HorizontalAlignment.Right;
			column8.Text = "Appraised value of assets";
			//! column8.Text = "資産評価額";
			column8.TextAlign=HorizontalAlignment.Right;
			column0.Width = 0;
			column1.Width = 100;
			column2.Width = 3;
			column3.Width = 60;
			column3x.Width = 60;
			column4.Width = 60;
			column5.Width = 3;
			column6.Width = 60;
			column7.Width = 60;
			column8.Width = 75;
			buildRetainListItems(_list);
			setSorter(_list);
			retainList.add(_list);
		}

		#region non public methods about building list.
		static protected void buildMarketListItems(ListView _list)
		{
			IEnumerator e = stockCom.getMarketStocks();
			while(e.MoveNext())	{
				ListViewItem item = new ListViewItem();
				rebuildMarketStockItem(item,(Company)e.Current);
				_list.Items.Add(item);
			}
		}

		static protected void buildRetainListItems(ListView _list)
		{
			IEnumerator e = stockCom.getRetainStocks();
			while(e.MoveNext())	{
				ListViewItem item = new ListViewItem();
				rebuildRetainStockItem(item,(Company)e.Current);
				_list.Items.Add(item);
			}
		}

		static protected void updateMarketListItems(ListView _list)
		{
			IEnumerator e = _list.Items.GetEnumerator();
			while(e.MoveNext())	
			{
				ListViewItem item = (ListViewItem)e.Current;
				rebuildMarketStockItem(item,(Company)item.Tag);
			}
		}

		static protected void updateRetainListItems(ListView _list)
		{
			IEnumerator e = _list.Items.GetEnumerator();
			while(e.MoveNext())	
			{
				ListViewItem item = (ListViewItem)e.Current;
				rebuildRetainStockItem(item,(Company)item.Tag);
			}
		}

		static public string formatPriceChange(int change) 
		{
			if(change == 0)
				return "0";
			if(change > 0 )
				return "+"+change.ToString();
			else
				return change.ToString();
		}

		static public string formatRate(double rate) 
		{
			return rate.ToString("P2");
		}

		static public string formatNum(int num) 
		{
			return num.ToString("N0");
		}

		static protected void rebuildMarketStockItem(ListViewItem item, Company com)
		{
			item.Tag = com;
			item.SubItems.Clear();
			item.Text = com.id;
			#region debug mode
			if( StockMarketConfig.debug )
			{
				item.SubItems.AddRange(new string[]{ 
													   com.name, 
													   com.type.name,
													   CurrencyUtil.format(com.stockData.currentPrice),
													   formatPriceChange(com.stockData.priceChange),
													   formatNum(com.stockData.numInMarket),
													   formatNum(com.stockData.numTotal),
													   formatRate(com.stockData.dividendRatio),
													   (com.stockData.dividend).ToString(),
													   formatNum(com.stockData.numOwn),
													   com._scoreNormal.ToString("P1"),
													   CurrencyUtil.format(com._salesReal),
													   CurrencyUtil.format(com._benefit),
													   com._scoreFuture.ToString("P1"),
													   CurrencyUtil.format(com.stockData._properPriceU/unit)});
			}
			#endregion
			else 
			{
				item.SubItems.AddRange(new string[]{ 
													   com.name, 
													   com.type.name,
													   CurrencyUtil.format(com.stockData.currentPrice),
													   formatPriceChange(com.stockData.priceChange),
													   formatNum(com.stockData.numInMarket),
													   formatNum(com.stockData.numTotal),
													   formatRate(com.stockData.dividendRatio),
													   (com.stockData.dividend).ToString("N2"),
													   formatNum(com.stockData.numOwn)});
			}
		}

		static protected void rebuildRetainStockItem(ListViewItem item, Company com)
		{
			item.Tag = com;
			item.SubItems.Clear();
			item.Text = com.id;
			item.SubItems.AddRange(new string[]{ 
												   com.name, 
												   com.type.name,
												   CurrencyUtil.format(com.stockData.currentPrice),
												   formatPriceChange(com.stockData.priceChange),
												   CurrencyUtil.format(com.stockData.averageBoughtPrice),
												   formatRate(com.stockData.dividendRatio),
												   (com.stockData.dividend).ToString("N2"),
												   formatNum(com.stockData.numOwn),
												   CurrencyUtil.format(com.stockData.assessedAmount)});
		}
		#endregion

		static public void RemoveList(ListView _list)
		{
			marketList.remove(_list);
			retainList.remove(_list);
		}

		static public ListViewItem findItem( ListView list, Company com )
		{
			IEnumerator e = list.Items.GetEnumerator();
			while(e.MoveNext())	
			{
				ListViewItem current = (ListViewItem)e.Current;
				if(current.Tag.Equals(com)) return current;
			}
			return null;
		}

		// Called when updated stockData value.
		static public void onUpdateMarket( Company com )
		{
			IEnumerator e;
			e = marketList.GetEnumerator();
			while(e.MoveNext())	{
				ListView list = (ListView)e.Current;
				updateMarketListItems(list);
			}
			e = retainList.GetEnumerator();
			while(e.MoveNext())	
			{
				ListView list = (ListView)e.Current;
				updateRetainListItems(list);
			}
		}

		// Called when ownership status changed.
		static public void onOwnershipStatusChanged(OwnershipStatus status, Company sender)
		{
			IEnumerator e;
			e = retainList.GetEnumerator();
			while(e.MoveNext())	{
				ListView list = (ListView)e.Current;
				ListViewItem item;
				if( status == OwnershipStatus.NEW_OWNERSHIP ) {
					item = new ListViewItem();
					rebuildRetainStockItem(item,sender);
					list.Items.Add(item);
				}
				else {
					item = findItem(list,sender);
					if( item!=null ) {
						switch(status) {
							case OwnershipStatus.LOST_OWNERSHIP:
								list.Items.Remove(item);							
								break;
							default:
								rebuildRetainStockItem(item,sender);
								break;
						}
					}
				}
			}
			e = marketList.GetEnumerator();
			while(e.MoveNext())	{
				ListView list = (ListView)e.Current;
				ListViewItem item = findItem(list,sender);
				if( item!=null ) 
					rebuildMarketStockItem(item,sender);
			}
		}

		// Restore data and delegate registrations (called on new world).
		static public void restoreData()
		{
			IEnumerator e;
			e = marketList.GetEnumerator();
			while(e.MoveNext())	
			{
				ListView list = (ListView)e.Current;
				list.Items.Clear();
				buildMarketListItems(list);
			}
			e = retainList.GetEnumerator();
			while(e.MoveNext())	
			{
				ListView list = (ListView)e.Current;
				list.Items.Clear();
				buildRetainListItems(list);
			}
		}

		static protected void setSorter( ListView _list )
		{
			_list.ColumnClick += new ColumnClickEventHandler(onColumnClick);
			_list.ListViewItemSorter = new Sorter(0,false);
		}

		static private void onColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			ListView _list = (ListView)sender;
			//ColumnHeader header = _list.Columns[e.Column];
			Sorter sorter = (Sorter)_list.ListViewItemSorter;
			sorter.index = e.Column;
			sorter.bNumeric = (e.Column>2);
			_list.Sort();
		}

		internal class Sorter : IComparer
		{
			public int index;
			public bool bNumeric;

			public Sorter(int idx, bool bNumeric) {
				index = idx;
				this.bNumeric = bNumeric;
			}

			public int Compare( object x, object y){
				string sx = ((ListViewItem)x).SubItems[index].Text;
				string sy = ((ListViewItem)y).SubItems[index].Text;
				if( bNumeric ) 
				{
					return (int)(double.Parse(sy.Replace("%",""))-double.Parse(sx.Replace("%","")));
				}
				else
					return string.Compare(sx,sy);
			}
		}


	}
}
