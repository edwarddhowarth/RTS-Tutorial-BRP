using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Instantiated by BuildingPlacer.cs 
 * 
 * Contains data and methods about a building
 */

public enum BuildingPlacement
{
    VALID,
    INVALID,
    FIXED
}

public class Building : Unit
{
    private BuildingPlacement _placement;
    private List<Material> _materials;
    private BuildingManager _buildingManager;

    public Building(BuildingData data) : this(data, new List<ResourceValue>() { }) { }
    public Building(BuildingData data, List<ResourceValue> production) : base(data, production)
    {
        _buildingManager = _transform.GetComponent<BuildingManager>();
        _materials = new List<Material>();
        foreach(Material material in _transform.Find("Mesh").GetComponent<Renderer>().materials)
        {
            _materials.Add(new Material(material));
        }

        _placement = BuildingPlacement.VALID;
        SetMaterials();
    }

    public void SetMaterials()
    {
        SetMaterials(_placement);
    }

    public void SetMaterials(BuildingPlacement placement)
    {
        List<Material> materials;
        if(placement == BuildingPlacement.VALID)
        {
            Material refMaterial = Resources.Load("Materials/Valid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if(placement == BuildingPlacement.INVALID)
        {
            Material refMaterial = Resources.Load("Materials/Invalid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if(placement == BuildingPlacement.FIXED)
        {
            materials = _materials;
        }
        else
        {
            return;
        }
        _transform.Find("Mesh").GetComponent<Renderer>().materials = materials.ToArray();
    }
    

    public override void Place()
    {
        base.Place();
        _placement = BuildingPlacement.FIXED;
        SetMaterials();
    }
    

    public void CheckValidPlacement()
    {
        if (_placement == BuildingPlacement.FIXED) return;
        _placement = _buildingManager.CheckPlacement()
            ? BuildingPlacement.VALID
            : BuildingPlacement.INVALID;
    }

    public bool IsFixed { get => _placement == BuildingPlacement.FIXED; }
    public bool HasValidPlacement { get => _placement == BuildingPlacement.VALID; }
    public int DataIndex
    {
        get
        {
            for(int i = 0; i < Globals.BUILDING_DATA.Length; i++)
            {
                if(Globals.BUILDING_DATA[i].code == _data.code)
                {
                    return i;
                }
            }
            return -1;
        }
    }

}

    

