
using UnityEngine;

public class LighDirection : MonoBehaviour
{
    [SerializeField] public GameObject prefab;

    private Transform prefabVisual;
    private Material enemyMaterial;

    private void Awake()
    {
        prefabVisual = prefab.transform.GetChild(0);
        enemyMaterial = prefabVisual.GetComponent<MeshRenderer>().sharedMaterial;
        enemyMaterial.SetVector("_Light",transform.rotation*(-Vector3.forward)*2);
    }
}
