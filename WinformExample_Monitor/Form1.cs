using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Diswerx;
using OpenDis.Dis1998;
using OpenDis.Enumerations;
using Diswerx.GeoSorting;
using Diswerx.Common;
using Diswerx.Common.Coordinates;
using Diswerx.GeoTiffLoader;
using Diswerx.Shapefile;

namespace WinformExample_Monitor
{
    public partial class Form1 : Form
    {
        private DisNetwork disNet;
        private bool autoDetectConnection = true;
        private bool foundDIS = false;
        private int elapsed = 0;
        private Dictionary<ushort, SpatialEntity> myEntities;
        private Dictionary<string, Bitmap> entityIcons;
        private SpatialFilterEngine filterEngine;

        public Form1()
        {
            InitializeComponent();
        }

        private void DetectDis()
        {            
            int device = disNet.GetDefaultBroadcastDevice();
            disNet.DetectDis(disNet_OnDisDetected, device);
            timer1.Start();
            connectButton.Enabled = false;

            DisplayMessage("I am looking for DIS on " + disNet.GetWindowsDeviceNames()[device] + ".");
            DisplayMessage("================");
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (autoDetectConnection)
            {
                DetectDis();
            }
            else
            {
                // Manual connect
                int device = deviceCombobox.SelectedIndex;
                ushort port = ushort.Parse(portTextbox.Text);
                DisSource source = new DisSource("Unknown", port, device);
                disNet.Connect(source);

                disNet.OnDisPduIn -= new DisNetwork.DisPduIn(disNet_OnDisPduIn);
                disNet.OnDisPduIn += new DisNetwork.DisPduIn(disNet_OnDisPduIn);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //Shapefile sf = new Shapefile();
            //sf.ReadShapeFile(@"C:\Diswerx\Data\TerrainData\us_roads.shp");

            //Console.WriteLine(sf.lines.Count);

            disNet = new DisNetwork(this);

            string path = @"C:\Diswerx\Data\";
            string worldMap = path + @"Geotiffs\us.tif";

            subSurface.LoadGeoTiff(worldMap);

            subSurface.OnMouse_Down += new MouseEventHandler(subSurface_OnMouse_Down);
            subSurface.OnMouseMove += new MouseEventHandler(subSurface_OnMouseMove);

            myEntities = new Dictionary<ushort, SpatialEntity>();
            
            // A kung fu example
            Bitmap a = (Bitmap)Bitmap.FromFile(path + @"Images\Armour.png");
            Bitmap b = (Bitmap)Bitmap.FromFile(path + @"Images\Inf.png");
            Bitmap c = (Bitmap)Bitmap.FromFile(path + @"Images\Unmanned.png");

            entityIcons = new Dictionary<string, Bitmap>();
            entityIcons.Add("1", a);
            entityIcons.Add("2", b);
            entityIcons.Add("0", c);

            filterEngine = new SpatialFilterEngine();

            deviceCombobox.Items.AddRange(disNet.GetWindowsDeviceNames().ToArray());
            deviceCombobox.SelectedIndex = disNet.GetDefaultBroadcastDevice();
        }

        void subSurface_OnMouseMove(object sender, MouseEventArgs e)
        {
            GeoCoordinate coord = subSurface.GetWorldCoordinate(e.Location);
            mousePositionLabel.Text = "Lat: " + coord.Northing + ", Lng: " + coord.Easting;
        }

        void subSurface_OnMouse_Down(object sender, MouseEventArgs e)
        {
            #region Spatial single click query

            // do a spatial query
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                GeoCoordinate mouseGeoPos = subSurface.GetWorldCoordinate(e.Location);                
                
                PointD p = subSurface.GetScreenCoordinate(mouseGeoPos);

                float xz = 50f;
                float xy = 32f;

                List<IQuadObject> result = filterEngine.FindObjects(new System.Windows.Rect(p.X, p.Y, xz, xy));

                string coord = mouseGeoPos.Northing + ", " + mouseGeoPos.Easting;

                foreach (IQuadObject obj in result)
                {
                    SpatialEntity sp = obj as SpatialEntity;

                    DisplayMessage("Found: Id = " + sp.Id);
                    //subSurface.ResizeDrawing(sp.Id.ToString(), 2f, 2f);
                    //lastClickedId = sp.Id.ToString();
                }
            }
            else
            {
                //if (lastClickedId != null)
                //{
                //    subSurface.ResizeDrawing(lastClickedId, 1f, 1f);
                //}
            }

            #endregion
        }

        void disNet_OnDisDetected(DisSource source)
        {
            // No longer need to listen to all ports
            disNet.Terminate();

            foundDIS = true;
            disNet.Connect(source);
            DisplayMessage("Found DIS...connecting");
            DisplayMessage("================");

            disNet.OnDisPduIn -= new DisNetwork.DisPduIn(disNet_OnDisPduIn);
            disNet.OnDisPduIn += new DisNetwork.DisPduIn(disNet_OnDisPduIn);
        }

        void disNet_OnDisPduIn(Pdu pdu)
        {            
            // Note that we wrap up the Pdu in a SpatialEntity...this is so
            // the SpatialQueryEngine can run queries on the dataset

            if (DisHelpers.GetPduType(pdu.PduType) == PduType.EntityState)
            {
                EntityStatePdu esPdu = pdu as EntityStatePdu;                

                ushort id = esPdu.EntityID.Entity;

                if (myEntities.ContainsKey(id))
                {
                    SpatialEntity sp = myEntities[id];

                    PointD pt = GeoHelpers.ToPointDLatLng(esPdu.EntityLocation);

                    PointD p = subSurface.GetScreenCoordinate(new GeoCoordinate(pt.Y, pt.X));
                    sp.Update(p.X, p.Y, 0.1f, 0.1f);

                    subSurface.TranslateDrawing(sp.Id.ToString(), p.X, p.Y);
                }
                else
                {
                    SpatialEntity sp = new SpatialEntity();
                    sp.Id = id;

                    myEntities.Add(sp.Id, sp);

                    // Just one way to do it
                    PointD pt = GeoHelpers.ToPointDLatLng(esPdu.EntityLocation);

                    // store spatial entities in screen space, of course could store in world space, etc
                    PointD p = subSurface.GetScreenCoordinate(new GeoCoordinate(pt.Y, pt.X));
                    sp.Update(p.X, p.Y, 0.1f, 0.1f);

                    string category = esPdu.EntityType.Category.ToString();

                    Bitmap entityIcon = entityIcons[category];
                    
                    subSurface.DrawIcon(entityIcon, Color.White, sp.Id.ToString(), p.X, p.Y);

                    filterEngine.AddObject(sp);
                }
            }
        }

        private void DisplayMessage(string message)
        {
            messageTextbox.AppendText(message + System.Environment.NewLine);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (disNet != null)
                disNet.Terminate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsed += timer1.Interval;

            if (foundDIS == false && elapsed == 5000)
            {
                DisplayMessage("Ok this is suspicious.");
                DisplayMessage("No DIS found yet.");
                DisplayMessage("================");
            }
            
            if (foundDIS == false && elapsed >= 10000)
            {
                DisplayMessage("Ok no luck finding DIS, stopping the search.");
                DisplayMessage("Am I looking on the wrong adapter perhaps?");
                DisplayMessage("================");
                elapsed = 0;
                timer1.Enabled = false;
                disNet.Terminate();
                connectButton.Enabled = true;
            }
        }

        private void autoDetectCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            autoDetectConnection = autoDetectCheckbox.Checked;
        }
    }
}
