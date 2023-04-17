using Clockwork.Common.Components;
using Clockwork.Common.GameObjects;
using Clockwork.ResourcesDeprecated;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestingUnit.States;

namespace TestingUnit
{
    internal class CubeBehavior : BaseBehavior
    {
        public MeshRenderer MeshRenderer;

        protected override void Initialize()
        {
            base.Initialize();

            Transform.Scale = new OpenTK.Mathematics.Vector3(5, 1, 5);

            MeshRenderer = GameObject.BindComponent(new MeshRenderer(Mesh.CreateCube(), Material.Copy(Game.DefaultMaterial, "Cube Material")));
            MeshRenderer.Texture = Resources.Get<Texture2D>("textures.plane.png");

            //MeshRenderer.Texture.TextureMinFilter = OpenTK.Graphics.OpenGL4.TextureMinFilter.Nearest;
            //MeshRenderer.Texture.TextureMagFilter = OpenTK.Graphics.OpenGL4.TextureMagFilter.Nearest;
        }
    }
}
