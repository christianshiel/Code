using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoSorting;

namespace GeoSorting
{
    public interface IGeoObject
    {
        string Name { get; set; }
        GeoCoordinate GeoCoordinate { get; set; }
        GeoArea GeoArea { get; set; }
    }
}
