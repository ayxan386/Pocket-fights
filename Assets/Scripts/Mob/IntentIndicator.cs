using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntentIndicator : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private List<IntentMapping> intents;

    public void UpdateIntent(ActionType type)
    {
        var intent = intents.Find(intent => intent.type == type);

        icon.sprite = intent.icon;
        icon.color = intent.color;
    }
}

[Serializable]
public class IntentMapping
{
    public ActionType type;
    public Sprite icon;
    public Color color = Color.white;
}