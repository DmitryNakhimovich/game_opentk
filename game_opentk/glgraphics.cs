using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using graph = OpenTK.Graphics;
using System.Data;
using System.Drawing.Imaging;
using System.Collections;

namespace game_opentk
{
    class glgraphics
    {
        public List<int> texturesIDs = new List<int>();
        public float latitude = 47.98f;
        public float longitude = 60.41f;
        public float radius = 5.385f;

        Vector3 cameraPosition = new Vector3(0, -20, 0); //позиция
        Vector3 cameraDirecton = new Vector3(0, 0, 0); //направление
        Vector3 cameraUp = new Vector3(0, 0, 1);

        public float coordX = 0;
        public float coordY = 3;
        public float coordZ = 1.5f;
        public float speed = 0.3f;

        // координаты ветора броска
        float _x = 0, _y;
        // Флаг на бросок
        bool Throw_flag = false;
        // отсчет времени 
        float global_time = 0;
        float speed_time = 0;
        // экземпляр класса BallModel
        private ThrowBall ball1 = new ThrowBall(15, 2, 20);

        public void Setup(int width, int height)
        {
            GL.ClearColor(Color.LightBlue); //буфер экрана одним цветом
            GL.ShadeModel(ShadingModel.Smooth); //тип отрисовки
            GL.Enable(EnableCap.DepthTest); //  буфер глубины 
            //матрица проекции под размер окна
            Matrix4 perspectiveMat = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            // SetupLightning();
            GL.LoadMatrix(ref perspectiveMat);  //загрузка в контекст OpenGL
        }

        public void Update() //очищает буферы
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 viewMat = Matrix4.LookAt(cameraPosition, cameraDirecton, cameraUp);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref viewMat);

            Render();           
        }       

        public void Render()
        {
            GL.Color3(Color.Red);            
            DrawSphereNew(0, 0, 0, 1, 1, 1, 40, 40, 0);

            GL.Color3(Color.Black);
            drawQuad(20, 0, 20, -10);
                       
        }
                

        public int LoadTexture(String filePath)
        {
            try
            {
                Bitmap image = new Bitmap(filePath);
                int texID = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texID);
                BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                image.UnlockBits(data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                return texID;
            }
            catch (System.IO.FileNotFoundException е)
            {
                return -1;
            }
        }

        private void SetupLightning()
        {
            GL.Enable(EnableCap.Lighting);
            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            //положение источника света
            Vector4 lightPosition = new Vector4(1.0f, 1.0f, 4.0f, 0.0f);
            GL.Light(LightName.Light0, LightParameter.Position, lightPosition);
            //ambient цвет источника – цвет, который будет иметь объект, не освещенный источником.
            Vector4 ambientColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Ambient, ambientColor);
            //diffuse цвет источника – цвет, который будет иметь объект, освещенный источником. 
            Vector4 diffuseColor = new Vector4(0.6f, 0.6f, 1.0f, 1.0f);
            GL.Light(LightName.Light0, LightParameter.Diffuse, diffuseColor);
            Vector4 materialSpecular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, materialSpecular);
            float materialShininess = 100;
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, materialShininess);
        }


        private void drawQuad(float a, float x, float y, float z)
        {
            // GL.Enable(EnableCap.Texture2D);            
            // GL.BindTexture(TextureTarget.Texture2D, texturesIDs[1]);
            GL.Begin(BeginMode.Quads);                       
            GL.Vertex3(x - a, y - a, z);
            GL.Vertex3(x - a, y + a, z);
            GL.Vertex3(x + a, y + a, z);           
            GL.Vertex3(x + a, y - a, z);
            GL.End();

            //GL.Disable(EnableCap.Texture2D);
        }

        private void DrawSphereNew(double xx, double yy, double zz, double rx, double ry, double rz, int nx, int ny, int text)
        {
           // GL.Enable(EnableCap.Texture2D);
           // GL.BindTexture(TextureTarget.Texture2D, text);
            int ix, iy;
            double x, y, z;
            for (iy = 0; iy < ny; ++iy)
            {
                GL.Begin(BeginMode.QuadStrip);
                for (ix = 0; ix <= nx; ++ix)
                {
                    x = rx * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + xx;
                    y = ry * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + yy;
                    z = rz * Math.Cos(iy * Math.PI / ny) + zz;
                    GL.Normal3(x, y, z);
                    GL.Vertex3(x, y, z);
                    x = rx * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + xx;
                    y = ry * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + yy;
                    z = rz * Math.Cos((iy + 1) * Math.PI / ny) + zz;
                    GL.Normal3(x, y, z);
                    GL.Vertex3(x, y, z);

                }                
                
                GL.End();
           //     GL.Disable(EnableCap.Texture2D);
            }
        }

    }
}
