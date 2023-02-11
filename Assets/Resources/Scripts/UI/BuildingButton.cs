using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * 
 * GameObject: UI Building Buttons
 * 
 * Provides visual feedback on UI building button elements
 */

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BuildingData _buildingData;

    public void Initialize(BuildingData buildingData)
    {
        _buildingData = buildingData;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.TriggerEvent(EventName.HoverBuildingButton, _buildingData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.TriggerEvent(EventName.UnhoverBuildingButton);
    }
}
