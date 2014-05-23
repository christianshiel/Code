using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Diswerx.SubControls
{
    public class Camera
    {
        // Basis
        private Vector3 forward; public Vector3 Forward { get { return forward; } }
        private Vector3 right; public Vector3 Right { get { return right; } }
        private Vector3 up; public Vector3 Up { get { return up; } }

        private Vector3 position; public Vector3 Position { get { return position; } }

        public float Near = 100f;
        public float Far = 5000000f;

        private float y_rotation = 0f; public float Y_Rotation { get { return y_rotation; } }
        private float x_rotation = 0f; public float X_Rotation { get { return x_rotation; } }

        public Camera()
        {
            this.forward = new Vector3(0, 0, 1);
            this.right = new Vector3(1, 0, 0);
            this.up = new Vector3(0, 1, 0);
        }

        public Matrix4 CreateView(float width, float height)
        {
            return Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, Near, Far);
        }

        public void SetPos(Vector3 pos)
        {
            this.position = pos;
        }

        public void SetPos(float x, float y, float z)
        {
            this.position.X = x;
            this.position.Y = y;
            this.position.Z = z;
        }

        public void Pitch(float pitch)
        {
            GL.PushMatrix();

            x_rotation = pitch;

            float rad_x = MathHelper.DegreesToRadians(x_rotation);

            Matrix4 xRot = Matrix4.CreateFromAxisAngle(right, rad_x);

            forward = Vector3.TransformVector(forward, xRot);
            up = Vector3.TransformVector(up, xRot);

            forward.Normalize();
            up.Normalize();

            GL.PopMatrix();
        }

        public void Yaw(float yaw)
        {
            GL.PushMatrix();

            y_rotation = yaw;

            float rad_y = MathHelper.DegreesToRadians(y_rotation);

            Matrix4 yRot = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), rad_y);

            forward = Vector3.TransformVector(forward, yRot);
            right = Vector3.TransformNormal(right, yRot);

            forward.Normalize();
            right.Normalize();

            GL.PopMatrix();
        }

        public void Translate(Vector3 dir, float amount)
        {
            GL.PushMatrix();
            Matrix4 t = Matrix4.CreateTranslation(dir * amount);
            position = Vector3.Transform(position, t);
            GL.PopMatrix();
        }

        public void TranslateUp(float amount)
        {
            GL.PushMatrix();
            Matrix4 t = Matrix4.CreateTranslation(Vector3.UnitY * amount);
            position = Vector3.Transform(position, t);
            GL.PopMatrix();
        }

    }
}
