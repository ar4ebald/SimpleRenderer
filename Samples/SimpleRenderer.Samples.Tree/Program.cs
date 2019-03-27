using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SimpleRenderer.Core;
using SimpleRenderer.Core.Modelling;
using SimpleRenderer.Core.Rendering;
using SimpleRenderer.Core.World;
using SimpleRenderer.Mathematics;
using SimpleRenderer.Windows;

namespace SimpleRenderer.Samples.Tree
{
    public struct Vertex
    {
        public Vector3 Normal;
        public Vector2 UV;
    }

    class Program : HostForm
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Program());
        }

        Matrix _projection;

        readonly Matrix _perspective;
        readonly Matrix _orthographic;
        readonly Vector3 _lightDirection;

        readonly Camera _camera;

        readonly SceneObject[] _objects;

        bool _d1IsPressed = false;
        bool _wireframe = false;

        double _lightIntensity = 0.4;

        public Program()
        {
            Size = SizeFromClientSize(new Size(600, 600));
            Text = "Компьютерная графика. КДЗ 1, вариант 1, Булатов Артур Ринатович, 2019";

            HelpButton = true;
            MinimizeBox = false;
            MaximizeBox = false;

            HelpButtonClicked += OnHelpButtonClicked;

            _objects = new[]
            {
                new SceneObject(
                    Model.ReadWavefrontObj(@"Models\Lowpoly_tree_sample.obj"),
                    new Vector3(0, 0, 0),
                    new Vector3(1)
                ),

                new SceneObject(
                    Model.ReadWavefrontObj(@"Models\Statue\12330_Statue_v1_L2.obj"),
                    new Vector3(20, 0, 0),
                    new Vector3(0.05),
                    Quaternion.RotationAxis(-Vector3.UnitX, Math.PI / 2)
                ),

                new SceneObject(
                    Model.ReadWavefrontObj(@"Models\Monkey\14090_Hear_No_Evil_Monkey_v2_L1.obj"),
                    new Vector3(-20, 0, 0),
                    new Vector3(0.25),
                    Quaternion.RotationAxis(-Vector3.UnitX, Math.PI / 2)
                ),

                //"Models\Grass\10438_Circular_Grass_Patch_v1_iterations-2.obj"

                //new SceneObject(
                //    Model.ReadWavefrontObj(@"Models\Mountains\lowpolymountains.obj"),
                //    new Vector3(0, 0, 0),
                //    new Vector3(10)
                //),

                //new SceneObject(
                //    Model.ReadWavefrontObj(@"Models\Mountain\part.obj"),
                //    new Vector3(0, 0, 0),
                //    new Vector3(10)
                //)
            };

            _camera = new Camera
            {
                Position = new Vector3(0, 0, -40)
            };

            _projection = _perspective = Matrix.Perspective(Math.PI / 180 * 60, 1, 0.1, 1000);

            const double s = 0.05;
            _orthographic = new Matrix(
                s, 0, 0, 0,
                0, s, 0, 0,
                0, 0, s, 0,
                0, 0, 0, 1
            );

            _lightDirection = new Vector3(1, 1, -1).Normalized;
        }

        void OnHelpButtonClicked(object sender, CancelEventArgs e)
        {
            var message = "Автор программы - Булатов Артур Ринатович (arbulatov@edumail.hse.ru)\r\nРеализованно:\r\n * Загрузка моделей\r\n * Нанесение текстур на модель\r\n * Перемещение камеры\r\n * Источник света, плоская закраска\r\n * Каркасная модель – solid модель\r\n * Переключение вида проекции\r\nУправление:\r\nW - вперед\r\nA - влево\r\nS - назад\r\nD - вправо\r\nQ - вверх\r\nE - вниз\r\nD1 - solid/wireframe\r\nD2 - перспективная проекция\r\nD3 - параллельная проекция\r\nD4/D5 вкл/выкл источник света\r\nSpace - ускорить движение\r\nR/F - повороты моделей";
            MessageBox.Show(message, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);

            e.Cancel = true;
        }

        protected override void Update(double dt)
        {
            if (IsKeyDown(Keys.Left))
            {
                _camera.Rotation *= Quaternion.RotationAxis(
                    IsKeyDown(Keys.ControlKey) ? Vector3.UnitZ : Vector3.UnitY,
                    -dt
                );
            }
            if (IsKeyDown(Keys.Right))
            {
                _camera.Rotation *= Quaternion.RotationAxis(
                    IsKeyDown(Keys.ControlKey) ? Vector3.UnitZ : Vector3.UnitY,
                    +dt
                );
            }

            if (IsKeyDown(Keys.Up))
                _camera.Rotation *= Quaternion.RotationAxis(Vector3.UnitX, -dt);
            if (IsKeyDown(Keys.Down))
                _camera.Rotation *= Quaternion.RotationAxis(Vector3.UnitX, +dt);

            Vector3 delta = default;

            if (IsKeyDown(Keys.W))
                delta += Vector3.UnitZ;
            if (IsKeyDown(Keys.A))
                delta -= Vector3.UnitX;
            if (IsKeyDown(Keys.S))
                delta -= Vector3.UnitZ;
            if (IsKeyDown(Keys.D))
                delta += Vector3.UnitX;
            if (IsKeyDown(Keys.E))
                delta += Vector3.UnitY;
            if (IsKeyDown(Keys.Q))
                delta -= Vector3.UnitY;

            var speed = IsKeyDown(Keys.Space) ? 10 : 1;

            _camera.Position += _camera.Rotation.Rotate(delta) * speed;

            if (IsKeyDown(Keys.D1) && !_d1IsPressed)
                _wireframe = !_wireframe;

            _d1IsPressed = IsKeyDown(Keys.D1);

            if (IsKeyDown(Keys.R))
            {
                foreach (var obj in _objects)
                    obj.Rotation = Quaternion.RotationAxis(Vector3.UnitY, Math.PI * dt) * obj.Rotation;
            }

            if (IsKeyDown(Keys.F))
            {
                foreach (var obj in _objects)
                    obj.Rotation = Quaternion.RotationAxis(Vector3.UnitY, -Math.PI * dt) * obj.Rotation;
            }

            if (IsKeyDown(Keys.D2))
                _projection = _perspective;

            if (IsKeyDown(Keys.D3))
                _projection = _orthographic;

            if (IsKeyDown(Keys.D4))
                _lightIntensity = 0;

            if (IsKeyDown(Keys.D5))
                _lightIntensity = 0.4;

        }

        protected override void Render(Canvas canvas, double dt)
        {
            var view = _camera.ViewMatrix;

            foreach (var obj in _objects)
            {
                var world = obj.WorldMatrix;
                var worldViewProjection = world * view * _projection;

                void VertexShader(in Face face, out Vertex vertex, out Vector4 projection)
                {
                    var position = obj.Model.Vertices[face.Vertex];
                    var normal = obj.Model.Normals[face.Normal];
                    var uv = face.Texture < 0 ? Vector2.Zero : obj.Model.TextureCoords[face.Texture];

                    projection = worldViewProjection * (position, 1);
                    normal = (world * (normal, 0)).XYZ.Normalized;

                    vertex = new Vertex
                    {
                        Normal = normal,
                        UV = uv
                    };
                }

                void PixelShader(Material material, in Vertex input, out Vector3 color)
                {

                    var shade = Vector3.Dot(input.Normal, _lightDirection);
                    shade = Math.Min(1, Math.Max(0, shade * _lightIntensity + (1 - _lightIntensity)));

                    color = material.DiffuseColor * shade;
                    if (material.AmbientTexture != null)
                        color = material.AmbientTexture[input.UV] * shade;
                }

                if (_wireframe)
                {
                    WireframeRenderer.Render(
                        canvas,
                        obj.Model,
                        obj.WorldMatrix * view * _projection,
                        Pixel.Red
                    );
                }
                else
                {
                    FillingRenderer.Render<Vertex>(
                        canvas,
                        obj.Model.Faces,
                        obj.Model.Triangles,
                        VertexShader, PixelShader,
                        CullingMode.Clockwise
                    );
                }
            }
        }
    }
}
