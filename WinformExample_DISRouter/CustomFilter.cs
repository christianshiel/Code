using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diswerx;
using OpenDis.Dis1998;
using System.Drawing;
using Diswerx.GeoSorting;
using Diswerx.Common;
using Diswerx.Common.Coordinates;

namespace WinformExample_DISRouter
{
    public class CustomFilter : DisFilter
    {
        private List<Pdu> pdus;

        private PointD[] poly;

        // For this example our custom filter will filter out anything inside a polygon
        public CustomFilter(List<PointD> poly)
        {
            this.poly = poly.ToArray();            
        }

        public override List<Pdu> FilterOut()
        {
            List<Pdu> result = new List<Pdu>();

            foreach (Pdu pdu in pdus)
            {
                if (!PassesFilter(pdu))
                {
                    result.Add(pdu);
                }
            }

            return result;
        }

        public override List<Pdu> FilterIn()
        {
            List<Pdu> result = new List<Pdu>();

            foreach (Pdu pdu in pdus)
            {
                if (PassesFilter(pdu))
                {
                    result.Add(pdu);
                }
            }

            return result;
        }

        public override void SetObservableEntities(List<Pdu> pdus)
        {
            this.pdus = pdus;
        }

        public override bool PassesFilter(Pdu pdu)
        {
            // Only allow filter on entity state for example
            if (DisHelpers.GetPduType(pdu.PduType) != OpenDis.Enumerations.PduType.EntityState)
                return false;

            EntityStatePdu esPdu = pdu as EntityStatePdu;

            PointD pos = GeoHelpers.ToPointDLatLng(esPdu.EntityLocation.X, esPdu.EntityLocation.Y, esPdu.EntityLocation.Z);

            if (GeoHelpers.IsPointInPolygon(poly, pos))
            {
                return true;
            }

            return false;
        }
    }
}
