using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject selectionCircle;

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
        if (!Globals.SELECTED_UNITS.Contains(this)) return;
        Globals.SELECTED_UNITS.Remove(this);
        selectionCircle.SetActive(false);
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
        Globals.SELECTED_UNITS.Add(this);
        selectionCircle.SetActive(true);
    }

    protected virtual bool IsActive()
    {
        return true;
    }

}
