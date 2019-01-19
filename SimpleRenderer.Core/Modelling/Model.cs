using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core.Modelling
{
    public sealed class Model
    {
        const string MaterialNamePrefix = "mtllib ";

        const string UseMtlPrefix = "usemtl ";

        const string VertexLinePrefix  = "v ";
        const string TextureLinePrefix = "vt ";
        const string NormalLinePrefix  = "vn ";
        const string FaceLinePrefix    = "f ";

        const string NewMtlPrefix           = "newmtl ";
        const string SpecularExponentPrefix = "Ns ";
        const string AmbientColorPrefix     = "Ka ";
        const string DiffuseColorPrefix     = "Kd ";
        const string SpecularColorPrefix    = "Ks ";

        const string AmbientTexturePrefix = "map_Ka ";
        const string DiffuseTexturePrefix = "map_Kd ";


        static readonly double _twoSqrtHalf = Math.Sqrt(2) / 2;
        static readonly char[] _wavefrontLineSeparator = { ' ' };


        Model(IReadOnlyList<Vector3> vertices, IReadOnlyList<Vector2> textureCoords, IReadOnlyList<Vector3> normals, IReadOnlyList<Face> faces, IReadOnlyList<Triangle> triangles)
        {
            Vertices = vertices;
            TextureCoords = textureCoords;
            Normals = normals;
            Faces = faces;
            Triangles = triangles;
        }

        public IReadOnlyList<Vector3> Vertices { get; }
        public IReadOnlyList<Vector2> TextureCoords { get; }
        public IReadOnlyList<Vector3> Normals { get; }
        public IReadOnlyList<Face> Faces { get; }
        public IReadOnlyList<Triangle> Triangles { get; }

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

            var faces = new[]
            {
                new Face(0, 0, 0), new Face(1, 1, 1),
                new Face(2, 2, 2), new Face(3, 3, 3),
            };

            var facesIndices = new[]
            {
                new Triangle(0, 1, 3, null),
                new Triangle(1, 2, 3, null)
            };

            return new Model(vertices, textures, normals, faces, facesIndices);
        }


        public static Model ReadWavefrontObj(string path)
        {
            var vertices = new List<Vector3>();
            var textureCoords = new List<Vector2>();
            var normals = new List<Vector3>();
            var faces = new List<Face>();
            var triangles = new List<Triangle>();

            var materialsByName = new Dictionary<string, Material>();
            var dir = Path.GetDirectoryName(path);

            Material currentMaterial = null;

            using (var reader = new StreamReader(path))
            {
                string line;
                for (int lineNum = 1; (line = reader.ReadLine()) != null; lineNum++)
                {
                    line = line.Trim();

                    if (line.StartsWith(UseMtlPrefix))
                        currentMaterial = materialsByName[line.Substring(UseMtlPrefix.Length)];
                    else if (line.StartsWith(MaterialNamePrefix))
                        ReadMaterials(line, dir, materialsByName);
                    else if (line.StartsWith(VertexLinePrefix))
                        vertices.Add(ReadVector3(line, VertexLinePrefix, lineNum));
                    else if (line.StartsWith(TextureLinePrefix))
                        textureCoords.Add(ReadVector2(line, TextureLinePrefix, lineNum));
                    else if (line.StartsWith(NormalLinePrefix))
                        normals.Add(ReadVector3(line, NormalLinePrefix, lineNum).Normalized);
                    else if (line.StartsWith(FaceLinePrefix))
                        ReadFace(line, lineNum, currentMaterial, faces, triangles);
                }
            }

            return new Model(vertices, textureCoords, normals, faces, triangles);
        }


        public static void ReadMaterials(string line, string directory, IDictionary<string, Material> materialsByName)
        {
            var path = Path.Combine(directory, line.Substring(MaterialNamePrefix.Length));

            Material[] materials;
            using (var reader = new StreamReader(path))
                materials = ReadMaterials(reader, directory);

            foreach (var material in materials)
                materialsByName.Add(material.Name, material);
        }

        public static Material[] ReadMaterials(TextReader reader, string dir)
        {
            string line;
            var lines = new List<string>();
            var newMtlIndices = new List<int>();
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.StartsWith(NewMtlPrefix))
                    newMtlIndices.Add(lines.Count);

                lines.Add(line);
            }

            var materials = new List<Material>();

            for (int i = 0; i < newMtlIndices.Count; ++i)
            {
                string name             = null;
                double specularExponent = 0;

                Vector3 ambientColor     = default;
                Vector3 diffuseColor     = default;
                Vector3 specularColor    = default;

                Texture ambientTexture = null;
                Texture diffuseTexture = null;

                int end = i + 1 < newMtlIndices.Count ? newMtlIndices[i + 1] : lines.Count;

                for (int j = newMtlIndices[i]; j < end; ++j)
                {
                    if (lines[j].StartsWith(NewMtlPrefix))
                        name = lines[j].Substring(NewMtlPrefix.Length).Trim();
                    else if (lines[j].StartsWith(AmbientColorPrefix))
                        ambientColor = ReadVector3(lines[j], AmbientColorPrefix, j + 1);
                    else if (lines[j].StartsWith(DiffuseColorPrefix))
                        diffuseColor = ReadVector3(lines[j], DiffuseColorPrefix, j + 1);
                    else if (lines[j].StartsWith(SpecularColorPrefix))
                        specularColor = ReadVector3(lines[j], SpecularColorPrefix, j + 1);
                    else if (lines[j].StartsWith(AmbientTexturePrefix))
                        ambientTexture = ReadTexture(lines[j], AmbientTexturePrefix, dir);
                    else if (lines[j].StartsWith(DiffuseTexturePrefix))
                        diffuseTexture = ReadTexture(lines[j], DiffuseTexturePrefix, dir);
                    else if (lines[j].StartsWith(SpecularExponentPrefix))
                        if (!double.TryParse(lines[j].Substring(SpecularExponentPrefix.Length), NumberStyles.Any, CultureInfo.InvariantCulture, out specularExponent))
                            throw new FormatException($"Invalid specular exponent format at line {j + 1}");
                }

                materials.Add(new Material(name, specularExponent, ambientColor, diffuseColor, specularColor, ambientTexture, diffuseTexture));
            }

            return materials.ToArray();
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

        static void ReadFace(string line, int lineNum, Material material, List<Face> faces, List<Triangle> triangles)
        {
            var faceIdxStrings = line.Substring(FaceLinePrefix.Length)
                .Split(_wavefrontLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            int facesIndicesBase = faces.Count;

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

                faces.Add(new Face(vertex - 1, texture - 1, normal - 1));
            }

            if (faces.Count - facesIndicesBase < 3)
                throw new FormatException($"Less than 3 indices at line {lineNum}");

            triangles.Add(new Triangle(
                facesIndicesBase++,
                facesIndicesBase++,
                facesIndicesBase,
                material));

            while (++facesIndicesBase < faces.Count)
            {
                triangles.Add(new Triangle(
                    facesIndicesBase - 3,
                    facesIndicesBase - 1,
                    facesIndicesBase - 0,
                    material
                ));
            }
        }

        static Texture ReadTexture(string line, string prefix, string dir)
        {
            var path = Path.Combine(dir, line.Substring(prefix.Length));
            return Texture.ReadFrom(path);
        }
    }
}
