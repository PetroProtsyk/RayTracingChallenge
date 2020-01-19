using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using Protsyk.RayTracer.Challenge.Core.Canvas;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Cameras;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;
using Protsyk.RayTracer.Challenge.Core.Scene.Lights;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using static Protsyk.RayTracer.Challenge.Core.Geometry.Vectors;
using static Protsyk.RayTracer.Challenge.ConsoleUtil.Figures;

namespace Protsyk.RayTracer.Challenge.ConsoleUtil
{
    public static class Figures
    {
        public static IFigure S(double cx, double cy, double cz, double radius, IMaterial material)
        {
            return S(P(cx, cy, cz), radius, material);
        }

        public static IFigure S(Tuple4 center, double radius, IMaterial material)
        {
            return new SphereFigure(center, radius, material);
        }

        public static ILight L(double x, double y, double z, double intensity)
        {
            return new SpotLight(P(x, y, z), intensity);
        }

        public static ILight A(double intensity)
        {
            return new AmbientLight(intensity);
        }

        public static ILight D(double dx, double dy, double dz, double intensity)
        {
            return new DirectionLight(V(dx, dy, dz), intensity);
        }
    }

    class Program
    {
        static (ICamera camera, BaseScene scene) ScenePetro()
        {
            // Camera
            var origin = P(10, 5,0);
            var fov = Math.PI/3;
            var camera = new FovCamera(origin, fov, 1920, 1080);

            // Scene
            var materials = new IMaterial[]{
                new SolidColorMaterial(P(250, 75, 75), 100),
                new SolidColorMaterial(P(75, 250, 75), 100),
                new SolidColorMaterial(P(75, 75, 250), 100),
                new SolidColorMaterial(P(250, 250, 75), 1000)
            };

            var scene = new BaseScene().WithFigures(
                               S(10  ,5  , 70, 50  , materials[3]),

                               // P
                               S(0  , 1  , 16, 1   , materials[1]),
                               S(0  , 1  , 16, 1   , materials[1]),
                               S(0  , 3  , 16, 1   , materials[1]),
                               S(0  , 5  , 16, 1   , materials[1]),
                               S(0  , 7  , 16, 1   , materials[1]),
                               S(0  , 9  , 16, 1   , materials[1]),
                               S(2  , 5  , 16, 1   , materials[1]),
                               S(3.5, 6  , 16, 0.5 , materials[1]),
                               S(3.5, 7  , 16, 0.5 , materials[1]),
                               S(3.5, 8  , 16, 0.5 , materials[1]),
                               S(2  , 9  , 16, 1   , materials[1]),

                               S(5  , 1  , 16, 0.5 , materials[2]),
                               S(5  , 2  , 16, 0.5 , materials[2]),
                               S(5  , 3  , 16, 0.5 , materials[2]),
                               S(5  , 4  , 16, 0.5 , materials[2]),
                               S(5  , 5  , 16, 0.5 , materials[2]),
                               S(6.0, 0.5, 16, 0.5 , materials[2]),
                               S(7.0, 0.5, 16, 0.5 , materials[2]),
                               S(6.0, 5  , 16, 0.5 , materials[2]),
                               S(7.0, 5  , 16, 0.5 , materials[2]),
                               S(6.0, 3  , 16, 0.5 , materials[2]),
                               S(7.0, 4  , 16, 0.5 , materials[2]),

                               // t
                               S(10 , 1  , 16, 0.5 ,  materials[0]),
                               S(10 , 2  , 16, 0.5 ,  materials[0]),
                               S(10 , 3  , 16, 0.5 ,  materials[0]),
                               S(10 , 4  , 16, 0.5 ,  materials[0]),
                               S( 9 , 4.5, 16, 0.5 ,  materials[0]),
                               S(11 , 4.5, 16, 0.5 ,  materials[0]),
                               S(10 , 5  , 16, 0.5 ,  materials[0]),
                               S(10 , 6  , 16, 0.5 ,  materials[0]),
                               S(11 , 0.5, 16, 0.5 ,  materials[0]),
                               S(12 , 0.5, 16, 0.5 ,  materials[0]),

                               // r
                               S(14  , 1   , 16, 0.5 ,  materials[1]),
                               S(14  , 2   , 16, 0.5 ,  materials[1]),
                               S(14  , 3   , 16, 0.5 ,  materials[1]),
                               S(14  , 4   , 16, 0.5 ,  materials[1]),
                               S(14  , 5   , 16, 0.5 ,  materials[1]),
                               S(15  , 4.75, 16, 0.5 ,  materials[1]),
                               S(15.5, 5.15, 16, 0.5 ,  materials[1]),
                               S(16.5, 4.75, 16, 0.5 ,  materials[1]),

                               // o
                               S(18 , 2  , 16, 0.5 ,  materials[2]),
                               S(18 , 3  , 16, 0.5 ,  materials[2]),
                               S(18 , 4  , 16, 0.5 ,  materials[2]),
                               S(19 , 1  , 16, 0.5 ,  materials[2]),
                               S(19 , 5  , 16, 0.5 ,  materials[2]),
                               S(20 , 2  , 16, 0.5 ,  materials[2]),
                               S(20 , 3  , 16, 0.5 ,  materials[2]),
                               S(20 , 4  , 16, 0.5 ,  materials[2])

                            ).WithLights(
                               L(20, 20, 10, 50),
                               L(0, 0, 10, 75)
                            );

           return (camera, scene);
        }

