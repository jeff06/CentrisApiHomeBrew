using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew.Utils
{
    public class StringManipulation
    {
        public static string ReplaceEncodingString(string content)
        {
            content = content.Replace("&#xE0;", "à");
            content = content.Replace("&#xE8;", "è");
            content = content.Replace("&#xA0;", " ");
            content = content.Replace("&amp;", "&");

            return content;
        }
    }
}
