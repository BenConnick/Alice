using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Vector2 MoveSpeed; // only constant velocity

    private Vector3 innerPos;

    // Start is called before the first frame update
    void Start()
    {
        innerPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Time.deltaTime * MoveSpeed;
        innerPos += new Vector3(move.x, move.y);
        transform.position = Util.GetLowResPosition(innerPos);
    }
}
