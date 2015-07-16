@echo off
ffmpeg -i %1 -y temp.mp3
ffmpeg -i temp.mp3 -ss 0 -t 1 -y sample.mp3
sox sample.mp3 -n noiseprof noise.prof
sox face.mp3 cleaned.mp3 noisered noise.prof %3
ffmpeg -i cleaned.mp3 -i %1 -vcodec copy -acodec copy -map 0:0 -map 1:0 -shortest -y %2

::usage rnoise.bat input output [intensity]