using UnityEngine;

public class DoorManager : Singleton<DoorManager>
{
    public DoorMap map;

    private CrossReferencedMap<string> connections;
    private Map<string> doorToScene;

    private void Start()
    {
        InitialiseMap();
    }

    private void InitialiseMap()
    {
        connections = new();
        doorToScene = new();

        foreach(var elem in map.connections)
        {
            connections.Add(elem.entry.door_id, elem.exit.door_id);
            doorToScene.Add(elem.entry.door_id, elem.entry.scene);
            doorToScene.Add(elem.exit.door_id, elem.exit.scene);
        }
    }


    public static string GetPair(string current)
    {
        return instance.connections.GetPair(current);
    }

}
