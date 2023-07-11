using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSessionManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    public class GameModelEvent : UnityEvent<PlayerSessionModel> {}

    public GameModelEvent OnSessionModelReset;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        OnSessionModelReset?.Invoke(_playerSessionModel);
    }

    private PlayerSessionModel _playerSessionModel;
    public GameSessionModel GameModel;
    public void SetSession(PlayerSessionModel playerSessionModel)
    {
        _playerSessionModel = playerSessionModel;
        NotifyModelChange();
    }

    public void SetGameModel(GameSessionModel gameModel)
    {
        if (GameModel != null || gameModel == null)
        {
            UnbindGameModelEvents();
        }
        GameModel = gameModel;
        if (gameModel != null)
        {
            BindGameModelEvents();    
        }
    }

    private void OnDestroy()
    {
        UnbindGameModelEvents();
    }
    
    private void BindGameModelEvents()
    {
        GameModel.OnPlayerIndexBlur += GameModelOnOnPlayerIndexBlur;
        GameModel.OnPlayerIndexFocus += GameModelOnOnPlayerIndexFocus;
        
        GameModel.OnHealthUpdate += GameModelOnOnHealthUpdate;
        GameModel.OnBossHealthUpdate += GameModelOnOnBossHealthUpdate;
        
        GameModel.OnPlayerDead += GameModelOnOnPlayerDead;
        GameModel.OnPlayerRez += GameModelOnOnPlayerRez;
        GameModel.OnAllPlayersDead += GameModelOnOnAllPlayersDead;
        GameModel.OnBossDead += GameModelOnOnBossDead;
        
    }
    
    private void UnbindGameModelEvents()
    {
        GameModel.OnPlayerIndexBlur -= GameModelOnOnPlayerIndexBlur;
        GameModel.OnPlayerIndexFocus -= GameModelOnOnPlayerIndexFocus;
        
        GameModel.OnHealthUpdate -= GameModelOnOnHealthUpdate;
        GameModel.OnBossHealthUpdate -= GameModelOnOnBossHealthUpdate;
        
        GameModel.OnPlayerDead -= GameModelOnOnPlayerDead;
        GameModel.OnPlayerRez -= GameModelOnOnPlayerRez;
        GameModel.OnAllPlayersDead -= GameModelOnOnAllPlayersDead;
        GameModel.OnBossDead -= GameModelOnOnBossDead;
    }

    public UnityEvent OnBossDead;
    
    private void GameModelOnOnBossDead()
    {
        OnBossDead?.Invoke();
    }

    public UnityEvent OnAllDead;
    
    private void GameModelOnOnAllPlayersDead()
    {
        OnAllDead?.Invoke();
    }

    public PlayerStateEvent OnPlayerRez;
    
    private void GameModelOnOnPlayerRez(int index)
    {
        OnPlayerRez?.Invoke(_playerSessionModel.GetPlayer(index));
    }

    public PlayerStateEvent OnPlayerDead;
    private void GameModelOnOnPlayerDead(int index)
    {
        OnPlayerDead?.Invoke(_playerSessionModel.GetPlayer(index));
    }

    private void GameModelOnOnBossHealthUpdate(float health)
    {
        OnBossHealthUpdate?.Invoke(health);
    }

    private void GameModelOnOnHealthUpdate(int index, float health)
    {
        OnHealthUpdate?.Invoke(_playerSessionModel.GetPlayer(index), health);
    }

    [Serializable]
    public class PlayerStateEvent : UnityEvent<PlayerSessionModel.PlayerEntry> {}

    public PlayerStateEvent OnPlayerFocus;
    public PlayerStateEvent OnPlayerBlur;
    
    [Serializable]
    public class FloatEvent : UnityEvent<float> {}
        
    [Serializable]
    public class IntEvent : UnityEvent<int> {}
    
    [Serializable]
    public class PlayerFloatEvent : UnityEvent<PlayerSessionModel.PlayerEntry, float> {}

    public PlayerFloatEvent OnHealthUpdate;
    public FloatEvent OnBossHealthUpdate;

    private void GameModelOnOnPlayerIndexFocus(int index)
    {
        OnPlayerFocus?.Invoke(_playerSessionModel.GetPlayer(index));
    }

    private void GameModelOnOnPlayerIndexBlur(int index)
    {
        OnPlayerBlur?.Invoke(_playerSessionModel.GetPlayer(index));
    }

    private void NotifyModelChange()
    {
        
    }

    public PlayerSessionModel GetSession()
    {
        return _playerSessionModel;
    }

    public void Reset()
    {
        GameModel.Reset();
        OnSessionModelReset?.Invoke(_playerSessionModel);
    }
}
