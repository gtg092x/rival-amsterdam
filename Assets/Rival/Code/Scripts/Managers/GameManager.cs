using System;
using System.Collections;
using System.Collections.Generic;
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

    private StateMachine<RivalGameState> fsm;
    
    void Awake(){
        fsm = new StateMachine<RivalGameState>(this); 
        fsm.Changed += FsmOnChanged;
        OnGameStateChange.AddListener(BroadcastExitAndEnter);
        fsm.ChangeState(RivalGameState.VOLUME_UP);
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
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
}
