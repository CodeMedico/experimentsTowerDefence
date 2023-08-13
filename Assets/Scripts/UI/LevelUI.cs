using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject containerTowerButtons;
    [SerializeField] private GameObject cylinderHightLight;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button mergeButton;
    [SerializeField] private Button[] towerButtons;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private List<TowerSO> towerSOs = new List<TowerSO>();
    [SerializeField] private Transform teamplate;
    [SerializeField] private DrawDistance drawDistance;
    [SerializeField] private List<TowerRecipeSO> towerRecipeSOs = new List<TowerRecipeSO>();


    private GameObject towerBuildingPreview;
    private List<GameObject> cylinderHighLightInstances = new List<GameObject>();

    public enum UIState
    {
        Main,
        BuildMenu,
        Building,
        Merge,
    }
    public UIState state;
    void Start()
    {
        foreach (Button button in towerButtons)
        {
            foreach (TowerSO towerSO in towerSOs)
            {
                if (towerSO.Name == button.GetComponent<ButtonName>().GetName())
                {
                    button.onClick.AddListener(() => TowerBuildPreview(towerSO));
                }
            }
        }
        buildButton.onClick.AddListener(BuildState);
        mergeButton.onClick.AddListener(MergeState);
        containerTowerButtons.SetActive(false);
        cylinderHightLight.SetActive(false);

    }
    private void Update()
    {
        if (state == UIState.BuildMenu)
        {

            Time.timeScale = 0f;
            Vector2 position = containerTowerButtons.GetComponent<RectTransform>().anchoredPosition;
            containerTowerButtons.GetComponent<RectTransform>().anchoredPosition =
                Vector2.MoveTowards(position, new Vector2(-50f, position.y), 2000f * Time.unscaledDeltaTime);
        }
        else if (state == UIState.Merge)
        {

        }
        else
        {
            Vector2 position = containerTowerButtons.GetComponent<RectTransform>().anchoredPosition;
            containerTowerButtons.GetComponent<RectTransform>().anchoredPosition =
                Vector2.MoveTowards(position, new Vector2(280f, position.y), 2000f * Time.unscaledDeltaTime);
            if (containerTowerButtons.GetComponent<RectTransform>().anchoredPosition.x >= 260f)
            {
                containerTowerButtons.SetActive(false);
            }
        }
    }

    private void TowerBuildPreview(TowerSO towerSO)
    {
        state = UIState.Building;
        towerBuildingPreview = Instantiate(towerSO.Prefab, playerInput.GetMousePositionOnPlane(), Quaternion.identity);
    }
    public void ShowQuad(Vector3 position)
    {
        teamplate.gameObject.SetActive(true);
        teamplate.GetComponent<TeamPlate>().SetTeamPlatePosition(position);
        if (towerBuildingPreview != null)
        {
            towerBuildingPreview.transform.position = position;
        }
    }

    public void ShowTowerRadius(Vector3 position)
    {
        drawDistance.gameObject.SetActive(true);
        drawDistance.SetCirclePosition(position);
        if (towerBuildingPreview != null)
        {
            float radius = towerBuildingPreview.GetComponent<Tower>().GetTowerSO().TowerRange * 2f;
            drawDistance.transform.localScale = new Vector3(radius, radius, radius);
            towerBuildingPreview.transform.position = position;
        }
    }

    public void FinishBuilding(bool isFinishBuilding)
    {
        state = UIState.Main;
        teamplate.gameObject.SetActive(false);
        drawDistance.gameObject.SetActive(false);
        buildButton.gameObject.SetActive(true);
        mergeButton.gameObject.SetActive(true);
        Time.timeScale = 1f;

        if (isFinishBuilding)
        {
            towerBuildingPreview.GetComponent<Tower>().enabled = true;
        }
        else
        {
            Destroy(towerBuildingPreview);
        }
    }

    public void BuildAndRepeat()
    {
        towerBuildingPreview.GetComponent<Tower>().enabled = true;
        if (towerBuildingPreview.TryGetComponent<Tower>(out Tower tower))
        {
            towerBuildingPreview = Instantiate(tower.GetTowerSO().Prefab, playerInput.GetMousePositionOnPlane(), Quaternion.identity);
        }
        teamplate.GetComponent<TeamPlate>().Occupuied();
    }

    private void BuildState()
    {
        containerTowerButtons.SetActive(true);
        HideMainBuildMenu();
        state = UIState.BuildMenu;
    }

    private void MergeState()
    {
        state = UIState.Merge;
        towerBuildingPreview = null;
        Time.timeScale = 0f;
        Dictionary<Vector3, Transform> towers = PathFindManager.Instance.GetbuildingsReferences();
        foreach (Transform transform in towers.Values)
        {
            if (transform.TryGetComponent<Tower>(out Tower tower))
            {
                if (tower.GetTowerSO().TowerTier == TowerSO.Tier.One)
                {
                    GameObject newHighLight = Instantiate(cylinderHightLight, transform.position, Quaternion.identity, transform);
                    cylinderHighLightInstances.Add(newHighLight);
                    newHighLight.SetActive(true);
                }
            }
        }
        HideMainBuildMenu();
    }

    private void HideMainBuildMenu()
    {
        buildButton.gameObject.SetActive(false);
        mergeButton.gameObject.SetActive(false);
    }

    public void PickupTower(GameObject gameObject)
    {
        Tower tower = gameObject.GetComponent<Tower>();
        PathFindManager.Instance.RemovePosition(tower as Tower);
        if (towerBuildingPreview != null)
        {
            MergeTower(tower);
        }
        else if (towerBuildingPreview == null)
        {
            towerBuildingPreview = gameObject;
            DestroyHighlights();
        }
    }

    public void MergeTower(Tower tower)
    {
        foreach (TowerRecipeSO towerRecipeSO in towerRecipeSOs)
        {
            if (tower.GetTowerSO() == towerRecipeSO.inFirstTower)
            {
                if (towerBuildingPreview.GetComponent<Tower>().GetTowerSO() == towerRecipeSO.inSecondTower)
                {
                    Destroy(tower.gameObject);
                    Destroy(towerBuildingPreview);
                    towerBuildingPreview = Instantiate(towerRecipeSO.outTower.Prefab, playerInput.GetMousePositionOnPlane(),Quaternion.identity);
                    FinishBuilding(true);
                    break;
                }
            }
            else if (tower.GetTowerSO() == towerRecipeSO.inSecondTower)
            {
                if (towerBuildingPreview.GetComponent<Tower>().GetTowerSO() == towerRecipeSO.inFirstTower)
                {
                    Destroy(tower.gameObject);
                    Destroy(towerBuildingPreview);
                    towerBuildingPreview = Instantiate(towerRecipeSO.outTower.Prefab, playerInput.GetMousePositionOnPlane(), Quaternion.identity);
                    FinishBuilding(true);
                    break;
                }
            }

        }

    }

    public void DestroyHighlights()
    {
        foreach (var cylinder in cylinderHighLightInstances)
        {
            Destroy(cylinder.gameObject);
        }
    }

    public GameObject GettowerBuildingPreview()
    {
        return towerBuildingPreview;
    }
}
