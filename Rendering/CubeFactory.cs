using mszcubemod.Textures;
using UnityEngine;

namespace mszcubemod.Rendering
{
    public static class CubeFactory
    {
        const string cubeName = "zcube";

        public static GameObject CreateCube(Vector3 position, IBlockTexture texture, Vector3 scale)
        {
            GameObject cube = new GameObject(cubeName);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            MeshFilter filter = cube.AddComponent<MeshFilter>();
            MeshRenderer renderer = cube.AddComponent<MeshRenderer>();
            filter.mesh = BuildCubeMesh();
            Material[] materials = new Material[]
            {
                CreateTextureMaterial(texture.West),
                CreateTextureMaterial(texture.East),
                CreateTextureMaterial(texture.Up),
                CreateTextureMaterial(texture.Down),
                CreateTextureMaterial(texture.North),
                CreateTextureMaterial(texture.South),
            };
            renderer.materials = materials;
            if (texture is IAnimatedBlockTexture animated)
            {
                animated.Updated += () =>
                {
                    materials[0].mainTexture = animated.West;
                    materials[1].mainTexture = animated.East;
                    materials[2].mainTexture = animated.Up;
                    materials[3].mainTexture = animated.Down;
                    materials[4].mainTexture = animated.North;
                    materials[5].mainTexture = animated.South;
                };
            }
            cube.AddComponent<BoxCollider>();
            return cube;
        }

        public static GameObject CreateWireframeCube(Vector3 position, Vector3 size)
        {
            GameObject obj = new("cubePreview");
            obj.transform.position = position;

            Vector3 h = size * 0.5f;
            Vector3[] corners = new Vector3[]
            {
                new Vector3(-h.x, -h.y, -h.z), new Vector3( h.x, -h.y, -h.z),
                new Vector3( h.x,  h.y, -h.z), new Vector3(-h.x,  h.y, -h.z),
                new Vector3(-h.x, -h.y,  h.z), new Vector3( h.x, -h.y,  h.z),
                new Vector3( h.x,  h.y,  h.z), new Vector3(-h.x,  h.y,  h.z),
            };

            int[][] edges = new int[][]
            {
                new[]{0,1}, new[]{1,2}, new[]{2,3}, new[]{3,0},
                new[]{4,5}, new[]{5,6}, new[]{6,7}, new[]{7,4},
                new[]{0,4}, new[]{1,5}, new[]{2,6}, new[]{3,7},
            };

            Material mat = new(Shader.Find("Unlit/Color"));
            mat.color = Color.black;

            foreach (int[] edge in edges)
            {
                GameObject line = new GameObject("Edge");
                line.transform.SetParent(obj.transform, false);
                LineRenderer lr = line.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, corners[edge[0]]);
                lr.SetPosition(1, corners[edge[1]]);
                lr.startWidth = 0.02f;
                lr.endWidth = 0.02f;
                lr.material = mat;
                lr.useWorldSpace = false;
            }

            return obj;
        }

        private static Mesh BuildCubeMesh()
        {
            Mesh mesh = new();

            Vector3[] vertices = new Vector3[]
            {
                // Right (+X)
                new(0.5f, -0.5f, -0.5f), new(0.5f,  0.5f, -0.5f),
                new(0.5f,  0.5f,  0.5f), new(0.5f, -0.5f,  0.5f),

                // Left (-X)
                new(-0.5f, -0.5f,  0.5f), new(-0.5f,  0.5f,  0.5f),
                new(-0.5f,  0.5f, -0.5f), new(-0.5f, -0.5f, -0.5f),

                // Top (+Y)
                new(-0.5f, 0.5f, -0.5f), new(-0.5f, 0.5f,  0.5f),
                new( 0.5f, 0.5f,  0.5f), new( 0.5f, 0.5f, -0.5f),

                // Bottom (-Y)
                new(-0.5f, -0.5f,  0.5f), new(-0.5f, -0.5f, -0.5f),
                new( 0.5f, -0.5f, -0.5f), new( 0.5f, -0.5f,  0.5f),

                // Front (+Z)
                new(-0.5f, -0.5f, 0.5f), new( 0.5f, -0.5f, 0.5f),
                new( 0.5f,  0.5f, 0.5f), new(-0.5f,  0.5f, 0.5f),

                // Back (-Z)
                new( 0.5f, -0.5f, -0.5f), new(-0.5f, -0.5f, -0.5f),
                new(-0.5f,  0.5f, -0.5f), new( 0.5f,  0.5f, -0.5f),
            };

            Vector2[] uvs =
            {
                // Right (+X): U = Z, V = Y
                new(0, 0), new(0, 1), new(1, 1), new(1, 0),

                // Left (-X): U = Z, V = Y
                new(0, 0), new(0, 1), new(1, 1), new(1, 0),

                // Top (+Y): U = X, V = Z
                new(0, 0), new(0, 1), new(1, 1), new(1, 0),

                // Bottom (-Y): U = X, V = Z
                new(0, 0), new(0, 1), new(1, 1), new(1, 0),

                // Front (+Z): U = X, V = Y
                new(0, 0), new(1, 0), new(1, 1), new(0, 1),

                // Back (-Z): U = X, V = Y
                new(0, 0), new(1, 0), new(1, 1), new(0, 1),
            };

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.subMeshCount = 6;
            for (int i = 0; i < 6; i++)
            {
                int b = i * 4;
                mesh.SetTriangles(new int[] { b, b + 1, b + 2, b, b + 2, b + 3 }, i);
            }
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Material CreateTextureMaterial(Texture2D tex)
        {
            Material mat = new(Shader.Find("Standard"));
            mat.SetTexture("_MainTex", tex);
            return mat;
        }
    }
}
