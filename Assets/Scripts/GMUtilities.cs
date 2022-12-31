using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

// Game Manager - Utility functions
// For functions (and data containers) that require a GameObject
// Functions that do not require a GameObject should go into the
// Util class
public static partial class GM
{
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return helperObject.StartCoroutine(routine);
    }

    // cached singletons for ease of reference
    private static readonly Dictionary<System.Type, MonoBehaviour> gameplayComponentsCache = new Dictionary<System.Type, MonoBehaviour>();
    // finds a single object in the scene with the requested component and caches it
    // for easily obtaining a reference to a singleton behavior across scripts without serializing
    public static T FindSingle<T>() where T : MonoBehaviour
    {
        // cached
        if (gameplayComponentsCache.ContainsKey(typeof(T)))
            return (T)gameplayComponentsCache[typeof(T)];

        // slow search
        T found = null;
        for (int i = 0; found == null && i < helperObject.SearchRoots.Length; i++)
        {
            var all = helperObject.SearchRoots[i].GetComponentsInChildren<T>(true);
            if (all.Length > 0) found = all[0];
        }

        // cache
        if (found != null)
            gameplayComponentsCache[typeof(T)] = found;

        return found;
    }
}