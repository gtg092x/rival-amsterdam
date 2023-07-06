using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIRouteListener : MonoBehaviour, IRoutable
{
    [SerializeField]
    private RouteEventData.Route _targetRoute;

    [SerializeField]
    private UISplashRouter _router;
    
    private void OnValidate()
    {
        _router ??= GetComponentInParent<UISplashRouter>();
    }


    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    

    public void RouteChange(RouteEventData.RouteChange change)
    {
        if (change.To == _targetRoute)
        {
            OnEnter?.Invoke();
        } else if (change.From == _targetRoute || change.From == RouteEventData.Route.NONE)
        {
            OnExit?.Invoke();
        } 
    }
}
