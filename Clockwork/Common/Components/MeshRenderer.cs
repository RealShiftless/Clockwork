using Clockwork.Common.GameObjects;
using Clockwork.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Clockwork.Common.Components
{
    public class MeshRenderer : RenderingComponent
    {
        public Mesh Mesh;
        public Material Material;

        public Texture2D Texture;

        public MeshRenderer(Mesh mesh, Material shader)
        {
            Mesh = mesh;
            Material = shader;
        }

        protected override void OnRender(object sender, FrameEventArgs e)
        {
            Material.Apply();

            Matrix4 transform = GameObject.Transform.GetMatrix();
            Material.Shader.SetMatrix4("transform", transform);
            Material.Shader.SetMatrix4("view", GameObject.ParentState.Camera.ViewMatrix);
            Material.Shader.SetMatrix4("projection", GameObject.ParentState.Camera.ProjectionMatrix);

            GL.BindVertexArray(Mesh.VAO);

            if (Texture != null) Texture.Bind(TextureUnit.Texture0);
            else Game.MissingTexture.Bind(TextureUnit.Texture0);

            GL.DrawElements(PrimitiveType.Triangles, Mesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
