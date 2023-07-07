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
        GameModel = gameModel;
    }

    private void NotifyModelChange()
    {
        
    }

    public PlayerSessionModel GetSession()
    {
        return _playerSessionModel;
    }
}
