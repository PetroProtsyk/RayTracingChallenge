using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Canvas;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Cameras;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;
using Protsyk.RayTracer.Challenge.Core.Scene.Lights;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Geometry.SignedDistanceFields;
using static Protsyk.RayTracer.Challenge.Core.Geometry.Vectors;
using static Protsyk.RayTracer.Challenge.ConsoleUtil.Figures;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials.Patterns;
using Protsyk.RayTracer.Challenge.Core.FileParser;

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

        public static IFigure S(IMatrix transformation, IMaterial material)
        {
            return new SphereFigure(transformation, material);
        }

        public static ILight L(double x, double y, double z, double intensity)
        {
            return new SpotLight(ColorModel.WhiteRGB, P(x, y, z), intensity);
        }

        public static ILight A(double intensity)
        {
            return new AmbientLight(intensity);
        }

        public static ILight D(double dx, double dy, double dz, double intensity)
        {
            return new DirectionLight(ColorModel.WhiteRGB, V(dx, dy, dz), intensity);
        }
    }

    class Program
    {
        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneWeb()
        {
            // Camera
            var origin = P(10, 5, 0);
            var fov = Math.PI / 3;
            var camera = new FovCamera(origin, fov, 320, 240);

            // Scene
            var materials = new IMaterial[]{
                SolidColorMaterial.fromColorAndShininess(P(250, 75, 75), 100),
                SolidColorMaterial.fromColorAndShininess(P(75, 250, 75), 100),
                SolidColorMaterial.fromColorAndShininess(P(75, 75, 250), 100),
                SolidColorMaterial.fromColorAndShininess(P(250, 250, 75), 1000)
            };
            var scene = new BaseScene().WithFigures(
                               S(10  ,5  , 16, 5  , materials[3]),
                               S(5  , 5  , 16, 3  , materials[1]),
                               S(15 , 5  , 16, 3  , materials[2]),
                               S(10 ,10  , 16, 3  , materials[0])
                            ).WithLights(
                               L(10, 3, 0, 0.75)
                            );

            return (camera, scene, ColorConverters.Tuple255);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) ScenePetro()
        {
            // Camera
            var origin = P(10, 5, -5);
            var fov = Math.PI / 3;
            var camera = new FovCamera(origin, fov, 1920, 1080);

            // Scene
            var materials = new IMaterial[]{
                SolidColorMaterial.fromColorAndShininess(P(250, 75, 75), 100),
                SolidColorMaterial.fromColorAndShininess(P(75, 250, 75), 100),
                SolidColorMaterial.fromColorAndShininess(P(75, 75, 250), 100),
                SolidColorMaterial.fromColorAndShininess(P(250, 250, 75), 1000)
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
                               L(20, 20, 10, 0.50),
                               L(0, 0, 10, 0.75)
                            );

            return (camera, scene, ColorConverters.Tuple255);
        }

        // https://m.habr.com/ru/post/342510/
        // https://github.com/ggambetta/computer-graphics-from-scratch
        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneSpheres(bool useFov, bool useRotation)
        {
            // Camera
            var origin = useRotation ? P(3, 0, 1) : P(0, 0, 0);
            ICamera camera;
            if (useFov)
            {
                var fov = Math.PI / 3;
                var rotation = useRotation ? new Matrix4x4(new double[]
                {
                    0.7071, 0, -0.7071, 0,
                    0,      1,       0, 0,
                    0.7071, 0,  0.7071, 0,
                    0,      0,       0, 0
                }) : Matrix4x4.Identity;
                camera = useRotation ? (ICamera)new FovCamera(origin, rotation, fov, 1920, 1080) :
                                       (ICamera)new FovCamera2(MatrixOperations.Geometry3D.RotateY(Math.PI), fov, 1920, 1080);
            }
            else
            {
                camera = new SimpleCamera(origin, 1.0, 600, 600);
            }

            // Scene
            var materials = new IMaterial[]{
                SolidColorMaterial.fromColorShininessReflective(P(255, 0, 0), 500, 0.2),
                SolidColorMaterial.fromColorShininessReflective(P(0, 0, 255), 500, 0.3),
                SolidColorMaterial.fromColorShininessReflective(P(0, 255, 0), 10, 0.4),
                SolidColorMaterial.fromColorShininessReflective(P(255, 255, 0), 1000, 0.5)
            };

            var scene = new BaseScene().WithFigures(
                               S(0  , -1  ,  3, 1   , materials[0]),
                               S(2  ,  0  ,  4, 1   , materials[1]),
                               S(-2 ,  0  ,  4, 1   , materials[2]),

                               S(0  ,-5001  , 0, 5000  , materials[3])
                            ).WithLights(
                               A(0.2),
                               L(2, 1, 0, 0.6),
                               D(1, 4, 4, 0.2)
                            );

            return (camera, scene, ColorConverters.Tuple255);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) ScenePlanets()
        {
            var origin = P(0, 0, -5);
            var fov = Math.PI / 3;
            var camera = new FovCamera(origin, fov, 1920, 1080);

            // Scene
            var materials = new IMaterial[]{
                SolidColorMaterial.fromColorAndShininess(P(155, 200, 155), 500),
                SolidColorMaterial.fromColorAndShininess(P(155, 155, 155), MaterialConstants.NoShine),
                SolidColorMaterial.fromColorAndShininess(P(255, 255, 0), MaterialConstants.NoShine)
            };

            var scene = new BaseScene().WithFigures(
                               S(0  ,  0  ,  3, 2.5  , materials[0]),
                               S(-3  , 3  ,  1, 1  , materials[1]),
                               S(-4 ,  -1  , 1, 0.5  , materials[2])
                            ).WithLights(
                               A(0.30),
                               L(0, 0, -25, 0.70)
                            );

            return (camera, scene, ColorConverters.Tuple255);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneSpheresTransformed(bool useFov)
        {
            // Camera
            var origin = P(0, 0, 0);
            ICamera camera;
            if (useFov)
            {
                var fov = Math.PI / 3;
                camera = new FovCamera(origin, fov, 1920, 1080);
            }
            else
            {
                camera = new SimpleCamera(origin, 1.0, 600, 600);
            }

            // Scene
            var materials = new IMaterial[]{
                SolidColorMaterial.fromColorAndShininess(P(255, 0, 0), 500),
                SolidColorMaterial.fromColorAndShininess(P(0, 0, 255), 500),
                SolidColorMaterial.fromColorAndShininess(P(0, 255, 0), 10),
                SolidColorMaterial.fromColorAndShininess(P(255, 255, 0), 1000)
            };

            var scene = new BaseScene().WithFigures(
                                 S(MatrixOperations.Multiply(MatrixOperations.Geometry3D.Translation(0, -1.0, 3.0),
                                   MatrixOperations.Multiply(MatrixOperations.Geometry3D.Shearing(1, 0, 0, 0, 0, 0),
                                                             MatrixOperations.Geometry3D.Scale(0.5, 1, 1))), materials[0]),
                                 S(MatrixOperations.Geometry3D.Translation(2, 0.0, 4.0), materials[1]),
                                 S(MatrixOperations.Multiply(MatrixOperations.Geometry3D.Translation(-1.0, 0.0, 4.0),
                                                             MatrixOperations.Geometry3D.Scale(1.0, 0.5, 1.2)), materials[2]),

                                 S(0, -5001, 0, 5000, materials[3])
                            ).WithLights(
                               A(0.20),
                               L(2, 1, 0, 0.60)
                            );

            return (camera, scene, ColorConverters.Tuple255);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter5(bool isSimple)
        {
            // Camera
            ICamera camera;
            if (isSimple)
            {
                var origin = P(0, 0, -Math.Sqrt(2.0));
                camera = new SimpleCamera(origin, 2.0, 600, 600);
            }
            else
            {
                camera = new ParCamera(-5.0, 2.0, 600, 600);
            }

            // Scene
            var materials = new IMaterial[]{
                new SolidColorMaterial(P(255, 0, 0), 1.0, 1.0, 1.0, 1000, 1.0, 1.0, 0.0)
            };

            var transformation =
                    MatrixOperations.Multiply(MatrixOperations.Geometry3D.Shearing(0.3, 0, 0, 0, 0, 0),
                                              MatrixOperations.Geometry3D.Scale(0.5, 1, 1));
                    //MatrixOperations.Identity(4);

            var scene = new BaseScene().WithFigures(
                                 S(transformation, materials[0])
                            ).WithLights(
                               A(1.0)
                            );

            return (camera, scene, ColorConverters.Tuple255);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter7(bool castShadows)
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 1.5, -5),
                                            P(0, 1, 0),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var defaultMaterial = MaterialConstants.Default;
            var floorMaterial = new SolidColorMaterial(
                                        new Tuple4(1, 0.9, 0.9, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        defaultMaterial.Diffuse,
                                        0,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var middleMaterial = new SolidColorMaterial(
                                        new Tuple4(0.1, 1, 0.5, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        0.7,
                                        0.3,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var rightMaterial = new SolidColorMaterial(
                                        new Tuple4(0.5, 1, 0.1, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        0.7,
                                        0.3,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var leftMaterial = new SolidColorMaterial(
                                        new Tuple4(1, 0.8, 0.1, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        0.7,
                                        0.3,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            // World
            var scene = new BaseScene().WithFigures(
                                 S(MatrixOperations.Geometry3D.Scale(10, 0.01, 10), floorMaterial), // floor
                                 S(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(0, 0, 5),
                                     MatrixOperations.Multiply(
                                         MatrixOperations.Multiply(
                                             MatrixOperations.Geometry3D.RotateY(-Math.PI / 4),
                                             MatrixOperations.Geometry3D.RotateX(Math.PI / 2)),
                                         MatrixOperations.Geometry3D.Scale(10, 0.01, 10))), floorMaterial), // left wall
                                 S(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(0, 0, 5),
                                     MatrixOperations.Multiply(
                                         MatrixOperations.Multiply(
                                             MatrixOperations.Geometry3D.RotateY(Math.PI / 4),
                                             MatrixOperations.Geometry3D.RotateX(Math.PI / 2)),
                                         MatrixOperations.Geometry3D.Scale(10, 0.01, 10))), floorMaterial), // right wall
                                 S(MatrixOperations.Geometry3D.Translation(-0.5, 1, 0.5), middleMaterial), // middle sphere
                                 S(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(1.5, 0.5, -0.5),
                                     MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5)), rightMaterial), // right sphere
                                 S(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(-1.5, 0.33, -0.75),
                                     MatrixOperations.Geometry3D.Scale(0.33, 0.33, 0.33)), leftMaterial) // left sphere
                            ).WithLights(
                               L(-10, 10, -10, 1),
                               A(1.0)
                            ).WithShadows(castShadows);

            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter9()
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 1.5, -5),
                                            P(0, 1, 0),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var defaultMaterial = MaterialConstants.Default;
            var floorMaterial = new SolidColorMaterial(
                                        new Tuple4(1, 0.9, 0.9, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        defaultMaterial.Diffuse,
                                        0,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var middleMaterial = new SolidColorMaterial(
                                        new Tuple4(0.1, 1, 0.5, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        0.7,
                                        0.3,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var rightMaterial = new SolidColorMaterial(
                                        new Tuple4(0.5, 1, 0.1, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        0.7,
                                        0.3,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var leftMaterial = new SolidColorMaterial(
                                        new Tuple4(1, 0.8, 0.1, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        0.7,
                                        0.3,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            // World
            var scene = new BaseScene().WithFigures(
                                 new PlaneFigure(MatrixOperations.Identity(4), floorMaterial), // floor
                                 S(MatrixOperations.Geometry3D.Translation(-0.5, 1, 0.5), middleMaterial), // middle sphere
                                 S(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(1.5, 0.5, -0.5),
                                     MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5)), rightMaterial), // right sphere
                                 S(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(-1.5, 0.33, -0.75),
                                     MatrixOperations.Geometry3D.Scale(0.33, 0.33, 0.33)), leftMaterial) // left sphere
                            ).WithLights(
                               L(-10, 10, -10, 1),
                               A(1.0)
                            ).WithShadows(true);

            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter10()
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 1.5, -5),
                                            P(0, 1, 0),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var defaultMaterial = MaterialConstants.Default;
            var floorMaterial = new PatternMaterial(
                                        new StripePattern(new Tuple4(0.2, 0.3, 0.8, TupleFlavour.Vector), new Tuple4(1, 0.9, 0.9, TupleFlavour.Vector)),
                                        defaultMaterial.Ambient,
                                        defaultMaterial.Diffuse,
                                        0,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var middleMaterial = new PatternMaterial(
                                        new CheckerPattern(MatrixOperations.Multiply(
                                                            MatrixOperations.Geometry3D.RotateX(Math.PI / 2),
                                                            MatrixOperations.Geometry3D.Scale(0.25, 0.25, 0.25)),
                                                          Tuple4.Vector(0.1, 1, 0.5),
                                                          Tuple4.Vector(1, 0.9, 0.9),
                                                          true),
                                        defaultMaterial.Ambient,
                                        0.7,
                                        0.3,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var rightMaterial = new PatternMaterial(
                                        new StripePattern(MatrixOperations.Multiply(
                                                            MatrixOperations.Geometry3D.RotateZ(Math.PI / 3),
                                                            MatrixOperations.Geometry3D.Scale(0.25, 2, 2)),
                                                            new Tuple4(1, 0.5, 0.1, TupleFlavour.Vector),
                                                            new Tuple4(1, 0.9, 0.9, TupleFlavour.Vector)),
                                        defaultMaterial.Ambient,
                                        0.7,
                                        0.3,
                                        defaultMaterial.Shininess,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var leftMaterial = new PatternMaterial(
                                                    new GradientPattern(MatrixOperations.Geometry3D.RotateZ(Math.PI / 6),
                                                                        new Tuple4(1, 0.8, 0.1, TupleFlavour.Vector),
                                                                        new Tuple4(1, 0.9, 0.9, TupleFlavour.Vector)),
                                                    defaultMaterial.Ambient,
                                                    0.7,
                                                    0.3,
                                                    defaultMaterial.Shininess,
                                                    defaultMaterial.Reflective,
                                                    defaultMaterial.RefractiveIndex,
                                                    defaultMaterial.Transparency);

            // World
            var scene = new BaseScene().WithFigures(
                                 new PlaneFigure(MatrixOperations.Identity(4), floorMaterial), // floor
                                 new PlaneFigure(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(0.0, 0.0, 4.0),
                                     MatrixOperations.Geometry3D.RotateX(Math.PI / 2)), floorMaterial), // floor
                                 S(MatrixOperations.Geometry3D.Translation(-0.5, 1, 0.5), middleMaterial), // middle sphere
                                 S(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(1.5, 0.5, -0.5),
                                     MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5)), rightMaterial), // right sphere
                                 S(MatrixOperations.Multiply(
                                     MatrixOperations.Geometry3D.Translation(-1.5, 0.33, -0.75),
                                     MatrixOperations.Geometry3D.Scale(0.33, 0.33, 0.33)), leftMaterial) // left sphere
                            ).WithLights(
                               L(-10, 10, -10, 1),
                               A(1.0)
                            ).WithShadows(true);

            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter11()
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 1.0, -6),
                                            P(0, 1, 0),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var defaultMaterial = MaterialConstants.Default;
            var floorMaterial = new PatternMaterial(
                                        new CheckerPattern(MatrixOperations.Geometry3D.RotateY(Math.PI / 3),
                                                           Tuple4.Vector(0, 0, 0),
                                                           Tuple4.Vector(0.4, 0.4, 0.4),
                                                           false),
                                        defaultMaterial.Ambient,
                                        defaultMaterial.Diffuse,
                                        0,
                                        defaultMaterial.Shininess,
                                        0.7,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var middleMaterial = new SolidColorMaterial(
                                        new Tuple4(0.31, 0.03, 0.027, TupleFlavour.Vector),
                                        defaultMaterial.Ambient,
                                        0.9,
                                        0.001,
                                        10,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            // World
            var scene = new BaseScene().WithFigures(
                                 new PlaneFigure(MatrixOperations.Identity(4), floorMaterial), // floor
                                 S(MatrixOperations.Geometry3D.Translation(-0.5, 1, 1.7), middleMaterial) // middle sphere
                            ).WithLights(
                               L(-10, 10, -10, 0.9),
                               A(1.0)
                            ).WithShadows(true);

            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter14()
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 2.0, -2),
                                            P(0, 1, -1),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var defaultMaterial = MaterialConstants.Default;
            var middleMaterial = new SolidColorMaterial(
                                        Tuple4.Vector(1.0, 1.0, 1.0),
                                        defaultMaterial.Ambient,
                                        0.9,
                                        0.001,
                                        10,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            // World
            var scene = new BaseScene().WithFigures(
                                 HexagonFigure.CreateHexagon(MatrixOperations.Multiply(
                                                                MatrixOperations.Geometry3D.RotateY(Math.PI / 9),
                                                                MatrixOperations.Geometry3D.RotateZ(Math.PI / 6)), middleMaterial)
                            ).WithLights(
                               L(-10, 10, -10, 0.9),
                               A(1.0)
                            ).WithShadows(true);

            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter15(string file)
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 7.0, -20),
                                            P(0, 1, -1),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            var pot = WavefrontObjParser.FromFile(Path.Combine(AppContext.BaseDirectory, file)).ToFigure();
            pot.Transformation = MatrixOperations.Multiply(MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5),
                                                           MatrixOperations.Geometry3D.RotateX(-Math.PI / 1.8));

            // World
            var scene = new BaseScene().WithFigures(
                               pot
                            ).WithLights(
                               L(-10, 10, -10, 1),
                               A(1.0)
                            ).WithShadows(false);

            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) GlassSpheres()
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 0, -5),
                                            P(0, 0, 0),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var defaultMaterial = MaterialConstants.Default;

            var floorMaterial = new PatternMaterial(
                                        new CheckerPattern(MatrixOperations.Geometry3D.Scale(0.3, 0.3, 0.3),
                                                           Tuple4.Vector(0, 0, 0),
                                                           Tuple4.Vector(0.7, 0.7, 0.7),
                                                           false),
                                        1.0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1.0,
                                        0.0);

            var greenMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0, 0.3, 0),
                                        0.1,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.3,
                                        0.7);

            var blueMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0, 0, 0.3),
                                        0.1,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.2,
                                        0.7);

            var redMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0.3, 0, 0),
                                        0.1,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.1,
                                        0.7);

            // World
            var scene = new BaseScene().WithFigures(
                                 new PlaneFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.RotateX(Math.PI / 2),
                                                                           MatrixOperations.Geometry3D.Translation(0, 0, 15)), floorMaterial), // floor
                                 new SphereFigure(MatrixOperations.Geometry3D.Translation(-0.5, 0.5, 0), blueMaterial),
                                 new SphereFigure(MatrixOperations.Geometry3D.Translation(0.5, 0.5, 0), greenMaterial),
                                 new SphereFigure(MatrixOperations.Geometry3D.Translation(0, -0.5, 0), redMaterial)
                            ).WithLights(
                               L(-10, 10, -10, 0.9),
                               A(1.0)
                            ).WithShadows(true);

            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter16()
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 0, -5),
                                            P(0, 0, 0),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var floorMaterial = new PatternMaterial(
                                        new CheckerPattern(MatrixOperations.Geometry3D.Scale(0.3, 0.3, 0.3),
                                                           Tuple4.Vector(0, 0, 0),
                                                           Tuple4.Vector(0.7, 0.7, 0.7),
                                                           false),
                                        1.0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1.0,
                                        0.0);

            var greenMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0, 0.3, 0),
                                        0.1,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.3,
                                        0.7);

            var blueMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0, 0, 0.3),
                                        0.1,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.2,
                                        0.7);

            var redMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0.3, 0, 0),
                                        0.1,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.1,
                                        0.7);

            // World
            var scene = new BaseScene().WithFigures(
                                 new PlaneFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.RotateX(Math.PI / 2),
                                                                           MatrixOperations.Geometry3D.Translation(0, 0, 15)), floorMaterial), // floor
                                 new CSGFigure("union",
                                    new SphereFigure(MatrixOperations.Geometry3D.Translation(-0.5, 0.5, 0), blueMaterial),
                                    new CSGFigure("union", new SphereFigure(MatrixOperations.Geometry3D.Translation(0.5, 0.5, 0), greenMaterial),
                                                           new SphereFigure(MatrixOperations.Geometry3D.Translation(0, -0.5, 0), redMaterial)))
                            ).WithLights(
                               L(-10, 10, -10, 0.9),
                               A(1.0)
                            ).WithShadows(true);

            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneChapter16_1()
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 0, -5),
                                            P(0, 0, 0),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var defaultMaterial = MaterialConstants.Default;

            var floorMaterial = new PatternMaterial(
                                        new CheckerPattern(MatrixOperations.Geometry3D.Scale(0.3, 0.3, 0.3),
                                                           Tuple4.Vector(0, 0, 0),
                                                           Tuple4.Vector(0.7, 0.7, 0.7),
                                                           false),
                                        1.0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        1.0,
                                        0.0);

            var redMaterial = new SolidColorMaterial(
                                        Tuple4.Point(1, 0.3, 0.2),
                                        defaultMaterial.Ambient,
                                        0.9,
                                        0.001,
                                        10,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            // World
            var scene = new BaseScene().WithFigures(
                                 new PlaneFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.RotateX(Math.PI / 2),
                                                                           MatrixOperations.Geometry3D.Translation(0, 0, 15)), floorMaterial), // floor
                                 new CSGFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.RotateX(Math.PI / 3),
                                                                         MatrixOperations.Geometry3D.RotateY(Math.PI / 3)),
                                     "intersection",
                                        new SphereFigure(MatrixOperations.Geometry3D.Translation(0, 0, 0), redMaterial),
                                        new CubeFigure(MatrixOperations.Geometry3D.Scale(0.7, 0.7, 0.7), redMaterial))
                            ).WithLights(
                               L(-10, 10, -10, 0.9),
                               A(1.0)
                            ).WithShadows(true);

            return (camera, scene, ColorConverters.Tuple1);
        }

        // This Scene is based on: https://github.com/javan/ray-tracer-challenge/blob/master/src/controllers/chapter_11_worker.js
        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) RelectionRefraction()
        {
            // Camera
            var camera = new FovCamera2(MatrixOperations.Geometry3D.LookAtTransform(
                                            P(0, 2, -7),
                                            P(1, 0, 0),
                                            V(0, 1, 0)), Math.PI / 3, 800, 600);

            // Materials
            var defaultMaterial = MaterialConstants.Default;
            var floorMaterial = new PatternMaterial(
                                        new CheckerPattern(MatrixOperations.Geometry3D.RotateY(Math.PI / 4),
                                                           Tuple4.Vector(0.35, 0.35, 0.35),
                                                           Tuple4.Vector(0.65, 0.65, 0.65),
                                                           false),
                                        defaultMaterial.Ambient,
                                        defaultMaterial.Diffuse,
                                        0.0,
                                        defaultMaterial.Shininess,
                                        0.4,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var redMaterial = new SolidColorMaterial(
                                        Tuple4.Point(1, 0.3, 0.2),
                                        defaultMaterial.Ambient,
                                        defaultMaterial.Diffuse,
                                        0.4,
                                        5.0,
                                        defaultMaterial.Reflective,
                                        defaultMaterial.RefractiveIndex,
                                        defaultMaterial.Transparency);

            var blueMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0, 0, 0.2),
                                        0.0,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.5,
                                        0.9);

            var greenMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0, 0.2, 0),
                                        0.0,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.5,
                                        0.9);

            var glassMaterial = new SolidColorMaterial(
                                        Tuple4.Point(0, 0, 0.3),
                                        0.0,
                                        0.4,
                                        0.9,
                                        300,
                                        0.9,
                                        1.5,
                                        0.9);

            // World
            var scene = new BaseScene().WithFigures(
                                 new PlaneFigure(Matrix4x4.Identity, floorMaterial),
                                 new SphereFigure(MatrixOperations.Geometry3D.Translation(6, 1, 4), redMaterial),
                                 new SphereFigure(MatrixOperations.Geometry3D.Translation(2, 1, 3), redMaterial),
                                 new SphereFigure(MatrixOperations.Geometry3D.Translation(-1, 1, 2), redMaterial),
                                 new SphereFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.Translation(0.6, 0.7, -0.6),
                                                                            MatrixOperations.Geometry3D.Scale(0.7, 0.7, 0.7)), blueMaterial),
                                 new SphereFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.Translation(-0.7, 0.5, -0.8),
                                                                            MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5)), greenMaterial),
                                 new CubeFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.Translation(3, 1.0, 0),
                                                                            MatrixOperations.Geometry3D.Scale(0.5, 0.7, 0.5)), glassMaterial),
                                 new ConeFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.Translation(1.5, 0.0, -3),
                                                                              MatrixOperations.Geometry3D.Scale(0.1, 0.3, 0.1)), redMaterial, 0.0, 4.0, false)
                            ).WithLights(
                              new SpotLight(ColorModel.WhiteNormalized, Tuple4.Point(-4.9, 4.9, -1), 1.0),
                              new AmbientLight(1.0)
                            ).WithShadows(true);


            return (camera, scene, ColorConverters.Tuple1);
        }

        static (ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter) SceneSDE(bool isSimple)
        {
            // Camera
            ICamera camera;
            if (isSimple)
            {
                var origin = P(0, 0, -Math.Sqrt(2.0));
                camera = new SimpleCamera(origin, 2.0, 600, 600);
            }
            else
            {
                camera = new ParCamera(-5.0, 2.0, 600, 600);
            }

            // Scene
            var materials = new IMaterial[]{
                SolidColorMaterial.fromColorAndShininess(P(255, 0, 0), 100),
            };

            var transformationBox =
                     MatrixOperations.Multiply(
                         MatrixOperations.Geometry3D.Scale(0.7, 0.7, 0.7),
                         MatrixOperations.Multiply(
                             MatrixOperations.Geometry3D.RotateX(Math.PI / 6),
                             MatrixOperations.Multiply(
                                                    MatrixOperations.Geometry3D.RotateY(Math.PI / 6),
                                                    MatrixOperations.Geometry3D.RotateZ(Math.PI / 2)))
                      );

            var transformationSphere = MatrixOperations.Geometry3D.Scale(0.75, 0.75, 0.75);

            var transformationSphereA = MatrixOperations.Multiply(
                            MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5),
                            MatrixOperations.Geometry3D.Translation(-0.5, 0.0, 0.0));
            var transformationSphereB = MatrixOperations.Multiply(
                            MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5),
                            MatrixOperations.Geometry3D.Translation(0.5, 0.0, 0.0));

            var scene = new BaseScene().WithFigures(
                                // S(transformationSphere, materials[0])
                                new SDFFigure(
                                    new IntersectSDF(
                                        new SphereSDF(/*transformationSphereA*/),
                                        new SphereSDF(/*transformationSphereB*/)
                                    ),
                                    materials[0]
                                )
                                // new SDFFigure(new BoxSDF(transformationBox), materials[0]),
                                // new SDFFigure(new SphereSDF(transformationSphere), materials[0]),
                                // new SDFFigure(
                                //     new IntersectSDF(
                                //         new BoxSDF(transformationBox),
                                //         new SphereSDF(transformationSphere)
                                //     ),
                                //     materials[0]
                                // )

                            ).WithLights(
                               A(0.2),
                               L(2, 1, -5, 0.6)
                            );

            return (camera, scene, ColorConverters.Tuple255);
        }

        static ICanvas Render(ICamera camera, BaseScene scene, IColorConverter<Tuple4> colorConverter)
        {
            var canvas = new MemoryCanvas((int)camera.ScreenWidth, (int)camera.ScreenHeight);
            var x = 0; var y = 0;
            for (double j = 0; j < camera.ScreenHeight; ++j)
            {
                x = 0;
                for (double i = 0; i < camera.ScreenWidth; ++i)
                {
                    var ray = camera.GetRay(i, j);
                    var color = scene.CastRay(ray);
                    canvas.SetPixel(x, y, colorConverter.From(color));
                    ++x;
                }
                ++y;
            }
            return canvas;
        }

        static void Main(string[] args)
        {
            var scenes = new Dictionary<string, Func<bool, (ICamera, BaseScene, IColorConverter<Tuple4>)>>(){
                { "Web", _ => SceneWeb() },
                { "Petro", _ => ScenePetro() },
                { "Spheres", fov => SceneSpheres(fov, false) },
                { "SpheresRotated", fov => SceneSpheres(fov, true) },
                { "Planets", _ => ScenePlanets() },
                { "SpheresTransformed", fov => SceneSpheresTransformed(fov) },
                { "SDE", isSimple => SceneSDE(isSimple) },

                { "Chapter5", isSimple => SceneChapter5(isSimple) },
                { "Chapter7", _=> SceneChapter7(false) },
                { "Chapter8", _=> SceneChapter7(true) },
                { "Chapter9", _=> SceneChapter9() },
                { "Chapter10", _=> SceneChapter10() },
                { "Chapter11", _=> SceneChapter11() },
                { "Chapter14", _=> SceneChapter14() },
                { "GlassSpheres", _=> GlassSpheres() },
                { "Chapter15", _=> SceneChapter15("teapot-lowtri.obj") },
                { "Chapter15_1", _=> SceneChapter15("teapot-low.obj") },
                { "Chapter16", _=> SceneChapter16() },
                { "Chapter16_1", _=> SceneChapter16_1() },
                { "RelectionRefraction" , _=> RelectionRefraction() }
            };

            var outputFileName = args.Length > 1 ? args[1] : "out.ppm";
            var sceneName = args.Length > 0 ? args[0] : "Spheres";

            // Camera, Scene
            var (camera, scene, colorConverter) = scenes[sceneName](true);

            var timer = Stopwatch.StartNew();
            var canvas = Render(camera, scene, colorConverter);
            Console.WriteLine($"Time to render: {timer.Elapsed}");

            timer.Restart();
            using (var file = File.OpenWrite(outputFileName))
            {
                CanvasConverters.PPM_P6.Convert(canvas, file);
            }
            Console.WriteLine($"Time to output: {timer.Elapsed}");
        }
    }
}