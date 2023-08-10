using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPlate : MonoBehaviour
{
    [SerializeField] private Transform teamplate;

    private MeshRenderer meshRenderer;
    private Material material;
    private bool positionChanged;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;

        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (positionChanged)
        {
            if (PathFindManager.Instance.IsTileOccupied(transform.position))
            {
                Occupuied();
            }
            else
            {
                Unoccupied();
            }
        }
    }
    public void Unoccupied()
    {
        Color color = new Color();
        color.g = 1f;
        color.a = .4f;
        material.color = color;
    }

    public void Occupuied()
    {
        Color color = new Color();
        color.r = 1f;
        color.a = .4f;
        material.color = color;
    }

    public void SetTeamPlatePosition(Vector3 position)
    {
        if (transform.position != new Vector3(position.x, 0.02f, position.z))
        {
            transform.position = new Vector3(position.x, 0.02f, position.z);
            positionChanged = true;
        }
        else
        {
            positionChanged = false;
        }
    }
}
