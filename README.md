Tuto
====

Tuto editor is a software which I used to create lots of educational videos. 
This documentation is written 4 years after the last of the videos was created, when I wanted to revive this software to produce a videoblog. So the documentation is fragmentary.
The main point of this software is that it does not allow to do much, but what it does, it does extremely fast.

Features:
* Two video streams, one for a face video and one for a screencap
* The videopair is separated into "chunks", and each chunk can be assigned either taken to the face video, or to the screencap, or dropped.
* The aforementioned assignment is done on the keyboard entirely, and is done "on the fly": you watch videos and assign as it goes. 
* One videopair can produce several "episodes" of the video, so you can record several records in one sessions.
* The sound is improved by `sox`, a third-party open-source application. 
* It might be possible to add subtitles and additional videos to the result, but I never fully tested this features.
* There are several other apps which purpose is obscure to me, they are somehow related to publishing videos on YouTube and bringing some order to their names. It only makes sence if you have dozens of videos.

Advantages:
* the time you need to process the video is 1.5-2 times higher, than the duration of the video. I was told by experienced people that it is much faster than you can achive with the professional tools. 
* kinda proven in production, I produced around 120 hours of video with this tool

The disadvantages are:
* You really need to practice before you can get to this performance.
* The application was not maintained in any way and now, when I'm running it 4 years later, I'm contantly facing some exceptions. It might be resolved with time.
* This is not, and was never intended to be, a user-friendly or ready-to-use app. The functionality is very strict and basically only covers what I needed for my courses. By external standards it can probably be considered as "proof-of-concept".
* The application only maintains "happy paths", when a user does what instructed. The application does not meet the user errors friendly and most often just dies with an exception.

The development is stalled and I'm not planning to return to this project. 

However, I'm still find the idea of this application very attractive. So if someone will choose to improve the project, feel free to do it: hereby I declare the application to be under MIT licence:

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.[3]



Installation
------------

You need to check out the repository, open it with Visual Studio, build and run Tuto.Navigator.

At the startup, we managed to write a nice configuration wizard that will instruct you which third-party software to install and where to get it. 
Follow the instructions very carefully!

In the end, it will allow you to create a videotheque. 
Videotheque is a folder that (by default) contains all the files linked to the project. Choose the first option, create a folder for videotheque somewhere, and write an arbitrary file name. 
The following folders will be created in videotheque:

* `RAW`: this is where you will place your recordings. Each recordings's files should be placed in the individual folder (maybe inside subfolders) and have to have the names `desktop.mp4` and `face.avi` (regardless of the actual formats of these files). E.g. `RAW/FirstLecture/Episode1/desktop.mp4` and `RAW/FirstLecture/Episode1/face.avi`
* `OUTPUT`: this is where your lectures will be produced
* `MODEL`: this is where your montage is located. 
* `TEMP`: for intermediate files. Feel free to clear this directory at any time.

If you want to use version control on your Tuto-files, there is a way to locate all folders except `MODEL` somewhere, so you only will have small text files under version control.

Workflow
--------

First, you need to create files 



