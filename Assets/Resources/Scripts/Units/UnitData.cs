using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base data class for Units such as Buildings and Characters
 */
[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    public string code;
    public string unitName;
    public string description;
    public int healthpoints;
    public GameObject prefab;
    public List<ResourceValue> cost;
    public List<SkillData> skills = new List<SkillData>();
    public float fieldOfView;

    public bool CanBuy()
    {
        foreach(ResourceValue resource in cost)
        {
            if (Globals.GAME_RESOURCES[resource.code].Amount < resource.amount)
                return false;
            
        }
        return true;
    }
}
