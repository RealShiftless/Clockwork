using Clockwork.Common.Resources;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common
{
    public sealed class Font : Resource
    {
        // Values
        private FontInfo _fontInfo;

        private byte[] _bytes;
        private Character[] _characters;


        // Properties
        public string Name => _fontInfo.Name;
        public uint Size => _fontInfo.Size;


        // Func
        internal void Initialize(Library lib, FontInfo fontInfo)
        {
            _characters = new Character[128];

            _fontInfo = fontInfo;

            Face face = new Face(lib, _bytes, 0);
            face.SetPixelSizes(0, Size);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (uint c = 0; c < _characters.Length; c++)
            {
                try
                {
                    face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
                    GlyphSlot glyph = face.Glyph;
                    FTBitmap bitmap = glyph.Bitmap;

                    int texObj = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texObj);
                    GL.TexImage2D(TextureTarget.Texture2D, 0,
                                  PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0,
                                  PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);

                    // set texture parameters
                    GL.TextureParameter(texObj, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TextureParameter(texObj, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TextureParameter(texObj, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TextureParameter(texObj, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                    // add character
                    Character ch = new Character();
                    ch.TextureHandle = texObj;
                    ch.Size = new Vector2(bitmap.Width, bitmap.Rows);
                    ch.Bearing = new Vector2(glyph.BitmapLeft, glyph.BitmapTop);
                    ch.Advance = glyph.Advance.X.Value;
                    _characters[c] = ch;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }


        // Overrides
        protected override void Populate(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                _bytes = memoryStream.ToArray();
            }
        }
        protected override void Dispose()
        {
            foreach (Character character in _characters)
            {
                GL.DeleteTexture(character.TextureHandle);
            }
        }


        // Structs
        private struct Character
        {
            public int TextureHandle;
            public Vector2 Size;
            public Vector2 Bearing;
            public int Advance;
        }
    }
}
