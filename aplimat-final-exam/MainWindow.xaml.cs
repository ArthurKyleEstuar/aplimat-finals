using aplimat_final_exam.Models;
using aplimat_final_exam.Utilities;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace aplimat_final_exam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Vector3 mousePos = new Vector3();

        #region Initialization
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_LIGHT0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        #endregion

        #region Mouse Func
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
        }
        #endregion

        
        private void ManageKeyPress()
        {

        }

        private CubeMesh myCube = new CubeMesh()
        {
            Position = new Vector3(-50,-15,0),
            Scale = new Vector3(1,1,1)
        };

        private CubeMesh target = new CubeMesh()
        {
            Position = new Vector3((float)Randomizer.Gaussian(0, 20), (float)Randomizer.Generate(0, 30), 0),
            Scale = new Vector3((float)Randomizer.Generate(1, 5), (float)Randomizer.Generate(1, 5), 0)
        };
        private float speed = 0.1f;
        private float bulletSpeed = 3.0f;
        private int airFrame = 0;

        private Vector3 shotPower = new Vector3(0,0,0);
        private bool onTheWay = false;
        private Vector3 Wind = new Vector3((float)Randomizer.Generate(-0.1, 0.1), 0, 0);
        private List<CubeMesh> bullets = new List<CubeMesh>();

        private CubeMesh bullet = new CubeMesh()
        {
            Position = new Vector3(0, 0, 0)
        };
        private void ResetProjectile()
        {
            bullet.Position = myCube.Position;
            bullet.Velocity = new Vector3();
            bullet.Acceleration = new Vector3();
            onTheWay = false;
            airFrame = 0;
        }
        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            this.Title = "APLIMAT Final Exam";
            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move Left And Into The Screen
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -100.0f);

            gl.Color(1.0, 1.0, 1.0);
            bullet.Draw(gl);

            gl.Color(0.0, 0.0, 1.0);
            myCube.Draw(gl);

            gl.Color(1.0, 0.0, 0.0);
            target.Draw(gl);

            if(!onTheWay)
            {
                bullet.Position = myCube.Position;
            }

            if (Keyboard.IsKeyDown(Key.W))
            {
                shotPower.y += 0.1f;
                shotPower.Clamp(5, 5, 0);
            }

            if (Keyboard.IsKeyDown(Key.D))
            {
                shotPower.x += 0.1f;
                shotPower.Clamp(5, 5, 0);
            }

            if (Keyboard.IsKeyDown(Key.A))
            {
                shotPower.x -= 0.1f;
                shotPower.ClampMin(-5, -5, 0);
            }
            if (Keyboard.IsKeyDown(Key.S))
            {
                shotPower.y -= 0.1f;
                shotPower.ClampMin(-5, -5, 0);
            }

            
            if (airFrame > 120)
            {
                ResetProjectile();
            }

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
               
            }

            if(Keyboard.IsKeyDown(Key.Space))
            {
                onTheWay = true;
                bullet.ApplyForce(shotPower);
            }

            if(onTheWay)
            {
                bullet.ApplyGravity();
                bullet.ApplyForce(Wind);
                bullet.BounceYBottom(myCube.Position.y);
                bullet.BounceXLeft(-80);
                bullet.BounceXRight(80);
                airFrame++;
            }

            if(bullet.Position.y <= myCube.Position.y)
            {
                bullet.ApplyFriction(0.5f,1);
            }

            if(target.isColliding(bullet))
            {
                target.Position = new Vector3((float)Randomizer.Gaussian(0, 20), (float)Randomizer.Generate(0, 30), 0);
                target.Scale = new Vector3((float)Randomizer.Generate(1, 5), (float)Randomizer.Generate(1, 5), 0);
            }
            

            gl.DrawText(20, 20, 1, 0, 0, "Arial", 25, "" + "Firing solution X: " + shotPower.x + " | Y: " + shotPower.y);
            gl.DrawText(20, 45, 1, 0, 0, "Arial", 25, "" + "Loaded: " + !onTheWay);
            gl.DrawText(20, 700, 1, 0, 0, "Arial", 25, "" + "Wind Speed: " + Wind.x + " kph");

            //myCube.ApplyGravity();

            //if (myCube.Position.y <= -50)
            //{
            //    myCube.Position.y = -50;
            //    myc
            //}
        }
    }
}
