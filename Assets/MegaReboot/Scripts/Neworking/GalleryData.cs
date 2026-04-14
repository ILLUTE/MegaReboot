using UnityEngine;

public class GalleryData : MonoBehaviour
{
    NativeGallery.MediaPickCallback mCallback;
   public void OpenGallery()
    {
        NativeGallery.GetImageFromGallery((path) => {

            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path);

                if(texture == null)
                {
                    Debug.LogError("Couldn't Load Texture");
                    return;
                }

                WhiteboardHandler.Instance.m_Whiteboard.UpdateTexture(texture);
                Debug.Log("Sending Texture to Player");
                OwnerPlayerHandler.Instance.OwnerLocalPlayer.ReceivedTextureToUpload(texture);
            }
        
        });
    }
}
