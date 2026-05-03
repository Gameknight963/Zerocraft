using InventoryFramework;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mszcubemod
{
    record PlacedBlock(string BlockId, Vector3 Position, GameObject Object);

    public class Core : MelonMod
    {
        GameObject? playerCamera;
        GameObject? ghostCube;

        public static Vector3 DefaultCubeSize => new(1f, 1f, 1f);
        public static readonly string ModResources = Path.Combine(MelonEnvironment.ModsDirectory, "Zerocraft");
        const string cubeName = "cube-2guyfhgweybvgfijbneurnbv";

        Block? activeBlock;

        /// <summary>
        /// do not edit this field directly, use SetPreviewMode()
        /// </summary>
        bool _previewMode = false;

        /// <param name="preview">true for ghost preview, false for wireframe</param>
        void SetPreviewMode(bool preview)
        {
            _previewMode = preview;
            if (ghostCube != null)
                UnityEngine.Object.Destroy(ghostCube);
            if (activeBlock == null) return;
            ghostCube = _previewMode
                ? CreateCube(Vector3.zero, activeBlock.Texture, activeBlock.Size)
                : CreateWireframeCube(Vector3.zero, activeBlock.Size);
            ghostCube.SetActive(false);
            if (!_previewMode)
            {
                BoxCollider? collider = ghostCube.GetComponent<BoxCollider>();
                if (collider != null)
                    collider.enabled = false;
            }
        }

        static void PlaceBlock(string blockId, Vector3 position)
        {
            WorldManager.Instance.PlaceBlock(blockId, position);
            CubeNetworking.Instance?.SendPlace(blockId, position);
        }

        static void DeleteBlock(Vector3 position)
        {
            WorldManager.Instance.DeleteBlock(position);
            CubeNetworking.Instance?.SendDelete(position);
        }

        public override void OnInitializeMelon()
        {
            WorldManager.Instance.Initialize(BlockLoader.LoadAll());
            CubeNetworking.Init();

            foreach (Block block in WorldManager.Instance.Blocks)
            {
                Sprite sprite = Sprite.Create(block.Texture, new Rect(0, 0, block.Texture.width, block.Texture.height), new Vector2(0.5f, 0.5f));
                InventoryManager.Instance.RegisterItem(new ItemDefinition(block.Id, block.Name, sprite));
            }

            InventoryManager.Instance.OnItemSelected += item =>
            {
                if (item == null)
                {
                    activeBlock = null;
                    ghostCube?.SetActive(false);
                    return;
                }

                activeBlock = WorldManager.Instance.Blocks.FirstOrDefault(b => b.Id == item.Definition.Id);
                if (activeBlock == null) return;

                SetPreviewMode(_previewMode);
            };
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Version 1.9 POST") return;
            playerCamera = GameObject.Find("playerCamera");
        }

        public override void OnUpdate()
        {
            if (playerCamera == null) return;
            if (SceneManager.GetActiveScene().name != "Version 1.9 POST") return;

            if (Input.GetMouseButtonDown(1) && activeBlock != null)
            {
                if (!RaycastFromCamera(out RaycastHit hit)) return;
                Vector3 position = SnapToGrid(hit.point + Vector3.Scale(activeBlock.Size * .5f, hit.normal), activeBlock.Size);
                PlaceBlock(activeBlock.Id, position);
            }

            if (Input.GetMouseButtonDown(0) && activeBlock != null)
            {
                if (!RaycastFromCamera(out RaycastHit hit)) return;
                if (!hit.transform) return;
                if (hit.collider.gameObject.name != cubeName) return;
                DeleteBlock(hit.transform.position);
            }

            if (ghostCube != null && activeBlock != null)
            {
                if (RaycastFromCamera(out RaycastHit hit))
                {
                    ghostCube.SetActive(true);
                    if (_previewMode)
                    {
                        ghostCube.transform.position =
                            SnapToGrid(hit.point + Vector3.Scale(ghostCube.transform.localScale * .5f, hit.normal), activeBlock.Size);
                    }
                    else
                    {
                        if (hit.transform.gameObject.name != cubeName) return;
                        ghostCube.transform.position = hit.transform.position;
                    }
                }
                else ghostCube.SetActive(false);
            }
        }

        public bool RaycastFromCamera(out RaycastHit hit)
        {
            if (playerCamera is null) throw new InvalidOperationException();
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            return Physics.Raycast(ray, out hit, 4f);
        }

        public static GameObject CreateCube(Vector3 position, Texture2D texture, Vector3 scale)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.name = cubeName;
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
            Material mat = new(Shader.Find("Standard"));
            mat.SetTexture("_MainTex", texture);
            renderer.material = mat;
            return cube;
        }

        public static Vector3 SnapToGrid(Vector3 position, Vector3 snapGrid)
        {
            return new Vector3(
                Mathf.Round(position.x / snapGrid.x) * snapGrid.x,
                Mathf.Round(position.y / snapGrid.y) * snapGrid.y,
                Mathf.Round(position.z / snapGrid.z) * snapGrid.z);
        }

        public GameObject CreateWireframeCube(Vector3 position, Vector3 size)
        {
            GameObject obj = new GameObject("cubePreview");
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
    }
}