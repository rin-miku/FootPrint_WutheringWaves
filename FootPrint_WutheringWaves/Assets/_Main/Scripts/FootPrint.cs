using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FootPrint : MonoBehaviour
{
    public Camera mainCamera;
    public MeshRenderer meshRenderer;
    public Material drawFootprintMaterial;
    private RenderTexture footprintNormalRT;
    private Material material;

    void Start()
    {
        material = meshRenderer.materials[0];

        footprintNormalRT = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
        footprintNormalRT.Create();

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
                DrawFootprint(uv);
            }
        }
    }

    void DrawFootprint(Vector2 uv)
    {
        drawFootprintMaterial.SetVector("_FootUV", new Vector4(uv.x, uv.y, 0, 0));

        Graphics.Blit(null, footprintNormalRT, drawFootprintMaterial);

        material.SetTexture("_BumpMap2", footprintNormalRT);
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), footprintNormalRT, ScaleMode.ScaleToFit);
    }
}
