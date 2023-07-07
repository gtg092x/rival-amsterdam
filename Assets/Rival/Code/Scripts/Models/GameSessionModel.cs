using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameSessionModel: IDisposable
{
    private float[] _playerHealths;
    private int[] _playerHits;
    private int[] _playerMisses;
    private float _bossHealth;

    public event Action<int, float> OnHealthUpdate;
    public event Action<int, float> OnHitUpdate;
    public event Action<int, float> OnMissUpdate;
    public event Action<float> OnBossHealthUpdate;

    public PlayerSessionModel session;

    public GameSessionModel(PlayerSessionModel sessionModel)
    {
        session = sessionModel;
        _playerHealths = new float[sessionModel.GetPlayerCount()];
        _playerHits = new int[sessionModel.GetPlayerCount()];
        _playerMisses = new int[sessionModel.GetPlayerCount()];
        
        session.OnPlayerAdd += SessionModelOnOnPlayerAdd;
    }

    public void Dispose()
    {
        session.OnPlayerAdd -= SessionModelOnOnPlayerAdd;
    }
    
    public const float DEFAULT_HEALTH = 1f;

    private void SessionModelOnOnPlayerAdd(PlayerSessionModel.PlayerEntry obj)
    {
        _playerHealths = _playerHealths.Append(DEFAULT_HEALTH).ToArray();
        _playerHits = _playerHits.Append(0).ToArray();
        _playerMisses = _playerMisses.Append(0).ToArray();
    }

    public float GetPlayerHealth(int index)
    {
        return _playerHealths[index];
    }
    
    public void SetPlayerHealth(int index, float health)
    {
        _playerHealths[index] = health;
        OnHealthUpdate?.Invoke(index, health);
    }
    
    public void SetPlayerHits(int index, int hit)
    {
        _playerHits[index] = hit;
        OnHitUpdate?.Invoke(index, hit);
    }
    
    public void SetPlayerMiss(int index, int hit)
    {
        _playerMisses[index] = hit;
        OnMissUpdate?.Invoke(index, hit);
    }
    
    public void SetBossHealth(float health)
    {
        _bossHealth = health;
        OnBossHealthUpdate?.Invoke(health);
    }
    
}
