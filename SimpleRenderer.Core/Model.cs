using System;
using System.Collections.Generic;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public sealed class Model
    {
        static readonly double _twoSqrtHalf = Math.Sqrt(2) / 2;

        public Model(IReadOnlyList<Vector3> vertices, IReadOnlyList<int> indices)
        {
            Vertices = vertices;
            Indices = indices;
        }

        public IReadOnlyList<Vector3> Vertices { get; }
        public IReadOnlyList<int> Indices { get; }


        public static Model CreatePlane(double size)
        {
            var r = size * _twoSqrtHalf;

            var vertices = new Vector3[]
            {
                (-r, +r, 0),
                (+r, +r, 0),
                (+r, -r, 0),
                (-r, -r, 0)
            };

            var indices = new[]
            {
                0, 1, 3,
                1, 2, 3
            };

            return new Model(vertices, indices);
        }
    }
}
