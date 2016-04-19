using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace game_opentk
{
    public partial class Form1 : Form
    {
        glgraphics glgraphics = new glgraphics();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            glgraphics.Setup(glControl1.Width, glControl1.Height);

            Application.Idle += Application_Idle;

            int texID = glgraphics.LoadTexture("1.jpg");
            glgraphics.texturesIDs.Add(texID);
            texID = glgraphics.LoadTexture("2.jpg");
            glgraphics.texturesIDs.Add(texID);
            texID = glgraphics.LoadTexture("3.jpg");
            glgraphics.texturesIDs.Add(texID);
            texID = glgraphics.LoadTexture("4.jpg");
            glgraphics.texturesIDs.Add(texID);

            textBox1.BackColor = glgraphics.mainColor;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
                glControl1.Refresh();            
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glgraphics.Update();
            glControl1.SwapBuffers();  
            
            if (glgraphics.BoxFlag)
            {
                textBox1.BackColor = glgraphics.mainColor;
                textBox1.Text = glgraphics.score.ToString();
            }          
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            glgraphics.Throw_flag = true;
        }
        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (glgraphics.Throw_flag)
            {
                glgraphics.speed_time += 0.01f;
            }
        }
        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (glgraphics.Throw_flag)
            {
                float[] Throw_vektor = new float[3];
                glgraphics._x = e.X - 620;
                glgraphics._y = e.Y - 320;
                Throw_vektor[0] = glgraphics._x;
                Throw_vektor[2] = -glgraphics._y * 1.5f;
                Throw_vektor[1] = (float)Math.Sqrt(((float)Math.Pow(Math.Abs(Throw_vektor[0]), 2) + (float)Math.Pow(Math.Abs(Throw_vektor[2]), 2))) *2 * glgraphics.speed_time;
                glgraphics.ball1.SetNewPosition(0, 0, 0);
                glgraphics.ball1.Throw(glgraphics.global_time, Throw_vektor[0], Throw_vektor[1], Throw_vektor[2]);

                glgraphics.Throw_flag = false;
                glgraphics.speed_time = 0;
            }
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 32: //probel
                    // устанавливаем новые координаты мяча 
                    glgraphics.ball1.SetNewPosition(0, 0, 0);
                    // активируем сам бросок
                    glgraphics.ball1.Throw(glgraphics.global_time, 0f, -10f, -1f);
                    break;

            }
        }
    }
}
