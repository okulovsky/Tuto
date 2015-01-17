using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tuto.Publishing
{
    public enum Statuses
    {
        NotAvailable,
        Error,
        Warning,
        OK
    }

    public class BlockStatus
    {
        public Statuses Status { get; private set; }
        public string Explanation { get; private set; }
        public Brush Brush
        {
            get
            {
                switch (Status)
                {
                    case Statuses.Error: return Brushes.Red;
                    case Statuses.OK: return Brushes.Green;
                    case Statuses.Warning: return Brushes.Yellow;
                    default: return Brushes.LightGray;
                }
            }
        }
        
        public static BlockStatus OK(string message = "") { return new BlockStatus { Status = Statuses.OK, Explanation = message }; }
        public static BlockStatus NA(string message = "") { return new BlockStatus { Status = Statuses.NotAvailable, Explanation = message }; }
        public static BlockStatus Error(string message = "") { return new BlockStatus { Status = Statuses.Error, Explanation = message }; }
        public static BlockStatus Warning(string message = "") { return new BlockStatus { Status = Statuses.Warning, Explanation = message }; }

    }
}
