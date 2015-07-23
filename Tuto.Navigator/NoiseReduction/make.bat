C:\ffmpeg\bin\ffmpeg -i voice.mp3 input.wav
C:\sox\sox input.wav loud.wav --norm

if not exist noise (
C:\ffmpeg\bin\ffmpeg -i loud.wav -ss 0 -t 3 -y sample.wav
gnp sample.wav noise
del sample.wav
)

nr loud.wav noise result.wav
C:\ffmpeg\bin\ffmpeg -i result.wav cleared.mp3
del result.wav
del loud.wav
del input.wav