using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RenderZooEntry : MonoBehaviour
{
    [Serializable]
    public struct GameObjectAndBonePath
    {
        public GameObject Game;
        public string BonePath;
    }
    
    [SerializeField]
    private GameObjectAndBonePath _targetPrototype;
    
    [SerializeField]
    private bool _isHeadOn;
    
    [SerializeField]
    private GameObject _zooBox;
    
    [SerializeField]
    private CinemachineVirtualCamera _headOnCam;
    
    [SerializeField]
    private CinemachineVirtualCamera _sideCam;
    
    [SerializeField]
    private Camera _camera;

    public GameObjectAndBonePath Target
    {
        get => _targetPrototype;
        set
        {
            _targetPrototype = value;
            OnChangeTarget(_targetPrototype);
        }
    }

    private RenderTexture _zooTexture;

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
    
    private void OnChangeTarget(GameObjectAndBonePath target)
    {
        foreach (Transform child in _zooBox.transform)
        {
            if (!Application.isPlaying)
            {
                StartCoroutine(Destroy(child.gameObject));
            }
            else
            {
                GameObject.Destroy(child.gameObject);    
            }
        }

        if (target.Game == null)
        {
            _sideCam.LookAt = null;
            _sideCam.Follow = null;
            _headOnCam.Follow = null;
            _headOnCam.LookAt = null;
        }
        else
        {
            var newTarget = GameObject.Instantiate(target.Game, Vector3.zero, Quaternion.identity, _zooBox.transform);
            _sideCam.LookAt = newTarget.transform.Find(target.BonePath);
            _sideCam.Follow = newTarget.transform;
            _headOnCam.Follow = newTarget.transform;
            _headOnCam.LookAt = newTarget.transform.Find(target.BonePath);
            if (LayerIndex > 0)
            {
                SetLayerRecursively(newTarget, LayerMask.NameToLayer($"Zoo{LayerIndex}"));    
            }
        }
    }

    public bool IsHeadOn
    {
        get => _isHeadOn;
        set
        {
            _isHeadOn = value;
            OnChangeHead(_isHeadOn);
        }
    }

    private void OnChangeHead(bool isHeadOn)
    {
        if (isHeadOn)
        {
            _headOnCam.gameObject.SetActive(true);
            _sideCam.gameObject.SetActive(false);
        }
        else
        {
            _headOnCam.gameObject.SetActive(false);
            _sideCam.gameObject.SetActive(true);
        }
    }

    void OnValidate()
    {
        Init();
    }

    void Init()
    {
        Target = _targetPrototype;
        IsHeadOn = _isHeadOn;
    }

    [SerializeField] private RenderTexture _baseRenderTexture;
    
    // Update is called once per frame
    void Start()
    {
        Init();
        _zooTexture ??= new RenderTexture(_baseRenderTexture);
        _camera.targetTexture = _zooTexture;
    }

    public RenderTexture ZooRenderTexture {
        get
        {
            _zooTexture ??= new RenderTexture(_baseRenderTexture);
            return _zooTexture;
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }
       
        obj.layer = newLayer;
       
        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public int LayerIndex
    {
        get;
        private set;
    }
    public void SetIndex(int i)
    {
        var layerIndex = LayerMask.NameToLayer($"Zoo{i}");
        if (layerIndex == -1)
        {
            throw new Exception($"Layer Zoo{i} not found. Make more.");
        }

        LayerIndex = i;
        SetLayerRecursively(gameObject, layerIndex);
        _camera.cullingMask = LayerMask.GetMask($"Zoo{i}");
    }
}
