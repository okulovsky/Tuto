function AddWatermarkPNG(clip c, string filename, int x, int y) {
	length = c.frameCount
	logo = ImageSource(filename, end=length, pixel_type="rgb32")
	mask = logo.showAlpha()
	return overlay(c, logo, x, y, mask=mask)
}

function AddEmptySoundIfNecessary(clip c) {
	ans = c.HasAudio() ? c : eval("""audio = BlankClip(c, audio_rate=48000) AudioDub(c, audio)""")
	return ans
}

function Intro(clip reference, string image, float ms) {
	length = Time2Frame(reference, ms)
	fps = reference.frameRate
	logo = ImageSource(image, end=length-1, fps=fps, pixel_type="rgb24")  # no alpha!
	
	# add silent audio stream
	silence = KillVideo(BlankClip(audio_rate=44100, channels=1))
	mixed = AudioDub(logo, silence)
	trimmed = Trim(mixed, 0, length)
	assert(trimmed.frameCount == length, "Invalid length: "+string(trimmed.frameCount)+" != "+string(length))
	return trimmed
}

function FadeInTime(clip c, float ms) {
	fadeIn(c, Time2Frame(c, ms))
}

function FadeOutTime(clip c, float ms) {
	fadeOut(c, Time2Frame(c, ms))
}

function CrossFadeTime(clip video, clip prev, float ms) {
	assert(round(prev.frameRate) == round(video.frameRate), "Clips must have same framerates: "+string(prev.frameRate)+" != "+string(video.frameRate))
	
	frameCount = Time2Frame(prev, ms)
	prevLastFrame = GetLastFrame(prev)
	prevLastLoop = loop(prevLastFrame, frameCount)
	
	mixedFragment = Dissolve(prevLastLoop, killAudio(video), frameCount)
	assert(mixedFragment.frameCount == video.frameCount, "Invalid mixed fragment length")
	
	mixedVideo = AudioDub(mixedFragment, KillVideo(video))
	assert(mixedVideo.frameCount == video.frameCount, "Invalid mixed video length")
	
	return mixedVideo
}


function GetLastFrame(clip c) {
	lastIndex = c.frameCount - 1
	lastFrame = Trim(c, lastIndex, 1)  # Trim() doesnt support negative start argument
	
	return killAudio(lastFrame)
}

/*
	Convert milliseconds to frames
	
	negative time specifies offset from the end of clip
	clip argument is essential to get framerate
	
	returns: int
*/
function Time2Frame(clip c, float ms) {
	f_ms = float(ms)  # well, that's a hack
	fps = c.frameRate  # float
	return round(f_ms/1000*fps)
}

function AssumeSameAVLength(clip c) {
	# can not just trim() clip to make its audio and video have same duration.
	# it works if audio is longer than video
	# in other case, should mix audio with silent stream to fill clip duration
	audio = c.killVideo()
	video = c.killAudio()
	aRate = audio.audioRate
	aChannels = audio.audioChannels
	vFrames = c.frameCount
	vFps = c.FrameRate
	
	#this creates black video with silent audio, both with correct duration.
	silence = blankClip(audio_rate=aRate, channels=aChannels, fps=vFps, length=vFrames)
	AssertLength(silence)
	
	silence = silence.killVideo()
	mixed = MixAudio(silence, audio)  # silence should be first arg!
	assert(mixed.audioLength == silence.audioLength)
	
	v = audioDub(video, mixed)
	
	AssertLength(v)
	
	return v
}

function AssertLength(clip v) {
	assert(v.audioLengthF/v.audioRate == v.framecount/v.framerate, \
	"audio length/rate: "+string(v.audioLengthF) + " / " + string(v.audioRate) \
	+ " = " + string(v.audioLengthF/v.audioRate) \
	+ " <> video framecount/framerate "+string(v.framecount) + " / " + string(v.framerate) \
	+ " = " + string(v.framecount/v.framerate))
}