
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
        private int fixedFps = 25;

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

        private int Time2Frame(double time)
        {
            return (int)(time / 1000 * fixedFps);
        }

        protected override string Format 
        {
            get 
            { 
                var type = Chunk.Mode.ToString().ToLower();
                var pattern =  type == "face" ? "face.Trim({0}, {1})" : "audiodub(desktop.Trim({0}), face.Trim({1}).KillVideo())";
                var startTime = Chunk.StartTime;
                var endTime = Chunk.Length + Chunk.StartTime;
                if (type =="face")
                {
                    var res = string.Format(pattern, Time2Frame(startTime), Time2Frame(endTime) - 1);
                    return "{0} = " + res;
                }
                var descTime = string.Format("{0},{1}", Time2Frame(startTime - SyncShift), Time2Frame(endTime - SyncShift) - 1);
                var faceTime = string.Format("{0},{1}", Time2Frame(startTime), Time2Frame(endTime) - 1);
                var final = string.Format(pattern, descTime, faceTime);
                return "{0} = " + final; 
            }
        }
    }
}