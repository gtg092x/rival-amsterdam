using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class GameLoopUIManager : MonoBehaviour
{
    public UIDocument MainUI;
    
    public PoseReaderController PoseReader;
    
    private VisualElement rootElement;
    private VisualElement VolumeGate => rootElement.Q<VisualElement>("VolumeGate");
    private VisualElement GameWin => rootElement.Q<VisualElement>("GameWin");
    private VisualElement Video => rootElement.Q<VisualElement>("Video");
    private VisualElement GameOver => rootElement.Q<VisualElement>("GameOver");
    private PlayerReadyElement PlayerCountdown => rootElement.Q<PlayerReadyElement>("PlayerReadyModal");
    private BossHealthElement BossHealth => rootElement.Q<BossHealthElement>();
    private VisualElement PreviewGate => rootElement.Q<VisualElement>("PreviewGate");
    private VisualElement PoseGate => rootElement.Q<VisualElement>("PoseGate");
    private VisualElement GameplayOverlay => rootElement.Q<VisualElement>("Gameplay");
    private VisualElement BossGameOver => rootElement.Q<VisualElement>("BossGameOver");

    private PlayerHealthElement GetPlayerHealthElement(int index)
    {
        return rootElement.Q<PlayerHealthElement>($"Player{index}");
    }

    [SerializeField]
    private GameManager _gameManager;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        HideAllSections();
        yield break;
    }

    private void Awake()
    {
        rootElement = MainUI.rootVisualElement;
        BindToControls();
        BindToStateChange();
    }

    private void BindToStateChange()
    {
        _gameManager.OnGameStateExit.AddListener(HandleStateExit);
        _gameManager.OnGameStateEnter.AddListener(HandleStateEnter);
        _gameManager.OnGameStateChange.AddListener(HandleGameStateChange);
    }

    private void HandleGameStateChange(GameManager.GameStateChange delta)
    {
        if (_gameManager.IsGameplayState(delta.From) && !_gameManager.IsGameplayState(delta.To))
        {
            HandleExitGameplay();
        } else if (_gameManager.IsGameplayState(delta.To) && !_gameManager.IsGameplayState(delta.From))
        {
            HandleEnterGameplay();
        }
    }

    private void OnDestroy()
    {
        UnbindToControls();
    }

    private void UnbindToControls()
    {
        rootElement.Q<Button>("VolumeContinue").clicked -= _gameManager.NextState;
        rootElement.Q<Button>("PreviewContinue").clicked -= _gameManager.NextState;
        //rootElement.Q<Button>("PoseContinue").clicked -= _gameManager.NextState;
        
        rootElement.Q<Button>("DebugHit").clicked -= _gameManager.DebugHit;
        rootElement.Q<Button>("DebugMiss").clicked -= _gameManager.DebugMiss;
        
        rootElement.Q<Button>("Retry").clicked -= _gameManager.Reset;
    }

    private void BindToControls()
    {
        rootElement.Q<Button>("VolumeContinue").clicked += _gameManager.NextState;
        rootElement.Q<Button>("PreviewContinue").clicked += _gameManager.NextState;
        //rootElement.Q<Button>("PoseContinue").clicked += _gameManager.NextState;
        
        //rootElement.Q<Button>("DebugHit").clicked += _gameManager.DebugHit;
        //rootElement.Q<Button>("DebugMiss").clicked += _gameManager.DebugMiss;
        
        rootElement.Q<Button>("Retry").clicked += _gameManager.Reset;
    }


    IEnumerator StartHandleCountdown()
    {
        var currentPlayer = _gameManager.GetCurrentPlayer();
        var ready = rootElement.Q<PlayerReadyElement>();
        ready.PlayerName = $"{currentPlayer.Avatar.AvatarName}";
        ready.PlayerAvatar = _zooManager.GetRender(currentPlayer.Avatar);
        ready.Init(2);
        yield return new WaitForSeconds(1f);
        yield return ready.PerformCountdown();
        _gameManager.NextState();
    }
    
    public void HandleSessionReset(PlayerSessionModel playerSessionModel)
    {
        ResetTopBar();
        foreach (var player in playerSessionModel.GetPlayers())
        {
            AddPlayerToTopBar(player);
        }

        SetCountdownAvatars(playerSessionModel);
        SetGameplayAvatars(playerSessionModel);
    }

    private void SetGameplayAvatars(PlayerSessionModel playerSessionModel)
    {
        BossHealth.EnemyAvatar = _zooManager.GetRender(playerSessionModel.BossAvatar, true);
        BossHealth.BossBarProgress = _gameManager.GetBossHealth();
        
        BossGameOver.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(_zooManager.GetRender(playerSessionModel.BossAvatar, true)));
    }

    private void SetCountdownAvatars(PlayerSessionModel playerSessionModel)
    {
        PlayerCountdown.EnemyAvatar = _zooManager.GetRender(playerSessionModel.BossAvatar, true);
        PlayerCountdown.PlayerAvatar = _zooManager.GetRender(playerSessionModel.GetPlayers().First().Avatar);
    }

    [SerializeField]
    private ZooController _zooManager;

    private void AddPlayerToTopBar(PlayerSessionModel.PlayerEntry player)
    {
        var topBar = MainUI.rootVisualElement.Q<VisualElement>(TopBarName);
        var render = _zooManager.GetRender(player.Avatar);
        var health = new PlayerHealthElement
        {
            Avatar = render,
            HighlightAvatar = _gameManager.IsCurrentPlayer(player.Index),
            HealthBarProgress = _gameManager.GetPlayerHealth(player.Index),
        };
        topBar.Add(health);
        health.name = $"Player{player.Index}";
    }

    private const string TopBarName = "TopBar";

    private void ResetTopBar()
    {
        var topBar = MainUI.rootVisualElement.Q<VisualElement>(TopBarName);
        foreach (var child in topBar.Children().ToArray())
        {
            topBar.Remove(child);
        }
    }
    
    private void HideAllSections()
    {
        VolumeGate.style.display = DisplayStyle.None;
        PreviewGate.style.display = DisplayStyle.None;
        PoseGate.style.display = DisplayStyle.None;
        PlayerCountdown.style.display = DisplayStyle.None;
        GameplayOverlay.style.display = DisplayStyle.None;
        GameOver.style.display = DisplayStyle.None;
        Video.style.display = DisplayStyle.None;
        GameWin.style.display = DisplayStyle.None;
    }

    void HandleStateExit(GameManager.RivalGameState state)
    {
        switch (state)
        {
            case GameManager.RivalGameState.None:
                break;
            case GameManager.RivalGameState.VolumeUp:
                VolumeGate.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.MovePreview:
                PreviewGate.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.StepBack:
                PoseGate.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.GameplayCountdown:
                PlayerCountdown.style.display = DisplayStyle.None;
                StopCoroutine(currentUIRoutine);
                break;
            case GameManager.RivalGameState.GameplayPlay:
                GameplayOverlay.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.GameOver:
                GameOver.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.EndGame:
                GameWin.style.display = DisplayStyle.None;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        
    }

    private void HandleExitGameplay()
    {
        UnbindGameplayEvents();
        ExitPoseRead();
        HandlePlayerBlur(_gameManager.GetCurrentPlayer());
    }

    private void HandleEnterGameplay()
    {
        BindGameplayEvents();
        EnterPoseRead();
        HandlePlayerFocus(_gameManager.GetCurrentPlayer());
        
    }
    
    private void ExitPoseRead()
    {
        Video.style.display = DisplayStyle.None;
        PoseReader.OnPowerPose.RemoveListener(HandlePowerPose);
        PoseReader.OnPose.RemoveListener(HandlePose);
        PoseReader.EndRead();
    }

    private void EnterPoseRead()
    {
        Video.style.display = DisplayStyle.Flex;
        PoseReader.OnPowerPose.AddListener(HandlePowerPose);
        PoseReader.OnPose.AddListener(HandlePose);
        PoseReader.Read();
    }

    private void HandlePose(PoseReaderController.Poses pose)
    {
        _gameManager.DebugHit();
    }

    private void HandlePowerPose()
    {
        _gameManager.NextState();
    }


    private void UnbindGameplayEvents()
    {
        PlayerSessionManager session = _gameManager.GetSessionManager();
        session.OnPlayerBlur.RemoveListener(HandlePlayerBlur);
        session.OnPlayerFocus.RemoveListener(HandlePlayerFocus);
        session.OnBossHealthUpdate.RemoveListener(GameModelOnOnBossHealthUpdate);
        session.OnHealthUpdate.RemoveListener(GameModelOnOnHealthUpdate);
    }

    private void GameModelOnOnHealthUpdate(PlayerSessionModel.PlayerEntry playerIndex, float health)
    {
        GetPlayerHealthElement(playerIndex.Index).HealthBarProgress = health;
    }

    private void GameModelOnOnBossHealthUpdate(float health)
    {
        BossHealth.BossBarProgress = health;
        // TODO animate boss
    }

    private void BindGameplayEvents()
    {
        PlayerSessionManager session = _gameManager.GetSessionManager();
        session.OnPlayerBlur.AddListener(HandlePlayerBlur);
        session.OnPlayerFocus.AddListener(HandlePlayerFocus);
        session.OnBossHealthUpdate.AddListener(GameModelOnOnBossHealthUpdate);
        session.OnHealthUpdate.AddListener(GameModelOnOnHealthUpdate);
        
        session.OnPlayerDead.AddListener(HandlePlayerDie);
        session.OnPlayerRez.AddListener(HandlePlayerRez);
    }

    private void HandlePlayerRez(PlayerSessionModel.PlayerEntry arg0)
    {
        GetPlayerHealthElement(arg0.Index).style.display = DisplayStyle.Flex;
    }

    private void HandlePlayerDie(PlayerSessionModel.PlayerEntry arg0)
    {
        GetPlayerHealthElement(arg0.Index).style.display = DisplayStyle.None;
    }

    private void HandlePlayerFocus(PlayerSessionModel.PlayerEntry player)
    {
        var element = GetPlayerHealthElement(player.Index);
        element.HighlightAvatar = true;
    }
    
    private void HandlePlayerBlur(PlayerSessionModel.PlayerEntry player)
    {
        var element = GetPlayerHealthElement(player.Index);
        element.HighlightAvatar = false;
    }

    private Coroutine currentUIRoutine;
    private void StartCountdown()
    {
        if (currentUIRoutine != null)
        {
            StopCoroutine(currentUIRoutine);    
        }
        currentUIRoutine = StartCoroutine(StartHandleCountdown());
    }

    void HandleStateEnter(GameManager.RivalGameState state)
    {
        switch (state)
        {
            case GameManager.RivalGameState.None:
                break;
            case GameManager.RivalGameState.VolumeUp:
                VolumeGate.style.display = DisplayStyle.Flex;
                break;
            case GameManager.RivalGameState.MovePreview:
                PreviewGate.style.display = DisplayStyle.Flex;
                break;
            case GameManager.RivalGameState.StepBack:
                PoseGate.style.display = DisplayStyle.Flex;
                break;
            case GameManager.RivalGameState.GameplayCountdown:
                PlayerCountdown.style.display = DisplayStyle.Flex;
                StartCountdown();
                break;
            case GameManager.RivalGameState.GameplayPlay:
                GameplayOverlay.style.display = DisplayStyle.Flex;
                break;
            case GameManager.RivalGameState.GameOver:
                GameOver.style.display = DisplayStyle.Flex;
                break;
            case GameManager.RivalGameState.EndGame:
                GameWin.style.display = DisplayStyle.Flex;
                StartCoroutine(AddFullOpacityToWin());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private IEnumerator AddFullOpacityToWin()
    {
        yield return new WaitForSeconds(0.1f);
        GameWin.AddToClassList("full-opacity");
    }

    private void OnValidate()
    {
        _gameManager ??= FindAnyObjectByType<GameManager>();
    }

    
}
