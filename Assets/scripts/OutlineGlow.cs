using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineGlow : MonoBehaviour
{
    [Header("Configuración del borde")]
    public Color colorBorde = Color.green;
    [Range(0.001f, 0.2f)]
    public float grosor = 0.1f;

    private GameObject bordeObj;
    private Material bordeMaterial;

    void Start()
    {
        CrearBorde();
    }

    void CrearBorde()
    {
        // Buscar el renderer principal (puede ser MeshRenderer o SkinnedMeshRenderer)
        Renderer originalRenderer = GetComponent<Renderer>();
        if (originalRenderer == null)
        {
            Debug.LogWarning("OutlineGlow: No se encontró un Renderer en el objeto.");
            return;
        }

        // Crear objeto duplicado para el borde
        bordeObj = new GameObject("OutlineBorde");
        bordeObj.transform.SetParent(transform, false);

        // Copiar el tipo de renderer según corresponda
        Renderer bordeRenderer;

        if (originalRenderer is SkinnedMeshRenderer)
        {
            var original = originalRenderer as SkinnedMeshRenderer;
            var skinned = bordeObj.AddComponent<SkinnedMeshRenderer>();
            skinned.sharedMesh = original.sharedMesh;
            skinned.rootBone = original.rootBone;
            skinned.bones = original.bones;
            bordeRenderer = skinned;
        }
        else
        {
            var original = originalRenderer as MeshRenderer;
            var filterOriginal = GetComponent<MeshFilter>();
            if (filterOriginal == null)
            {
                Debug.LogWarning("OutlineGlow: No se encontró MeshFilter para duplicar.");
                return;
            }
            var filter = bordeObj.AddComponent<MeshFilter>();
            filter.sharedMesh = filterOriginal.sharedMesh;
            bordeRenderer = bordeObj.AddComponent<MeshRenderer>();
        }

        // Crear el material del borde
        bordeMaterial = new Material(Shader.Find("Custom/OutlineGlowShader"));
        bordeMaterial.SetColor("_OutlineColor", colorBorde);
        bordeMaterial.SetFloat("_OutlineWidth", grosor);

        bordeRenderer.material = bordeMaterial;

        // Ajustar capa y visibilidad
        bordeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        bordeRenderer.receiveShadows = false;
    }

    void OnDestroy()
    {
        if (bordeObj != null)
            Destroy(bordeObj);
    }
}
