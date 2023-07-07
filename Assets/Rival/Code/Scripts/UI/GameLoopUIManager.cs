using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLoopUIManager : MonoBehaviour
{
    public UIDocument MainUI;

    private VisualElement rootElement;
    private VisualElement VolumeGate => rootElement.Q<VisualElement>("VolumeGate");
    private PlayerReadyElement PlayerCountdown => rootElement.Q<PlayerReadyElement>("PlayerReadyModal");
    private VisualElement PreviewGate => rootElement.Q<VisualElement>("PreviewGate");
    private VisualElement PoseGate => rootElement.Q<VisualElement>("PoseGate");
    
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
    }

    private void OnDestroy()
    {
        UnbindToControls();
    }

    private void UnbindToControls()
    {
        rootElement.Q<Button>("VolumeContinue").clicked -= _gameManager.NextState;
        rootElement.Q<Button>("PreviewContinue").clicked -= _gameManager.NextState;
        rootElement.Q<Button>("PoseContinue").clicked -= _gameManager.NextState;
    }

    private void BindToControls()
    {
        rootElement.Q<Button>("VolumeContinue").clicked += _gameManager.NextState;
        rootElement.Q<Button>("PreviewContinue").clicked += _gameManager.NextState;
        rootElement.Q<Button>("PoseContinue").clicked += _gameManager.NextState;
    }


    IEnumerator StartHandleCountdown()
    {
        var ready = rootElement.Q<PlayerReadyElement>();
        ready.Init(3);
        yield return new WaitForSeconds(1f);
        ready.PlayerName = "Matt";
        yield return ready.PerformCountdown();
    }

    public void HandleSessionReset(PlayerSessionModel playerSessionModel)
    {
        ResetTopBar();
        foreach (var player in playerSessionModel.GetPlayers())
        {
            AddPlayerToTopBar(player);
        }

        SetCountdownAvatars(playerSessionModel);
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
            Avatar = render
        };
        topBar.Add(health);
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
    }

    void HandleStateExit(GameManager.RivalGameState state)
    {
        switch (state)
        {
            case GameManager.RivalGameState.NONE:
                break;
            case GameManager.RivalGameState.VOLUME_UP:
                VolumeGate.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.MOVE_PREVIEW:
                PreviewGate.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.STEP_BACK:
                PoseGate.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.GAMEPLAY_COUNTDOWN:
                PlayerCountdown.style.display = DisplayStyle.None;
                break;
            case GameManager.RivalGameState.GAMEPLAY_PLAY:
                break;
            case GameManager.RivalGameState.GAMEPLAY_DAMAGE_SUMMARY:
                break;
            case GameManager.RivalGameState.ENDGAME:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private Coroutine currentCountdown;
    private void StartCountdown()
    {
        if (currentCountdown != null)
        {
            StopCoroutine(currentCountdown);    
        }
        currentCountdown = StartCoroutine(StartHandleCountdown());
    }

    void HandleStateEnter(GameManager.RivalGameState state)
    {
        switch (state)
        {
            case GameManager.RivalGameState.NONE:
                break;
            case GameManager.RivalGameState.VOLUME_UP:
                VolumeGate.style.display = DisplayStyle.Flex;
                break;
            case GameManager.RivalGameState.MOVE_PREVIEW:
                PreviewGate.style.display = DisplayStyle.Flex;
                break;
            case GameManager.RivalGameState.STEP_BACK:
                PoseGate.style.display = DisplayStyle.Flex;
                break;
            case GameManager.RivalGameState.GAMEPLAY_COUNTDOWN:
                PlayerCountdown.style.display = DisplayStyle.Flex;
                StartCountdown();
                break;
            case GameManager.RivalGameState.GAMEPLAY_PLAY:
                break;
            case GameManager.RivalGameState.GAMEPLAY_DAMAGE_SUMMARY:
                break;
            case GameManager.RivalGameState.ENDGAME:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }    
    }

    private void OnValidate()
    {
        _gameManager ??= FindAnyObjectByType<GameManager>();
    }

    
}
