using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public static class ChunkListExtensions 
    {
        public static int FindChunkIndex(this List<ChunkData> data, int ms)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var e = data[i];
                if (e.StartTime <= ms && (e.StartTime + e.Length) > ms) return i;
            }
            return -1;
        }

        static void ShiftLeftBorderToLeftInternal(this List<ChunkData> data, int chunkIndex, int delta)
        {
            if (delta < 0) throw new ArgumentException();

            var remain = delta;

            for (int index = chunkIndex - 1; index >= 0; index--)
            {
                if (remain == 0) break;
                if (data[index].Length < remain)
                {
                    remain -= data[index].Length;
                    data[index].Length = 0;
                }
                else
                {
                    data[index].Length -= remain;
                    remain = 0;
                    break;
                }
            }

            data[chunkIndex].StartTime -= delta-remain;
            data[chunkIndex].Length += delta-remain;
        }

        static void ShiftRightBorderToRightInternal(this List<ChunkData> data, int chunkIndex, int delta)
        {
            if (delta < 0) throw new ArgumentException();
            var remain = delta;
            var lastChanged=chunkIndex+1;
            for (; lastChanged < data.Count; lastChanged++)
            {
                if (remain == 0) break;
                if (data[lastChanged].Length < remain)
                {
                    remain -= data[lastChanged].Length;
                    data[lastChanged].Length = 0;
                }
                else
                {
                    data[lastChanged].Length -= remain;
                    remain = 0;
                    break;
                }
            }
            data[chunkIndex].Length += delta - remain;
            for (int i = chunkIndex + 1; i <= lastChanged; i++)
                data[lastChanged].StartTime = data[chunkIndex].EndTime;

        }


        public static void ShiftLeftBorderToRight(this List<ChunkData> data, int chunkIndex, int delta)
        {
            if (chunkIndex == 0) throw new Exception("This is leftmost chunk, cannot shift");
            if (delta > 0) ShiftLeftBorderToLeftInternal(data, chunkIndex, delta);
            else ShiftRightBorderToRightInternal(data, chunkIndex - 1, -delta);
        }

        public static void ShiftRightBorderToRight(this List<ChunkData> data, int chunkIndex, int delta)
        {
            if (chunkIndex == data.Count - 1) throw new Exception("This is rightmost chunk, cannot shift");
            if (delta > 0) ShiftRightBorderToRightInternal(data, chunkIndex, delta);
            else ShiftLeftBorderToLeftInternal(data, chunkIndex + 1, -delta);
        }

    }
}
