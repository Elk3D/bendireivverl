using UnityEngine;

public class FPModelLayers : FPMonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] private string m_CutsceneLayer;
    [SerializeField] private GameObject[] m_CutsceneLayers;

    [Header("First Person (Mirror)")]
    [SerializeField] private string m_MirrorLayer;
    [SerializeField] private GameObject[] m_MirrorLayers;

    [Header("3rd Person / Default")]
    [SerializeField] private string m_DefaultLayer;
    [SerializeField] private GameObject[] m_AllLayers;

    [Header("Head")]
    [SerializeField] private string m_HeadLayer;
    [SerializeField] private GameObject[] m_HeadLayers;

    public void EnableAll()       => SetLayer(ref m_AllLayers, m_DefaultLayer);
    public void EnableFirstPerson() => SetLayer(ref m_MirrorLayers, m_MirrorLayer);
    public void EnableCutscene()  => SetLayer(ref m_CutsceneLayers, m_CutsceneLayer);
    public void DisableHead()     => SetLayer(ref m_HeadLayers, m_HeadLayer);

    private void SetLayer(ref GameObject[] objects, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        foreach (var go in objects) if (go != null) go.layer = layer;
    }
}
