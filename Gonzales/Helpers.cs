using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Windows;

namespace Diswerx.GeoSorting
{
    /// <summary>
    /// Help manage spatial queries (not spatial partitioned).
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Get the bounds of a polygon.
        /// </summary>
        /// <param name="polygon">Describe the 2d (euclidean) polygon, no need for closing point</param>
        /// <returns></returns>
        public static Rect GetBounds(List<Vector2> polygon)
        {
            var minX = polygon.Min(p => p.X);
            var minY = polygon.Min(p => p.Y);
            var maxX = polygon.Max(p => p.X);
            var maxY = polygon.Max(p => p.Y);

            return new Rect(new Point(minX, minY), new Size(maxX - minX, maxY - minY));
        }

        /// <summary>
        /// Test if a point is within a 2d (euclidean) polygon.
        /// </summary>
        /// <param name="polygon">The polygon</param>
        /// <param name="point">Query if this point is within</param>
        /// <returns></returns>
        public static bool IsPointInPolygon(List<Vector2> polygon, Vector2 point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) *
                (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
    }
}
