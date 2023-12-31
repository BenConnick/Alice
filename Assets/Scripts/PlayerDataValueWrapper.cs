using System;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Provides an event that fires whenever this value is changed.
/// </summary>
/// <remarks>Only works if the value is changed through the <see cref="Set"/> function of this class.
/// If the value accessed from elsewhere, the event will not fire.</remarks>
/// <typeparam name="T"></typeparam>
[Serializable]
public abstract class PlayerDataValueWrapper<T> : IPlayerDataValueWrapper
{
    [SerializeField]
    protected T backingField;
    
    public string KeyName { get; protected set; }
    
    public abstract string Serialize();

    public abstract void Deserialize(string serialized);
    
    public PlayerDataValueWrapper<TAny> Unbox<TAny>()
    {
        return this as PlayerDataValueWrapper<TAny>;
    }

    public T Value => backingField;

    public event Action<T> ValueWasChangedEvent;

    public PlayerDataValueWrapper(T initialValue, string keyName)
    {
        KeyName = keyName;
        backingField = initialValue;
    }

    public void Set(T newValue)
    {
        if (Equals(newValue, backingField)) return;

        backingField = newValue;
        
        ValueWasChangedEvent?.Invoke(newValue);
    }
}

public interface IPlayerDataValueWrapper
{
    string KeyName { get; }
    string Serialize();
    void Deserialize(string serialized);
    PlayerDataValueWrapper<T> Unbox<T>();
}

[Serializable]
public class FloatWrapper : PlayerDataValueWrapper<float>
{
    public FloatWrapper(float initialValue, string keyName) : base(initialValue, keyName)
    {
    }

    public override string Serialize()
    {
        return backingField.ToString(CultureInfo.InvariantCulture);
    }

    public override void Deserialize(string serialized)
    {
        float.TryParse(serialized, out backingField);
    }
}

[Serializable]
public class IntWrapper : PlayerDataValueWrapper<int>
{
    public IntWrapper(int initialValue, string keyName) : base(initialValue, keyName)
    {
    }


    public override string Serialize()
    {
        return backingField.ToString(CultureInfo.InvariantCulture);
    }

    public override void Deserialize(string serialized)
    {
        int.TryParse(serialized, out backingField);
    }
}

[Serializable]
public class BoolWrapper : PlayerDataValueWrapper<bool>
{
    public BoolWrapper(bool initialValue, string keyName) : base(initialValue, keyName)
    {
    }
    
    public override string Serialize()
    {
        return backingField.ToString(CultureInfo.InvariantCulture);
    }

    public override void Deserialize(string serialized)
    {
        bool.TryParse(serialized, out backingField);
    }
}

[Serializable]
public class EnumWrapper<T> : PlayerDataValueWrapper<T> where T : Enum
{
    public EnumWrapper(T initialValue, string keyName) : base(initialValue, keyName)
    {
    }

    public override string Serialize()
    {
        return GetValueIndex(backingField).ToString(CultureInfo.InvariantCulture);
    }

    public override void Deserialize(string serialized)
    {
        int.TryParse(serialized, out int result);
        backingField = (T)Enum.ToObject(typeof(T), result);
    }

    private static int GetValueIndex(Enum enumValue)
    {
        var values = Enum.GetValues(typeof(T));
        for (int i = 0; i < values.Length; i++)
        {
            if (Equals(enumValue, (T)values.GetValue(i)))
            {
                return i;
            }
        }

        return 0;
    }
}