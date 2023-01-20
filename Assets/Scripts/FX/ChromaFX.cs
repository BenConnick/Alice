using UnityEngine;
using System.Collections;
using Unity.Collections;
using UnityEngine.UI;

public class ChromaFX : MonoBehaviour
{
    public Material SourceMaterial;

    public string ShiftPropertyName = "_Shift";

    [Range(-1,1)]
    public float Shift;

    public float ShiftScale=1;

    [ReadOnlyField]
    public Material MaterialInstance;

    [SerializeField] private RawImage renderer;

    // Use this for initialization
    void Start()
    {
        MaterialInstance = new Material(SourceMaterial);
        renderer.material = MaterialInstance;
    }

    // Update is called once per frame
    void Update()
    {
        MaterialInstance.SetFloat(ShiftPropertyName, Shift*ShiftScale);
    }
}
