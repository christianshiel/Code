using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDis.Dis1998;
using Diswerx;
using Diswerx.Surface2D;
using Diswerx.Common;
using System.Drawing;
using System.ComponentModel;
using Diswerx.Common.Coordinates;
using Diswerx.GeoSorting;
using OpenDis.Enumerations.EntityState;

namespace WinformExample_SAF
{           
    public enum EntityType
    {
        Air = 0,
        Tank = 1,
        Infantry = 2,
    }

    public enum Side
    {
        Blue = 0,
        Red = 1,
    }

    public enum Order
    {
        Idle = 0,
        Patrol_And_Destroy = 1,
    }

    [Browsable(true)]
    public class MovingEntity: SpatialEntity
    {
        private Side side; public Side Side { get { return side; } set { side = value; } }
        private Order order; public Order Order { get { return order; } set { order = value; } }
        private EntityID disId; public EntityID DisId { get { return disId; } set { disId = value; } }
        private EntityType entityType = EntityType.Air; public EntityType EntityType { get { return entityType; } set { entityType = value; } }

        private SubSurface subsurface;
        private SpatialFilterEngine filterEngine;

        private EntityStatePdu espdu;
        private FirePdu firepdu;
        private DetonationPdu detpdu;
        private ArticulationParameter ap;

        private List<Pdu> toSend;

        private string entityId; public string EntityId { get { return entityId; } set { entityId = value; } }
        private GeoCoordinate location; public GeoCoordinate Location { get { return location; } set { location = value; } }
        private PointD screenLocation;
        private float speed = 0.0005f; public float Speed { get { return speed; } set { speed = value; } }
        private float bbSize = 0.01f;
        private float headingX = 1f; public float HeadingX { get { return headingX; } set { headingX = value; } }
        private float headingY = 0f; public float HeadingY { get { return headingY; } set { headingY = value; } }

        private float phi, psi, theta;

        private Vector3Float velocity = new Vector3Float();
        private float lastTime = 0f;

        private Route route; public Route Route { get { return route; } set { route = value; } }

        //private static ushort disIndexId = 0;

        public MovingEntity(SubSurface subsurface, 
                            SpatialFilterEngine filterEngine, 
                            Bitmap entityIcon, 
                            System.Drawing.Point point,
                            Side side,
                            string id, ushort entityCount)
        {
            this.entityId = id;
            this.subsurface = subsurface;
            this.filterEngine = filterEngine;
            this.side = side;

            espdu = new EntityStatePdu();
            firepdu = new FirePdu();
            detpdu = new DetonationPdu();
            ap = new ArticulationParameter();
            espdu.ArticulationParameters.Add(ap);

            //espdu.Marking = DisHelpers.SetMarking(this.entityId);
            //espdu.EntityID = new EntityID() { Application = 0, Entity = disIndexId++, Site = 0 };

            this.disId = new EntityID() { Application = 0, Entity = entityCount, Site = 0 };

            byte b = byte.Parse(((int)entityType).ToString());
            espdu.EntityType = new OpenDis.Dis1998.EntityType() { Category = b };

            GeoCoordinate coord = subsurface.GetWorldCoordinate(point);
            this.location = coord;
            Update(location.Easting, location.Northing, bbSize, bbSize);

            PointD p = subsurface.GetScreenCoordinate(coord);

            Color sideColour = Color.White;

            if (side == WinformExample_SAF.Side.Blue)
                sideColour = Color.LightBlue;
            else if (side == WinformExample_SAF.Side.Red)
                sideColour = Color.IndianRed;

            subsurface.DrawIcon(entityIcon, sideColour, id, p.X, p.Y);

            screenLocation = new PointD(p.X, p.Y);

            toSend = new List<Pdu>();
        }

        private float GetThetaTo(GeoCoordinate target)
        {
            PointD pt1 = subsurface.GetScreenCoordinate(target);
            PointD pt2 = subsurface.GetScreenCoordinate(location);
            double x = (pt2.X - pt1.X);
            double y = (pt2.Y - pt1.Y);
            double a = Math.Atan2(y, x);
            return (float)(a * 57.2957795) - 90f;
        }

        public void MoveTo(GeoCoordinate target)
        {
            PointD pt1 = subsurface.GetScreenCoordinate(target);
            PointD pt2 = subsurface.GetScreenCoordinate(location);

            double x = (pt2.X - pt1.X);
            double y = (pt2.Y - pt1.Y);

            double len = Math.Sqrt((x * x) + (y * y));

            if (len > 0)
            {
                HeadingX = (float)(x / len);
                HeadingY = (float)(y / len);
            }

            // -/+ depends on world quadrant
            location.Easting -= HeadingX * speed;
            location.Northing += HeadingY * speed;

            // Update the velocity ???
            velocity.X = (HeadingX * speed) / lastTime;
            velocity.Z = (HeadingY * speed) / lastTime;

            theta = GetThetaTo(target);

            // Updates the Spatial Filter Engine
            Update(location.Easting, location.Northing, bbSize, bbSize);

            subsurface.TranslateDrawing(entityId, pt2.X, pt2.Y);
        }

