using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouchKxMovie;
using System.Drawing;

namespace MonoTouchKxMovieExample
{
	public partial class MainViewController : UIViewController
	{
		NSMutableDictionary gHistory;
		KxMovieDecoder decoder1, decoder2;
		KxMovieGLView glView1, glView2;
		NSMutableArray _videoFrames;
		NSMutableArray _audioFrames;
		NSMutableArray _subtitles;
		NSData _currentAudioFrame;
		UInt32 _currentAudioFramePos;
		float _moviePosition;
		bool _disableUpdateHUD;
		long _tickCorrectionTime;
		long _tickCorrectionPosition;
		long _tickCounter;
		bool _fullscreen;
		bool _hiddenHUD;
		bool _fitMode;
		bool _infoMode;
		bool _restoreIdleTimer;
		bool _interrupted;
		float _bufferedDuration;
		float _minBufferedDuration;
		float _maxBufferedDuration;
		bool _buffered;
		const float LOCAL_MIN_BUFFERED_DURATION = 0.2f;
		const float LOCAL_MAX_BUFFERED_DURATION = 0.4f;
		const float NETWORK_MIN_BUFFERED_DURATION = 2.0f;
		const float NETWORK_MAX_BUFFERED_DURATION = 4.0f;

		public bool playing{ get; private set; }

		public bool decoding { get; private set; }

		private bool interruptDecoder {
			get {
				return _interrupted;
			}
		}

		public MainViewController () : base ("MainViewController", null)
		{
			// Custom initialization
			_moviePosition = 0;
			this.WantsFullScreenLayout = true;



			decoder1 = new KxMovieDecoder ();
			decoder2 = new KxMovieDecoder ();

			decoder1.interruptCallback = () => {
				return this.interruptDecoder;
			};
			decoder2.interruptCallback = () => {
				return this.interruptDecoder;
			};

			NSError err1 = null, err2 = null;
			decoder1.openFile ("capturedvideo.MOV", out err1);
			decoder2.openFile ("capturedvideo2.MOV", out err2);

			setMovieDecoder (decoder1, err1);
			setMovieDecoder (decoder2, err2);
		}

		public override void LoadView ()
		{
			base.LoadView ();

			if (decoder1 != null) {
				this.setupPresentView ();
			}

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
 
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (decoder1 != null) {
				this.restorePlay ();
			}

			NSNotificationCenter.DefaultCenter.AddObserver (this,
				new MonoTouch.ObjCRuntime.Selector ("applicationWillResignActive:"), 
				UIApplication.WillResignActiveNotification,
				UIApplication.SharedApplication);

		}

		[Export ("applicationWillResignActive:")]
		void applicationWillResignActive ()
		{

		}

		private void setMoviePositionFromDecoder ()
		{
			_moviePosition = decoder1.position;
		}

		public override void ViewWillDisappear (bool animated)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
			base.ViewWillDisappear (animated);

			if (decoder1 != null) {

				this.pause ();

				if (_moviePosition == 0 || decoder1.isEOF) {
					gHistory.Remove (new NSString (decoder1.path));
				} else if (!decoder1.isNetwork) {
					gHistory.SetValueForKey (new NSNumber (_moviePosition), new NSString (decoder1.path));
				}
			}

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		partial void showInfo (NSObject sender)
		{
//			var controller = new FlipsideViewController () {
//				ModalTransitionStyle = UIModalTransitionStyle.FlipHorizontal,
//			};
//			
//			controller.Done += delegate {
//				DismissModalViewControllerAnimated (true);
//			};
//			
//			PresentModalViewController (controller, true);
		}

