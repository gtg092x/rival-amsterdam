using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSessionController : MonoBehaviour
{
    [SerializeField]
    private PlayerSessionManager playerSessionManager;

    private void OnValidate()
    {
        playerSessionManager ??= FindAnyObjectByType<PlayerSessionManager>();
    }

    public AvatarObject[] PlayerAvatars;
    public AvatarObject BossAvatar;
    
    void Start()
    {
        var demoModel = new PlayerSessionModel();
        foreach (var player in PlayerAvatars)
        {
            demoModel.AddPlayer(player);
        }
        demoModel.SetBossAvatar(BossAvatar);
        
        playerSessionManager.SetSession(demoModel);
        var gameModel = new GameSessionModel(demoModel);
        playerSessionManager.SetGameModel(gameModel);
    }
}
