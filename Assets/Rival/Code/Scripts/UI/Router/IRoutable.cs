using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IRoutable : IEventSystemHandler
{
    void RouteChange(RouteEventData.RouteChange routeData);
}
