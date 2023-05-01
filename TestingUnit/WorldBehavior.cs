using Clockwork.Common;
using Clockwork.Common.Components;
using Clockwork.Common.GameObjects;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingUnit
{
    internal class WorldBehavior : BaseBehavior
    {
        Mesh _quad;

        protected override void Initialize()
        {
            _quad = Mesh.CreateQuad();

            GameObject.BindComponent(new MeshRenderer(_quad, Resources.Load<Material>(Game.EntryAssembly, "res.materials.defualt_material.cwmat")) { Texture = Resources.Load<Texture2D>(Game.EntryAssembly, "textures.plane.png") });

            GameObject.Transform.Position = new Vector3(0, -.5f, 0);
            GameObject.Transform.Scale.X = 20;
            GameObject.Transform.Scale.Y = 20;
            GameObject.Transform.Rotation = Quaternion.FromEulerAngles(-MathHelper.PiOver2, 0, 0);
        }
    }
}
