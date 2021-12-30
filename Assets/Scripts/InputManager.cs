using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class InputManager : MonoBehaviour
{
    private FacingDirection prevWalkDirection;

    private readonly List<IInputListener> inputListeners = new List<IInputListener>();

    public static void AddInputListener(IInputListener listener)
    {
        var inst = UnityEngine.Object.FindObjectOfType<InputManager>();
        if (inst == null) return;
        inst.inputListeners.Add(listener);
    }

    public static void RemoveInputListener(IInputListener listener)
    {
        var inst = UnityEngine.Object.FindObjectOfType<InputManager>();
        if (inst == null) return;
        inst.inputListeners.Remove(listener);
    }

    // Update is called once per frame
    void Update()
    {
        // convert input to direction
        FacingDirection walkDirection = FacingDirection.None;
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            walkDirection = FacingDirection.Right;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            walkDirection = FacingDirection.Left;
        }
        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            walkDirection = FacingDirection.Up;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            walkDirection = FacingDirection.Down;
        }
        else
        {
            walkDirection = FacingDirection.None;
        }

        // OnDirectionChange
        if (walkDirection != prevWalkDirection)
        {
            foreach (var l in inputListeners)
            {
                l.OnDirectionChange(walkDirection);
            }
        }

        // OnInteractKeyUp
        //if (Input.GetKeyUp(DialogueUI.DialogueKey))
        //{
        //    Debug.Log("dialogue key");
        //    foreach (var l in inputListeners)
        //    {
        //        l.OnInteractKeyUp();
        //    }
        //}

        // always after all input
        prevWalkDirection = walkDirection;
    }
}

public interface IInputListener
{
    void OnDirectionChange(FacingDirection direction);
    void OnInteractKeyUp();
}