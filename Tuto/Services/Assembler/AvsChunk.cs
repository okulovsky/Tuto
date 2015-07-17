using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using Tuto.Model;

namespace Tuto.TutoServices.Assembler
{
    public class AvsChunk : AvsNode
    {
        public StreamChunk Chunk { get; set; }
        public int ConvertToFps { get; set; }

        public override void SerializeToContext(AvsContext context)
        {
            id = context.Id;
            var src = Chunk.Mode == Mode.Face ? "face-converted.avi" : "desktop-converted.avi";
            
            context.AddData(String.Format(Format, Id, Chunk, ConvertToFps));
        }

        public override IList<AvsNode> ChildNodes
        {
            get { return new AvsNode[] {}; }
        }

        protected override string Format 
        {
            get 
            { 
                var type = Chunk.Mode.ToString().ToLower();
                var conv = "Time2Frame({0}, {1})";
                var pattern =  type == "face" ? "face.Trim({0}, {1})" : "audiodub(desktop.Trim({0}), face.Trim({1}).KillVideo())";
                var startTime = Chunk.StartTime;
                var endTime = Chunk.Length + Chunk.StartTime;
                if (type =="face")
                {
                    var res = string.Format(pattern, string.Format(conv,type, startTime), string.Format(conv, type, endTime));
                    return "{0} = " + res;
                }
                var descTime = string.Format("{0},{1}", string.Format(conv, type, startTime - SyncShift), string.Format(conv, type, endTime - SyncShift));
                var faceTime = string.Format("{0},{1}", string.Format(conv, "face", startTime), string.Format(conv, "face", endTime));
                var final = string.Format(pattern, descTime, faceTime);
                return "{0} = " + final; 
            }
        }
    }
}