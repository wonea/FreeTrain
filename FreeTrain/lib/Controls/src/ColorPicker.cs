using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace freetrain.controls
{	

	public interface IColorLibrary : IEnumerable
	{
		int size { get; }
		Color this[int index]{ get; }
	}
	
	/// <summary>
	/// ColorPicker の概要の説明です。
	/// </summary>
	public class ColorPicker : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button tmp_btn;
		private System.Windows.Forms.Panel mainpanel;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		//protected IColorLibrary[] colors;
		protected ArrayList[] palettes;
		protected ArrayList separators;

		public ColorPicker(int palettesInRow) : this(new IColorLibrary[]{new DefaultLibrary()},palettesInRow)
		{
		}

		public ColorPicker() : this(new IColorLibrary[]{new DefaultLibrary()},4)
		{
		}

		public ColorPicker(IColorLibrary list, int palettesInRow) : this(new IColorLibrary[]{ list },4)
		{
		}

		public ColorPicker(IColorLibrary list) : this(new IColorLibrary[]{ list })
		{
		}

		public ColorPicker(IColorLibrary[] listarray) : this(listarray, 8)
		{
		}

		public ColorPicker(IColorLibrary[] listarray, int palettesInRow)
		{
			this.palettesInRow = palettesInRow;
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();
			Size = new Size(palettesInRow*paletteSize,paletteSize);
			if(listarray!=null)
			{
				mainpanel.Controls.Remove(tmp_btn);
				palettes = new ArrayList[listarray.Length];
				separators = new ArrayList(listarray.Length-1);
				for(int i = 0; i<listarray.Length; i++)
				{					
					IColorLibrary cl = listarray[i];
					palettes[i] = new ArrayList(cl.size);
					for(int j=0; j<cl.size; j++)
					{
						Color c = cl[j];
						Button b = createButton(c);
						mainpanel.Controls.Add(b);
						b.Click+=new EventHandler(button_Click);
						palettes[i].Add(b);
					}
				}
				int n = separators.Count;
				for(int i=n; i<listarray.Length-1; i++)
				{
					Label s = createSeparator(i);
					mainpanel.Controls.Add(s);
					separators.Add(s);
				}
			}
		}

		public void setColors( IColorLibrary[] libColors )
		{
			if(libColors != null )
			{
				for(int i = 0; i<palettes.Length; i++)
				{					
					IColorLibrary cl = libColors[i];
					if(cl.size>palettes[i].Count)
					{
						for(int j=0; j<palettes[i].Count; j++)
						{
							Button b = (Button)palettes[i][j];
							b.ForeColor = cl[j];
							b.Visible = true;
						}
						for(int j=palettes[i].Count; j<cl.size; j++)
						{
							Color c = cl[j];
							Button b = createButton(c);
							mainpanel.Controls.Add(b);
							b.Click+=new EventHandler(button_Click);
							palettes[i].Add(b);
						}
					}
					else
					{
						for(int j=0; j<cl.size; j++)
						{
							Button b = (Button)palettes[i][j];
							b.ForeColor = cl[j];
							b.Visible = true;
						}
						for(int j=cl.size; j<palettes.Length; j++)
						{
							Button b = (Button)palettes[i][j];
							b.Visible = false;
						}
					}
				}
				// extend array if necessaly
				if( palettes.Length < libColors.Length )
				{
					ArrayList[] newarr = new ArrayList[libColors.Length];
					for(int i=0; i<palettes.Length; i++ )
						newarr[i] = palettes[i];
					for(int i=palettes.Length; i<libColors.Length; i++ )
					{
						IColorLibrary cl = libColors[i];
						newarr[i] = new ArrayList(libColors[i].size);
						for(int j=0; j<cl.size; j++)
						{
							Color c = cl[j];
							Button b = createButton(c);
							mainpanel.Controls.Add(b);
							b.Click+=new EventHandler(button_Click);
							palettes[i].Add(b);
						}
					}
					palettes = newarr;
				}
				int n = separators.Count;
				for(int i=n; i<libColors.Length-1; i++)
				{
					Label s = createSeparator(i);
					mainpanel.Controls.Add(s);
					separators.Add(s);
				}
			}
			layoutPalettes();
		}

		protected int paletteSize = 8;
		public int PaletteSize 
		{
			get{ return paletteSize; }
			set{ 
				paletteSize = value;
				if(!IsDisposed)
					layoutPalettes();
			}
		}
		protected int palettesInRow = 8;
		public int PalettesInRow
		{
			get{ return palettesInRow; }
			set
			{ 
				palettesInRow = value; 
				if(!IsDisposed)
					layoutPalettes();
			}
		}

		protected Color selected;
		public Color SelectedColor
		{
			get{ return selected; }
			set{
				foreach(ArrayList al in palettes)
					foreach(Button b in al)
						if( b.BackColor.Equals(value) )
						{
							selected = value;
							b.Select();
							b.Focus();
							return;
						}
			}
		}

		public event EventHandler OnColorSelected;

		protected void layoutPalettes()
		{
			if( palettes == null ) return;
			int row = 0;
			int col = 0;
			int group = 0;
			int max_w = 0;
			foreach(ArrayList al in palettes)
			{
				foreach(Button b in al)
				{
					if(!b.Visible) break;
					b.Size = new Size(paletteSize,paletteSize);
					b.Location = new Point(col*paletteSize,row*paletteSize+group*2);
					col++;
					if(col==palettesInRow)
					{
						col = 0;
						row++;
					}
				}
				int c = Math.Min(al.Count,palettesInRow);
				if(max_w<c) max_w = c;
				if(col!=0) row++;
				col = 0;
				if(separators.Count>group)
				{
					Label sepa = (Label)separators[group];
					sepa.Size = new Size(paletteSize*palettesInRow,2);
					sepa.Location = new Point(0,row*paletteSize+group*2);
				}
				group++;
			}
			int w = max_w*paletteSize;
			int h = ((col==0?0:1)+row)*paletteSize+group*2-2;
			this.Size = new Size(w,h);
		}

		protected Button createButton(Color c)
		{
			Button b =new Button();
			b.BackColor = c;
			b.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			b.ForeColor = System.Drawing.Color.Transparent;
			b.Name = c.Name;
			b.Location = new Point(0,0);
			b.Size = new System.Drawing.Size(paletteSize, paletteSize);
			return b;
		}

		protected Label createSeparator(int idx)
		{
			Label s = new Label();
			s.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			s.Name = "separator"+idx;
			s.Location = new Point(0,0);
			s.Text="";
			s.Size = new System.Drawing.Size(palettesInRow*paletteSize, 2);
			return s;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			layoutPalettes();
		}


		#region コンポーネント デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.tmp_btn = new System.Windows.Forms.Button();
			this.mainpanel = new System.Windows.Forms.Panel();
			this.mainpanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// tmp_btn
			// 
			this.tmp_btn.BackColor = System.Drawing.Color.Gold;
			this.tmp_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.tmp_btn.ForeColor = System.Drawing.Color.Transparent;
			this.tmp_btn.Location = new System.Drawing.Point(0, 0);
			this.tmp_btn.Name = "tmp_btn";
			this.tmp_btn.Size = new System.Drawing.Size(8, 8);
			this.tmp_btn.TabIndex = 8;
			// 
			// mainpanel
			// 
			this.mainpanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mainpanel.Controls.Add(this.tmp_btn);
			this.mainpanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainpanel.Location = new System.Drawing.Point(0, 0);
			this.mainpanel.Name = "mainpanel";
			this.mainpanel.Size = new System.Drawing.Size(32, 24);
			this.mainpanel.TabIndex = 9;
			// 
			// ColorPicker
			// 
			this.Controls.Add(this.mainpanel);
			this.Name = "ColorPicker";
			this.Size = new System.Drawing.Size(32, 24);
			this.mainpanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button_Click(object sender, EventArgs e)
		{
			selected = ((Button)sender).BackColor;
			if(OnColorSelected!=null)
				OnColorSelected(sender,e);
		}
	}

	class DefaultLibrary : IColorLibrary
	{
		protected ArrayList list = new ArrayList(7);

		public DefaultLibrary()
		{
			list.Add(Color.Red);
			list.Add(Color.Orange);
			list.Add(Color.Yellow);
			list.Add(Color.YellowGreen);
			list.Add(Color.Green);
			list.Add(Color.SkyBlue);
			list.Add(Color.Blue);
			list.Add(Color.BlueViolet);
			list.Add(Color.Magenta);
			list.Add(Color.White);
			list.Add(Color.Gray);
			list.Add(Color.Black);
		}
		
		public int size
		{
			get{ return list.Count; }
		}

		public Color this[int index]
		{
			get{ return (Color)list[index];}
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
