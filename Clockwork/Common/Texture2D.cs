using Clockwork.Common.Resources;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common
{
    internal class Texture2D : Resource
    {
        // Values
        private int _handle;

        private Image<Rgba32> _image;

        private TextureMinFilter _textureMinFilter;
        private TextureMagFilter _textureMagFilter;

        private TextureWrapMode _textureWrapS;
        private TextureWrapMode _textureWrapT;


        // Properties
        public int TextureHandle => _handle;

        public int Width => _image.Width;
        public int Height => _image.Height;

        public TextureMinFilter TextureMinFilter
        {
            get
            {
                return _textureMinFilter;
            }
            set
            {
                _textureMinFilter = value;
                SetTextureParameter(TextureParameterName.TextureMinFilter, (int)value);
            }
        }
        public TextureMagFilter TextureMagFilter
        {
            get
            {
                return _textureMagFilter;
            }
            set
            {
                _textureMagFilter = value;
                SetTextureParameter(TextureParameterName.TextureMagFilter, (int)value);
            }
        }

        public TextureWrapMode TextureWrapS
        {
            get
            {
                return _textureWrapS;
            }
            set
            {
                _textureWrapS = value;
                SetTextureParameter(TextureParameterName.TextureWrapS, (int)value);
            }
        }
        public TextureWrapMode TextureWrapT
        {
            get
            {
                return _textureWrapT;
            }
            set
            {
                _textureWrapT = value;
                SetTextureParameter(TextureParameterName.TextureWrapT, (int)value);
            }
        }


        // Static Func
        public static Texture2D Create(int width, int height)
        {
            Image<Rgba32> image = new Image<Rgba32>(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    image[x, y] = new Rgba32(255, 255, 255, 0);
                }
            }

            Texture2D texture = new Texture2D();
            texture.Populate(image);

            return texture;
        }


        // Resource Func
        protected override void Populate(Stream stream)
        {
            Populate(Image.Load<Rgba32>(stream));
        }
        private void Populate(Image<Rgba32> image)
        {
            // Create and bind the Texture
            _handle = GL.GenTexture();
            Bind(TextureUnit.Texture0);

            // Load the pixels into an array and set them to the Texture
            _image = image;
            byte[] pixels = new byte[4 * _image.Width * _image.Height];
            _image.CopyPixelDataTo(pixels);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _image.Width, _image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            // Set the default values to what I like
            TextureMinFilter = TextureMinFilter.Linear;
            TextureMagFilter = TextureMagFilter.Linear;

            TextureWrapS = TextureWrapMode.Repeat;
            TextureWrapT = TextureWrapMode.Repeat;

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        }
        protected override void Dispose()
        {
            GL.DeleteTexture(TextureHandle);
        }


        // Func
        public void SetTextureParameter(TextureParameterName parameter, int value)
        {
            Bind(TextureUnit.Texture0);
            GL.TexParameter(TextureTarget.Texture2D, parameter, value);
        }

        public void Bind(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
        }

        public void SetPixel(int x, int y, Color4 color)
        {
            _image[x, y] = new Rgba32(color.R, color.G, color.B, color.A);
        }
        public void SetPixel(Vector2i position, Color4 color)
        {
            SetPixel(position.X, position.Y, color);
        }

        public void Save(string path, EncoderType type = EncoderType.Png)
        {
            switch (type)
            {
                case EncoderType.Png:
                    _image.SaveAsPng(path);
                    break;

                case EncoderType.Bmp:
                    _image.SaveAsBmp(path);
                    break;

                case EncoderType.Jpeg:
                    _image.SaveAsJpeg(path);
                    break;

                case EncoderType.Tiff:
                    _image.SaveAsTiff(path);
                    break;

                case EncoderType.WebP:
                    _image.SaveAsWebp(path);
                    break;

                case EncoderType.Pbm:
                    _image.SaveAsPbm(path);
                    break;
            }
        }
    }

    public enum EncoderType
    {
        Bmp = 0,
        Jpeg,
        Png,
        Tiff,
        WebP,
        Pbm
    }
}
