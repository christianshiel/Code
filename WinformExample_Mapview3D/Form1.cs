using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Diswerx;
using Diswerx.SubControls;
using Diswerx.DemRender;
using Diswerx.GeoTiffLoader;
using OpenDis.Dis1998;
using OpenDis.Enumerations;
using Diswerx.Common.Coordinates;

namespace WinformExample_Mapview3D
{
    public partial class Form1 : Form
    {
        /*
         * In this example everything runs on the GUI thread
         * which we wouldn't want if we have too many entities
         * as the graphics and network compete for processing
         */

        //System.Timers.Timer timer;

        Dictionary<ushort, Tuple<SpatialEntity, Model>> entities;
        DisNetwork disNet;
        Model original;
        string path;
        const float RAD_TO_DEG = 57.295779f;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            int device = disNet.GetDefaultBroadcastDevice();
            disNet.DetectDis(disNet_OnDisDetected, device);
            DisplayMessage("Looking for entities");
            DisplayMessage("====================");
        }

        void disNet_OnDisDetected(DisSource source)
        {
            DisplayMessage("Found entities");
            DisplayMessage("====================");

            disNet.Terminate();
            disNet.Connect(source);
            disNet.OnDisPduIn += new DisNetwork.DisPduIn(disNet_OnDisPduIn);

            //timer = new System.Timers.Timer(100);
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            //timer.Enabled = true;
        }

        //void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        void disNet_OnDisPduIn(OpenDis.Dis1998.Pdu pdu)
        {
            if (DisHelpers.GetPduType(pdu.PduType) == PduType.EntityState)
            {
                EntityStatePdu esPdu = pdu as EntityStatePdu;

                ushort id = esPdu.EntityID.Entity;

                if (entities.ContainsKey(id))
                {
                    SpatialEntity sp = entities[id].Item1;
                    Model m = entities[id].Item2;

                    double old_x = m.Location.X, old_y = m.Location.Z;

                    PointD newPoint = GeoHelpers.ToPointDLatLng(esPdu.EntityLocation);
                    double new_x = 0, new_y = 0;
                    subTerrain.LatLngToScreenXZ(newPoint.X, newPoint.Y, ref new_x, ref new_y);

                    //float h = terrain.GetExactHeightAt((float)new_x, (float)new_y);

                    //double a = Math.Atan2(new_y - old_y, new_x - old_x);
                    //float heading = (float)a * RAD_TO_DEG;

                    //m.RotateY(270f - heading);

                    //m.Translate((float)new_x, h + 10000, (float)new_y);
                }
                else
                {
                    SpatialEntity sp = new SpatialEntity();
                    sp.Id = id;

                    // Load a new model in...
                    // we create instances of this model because we have many copies
                    Model newEntity = new Model();
                    newEntity.Copy(original);
                    subTerrain.AddMesh(newEntity);

                    entities.Add(sp.Id, new Tuple<SpatialEntity, Model>(sp, newEntity));

                    PointD pt = GeoHelpers.ToPointDLatLng(esPdu.EntityLocation);
                    double x = 0, y = 0;
                    subTerrain.LatLngToScreenXZ(pt.X, pt.Y, ref x, ref y);
                    //float h = terrain.GetExactHeightAt((float)x, (float)y);

                    //newEntity.Translate((float)x, h + 10000, (float)y);
                }
            }

            ClearMesssages();

            List<Tuple<SpatialEntity, Model>> entityList = entities.Values.ToList<Tuple<SpatialEntity, Model>>();
            foreach (Tuple<SpatialEntity, Model> t in entityList)
            {
                SpatialEntity sp = t.Item1;
            }
        }

        private void ClearMesssages()
        {
            msgTextbox.Clear();
        }

        private void DisplayMessage(string message)
        {
            msgTextbox.AppendText(message + System.Environment.NewLine);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            disNet = new DisNetwork(this);
            entities = new Dictionary<ushort, Tuple<SpatialEntity, Model>>();

            subTerrain.OnMouse_Down += new MouseEventHandler(subTerrain_OnMouse_Down);
            subTerrain.OnMouseMove += new MouseEventHandler(subTerrain_OnMouseMove);

            subTerrain.Setup();
        }

        private void Setup()
        {
            path = @"C:\Diswerx\Data\";

            TileSet tileSet = new TileSet();
            TileCache tileCache = new TileCache();

            Diswerx.SubControls.SubTerrain.DEM_Params dem_params = new Diswerx.SubControls.SubTerrain.DEM_Params();
            
            //tileSet.LoadTileSet(tileCache, ref dem_params, path + @"TerrainSets\Canyon\", 1, 1, 500);

            //GeoTiff tiff = new GeoTiff();
            //tiff.Load(path + @"TerrainSets\Canyon\" + "tile_01_01.tif");
            //Helpers.LoadTexture(tiff.Bitmap, 0);
            //tiff.Bitmap.Dispose();
            //TerrainMesh mesh = tileCache.LoadMesh("tile_01_01");
            ////mesh.Texture = 0;
            ////mesh.LoadBuffers();
            ////subTerrain.AddMesh(mesh);            

            //subTerrain.Setup_DEM_Params(dem_params);

            tileCache.OptimizeCache();

            subTerrain.Cam.TranslateUp(10000);
            subTerrain.Cam.Yaw(45);

            original = new Model();
            original.Load(path + @"Models\Predator\", "MQ-9.3DS");
            subTerrain.AddMesh(original);

            subTerrain.ShowMousePointer = true;
        }

        void subTerrain_OnMouseMove(object sender, MouseEventArgs e)
        {
            mousePosLabel.Text = ("Lat: " + subTerrain.MouseLat + ", Lng: " + subTerrain.MouseLng);
        }

        void subTerrain_OnMouse_Down(object sender, MouseEventArgs e)
        {
        }

        private void loadTerrainFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setup();
            connectButton.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //timer.Enabled = false;

            if (disNet != null)
            {
                disNet.Terminate();
            }
        }
    }
}
