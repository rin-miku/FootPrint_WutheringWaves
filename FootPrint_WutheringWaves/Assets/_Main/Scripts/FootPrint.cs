using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class FootPrintData
{
    public Vector2 uv;
    public float angle;
    public float startTime;
}

public class FootPrint : MonoBehaviour
{
    public Terrain terrain;
    public Animator animator;
    public Material footprintMat;
    public float showTime = 3f;
    public float fadeOutTime = 3f;
    public List<Transform> footsTransform;

    private CustomRenderTexture customRT;
    private float lastPivotWeight;
    private Material terrainMat;
    private List<FootPrintData> footPrintDatas = new List<FootPrintData>();

    void Start()
    {
        terrainMat = terrain.materialTemplate;
        
        customRT = new CustomRenderTexture(2048, 2048);
        customRT.material = footprintMat;
        customRT.initializationColor = new Color(0f, 0f, 0f, 0f);
        customRT.dimension = TextureDimension.Tex2D;
        customRT.format = RenderTextureFormat.ARGB32;
        customRT.updateMode = CustomRenderTextureUpdateMode.Realtime;
        customRT.doubleBuffered = true;
        customRT.Create();
        customRT.Initialize();

        Vector4[] uvs = Enumerable.Repeat(new Vector4(-1f, -1f, 0f, 0f), 100).ToArray();
        footprintMat.SetVectorArray("_FootUVArray", uvs);

        terrainMat.SetTexture("_OverlayNormalMap", customRT);
        
    }

    void Update()
    {
        UpdateFootprint();
    }

    private void UpdateFootprint()
    {
        CheckPivotWeight();

        UpdateFootPrintDatas();

        customRT.Update();
    }

    private void CheckPivotWeight()
    {
        float pivotWeight = animator.pivotWeight;
        float tempPivotWeight = Mathf.Abs(pivotWeight - 0.5f);

        if (tempPivotWeight > 0.4f && lastPivotWeight < 0.4f)
        {

            Transform foot = footsTransform[pivotWeight >= 0.5 ? 0 : 1];
            Ray ray = new Ray(foot.transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.transform.tag.Equals("Ground"))
                {
                    Vector2 hitCoord = hitInfo.textureCoord;
                    float angle = animator.transform.parent.rotation.eulerAngles.y - 90f;
                    footPrintDatas.Add(new FootPrintData() { uv = hitCoord, angle = angle, startTime = Time.time });
                }
            }
        }

        lastPivotWeight = tempPivotWeight;
    }

    private void UpdateFootPrintDatas()
    {
        float curTime = Time.time;

        footPrintDatas = footPrintDatas.Where(p => curTime - p.startTime < (showTime + fadeOutTime)).ToList();
        int count = footPrintDatas.Count;
        if (count <= 0) return;

        List<Vector4> datas = new List<Vector4>();
        for (int i = 0; i < count; i++)
        {
            float duration = curTime - footPrintDatas[i].startTime < showTime ? 1f : 1 - (curTime - footPrintDatas[i].startTime - showTime) / fadeOutTime;
            datas.Add(new Vector4(footPrintDatas[i].uv.x, footPrintDatas[i].uv.y, duration, footPrintDatas[i].angle));
        }
        footprintMat.SetInt("_FootprintCount", count);
        footprintMat.SetVectorArray("_FootUVArray", datas);
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(256, 0, 256, 256), customRT, ScaleMode.ScaleToFit);
    }
}
