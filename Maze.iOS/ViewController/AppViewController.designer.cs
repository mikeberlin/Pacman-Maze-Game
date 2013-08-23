// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Maze.iOS
{
	[Register ("AppViewController")]
	partial class AppViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView exit { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ghost1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ghost2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ghost3 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView pacman { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView[] wall { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (pacman != null) {
				pacman.Dispose ();
				pacman = null;
			}

			if (ghost1 != null) {
				ghost1.Dispose ();
				ghost1 = null;
			}

			if (ghost2 != null) {
				ghost2.Dispose ();
				ghost2 = null;
			}

			if (ghost3 != null) {
				ghost3.Dispose ();
				ghost3 = null;
			}

			if (exit != null) {
				exit.Dispose ();
				exit = null;
			}
		}
	}
}
