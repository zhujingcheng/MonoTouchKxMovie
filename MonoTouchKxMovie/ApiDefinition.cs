using System;
using System.Drawing;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MonoTouchKxMovie
{

	//typedef void (^KxAudioManagerOutputBlock)(float *data, UInt32 numFrames, UInt32 numChannels);
	delegate void KxAudioManagerOutputBlock (float data,UInt32 numFrames,UInt32 numChannels);

	[Model, BaseType(typeof(NSObject))]
	interface KxAudioManagerInterface {


		//@property (readonly) UInt32             numOutputChannels;
		[Export("numOutputChannels")]
		UInt32 numOutputChannels { get;}

		//@property (readonly) Float64            samplingRate;
		[Export("samplingRate")]
		double samplingRate { get;}

		//@property (readonly) UInt32             numBytesPerSample;
		[Export("numBytesPerSample")]
		UInt32 numBytesPerSample { get;}

		//@property (readonly) Float32            outputVolume;
		[Export("outputVolume")]
		float outputVolume { get;}

		//@property (readonly) BOOL               playing;
		[Export("playing")]
		bool playing { get;}

		//@property (readonly, strong) NSString   *audioRoute;
		[Export("audioRoute")]
		string audioRoute { get;}

		//@property (readwrite, copy) KxAudioManagerOutputBlock outputBlock;
//		[Export("outputBlock")]
//		KxAudioManagerOutputBlock outputBlock { get; set; }

		//- (BOOL) activateAudioSession;
		[Export("activateAudioSession")]
		bool activateAudioSession();
		//- (void) deactivateAudioSession;
		[Export("deactivateAudioSession")]
		void deactivateAudioSession();
		//- (BOOL) play;
		[Export("play")]
		bool play();
		//- (void) pause;
		[Export("pause")]
		void pause();
	}

	//typedef void (^KxAudioManagerOutputBlock)(float *data, UInt32 numFrames, UInt32 numChannels);
	//delegate void KxAudioManagerOutputBlock (float data,UInt32 numFrames,UInt32 numChannels);

	[BaseType(typeof(NSObject))]
	interface KxAudioManager {

		//+ (id<KxAudioManagerInterface>) audioManager;
		[Static,Export("audioManager")]
		KxAudioManagerInterface audioManager();
	}






	[BaseType(typeof(NSObject))]
	interface KxMovieFrame {

		//@property (readonly, nonatomic) KxMovieFrameType type;
		[Export("type")]
		KxMovieFrameType type { get;}

		//@property (readonly, nonatomic) CGFloat position;
		[Export("position")]
		float position { get;}

		//@property (readonly, nonatomic) CGFloat duration;
		[Export("duration")]
		float duration { get;}

	}


	[BaseType(typeof(KxMovieFrame))]
	interface KxAudioFrame {

		//@property (readonly, nonatomic, strong) NSData *samples;
		[Export("samples")]
		NSData samples { get;}

	}


	[BaseType(typeof(KxMovieFrame))]
	interface KxVideoFrame {

		//@property (readonly, nonatomic) KxVideoFrameFormat format;
		[Export("format")]
		KxVideoFrameFormat format { get;}

		//@property (readonly, nonatomic) NSUInteger width;
		[Export("width")]
		UInt32 width { get;}

		//@property (readonly, nonatomic) NSUInteger height;
		[Export("height")]
		UInt32 height { get;}

	}


	[BaseType(typeof(KxVideoFrame))]
	interface KxVideoFrameRGB {

		//@property (readonly, nonatomic) NSUInteger linesize;
		[Export("linesize")]
		UInt32 linesize { get;}

		//@property (readonly, nonatomic, strong) NSData *rgb;
		[Export("rgb")]
		NSData rgb { get;}

		//- (UIImage *) asImage;
		[Export("asImage")]
		UIImage asImage();
	}


	[BaseType(typeof(KxVideoFrame))]
	interface KxVideoFrameYUV {

		//@property (readonly, nonatomic, strong) NSData *luma;
		[Export("luma")]
		NSData luma { get;}

		//@property (readonly, nonatomic, strong) NSData *chromaB;
		[Export("chromaB")]
		NSData chromaB { get;}

		//@property (readonly, nonatomic, strong) NSData *chromaR;
		[Export("chromaR")]
		NSData chromaR { get;}

	}


	[BaseType(typeof(KxMovieFrame))]
	interface KxArtworkFrame {

		//@property (readonly, nonatomic, strong) NSData *picture;
		[Export("picture")]
		NSData picture { get;}

		//- (UIImage *) asImage;
		[Export("asImage")]
		UIImage asImage();
	}


	[BaseType(typeof(KxMovieFrame))]
	interface KxSubtitleFrame {

		//@property (readonly, nonatomic, strong) NSString *text;
		[Export("text")]
		string text { get;}

	}



	//typedef BOOL(^KxMovieDecoderInterruptCallback)();
	delegate bool KxMovieDecoderInterruptCallback ();

	[BaseType(typeof(NSObject))]
	interface KxMovieDecoder {

		//@property (readonly, nonatomic, strong) NSString *path;
		[Export("path")]
		string path { get;}

		//@property (readonly, nonatomic) BOOL isEOF;
		[Export("isEOF")]
		bool isEOF { get;}

		//@property (readwrite,nonatomic) CGFloat position;
		[Export("position")]
		float position { get; set; }

		//@property (readonly, nonatomic) CGFloat duration;
		[Export("duration")]
		float duration { get;}

		//@property (readonly, nonatomic) CGFloat fps;
		[Export("fps")]
		float fps { get;}

		//@property (readonly, nonatomic) CGFloat sampleRate;
		[Export("sampleRate")]
		float sampleRate { get;}

		//@property (readonly, nonatomic) NSUInteger frameWidth;
		[Export("frameWidth")]
		UInt32 frameWidth { get;}

		//@property (readonly, nonatomic) NSUInteger frameHeight;
		[Export("frameHeight")]
		UInt32 frameHeight { get;}

		//@property (readonly, nonatomic) NSUInteger audioStreamsCount;
		[Export("audioStreamsCount")]
		UInt32 audioStreamsCount { get;}

		//@property (readwrite,nonatomic) NSInteger selectedAudioStream;
		[Export("selectedAudioStream")]
		UInt32 selectedAudioStream { get; set; }

		//@property (readonly, nonatomic) NSUInteger subtitleStreamsCount;
		[Export("subtitleStreamsCount")]
		UInt32 subtitleStreamsCount { get;}

		//@property (readwrite,nonatomic) NSInteger selectedSubtitleStream;
		[Export("selectedSubtitleStream")]
		UInt32 selectedSubtitleStream { get; set; }

		//@property (readonly, nonatomic) BOOL validVideo;
		[Export("validVideo")]
		bool validVideo { get;}

		//@property (readonly, nonatomic) BOOL validAudio;
		[Export("validAudio")]
		bool validAudio { get;}

		//@property (readonly, nonatomic) BOOL validSubtitles;
		[Export("validSubtitles")]
		bool validSubtitles { get;}

		//@property (readonly, nonatomic, strong) NSDictionary *info;
		[Export("info")]
		NSDictionary info { get;}

		//@property (readonly, nonatomic, strong) NSString *videoStreamFormatName;
		[Export("videoStreamFormatName")]
		string videoStreamFormatName { get;}

		//@property (readonly, nonatomic) BOOL isNetwork;
		[Export("isNetwork")]
		bool isNetwork { get;}

		//@property (readonly, nonatomic) CGFloat startTime;
		[Export("startTime")]
		float startTime { get;}

		//@property (readwrite, nonatomic) BOOL disableDeinterlacing;
		[Export("disableDeinterlacing")]
		bool disableDeinterlacing { get; set; }

		//@property (readwrite, nonatomic, strong) KxMovieDecoderInterruptCallback interruptCallback;
		[Export("interruptCallback")]
		KxMovieDecoderInterruptCallback interruptCallback { get; set; }


		//+ (id) movieDecoderWithContentPath: (NSString *) path                             error: (NSError **) perror;
		[Static,Export("movieDecoderWithContentPath:error:")]
		KxMovieDecoder movieDecoderWithContentPath(string path ,out NSError perror);
	

		//- (BOOL) openFile: (NSString *) path            error: (NSError **) perror;
		[Export("openFile:error:")]
		bool openFile(string path ,out NSError perror);

		//-(void) closeFile;
		[Export("closeFile")]
		void closeFile();
		//- (BOOL) setupVideoFrameFormat: (KxVideoFrameFormat) format;
		[Export("setupVideoFrameFormat:")]
		bool setupVideoFrameFormat(KxVideoFrameFormat format );

		//- (NSArray *) decodeFrames: (CGFloat) minDuration;
		[Export("decodeFrames:")]
		NSArray decodeFrames(float minDuration );

	}


	[BaseType(typeof(NSObject))]
	interface KxMovieSubtitleASSParser {

		//+ (NSArray *) parseEvents: (NSString *) events;
		[Static,Export("parseEvents:")]
		NSArray parseEvents(string events );

		//+ (NSArray *) parseDialogue: (NSString *) dialogue                  numFields: (NSUInteger) numFields;
		[Static,Export("parseDialogue:numFields:")]
		NSArray parseDialogue(string dialogue ,UInt32 numFields);

		//+ (NSString *) removeCommandsFromEventText: (NSString *) text;
		[Static,Export("removeCommandsFromEventText:")]
		string removeCommandsFromEventText(string text );

	}


	[BaseType(typeof(UIView))]
	interface KxMovieGLView {

		//- (id) initWithFrame:(CGRect)frame             decoder: (KxMovieDecoder *) decoder;
		[Export("initWithFrame:decoder:")]
		IntPtr Constructor(RectangleF frame ,KxMovieDecoder decoder);

		//- (void) render: (KxVideoFrame *) frame;
		[Export("render:")]
		void render(KxVideoFrame frame );

	}


}

