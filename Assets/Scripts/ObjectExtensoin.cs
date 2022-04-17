using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtension
{
    private static readonly List<GameObject> SavedObjects = new();

    public static void DontDestroyOnLoad(this GameObject obj)
    {
        SavedObjects.Add(obj);
        Object.DontDestroyOnLoad(obj);
    }

    public static void Destroy(this GameObject obj)
    {
        SavedObjects.Remove(obj);
        Destroy(obj);
    }

    public static List<GameObject> GetSavedObjects()
    {
        return new List<GameObject>(SavedObjects);
    }
}