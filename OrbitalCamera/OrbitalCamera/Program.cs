using Aiv.Fast3D;
using Aiv.Fast2D;
using OpenTK;

namespace OrbitalCamera
{
    internal class Program
    {
        public static Window Window;

        static void Main(string[] args)
        {
            Window = new Window(800, 600, "Orbital Camera");
            Window.SetVSync(false);
            Window.SetClearColor(128, 128, 128);
            Window.EnableDepthTest();
            Window.CullBackFaces();

            Player player = new Player();

            Plane plane = new Plane();
            GeneratePlane(plane);

            Material planeMat = new Material();
            planeMat.Diffuse = new Texture("Assets/cartoon_grass.png");
            planeMat.SpecularMap = new Texture("Assets/cartoon_grass_spec.jpg");

            AddMaterialOnPlane(planeMat, plane);

            while (Window.IsOpened)
            {
                Window.SetTitle("FPS: " + (int)(1 / Window.DeltaTime));

                //INPUT
                player.Input();

                //UPDATE
                player.Update();

                //DRAW
                //bulbasaur.DrawPhong(material);
                plane.DrawPhong(planeMat);
                player.Ivysaur.Draw();

                Window.Update();
            }
        }

        static void GeneratePlane(Plane plane)
        {
            int scalePlane = 50;
            plane.Position3 = Vector3.Zero;
            plane.EulerRotation3 = new Vector3(-90, 0, 0);
            plane.Scale3 = new Vector3(scalePlane);

            for (int i = 0; i < plane.uv.Length; i++)
            {
                plane.uv[i] *= scalePlane;
            }

            plane.UpdateUV();
        }

        static void AddMaterialOnPlane(Material material, Plane plane)
        {
            material.Diffuse.SetRepeatX();
            material.Diffuse.SetRepeatY();
            material.SpecularMap.SetRepeatX();
            material.SpecularMap.SetRepeatY();
            material.Lights = new Light[] { new DirectionalLight(new Vector3(-0.25f, -1f, 0)) };
            material.Ambient = new Vector3(0.2f);        
        }

    }
}
