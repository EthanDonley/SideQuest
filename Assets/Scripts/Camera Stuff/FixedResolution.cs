using UnityEngine;

public class FixedResolution : MonoBehaviour
{
    public int targetWidth = 64;
    public int targetHeight = 64;

    void Start()
    {
        // Set the resolution
        Screen.SetResolution(targetWidth, targetHeight, false);
        Camera.main.orthographicSize = targetHeight / 2;

        // Lock the aspect ratio to 1:1
        float targetAspect = (float)targetWidth / (float)targetHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = Camera.main;

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
