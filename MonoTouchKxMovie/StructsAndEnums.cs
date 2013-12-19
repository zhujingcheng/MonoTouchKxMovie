using System;

namespace MonoTouchKxMovie
{
	public enum kxMovieError { 
		kxMovieErrorNone,
		kxMovieErrorOpenFile,
		kxMovieErrorStreamInfoNotFound,
		kxMovieErrorStreamNotFound,
		kxMovieErrorCodecNotFound,
		kxMovieErrorOpenCodec,
		kxMovieErrorAllocateFrame,
		kxMovieErroSetupScaler,
		kxMovieErroReSampler,
		kxMovieErroUnsupported,
	}

	public enum KxMovieFrameType { 
		KxMovieFrameTypeAudio,
		KxMovieFrameTypeVideo,
		KxMovieFrameTypeArtwork,
		KxMovieFrameTypeSubtitle,
	}

	public enum KxVideoFrameFormat { 
		KxVideoFrameFormatRGB,
		KxVideoFrameFormatYUV,
	}
}

