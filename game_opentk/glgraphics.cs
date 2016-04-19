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
        Random random = new Random();
        public List<int> texturesIDs = new List<int>();
        public Color[] colors = { Color.Blue, Color.Yellow, Color.Purple, Color.MediumSeaGreen };
        
        Vector3 cameraPosition = new Vector3(0, -20, 0); //позиция
        Vector3 cameraDirecton = new Vector3(0, 0, 0); //направление
        Vector3 cameraUp = new Vector3(0, 0, 1);
        
        // координаты ветора броска
        public  float _x = 0, _y;
        // Флаг на бросок
        public bool Throw_flag = false;
        // отсчет времени 
        public float global_time = 0;
        // начальная скорость вектора 
        public float speed_time = 0;
        // мяч
        public ThrowBall ball1 = new ThrowBall(0, 0, 0);
        // квадраты игровые
        public GameQuad quad1, quad2, quad3, quad4;
        public Color mainColor;
        public float score = 0;
        public bool BoxFlag = false;


        public void Setup(int width, int height)
        {
            GL.ClearColor(Color.LightBlue); //буфер экрана одним цветом
            GL.ShadeModel(ShadingModel.Smooth); //тип отрисовки
            GL.Enable(EnableCap.DepthTest); //  буфер глубины 
            //матрица проекции под размер окна
            Matrix4 perspectiveMat = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            //SetupLightning();
            GL.LoadMatrix(ref perspectiveMat);  //загрузка в контекст OpenGL

            quad1 = new GameQuad(colors[0], colors[random.Next(0, 4)], 0.5f, -10, 40, 5, 10);
            quad2 = new GameQuad(colors[1], colors[random.Next(0, 4)], 0.5f, 10, 40, 5, 10);
            quad3 = new GameQuad(colors[2], colors[random.Next(0, 4)], 0.5f, -10, 40, -5, 10);
            quad4 = new GameQuad(colors[3], colors[random.Next(0, 4)], 0.5f, 10, 40, -5, 10);
            mainColor = colors[random.Next(0, 4)];
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

        public void Update() //очищает буферы
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 viewMat = Matrix4.LookAt(cameraPosition, cameraDirecton, cameraUp);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref viewMat);
                        
            Render();

            global_time += 0.00015f;   
        }       

        public void Render()
        {
                      
            DrawBall(ball1.GetBallPositionX(), ball1.GetBallPositionY(), ball1.GetBallPositionZ(), 1, 1, 1, 40, 40);

            if (quad1.Update(global_time, mainColor, colors[random.Next(0, 4)], ball1.GetBallPositionX(), ball1.GetBallPositionY(), ball1.GetBallPositionZ())
              || quad2.Update(global_time, mainColor, colors[random.Next(0, 4)], ball1.GetBallPositionX(), ball1.GetBallPositionY(), ball1.GetBallPositionZ())
              || quad3.Update(global_time, mainColor, colors[random.Next(0, 4)], ball1.GetBallPositionX(), ball1.GetBallPositionY(), ball1.GetBallPositionZ())
              || quad4.Update(global_time, mainColor, colors[random.Next(0, 4)], ball1.GetBallPositionX(), ball1.GetBallPositionY(), ball1.GetBallPositionZ())
               )
            {
                mainColor = colors[random.Next(0, 4)];
                score++;
                BoxFlag = true;             
            }

            ball1.Calculate(global_time);//пересчет полета

            DrawFloor(20, 0, 20, -10);
            DrawTop(20, 0, 20, 10);
            
            DrawWall(20, -20, 20, 0);
            DrawWall(20, 20, 20, 0);
            
            DrawBack(quad1.GetA(), quad1.GetX(), quad1.GetY(), quad1.GetZ(), quad1.Now());
            DrawBack(quad2.GetA(), quad2.GetX(), quad2.GetY(), quad2.GetZ(), quad2.Now());
            DrawBack(quad3.GetA(), quad3.GetX(), quad3.GetY(), quad3.GetZ(), quad3.Now());
            DrawBack(quad4.GetA(), quad4.GetX(), quad4.GetY(), quad4.GetZ(), quad4.Now());

        }



        private void DrawBack(float a, float x, float y, float z, Color color)
        {
            GL.Color3(color);
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(x - a, y, z - a/2);
            GL.Vertex3(x - a, y, z + a/2);
            GL.Vertex3(x + a, y, z + a/2);
            GL.Vertex3(x + a, y, z - a/2);

            GL.End();
            GL.Color3(Color.White);
        }
        private void DrawFloor(float a, float x, float y, float z)
        {
            GL.Enable(EnableCap.Texture2D);            
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[0]);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(x - a, y - a, z);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(x - a, y + a, z);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(x + a, y + a, z);
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(x + a, y - a, z);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
        private void DrawTop(float a, float x, float y, float z)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[3]);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(x - a, y - a, z);
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(x - a, y + a, z);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(x + a, y + a, z);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(x + a, y - a, z);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
        private void DrawWall(float a, float x, float y, float z)
        {
            GL.Enable(EnableCap.Texture2D);            
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[2]);

            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0, 0.0);
            GL.Vertex3(x, y - a, z - a/2);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex3(x, y + a, z - a/2);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex3(x, y + a, z + a/2);
            GL.TexCoord2(0.0, 1.0);
            GL.Vertex3(x, y - a, z + a/2);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }
        private void DrawBall(double xx, double yy, double zz, double rx, double ry, double rz, int nx, int ny)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texturesIDs[1]);

            int ix, iy;
            double x, y, z;
            for (iy = 0; iy < ny; ++iy)
            {
                GL.Begin(PrimitiveType.QuadStrip);

                for (ix = 0; ix <= nx; ++ix)
                {
                    x = rx * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + xx;
                    y = ry * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + yy;
                    z = rz * Math.Cos(iy * Math.PI / ny) + zz;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)iy / (double)ny);
                    GL.Vertex3(x, y, z);
                    x = rx * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx) + xx;
                    y = ry * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx) + yy;
                    z = rz * Math.Cos((iy + 1) * Math.PI / ny) + zz;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)(iy + 1) / (double)ny);
                    GL.Vertex3(x, y, z);
                }              
                
                GL.End();                
            }

        GL.Disable(EnableCap.Texture2D);
        }

    }
}
