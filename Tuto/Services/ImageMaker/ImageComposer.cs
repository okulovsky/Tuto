using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.TutoServices
{
    public abstract class ImageComposer
    {
        public abstract Bitmap Compose(GlobalData globalData, Guid itemGuid);

    }
}
