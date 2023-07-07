using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameLoopUIManager : MonoBehaviour
{
    public UIDocument MainUI;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        var ready = MainUI.rootVisualElement.Q<PlayerReadyElement>();
        ready.Init(3);
        yield return new WaitForSeconds(1f);
        ready.PlayerName = "Matt";
        yield return ready.PerformCountdown();
        Debug.Log("GO");
        ready.Init(5);
        yield return ready.PerformCountdown();
    }

    public void HandleSessionReset(PlayerSessionModel playerSessionModel)
    {
        ResetTopBar();
        foreach (var player in playerSessionModel.GetPlayers())
        {
            AddPlayerToTopBar(player);
        }
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

    public void HandleStateExit(GameManager.RivalGameState state)
    {
        
    }
    
    public void HandleStateEnter(GameManager.RivalGameState state)
    {
        
    }
}
