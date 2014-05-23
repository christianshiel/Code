using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Diswerx;
using Diswerx.Common;
using Diswerx.Common.Coordinates;
using Diswerx.GeoSorting;
using Diswerx.Shapefile;
using Diswerx.Surface2D;
using Networking;
using Diswerx.GeoTiffLoader;

namespace WinformExample_SAF
{
    public partial class Form1 : Form
    {
        public enum MouseMode
        {
            Navigate = 0,
            Place_Entity = 1,
            Place_Waypoint = 2,
        }

        private MouseMode mouseMode = MouseMode.Navigate;

        //private DisNetwork disNet;
        private NetworkManager3 net;
        private SpatialFilterEngine filterEngine;

        private List<MovingEntity> entities;
        private List<MovingEntity> create;
        private int entityCount = 0;
        private Point lastClickPoint;

        private Dictionary<string, Bitmap> entityIcons;

        System.Timers.Timer timer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Broadcast()
        {
            timer = new System.Timers.Timer(GetSendInterval());
            timer.Elapsed -= new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;

            net = new NetworkManager3(this);
            net.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateSim();
        }

        private int GetSendInterval()
        {
            int res = 2000;
            Int32.TryParse(intervalTextbox.Text, out res);
            return res;
        }

        private ushort GetPort()
        {
            ushort port = 4000;
            UInt16.TryParse(portTextbox.Text, out port);
            return port;
        }

        public int DeviceIndex
        {
            get
            {
                int deviceIndex = 0;
                deviceCombobox.Invoke(new MethodInvoker(delegate
                {
                    deviceIndex = deviceCombobox.SelectedIndex;
                }));
                return deviceIndex;
            }
            set
            {
                deviceCombobox.SelectedIndex = value;
            }
        }

        private void Disconnect()
        {
            if (timer != null)
                timer.Enabled = false;

            //if (disNet != null)
            //{
            //    disNet.Terminate();
            //}

            if (net != null)
            {
                net.Terminate();
            }
        }

        private void broadcastButton_Click(object sender, EventArgs e)
        {
            Broadcast();
        }

