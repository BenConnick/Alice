using System;
using UnityEngine;

[Serializable]
public abstract class SerializableDataAsset<TData> : ScriptableObject
{
    [SerializeField] 
    public TData Data;
}