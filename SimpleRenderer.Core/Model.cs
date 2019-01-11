using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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


        static readonly char[] _wavefrontLineSeparator = { ' ' };

        public static Model ReadWavefrontObj(TextReader reader)
        {
            const string vertexLinePrefix = "v ";
            const string faceLinePrefix = "f ";

            var vertices = new List<Vector3>();
            var indices = new List<int>();

            string line;
            for (int lineNum = 1; (line = reader.ReadLine()) != null; lineNum++)
            {
                if (line.StartsWith(vertexLinePrefix))
                {
                    var parts = line.Substring(vertexLinePrefix.Length)
                        .Split(_wavefrontLineSeparator, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 3)
                        throw new FormatException($"3 values expected at line {lineNum}");

                    if (!double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) ||
                        !double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y) ||
                        !double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var z))
                    {
                        throw new FormatException($"Failed to parse coordinate at line {lineNum}");
                    }

                    vertices.Add(new Vector3(x, y, z));
                    continue;
                }

                if (line.StartsWith(faceLinePrefix))
                {
                    var parts = line.Substring(faceLinePrefix.Length)
                        .Split(_wavefrontLineSeparator, StringSplitOptions.RemoveEmptyEntries);

                    for (int partIndex = 0; partIndex < parts.Length; ++partIndex)
                    {
                        var indStr = parts[partIndex].Split('/');
                        if (indStr.Length < 1)
                            throw new FormatException($"Invalid face format at line {lineNum}");

                        if (!int.TryParse(indStr[0], NumberStyles.None, CultureInfo.InvariantCulture, out int ind0))
                            throw new FormatException($"Invalid face format at line {lineNum}");

                        if (partIndex > 2)
                        {
                            indices.Add(indices[indices.Count - 2]);
                            indices.Add(indices[indices.Count - 1]);
                        }

                        indices.Add(ind0 - 1);
                    }
                }
            }

            return new Model(vertices, indices);
        }
    }
}
