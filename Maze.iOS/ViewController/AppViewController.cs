using System;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Maze.iOS
{
	public partial class AppViewController : UIViewController
	{
		public AppViewController () : base ("AppViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			PointF origin1 = this.ghost1.Center;
			PointF origin2 = this.ghost2.Center;
			PointF origin3 = this.ghost3.Center;

			PointF target1 = new PointF (this.ghost1.Center.X, this.ghost1.Center.Y - 124);
			PointF target2 = new PointF (this.ghost2.Center.X, this.ghost2.Center.Y + 284);
			PointF target3 = new PointF (this.ghost3.Center.X, this.ghost3.Center.Y - 284);

			CABasicAnimation bounce1 = new CABasicAnimation ();
			bounce1.KeyPath = "position.y";
			bounce1.From = new NSNumber(origin1.Y);
			bounce1.To = new NSNumber(target1.Y);
			bounce1.Duration = 2;
			bounce1.AutoReverses = true;
			bounce1.RepeatCount = float.MaxValue;

			CABasicAnimation bounce2 = CABasicAnimation.FromKeyPath ("position.y");
			bounce2.From = new NSNumber (origin2.Y);
			bounce2.To = new NSNumber (target2.Y);
			bounce2.Duration = 2;
			bounce2.AutoReverses = true;
			bounce2.RepeatCount = float.MaxValue;

			CABasicAnimation bounce3 = CABasicAnimation.FromKeyPath ("position.y");
			bounce3.From = new NSNumber (origin3.Y);
			bounce3.To = new NSNumber (target3.Y);
			bounce3.Duration = 2;
			bounce3.AutoReverses = true;
			bounce3.RepeatCount = float.MaxValue;

			this.ghost1.Layer.AddAnimation (bounce1, "position");
			this.ghost2.Layer.AddAnimation (bounce2, "position");
			this.ghost3.Layer.AddAnimation (bounce3, "position");
		}
	}
}