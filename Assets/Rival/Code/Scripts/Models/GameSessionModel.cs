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
    private float _bossHealth = DEFAULT_HEALTH;

    public event Action<int, float> OnHealthUpdate;
    public event Action<int, int> OnHitUpdate;
    public event Action<int, float> OnMissUpdate;
    public event Action<float> OnBossHealthUpdate;
    public event Action<int> OnPlayerDead;
    public event Action<int> OnPlayerRez;
    public event Action OnBossDead;
    public event Action OnAllPlayersDead;
    
    public event Action<int> OnPlayerIndexFocus;
    public event Action<int> OnPlayerIndexBlur;
    

    private int _currentPlayerIndex = -1;
    public PlayerSessionModel session;

    public GameLevelModel GetLevelState()
    {
        return _level;
    }
    
    private GameLevelModel _level;

    [Serializable]
    public struct GameConfig
    {
        public float hitCooldown;
    }
    
    public GameSessionModel(PlayerSessionModel sessionModel, GameLevelModel level, GameConfig config)
    {
        session = sessionModel;
        _level = level;
        ResetData();
        session.OnPlayerAdd += SessionModelOnOnPlayerAdd;
        _hitCooldown = config.hitCooldown;
    }

    void ResetData()
    {
        var sessionModel = session; 
        _playerHealths = new float[sessionModel.GetPlayerCount()];
        _playerHits = new int[sessionModel.GetPlayerCount()];
        _playerMisses = new int[sessionModel.GetPlayerCount()];
        _currentPlayerIndex = -1;
        for (int i = 0; i < _playerHealths.Length; i++)
        {
            _playerHealths[i] = DEFAULT_HEALTH;
            _playerHits[i] = _level.GetDefaultHitsPerPlayer();
            _playerMisses[i] = 0;
        }

        _bossHealth = DEFAULT_HEALTH;
    }

    public PlayerSessionModel.PlayerEntry GetCurrentPlayer()
    {
        return session.GetPlayer(_currentPlayerIndex);
    }
    
    public void Dispose()
    {
        session.OnPlayerAdd -= SessionModelOnOnPlayerAdd;
    }
 
    public const float DEFAULT_HEALTH = 1f;

    private void SessionModelOnOnPlayerAdd(PlayerSessionModel.PlayerEntry obj)
    {
        _playerHealths = _playerHealths.Append(DEFAULT_HEALTH).ToArray();
        _playerHits = _playerHits.Append(_level.GetDefaultHitsPerPlayer()).ToArray();
        _playerMisses = _playerMisses.Append(0).ToArray();
    }

    public float GetPlayerHealth(int index)
    {
        return _playerHealths[index];
    }
    
    public int GetPlayerHits(int index)
    {
        return _playerHits[index];
    }
    
    public void SetPlayerHealth(int index, float health)
    {
        var prevHealth = _playerHealths[index];
        _playerHealths[index] = health;
        if (prevHealth <= 0f && health > 0f)
        {
            OnPlayerRez?.Invoke(index);
        }
        OnHealthUpdate?.Invoke(index, health);
        if (health <= 0f)
        {
            OnPlayerDead?.Invoke(index);
            if (GetLivePlayerCount() <= 0)
            {
                OnAllPlayersDead?.Invoke();
            }
        }
    }

    public int GetLivePlayerCount()
    {
        return session.GetPlayers()
            .Count(ix => ix.IsActive && isAlive(ix));
    }

    public void AddPlayerHealth(int index, float health)
    {
        SetPlayerHealth(index, health + GetPlayerHealth(index));
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
        if (health <= 0f)
        {
            OnBossDead?.Invoke();
        }
    }
    
    public void AddBossHealth(float health)
    {
        SetBossHealth(health + GetBossHealth());
    }

    public void IncrementCurrentPlayer()
    {
        SetPlayerIndex(GetNextLivingPlayerIndex(_currentPlayerIndex));
    }

    private void BlurPlayer(int playerIndex)
    {
        OnPlayerIndexBlur?.Invoke(playerIndex);    
    }

    private void FocusPlayer(int playerIndex)
    {
        _playerHits[_currentPlayerIndex] = _level.GetDefaultHitsPerPlayer();
        OnPlayerIndexFocus?.Invoke(playerIndex);
    }

    public bool HasCurrentPlayer()
    {
        return _currentPlayerIndex > -1;
    }

    public void HandleMissForCurrentPlayer()
    {
        AddPlayerHealth(_currentPlayerIndex, -_level.GetHitDamage());
    }

    private float hitCooldownRemaining = 0f;

    public void Tick(float delta)
    {
        hitCooldownRemaining = Math.Max(hitCooldownRemaining - delta, 0f);
    }

    private float _hitCooldown = 1f;
    public void HandleHitForCurrentPlayer()
    {
        if (hitCooldownRemaining <= 0f)
        {
            AddBossHealth(-_level.GetHitDamage());
            ReduceHitsRemaining();
            hitCooldownRemaining += _hitCooldown;
        }
        
    }

    private void ReduceHitsRemaining()
    {
        _playerHits[_currentPlayerIndex] -= 1;
        OnHitUpdate?.Invoke(_currentPlayerIndex, _playerHits[_currentPlayerIndex]);
        if (_playerHits[_currentPlayerIndex] <= 0)
        {
            IncrementCurrentPlayer();
        }
    }

    public float GetBossHealth()
    {
        return _bossHealth;
    }
    
    public int GetNextLivingPlayerIndex(int currentPlayerIndex)
    {
        var players = session.GetPlayers();
        for (int i = currentPlayerIndex + 1; i < players.Length; i++)
        {
            if (isAlive(players[i]))
            {
                return i;
            }
        }
        
        for (int i = 0; i < currentPlayerIndex; i++)
        {
            if (isAlive(players[i]))
            {
                return i;
            }
        }

        return -1;
    }

    public bool isAlive(PlayerSessionModel.PlayerEntry player)
    {
        return player.IsActive && _playerHealths[player.Index] > 0f;
    }

    public void Reset()
    {
        ResetData();
    }

    public void SetPlayerIndex(int index)
    {
        if (index == _currentPlayerIndex)
        {
            return;
        }
        Debug.Log($"SETTING PLAYER INDEX {index}");
        var prevPlayer = _currentPlayerIndex;
        _currentPlayerIndex = index;
        if (prevPlayer > -1)
        {
            BlurPlayer(prevPlayer);
        }
        if (index > -1)
        {
            FocusPlayer(index);
        }
    }
    
    public void ResetCurrentPlayer()
    {
        SetPlayerIndex(-1);
    }

    public void ResetCooldowns()
    {
        hitCooldownRemaining = 0f;
    }
}
