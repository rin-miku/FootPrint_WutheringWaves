using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainTrail : MonoBehaviour
{
    public Terrain terrain;
    public Material terrainMat;
    public Material terrainTrailHeightMat;
    public List<Transform> footsTransform;

    private CustomRenderTexture customRT;

    private void Start()
    {
        customRT = new CustomRenderTexture(1024, 1024);
        customRT.material = terrainTrailHeightMat;
        customRT.dimension = TextureDimension.Tex2D;
        customRT.format = RenderTextureFormat.R8;
        customRT.updateMode = CustomRenderTextureUpdateMode.Realtime;
        customRT.doubleBuffered = true;
        customRT.Initialize();

        terrain.materialTemplate = terrainMat;
        terrain.materialTemplate.SetTexture("_HeightMap", customRT);
    }

    private void Update()
    {
        UpdateTrails();

        customRT.Update();
    }

    private void UpdateTrails()
    {
        foreach(Transform foot in footsTransform)
        {
            Ray ray = new Ray(foot.transform.position, Vector3.down);

            if(Physics.Raycast(ray, out RaycastHit hitInfo, 0.3f))
            {
                //Debug.Log(hitInfo.transform.name);
                if (hitInfo.transform.tag.Equals("Ground"))
                {
                    Vector2 hitCoord = hitInfo.textureCoord;
                    float angle = foot.transform.rotation.eulerAngles.y;
                    //Debug.Log($"{hitCoord}:{angle}");

                    terrainTrailHeightMat.SetVector("_TrialPosition", hitCoord);
                    terrainTrailHeightMat.SetFloat("_TrailAngle", angle);
                }
            }
        }
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(10, 10, 256, 256), customRT, ScaleMode.ScaleToFit, false);
    }
}
