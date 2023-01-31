using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * 
 * GameObject: GAME
 * 
 * Manages and updates UI elements.
 */

public class UIManager : MonoBehaviour
{
    private BuildingPlacer _buildingPlacer;
    private Dictionary<string, TextMeshProUGUI> _resourceTexts;
    private Dictionary<string, Button> _buildingButtons;

    public Transform buildingMenu;
    public GameObject buildingButtonPrefab;
    public Transform resourcesUIParent;
    public GameObject gameResourceDisplayPrefab;

    public GameObject infoPanel;
    public GameObject gameResourceCostPrefab;
    private TextMeshProUGUI _infoPanelTitleText;
    private TextMeshProUGUI _infoPanelDescriptionText;
    private Transform _infoPanelResourcesCostParent;

    private void Awake()
    {
        _buildingPlacer = GetComponent<BuildingPlacer>();

        _resourceTexts = new Dictionary<string, TextMeshProUGUI>();
        foreach(KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourceDisplayPrefab, resourcesUIParent);
            display.name = pair.Key;
            _resourceTexts[pair.Key] = display.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            _SetResourceText(pair.Key, pair.Value.Amount);
        }

        //Create building buttons on UI for each type
        _buildingButtons = new Dictionary<string, Button>();
        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            BuildingData data = Globals.BUILDING_DATA[i];
            GameObject button = GameObject.Instantiate(buildingButtonPrefab,buildingMenu);
            button.name = data.unitName;
            button.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = data.unitName;
            Button b = button.GetComponent<Button>();
            _AddBuildingButtonListener(b, i);
            _buildingButtons[data.code] = b;
            if(!Globals.BUILDING_DATA[i].CanBuy())
            {
                b.interactable = false;
            }
            button.GetComponent<BuildingButton>().Initialize(Globals.BUILDING_DATA[i]);

        }

        //Building Info Panel
        Transform infoPanelTransform = infoPanel.transform;
        _infoPanelTitleText = infoPanelTransform.Find("Content/Title").GetComponent<TextMeshProUGUI>();
        _infoPanelDescriptionText = infoPanelTransform.Find("Content/Description").GetComponent<TextMeshProUGUI>();
        _infoPanelResourcesCostParent = infoPanelTransform.Find("Content/ResourcesCost");
        Debug.Log(_infoPanelResourcesCostParent);
        ShowInfoPanel(false);
    }

    private void _AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _buildingPlacer.SelectPlacedBuilding(i));
    }

    private void _SetResourceText(string resource, int value)
    {
        _resourceTexts[resource].text = value.ToString();
    }

    public void UpdateResourceTexts()
    {
        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            _SetResourceText(pair.Key, pair.Value.Amount);
        }
    }

    public void CheckBuildingButtons()
    {
        foreach(BuildingData data in Globals.BUILDING_DATA)
        {
            _buildingButtons[data.code].interactable = data.CanBuy();
        }
    }

    //Events
    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.AddListener("CheckBuildingButtons", _OnCheckBuildingButtons);

        //Info Panels
        EventManager.AddTypedListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.AddListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", _OnCheckBuildingButtons);

        //Info Panels
        EventManager.RemoveTypedListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
    }

    private void _OnUpdateResourceTexts()
    {
        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
            _SetResourceText(pair.Key, pair.Value.Amount);
    }

    private void _OnCheckBuildingButtons()
    {
        foreach (BuildingData data in Globals.BUILDING_DATA)
            _buildingButtons[data.code].interactable = data.CanBuy();
    }

    //Info Panel
    private void _OnHoverBuildingButton(CustomEventData data)
    {
        SetInfoPanel(data.buildingData);
        ShowInfoPanel(true);
    }

    private void _OnUnhoverBuildingButton()
    {
        ShowInfoPanel(false);
    }

    public void SetInfoPanel(BuildingData data)
    {
        //update text
        if(data.code != "")
        {
            _infoPanelTitleText.text = data.code;
        }
        if(data.description != "")
        {
            _infoPanelDescriptionText.text = data.description;
        }

        //clear resource costs and reinstantiate new ones
        foreach (Transform child in _infoPanelResourcesCostParent)
            Destroy(child.gameObject);

        if(data.cost.Count > 0)
        {
            GameObject g;
            Transform t;
            foreach(ResourceValue resource in data.cost)
            {
                g = GameObject.Instantiate(gameResourceCostPrefab, _infoPanelResourcesCostParent);
                t = g.transform;
                t.Find("Text").GetComponent<TextMeshProUGUI>().text = resource.amount.ToString();
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{resource.code}");
            }
        }
    }

    public void ShowInfoPanel(bool show)
    {
        infoPanel.SetActive(show);
    }

}
