using UnityEngine;
using UnityEngine.Events;

public class SimpleKeyListener : MonoBehaviour
{
    public KeyCode[] Keys;
    public UnityEvent KeyPressedEvent;

    // Update is called once per frame
    void Update()
    {
        foreach (var k in Keys)
        {
            if (Input.GetKeyDown(k))
            {
                KeyPressedEvent.Invoke();
                return;
            }
        }
    }
}
