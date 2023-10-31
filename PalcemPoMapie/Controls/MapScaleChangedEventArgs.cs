using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalcemPoMapie.Controls
{
    public class MapScaleChangedEventArgs : EventArgs
    {
        public MapScaleChangedEventArgs(double newScale, double oldScale)
        {
            NewScale = newScale;
            OldScale = oldScale;
            DeltaScale = newScale / oldScale;
        }

        public double DeltaScale { get; }

        public double NewScale { get; }

        public double OldScale { get; }

        public bool SupressImageScale { get; set; }

        public bool SupressContentScale { get; set; }
    }
}
