using System;
using UnityEngine;

/// <summary>
/// Subclass this to create an asset that has a singleton which is lazily loaded the first time it is referenced. 
/// </summary>
/// <typeparam name="T">The type of the subclass that inherits from this</typeparam>
/// <typeparam name="TAsset">The asset that loads the underlying type</typeparam>
[Serializable]
public abstract class LazyLoadedGlobalResource<T, TAsset> where TAsset : SerializableDataAsset<T>
{
    protected static T _Singleton;
    protected static bool _Loaded;

    public static T GetSingleton()
    {
        LazyLoad();
        return _Singleton;
    }
    
    protected static void LazyLoad()
    {
        if (_Loaded) return;

        string resourceLocation = typeof(T).Name;
        var asset = Resources.Load<TAsset>(resourceLocation);
        if (asset == null)
        {
            Debug.LogWarning($"Failed to load resource '{resourceLocation}'");
        }
        else
        {
            _Singleton = asset.Data;
            _Loaded = true;
        }
    }
}