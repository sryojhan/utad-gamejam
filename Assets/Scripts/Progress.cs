using System.Collections.Generic;
using UnityEngine;

public class Progress : Singleton<Progress>
{
    protected override bool AutoInitialise => true;
    protected override bool ConserveBetweenScenes => true;


    private readonly HashSet<string> currentProgress = new();

    private void Awake()
    {
        if (DestroyIfInitialised(this)) return;

        EnsureInitialised();
    }


    public string last_door_id = "";

    public static void Unlock(string id)
    {
        if (!instance.currentProgress.Contains(id))
        {
            print("Unlock: id");
            instance.currentProgress.Add(id);
        }
    }

    public static bool IsUnlocked(string id)
    {
        return instance.currentProgress.Contains(id);
    }


    readonly Map<object> persistentData = new();

    public void UpdatePersistent(string id, object obj)
    {
        persistentData[id] = obj;
    }

    public bool ContainsPersistent(string id)
    {
        return persistentData.ContainsKey(id);
    }

    public object GetPersistent(string id)
    {
        return persistentData[id];
    }

}
