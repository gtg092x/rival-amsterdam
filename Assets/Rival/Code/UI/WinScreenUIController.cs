using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WinScreenUIController : MonoBehaviour
{
    [SerializeField]
    private UIDocument _ui;
    
    [SerializeField]
    private ZooController _renderZoo;

    [SerializeField] private AvatarObject _defeatedBoss;
    
    private VisualElement rootElement;

    private VisualElement BossAvatar => rootElement.Q<VisualElement>("Boss");
    private Label BossName => rootElement.Q<Label>("BossName");
    
    private void OnValidate()
    {
        _ui ??= GetComponent<UIDocument>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        rootElement = _ui.rootVisualElement;
    }

    private IEnumerator Start()
    {
        rootElement.Q<Button>("Restart").clicked += Restart;
        BossAvatar.style.backgroundImage = Background.FromRenderTexture(_renderZoo.GetRender(_defeatedBoss, true));
        BossName.text = $"You defeated {_defeatedBoss.AvatarName}";
        yield return new WaitForSeconds(0.5f);
        GetComponent<AudioSource>().PlayOneShot(_defeatedBoss.LoseSound);
        _renderZoo.AnimateExistingRender(_defeatedBoss, _defeatedBoss.Die);
    }

    private void Restart()
    {
        StartCoroutine(RestartRoutine());
    }

    [SerializeField] private SceneReference LoopScene;
    
    private IEnumerator RestartRoutine()
    {
        var op = SceneManager.LoadSceneAsync(LoopScene);
        yield return new WaitUntil(() => op.isDone);
    }

    private void OnDestroy()
    {
        rootElement.Q<Button>("Restart").clicked -= Restart;
    }
}
