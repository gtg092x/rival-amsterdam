using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.MediaPipeParser;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SentisMirror : MonoBehaviour
{
    public SpatialUIMarkers Markers;
    
    void Sync()
    {
        
    }

    private Color[] colors;

    
    private void OnValidate()
    {
        Sync();
    }

    // Update is called once per frame
    void Update()
    {
        Sync();
    }

    void Start()
    {
        colors = new Color[10];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Random.ColorHSV();
        }
    }
    
    public void HandleSentisUpdate(ParseMediaPipe.MediaPipePoseSet results)
    {
        if (Markers == null) return;

        List<SpatialUIMarkers.MarkerData> markerDatas = new List<SpatialUIMarkers.MarkerData>();
        int i = 0;
        foreach (var res in results.Keypoints)
        {
            var color = colors[i % colors.Length];
            i++;
            markerDatas.Add(new SpatialUIMarkers.MarkerData()
            {
                Color = color,
                x = res.Position.x,
                y = res.Position.y,
                Name = res.Label,
                isVis = res.Visibility > 0f
            });
        }
        Markers.SetMarkers(markerDatas.ToArray(), ParseMediaPipe.GetJoints(results));
    }
}