        private void mapClickModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (mapClickModeCheckbox.Checked)
            {
                mouseMode = MouseMode.Place_Entity;
            }
            else
            {
                mouseMode = MouseMode.Navigate;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        private object sync = new object();

        private void UpdateSim()
        {
            lock (sync)
            {
                //if (disNet != null)
                {
                    entities.AddRange(create);
                    create.Clear();

                    foreach (MovingEntity me in entities)
                    {
                        me.Update(timer1.Interval);
                    }

                    foreach (MovingEntity me in entities)
                    {
                        List<byte[]> pdus = me.GetDisPdus();

                        foreach (byte[] pdu in pdus)
                        {
                            //disNet.Broadcast(DeviceIndex, pdu, GetPort());
                            net.Send(pdu);
                        }
                    }
                }
            }
        }

        // delete?
        private void timer1_Tick(object sender, EventArgs e)
        {
            //net.Send("Roger");
            //UpdateSim();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = @"C:\Diswerx\Data\";
            string worldMap = path + @"Geotiffs\s.tif";            

            subSurface.LoadGeoTiff(worldMap);
            LoadShapefiles(path + "\\TerrainData\\houses.shp");

            GeoTiff geo = new GeoTiff();
            geo.Load(worldMap);
                    
            subSurface.OnMouse_Down += new MouseEventHandler(subSurface_OnMouse_Down);
            subSurface.OnMouseMove += new MouseEventHandler(subSurface_OnMouseMove);

            entities = new List<MovingEntity>();
            create = new List<MovingEntity>();

            filterEngine = new SpatialFilterEngine();

            Bitmap a = (Bitmap)Bitmap.FromFile(path + @"Images\Armour.png");
            Bitmap b = (Bitmap)Bitmap.FromFile(path + @"Images\Inf.png");
            Bitmap c = (Bitmap)Bitmap.FromFile(path + @"Images\Unmanned.png");
            Bitmap d = (Bitmap)Bitmap.FromFile(path + @"Images\Waypoint.png");

            entityIcons = new Dictionary<string, Bitmap>();
            entityIcons.Add("1", a);
            entityIcons.Add("2", b);
            entityIcons.Add("0", c);
            entityIcons.Add("Waypoint", d);

            //disNet = new DisNetwork(this);       

            //deviceCombobox.Items.AddRange(disNet.GetWindowsDeviceNames().ToArray());
        }

        private void LoadShapefiles(string path)
        {
            Shapefile sf = new Shapefile();
            sf.ReadShapeFile(path);

            int lineIndex = 0;
            foreach (Shapefile.Line line in sf.lines)
            {
                PointD[] pts = line.points;
                List<PointD> geoPts = new List<PointD>();

                foreach (PointD p in pts)
                {
                    GeoCoordinate coord = new GeoCoordinate() { Easting = p.X, Northing = -p.Y };
                    PointD sp = subSurface.GetScreenCoordinate(coord);
                    geoPts.Add(sp);
                }

                subSurface.DrawLines("Line" + lineIndex++, geoPts, Color.DarkGray, 2f);                
            }

            int polyIndex = 0;
            foreach (Shapefile.Polygon poly in sf.polygons)
            {
                PointD[] pts = poly.points;
                List<PointD> geoPts = new List<PointD>();

                foreach (PointD p in pts)
                {
                    GeoCoordinate coord = new GeoCoordinate() { Easting = p.X, Northing = -p.Y };
                    PointD sp = subSurface.GetScreenCoordinate(coord);
                    geoPts.Add(sp);
                }

                subSurface.DrawLines("Poly" + polyIndex++, geoPts, Color.Brown, 2f);
            }
        }

        void subSurface_OnMouseMove(object sender, MouseEventArgs e)
        {
            GeoCoordinate coord =  subSurface.GetWorldCoordinate(e.Location);
            mousePositionLabel.Text = "Lat: " + coord.Northing + ", Lng: " + coord.Easting;
        }

        void subSurface_OnMouse_Down(object sender, MouseEventArgs e)
        {
            if (mouseMode == MouseMode.Place_Entity)
            {
                EntityType newType = EntityType.Air;
                MovingEntity me = CreateNewEntity(e.Location, newType);
                filterEngine.AddObject(me);

                lastClickPoint = e.Location;
            }
            else if (mouseMode == MouseMode.Navigate)
            {
                // To fix...
                //GeoCoordinate mouseGeoPos = subSurface.GetWorldCoordinate(e.Location);
                //PointD p = subSurface.GetScreenCoordinate(mouseGeoPos);

                //float xz = 50f;
                //float xy = 32f;

                //List<IQuadObject> result = filterEngine.FindObjects(new System.Windows.Rect(p.X, p.Y, xz, xy));

                //foreach (IQuadObject obj in result)
                //{
                //    MovingEntity me = obj as MovingEntity;
                //    entityPropertyGrid.SelectedObject = me;
                //}
            }
            else if (mouseMode == MouseMode.Place_Waypoint)
            {
                GeoCoordinate mouseGeoPos = subSurface.GetWorldCoordinate(e.Location);
                PointD p = subSurface.GetScreenCoordinate(mouseGeoPos);                

                MovingEntity me = entityPropertyGrid.SelectedObject as MovingEntity;

                if (me != null)
                {
                    string wpId = me.Route.Id + me.Route.Waypoints.Count;
                    subSurface.DrawIcon(entityIcons["Waypoint"], Color.White, wpId, p.X, p.Y);
                    me.Route.Waypoints.Add(mouseGeoPos);
                }
            }
        }

        private MovingEntity CreateNewEntity(Point point, EntityType et)
        {
            string id = "Entity " + entityCount;
            entityCount++;
            string eType = ((byte)et).ToString();
            Bitmap b = entityIcons[eType];
            MovingEntity me = new MovingEntity(subSurface, filterEngine, b, point, Side.Blue, id, (ushort)entityCount);
            create.Add(me);  
            entityPropertyGrid.SelectedObject = me;
            entityComboBox.Items.Add(id);
            return me;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            mapClickModeCheckbox.CheckState = CheckState.Unchecked;
            waypointCheckbox.CheckState = CheckState.Unchecked;
        }

        private void entityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (entityComboBox.Items.Count > 0)
            {
                string id = entityComboBox.Items[entityComboBox.SelectedIndex] as string;
                MovingEntity me = entities.Find(x => x.EntityId == id);

                if (me != null)
                {
                    entityPropertyGrid.SelectedObject = me;
                    routePropertyGrid.SelectedObject = me.Route;
                }
                else
                {
                    me = create.Find(x => x.EntityId == id);
                    entityPropertyGrid.SelectedObject = me;
                    routePropertyGrid.SelectedObject = me.Route;
                }
            }
        }

        private void entityPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string propChange = e.ChangedItem.Label;
            if (propChange == "EntityType" || propChange == "Side")
            {
                // Update entity
                MovingEntity me = entityPropertyGrid.SelectedObject as MovingEntity;
                Bitmap b = entityIcons[((int)me.EntityType).ToString()];
                subSurface.RemoveDrawing(me.EntityId);
                me = new MovingEntity(subSurface, filterEngine, b, lastClickPoint, me.Side, me.EntityId, (ushort)entityCount);
            }

            mapClickModeCheckbox.Checked = false;
        }

        private void createRouteButton_Click(object sender, EventArgs e)
        {
            MovingEntity me = entityPropertyGrid.SelectedObject as MovingEntity;

            if (me != null)
            {
                Route route = new Route();
                route.Id = me.EntityId + " Route";
                me.Route = route;

                routePropertyGrid.SelectedObject = route;
            }
        }

        private void waypointCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (waypointCheckbox.Checked)
            {
                mouseMode = MouseMode.Place_Waypoint;
            }
            else
            {
                mouseMode = MouseMode.Navigate;
            }
        }
    }
}
