using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpatialUIMarkers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Clear();
        _markers = new Dictionary<string, GameObject>();
        _lines = new Dictionary<(int, int), GameObject>();
        Joints = new (int, int)[]{};
    }

    public GameObject MarkerProto;

    [Serializable]
    public class MarkerData
    {
        public float x;
        public float y;
        public Color Color;
        public string Name;
        public bool isVis;
    }

    public MarkerData[] Markers;
    private Dictionary<string, GameObject> _markers;
    private Dictionary<(int,int), GameObject> _lines;
    private Dictionary<string, Vector3> _newPositions;
    public Vector2 Scale = Vector2.one;

    public (int, int)[] Joints;
    const float LOW_CONFIDENCE_ALPHA = 0.2f;
    public RawImage Source;
    void Draw()
    {
        if (Source == null || MarkerProto == null)
        {
            return;
        }
        var rt = GetComponent<RectTransform>();
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        var bottomLeft = v[0];
        var topRight = v[2];
        var size = topRight - bottomLeft;
        if (_newPositions == null)
        {
            _newPositions = new Dictionary<string, Vector3>();
        }

        
        
        for (int i = 0; i < Markers.Length; i++)
        {
            var marker = Markers[i];
            if (_markers != null && !_markers.ContainsKey(marker.Name))
            {
                _markers[marker.Name] = GameObject.Instantiate(MarkerProto, this.transform);
            }
            var markerObj = _markers != null 
                ? _markers[marker.Name] 
                : GameObject.Instantiate(MarkerProto, this.transform);

            if (!string.IsNullOrEmpty(marker.Name))
            {
                markerObj.name = marker.Name;    
            }
            
            var rend = markerObj.GetComponent<Renderer>();
            if (Application.isPlaying)
            {
                var mat  = rend.material;
                var color = marker.Color;
                if (isVis(marker, Markers))
                {
                    color.a = LOW_CONFIDENCE_ALPHA;
                }
                mat.color = color;
                
                 
            }

            var x = (marker.x * Scale.x) * size.x + bottomLeft.x;
            var y = (marker.y * Scale.y) * size.y + bottomLeft.y;
            var z = 0.5f * size.z + bottomLeft.z;
            if (!Application.isPlaying)
            {
                markerObj.transform.position = new Vector3(x, y, z);  
            }
            else
            {
                _newPositions[marker.Name] = new Vector3(x, y, z);    
            }
        }
        if (Joints != null)
        for (int i = 0; i < Joints.Length; i++)
        {
            var joint = Joints[i];
            if (_lines != null && !_lines.ContainsKey(joint))
            {
                _lines[joint] = new GameObject("Line");
                _lines[joint].transform.parent = this.transform;
                var line = _lines[joint].AddComponent<LineRenderer>();
                line.material = LineMaterial;
                line.startColor = Color.white;
                line.endColor = Color.white;
                line.startWidth = 3f;
                line.endWidth = 3f;
                line.positionCount = 2;
                line.useWorldSpace = true;    
            }

            if (_lines != null)
            {
                var line = _lines[joint];
                var lineRenderer = line.GetComponent<LineRenderer>();
                var startLabel = Markers[joint.Item1].Name;
                var endLabel = Markers[joint.Item2].Name;

                if (!isVis(Markers[joint.Item1], Markers) || !isVis(Markers[joint.Item2], Markers))
                {
                    var mat  = lineRenderer.material;
                    var color = mat.color;
                    color.a = LOW_CONFIDENCE_ALPHA;
                    mat.color = color;
                }
                lineRenderer.SetPosition(0, _markers[startLabel].transform.position);
                lineRenderer.SetPosition(1, _markers[endLabel].transform.position);
            }
        }

        if (_lerp != null)
        {
            StopCoroutine(_lerp);
        }

        if (Application.isPlaying)
        {
            _lerp = StartCoroutine(LerpLocations(_markers, _newPositions));    
        }
        
    }

    private bool isVis(MarkerData marker, MarkerData[] all)
    {
        return marker.isVis;
    }

    public Material LineMaterial;

    private IEnumerator LerpLocations(Dictionary<string, GameObject> markers, Dictionary<string, Vector3> newPositions)
    {
        float t = 0f;
        var currentPoses = markers.ToDictionary(x =>
        
            x.Key, x=> x.Value.transform.position
        );
        while (t <= 1f)
        {
            foreach (var kvp in markers)
            {
                var targetPos = newPositions[kvp.Key];
                var startPos = currentPoses[kvp.Key];
                kvp.Value.transform.position = Vector3.Lerp(startPos, targetPos, t);
            }
            t += Time.deltaTime * SCALE;
            yield return new WaitForEndOfFrame();
        }
    }

    public float SCALE = 1f;

    private Coroutine _lerp;

    private void OnValidate()
    {
        if (!Application.isPlaying)
        Redraw();
    }

    void Clear()
    {
        foreach (Transform child in transform)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode )
            {
                UnityEditor.EditorApplication.delayCall+=()=>
                {
                    DestroyImmediate(child.gameObject);
                };
            }
            else
            {
                Destroy(child.gameObject);    
            }
            
        }
    }

    void Redraw()
    {
        Clear();
        Draw();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMarkers(MarkerData[] data, (int, int)[] joints)
    {
        Markers = data;
        Joints = joints;
        Draw();
    }
}
