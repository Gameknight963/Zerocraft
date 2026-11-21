using InventoryFramework;
using MelonLoader;
using MelonLoader.Utils;
using mszcubemod.Components;
using mszcubemod.Networking;
using mszcubemod.Rendering;
using mszcubemod.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mszcubemod
{
    public class Core : MelonMod
    {
        GameObject? playerCamera;
        GameObject? ghostCube;

        public static readonly string ModResources = Path.Combine(MelonEnvironment.ModsDirectory, "Zerocraft");

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
                ? CubeFactory.CreateCube(Vector3.zero, activeBlock.Texture, activeBlock.Size)
                : CubeFactory.CreateWireframeCube(Vector3.zero, activeBlock.Size);
            ghostCube.SetActive(false);
            if (!_previewMode)
            {
                BoxCollider? collider = ghostCube.GetComponent<BoxCollider>();
                collider?.enabled = false;
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
                Texture2D preview = BlockPreviewRenderer.CreateIsometricPreview(block.Texture);
                Sprite sprite = Sprite.Create(preview,
                    new Rect(0, 0, preview.width, preview.height),
                    new Vector2(0.5f, 0.5f));
                InventoryManager.Instance.RegisterItem(
                    new ItemDefinition(block.Id, block.Name, sprite));
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
                if (!hit.collider.TryGetComponent<PlacedBlockComponent>(out _)) return;
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
                        if (!hit.collider.TryGetComponent<PlacedBlockComponent>(out _)) return;
                        ghostCube.transform.position = hit.transform.position;
                    }
                }
                else ghostCube.SetActive(false);
            }
        }

        private bool RaycastFromCamera(out RaycastHit hit)
        {
            if (playerCamera is null) throw new InvalidOperationException();
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            return Physics.Raycast(ray, out hit, 4f);
        }

        private static Vector3 SnapToGrid(Vector3 position, Vector3 snapGrid)
        {
            return new Vector3(
                Mathf.Round(position.x / snapGrid.x) * snapGrid.x,
                Mathf.Round(position.y / snapGrid.y) * snapGrid.y,
                Mathf.Round(position.z / snapGrid.z) * snapGrid.z);
        }
    }
}