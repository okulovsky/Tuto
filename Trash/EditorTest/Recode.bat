ffmpeg -i %1\face.mp4    -vf scale=1280:720 -r 30 -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k %1\face-converted.avi
ffmpeg -i %1\desktop.avi -vf scale=1280:720 -r 30 -qscale 0 -an %1\desktop-converted.avi