		private void setMovieDecoder (KxMovieDecoder   decoder, NSError error)
		{
			_videoFrames = new NSMutableArray ();

			if (decoder1.isNetwork) {

				_minBufferedDuration = NETWORK_MIN_BUFFERED_DURATION;
				_maxBufferedDuration = NETWORK_MAX_BUFFERED_DURATION;

			} else {

				_minBufferedDuration = LOCAL_MIN_BUFFERED_DURATION;
				_maxBufferedDuration = LOCAL_MAX_BUFFERED_DURATION;
			}

			if (!decoder1.validVideo)
				_minBufferedDuration *= 10.0f; 

			if (_maxBufferedDuration < _minBufferedDuration)
				_maxBufferedDuration = _minBufferedDuration * 2;

			if (this.IsViewLoaded) {
				setupPresentView ();

				this.restorePlay ();
			}

		}

		private void restorePlay ()
		{

//			NSNumber n =(NSNumber) gHistory.ValueForKey ( new NSString(decoder1.path));
//			if (n != NSNull) {
//				this.updatePosition (n.FloatValue, true);
//			} else {
//				this.play ();
//			}

			this.play ();
		}

		private void updatePosition (float position, bool playMode)
		{

		}

		private void setupPresentView ()
		{
			RectangleF bounds = this.View.Bounds;
			glView1 = new KxMovieGLView (new RectangleF (0, 20, bounds.Width, 200), decoder1);
			glView2 = new KxMovieGLView (new RectangleF (0, 240, bounds.Width, 200), decoder1);

			UIView frameView = glView1;
			frameView.ContentMode = UIViewContentMode.ScaleAspectFit;
			frameView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleBottomMargin;

			this.View.InsertSubview (frameView, 0);


			UIView frameView2 = glView2;
			frameView2.ContentMode = UIViewContentMode.ScaleAspectFit;
			frameView2.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleBottomMargin;

			this.View.InsertSubview (frameView2, 0);

			if (decoder1.validVideo)
				this.setupUserInteraction ();
		}

		private void setupUserInteraction ()
		{

		}

		private void asyncDecodeFrames ()
		{
			if (this.decoding)
				return;

			float duration = decoder1.isNetwork ? 0 : 0.1f;
			this.decoding = true;

			this.InvokeOnMainThread (() => {

				bool good = true;

				using (var pool = new NSAutoreleasePool ()) { 
					while (good && !decoder1.isEOF) {
						NSArray frames = decoder1.decodeFrames (duration);
						Console.WriteLine("position:{0}",decoder1.position);
						if (frames.Count > 0) {
							good = this.addFrames (frames);
						}
					}
				}

				this.decoding = false;
			});
		}

		private void play ()
		{
			if (this.playing)
				return;
    
			if (!decoder1.validVideo &&
			    !decoder1.validAudio) {
        
				return;
			}
    
			if (_interrupted)
				return;

			this.playing = true;
			_interrupted = false;
			_disableUpdateHUD = false;
			_tickCorrectionTime = 0;
			_tickCounter = 0;

 

			this.asyncDecodeFrames ();
     
			System.Timers.Timer timer = new System.Timers.Timer (100);
			timer.Elapsed += (sender, e) => {
				System.Timers.Timer s = (System.Timers.Timer)sender;
				s.Stop ();
				this.tick ();
			};
			timer.Start ();

			Console.WriteLine (@"play movie"); 
		}

		private void pause ()
		{
			if (!this.playing)
				return;

			this.playing = false;
			Console.WriteLine (@"pause movie");
		}

