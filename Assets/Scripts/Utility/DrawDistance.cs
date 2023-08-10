using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawDistance : MonoBehaviour
{
    [SerializeField] private Transform circle;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetCirclePosition(Vector3 position)
    {
        if (transform.position != new Vector3(position.x, 0.02f, position.z))
        {
            transform.position = new Vector3(position.x, 0.02f, position.z);
        }
    }
}
