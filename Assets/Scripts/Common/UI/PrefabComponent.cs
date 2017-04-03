using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIPrefab
{
    public string name;
    public Sprite sprite;
}

public class PrefabComponent : MonoBehaviour 
{
    [SerializeField]
    public UIPrefab[] uiPrefab;

    private Dictionary<string, Sprite> spriteDic;

    public Dictionary<string, Sprite> SpriteDic
    {
        get { return spriteDic; }
    }

    public void Init()
    {
        spriteDic = new Dictionary<string, Sprite>();
        for (int i = 0; i < uiPrefab.Length; ++i )
        {
            spriteDic.Add(uiPrefab[i].name, uiPrefab[i].sprite);
        }
    }
}
