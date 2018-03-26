using ParticleFilter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ParticleFilterDemo
{
    public partial class Form1 : Form
    {
        private ParticleFilter2D _filter;
        private Point _previousPosition = new Point(0,0);
        private int _predictionScale = 10; //scale factor to get more suitable visualization


        public Form1()
        {
            InitializeComponent();

            _filter = new ParticleFilter2D();
            _filter.Particles = Helper.GenerateParticles(200, pictureBox1.Height, pictureBox1.Height);
                       
        }




        private void _drawParticles(Graphics g)
        {

            for(int i = 0; i < _filter.Particles.Count; i++)
            {
                var X = _previousPosition.X + (int)_filter.Particles[i].X;
                var Y = _previousPosition.Y + (int)_filter.Particles[i].Y;

                GraphicHelper.DrawCross(g, new Point(X,Y), Pens.LightBlue, 2);
            }
        }






        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_previousPosition.X == 0 && _previousPosition.Y == 0)
                _previousPosition = e.Location;

            if (pictureBox1.Image == null)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                }
                pictureBox1.Image = bmp;
            }
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.Clear(Color.White);

                GraphicHelper. DrawCross(g, e.Location, Pens.Red, 3);

                _filter.Predict(0.9f);
                _filter.Update(  new PointF((e.X - _previousPosition.X) * _predictionScale,
                                        (e.Y - _previousPosition.Y) * _predictionScale));
                _drawParticles(g);
            }
            pictureBox1.Invalidate();
            _previousPosition = e.Location;
        }

      
    }


    public static class GraphicHelper
    {

        public static void DrawCross(Graphics graphics, Point point, Pen pen, int size)
        {
            graphics.DrawLine(pen, new Point(point.X - size, point.Y - size), new Point(point.X + size, point.Y + size));
            graphics.DrawLine(pen, new Point(point.X - size, point.Y + size), new Point(point.X + size, point.Y - size));
        }
    }
}