        // https://m.habr.com/ru/post/342510/
        static (ICamera camera, BaseScene scene) SceneSpheres(bool useFov)
        {
            // Camera
            var origin = P(0,0,0);
            ICamera camera;
            if (useFov)
            {
                var fov = Math.PI/3;
                camera = new FovCamera(origin, fov, 1920, 1080);
            }
            else
            {
                camera = new ParCamera(origin, 1.0, 600, 600);
            }

            // Scene
            var materials = new IMaterial[]{
                new SolidColorMaterial(P(255, 0, 0), 500),
                new SolidColorMaterial(P(0, 0, 255), 500),
                new SolidColorMaterial(P(0, 255, 0), 10),
                new SolidColorMaterial(P(255, 255, 0), 1000)
            };

            var scene = new BaseScene().WithFigures(
                               S(0  , -1  ,  3, 1   , materials[0]),
                               S(2  ,  0  ,  4, 1   , materials[1]),
                               S(-2 ,  0  ,  4, 1   , materials[2]),

                               S(0  ,-5001  , 0, 5000  , materials[3])
                            ).WithLights(
                               A(20),
                               L(2, 1, 0, 60),
                               D(1, 4, 4, 20)
                            );
            return (camera, scene);
        }

        static (ICamera camera, BaseScene scene) ScenePlanets()
        {
            var origin = P(0,0,-5);
            var fov = Math.PI/3;
            var camera = new FovCamera(origin, fov, 1920, 1080);

            // Scene
            var materials = new IMaterial[]{
                new SolidColorMaterial(P(155, 200, 155), 500),
                new SolidColorMaterial(P(155, 155, 155), MaterialConstants.NoShine),
                new SolidColorMaterial(P(255, 255, 0), MaterialConstants.NoShine)
            };

            var scene = new BaseScene().WithFigures(
                               S(0  ,  0  ,  3, 2.5  , materials[0]),
                               S(-3  , 3  ,  1, 1  , materials[1]),
                               S(-4 ,  -1  , 1, 0.5  , materials[2])
                            ).WithLights(
                               A(30),
                               L(0, 0, -25, 70)
                            );
            return (camera, scene);
        }

        static ICanvas Render(ICamera camera, BaseScene scene)
        {
            var canvas = new MemoryCanvas((int)camera.ScreenWidth, (int)camera.ScreenHeight);
            var x = 0; var y =0;
            for (double j = 0; j < camera.ScreenHeight; ++j)
            {
                x = 0;
                for (double i = 0; i < camera.ScreenWidth; ++i)
                {
                    var direction = camera.GetDirection(i, j);
                    var color = scene.CastRay(camera.Origin, direction);
                    canvas.SetPixel(x, y, ColorConverters.Tuple255.From(color));
                    ++x;
                }
                ++y;
            }
            return canvas;
        }

        static void Main(string[] args)
        {
            // Camera, Scene
            var (camera, scene) = SceneSpheres(false);

            var timer = Stopwatch.StartNew();
            var canvas = Render(camera, scene);
            Console.WriteLine($"Time to render: {timer.Elapsed}");

            timer.Restart();
            using (var file = File.OpenWrite("out.ppm"))
            {
               CanvasConverters.PPM_P6.Convert(canvas, file);
            }
            Console.WriteLine($"Time to output: {timer.Elapsed}");
        }
    }
}