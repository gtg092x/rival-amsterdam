using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIRouteTranslater : MonoBehaviour
{
    [SerializeField]
    private string _exitClassName = "transition-push-exit";
    
    [SerializeField]
    private string _enterClassName = "transition-push-enter-start";
    
    [SerializeField]
    private PanelSettings _renderPanelSettings;
    
    [SerializeField]
    private PanelSettings _exitPanelSettings;

    [SerializeField]
    private UIDocument _uiDocument;

    private VisualElement _rootElement;

    private void OnValidate()
    {
        _uiDocument ??= GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _rootElement = _uiDocument.rootVisualElement;
        _rootElement.RegisterCallback<TransitionEndEvent>(Root_TransitionEnd);
    }

    private void OnDisable()
    {
        _rootElement.UnregisterCallback<TransitionEndEvent>(Root_TransitionEnd);
    }

    public void Exit()
    {
        _uiDocument.enabled = false;
        return;
        if (!Application.isPlaying)
        {
            _uiDocument.enabled = false;
        }
        else
        {
            _uiDocument.panelSettings = _exitPanelSettings;
            _rootElement.AddToClassList(_exitClassName);
        }
    }
    
    public void Enter()
    {
        _uiDocument.enabled = true;
        return;
        if (!Application.isPlaying)
        {
            _uiDocument.enabled = true;
        }
        else
        {
            _uiDocument.panelSettings = _renderPanelSettings;
            _rootElement.AddToClassList(_enterClassName);
            _uiDocument.enabled = true;
        }
        
    }
    
    private void Root_TransitionEnd(TransitionEndEvent evt)
    {
        if (_uiDocument.panelSettings == _exitPanelSettings)
        {
            _rootElement.RemoveFromClassList(_exitClassName); 
            _uiDocument.panelSettings = _renderPanelSettings;
            _uiDocument.enabled = false;
        }
        else
        {
            _rootElement.RemoveFromClassList(_enterClassName);    
        }
        
        
    }
}
