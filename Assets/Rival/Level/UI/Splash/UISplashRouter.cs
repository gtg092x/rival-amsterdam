using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISplashRouter : MonoBehaviour
{


    [SerializeField]
    private RouteEventData.Route _currentRouteValue;
    
    [Serializable]
    public class RouteChangeUnityEvent : UnityEvent<RouteEventData.RouteChange> {}

    public RouteChangeUnityEvent OnRouteChange;

    private void OnValidate()
    {
        CurrentRoute = _currentRouteValue;
        BroadcastRouteChange(new RouteEventData.RouteChange
        {
            From = RouteEventData.Route.NONE,
            To = _currentRouteValue
        });
    }

    public void NavigateTo(RouteEventData.Route newRoute)
    {
        CurrentRoute = newRoute;
    }
    
    public void NavigateTo(string newRoute)
    {
        if (Enum.TryParse<RouteEventData.Route>(newRoute, true, out RouteEventData.Route parsed))
        {
            NavigateTo(parsed);    
        }
        else
        {
            Debug.LogError($"No route found for {newRoute}");
        }
    }
    
    private RouteEventData.Route CurrentRoute
    {
        get => _currentRouteValue;
        set
        {
            if (_currentRouteValue == value) return;
            var old = _currentRouteValue;
            _currentRouteValue = value;
            BroadcastRouteChange(new RouteEventData.RouteChange
            {
                From = old,
                To = _currentRouteValue
            });
        }
    }

    private void BroadcastRouteChange(RouteEventData.RouteChange routeChange)
    {
        foreach (var routable in GetComponentsInChildren<IRoutable>())
        {
            routable.RouteChange(routeChange);
        }
        OnRouteChange?.Invoke(new RouteEventData.RouteChange
        {
            From = routeChange.From,
            To = routeChange.To
        });
    }
}


