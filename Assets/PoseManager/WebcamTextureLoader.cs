using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class WebcamTextureLoader : MonoBehaviour
{
    private WebCamTexture webCamera;

    public RenderTexture TargetImage;
    // Start is called before the first frame update
    public bool PlayOnStart = false;
    void Start()
    {
        if (TargetImage == null)
        {
            return;
        }
        webCamera = new WebCamTexture(TargetImage.width, TargetImage.height);
        if (PlayOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        webCamera.Play();
    }

    public void Stop()
    {
        webCamera.Stop();
    }

    [Serializable]
    public class WebCamTickEvent : UnityEvent<Texture>
    {
    }

    public WebCamTickEvent OnTick;
    // Update is called once per frame
    void Update()
    {
        if (webCamera != null)
        {
            OnTick?.Invoke(webCamera);
            if (webCamera.isPlaying)
            {
                Graphics.Blit(webCamera, TargetImage);
            }
        }
    }
}
