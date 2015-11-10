using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using Tuto.Model;

namespace Tuto.TutoServices.Assembler
{
    public class AvsPatchChunk : AvsNode
    {
        public double Start { get; set; }
        public double End { get; set; }
        public string Path { get; set; }
        public int ConvertToFps { get; set; }

        public void Load(string path, double start, double end)
        {
            Start = start;
            Path = path;
            End = end;
        }
        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            context.AddData(String.Format(Format, Id));
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new AvsNode[] {}; }
        }

        protected override string Format 
        {
            get 
            { 
                var clip = string.Format("DirectShowSource(\"{0}\")", Path);
                var frameRate = 25;
                var conv = "Time2Frame({0}, {1})";
                var startTime = string.Format(conv, clip, Start);
                var endTime = string.Format(conv, clip, End);
                var final = string.Format("AddEmptySoundIfNecessary(DirectShowSource(\"{0}\").Trim({1},{2}))", Path, (int)(Start * frameRate), (int)(End * frameRate));
                return "{0} = " + final; 
            }
        }
    }
}