using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    [SerializeField] private MeshRenderer m_MeshRenderer;

    public void UpdateTexture(Texture texture)
    {
        m_MeshRenderer.material.mainTexture = texture;
    }
}
