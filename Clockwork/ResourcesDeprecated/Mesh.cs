using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace Clockwork.ResourcesDeprecated
{
    public class Mesh
    {
        // Pointers
        internal readonly int VBO;
        internal readonly int VAO;
        internal readonly int EBO;


        // Values
        public Vector3[] Vertices;
        public Vector2[] UVs;

        public uint[] Indices;

        private float[] _data;


        // Properties
        public float[] Data => _data;


        // Constructor
        public Mesh()
        {
            // Setup the VBO
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            // Setup the VAO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            // Vertex Position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // UVs
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Setup the EBO
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        }
        public Mesh(Vector3[] vertices, Vector2[] uvs, uint[] indices) : this()
        {
            if (vertices.Length != uvs.Length)
                throw new ArgumentException("Vertices where not the same length as UVs! (" + vertices.Length + " != " + uvs.Length + ")!");

            _data = new float[vertices.Length * 5];

            Vertices = vertices;
            UVs = uvs;

            Indices = indices;

            ApplyChanges();
        }

        public static Mesh CreateQuad(float width, float height)
        {
            width /= 2;
            height /= 2;

            return new Mesh(
                new Vector3[]
                {
                    new Vector3(-width,  height,  0.0f),
                    new Vector3( width,  height,  0.0f),
                    new Vector3(-width, -height,  0.0f),
                    new Vector3( width, -height,  0.0f)
                },
                new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
                },
                new uint[]
                {
                    0, 2, 1,
                    1, 2, 3
                }
            );
        }
        public static Mesh CreateQuad()
        {
            return CreateQuad(1, 1);
        }
        public static Mesh CreateCube(float width, float height, float depth)
        {
            width /= 2;
            height /= 2;
            depth /= 2;

            return new Mesh(
                new Vector3[]
                {
                    new Vector3(-width,  height,  depth),
                    new Vector3( width,  height,  depth),
                    new Vector3(-width, -height,  depth),
                    new Vector3( width, -height,  depth),

                    new Vector3(-width,  height, -depth),
                    new Vector3( width,  height, -depth),
                    new Vector3(-width, -height, -depth),
                    new Vector3( width, -height, -depth),

                    new Vector3(-width,  height, -depth),
                    new Vector3(-width,  height,  depth),
                    new Vector3(-width, -height, -depth),
                    new Vector3(-width, -height,  depth),

                    new Vector3( width,  height, -depth),
                    new Vector3( width,  height,  depth),
                    new Vector3( width, -height, -depth),
                    new Vector3( width, -height,  depth),

                    new Vector3(-0.5f,  0.5f,  0.5f),
                    new Vector3( 0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f, -0.5f),
                    new Vector3( 0.5f,  0.5f, -0.5f),

                    new Vector3(-0.5f, -0.5f,  0.5f),
                    new Vector3( 0.5f, -0.5f,  0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3( 0.5f, -0.5f, -0.5f),
                },
                new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),

                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),

                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),

                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0),

                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                },
                new uint[]
                {
                    0, 2, 1,
                    1, 2, 3,

                    4, 5, 6,
                    5, 7, 6,

                    9, 8, 11,
                    8, 10, 11,

                    12, 13, 14,
                    13, 15, 14,

                    16, 17, 18,
                    17, 19, 18,

                    22, 23, 20,
                    23, 21, 20
                }
            );
        }
        public static Mesh CreateCube()
        {
            return CreateCube(1, 1, 1);
        }

        public void ApplyChanges()
        {
            // Reset the Data array
            _data = new float[Vertices.Length * 5];

            // Go thru the different data sources and set them into the Data array
            for (int i = 0; i < Vertices.Length; i++)
            {
                // Position
                _data[i * 5 + 0] = Vertices[i].X;
                _data[i * 5 + 1] = Vertices[i].Y;
                _data[i * 5 + 2] = Vertices[i].Z;

                // UVs
                _data[i * 5 + 3] = UVs[i].X;
                _data[i * 5 + 4] = UVs[i].Y;
            }

            // Binding the buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            // Setting the data of the buffers
            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * sizeof(float), _data, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
        }


    }
}
