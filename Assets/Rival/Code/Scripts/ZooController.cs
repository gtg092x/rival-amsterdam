using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZooController : MonoBehaviour
{
    [SerializeField]
    private GameObject _renderProto;
    
    public int MaxLayers = 7;
    
    private Dictionary<GameObject, RenderZooEntry> _entries;
    
    void Start()
    {
        var allEntries = FindObjectsByType<RenderZooEntry>(FindObjectsSortMode.None).ToArray();
        _entries = new Dictionary<GameObject, RenderZooEntry>();
        foreach (var entry in allEntries)
        {
            _entries[entry.Target.Game] = entry;
            entry.SetIndex(_entries.Count);
        }

    }

    public RenderZooEntry AddToZoo(AvatarObject avatar)
    {
        var entry = GameObject.Instantiate(_renderProto, Vector3.one, Quaternion.identity, this.transform);
        var zooEntry = entry.GetComponent<RenderZooEntry>();
        zooEntry.Target = new RenderZooEntry.GameObjectAndBonePath()
        {
            Game = avatar.Prefab,
            BonePath = avatar.BoneFocus
        };
        _entries[avatar.Prefab] = zooEntry;
        var allIndexes = _entries.Values
            .OrderBy(x => x.LayerIndex)
            .Select(x => x.LayerIndex);
        var prev = 0;
        bool isSet = false;
        foreach (var val in allIndexes)
        {
            if (val - prev > 1)
            {
                zooEntry.SetIndex(prev + 1);
                isSet = true;
                break;
            }
            prev = val;
        }

        if (prev < MaxLayers && !isSet)
        {
            zooEntry.SetIndex(prev + 1);
        }

        return zooEntry;
    }
    
    public RenderTexture GetRender(AvatarObject avatar, bool isHeadOn = false)
    {
        if (!_entries.ContainsKey(avatar.Prefab))
        {
            var entry = AddToZoo(avatar);
            entry.IsHeadOn = isHeadOn;
            return entry.ZooRenderTexture;
        }
        else
        {
            var entry = _entries[avatar.Prefab];
            entry.IsHeadOn = isHeadOn;
            return entry.ZooRenderTexture;
        }
    }
    
    public void RemoveFromZoo(AvatarObject avatar)
    {
        if (!_entries.ContainsKey(avatar.Prefab))
        {
            return;
        }

        var entry = _entries[avatar.Prefab];
        entry.ZooRenderTexture.Release();
        GameObject.Destroy(entry);
        _entries.Remove(avatar.Prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
