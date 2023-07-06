using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainFlowUI : MonoBehaviour
{
    [SerializeField]
    private UIDocument _mainFlowUi;
    
    [SerializeField]
    private AudioSource _uiAudio;
    
    [SerializeField]
    private AudioClip _splashSound;

    private VisualElement _top;
    private VisualElement _bottom;

    public UnityEvent OnComplete;
    
    private void OnValidate()
    {
        _mainFlowUi ??= GetComponent<UIDocument>();
    }
    
    private void OnEnable()
    {
        var root = _mainFlowUi.rootVisualElement;
        _top = root.Q<VisualElement>("RivalMarquee");
        _bottom = root.Q<VisualElement>("BeatMarquee");
        
        _top.AddToClassList(POP_DOWN_CLASS_NAME);
        _bottom.AddToClassList(POP_UP_CLASS_NAME);
    }
    
    private void OnDisable()
    {
        
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        _top.RemoveFromClassList(POP_DOWN_CLASS_NAME);
        _uiAudio.PlayOneShot(_splashSound);
        yield return new WaitForSeconds(1.4f);
        _bottom.RemoveFromClassList(POP_UP_CLASS_NAME);
        yield return new WaitForSeconds(1.0f);
        OnComplete?.Invoke();
        
    }

    private const string POP_UP_CLASS_NAME = "pop-up-start";
    private const string POP_DOWN_CLASS_NAME = "pop-down-start";

    // Update is called once per frame
    void Update()
    {
        
    }
}
