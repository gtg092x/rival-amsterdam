using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
