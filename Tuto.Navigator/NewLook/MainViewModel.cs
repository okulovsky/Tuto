using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Navigator.NewLook
{
    public class MainViewModel
    {
        public Videotheque Videotheque { get; private set; }

        public List<EditorViewModel> EditorViewModels { get; private set;}


        public MainViewModel(Videotheque v)
        {
            this.Videotheque = v;
            EditorViewModels = v.EditorModels
                .OrderByDescending(z=>z.Montage.ModificationTime)
                .Select(z => new EditorViewModel(z))
                .ToList();
        }
    }
}
