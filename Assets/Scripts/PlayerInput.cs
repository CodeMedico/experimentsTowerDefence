using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private LayerMask planeLayerMask;
    [SerializeField] private LevelUI levelUI;

    private Vector3 mousePointRounded;

    private void Update()
    {
        if (levelUI.state == LevelUI.UIState.Building)
        {
            mousePointRounded = ConvertScreenMousePositionToPlane();
            levelUI.ShowQuad(mousePointRounded);
            levelUI.ShowTowerRadius(mousePointRounded);
            if (Input.GetMouseButtonDown(0) && !PathFindManager.Instance.IsTileOccupied(mousePointRounded))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    levelUI.BuildAndRepeat();
                }
                else
                {
                    levelUI.FinishBuilding(true);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                levelUI.FinishBuilding(false);
            }
        }
        else if (levelUI.state == LevelUI.UIState.Merge)
        {
            mousePointRounded = ConvertScreenMousePositionToPlane();
            levelUI.ShowQuad(mousePointRounded);
            if (Input.GetMouseButtonDown(0))
            {
                if (PathFindManager.Instance.GetbuildingsReferences().ContainsKey(mousePointRounded) && 
                    PathFindManager.Instance.GetbuildingsReferences()[mousePointRounded].TryGetComponent<Tower>(out Tower tower))
                {
                    
                    
                        levelUI.PickupTower(tower.gameObject);
                    
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                levelUI.FinishBuilding(false);
                levelUI.DestroyHighlights();
            }
        }
    }

    private Vector3 ConvertScreenMousePositionToPlane()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Physics.Raycast(ray, out RaycastHit hitToPlane, Mathf.Infinity, planeLayerMask);
        return PathFindManager.Instance.RoundVector(hitToPlane.point);
    }

    public Vector3 GetMousePositionOnPlane()
    {
        return mousePointRounded;
    }

}
