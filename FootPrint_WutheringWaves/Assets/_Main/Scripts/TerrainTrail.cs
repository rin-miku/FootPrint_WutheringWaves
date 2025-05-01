using UnityEngine;
using UnityEngine.Rendering;

public class TerrainTrail : MonoBehaviour
{
    public Terrain terrain;
    public Material terrainMat;
    public Material terrainTrailHeightMat;
    public Transform playerTransform;

    private CustomRenderTexture customRT;

    private void Start()
    {
        customRT = new CustomRenderTexture(2048, 2048);
        customRT.material = terrainTrailHeightMat;
        customRT.dimension = TextureDimension.Tex2D;
        customRT.format = RenderTextureFormat.R8;
        customRT.updateMode = CustomRenderTextureUpdateMode.Realtime;
        customRT.doubleBuffered = true;
        customRT.Create();
        customRT.Initialize();

        terrain.materialTemplate = terrainMat;
        terrain.materialTemplate.SetTexture("_HeightMap", customRT);
    }

    private void Update()
    {
        UpdateTrail();
    }

    private void UpdateTrail()
    {
        Ray ray = new Ray(playerTransform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.tag.Equals("Ground"))
            {
                Vector2 hitCoord = hitInfo.textureCoord;
                float angle = playerTransform.rotation.eulerAngles.y;

                terrainTrailHeightMat.SetVector("_TrialPosition", hitCoord);
                terrainTrailHeightMat.SetFloat("_TrailAngle", angle);
            }
        }

        customRT.Update();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), customRT, ScaleMode.ScaleToFit, false);
    }
}
