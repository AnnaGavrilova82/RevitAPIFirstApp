using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIFirstApp
{
    class SortRoomByVolume : IComparer<Room>
    {
        public int Compare(Room x, Room y)
        {
            if (x.Volume > y.Volume)
                return 1;
            else if (x.Volume < y.Volume)
                return -1;
            else return 0;
        }
    }
}
