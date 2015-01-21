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
		public bool InheritedFromChildren { get; private set; }
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
        
        public static BlockStatus OK(string message = "", bool inherited=false) { return new BlockStatus { Status = Statuses.OK, Explanation = message, InheritedFromChildren=inherited}; }
		public static BlockStatus NA(string message = "", bool inherited = false) { return new BlockStatus { Status = Statuses.NotAvailable, Explanation = message, InheritedFromChildren = inherited }; }
		public static BlockStatus Error(string message = "", bool inherited = false) { return new BlockStatus { Status = Statuses.Error, Explanation = message, InheritedFromChildren = inherited }; }
		public static BlockStatus Warning(string message = "", bool inherited = false) { return new BlockStatus { Status = Statuses.Warning, Explanation = message, InheritedFromChildren = inherited }; }

    }
}
