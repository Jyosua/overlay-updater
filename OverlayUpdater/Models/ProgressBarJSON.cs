using System;
using System.Collections.Generic;
using System.Text;

namespace OverlayUpdater.Models
{
    public class ProgressBarJSON
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public uint Max { get; set; }
        public uint Current { get; set; }
    }
}
