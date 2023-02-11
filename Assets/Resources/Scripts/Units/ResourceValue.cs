using System.Collections;
using System.Collections.Generic;

/*
 * Defines a resource and a value required to create a unit
 * Used in a list to determine the cost of a unit
 */

[System.Serializable]
public class ResourceValue
{
    public string code = "";
    public int amount = 0;

    public ResourceValue(string code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}
