function AddWatermarkPNG(clip c, string filename, int x, int y) {
	length = c.frameCount
	logo = ImageSource(filename, end=length, pixel_type="rgb32")
	mask = logo.showAlpha()
	return overlay(c, logo, x, y, mask=mask)
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
