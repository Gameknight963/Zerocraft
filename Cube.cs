using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Il2CppRootMotion.FinalIK.GenericPoser;

namespace mszcubemod
{
    public class Cube : MelonMod
    {
        GameObject kiri;
        GameObject playerCamera;
        string gameRootDirectory = Directory.GetCurrentDirectory();
        string texturePath;
        Texture2D tex;
        GameObject ghostCube;
        MeshRenderer ghostCubeMeshRenderer;
        Vector3 cubeSize = new Vector3(.5f, .5f, .5f);
        public override void OnInitializeMelon()
        {
            texturePath = Path.Combine(gameRootDirectory, "Mods", "Images", "plank.jpg");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {

            if (sceneName != "Version 1.9 POST")
            {
                return;
            }
            kiri = GameObject.Find("Kiri");
            playerCamera = GameObject.Find("playerCamera");

            byte[] fileData = File.ReadAllBytes(texturePath);
            tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            bool success = ImageConversion.LoadImage(tex, fileData);

            if (success)
            {
                tex.name = "CustomCatTexture";
                tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
                LoggerInstance.Msg("[DEBUG] loaded fuck yes");
            }
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
            if (kiri == null) { return; }
            if (Input.GetMouseButtonDown(0) && tex != null)
            {
                RaycastHit hit = RaycastFromCamera();
                GameObject newCube =  CreateCube(hit.point, tex, cubeSize);
                newCube.transform.position = SnapToGrid(hit.point + Vector3.Scale(newCube.transform.localScale * .5f, hit.normal));
                LoggerInstance.Msg(newCube.GetComponent<MeshRenderer>().material.color.a);
            }
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit = RaycastFromCamera();
                if (hit.transform)
                {
                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj.name == "cube-2guyfhgweybvgfijbneurnbv")
                    {
                        GameObject.Destroy(hitObj);
                    }
                }
            }
            if (ghostCube)
            {
                RaycastHit hit = RaycastFromCamera();
                ghostCube.transform.position = SnapToGrid(hit.point + Vector3.Scale(ghostCube.transform.localScale * .5f, hit.normal));
            }
        }
        public RaycastHit RaycastFromCamera()
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, 4f))
            {
                //yes this is the way im checking if it hit or not. i hate structs
                hit.point = new Vector3(6969, 6969, 6969);
            }
            return hit;
        }
        public GameObject CreateCube(Vector3 positon, Texture2D texture, Vector3 scale)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = positon;
            cube.transform.localScale = scale;
            cube.name = "cube-2guyfhgweybvgfijbneurnbv";
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();

            Shader shader = Shader.Find("Standard");
            if (shader == null)
            {
                return null;
            }

            Material mat = new Material(shader);
            mat.SetTexture("_MainTex", texture);

            renderer.material = mat;

            renderer.material = mat;
            return cube;
        }
        public Vector3 SnapToGrid(Vector3 position)
        {
            Vector3 snapped = new Vector3(
                Mathf.Round(position.x / cubeSize.x) * cubeSize.x,
                Mathf.Round(position.y / cubeSize.y) * cubeSize.y,
                Mathf.Round(position.z / cubeSize.z) * cubeSize.z);
            return snapped;
        }
    }
}
