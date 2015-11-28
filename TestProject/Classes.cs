using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace TestProject
{
    class Program
    {
        public static void Main()
        {
            string test =
@"  {
		//#video CRQnC44OO_A

        }";
            var regexp = new Regex("//#video ([a-zA-Z0-9_]+)");
            Console.WriteLine(regexp.Replace(test, "//#video mystrings"));

        }
    }
}
