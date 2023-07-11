using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectEmitterManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _hitPrefab;
    
    [SerializeField]
    private GameObject _switchPrefab;
    
    [SerializeField]
    private GameManager _gameManager;

    private void OnValidate()
    {
        _gameManager ??= FindAnyObjectByType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager ??= FindAnyObjectByType<GameManager>();
        BindGameEvents();
    }

   

    private void OnDestroy()
    {
        UnbindGameEvents();
    }

    private void BindGameEvents()
    {
        _gameManager.GetSessionManager().OnBossHealthUpdate.AddListener(HandleHit);
        _gameManager.GetSessionManager().OnPlayerBlur.AddListener(HandlePlayerFocus);
    }

    private void HandlePlayerFocus(PlayerSessionModel.PlayerEntry arg0)
    {
        if (_gameManager.IsAlive(arg0))
        {
            GameObject.Destroy(GameObject.Instantiate(_switchPrefab, this.transform), 4f);
        }
        
    }

    private void HandleHit(float arg0)
    {
        GameObject.Destroy(GameObject.Instantiate(_hitPrefab, this.transform), 4f);
    }

    private void UnbindGameEvents()
    {
        _gameManager.GetSessionManager().OnBossHealthUpdate.RemoveListener(HandleHit);
        _gameManager.GetSessionManager().OnPlayerBlur.RemoveListener(HandlePlayerFocus);
    }
}
