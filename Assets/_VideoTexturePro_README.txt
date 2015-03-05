Video Texture Pro 2 for Unity Pro
Copyright 2011-2014 Brian Chasalow & Anton Marini
AVFoundation-based video player for OSX and iOS for fast video texture playback in Unity.
Questions/Bugs/donations? brian@chasalow.com

Note: if you need alpha channel support for ProRes4444 movies (only available on OSX), call VTP.useFastPathTextureCopying = false; Otherwise, leave it as true- as it is way more efficient.

------------------------------------------------------------------------------------------------------------------------
CHANGELOG: 
------------------------------------------------------------------------------------------------------------------------ 
2.1.9 : public requests: added a'destroyed' bool on destroy to avoid issues with fast destruction, added ipod-library:// prefix for iOS, changed enums to better support Oculus, fixed OnDestroy order of operations bug with internal VTP.removeInstance() call
2.1.8 : removed calls to UnityRenderEvent on iOS to avoid conflicts with android plugins, etc
2.1.7 : fixed black flash bug in new fastpath backingType.
2.1.6 : added limited support for no-texture-copy video decoding _only_ on iOS, using CreateExternalTexture/UpdateExternalTexture.
	 	To enable, set the VideoTexture.backingType to VideoTextureBackingType.IOSONLY_FASTPATH_RGBA (from debug inspector, or via code)
		in this instance, it uses .NativeTexture instead of .RenderTex.
		There are caveats: your texture will be upside down, if you use this mode, and you need to handle its rotation yourself.
		if you try to use this mode on OSX, it will use the standard texture decoding mechanism in the editor, where your texture will be right side up.
		Note: if you use IOSONLY_FASTPATH_RGBA, VideoTextureLoaded() will tell you when the .NativeTexture is ready to use.
2.1.5 : added support for ALAssetLibrary paths. Added support for VideoRotationAngle- 
		if you record a video in the iOS camera app in portrait and play it back, its data may be written 16:9 instead of 9:16,
		but it will tell you its rotation angle is 90. Added the VideoRotationAngle int property to the VideoTextureLoaded callback.		
2.1.4 : broke the iOS library in that last build -- fixed.
2.1.3 : remove extraneous comments from library
2.1.2 : kill a warning that is safe to ignore
2.1.1 : Minor edge case bugfix re: volume control on iOS, additional checkIfVTPSupported() edge cases, and added a script to cleanup on recompile.
2.1.0 : Major update: uses texture cache behind the scenes for faster rendering. 
		Supports 64 bit builds on OSX.
        Now supports volume control on iOS.
        Now PlaneScaler shouldn't cause redefinition issues when using Syphon
        VTP.checkIfVTPSupported() added- you may use this method to use a different movie rendering method if VTP is not supported.
         
2.0.5 : Forced more precision on video time seeks. Also, now supports Unity 4.3.
2.0.4 : Fixed an issue with internal function name mangling causing issues with enterprise builds.
2.0.3 : Now if headphones are unplugged, and video was playing, it will continue to play.
2.0.2 : Issue fixed where on plugin start, the video would always play no matter what.
2.0.1 : Plays nicely with Syphon plugin.
2.0.0 : First release of AVFoundation based player.

------------------------------------------------------------------------------------------------------------------------
USAGE:
------------------------------------------------------------------------------------------------------------------------
1) Put the VideoTexture script on a GameObject. 
2) use the GUI to select a video path for an individual movie, or load an entire movie folder.
3) If you want to embed a movie inside your app for deployment, create a StreamingAssets folder, rename your movies to the .m4v extension, and put them in that folder.
They should automatically get copied into your Xcode project and 'just work.' On iOS, you may need to add libc++.dylib to your Xcode build phases linker step.
4) As of VTP 2.1, VTP should automatically add a VideoTextureCacheContext script to your Main Camera on Play. if you are doing fancy camera things and Unity can't find your camera, you may need to add this script yourself to the camera.



------------------------------------------------------------------------------------------------------------------------
VideoTexture instance properties/fields:
------------------------------------------------------------------------------------------------------------------------
bool IsVideoLoaded 
get: if(myVideoTexture.IsVideoLoaded)
set: read-only.

VideoLoopType LoopType
// VideoLoopType enum types available:
//VideoLoopType.LOOP_QUEUE, VideoLoopType.PLAY_QUEUE_AND_STOP, VideoLoopType.LOOP_VIDEO, VideoLoopType.PLAY_VIDEO_AND_STOP
get: if(myVideoTexture.LoopType == VideoLoopType.LOOP_QUEUE)
set: myVideoTexture.LoopType = VideoLoopType.LOOP_QUEUE;

bool ErrorVideoLoading
//TODO: better error handling - this method reserved for future implementation.

float VideoSpeed
get: if(myVideoTexture.VideoSpeed == 1.0f)
set: myVideoTexture.VideoSpeed = 2.0f;

bool IsPaused
get: return myVideoTexture.IsPaused;
set: myVideoTexture.IsPaused = true;

float VideoTime
//returns and can set video time in seconds.
get: if(myVideoTexture.VideoTime > 10.0f)
set: myVideoTexture.VideoTime = 0.0f;

float VideoVolume
get: if(myVideoTexture.VideoVolume == 1.0f)
set: myVideoTexture.VideoVolume = 0.0f;

