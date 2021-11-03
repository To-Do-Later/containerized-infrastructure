using System;
using System.Collections.Generic;
using System.Text;

namespace Build.Models
{
    public class NetCoreProject
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsPackable { get; set; }
        public bool IsTestable { get; set; }
        public bool IsPublishable { get; set; }
        public string Version { get; set; }

        public NetCoreProject()
        {

        }
    }
}
