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
        const string VertexLinePrefix = "v ";
        const string NormalLinePrefix = "vn ";
        const string FaceLinePrefix   = "f ";

        static readonly double _twoSqrtHalf = Math.Sqrt(2) / 2;

        Model(IReadOnlyList<Vector3> vertices, IReadOnlyList<int> verticesIndices, IReadOnlyList<Vector3> normals, IReadOnlyList<int> normalsIndices)
        {
            Vertices = vertices;
            VerticesIndices = verticesIndices;
            Normals = normals;
            NormalsIndices = normalsIndices;
        }

        public IReadOnlyList<Vector3> Vertices { get; }
        public IReadOnlyList<int> VerticesIndices { get; }
        public IReadOnlyList<Vector3> Normals { get; }
        public IReadOnlyList<int> NormalsIndices { get; }

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

            var verticesIndices = new[]
            {
                0, 1, 3,
                1, 2, 3
            };

            var normals = new Vector3[]
            {
                (0, 0, 1)
            };

            var normalsIndices = new[]
            {
                0
            };

            return new Model(vertices, verticesIndices, normals, normalsIndices);
        }


        static readonly char[] _wavefrontLineSeparator = { ' ' };

        public static Model ReadWavefrontObj(TextReader reader)
        {
            var vertices = new List<Vector3>();
            var verticesIndices = new List<int>();
            var normals = new List<Vector3>();
            var normalsIndices = new List<int>();

            string line;
            for (int lineNum = 1; (line = reader.ReadLine()) != null; lineNum++)
            {
                if (line.StartsWith(VertexLinePrefix))
                {
                    vertices.Add(ReadVector3(line, VertexLinePrefix, lineNum));
                    continue;
                }

                if (line.StartsWith(NormalLinePrefix))
                {
                    normals.Add(ReadVector3(line, NormalLinePrefix, lineNum).Normalized);
                    continue;
                }

                if (line.StartsWith(FaceLinePrefix))
                {
                    ReadFace(line, lineNum, verticesIndices, normalsIndices);
                    continue;
                }
            }

            return new Model(vertices, verticesIndices, normals, normalsIndices);
        }

        static Vector3 ReadVector3(string line, string linePrefix, int lineNum)
        {
            var parts = line.Substring(linePrefix.Length)
                .Split(_wavefrontLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
                throw new FormatException($"3 values expected at line {lineNum}");

            if (!double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) ||
                !double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y) ||
                !double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var z))
            {
                throw new FormatException($"Failed to parse coordinate at line {lineNum}");
            }

            return (x, y, z);
        }

        static void ReadFace(string line, int lineNum, List<int> verticesIndices, List<int> normalsIndices)
        {
            var faceIdxStrings = line.Substring(FaceLinePrefix.Length)
                .Split(_wavefrontLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            var faceIndices = new List<(int Vertex, int Normal)>();

            foreach (var idxString in faceIdxStrings)
            {
                var indStr = idxString.Split('/');

                if (indStr.Length < 1)
                    throw new FormatException($"Invalid face format at line {lineNum}");

                if (!int.TryParse(indStr[0], NumberStyles.None, CultureInfo.InvariantCulture, out int vertex))
                    throw new FormatException($"Invalid face format at line {lineNum}");

                if (!int.TryParse(indStr[2], NumberStyles.None, CultureInfo.InvariantCulture, out int normal))
                    throw new FormatException($"Invalid face format at line {lineNum}");

                faceIndices.Add((vertex - 1, normal - 1));
            }

            if (faceIndices.Count < 3)
                throw new FormatException($"Less than 3 indices at line {lineNum}");


            verticesIndices.Add(faceIndices[0].Vertex);
            verticesIndices.Add(faceIndices[1].Vertex);
            verticesIndices.Add(faceIndices[2].Vertex);

            normalsIndices.Add(faceIndices[0].Normal);
            normalsIndices.Add(faceIndices[1].Normal);
            normalsIndices.Add(faceIndices[2].Normal);

            for (int i = 3; i < faceIndices.Count; ++i)
            {
                verticesIndices.Add(faceIndices[i - 3].Vertex);
                verticesIndices.Add(faceIndices[i - 1].Vertex);
                verticesIndices.Add(faceIndices[i - 0].Vertex);

                normalsIndices.Add(faceIndices[i - 3].Normal);
                normalsIndices.Add(faceIndices[i - 1].Normal);
                normalsIndices.Add(faceIndices[i - 0].Normal);
            }
        }
    }
}
