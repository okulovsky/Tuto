ffmpeg -i face.mp4    -vf scale=1280:720 -r 30 -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k face-converted.avi
ffmpeg -i desktop.avi -vf scale=1280:720 -r 30 -qscale 0 -an desktop-converted.avi
