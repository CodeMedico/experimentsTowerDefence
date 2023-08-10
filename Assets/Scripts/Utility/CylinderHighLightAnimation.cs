using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CylinderHighLightAnimation : MonoBehaviour
{
    [SerializeField] private float speedColorChange;
    private const string EMMISION = "_EmissionColor";
    private Color emmisionColor;
    private Material material;
    private bool depletGreen;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        emmisionColor = material.GetColor(EMMISION);
    }

    void Update()
    {
        if(emmisionColor.g > 30)
        {
            depletGreen= true;
        }
        if(emmisionColor.g <= 1)
        {
            depletGreen= false;
        }
        if(depletGreen)
        {
            emmisionColor.g -= Time.unscaledDeltaTime*speedColorChange;
            material.SetColor(EMMISION, emmisionColor);
        }
        else
        {
            emmisionColor.g += Time.unscaledDeltaTime*speedColorChange;
            material.SetColor(EMMISION, emmisionColor);
        }
    }

}
