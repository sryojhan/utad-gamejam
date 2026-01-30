using System.Collections.Generic;
using UnityEngine;

public class Progress : Singleton<Progress>
{
    protected override bool AutoInitialise => true;
    protected override bool ConserveBetweenScenes => true;


    private readonly HashSet<string> currentProgress = new();

    public static void Unlock(string id)
    {
        if (!instance.currentProgress.Contains(id))
            instance.currentProgress.Add(id);
    }

    public static bool IsUnlocked(string id)
    {
        return instance.currentProgress.Contains(id);
    }

}
