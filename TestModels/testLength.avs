import("C:\Tuto\Tuto.Navigator\bin\Debug\library.avs")
loadplugin("C:\Tuto\Tuto.Navigator\bin\Debug\autolevels_0.6_20110109.dll")

function MakeTestClip(int vLen, int aLen) {
	bars = colorbars(640, 480).assumefps(25).convertToYV12()
	tbars = info(bars.trim(0,vLen-1).killAudio())
	assert(tbars.framecount == vlen)
	wave = Tone(channels=1, samplerate=44100, type="sine", level=0.4)
	twave = audiodub(blankClip(), wave).trim(0, alen).killVideo()
	
	v = AudioDub(tbars, twave)
	return v
}
function ver() {
	return version().assumefps(25).audiodub(BlankClip()).bilinearResize(640,480).convertToYV12().trim(0,299)
}

# testing lengths
# video < audio
v1 = MakeTestClip(150, 200)
v2 = AssumeSameAVLength(v1)

#video > audio
v3 = MakeTestClip(100, 50)
v4 = AssumeSameAVLength(v3)

# note that files saved after this conversion have WRONG length in random way
# use when concatenating large amount of clips


#AssertLength(v1) #fails
AssertLength(v2)
#AssertLength(v3) #fails
AssertLength(v4)

#return v1+ver+v1+ver  # outputs desynchronized video
return v2+ver+v2+ver  # its OK
#return v1+ver+v1+ver  # outputs desynchronized video
#return v2+ver+v2+ver  # its OK
