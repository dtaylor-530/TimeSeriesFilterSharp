using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter.Utility
{
    public static class PathHelper
    {

        public static string GetParentPathName()
        {

            return Directory.GetParent(
                     Directory.GetCurrentDirectory()).Parent.FullName;
        }
    }
}
