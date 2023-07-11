using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterEffectsManager : MonoBehaviour
{
    
    [SerializeField] private GameManager _gameManager;

    [SerializeField]
    private AudioSource _audio;
    private void OnValidate()
    {
        _gameManager ??= FindAnyObjectByType<GameManager>();
        _audio ??= GetComponent<AudioSource>();
    }

   private void Start()
    {
        _gameManager.GetSessionManager().OnBossHealthUpdate.AddListener(HandleBossHurt);
        _gameManager.OnGameStateEnter.AddListener(BossTaunt);
        
    }

   private void BossTaunt(GameManager.RivalGameState state)
   {
       if (state == GameManager.RivalGameState.GameplayCountdown)
       {
           var avatar = _gameManager.GetSessionManager().GetSession().BossAvatar;
           if (avatar.Taunts.Length == 0) return;
           var index = Mathf.FloorToInt(Random.value * avatar.Taunts.Length);
           var ouch = avatar.Taunts[index];
           _audio.PlayOneShot(ouch);
       }
       
   }

   private void HandleBossHurt(float hurt)
    {
        var avatar = _gameManager.GetSessionManager().GetSession().BossAvatar;
        if (avatar.Ouches.Length == 0) return;
        var ouch = avatar.Ouches[Mathf.FloorToInt(Random.value * avatar.Ouches.Length)];
        _audio.PlayOneShot(ouch);
    }
}
