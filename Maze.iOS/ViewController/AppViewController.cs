using System;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreMotion;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Maze.iOS
{
	public partial class AppViewController : UIViewController
	{
		private const float UPDATE_INTERVAL = (1.0f / 60.0f);

		private PointF CurrentPoint;
		private PointF PreviousPoint;
		private float PacmanXVelocity;
		private float PacmanYVelocity;
		private float Angle;
		private CMAcceleration Acceleration;
		private CMMotionManager MotionManager;
		private DateTime LastUpdateTime;

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
			this.InitializeGhosts ();
			this.SetupAccelerometerReading ();
		}

		#region Private Methods

		private void InitializeGhosts()
		{
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

		private void SetupAccelerometerReading()
		{
			// Movement of Pacmn
			this.LastUpdateTime = DateTime.Now;
			this.CurrentPoint = new PointF (0, 144);
			this.MotionManager = new CMMotionManager ();

			//this.MotionManager.AccelerometerUpdateInterval = UPDATE_INTERVAL;

			//this.MotionManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, (data, CMError) =>
			//{
			//	Console.WriteLine("{0}", data.Acceleration.X.ToString("0.0000000"));
			//});

			//this.MotionManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, (data, error) => {
				//this.Acceleration = data.Acceleration;
				//this.InvokeOnMainThread(UpdatePacman);
			//});
		}

		private void UpdatePacman()
		{
			int numSecondsSinceLastDraw = -(DateTime.Now - this.LastUpdateTime).Seconds;

			this.PacmanYVelocity = this.PacmanYVelocity - ((float)this.Acceleration.X * numSecondsSinceLastDraw);
			this.PacmanXVelocity = this.PacmanXVelocity - ((float)this.Acceleration.Y * numSecondsSinceLastDraw);

			float xDelta = (numSecondsSinceLastDraw * this.PacmanXVelocity * 500);
			float yDelta = (numSecondsSinceLastDraw * this.PacmanYVelocity * 500);

			this.CurrentPoint = new PointF (this.CurrentPoint.X + xDelta, this.CurrentPoint.Y + yDelta);

			this.MovePacman ();
			this.LastUpdateTime = DateTime.Now;
		}

		private void MovePacman()
		{
			this.PreviousPoint = this.CurrentPoint;

			RectangleF frame = this.pacman.Frame;
			frame.X = this.CurrentPoint.X;
			frame.Y = this.CurrentPoint.Y;

			this.pacman.Frame = frame;

			// Rotate the sprite
			float newAngle = (this.PacmanXVelocity + this.PacmanYVelocity) * (float)Math.PI * 4;
			this.Angle += newAngle * UPDATE_INTERVAL;

			CABasicAnimation rotate = new CABasicAnimation ();
			rotate.KeyPath = "transform.rotation";
			rotate.From = new NSNumber (0);
			rotate.To = new NSNumber (this.Angle);
			rotate.Duration = UPDATE_INTERVAL;
			rotate.RepeatCount = 1;
			rotate.RemovedOnCompletion = false;
			rotate.FillMode = CAFillMode.Forwards;

			this.pacman.Layer.AddAnimation (rotate, "10");
		}

		#endregion
	}
}