using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FootPrintData
{
    public Vector2 uv;
    public float startTime;
}

public class FootPrint : MonoBehaviour
{
    public Camera mainCamera;
    public Terrain terrain;
    public Material drawFootprintMaterial;

    public float showTime = 3f;
    public float fadeOutTime = 3f;
    private RenderTexture footprintNormalRT;

    private Material material;
    private List<FootPrintData> footPrintDatas = new List<FootPrintData>();

    void Start()
    {
        material = terrain.materialTemplate;

        footprintNormalRT = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
        footprintNormalRT.Create();

        Vector4[] uvs = Enumerable.Repeat(new Vector4(-1f, -1f, 0f, 0f), 100).ToArray();
        drawFootprintMaterial.SetVectorArray("_FootUVArray", uvs);

        Texture2D flatNormal = new Texture2D(1, 1);
        flatNormal.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
        flatNormal.Apply();
        Graphics.Blit(flatNormal, footprintNormalRT);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector2 uv = hitInfo.textureCoord;
                footPrintDatas.Add(new FootPrintData() { uv = uv, startTime = Time.time });
            }
        }

        UpdateFootprints();
    }

    private void UpdateFootprints()
    {
        float curTime = Time.time;
        footPrintDatas = footPrintDatas.Where(p => curTime - p.startTime < (showTime + fadeOutTime)).ToList();

        int count = footPrintDatas.Count;
        if (count <= 0) return;

        List<Vector4> datas = new List<Vector4>();
        for(int i = 0; i < count; i++)
        {
            float duration = curTime - footPrintDatas[i].startTime < showTime ? 1f : 1 - (curTime - footPrintDatas[i].startTime - showTime) / fadeOutTime;
            datas.Add(new Vector4(footPrintDatas[i].uv.x, footPrintDatas[i].uv.y, duration));
        }
        drawFootprintMaterial.SetInt("_FootprintCount", count);
        drawFootprintMaterial.SetVectorArray("_FootUVArray", datas);

        Graphics.Blit(null, footprintNormalRT, drawFootprintMaterial);

        terrain.materialTemplate.SetTexture("_OverlayNormalMap", footprintNormalRT);
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(256, 0, 256, 256), footprintNormalRT, ScaleMode.ScaleToFit);
    }
}