		private void tick ()
		{

			//Console.WriteLine (@"tick");
			if (_buffered && ((_bufferedDuration > _minBufferedDuration) || decoder1.isEOF)) {

				_tickCorrectionTime = 0;
				_buffered = false;
				     
			}

			float interval = 0;
			if (!_buffered)
				interval = this.presentFrame ();
			if (float.IsNaN (interval))
				interval = 0.033333f;

			if (this.playing) {

				UInt32 leftFrames =
					(decoder1.validVideo ? _videoFrames.Count : 0) +
					(decoder1.validAudio ? _audioFrames.Count : 0);

				if (0 == leftFrames) {

					if (decoder1.isEOF) {
						this.pause ();
						return;
					}
					  
					if (_minBufferedDuration > 0 && !_buffered) {
						_buffered = true;
						 
					}
				}

				if (leftFrames == 0 ||
				    !(_bufferedDuration > _minBufferedDuration)) {

					this.asyncDecodeFrames ();
				}


				float correction = this.tickCorrection ();
				float time = Math.Max (interval + correction, 0.01f);
				if (float.IsNaN (time)) {
					time = 0.02f;
				}

				Console.WriteLine ("time:{0}", time);

				System.Threading.Timer timer=new System.Threading.Timer(new System.Threading.TimerCallback( (state)=>{
					System.Threading.Timer t = (System.Threading.Timer)state;
					//释放定时器资源
					t.Dispose();
					this.tick ();
					Console.WriteLine ("timer.Elapsed");
				} ));
				timer.Change (Convert.ToInt32 (time * 1000),  System.Threading.Timeout.Infinite);

//				System.Timers.Timer timer = new System.Timers.Timer (10000);//Convert.ToInt32 (time * 1000));
//				timer.Elapsed += (sender, e) => {
//					System.Timers.Timer s = (System.Timers.Timer)sender;
//					s.Stop ();
//					this.tick ();
//
//					Console.WriteLine ("timer.Elapsed");
//				};
//				timer.Start ();
			}

		}

		private float tickCorrection ()
		{
			if (_buffered)
				return 0;

			long now = DateTime.Now.Ticks;

			if (_tickCorrectionTime == 0) {
				_tickCorrectionTime = now;
				_tickCorrectionPosition = Convert.ToInt64 (_moviePosition * 1000);
				return 0;
			}

			long dPosition = Convert.ToInt64 (_moviePosition * 1000) - _tickCorrectionPosition;
			long dTime = now - _tickCorrectionTime;
			long correction = dPosition - dTime;

			//if ((_tickCounter % 200) == 0)
			//    NSLog(@"tick correction %.4f", correction);

			if (correction > 1000 || correction < -1000) {
				//Console.WriteLine (@"tick correction reset {0}", correction);
				correction = 0;
				_tickCorrectionTime = 0;
			}

			return Convert.ToSingle (correction / 1000.0);
		}

		private float presentFrame ()
		{
			float interval = 0;
    
			if (decoder1.validVideo) {
        
				KxVideoFrame frame = null;
        
				lock (_videoFrames) {
            
					if (_videoFrames.Count > 0) {
                
						frame = _videoFrames.GetItem<KxVideoFrame> (0);
						_videoFrames.RemoveObject (0);
						_bufferedDuration -= frame.duration;
					}
				}
        
				if (frame != null)
					interval = this.presentVideoFrame (frame);
        
			}

			return interval;
		}

		private  float presentVideoFrame (KxVideoFrame   frame)
		{
			glView1.render (frame);
			glView2.render (frame);
			_moviePosition = frame.position;
			if (float.IsNaN (_moviePosition))
				_moviePosition = 0;
			//Console.WriteLine ("_moviePosition:{0}", _moviePosition);
			return frame.duration;
		}

		private bool addFrames (NSArray frames)
		{
			if (decoder1.validVideo) {
				lock (_videoFrames) {
					for (int i = 0; i < frames.Count; i++) {
						KxMovieFrame frame = frames.GetItem<KxMovieFrame> (i);
						;
						if (frame.type == KxMovieFrameType.KxMovieFrameTypeVideo) {
							_videoFrames.Add (frame);
							_bufferedDuration += frame.duration;
						}
					}
//					foreach (NSObject f in frames) {
//						KxMovieFrame frame = (KxMovieFrame)f;
//						if (frame.type == KxMovieFrameType.KxMovieFrameTypeVideo) {
//							_videoFrames.AddObjects (frame);
//							_bufferedDuration += frame.duration;
//						}
//					}
				}	
			}

			return this.playing && _bufferedDuration < _maxBufferedDuration;
		}
	}
}

