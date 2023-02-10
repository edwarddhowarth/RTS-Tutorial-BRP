using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * GameObject: Any Unit
 * 
 * Handles the player interaction elements of units
 */

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    public GameObject selectionCircle;

    private Transform _canvas;
    private GameObject _healthbar;
    protected BoxCollider _collider;
    public virtual Unit Unit { get; set; }

    private void Awake()
    {
        _canvas = GameObject.Find("Canvas").transform;
    }

    public void Select() { Select(false, false); }
    public void Select(bool singleClick, bool holdingShift)
    {
        if(!singleClick)
        {
            _SelectUtil();
            return;
        }

        if(!holdingShift)
        {
            List<UnitManager> selectedUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
            foreach (UnitManager um in selectedUnits)
                um.Deselect();
            _SelectUtil();
        }
        else
        {
            if (!Globals.SELECTED_UNITS.Contains(this))
                _SelectUtil();
            else
                Deselect();
        }
    }

    public void Deselect()
    {
        Globals.SELECTED_UNITS.Remove(this);
        selectionCircle.SetActive(false);
        Destroy(_healthbar);
        _healthbar = null;
        EventManager.TriggerTypedEvent("DeselectUnit", new CustomEventData(Unit));
    }

    private void OnMouseDown()
    {
        if (IsActive())
            Select(
                true, 
                Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
                );
    }

    private void _SelectUtil()
    {
        if (Globals.SELECTED_UNITS.Contains(this)) return;
        
        EventManager.TriggerTypedEvent("SelectUnit", new CustomEventData(Unit));
        Globals.SELECTED_UNITS.Add(this);
        selectionCircle.SetActive(true);
        if (_healthbar == null)
        {
            _healthbar = GameObject.Instantiate(Resources.Load("Prefabs/UI/Healthbar")) as GameObject;
            _healthbar.transform.SetParent(_canvas);
            Healthbar h = _healthbar.GetComponent<Healthbar>();
            Rect boundingBox = Utils.GetBoundingBoxOnScreen(
                transform.Find("Mesh").GetComponent<Renderer>().bounds,
                Camera.main
                );
            h.Initialize(transform, boundingBox.height);
            h.SetPosition();
            
        }
        
    }

    protected virtual bool IsActive()
    {
        return true;
    }

    public void Initialize(Unit unit)
    {
        _collider = GetComponent<BoxCollider>();
        Unit = unit;
    }

    

}
