using Clockwork.Rendering;
using OpenTK.Graphics.OpenGL4;
using Clockwork.Common.Components;
using Clockwork.Common.GameObjects;
using Clockwork.Common;

namespace TestingUnit
{
    public class PlayerBehavior : UpdatingBehavior
    {
        private SpriteRenderer _spriteRenderer;

        protected override void Initialize()
        {
            base.Initialize();

            Texture2D texture = Resources.Load<Texture2D>(Game.EntryAssembly, "textures.player.png");

            texture.TextureMinFilter = TextureMinFilter.Nearest;
            texture.TextureMagFilter = TextureMagFilter.Nearest;

            _spriteRenderer = GameObject.BindComponent(new SpriteRenderer(texture, Material.Copy(Game.DefaultMaterial, "Sprite Material")));

        }

        protected override void Update(object sender, FrameEventArgs e)
        {
            //Transform.Rotation = Quaternion.FromEulerAngles(new Vector3(0, e.Time.Ellapsed / 4, 0));
        }

        protected override void LateUpdate(object sender, FrameEventArgs e)
        {
        }
    }
}
