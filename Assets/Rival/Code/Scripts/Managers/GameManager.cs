using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public enum RivalGameState
    {
        NONE,
        VOLUME_UP,
        MOVE_PREVIEW,
        STEP_BACK,
        GAMEPLAY_COUNTDOWN,
        GAMEPLAY_PLAY,
        GAMEPLAY_DAMAGE_SUMMARY,
        ENDGAME,
    }

    public static RivalGameState[] StatesInOrder = new[]
    {
        RivalGameState.NONE,
        RivalGameState.VOLUME_UP,
        RivalGameState.MOVE_PREVIEW,
        RivalGameState.STEP_BACK,
        RivalGameState.GAMEPLAY_COUNTDOWN,
        RivalGameState.GAMEPLAY_PLAY,
        RivalGameState.GAMEPLAY_DAMAGE_SUMMARY,
        RivalGameState.ENDGAME,
    };
    
    public void NextState()
    {
        var index = StatesInOrder.Select((_, i) => new { _, i }).First(x => x._ == fsm.State).i;
        if (index >= StatesInOrder.Length - 1)
        {
            return;
        }

        var nextState = StatesInOrder[index + 1];
        fsm.ChangeState(nextState);
    }

    private StateMachine<RivalGameState> fsm;
    
    void Awake(){
        fsm = new StateMachine<RivalGameState>(this); 
        fsm.Changed += FsmOnChanged;
        OnGameStateChange.AddListener(BroadcastExitAndEnter);
        fsm.ChangeState(RivalGameState.NONE);
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        NextState();
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
            From = fsm.LastStateExists ? fsm.LastState : RivalGameState.NONE,
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
    
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestState(RivalGameState state)
    {
        
    }

}
