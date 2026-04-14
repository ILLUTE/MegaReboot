using UnityEngine;

public class OwnerPlayerHandler : MonoBehaviour
{
    private static OwnerPlayerHandler instance;

    public static OwnerPlayerHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<OwnerPlayerHandler>();

                if (instance == null)
                {
                    GameObject go = new("OwnerPlayerHandler");
                    instance = go.AddComponent<OwnerPlayerHandler>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }


        if (instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private NetworkPlayer networklocalPlayer;
    public NetworkPlayer OwnerLocalPlayer
    {
        get
        {
            if (networklocalPlayer == null)
            {
                NetworkPlayer[] players = FindObjectsByType<NetworkPlayer>(FindObjectsSortMode.None);

                foreach (NetworkPlayer p in players)
                {
                    if (p.IsLocalPlayer)
                    {
                        networklocalPlayer = p;
                        break;
                    }
                }
            }

            return networklocalPlayer;
        }
    }
}
