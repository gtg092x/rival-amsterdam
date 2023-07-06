using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundUiAnimator : MonoBehaviour
{
    [SerializeField]
    private UIDocument _backgroundUi;

    private VisualElement _gradient;
    private VisualElement _container;
    private VisualElement _stars;
    private VisualElement _design;

    // Start is called before the first frame update
    void OnValidate()
    {
        _backgroundUi ??= GetComponent<UIDocument>();
    }

    
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        Design_TransitionEnd();
        Stars_TransitionEnd();
    }

    private const string STYLE_TRANSFORM_CLASS = "transform-style-left";
    private const string STARS_TRANSFORM_CLASS = "transform-stars-down";

    private void OnEnable()
    {
        var root = _backgroundUi.rootVisualElement;
        _container = root.Q<VisualElement>("Container");
        _gradient = _container.Q<VisualElement>("MainGradient");
        _design = _container.Q<VisualElement>("StyleOverlay");
        _stars = _container.Q<VisualElement>("Stars");

        _gradient.RegisterCallback<TransitionEndEvent>(Gradient_TransitionEnd);
        _design.RegisterCallback<TransitionEndEvent>(Design_TransitionEnd);
        _stars.RegisterCallback<TransitionEndEvent>(Stars_TransitionEnd);
    }
    
    private void OnDisable()
    {
        _gradient.UnregisterCallback<TransitionEndEvent>(Gradient_TransitionEnd);
        _design.UnregisterCallback<TransitionEndEvent>(Design_TransitionEnd);
        _stars.UnregisterCallback<TransitionEndEvent>(Stars_TransitionEnd);
    }

    private void Stars_TransitionEnd(TransitionEndEvent evt = null)
    {
        if (_stars.ClassListContains(STARS_TRANSFORM_CLASS))
        {
            _stars.RemoveFromClassList(STARS_TRANSFORM_CLASS);
        }
        else
        {
            _stars.AddToClassList(STARS_TRANSFORM_CLASS);
        }
    }

    

    private void Design_TransitionEnd(TransitionEndEvent evt = null)
    {
        if (_design.ClassListContains(STYLE_TRANSFORM_CLASS))
        {
            _design.RemoveFromClassList(STYLE_TRANSFORM_CLASS);
        }
        else
        {
            _design.AddToClassList(STYLE_TRANSFORM_CLASS);
        }
    }
    
    private void Gradient_TransitionEnd(TransitionEndEvent evt)
    {
        
    }

    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
