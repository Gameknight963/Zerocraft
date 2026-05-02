using MelonLoader;
using MelonLoader.Utils;
using System.IO;
using UnityEngine;

namespace mszcubemod
{
    public class Mszcubemod : MelonMod
    {
        GameObject kiri;
        GameObject playerCamera;
        string texturePath;
        Texture2D tex;
        GameObject ghostCube;
        MeshRenderer ghostCubeMeshRenderer;
        Vector3 cubeSize = new Vector3(.5f, .5f, .5f);

        const string cubeName = "cube-2guyfhgweybvgfijbneurnbv";
        public override void OnInitializeMelon()
        {
            texturePath = Path.Combine(MelonEnvironment.ModsDirectory, "Zerocraft", "plank.jpg");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {

            if (sceneName != "Version 1.9 POST") return;

            kiri = GameObject.Find("Kiri");
            playerCamera = GameObject.Find("playerCamera");

            byte[] fileData = File.ReadAllBytes(texturePath);
            tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(tex, fileData);
            tex.hideFlags = HideFlags.DontUnloadUnusedAsset;

            if (!ghostCube)
            {
                ghostCube = CreateCube(new Vector3(0, 0, 0), tex, cubeSize);
                ghostCubeMeshRenderer = ghostCube.GetComponent<MeshRenderer>();
                ghostCubeMeshRenderer.material.color = new Color(0f, .8f, 1f, .5f);
                ghostCube.GetComponent<BoxCollider>().enabled = false;
                ghostCube.name = "cubePreview";
            }
        }

        public override void OnUpdate()
        {
            if (kiri == null) return;

            if (Input.GetMouseButtonDown(0) && tex != null)
            {
                if (!RaycastFromCamera(out RaycastHit hit)) return;

                GameObject newCube = CreateCube(hit.point, tex, cubeSize);
                newCube.transform.position = SnapToGrid(hit.point + Vector3.Scale(newCube.transform.localScale * .5f, hit.normal), cubeSize);
                LoggerInstance.Msg(newCube.GetComponent<MeshRenderer>().material.color.a);
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (!RaycastFromCamera(out RaycastHit hit)) return;

                if (!hit.transform) return;
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.name == cubeName)
                    GameObject.Destroy(hitObj);
            }
            if (ghostCube)
            {
                if (RaycastFromCamera(out RaycastHit hit))
                {
                    ghostCube.transform.position =
                        SnapToGrid(hit.point + Vector3.Scale(ghostCube.transform.localScale * .5f, hit.normal), cubeSize);
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
