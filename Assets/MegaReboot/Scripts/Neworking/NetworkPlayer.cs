using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsHost && IsOwner)
        {
            NetworkManager.SceneManager.LoadScene("PaintingInArea", LoadSceneMode.Additive);
        }
        FindAnyObjectByType<MenuCanvas>().GetComponent<Canvas>().enabled = false;
    }
    const int CHUNK_SIZE = 1 * 1024;

    private Dictionary<ulong, List<byte[]>> receivedChunks = new();

    public void ReceivedTextureToUpload(Texture2D texture)
    {
        StartCoroutine(UploadTextureRoutine(texture));
    }

    private IEnumerator UploadTextureRoutine(Texture2D texture)
    {
        Texture2D readable = MakeReadable(texture);
        byte[] data = readable.EncodeToJPG(70);
        var chunks = ChunkData(data);

        for (int i = 0; i < chunks.Count; i++)
        {
            yield return null;
            SendTextureChunk_ServerRpc(chunks[i], i, chunks.Count, OwnerClientId);
        }
    }

    List<byte[]> ChunkData(byte[] data)
    {
        List<byte[]> chunks = new List<byte[]>();

        for (int i = 0; i < data.Length; i += CHUNK_SIZE)
        {
            int size = Mathf.Min(CHUNK_SIZE, data.Length - i);
            byte[] chunk = new byte[size];
            System.Buffer.BlockCopy(data, i, chunk, 0, size);
            chunks.Add(chunk);
        }

        return chunks;
    }

    // ---------------- SERVER ----------------

    [ServerRpc]
    private void SendTextureChunk_ServerRpc(byte[] chunk, int index, int totalChunks, ulong senderId)
    {
        Debug.Log("Received Chunk on Server");
        SendTextureChunk_ClientRpc(chunk, index, totalChunks, senderId);
    }

    // ---------------- CLIENT ----------------

    [ClientRpc]
    private void SendTextureChunk_ClientRpc(byte[] chunk, int index, int totalChunks, ulong senderId)
    {
        Debug.Log("Chunk on Client");
        if (!receivedChunks.ContainsKey(senderId))
        {
            receivedChunks[senderId] = new List<byte[]>(new byte[totalChunks][]);
        }

        receivedChunks[senderId][index] = chunk;

        // Check if all chunks arrived
        bool complete = true;
        foreach (var c in receivedChunks[senderId])
        {
            if (c == null)
            {
                complete = false;
                Debug.Log("Chunk Missing");
                break;
            }
        }

        if (complete)
        {
            byte[] fullData = CombineChunks(receivedChunks[senderId]);
            receivedChunks.Remove(senderId);

            ApplyTexture(fullData);
        }
    }

    byte[] CombineChunks(List<byte[]> chunks)
    {
        int totalSize = 0;
        foreach (var c in chunks)
            totalSize += c.Length;

        byte[] result = new byte[totalSize];
        int offset = 0;

        foreach (var c in chunks)
        {
            System.Buffer.BlockCopy(c, 0, result, offset, c.Length);
            offset += c.Length;
        }

        return result;
    }

    void ApplyTexture(byte[] data)
    {
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(data);

        WhiteboardHandler.Instance.m_Whiteboard.UpdateTexture(tex);

        Debug.Log("Trying to Apply Texture");
    }

    Texture2D MakeReadable(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTex = new Texture2D(source.width, source.height);
        readableTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTex;
    }
}