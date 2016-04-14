using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.IO;
using Tuto.Model;
using System.ComponentModel;
using System.Threading;
using System.Text.RegularExpressions;

namespace Tuto.BatchWorks
{
    public abstract class CompositeWork : BatchWork
    {
        public List<BatchWork> Tasks;
        public CompositeWork()
        {
            Tasks = new List<BatchWork>();
        }
    }
}
