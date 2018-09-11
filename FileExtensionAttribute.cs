using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceOutputStreaming
{
    public class FileExtensionAttribute : Attribute
    {
        public string Extension { get; }

        public FileExtensionAttribute(string ext)
        {
            Extension = ext;
        }
    }
}
