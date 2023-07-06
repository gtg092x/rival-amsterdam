using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RouteEventData
{
    [Serializable]
    public enum Route
    {
        NONE,
        SPLASH,
        WELCOME,
        WELCOME_RECOGNIZED,
        LOGIN,
        CREATE_PROFILE
    }
    
    [Serializable]
    public struct RouteChange
    {
        public Route From;
        public Route To;
    }
}
