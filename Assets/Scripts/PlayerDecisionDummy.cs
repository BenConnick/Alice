using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDecisionDummy : MonoBehaviour
{
    [SerializeField] private float range;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseApprox = 2*(Input.mousePosition.x - Screen.width*.5f) / Screen.width;
        transform.localPosition = new Vector3(mouseApprox * range, 0, 0);
    }
}
