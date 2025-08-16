using Animocity.Cities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.CoreCode.Animocity.Cities
{
    public class DemolitionEventArgs
    {
        public HashSet<Building> buildingsThreatened;
        public DemolitionEventArgs() 
        { 
            buildingsThreatened = new HashSet<Building>();
        }
    }
}
