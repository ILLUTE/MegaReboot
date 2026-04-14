using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkHUD : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_InputField;
    private NetworkManager m_NetworkManager;

    private void Start()
    {
        m_NetworkManager = NetworkManager.Singleton;
    }

    public void StartServer()
    {
        UnityTransport utp = m_NetworkManager.GetComponent<UnityTransport>();
        utp.ConnectionData.Address = "0.0.0.0";
        utp.ConnectionData.Port = (ushort)7777;
        m_NetworkManager.StartHost();

        Debug.Log($"Starting Server @ {utp.ConnectionData.Address} : {utp.ConnectionData.Port}");

    }

    public void StartClient()
    {
        UnityTransport utp = m_NetworkManager.GetComponent<UnityTransport>();
        utp.ConnectionData.Address = m_InputField.text;
        utp.ConnectionData.Port = (ushort)7777;
        m_NetworkManager.StartClient();
    }
}
