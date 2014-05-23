using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Windows.Input;
using Diswerx.Common;
using Diswerx.GeoSorting;
using Diswerx.SubControls;
using Diswerx.DemRender;

namespace Diswerx.SubControls
{
    public partial class SubTerrain : UserControl
    {
        public struct Ray
        {
            public Vector3 Start;
            public Vector3 End;
            public Vector3 Direction;
        }

        public struct DEM_Params
        {
            public float Y_Exaggeration;
            public double EastingEast;
            public double EastingWest;
            public double NorthingNorth;
            public double NorthingSouth;
            public double ResolutionEastWest;
            public double ResolutionNorthSouth;
        }

        public bool ShowMousePointer = false;

        private GLControl glControl;

        private Matrix4 matrixProjection = Matrix4.Identity;
        private Matrix4 matrixModelview = Matrix4.Identity;

        private Camera cam; public Camera Cam { get { return cam; } }

        public event System.Windows.Forms.MouseEventHandler OnMouseMove;
        public event System.Windows.Forms.MouseEventHandler OnMouse_Down;

        private Stopwatch stopWatch;

        private List<IDraw> meshes;

        private SpatialFilterEngine spf;

        private double eastingE;
        private double eastingW;
        private double northingN;
        private double northingS;
        
        double width;
        double height;
        double widthMeters;
        double heightMeters;

        private float res = 30f;
        private float mouse_x, mouse_y;
        private Ray mouseRay;
        private Vector3 mouseWorldPos = new Vector3();

        public Vector3 MouseWorldPosition { get { return mouseWorldPos; } }

        private double mouseLat;
        private double mouseLng;

        public double MouseLat { get { return mouseLat; } }
        public double MouseLng { get { return mouseLng; } }

        public SubTerrain()
        {
            InitializeComponent();
        }

        public void Setup()
        {
            if (DesignMode == true)
                return;

            glControl = new GLControl();

            glControl.Dock = DockStyle.Fill;
            this.Controls.Add(glControl);

            spf = new SpatialFilterEngine();

            stopWatch = new Stopwatch();
            meshes = new List<IDraw>();

            cam = new Camera();

            Color c = Color.FromArgb(68, 68, 68);

            GL.ClearColor(c);
            GL.Enable(EnableCap.DepthTest);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            GL.Enable(EnableCap.Lighting);

            glControl.Resize += new EventHandler(glControl_Resize);
            glControl.Paint += new PaintEventHandler(glControl_Paint);
            glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(glControl_MouseMove);
            glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(glControl_MouseDown);
            glControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(glControl_MouseWheel);

            SetupViewport();
            SetupLighting();

            Application.Idle += new EventHandler(Application_Idle);
        }

        public void SetTerrainYResolution(float res)
        {
            this.res = res;
        }

        private void SetupLighting()
        {
            #region Setup lights

            GL.ShadeModel(ShadingModel.Smooth);

            GL.PushMatrix();

            GL.LoadIdentity();

            //float[] light_ambient = { 0.4f, 0.4f, 0.4f, 0.4f };
            //float[] light_diffuse = { 0.4f, 0.4f, 0.4f, 0.4f };
            //float[] light_position = { -10000, -100f, 0.0f, 0f };
            //float[] light_specular = { 0, 0, 0, 0 };

            float[] light_ambient = { 0.4f, 0.4f, 0.4f, 0.6f };
            float[] light_diffuse = { 0.4f, 0.4f, 0.4f, 0.6f };
            float[] light_position = { 1, 1, 0f, 0f };
            float[] light_specular = { 0.1f, 0.1f, 0.1f, 0.1f };

            GL.Light(LightName.Light0, LightParameter.Ambient, light_ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, light_diffuse);
            GL.Light(LightName.Light0, LightParameter.Position, light_position);
            GL.Light(LightName.Light0, LightParameter.Specular, light_specular);

            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, light_ambient);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, light_diffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, light_position);
            GL.Material(MaterialFace.Front, MaterialParameter.Emission, light_specular);

            GL.PopMatrix();

