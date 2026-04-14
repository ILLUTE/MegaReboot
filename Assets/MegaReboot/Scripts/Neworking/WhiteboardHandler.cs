using UnityEngine;

public class WhiteboardHandler : MonoBehaviour
{
    private static WhiteboardHandler instance;

    public static WhiteboardHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<WhiteboardHandler>();
            }
            return instance;
        }
    }

    public Whiteboard m_Whiteboard;
}
