using UnityEngine;

public class SimpleRotateBehavior : MonoBehaviour
{
    [SerializeField] private float startingRotation;
    [SerializeField] private float rotationSpeed;

    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        angle = startingRotation;
    }

    // Update is called once per frame
    void Update()
    {
        angle += Time.deltaTime * rotationSpeed;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
