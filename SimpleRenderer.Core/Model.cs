using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public sealed class Model
    {
        const string VertexLinePrefix = "v ";
        const string TextureLinePrefix = "vt ";
        const string NormalLinePrefix = "vn ";
        const string FaceLinePrefix   = "f ";

        static readonly double _twoSqrtHalf = Math.Sqrt(2) / 2;

        Model(IReadOnlyList<Vector3> vertices, IReadOnlyList<Vector2> textureCoords, IReadOnlyList<Vector3> normals, IReadOnlyList<Face> faces)
        {
            Vertices = vertices;
            TextureCoords = textureCoords;
            Normals = normals;
            Faces = faces;
        }

        public IReadOnlyList<Vector3> Vertices { get; }
        public IReadOnlyList<Vector2> TextureCoords { get; }
        public IReadOnlyList<Vector3> Normals { get; }
        public IReadOnlyList<Face> Faces { get; }

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

            var textures = new Vector2[]
            {
                (0, 0),
                (1, 0),
                (1, 1),
                (0, 1)
            };

            var normals = new Vector3[]
            {
                (0, 0, 1)
            };

            var faces = new Face[]
            {
                (0, 0, 0), (1, 1, 1), (3, 3, 3),
                (1, 1, 1), (2, 2, 2), (3, 3, 3)
            };

            return new Model(vertices, textures, normals, faces);
        }


        static readonly char[] _wavefrontLineSeparator = { ' ' };

        public static Model ReadWavefrontObj(TextReader reader)
        {
            var vertices = new List<Vector3>();
            var textureCoords = new List<Vector2>();
            var normals = new List<Vector3>();
            var faces = new List<Face>();

            string line;
            for (int lineNum = 1; (line = reader.ReadLine()) != null; lineNum++)
            {
                if (line.StartsWith(VertexLinePrefix))
                    vertices.Add(ReadVector3(line, VertexLinePrefix, lineNum));
                else if (line.StartsWith(TextureLinePrefix))
                    textureCoords.Add(ReadVector2(line, TextureLinePrefix, lineNum));
                else if (line.StartsWith(NormalLinePrefix))
                    normals.Add(ReadVector3(line, NormalLinePrefix, lineNum).Normalized);
                else if (line.StartsWith(FaceLinePrefix))
                    ReadFace(line, lineNum, faces);
            }

            return new Model(vertices, textureCoords, normals, faces);
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

        static Vector2 ReadVector2(string line, string linePrefix, int lineNum)
        {
            var parts = line.Substring(linePrefix.Length)
                .Split(_wavefrontLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                throw new FormatException($"At least 2 values expected at line {lineNum}");

            if (!double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var x) ||
                !double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var y))
            {
                throw new FormatException($"Failed to parse coordinate at line {lineNum}");
            }

            return (x, y);
        }

        static void ReadFace(string line, int lineNum, List<Face> faces)
        {
            var faceIdxStrings = line.Substring(FaceLinePrefix.Length)
                .Split(_wavefrontLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            var faceIndices = new List<(int Vertex, int Texture, int Normal)>();

            foreach (var idxString in faceIdxStrings)
            {
                var indStr = idxString.Split('/');

                if (indStr.Length < 1)
                    throw new FormatException($"Invalid face format at line {lineNum}");

                if (!int.TryParse(indStr[0], NumberStyles.None, CultureInfo.InvariantCulture, out int vertex))
                    throw new FormatException($"Invalid face format at line {lineNum}");

                int texture = 0;
                if (indStr.Length >= 2 && !int.TryParse(indStr[1], NumberStyles.None, CultureInfo.InvariantCulture, out texture))
                    throw new FormatException($"Invalid face format at line {lineNum}");

                int normal = 0;
                if (indStr.Length >= 3 && !int.TryParse(indStr[2], NumberStyles.None, CultureInfo.InvariantCulture, out normal))
                    throw new FormatException($"Invalid face format at line {lineNum}");

                faceIndices.Add((vertex - 1, texture - 1, normal - 1));
            }

            if (faceIndices.Count < 3)
                throw new FormatException($"Less than 3 indices at line {lineNum}");

            faces.Add(faceIndices[0]);
            faces.Add(faceIndices[1]);
            faces.Add(faceIndices[2]);

            for (int i = 3; i < faceIndices.Count; ++i)
            {
                faces.Add(faceIndices[i - 3]);
                faces.Add(faceIndices[i - 1]);
                faces.Add(faceIndices[i - 0]);
            }
        }
    }
}
