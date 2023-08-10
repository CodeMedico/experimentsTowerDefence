using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTowerUperUnimaation : MonoBehaviour
{
    public float speedOfFloating = 1f;
    private float startingY;

    private void Start()
    {
        startingY = transform.position.y;
    }
    void Update()
    {
        transform.position = new Vector3(transform.position.x, startingY + Mathf.Sin(Time.time)*0.3f, transform.position.z);
        transform.Rotate(Vector3.up * Mathf.Sin(Time.time)*5);
    }
}
