using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using freetrain.framework;

namespace freetrain.world.development
{
	public class DevelopConfigure : freetrain.controllers.AbstractControllerForm
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox tbLandPliceScale;
		private System.Windows.Forms.TextBox tbMaxPricePower;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbStrDiffuse;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbReplacePriceFactor;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbPopAmpScale;
		private System.Windows.Forms.TextBox tbPopAmpPower;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox tbQAlpha;
		private System.Windows.Forms.TextBox tbAddedQScale;
		private System.Windows.Forms.TextBox tbLandValuePower;
		private System.Windows.Forms.TextBox tbQDiffuse;
		private System.Windows.Forms.TextBox tbBaseRho;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.TextBox a;
		private System.ComponentModel.IContainer components = null;

		public DevelopConfigure() :base()
		{			
			
			// この呼び出しは Windows フォーム デザイナで必要です。
			InitializeComponent();
			
			tbLandPliceScale.Text = string.Format("{0:0.00}",SearchPlan.F_LandPriceScale.ToString());
			tbMaxPricePower.Text = string.Format("{0:0.00}", SearchPlan.F_MaxPricePower);
			tbStrDiffuse.Text = string.Format("{0:0.00}", SearchPlan.F_StrDiffuse);
			tbReplacePriceFactor.Text = string.Format("{0:0.00}", SearchPlan.F_ReplacePriceFactor);
			tbPopAmpScale.Text = string.Format("{0:0.00}", SearchPlan.F_PopAmpScale);			
			tbPopAmpPower.Text = string.Format("{0:0.00}", SearchPlan.F_PopAmpPower);			

			tbQAlpha.Text = string.Format("{0:0.000}", LandValue.ALPHA);			
			tbAddedQScale.Text = string.Format("{0}", LandValue.UPDATE_FREQUENCY);			
			tbLandValuePower.Text = string.Format("{0:0.00}", LandValue.LAND_VAL_POWER);			
			tbQDiffuse.Text = string.Format("{0:0.000}", LandValue.DIFF);			
			tbBaseRho.Text = string.Format("{0:0.00}", LandValue.RHO_BARE_LAND);			

		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region デザイナで生成されたコード
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DevelopConfigure));
            this.tbLandPliceScale = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbMaxPricePower = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.tbStrDiffuse = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbReplacePriceFactor = new System.Windows.Forms.TextBox();
            this.tbPopAmpScale = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbPopAmpPower = new System.Windows.Forms.TextBox();
            this.tbQDiffuse = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbQAlpha = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbBaseRho = new System.Windows.Forms.TextBox();
            this.tbAddedQScale = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tbLandValuePower = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.a = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbLandPliceScale
            // 
            this.tbLandPliceScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLandPliceScale.Location = new System.Drawing.Point(252, 15);
            this.tbLandPliceScale.Name = "tbLandPliceScale";
            this.tbLandPliceScale.Size = new System.Drawing.Size(72, 19);
            this.tbLandPliceScale.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbLandPliceScale, "Multiplier for land price to calculate lower limit of price of structure to be built" +
                    "");
			//! this.toolTip1.SetToolTip(this.tbLandPliceScale, "地価に対して乗じて、建物の最低価格にする");
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(248, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Coeff. structure minimum price (>=0.0):";
			//! this.label2.Text = "建物下限価格・対地価係数 (>=0.0):";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label2, "Multiplier for land price to calculate lower limit of price of structure to be built" +
                    "");
			//! this.toolTip1.SetToolTip(this.label2, "地価に対して乗じて、建物の最低価格にする");
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(248, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Coeff. structure maximum price (> 0.0):";
			//! this.label3.Text = "建物上限価格・対降車客指数 (> 0.0):";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label3, "Exponential for passengers to caluculate upper limit of price of structure to be bui" +
                    "lt");
			//! this.toolTip1.SetToolTip(this.label3, "積算降車客数の指数として適用し、建物の価格最大値を決める");
            // 
            // tbMaxPricePower
            // 
            this.tbMaxPricePower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMaxPricePower.Location = new System.Drawing.Point(252, 55);
            this.tbMaxPricePower.Name = "tbMaxPricePower";
            this.tbMaxPricePower.Size = new System.Drawing.Size(72, 19);
            this.tbMaxPricePower.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbMaxPricePower, "Exponential for passengers to calucurate upper limit of price of structure to be bui" +
                    "lt");
			//! this.toolTip1.SetToolTip(this.tbMaxPricePower, "積算降車客数の指数として適用し、建物の価格最大値を決める");
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(476, 296);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(80, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
			//! this.btnApply.Text = "適用";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(572, 296);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
			//! this.btnCancel.Text = "ｷｬﾝｾﾙ";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(6, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "1-[weekly effect reduce ratio] (0.0-1.0):";
			//! this.label1.Text = "1-発展度減衰率 (0.0〜1.0):";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label1, "Weekly multiplier for reducing intensity of development");
			//! this.toolTip1.SetToolTip(this.label1, "発展スコアに毎週乗じる係数");
            // 
            // tbStrDiffuse
            // 
            this.tbStrDiffuse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStrDiffuse.Location = new System.Drawing.Point(252, 94);
            this.tbStrDiffuse.Name = "tbStrDiffuse";
            this.tbStrDiffuse.Size = new System.Drawing.Size(72, 19);
            this.tbStrDiffuse.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbStrDiffuse, "Weekly multiplier for reducing intensity of development");
			//! this.toolTip1.SetToolTip(this.tbStrDiffuse, "発展スコアに毎週乗じる係数");
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(6, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(248, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Coeff. replacement threshold (>=0.0):";
			//! this.label4.Text = "建て替え価格係数 (>=0.0):";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label4, "Replacement occurs when existing structure price is lower than this coeff. mutipli" +
                    "ed with the land price.");
			//! this.toolTip1.SetToolTip(this.label4, "地価にこの値を乗じた値より、価格が低い建物は建て替え可能");
            // 
            // tbReplacePriceFactor
            // 
            this.tbReplacePriceFactor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReplacePriceFactor.Location = new System.Drawing.Point(252, 95);
            this.tbReplacePriceFactor.Name = "tbReplacePriceFactor";
            this.tbReplacePriceFactor.Size = new System.Drawing.Size(72, 19);
            this.tbReplacePriceFactor.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbReplacePriceFactor, "Replacement occurs when existing structure price is lower than this coeff. mutipli" +
                    "ed with the land price.");
			//! this.toolTip1.SetToolTip(this.tbReplacePriceFactor, "地価にこの値を乗じた値より、価格が低い建物は建て替え可能");
            // 
            // tbPopAmpScale
            // 
            this.tbPopAmpScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPopAmpScale.Location = new System.Drawing.Point(252, 54);
            this.tbPopAmpScale.Name = "tbPopAmpScale";
            this.tbPopAmpScale.Size = new System.Drawing.Size(72, 19);
            this.tbPopAmpScale.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbPopAmpScale, "Multiplier for station effective area (>= 0.0):");
			//! this.toolTip1.SetToolTip(this.tbPopAmpScale, "駅の発展範囲を決定するため地価に乗ずる値");
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(6, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(248, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "Multiplier for station effective area (>= 0.0):";
			//! this.label7.Text = "駅影響範囲・対地価乗数 (>= 0.0):";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label7, "Multiplier for land price to calculate effective area");
			//! this.toolTip1.SetToolTip(this.label7, "駅の発展範囲を決定するため地価に乗ずる値");
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(6, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(248, 16);
            this.label8.TabIndex = 1;
            this.label8.Text = "Coeff. station effective area (> 0.0):";
			//! this.label8.Text = "駅影響範囲・対地価指数 (> 0.0):";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label8, "Coefficient for station effective area against land price");
			//! this.toolTip1.SetToolTip(this.label8, "駅の発展範囲を決定する地価に対する指数");
            // 
            // tbPopAmpPower
            // 
            this.tbPopAmpPower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPopAmpPower.Location = new System.Drawing.Point(252, 14);
            this.tbPopAmpPower.Name = "tbPopAmpPower";
            this.tbPopAmpPower.Size = new System.Drawing.Size(72, 19);
            this.tbPopAmpPower.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbPopAmpPower, "Coefficient for station effective area against land price");
			//! this.toolTip1.SetToolTip(this.tbPopAmpPower, "駅の発展範囲を決定する地価に対する指数");
            // 
            // tbQDiffuse
            // 
            this.tbQDiffuse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbQDiffuse.Location = new System.Drawing.Point(246, 56);
            this.tbQDiffuse.Name = "tbQDiffuse";
            this.tbQDiffuse.Size = new System.Drawing.Size(72, 19);
            this.tbQDiffuse.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbQDiffuse, "Multiplied every phase to reduce land price");
			//! this.toolTip1.SetToolTip(this.tbQDiffuse, "毎回現在地価に乗ずる係数");
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(8, 56);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(238, 16);
            this.label11.TabIndex = 1;
            this.label11.Text = "1-[land price emit ratio] (0.0-0.999):";
			//! this.label11.Text = "1-地価発散率 (0.0〜0.999):";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label11, "Multiplied every phase to reduce land price");
			//! this.toolTip1.SetToolTip(this.label11, "毎回現在地価に乗ずる係数");
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Location = new System.Drawing.Point(8, 96);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(238, 16);
            this.label12.TabIndex = 1;
            this.label12.Text = "Land price conductivity (0-0.25):";
			//! this.label12.Text = "地価伝導率 (0〜0.25):";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label12, "The ratio of influence on land price by neighboring voxel");
			//! this.toolTip1.SetToolTip(this.label12, "隣のボクセルから伝わる地価落差に乗ずる係数");
            // 
            // tbQAlpha
            // 
            this.tbQAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbQAlpha.Location = new System.Drawing.Point(246, 96);
            this.tbQAlpha.Name = "tbQAlpha";
            this.tbQAlpha.Size = new System.Drawing.Size(72, 19);
            this.tbQAlpha.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbQAlpha, "The ratio of influence on land price by neighboring voxel");
			this.toolTip1.SetToolTip(this.tbQAlpha, "隣のボクセルから伝わる地価落差に乗ずる係数");
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Location = new System.Drawing.Point(8, 15);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(238, 17);
            this.label13.TabIndex = 1;
            this.label13.Text = "Land price diffusivity (0.4-0.999):";
			//! this.label13.Text = "標準地価伝播密度 (0.4〜0.999):";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label13, "The ratio of land price diffusion for neighboring voxels");
			//! this.toolTip1.SetToolTip(this.label13, "地価が隣のボクセルに伝播する割合");
            // 
            // tbBaseRho
            // 
            this.tbBaseRho.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBaseRho.Location = new System.Drawing.Point(246, 16);
            this.tbBaseRho.Name = "tbBaseRho";
            this.tbBaseRho.Size = new System.Drawing.Size(72, 19);
            this.tbBaseRho.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbBaseRho, "The ratio of land price diffusion for neighboring voxels");
			//! this.toolTip1.SetToolTip(this.tbBaseRho, "地価が隣のボクセルに伝播する割合");
            // 
            // tbAddedQScale
            // 
            this.tbAddedQScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAddedQScale.Location = new System.Drawing.Point(246, 136);
            this.tbAddedQScale.Name = "tbAddedQScale";
            this.tbAddedQScale.Size = new System.Drawing.Size(72, 19);
            this.tbAddedQScale.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbAddedQScale, "Multiplier for land price increment when passengers arrives");
			//! this.toolTip1.SetToolTip(this.tbAddedQScale, "駅降車時の地価加算値に乗ずる係数");
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(8, 136);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(238, 16);
            this.label14.TabIndex = 1;
            this.label14.Text = "Multiplier for land price increase (>=0, int):";
			//! this.label14.Text = "地価上昇補正係数 (>= 0, 整数):";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label14, "Multiplier for land price increment when passengers arrives");
			//! this.toolTip1.SetToolTip(this.label14, "駅降車時の地価加算値に乗ずる係数");
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.Location = new System.Drawing.Point(8, 176);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(238, 16);
            this.label15.TabIndex = 1;
            this.label15.Text = "Land price modifier (> 0.0):";
			//! this.label15.Text = "最終地価補正指数 (> 0.0):";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label15, "Multiplier from converted internal land value to land price");
			//! this.toolTip1.SetToolTip(this.label15, "内部地価データを実際の地価に換算するための補正指数");
            // 
            // tbLandValuePower
            // 
            this.tbLandValuePower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLandValuePower.Location = new System.Drawing.Point(246, 176);
            this.tbLandValuePower.Name = "tbLandValuePower";
            this.tbLandValuePower.Size = new System.Drawing.Size(72, 19);
            this.tbLandValuePower.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbLandValuePower, "Multiplier from converted internal land value to land price");
			//! this.toolTip1.SetToolTip(this.tbLandValuePower, "内部地価データを実際の地価に換算するための補正指数");
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.BackColor = System.Drawing.SystemColors.Highlight;
            this.label10.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label10.Location = new System.Drawing.Point(8, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(238, 16);
            this.label10.TabIndex = 1;
            this.label10.Text = "The larger the more widely is land price raised";
			//! this.label10.Text = "大きくすると広範囲に地価が広がります";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label10, "The ratio of land price diffusion for neighboring voxels");
			//! this.toolTip1.SetToolTip(this.label10, "地価が隣のボクセルに伝播する割合");
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.BackColor = System.Drawing.SystemColors.Highlight;
            this.label16.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label16.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label16.Location = new System.Drawing.Point(8, 72);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(238, 16);
            this.label16.TabIndex = 1;
            this.label16.Text = "The larger the more widely is land price raised";
			//! this.label16.Text = "大きくすると広範囲に地価が広がります";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label16, "Multiplied every phase to reduce land price");
			//! this.toolTip1.SetToolTip(this.label16, "毎回現在地価に乗ずる係数");
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.BackColor = System.Drawing.SystemColors.Highlight;
            this.label17.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label17.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label17.Location = new System.Drawing.Point(8, 112);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(238, 16);
            this.label17.TabIndex = 1;
            this.label17.Text = "The larger the more quickly land price rises";
			//! this.label17.Text = "大きくすると地価上昇が早く広がります";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label17, "The ratio in which land price affects neighboring voxels");
			//! this.toolTip1.SetToolTip(this.label17, "隣のボクセルから伝わる地価落差に乗ずる係数");
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.BackColor = System.Drawing.SystemColors.Highlight;
            this.label18.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label18.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label18.Location = new System.Drawing.Point(8, 152);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(238, 16);
            this.label18.TabIndex = 1;
            this.label18.Text = "Enhance land price increment for transportation";
			//! this.label18.Text = "大きくすると少ない輸送で地価が上昇します";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label18, "Multiplier for land price increment when passengers arrives");
			//! this.toolTip1.SetToolTip(this.label18, "駅降車時の地価加算値に乗ずる係数");
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.BackColor = System.Drawing.SystemColors.Highlight;
            this.label19.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label19.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label19.Location = new System.Drawing.Point(8, 192);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(238, 16);
            this.label19.TabIndex = 1;
            this.label19.Text = "Enhance land price increment for transportation";
			//! this.label19.Text = "大きくすると少ない輸送で地価が上昇します";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label19, "Multiplier for land price increment when passengers arrives");
			//! this.toolTip1.SetToolTip(this.label19, "駅降車時の地価加算値に乗ずる係数");
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label20.BackColor = System.Drawing.SystemColors.Highlight;
            this.label20.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label20.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label20.Location = new System.Drawing.Point(6, 32);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(248, 16);
            this.label20.TabIndex = 1;
            this.label20.Text = "The larger the wider area around stations develops";
			//! this.label20.Text = "大きくすると駅の発展範囲が広くなります";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label20, "Coefficient for station effective area against land price");
			//! this.toolTip1.SetToolTip(this.label20, "駅の発展範囲を決定する地価に対する指数");
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label21.BackColor = System.Drawing.SystemColors.Highlight;
            this.label21.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label21.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label21.Location = new System.Drawing.Point(6, 72);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(248, 16);
            this.label21.TabIndex = 1;
            this.label21.Text = "The larger the wider area around stations develops";
			//! this.label21.Text = "大きくすると駅の発展範囲が広くなります";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label21, "Multiplier for land price to calculate effective area");
			//! this.toolTip1.SetToolTip(this.label21, "駅の発展範囲を決定するため地価に乗ずる値");
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label22.BackColor = System.Drawing.SystemColors.Highlight;
            this.label22.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label22.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label22.Location = new System.Drawing.Point(6, 112);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(248, 16);
            this.label22.TabIndex = 1;
            this.label22.Text = "The larger the more expensive structures are built";
			//! this.label22.Text = "大きくすると高価な建築が建ちやすくなります";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label22, "Weekly multiplier to reduce intensity of development");
			//! this.toolTip1.SetToolTip(this.label22, "駅の発展範囲を決定するため地価に乗ずる値");
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.BackColor = System.Drawing.SystemColors.Highlight;
            this.label9.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label9.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label9.Location = new System.Drawing.Point(6, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(248, 16);
            this.label9.TabIndex = 1;
            this.label9.Text = "The larger the cheaper structures are built";
			//! this.label9.Text = "大きくすると安価な建築が建ちにくくなります";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label9, "Multiplier for land price to calculate lower limit of the price of structures to be built" +
                    "");
			//! this.toolTip1.SetToolTip(this.label9, "地価に対して乗じて、建物の最低価格にする");
            // 
            // label23
            // 
            this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label23.BackColor = System.Drawing.SystemColors.Highlight;
            this.label23.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label23.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label23.Location = new System.Drawing.Point(6, 72);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(248, 16);
            this.label23.TabIndex = 1;
            this.label23.Text = "The larger the more expensive structures are built";
			//! this.label23.Text = "大きくすると高価な建築が建ちやすくなります";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label23, "Exponential for passengers to caluculate upper limit of the price of structures to be bui" +
                    "lt");
			//! this.toolTip1.SetToolTip(this.label23, "積算降車客数の指数として適用し、建物の価格最大値を決める");
            // 
            // label24
            // 
            this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label24.BackColor = System.Drawing.SystemColors.Highlight;
            this.label24.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label24.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label24.Location = new System.Drawing.Point(6, 112);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(248, 16);
            this.label24.TabIndex = 1;
            this.label24.Text = "The smaller the more replacement occurs";
			//! this.label24.Text = "大きくすると建て替えが起こりにくくなります";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label24, "Replacement occurs when existing structure price is lower than the land value" +
                    "multiplied with this coeff.");
			//! this.toolTip1.SetToolTip(this.label24, "地価にこの値を乗じた値より、価格が低い建物は建て替え可能");
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(8, 288);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(652, 2);
            this.label5.TabIndex = 3;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(16, 296);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save Settings";
			//! this.btnSave.Text = "設定保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.Location = new System.Drawing.Point(112, 296);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(80, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load Settings";
			//! this.btnLoad.Text = "設定読込";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.tbQDiffuse);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.tbLandValuePower);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.tbAddedQScale);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.tbBaseRho);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.tbQAlpha);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Location = new System.Drawing.Point(0, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 216);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Land price";
			//! this.groupBox1.Text = "地価";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tbPopAmpScale);
            this.groupBox2.Controls.Add(this.tbStrDiffuse);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbPopAmpPower);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Location = new System.Drawing.Point(332, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(332, 136);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Area development rate";
			//! this.groupBox2.Text = "発展度";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.tbReplacePriceFactor);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.tbMaxPricePower);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.tbLandPliceScale);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Controls.Add(this.label24);
            this.groupBox3.Location = new System.Drawing.Point(332, 144);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(332, 136);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Structure selection";
			//! this.groupBox3.Text = "建物価格";
            // 
            // a
            // 
            this.a.BackColor = System.Drawing.SystemColors.Control;
            this.a.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.a.ForeColor = System.Drawing.SystemColors.InfoText;
            this.a.Location = new System.Drawing.Point(6, 224);
            this.a.Multiline = true;
            this.a.Name = "a";
            this.a.ReadOnly = true;
            this.a.Size = new System.Drawing.Size(312, 64);
            this.a.TabIndex = 7;
            this.a.Text = resources.GetString("a.Text");
            // 
            // DevelopConfigure
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(670, 323);
            this.Controls.Add(this.a);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Name = "DevelopConfigure";
            this.Text = "Development Algorithm: Parameters";
			//! this.Text = "発展アルゴリズム：パラメータ";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btnCancel_Click(object sender, System.EventArgs e) {
			Dispose();
		}

		private void btnApply_Click(object sender, System.EventArgs e) {
			try{
				SearchPlan.F_LandPriceScale = double.Parse(tbLandPliceScale.Text);
				SearchPlan.F_MaxPricePower = double.Parse(tbMaxPricePower.Text);
				SearchPlan.F_StrDiffuse = double.Parse(tbStrDiffuse.Text);
				SearchPlan.F_ReplacePriceFactor = double.Parse(tbReplacePriceFactor.Text);
				SearchPlan.F_PopAmpScale = double.Parse(tbPopAmpScale.Text);
				SearchPlan.F_PopAmpPower = double.Parse(tbPopAmpPower.Text);

				LandValue.ALPHA = float.Parse(tbQAlpha.Text);
				LandValue.UPDATE_FREQUENCY = int.Parse(tbAddedQScale.Text);
				LandValue.LAND_VAL_POWER = double.Parse(tbLandValuePower.Text);
				LandValue.DIFF = float.Parse(tbQDiffuse.Text);
				LandValue.RHO_BARE_LAND = float.Parse(tbBaseRho.Text);
			}finally{
				Dispose();
			}
		}

		private void btnSave_Click(object sender, System.EventArgs e) {
			int i=0;
			Core.options.devParams[i++] = double.Parse(tbLandPliceScale.Text);
			Core.options.devParams[i++] = double.Parse(tbMaxPricePower.Text);
			Core.options.devParams[i++] = double.Parse(tbStrDiffuse.Text);
			Core.options.devParams[i++] = double.Parse(tbReplacePriceFactor.Text);
			Core.options.devParams[i++] = double.Parse(tbPopAmpScale.Text);
			Core.options.devParams[i++] = double.Parse(tbPopAmpPower.Text);

			Core.options.devParams[i++] = float.Parse(tbQAlpha.Text);
			Core.options.devParams[i++] = int.Parse(tbAddedQScale.Text);
			Core.options.devParams[i++] = double.Parse(tbLandValuePower.Text);
			Core.options.devParams[i++] = float.Parse(tbQDiffuse.Text);
			Core.options.devParams[i++] = float.Parse(tbBaseRho.Text);		
		}

		private void btnLoad_Click(object sender, System.EventArgs e) {
			int i=0;
			tbLandPliceScale.Text =  string.Format("{0:0.00}",Core.options.devParams[i++]);
			tbMaxPricePower.Text = string.Format("{0:0.00}",Core.options.devParams[i++]);
			tbStrDiffuse.Text = string.Format("{0:0.00}",Core.options.devParams[i++]);
			tbReplacePriceFactor.Text = string.Format("{0:0.00}",Core.options.devParams[i++]);
			tbPopAmpScale.Text = string.Format("{0:0.00}",Core.options.devParams[i++]);
			tbPopAmpPower.Text = string.Format("{0:0.00}",Core.options.devParams[i++]);

			tbQAlpha.Text = string.Format("{0:0.000}",Core.options.devParams[i++]);
			tbAddedQScale.Text = string.Format("{0}",(int)Core.options.devParams[i++]);
			tbLandValuePower.Text = string.Format("{0:0.00}",Core.options.devParams[i++]);
			tbQDiffuse.Text = string.Format("{0:0.000}",Core.options.devParams[i++]);
			tbBaseRho.Text = string.Format("{0:0.00}",Core.options.devParams[i++]);
		}
	}
}

