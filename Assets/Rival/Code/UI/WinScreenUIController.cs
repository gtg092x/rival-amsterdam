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
    
    private VisualElement rootElement;

    private void OnValidate()
    {
        _ui ??= GetComponent<UIDocument>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        rootElement = _ui.rootVisualElement;
    }

    private void Start()
    {
        rootElement.Q<Button>("Restart").clicked += Restart;
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
