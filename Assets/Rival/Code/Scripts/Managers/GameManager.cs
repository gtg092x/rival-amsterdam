using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterLove.StateMachine;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public enum RivalGameState
    {
        None,
        VolumeUp,
        MovePreview,
        StepBack,
        GameplayCountdown,
        GameplayPlay,
        EndGame,
        GameOver
    }

    public static RivalGameState[] StatesInOrder = new[]
    {
        RivalGameState.None,
        RivalGameState.VolumeUp,
        // RivalGameState.MovePreview,
        RivalGameState.StepBack,
        RivalGameState.GameplayCountdown,
        RivalGameState.GameplayPlay,
        RivalGameState.GameOver,
        RivalGameState.EndGame
    };

    GameSessionModel GetGameSessionModel()
    {
        return playerSessionManager.GameModel;
    }
    
    public void RestartGameplayLoopOrEnd()
    {
        if (GetGameSessionModel().GetLevelState().IsCompleted())
        {
            fsm.ChangeState(RivalGameState.EndGame);
            return;
        }
        fsm.ChangeState(RivalGameState.GameplayCountdown);
    }
    
    void GameplayCountdown_Enter()
    {
        // playerSessionManager.GameModel.IncrementCurrentPlayer();
    }

    public RivalGameState GetCurrentState()
    {
        return fsm.State;
    }
    
    public void NextState()
    {
        var index = StatesInOrder.Select((_, i) => new { _, i }).First(x => x._ == fsm.State).i;
        if (index >= StatesInOrder.Length - 1)
        {
            return;
        }

        var nextState = StatesInOrder[index + 1];

        if (nextState == RivalGameState.GameplayCountdown)
        {
            RestartGameplayLoopOrEnd();
            return;
        }
        
        fsm.ChangeState(nextState);
    }

    private StateMachine<RivalGameState> fsm;
    
    void Awake(){
        fsm = new StateMachine<RivalGameState>(this); 
        fsm.Changed += FsmOnChanged;
        OnGameStateChange.AddListener(BroadcastExitAndEnter);
        fsm.ChangeState(RivalGameState.None);
        OnGameStateChange.AddListener(HandleGameplayEnter);
        OnGameStateChange.AddListener(HandleAllBroadcasts);
        
    }

    private void HandleAllBroadcasts(GameStateChange arg0)
    {
        if (arg0.To == RivalGameState.StepBack)
        {
            OnStepBack?.Invoke();
        }
    }

    private void HandleGameplayEnter(GameStateChange delta)
    {
        playerSessionManager.GameModel.ResetCooldowns();
        if (IsGameplayState(delta.To) && !IsGameplayState(delta.From))
        {
            playerSessionManager.GameModel.SetPlayerIndex(0);
            OnGameplayStateEnter?.Invoke();
        } else if (IsGameplayState(delta.From) && !IsGameplayState(delta.To))
        {
            playerSessionManager.GameModel.ResetCurrentPlayer();
            OnGameplayStateExit?.Invoke();
        }
    }

    IEnumerator Start()
    {
        BindGameModelEvents();
        yield return new WaitForEndOfFrame();
        NextState();
    }

    public UnityEvent OnStepBack;
    
    

    private void BindGameModelEvents()
    {
        playerSessionManager.OnPlayerDead.AddListener(HandlePlayerDie);
        playerSessionManager.OnBossDead.AddListener(GameWin);
        playerSessionManager.OnPlayerFocus.AddListener(HandlePlayerChange);
    }

    private void HandlePlayerChange(PlayerSessionModel.PlayerEntry player)
    {
        if (GetCurrentPlayer().Index == player.Index && HasLivingPlayers())
        {
            fsm.ChangeState(RivalGameState.StepBack);
        }
    }

    private void HandlePlayerDie(PlayerSessionModel.PlayerEntry player)
    {
        if (!HasLivingPlayers())
        {
            fsm.ChangeState(RivalGameState.GameOver);
        }
    }

    private bool HasLivingPlayers()
    {
        return playerSessionManager.GameModel.GetLivePlayerCount() > 0;
    }

    private void OnDestroy()
    {
        UnbindGameModelEvents();
    }

    private void UnbindGameModelEvents()
    {
        playerSessionManager.OnBossDead.RemoveListener(GameWin);
        playerSessionManager.OnPlayerDead.RemoveListener(HandlePlayerDie);
        playerSessionManager.OnPlayerFocus.RemoveListener(HandlePlayerChange);
    }

    [SerializeField]
    private PlayerSessionManager playerSessionManager;

    private void OnValidate()
    {
        playerSessionManager ??= FindAnyObjectByType<PlayerSessionManager>();
    }


    private void BroadcastExitAndEnter(GameStateChange change)
    {
        OnGameStateExit?.Invoke(change.From);
        OnGameStateEnter?.Invoke(change.To);
    }

    private void FsmOnChanged(RivalGameState obj)
    {
        OnGameStateChange?.Invoke(new GameStateChange()
        {
            From = fsm.LastStateExists ? fsm.LastState : RivalGameState.None,
            To = obj,
        });
    }

    [Serializable]
    public struct GameStateChange
    {
        public RivalGameState From;
        public RivalGameState To;
    }
    
    [Serializable]
    public class GameStateChangeEvent : UnityEvent<GameStateChange> {}
    
    [Serializable]
    public class GameStateEvent : UnityEvent<RivalGameState> {}

    public GameStateChangeEvent OnGameStateChange;
    
    public GameStateEvent OnGameStateExit;
    public GameStateEvent OnGameStateEnter;
    public UnityEvent OnGameplayStateEnter;
    public UnityEvent OnGameplayStateExit;
  
    // Update is called once per frame
    void Update()
    {
        if (playerSessionManager != null && playerSessionManager.GameModel != null)
        {
            playerSessionManager.GameModel.Tick(Time.deltaTime);    
        }
    }

    public void RequestState(RivalGameState state)
    {
        
    }

    public PlayerSessionModel.PlayerEntry GetCurrentPlayer()
    {
        return playerSessionManager.GameModel.GetCurrentPlayer();
    }

    public bool IsGameplayState(RivalGameState state)
    {
        return state == RivalGameState.GameplayPlay || state == RivalGameState.GameplayCountdown || state == RivalGameState.StepBack;
    }

    public PlayerSessionManager GetSessionManager()
    {
        return playerSessionManager;
    }
    
    
    private IEnumerator GameplayPlay_Enter()
    {
        Debug.Log("WRITE GAMEPLAY LOOP");
        yield return null;
    }

    public bool IsCurrentPlayer(int playerIndex)
    {
        return playerSessionManager.GameModel.HasCurrentPlayer() && GetCurrentPlayer().Index == playerIndex;
    }

    public float GetPlayerHealth(int playerIndex)
    {
        return playerSessionManager.GameModel.GetPlayerHealth(playerIndex);
    }

    public void DebugMiss()
    {
        playerSessionManager.GameModel.HandleMissForCurrentPlayer();
    }

    public void DebugHit()
    {
        playerSessionManager.GameModel.HandleHitForCurrentPlayer();
    }

    public float GetBossHealth()
    {
        return playerSessionManager.GameModel.GetBossHealth();
    }

    [SerializeField] private SceneReference WinScene;
    
    IEnumerator EndGame_Enter()
    {
        yield return new WaitForSeconds(0.1f);
        var op = SceneManager.LoadSceneAsync(WinScene);
        yield return new WaitUntil(() => op.isDone);
    }

    private void GameWin()
    {
        fsm.ChangeState(RivalGameState.EndGame);
    }

    public void Reset()
    {
        playerSessionManager.Reset();
        fsm.ChangeState(RivalGameState.StepBack);
    }

    public bool IsAlive(PlayerSessionModel.PlayerEntry arg0)
    {
        return playerSessionManager.GameModel.isAlive(arg0);
    }
}
