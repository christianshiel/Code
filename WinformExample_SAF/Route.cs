using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diswerx.Common.Coordinates;
using DotNetCoords;

namespace WinformExample_SAF
{
    public class Route
    {
        private List<GeoCoordinate> waypoints; public List<GeoCoordinate> Waypoints { get { return waypoints; } set { waypoints = value; } }
        private string id; public string Id { get { return id; } set { id = value; } }
        private double stopDistance = 0.1f; public double StopDistance { get { return stopDistance; } set { stopDistance = value; } }

        private int index = 0;

        public GeoCoordinate GetCurrent()
        {
            return waypoints[index];
        }

        public void MoveNext()
        {
            if (index >= waypoints.Count-1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }

        public void Update(GeoCoordinate pos)
        {
            LatLng a = new LatLng(pos.Northing, pos.Easting);
            LatLng b = new LatLng(GetCurrent().Northing, GetCurrent().Easting);

            double d = b.DistanceMiles(a);

            if (d <= stopDistance)
            {
                MoveNext();                
            }

            //Console.WriteLine(d.ToString());
        }

        public Route()
        {
            waypoints = new List<GeoCoordinate>();
        }
    }
}