        private bool HostilesNearby(ref List<MovingEntity> mes)
        {
            System.Windows.Rect r = new System.Windows.Rect(location.Easting, location.Northing, 0.1, 0.1);
            List<IQuadObject> objs = filterEngine.FindObjects(r);

            if (objs.Count > 0)
            {
                mes = new List<MovingEntity>();

                foreach (IQuadObject obj in objs)
                {
                    MovingEntity me = obj as MovingEntity;

                    if (me.Side != this.Side)
                    {
                        mes.Add(me);
                    }
                }

                if (mes.Count > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        private void AttackHostile(MovingEntity hostile)
        {
            theta = GetThetaTo(hostile.Location);

            firepdu.LocationInWorldCoordinates = DisHelpers.ToEarthFixed(location.Northing, location.Easting);
            firepdu.Range = 1f;
            firepdu.TargetEntityID = hostile.DisId;
            firepdu.FiringEntityID = DisId;
            firepdu.LocationInWorldCoordinates = DisHelpers.ToEarthFixed(hostile.Location.Northing, hostile.Location.Easting);
            //firepdu.MunitionID = new EntityID() { Application = 0, Entity = 0, Site = 0 };
            //firepdu.BurstDescriptor = new BurstDescriptor();
            firepdu.BurstDescriptor.Munition = new OpenDis.Dis1998.EntityType() 
            { 
                Category = 4, 
                Country = 0, 
                Domain = 0, 
                EntityKind = 0, 
                Extra = 0, 
                Specific = 0, 
                Subcategory = 0 
            };

            firepdu.BurstDescriptor.Fuse = (ushort)OpenDis.Enumerations.Warfare.Fuse.AltitudeAirBurst;
            firepdu.BurstDescriptor.Warhead = (ushort)OpenDis.Enumerations.Warfare.Warhead._5Um;

            toSend.Add(firepdu);
    
            // won't send out for now
            /*
            detpdu.FiringEntityID = disId;            
            detpdu.LocationInWorldCoordinates = DisHelpers.ToEarthFixed(hostile.Location.Northing, hostile.Location.Easting);
            detpdu.MunitionID = new EntityID() { Application = 0, Entity = 0, Site = 0 };
            detpdu.BurstDescriptor = new BurstDescriptor();
            detpdu.BurstDescriptor.Munition = new OpenDis.Dis1998.EntityType()
            {
                Category = 5,
                Country = 0,
                Domain = 0,
                EntityKind = 0,
                Extra = 0,
                Specific = 0,
                Subcategory = 0
            };
            detpdu.BurstDescriptor.Fuse = (ushort)OpenDis.Enumerations.Warfare.Fuse.AltitudeAirBurst;
            detpdu.DetonationResult = (byte)OpenDis.Enumerations.Warfare.DetonationResult.KillWithFragmentType1;
            detpdu.BurstDescriptor.Warhead = (ushort)OpenDis.Enumerations.Warfare.Warhead._5Um;
            detpdu.NumberOfArticulationParameters = 0;
            detpdu.TargetEntityID = hostile.DisId;            
            detpdu.BurstDescriptor.Warhead = 0;
            detpdu.DetonationResult = 1;
            toSend.Add(detpdu);
             */
        }

        public void Update(float t)
        {
            lastTime = t;

            if (entityType == WinformExample_SAF.EntityType.Infantry) speed = 0.00005f;
            else if (entityType == WinformExample_SAF.EntityType.Tank) speed = 0.0001f;
            else if (entityType == WinformExample_SAF.EntityType.Air) speed = 0.0005f;

            if (order == WinformExample_SAF.Order.Idle)
            {
                // do nothing
            }
            else if (order == WinformExample_SAF.Order.Patrol_And_Destroy)
            {
                List<MovingEntity> hostiles = null;
                
                if (HostilesNearby(ref hostiles))
                {
                    AttackHostile(hostiles[0]);
                }

                if (route != null)
                {
                    route.StopDistance = speed * 100f; // at a guess
                    route.Update(location);
                    MoveTo(route.GetCurrent());
                    //UpdatePdu();
                }                                
            }
           
            UpdatePdu();
        }

        private void UpdatePdu()
        {
            byte b = byte.Parse(((int)entityType).ToString());
            espdu.EntityType = new OpenDis.Dis1998.EntityType() 
            { 
                Category = b, 
                Country = (ushort)side, 
                Domain = 0, 
                EntityKind = 0, 
                Extra = 0, 
                Specific = 0, 
                Subcategory = 0 
            };   
         
            espdu.EntityLocation = DisHelpers.ToEarthFixed(location.Northing, location.Easting);
            espdu.EntityLinearVelocity = velocity;
            espdu.NumberOfArticulationParameters = 1;
            espdu.Marking = DisHelpers.SetMarking(this.entityId);
            espdu.EntityID = DisId;

            espdu.EntityOrientation.Phi = phi;
            espdu.EntityOrientation.Psi = psi;
            espdu.EntityOrientation.Theta = theta;

            ap.ParameterTypeDesignator = (byte)ParameterTypeDesignator.ArticulatedPart;
            ap.ChangeIndicator = 0; // to inc every time 
            ap.PartAttachedTo = 0;          
            
            // articulated part high (primary turret, etc)  + articulated part low (position, elev, etc)
            int pt = DisHelpers.SetArticulationParameters(ArticulatedPartIndex.PrimaryGunNumber1, ArticulatedPartOffset.Rotation);
            ap.ParameterType = pt;  
            ap.ParameterValue = 45;                        

            toSend.Add(espdu);
        }

        //public byte[] GetDisPdu()
        //{
        //    return DisHelpers.MarshalPdu(espdu);
        //}

        public List<byte[]> GetDisPdus()
        {
            List<byte[]> packets = new List<byte[]>();

            foreach (Pdu pdu in toSend)
            {
                byte[] data = DisHelpers.MarshalPdu(pdu);
                packets.Add(data);
            }

            toSend.Clear();

            return packets;
        }
    }
}
