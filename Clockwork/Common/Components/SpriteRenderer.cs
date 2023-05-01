using Clockwork.Common.GameObjects;
using Clockwork.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Components
{
    public class SpriteRenderer : RenderingComponent
    {
        // Values
        public int PixelsPerUnit = 16;

        public Material Material;

        private Mesh _mesh;
        private Texture2D _texture;


        // Properties
        public Texture2D Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                _mesh = Mesh.CreateQuad(_texture.Width / (float)PixelsPerUnit, _texture.Height / (float)PixelsPerUnit);
            }
        }


        // Constructors
        public SpriteRenderer(Texture2D texture, Material material)
        {
            Texture = texture;
            Material = material;
        }


        // Lifetime
        protected override void OnRender(object sender, FrameEventArgs e)
        {
            Material.Apply();

            Matrix4 transform = GameObject.Transform.GetMatrix();
            Material.Shader.SetMatrix4("transform", transform);
            Material.Shader.SetMatrix4("view", GameObject.ParentState.Camera.ViewMatrix);
            Material.Shader.SetMatrix4("projection", GameObject.ParentState.Camera.ProjectionMatrix);

            GL.BindVertexArray(_mesh.VAO);

            Texture.Bind(TextureUnit.Texture0);

            GL.DrawElements(PrimitiveType.Triangles, _mesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