string[] videoPaths
//array of strings representing the videos you would like to load.
get: return videoPaths[CurrentlyPlayingIndex];
set: myVideoTexture.videoPaths[0] = "/Users/myComputer/myMovie.mov";

int CurrentlyPlayingIndex
//the index into the videoPaths array of the currently playing movie
get: return myVideoTexture.CurrentlyPlayingIndex;
set: read-only.

RenderTexture RenderTex
//the video texture
get: return myVideoTexture.RenderTex;
set: read-only. 

float VideoDuration
//the length of the movie, in seconds
get: return myVideoTexture.VideoDuration;
set: read-only

int VideoTextureWidth
//width of the encoded texture size
get: return myVideoTexture.VideoTextureWidth;
set: read-only

int VideoTextureHeight
//height of the encoded texture size
get: return myVideoTexture.VideoTextureHeight;
set: read-only

PLEASE NOTE: videos can have a different internal texture size than their aspect ratio size.
if calculating aspect ratio for display purposes,
use VideoAspectWidth and VideoAspectHeight instead of VideoTextureWidth and VideoTextureHeight.

int VideoAspectWidth
//w of the 'natural' size of the movie, for display
get: return myVideoTexture.VideoAspectWidth;
set: read-only

int VideoAspectHeight
//h of the 'natural' size of the movie, for display
get: return myVideoTexture.VideoAspectHeight;
set: read-only

int VideoRotationAngle
//if you record a video in the iOS camera app in portrait and play it back, its data may be written 16:9 instead of 9:16, but it will tell you its rotation angle is 90.
get: return myVideoTexture.VideoRotationAngle;
set: read-only
//will return one of these values:
//Portrait: 90
//PortraitUpsideDown: 270
//LandscapeLeft (rotation angle not present): 0
//LandscapeRight: 180

Texture2D NativeTexture
// only exists on iOS when backingType == VideoTextureBackingType.IOSONLY_FASTPATH_RGBA
// VideoTextureLoaded() will fire when this is valid.  RenderTex is not used in this mode.
get: myVideoTexture.NativeTexture
set: read-only

------------------------------------------------------------------------------------------------------------------------
VideoTexture.cs methods:
------------------------------------------------------------------------------------------------------------------------
public void load();
if you rearrange movies in the videoPaths array, you will have to call myVideoTexture.load() again.
//TODO: make it so that queuelist modification/reordering can happen on the fly without needing to call this method

public void jumpToVideo(int i);
this lets you jump to a particular movie index in the videoPaths array.

public int setVideoPathsToDir(string myDirectory);
this will set the videoPaths to a particular directory on disk. takes a string in the form: "/Users/bob/Desktop/movies/"
you may choose to read from an xml file and set the video paths on Awake(), using this method.


------------------------------------------------------------------------------------------------------------------------
CALLBACKS:  (see PlaneScaler.cs for an example)
------------------------------------------------------------------------------------------------------------------------
public void UpdateAspectRatio(Vector2 texWidthHeight);
If your video texture resizes, any script on the same object as your VideoTexture script gets the notification
that the video has been resized, and provides the new VideoAspectWidth and VideoAspectHeight so that you may handle adjusting aspect ratio.

void VideoTextureLoaded(int rotationAngle);
When a video successfully loads or the next movie in a queue starts, this callback is triggered on any script on the same gameObject as your VideoTexture.
if you record a video in the iOS camera app in portrait and play it back, its data may be written 16:9 instead of 9:16, but it will tell you its rotation angle is 90.
on video load, this method will tell you this, so you can immediately handle the case yourself.
NOTE: if  VideoTexture.backingType is set to VideoTextureBackingType.IOSONLY_FASTPATH_RGBA (from debug inspector, or via code), this callback will fire when its .NativeTexture property is valid and filled with valid textures. in this instance,
.RenderTex is not used.

void VideoPlaybackEnded();
If a video has the loopType VideoLoopType.PLAY_QUEUE_AND_STOP, PLAY_VIDEO_AND_STOP, or LOOP_VIDEO, and the video stops, this callback gets triggered.
if the loopType is PLAY_QUEUE_AND_STOP or PLAY_VIDEO_AND_STOP, to continue, you are then required to do one of the following:
myVideoTexture.IsPaused = false;  //continues to the next movie in the queue
myVideoTexture.jumpToVideo(int i); //jumps to another movie in the queue


------------------------------------------------------------------------------------------------------------------------
VTP global methods: (VTP2.1 and later)
------------------------------------------------------------------------------------------------------------------------
VTP.checkIfVTPSupported(); 
use this method to use a different movie rendering method if VTP is not supported.


------------------------------------------------------------------------------------------------------------------------
COMMON QUESTIONS:
------------------------------------------------------------------------------------------------------------------------
Q: Does it work on Android/Linux/Windows/OSX 10.7/ios 5?
A: No. Due to AVFoundation requirements, you need OSX 10.8+ or ios 6+. see VTP.checkIfVTPSupported()

Q: This is cool, why is this free?
A: Good video playback is a feature that any modern game engine _should_ have. By making this free, hopefully Unity is incentivized to add such support directly into the engine. 
   Good video support in Unity is very popular (see my other Unity video plugin, Syphon for Unity)
   Also, I rely on the generosity of strangers - feel free to send $ via PayPal at brian@chasalow.com

Q: I want to license the source code to add _x_ feature.
A: Shoot me an email.
