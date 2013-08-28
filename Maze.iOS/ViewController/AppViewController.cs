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

		private PointF _currentPoint;
		private PointF _previousPoint;
		private float _pacmanXVelocity;
		private float _pacmanYVelocity;
		private float _angle;
		private CMAcceleration _acceleration;
		private CMMotionManager _motionManager;
		private DateTime _lastUpdateTime;

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
			InitializeGhosts ();
			SetupAccelerometerReading ();
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
			// Movement of Pacman
			_lastUpdateTime = DateTime.Now;
			_currentPoint = new PointF (0, 144);
			_motionManager = new CMMotionManager ();
			_motionManager.AccelerometerUpdateInterval = UPDATE_INTERVAL;

			_motionManager.StartAccelerometerUpdates (NSOperationQueue.CurrentQueue, (data, error) => {
				_acceleration = data.Acceleration;
				InvokeOnMainThread(UpdatePacman);
			});
		}

		private void UpdatePacman()
		{
            double numSecondsSinceLastDraw = (DateTime.Now - _lastUpdateTime).TotalSeconds;

			_pacmanYVelocity = _pacmanYVelocity - (float)(_acceleration.X * numSecondsSinceLastDraw);
			_pacmanXVelocity = _pacmanXVelocity - (float)(_acceleration.Y * numSecondsSinceLastDraw);

			float xDelta = ((float)numSecondsSinceLastDraw * _pacmanXVelocity * 500);
			float yDelta = ((float)numSecondsSinceLastDraw * _pacmanYVelocity * 500);

			_currentPoint = new PointF (_currentPoint.X + xDelta, _currentPoint.Y + yDelta);

			MovePacman ();
			_lastUpdateTime = DateTime.Now;
		}

		private void MovePacman()
		{
			CollisionWithExit ();
			CollisionWithGhosts ();
			CollisionWithWalls ();
			CollisionWithBoundaries ();

			_previousPoint = _currentPoint;

			RectangleF frame = pacman.Frame;
			frame.X = _currentPoint.X;
			frame.Y = _currentPoint.Y;
			pacman.Frame = frame;

			// Rotate the sprite
			float newAngle = (_pacmanXVelocity + _pacmanYVelocity) * (float)Math.PI * 4;
			_angle += newAngle * UPDATE_INTERVAL;

			CABasicAnimation rotate = new CABasicAnimation ();
			rotate.KeyPath = "transform.rotation";
			rotate.From = new NSNumber (0);
			rotate.To = new NSNumber (_angle);
			rotate.Duration = UPDATE_INTERVAL;
			rotate.RepeatCount = 1;
			rotate.RemovedOnCompletion = false;
			rotate.FillMode = CAFillMode.Forwards;

			pacman.Layer.AddAnimation (rotate, "10");
		}

		private void CollisionWithBoundaries()
		{
			if (_currentPoint.X < 0) {
				_currentPoint.X = 0;
				_pacmanXVelocity = -(float)(_pacmanXVelocity / 2.0);
			}

			if (_currentPoint.Y < 0) {
				_currentPoint.Y = 0;
				_pacmanYVelocity = -(float)(_pacmanYVelocity / 2.0);
			}

			if (_currentPoint.X > this.View.Bounds.Size.Width - pacman.Image.Size.Width) {
				_currentPoint.X = this.View.Bounds.Size.Width - pacman.Image.Size.Width;
				_pacmanXVelocity = -(float)(_pacmanXVelocity / 2.0);
			}

			if (_currentPoint.Y > this.View.Bounds.Size.Height - pacman.Image.Size.Height) {
				_currentPoint.Y = this.View.Bounds.Size.Height - pacman.Image.Size.Height;
				_pacmanYVelocity = -(float)(_pacmanYVelocity / 2.0);
			}
		}

		private void CollisionWithExit()
		{
			if (pacman.Frame.IntersectsWith(exit.Frame)) {
				_motionManager.StopAccelerometerUpdates ();
				_pacmanXVelocity = 0;
				_pacmanYVelocity = 0;

				new UIAlertView ("Congratulations", "You've won the game!", null, "OK", null).Show ();
			}
		}

		private void CollisionWithGhosts()
		{
			CALayer ghostLayer1 = ghost1.Layer.PresentationLayer;
			CALayer ghostLayer2 = ghost2.Layer.PresentationLayer;
			CALayer ghostLayer3 = ghost3.Layer.PresentationLayer;

			if (pacman.Frame.IntersectsWith (ghostLayer1.Frame) ||
				pacman.Frame.IntersectsWith (ghostLayer2.Frame) ||
				pacman.Frame.IntersectsWith (ghostLayer3.Frame)) {

				_currentPoint = new PointF (0, 144);

				new UIAlertView ("Oops", "Mission Failed!", null, "OK", null).Show ();
			}
		}

		private void CollisionWithWalls()
		{
			RectangleF frame = pacman.Frame;
			frame.X = _currentPoint.X;
			frame.Y = _currentPoint.Y;

			foreach (UIImageView image in wall) {

				if (frame.IntersectsWith (image.Frame)) {
					// Compute collision angle
					PointF pacmanCenter = new PointF (frame.X + (frame.Size.Width / 2),
					                                 frame.Y + (frame.Size.Height / 2));

					PointF imageCenter = new PointF (image.Frame.X + (image.Frame.Size.Width / 2),
					                                image.Frame.Y + (image.Frame.Size.Height / 2));

					float angleX = (pacmanCenter.X - imageCenter.X);
					float angleY = (pacmanCenter.Y - imageCenter.Y);

					if (Math.Abs (angleX) > Math.Abs (angleY)) {
						_currentPoint.X = _previousPoint.X;
						_pacmanXVelocity = -(float)(_pacmanXVelocity / 2.0);
					} else {
						_currentPoint.Y = _previousPoint.Y;
						_pacmanYVelocity = -(float)(_pacmanYVelocity / 2.0);
					}
				}

			}
		}

		#endregion
	}
}