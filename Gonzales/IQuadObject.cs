using System;
using System.Windows;

namespace Diswerx.GeoSorting
{
    public interface IQuadObject
    {
        Rect Bounds { get; }
        event EventHandler BoundsChanged;
    }
}