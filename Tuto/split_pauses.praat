# Open file, generate voice and silence parts index, save
#
# example usage:
# 		praatcon.exe test.praat in.mp3 out.textgrid -- ++ 100 0 -27 0.5 0.1 
#
# parameters with examples:
#		filename = "in.mp3"  ; sound to analyze
#		output_filename = "out.textgrid"  ; file to write results into
#		silent_label = "--"  ; marks inside the file
#		sound_label = "++"
# parameters for intensity analysis
#		min_pitch = 100  ; (Hz)
#		time_step = 0  ; (sec) 0 is auto
# parameters for silent intervals detection
#		silence_threshold = -27  ; (dB)
#		min_silent_interval = 0.5  ; (sec)
#		min_sound_interval = 0.1  ; (sec)
#

form arguments here
	# filenames, labels
	text filename
	text output_filename
	text silent_label
	text sound_label
	
	# parameters for intensity analysis
	real min_pitch
	real time_step
	
	# parameters for silent intervals detection
	real silence_threshold
	real min_silent_interval
	real min_sound_interval
	
endform

Read from file: filename$

# do actual work
snd = selected("Sound")
selectObject: snd
To TextGrid (silences): min_pitch, time_step, silence_threshold,
... min_silent_interval, min_sound_interval, silent_label$, sound_label$

Write to short text file: output_filename$
# Write to text file: "long.TextGrid"  # writes more info to file
# cleanup
select all
Remove