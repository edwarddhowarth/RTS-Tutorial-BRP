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

    //Building Panel
    public Transform buildingMenu;
    public GameObject buildingButtonPrefab;
    
    //Resource Panel
    public Transform resourcesUIParent;
    public GameObject gameResourceDisplayPrefab;
    
    //Info Panel
    public GameObject infoPanel;
    public GameObject gameResourceCostPrefab;
    public Color invalidTextColor;
    private TextMeshProUGUI _infoPanelTitleText;
    private TextMeshProUGUI _infoPanelDescriptionText;
    private Transform _infoPanelResourcesCostParent;
    
    //Selected Units List Panel
    public Transform selectedUnitsListParent;
    public GameObject selectedUnitsDisplayPrefab;
    
    //Control Group Panel
    public Transform selectionGroupsParent;
    
    //Selected Unit Control Panel
    public GameObject selectedUnitMenu;
    private RectTransform _selectedUnitContentRectTransform;
    private RectTransform _selectedUnitButtonsRectTransform;
    private TextMeshProUGUI _selectedUnitTitleText;
    private TextMeshProUGUI _selectedUnitLevelText;
    private Transform _selectedUnitResourcesProductionParent;
    private Transform _selectedUnitActionButtonsParent;
    

    private void Awake()
    {
        _buildingPlacer = GetComponent<BuildingPlacer>();

        _resourceTexts = new Dictionary<string, TextMeshProUGUI>();
        foreach(KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourceDisplayPrefab, resourcesUIParent);
            display.name = pair.Key;
            _resourceTexts[pair.Key] = display.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            display.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{pair.Key}");
            Debug.Log(display.transform.Find("Icon").GetComponent<Image>().sprite);
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
        ShowInfoPanel(false);
        
        //Selection Group
        for (int i = 1; i <= 9; i++)
            ToggleSelectionGroupButton(i, false);
        
        //Controlled Unit Panel
        Transform selectedUnitMenuTransform = selectedUnitMenu.transform;
        _selectedUnitContentRectTransform = selectedUnitMenuTransform.Find("Content").GetComponent<RectTransform>();
        _selectedUnitButtonsRectTransform = selectedUnitMenuTransform.Find("Buttons").GetComponent<RectTransform>();
        _selectedUnitTitleText = selectedUnitMenuTransform.Find("Content/Title").GetComponent<TextMeshProUGUI>();
        _selectedUnitLevelText = selectedUnitMenuTransform.Find("Content/Level").GetComponent<TextMeshProUGUI>();
        _selectedUnitResourcesProductionParent = selectedUnitMenuTransform.Find("Content/ResourcesProduction");
        _selectedUnitActionButtonsParent = selectedUnitMenuTransform.Find("Buttons/SpecificActions");

        _ShowSelectedUnitMenu(false);
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
        
        //Selected Units Panel
        EventManager.AddTypedListener("SelectUnit", _OnSelectUnit);
        EventManager.AddTypedListener("DeselectUnit", _OnDeselectUnit);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", _OnCheckBuildingButtons);

        //Info Panels
        EventManager.RemoveTypedListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
        
        //Selected Units Panel
        EventManager.RemoveTypedListener("SelectUnit", _OnSelectUnit);
        EventManager.RemoveTypedListener("DeselectUnit", _OnDeselectUnit);
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
        SetInfoPanel(data.unitData);
        ShowInfoPanel(true);
    }

    private void _OnUnhoverBuildingButton()
    {
        ShowInfoPanel(false);
    }

    public void SetInfoPanel(UnitData data)
    {
        //update text
        if(data.unitName != "")
        {
            _infoPanelTitleText.text = data.unitName;
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

                //Check resource requirement and set correct text color
                if(Globals.GAME_RESOURCES[resource.code].Amount < resource.amount)
                {
                    t.Find("Text").GetComponent<TextMeshProUGUI>().color = invalidTextColor;
                }
            }
        }
    }

    public void ShowInfoPanel(bool show)
    {
        infoPanel.SetActive(show);
    }

    private void _OnSelectUnit(CustomEventData data)
    {
        _AddSelectedUnitToUIList(data.unit);
        _SetSelectedUnitMenu(data.unit);
        _ShowSelectedUnitMenu(true);
    }

    private void _OnDeselectUnit(CustomEventData data)
    {
        _RemoveSelectedUnitFromUIList(data.unit.Code);
        if (Globals.SELECTED_UNITS.Count == 0)
            _ShowSelectedUnitMenu(false);
        else
            _SetSelectedUnitMenu(Globals.SELECTED_UNITS[Globals.SELECTED_UNITS.Count - 1].Unit);
    }

    public void _AddSelectedUnitToUIList(Unit unit)
    {
        //Check for already added units of same type and increase counter
        Transform alreadyInstantiatedChild = selectedUnitsListParent.Find(unit.Code);
        if (alreadyInstantiatedChild != null)
        {
            TextMeshProUGUI t = alreadyInstantiatedChild.Find("Count").GetComponent<TextMeshProUGUI>();
            int count = int.Parse(t.text);
            t.text = (count + 1).ToString();
        }
        else //Add new unit counter icon to list
        {
            GameObject g = GameObject.Instantiate(selectedUnitsDisplayPrefab, selectedUnitsListParent);
            g.name = unit.Code;
            Transform t = g.transform;
            t.Find("Count").GetComponent<TextMeshProUGUI>().text = "1";
            t.Find("Name").GetComponent<TextMeshProUGUI>().text = unit.Data.unitName;
        }
    }

    public void _RemoveSelectedUnitFromUIList(string code)
    {
        Transform listItem = selectedUnitsListParent.Find(code);
        if (listItem == null) return;
        TextMeshProUGUI t = listItem.Find("Count").GetComponent<TextMeshProUGUI>();
        int count = int.Parse(t.text);
        count -= 1;
        if(count == 0)
            DestroyImmediate(listItem.gameObject);
        else
            t.text = count.ToString();
    }
    
    //Selection Group
    public void ToggleSelectionGroupButton(int groupIndex, bool on)
    {
        selectionGroupsParent.Find(groupIndex.ToString()).gameObject.SetActive(on);
    }
    
    //Controlled Unit Panel
    private void _SetSelectedUnitMenu(Unit unit)
    {
        _selectedUnitTitleText.text = unit.Data.unitName;
        _selectedUnitLevelText.text = $"Level {unit.Level}";
        
        foreach(Transform child in _selectedUnitResourcesProductionParent)
            Destroy(child.gameObject);

        //Shows icons if the unit/building adds resources to the player
        if (unit.Production.Count > 0)
        {
            GameObject g;
            Transform t;

            foreach (ResourceValue resource in unit.Production)
            {
                Debug.Log("Adding Resources");
                g = GameObject.Instantiate(gameResourceCostPrefab, _selectedUnitResourcesProductionParent);
                t = g.transform;
                t.Find("Text").GetComponent<TextMeshProUGUI>().text = $"+{resource.amount}";
                t.Find("Icon").GetComponent<Image>().sprite =
                    Resources.Load<Sprite>($"Textures/GameResources/{resource.code}");
            }
        }
        else
            Debug.Log("No Resources");
    }   

    private void _ShowSelectedUnitMenu(bool show)
    {
        selectedUnitMenu.SetActive(show);
    }

}
