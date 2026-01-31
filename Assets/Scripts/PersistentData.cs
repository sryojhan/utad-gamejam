using UnityEngine;


[System.Serializable]
public class PersistentData<T>
{
    public string data_id;

    [HideInInspector]
    public T value;


    public void Load()
    {
        if (!Progress.instance.ContainsPersistent(data_id)) return;
        value = (T)Progress.instance.GetPersistent(data_id);
    }


    public void Save()
    {
        Progress.instance.UpdatePersistent(data_id, value);
    }
}
