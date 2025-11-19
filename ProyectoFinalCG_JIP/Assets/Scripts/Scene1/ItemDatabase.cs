using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    private static Dictionary<string, Sprite> iconDict;

    public static void LoadIcons()
    {
        iconDict = new Dictionary<string, Sprite>();

        Sprite[] icons = Resources.LoadAll<Sprite>("ItemIcons");

        foreach (var icon in icons)
        {
            iconDict[icon.name] = icon;
        }
    }

    public static Sprite GetIcon(string itemName)
    {
        if (iconDict == null) LoadIcons();

        return iconDict.ContainsKey(itemName) ? iconDict[itemName] : null;
    }
}