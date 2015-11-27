﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Threading;
using Tuto.Publishing;
using Tuto.Publishing.Youtube;

namespace Tuto.BatchWorks
{
    public class YoutubeWork : BatchWork
    {
        EditorModel editorModel;
        int episodeNumber;
        EpisodInfo episode;
        FileInfo pathToFile;


        public YoutubeWork(EditorModel model, int number, FileInfo path)
        {
            this.editorModel = model;
            this.episodeNumber = number;
            episode = model.Montage.Information.Episodes[number];
            this.pathToFile = path;
            Name = "Uploading: " + episode.Name;
        }


        public override void Work()
        {
            if (episode.YoutubeId!=null) YoutubeApisProcessor.Current.DeleteVideo(episode.YoutubeId);
            episode.YoutubeId = YoutubeApisProcessor.Current.UploadVideo(pathToFile, episode.Name, episode.Guid);
			editorModel.Save();
        }

        
        public override void Clean()
        {
            FinishProcess();
        }
    }
}
