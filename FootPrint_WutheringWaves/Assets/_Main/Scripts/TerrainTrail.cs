using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainTrail : MonoBehaviour
{
    public Terrain terrain;
    public Material terrainMat;
    public Material terrainTrailHeightMat;

    private void Start()
    {
        CustomRenderTexture customRT = new CustomRenderTexture(1024, 1024);
        customRT.material = terrainTrailHeightMat;
        customRT.dimension = TextureDimension.Tex2D;
        customRT.format = RenderTextureFormat.R8;
        customRT.updateMode = CustomRenderTextureUpdateMode.Realtime;
        customRT.doubleBuffered = true;
        customRT.Initialize();

        terrain.materialTemplate = terrainMat;
        terrain.materialTemplate.SetTexture("", customRT);
    }

    private void Update()
    {
        UpdateTrails();
    }

    private void UpdateTrails()
    {

    }
}
