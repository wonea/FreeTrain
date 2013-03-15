using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.views.map;
using freetrain.world;

namespace freetrain.controllers
{
	public class AbstractControllerImpl : AbstractControllerForm, ModalController
	{
		public AbstractControllerImpl() {
		}

		protected override void OnActivated( EventArgs e ) {
			base.OnActivated(e);
			// Attach the control when activated.
			try
			{
				MainWindow.mainWindow.attachController(this);
			}
			catch(NullReferenceException nre)
			{
				Debug.WriteLine(nre);
			}
		}

		/// <summary>
		/// Derived class still needs to extend this method and maintain
		/// the singleton.
		/// </summary>
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			// Detach it when it is closed.
			if(MainWindow.mainWindow.currentController==this)
				MainWindow.mainWindow.detachController();
		}

		//
		// default implementation for ModalController
		//
		public void close() {
			base.Close();
		}

		public string name { get { return Text; } }

		public virtual LocationDisambiguator disambiguator { get { return null;}  }

		public virtual MapOverlay overlay { get { return this as MapOverlay; } }

		public virtual void onAttached() {}
		public virtual void onDetached() {
			// redraw the entire surface to erase any left-over from this controller
			World.world.onAllVoxelUpdated();
		}

		public virtual void onClick( MapViewWindow source, Location loc, Point ab ) {}
		public virtual void onMouseMove( MapViewWindow view, Location loc, Point ab ) {}
		public virtual void onRightClick( MapViewWindow source, Location loc, Point ab ) {
			Close();	// cancel
		}
	}
}
