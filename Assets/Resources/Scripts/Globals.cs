using System.Collections.Generic;
using Unity.AI.Navigation;

public class Globals
{
    public static int TERRAIN_LAYER_MASK = 1 << 8;

    //Stores buildings that the player has available to build
    public static BuildingData[] BUILDING_DATA;

    //Player's initial Resources and amount
    public static Dictionary<string, GameResource> GAME_RESOURCES =
        new Dictionary<string, GameResource>()
        {
            { "gold", new GameResource("Gold", 3000) },
            { "wood", new GameResource("Wood", 3000) },
            { "stone", new GameResource("Stone", 3000) }
        };

    //Stores units that have been selected by the player
    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();

    public static NavMeshSurface NAV_MESH_SURFACE;
    
    public static void UpdateNaveMeshSurface()
    {
        NAV_MESH_SURFACE.UpdateNavMesh(NAV_MESH_SURFACE.navMeshData);
    }

}
