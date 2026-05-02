using InventoryFramework;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace mszcubemod
{
    public class Core : MelonMod
    {
        GameObject playerCamera;
        GameObject ghostCube;
        MeshRenderer ghostCubeMeshRenderer;

        public static Vector3 DefaultCubeSize => new Vector3(0.5f, 0.5f, 0.5f);
        public static readonly string ModResources = Path.Combine(MelonEnvironment.ModsDirectory, "Zerocraft");
        const string cubeName = "cube-2guyfhgweybvgfijbneurnbv";

        List<Block> blocks;
        Block activeBlock;

        public override void OnInitializeMelon()
        {
            blocks = BlockLoader.LoadAll();

            foreach (Block block in blocks)
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
                activeBlock = blocks.FirstOrDefault(b => b.Id == item.Definition.Id);
                if (activeBlock == null) return;

                if (ghostCube == null)
                {
                    ghostCube = CreateCube(Vector3.zero, activeBlock.Texture, activeBlock.Size);
                    ghostCubeMeshRenderer = ghostCube.GetComponent<MeshRenderer>();
                    ghostCubeMeshRenderer.material.color = new Color(0f, .8f, 1f, .5f);
                    ghostCube.GetComponent<BoxCollider>().enabled = false;
                    ghostCube.name = "cubePreview";
                }
                else
                {
                    ghostCube.SetActive(true);
                    ghostCubeMeshRenderer.material.SetTexture("_MainTex", activeBlock.Texture);
                }
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

            if (Input.GetMouseButtonDown(0) && activeBlock != null)
            {
                if (!RaycastFromCamera(out RaycastHit hit)) return;

                GameObject newCube = CreateCube(hit.point, activeBlock.Texture, activeBlock.Size);
                newCube.transform.position = SnapToGrid(hit.point + Vector3.Scale(newCube.transform.localScale * .5f, hit.normal), activeBlock.Size);
            }
            if (Input.GetMouseButtonDown(1))
            {   
                if (!RaycastFromCamera(out RaycastHit hit)) return;

                if (!hit.transform) return;
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.name == cubeName)
                    GameObject.Destroy(hitObj);
            }
            if (ghostCube && activeBlock != null)
            {
                if (RaycastFromCamera(out RaycastHit hit))
                {
                    ghostCube.transform.position =
                        SnapToGrid(hit.point + Vector3.Scale(ghostCube.transform.localScale * .5f, hit.normal), activeBlock.Size);
                }
            }
        }
        public bool RaycastFromCamera(out RaycastHit hit)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            return Physics.Raycast(ray, out hit, 4f);
        }
        public GameObject CreateCube(Vector3 positon, Texture2D texture, Vector3 scale)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = positon;
            cube.transform.localScale = scale;
            cube.name = cubeName;
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();

            Shader shader = Shader.Find("Standard");
            if (shader == null) return null;

            Material mat = new Material(shader);
            mat.SetTexture("_MainTex", texture);

            renderer.material = mat;
            return cube;
        }
        public Vector3 SnapToGrid(Vector3 position, Vector3 snapGrid)
        {
            Vector3 snapped = new Vector3(
                Mathf.Round(position.x / snapGrid.x) * snapGrid.x,
                Mathf.Round(position.y / snapGrid.y) * snapGrid.y,
                Mathf.Round(position.z / snapGrid.z) * snapGrid.z);
            return snapped;
        }
    }
}
