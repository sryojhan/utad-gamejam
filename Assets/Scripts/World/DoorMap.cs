using UnityEngine;

[CreateAssetMenu(fileName = "Door map")]
public class DoorMap : ScriptableObject
{
    [System.Serializable]
    public struct DoorConnection
    {
        [System.Serializable]
        public struct DoorInfo
        {
            public string scene;
            public string door_id;
        }

        public DoorInfo entry;
        public DoorInfo exit;
    }


    public DoorConnection[] connections;
}
