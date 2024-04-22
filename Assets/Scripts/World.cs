using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class World
{
    // cached singletons for ease of reference
    public static readonly Dictionary<System.Type, MonoBehaviour> gameplayComponentsCache = new Dictionary<System.Type, MonoBehaviour>();
    
    /// <summary>
    /// Finds a single object in the scene with the requested component and caches it
    /// for easily obtaining a reference to a singleton behavior across scripts without serializing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Get<T>() where T : MonoBehaviour
    {
        // cached
        if (gameplayComponentsCache.ContainsKey(typeof(T)))
            return (T)gameplayComponentsCache[typeof(T)];

        // search GM helper first (slow)
        T found = null;
        bool didFind = false;
        for (int i = 0; found == null && i < ApplicationLifetime.GetGameObject().SearchRoots.Length; i++)
        {
            var all = ApplicationLifetime.GetGameObject().SearchRoots[i].GetComponentsInChildren<T>(true);
            if (all.Length > 0)
            {
                didFind = true;
                found = all[0];
                break;
            }
        }

        // super slow search, avoid for performance
        if (!didFind)
        {
            found = Object.FindObjectOfType<T>();
            didFind = found != null;
        }

        // cache
        if (didFind)
        {
            gameplayComponentsCache[typeof(T)] = found;
        }

        return found;
    }
    
    public static Coroutine StartGlobalCoroutine(IEnumerator routine)
    {
        return ApplicationLifetime.GetGameObject().StartCoroutine(routine);
    }
}
