using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace freetrain.controls
{
	/// <summary>
	/// the button pick color by opening ColorPicker
	/// </summary>
	public class ColorPickButton : Button
	{
		public ColorPickButton()
		{
			SelectedColor = Color.Black;
			Size = new Size(16,16);
			ForeColor = Color.Transparent;
		}

		protected IColorLibrary[] lib;
		public IColorLibrary[] colorLibraries { 
			get { return lib; } 
			set
			{
				lib = value;
				if(picker!=null)
				{
					picker.Dock = DockStyle.None;
					picker.setColors(lib);
					picker.PalettesInRow = lib.Length<3?4:8;
					pickerForm.ClientSize = new Size(picker.Width+2,picker.Height+2);
					picker.Dock = DockStyle.Fill;
				}
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl ();
			if(lib!=null)
				picker = new ColorPicker(lib,lib.Length<3?4:8);
			else
				picker = new ColorPicker();
			
			pickerForm = new Form();
			pickerForm.ControlBox = false;
			pickerForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			//pickerForm.FormBorderStyle = FormBorderStyle.None;
			pickerForm.MaximizeBox = false;
			pickerForm.MinimizeBox = false;
			pickerForm.Name = "ColorPickerForm";
			pickerForm.ShowInTaskbar = false;
			pickerForm.SuspendLayout();
			picker.CreateControl();
			picker.SelectedColor = selected;
			picker.OnColorSelected +=new EventHandler(picker_OnColorSelected);
			pickerForm.ClientSize = new Size(picker.Width+2,picker.Height+2);
			picker.Dock = DockStyle.Fill;
			pickerForm.Controls.Add(picker);
			pickerForm.Deactivate+=new EventHandler(picker_LostFocus);
			pickerForm.ResumeLayout(false);
			pickerForm.Show();
			pickerForm.Hide();
		}

		protected Color selected;
		public Color SelectedColor
		{
			get	{ return selected; }
			set
			{
				selected =value;
				BackColor = selected;
//				if(picker!=null)
//					picker.SelectedColor = value;
			}
		}

		protected Form pickerForm;
		protected ColorPicker picker;
		public ColorPicker Picker{ get { return picker; } }

		private void picker_OnColorSelected(object sender, EventArgs e)
		{
			SelectedColor = picker.SelectedColor;
			pickerForm.Hide();
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick (e);
			//pickerForm.ClientSize = new Size(picker.Width,picker.Height);
			pickerForm.Location = PointToScreen(new Point(0,Height));
			picker.SelectedColor = selected;
			pickerForm.Focus();
			pickerForm.Show();
		}

		private void picker_LostFocus(object sender, EventArgs e)
		{
			pickerForm.Hide();
		}
	}
}