            #endregion
        }

        public void Setup_DEM_Params(DEM_Params dem) //DemModel dem)
        {
            //eastingE = dem.EastingNorthEast;
            //eastingW = dem.EastingNorthWest;
            //northingN = dem.NorthingNorthEast;
            //northingS = dem.NorthingSouthEast;

            eastingE = dem.EastingEast;
            eastingW = dem.EastingWest;
            northingN = dem.NorthingNorth;
            northingS = dem.NorthingSouth;

            width = (eastingE - eastingW);
            height = (northingN - northingS);

            widthMeters = width * dem.ResolutionEastWest;
            heightMeters = height * dem.ResolutionNorthSouth;

            SetTerrainYResolution(dem.Y_Exaggeration);
        }

        // Probably want to tidy up either equations

        public void LatLngToScreenXZ(double lat, double lng, ref double x, ref double y)
        {
            y = ((northingN / 3600.0) - lat) * 3600.0 * res;
            x = -(3600.0 * res * (-lng + (eastingW / 3600.0)));
        }

        void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //FindMousePos();

            if (OnMouse_Down != null)
                OnMouse_Down.Invoke(sender, e);
        }

        void glControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //eyeZ += (float)(e.Delta / 100f);
        }

        private void FindMousePos()
        {
            if (meshes.Count > 0)
            {
                mouseRay = GetPointerRay2();
                //Ray shorterRay = LinearSearch(ray, meshes[0] as VboTerrainMesh);
                Vector3 p = BinarySearch(mouseRay, meshes[0] as TerrainMesh);

                mouseWorldPos = p;

                double northing = (northingN / 3600.0) - ((p.Z / res) / 3600.0);
                double easting = ((-p.X / res) / 3600.0) - (eastingW / 3600.0);

                float y = (float)((northingN / 3600f) - northing) * 3600f * res;
                float x = (3600f * res * ((float)easting + ((float)eastingW / 3600f)));

                mouseLat = northing;
                mouseLng = -easting;
            }
        }

        void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //{
            //    cam_x = (e.X - (Width / 2)) / 5f;
            //    cam_y = (e.Y - (Height / 2)) / 5f;

            //    cam.Pitch(-cam_y);
            //    cam.Yaw(-cam_x);
            //}

            mouse_x = (float)e.X;
            mouse_y = (float)e.Y;

            //if (meshes.Count > 0)
            //{
            //    mouseRay = GetPointerRay2();
            //    //Ray shorterRay = LinearSearch(ray, meshes[0] as VboTerrainMesh);
            //    mouseWorldPos = BinarySearch(mouseRay, meshes[0] as TerrainMesh);
            //}

            FindMousePos();

            if (OnMouseMove != null)
                OnMouseMove.Invoke(sender, e);
        }

        private void HandleKeyboard()
        {
            //camRotationX = cam.X_Rotation;
            //camRotationY = cam.Y_Rotation;

            if ((Keyboard.GetKeyStates(Key.Q) & KeyStates.Down) > 0)
            {
                cam.TranslateUp(200f);
            }

            if ((Keyboard.GetKeyStates(Key.A) & KeyStates.Down) > 0)
            {
                cam.TranslateUp(-200f);
            }

            if ((Keyboard.GetKeyStates(Key.W) & KeyStates.Down) > 0)
            {
                cam.Pitch(-1f);
            }

            if ((Keyboard.GetKeyStates(Key.S) & KeyStates.Down) > 0)
            {
                cam.Pitch(1f);
            }

            if ((Keyboard.GetKeyStates(Key.Right) & KeyStates.Down) > 0)
            {
                cam.Yaw(-1f);
            }

            if ((Keyboard.GetKeyStates(Key.Left) & KeyStates.Down) > 0)
            {
                cam.Yaw(1f);
            }

            if ((Keyboard.GetKeyStates(Key.Up) & KeyStates.Down) > 0)
            {
                cam.Translate(cam.Forward, 1000);
            }

            if ((Keyboard.GetKeyStates(Key.Down) & KeyStates.Down) > 0)
            {
                cam.Translate(cam.Forward, -1000);
            }

            //if ((Keyboard.GetKeyStates(Key.OemPlus) & KeyStates.Down) > 0)
            //{
            //    scale += 0.01f;
            //}

            //if ((Keyboard.GetKeyStates(Key.OemMinus) & KeyStates.Down) > 0)
            //{
            //    scale -= 0.01f;
            //}


        }

        public void AddMesh(IDraw m)
        {
            meshes.Add(m);
            //spf.AddObjects(m.GetQuads());
        }

        private double SyncDelta()
        {
            stopWatch.Stop();
            double milliseconds = stopWatch.Elapsed.TotalMilliseconds;
            stopWatch.Reset();
            stopWatch.Start();

            return milliseconds;
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            float delta = (float)SyncDelta();

            #region Camera

            GL.PushMatrix();

            Vector3 v = cam.Forward * 10000000f;
            Vector3 look = v;

            matrixModelview = Matrix4.LookAt(cam.Position, look, Vector3.UnitY);

            GL.PopMatrix();

            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref matrixModelview);

            GL.Light(LightName.Light0, LightParameter.Position, new float[] { -1000, 1000f, 0.0f, 0.75f });

            //GL.Scale(scale, scale, scale);

            #endregion

            #region Drawing

            foreach (IDraw m in meshes)
            {
                m.Draw();
            }

            #endregion

            // Draw mouse pos
            if (ShowMousePointer)
            {
                GL.Disable(EnableCap.Texture2D);
                GL.LineWidth(50f);
                GL.Color4(Color.Red);
                GL.Begin(BeginMode.Lines);
                //GL.Vertex3(mouseRay.Start);
                //mouseRay.End.Y = 10f;
                //GL.Vertex3(mouseRay.End);
                GL.Vertex3(mouseWorldPos);
                Vector3 b = new Vector3(mouseWorldPos.X, mouseWorldPos.Y + 4000f, mouseWorldPos.Z);
                GL.Vertex3(b);
                GL.End();
                GL.Color4(Color.White);
                GL.Enable(EnableCap.Texture2D);
            }

            GL.PopMatrix();

            glControl.SwapBuffers();

            modelInv = new Matrix4(matrixModelview.Row0, matrixModelview.Row1, matrixModelview.Row2, matrixModelview.Row3);
            projInv = new Matrix4(matrixProjection.Row0, matrixProjection.Row1, matrixProjection.Row2, matrixProjection.Row3);
        }

        Matrix4 modelInv;
        Matrix4 projInv;

        public Ray LinearSearch(Ray ray, TerrainMesh terrain)
        {
            //Vector3 dir = ray.End - ray.Start;
            //dir.Normalize();
            ray.Direction /= 300f;
            Vector3 nextPoint = ray.Start + ray.Direction;
            float heightAtNextPoint = terrain.GetExactHeightAt(nextPoint.X, -nextPoint.Z);

            while (heightAtNextPoint < nextPoint.Y)
            {
                ray.Start = nextPoint;
                nextPoint = ray.Start + ray.Direction;
                heightAtNextPoint = terrain.GetExactHeightAt(nextPoint.X, -nextPoint.Z);
            }

            return ray;
        }

        public Vector3 BinarySearch(Ray ray, TerrainMesh terrain)
        {
            float accuracy = 0.01f;

            float heightAtStartingPoint = terrain.GetExactHeightAt(ray.Start.X, -ray.Start.Z);
            float error = ray.Start.Y - heightAtStartingPoint;

            //dir.Normalize();

            ray.Direction = ray.End - ray.Start;

            int counter = 0;
            while (error > accuracy)
            {
                ray.Direction /= 2f;
                Vector3 nextPoint = ray.Start + ray.Direction;
                float x = nextPoint.X; float y = -nextPoint.Z;
                float heightAtNextPoint = terrain.GetExactHeightAt(x, y);

                if (nextPoint.Y > heightAtNextPoint)
                {
                    ray.Start = nextPoint;
                    error = ray.Start.Y - heightAtNextPoint;
                }
                if (counter++ == 1000) break;
            }

            return ray.Start;
        }

        public static Vector4 UnProject(ref Matrix4 projection, Matrix4 view, Size viewport, Vector2 mouse, float z)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = z;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(view);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > float.Epsilon || vec.W < float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec;
        }

        public Ray GetPointerRay2()
        {
            Ray ray = new Ray();

            Vector2 win = new Vector2(mouse_x, mouse_y);
            Vector4 worldPositionNear = UnProject(ref matrixProjection, matrixModelview, glControl.Size, win, 0f);

            ray.Start = worldPositionNear.Xyz;

            Vector4 worldPositionFar = UnProject(ref matrixProjection, matrixModelview, glControl.ClientSize, win, 1f);

            ray.End = worldPositionFar.Xyz;

            return ray;
        }

        public Ray GetPointerRay()
        {
            // normalise mouse
            float xpos = 2f * ((float)mouse_x / (float)glControl.Width) - 1f;
            float ypos = 2f * ((float)1 - mouse_y / (float)glControl.Height) - 1f;

            Vector4 startRay = new Vector4(xpos, ypos, -1f, 1);
            Vector4 endRay = new Vector4(xpos, ypos, 1f, 1);

            // Reverse Project
            Matrix4 trans = matrixModelview * matrixProjection;
            trans.Invert();

            startRay = Vector4.Transform(startRay, trans);
            endRay = Vector4.Transform(endRay, trans);
            Vector3 sr = startRay.Xyz / startRay.W;
            Vector3 er = endRay.Xyz / endRay.W;

            Ray ray = new Ray();
            ray.Start = sr;
            ray.End = er;
            ray.Direction = ray.End - ray.Start;

            //ray.Direction.Normalize();
            //ray.Direction *= 250f;

            return ray;
        }

        private void SetupViewport()
        {
            GL.PushMatrix();
            GL.Viewport(0, 0, Width, Height);
            matrixProjection = cam.CreateView(Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref matrixProjection);
            GL.PopMatrix();
        }

        void Application_Idle(object sender, EventArgs e)
        {
            // Main loop here...
            HandleKeyboard();
            glControl.Invalidate();
        }

        void glControl_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        void glControl_Resize(object sender, EventArgs e)
        {
            if (DesignMode == true)
                return;

            SetupViewport();
        }

        private void SubTerrain_Load(object sender, EventArgs e)
        {

        }


    }
}