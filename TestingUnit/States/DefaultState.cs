using Clockwork;
using Clockwork.Common;
using Clockwork.Common.GameObjects;
using Clockwork.Rendering;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TestingUnit.States
{
    internal class DefaultState : GameState
    {
        private GameObject _cube;
        private GameObject _player;

        private Texture2D _texture;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        public override void Initialize()
        {
            base.Initialize();

            Game.Renderer.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

            Update += OnStateUpdate;

            // Load Resources
            _texture = Resources.Load<Texture2D>(Game.EntryAssembly, "textures.container.png");

            // Instantiate GameObjects
            //InstantiateGameObject<WorldBehavior>("world");

            _cube = InstantiateGameObject<CubeBehavior>("cube");

            _player = InstantiateGameObject<PlayerBehavior>("player");
            _player.Transform.Position = new Vector3(0, 1.5f, 0);

            //Material material = Resources.Get<Material>("materials.material.json");

            
            Material material = Material.Create("Default Material", Game.DefaultShader);

            material.SetColor("color", Color4.Red);
        }

        private void OnStateUpdate(object sender, FrameEventArgs e)
        {
            if (!Game.Renderer.IsFocused) // Check to see if the window is focused
            {
                return;
            }

            KeyboardState input = e.Keyboard;

            if (input.IsKeyDown(Keys.Escape))
            {
                Game.Stop();
            }

            const float cameraSpeed = 3f;
            const float sensitivity = 0.2f;

            float speedMult = 1;

            if (input.IsKeyDown(Keys.LeftControl))
            {
                speedMult = 3;
            }


            if (input.IsKeyDown(Keys.W))
            {
                Camera.Position += Camera.Forward * cameraSpeed * speedMult * e.Time.Delta; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                Camera.Position -= Camera.Forward * cameraSpeed * speedMult * e.Time.Delta; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                Camera.Position -= Camera.Right * cameraSpeed * speedMult * e.Time.Delta; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                Camera.Position += Camera.Right * cameraSpeed * speedMult * e.Time.Delta; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                Camera.Position += Vector3.UnitY * cameraSpeed * speedMult * e.Time.Delta; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                Camera.Position -= Vector3.UnitY * cameraSpeed * speedMult * e.Time.Delta; // Down
            }

            // Get the mouse state
            MouseState mouse = e.Mouse;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                Camera.Yaw += deltaX * sensitivity;
                Camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }
    }
}
