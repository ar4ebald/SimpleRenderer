using System;
using SimpleRenderer.Mathematics;

namespace SimpleRenderer.Core
{
    public sealed class Material
    {
        public readonly string Name;

        public readonly double SpecularExponent;
        public readonly Vector3 AmbientColor;
        public readonly Vector3 DiffuseColor;
        public readonly Vector3 SpecularColor;

        public readonly Texture AmbientTexture;
        public readonly Texture DiffuseTexture;

        public Material(string name, double specularExponent, Vector3 ambientColor, Vector3 diffuseColor, Vector3 specularColor, Texture ambientTexture, Texture diffuseTexture)
        {
            Name = name;
            SpecularExponent = specularExponent;
            AmbientColor = ambientColor;
            DiffuseColor = diffuseColor;
            SpecularColor = specularColor;
            AmbientTexture = ambientTexture;
            DiffuseTexture = diffuseTexture;
        }
    }
}
