using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(fileName = "DialogueEntry", menuName = "Game/Dialogue Entry")]
public class DialogueEntry : ScriptableObject
{
    public QuestData quest;
    public QuestData questToGrantAfter;
    public DialogueTrigger trigger;
    public List<string> lines;
    public UnityEvent onExhausted;

    public bool Matches(QuestManager manager) => trigger switch
    {
        DialogueTrigger.Before => manager.ActiveQuest?.Data != quest && !manager.IsCompleted(quest),
        DialogueTrigger.During => manager.ActiveQuest?.Data == quest,
        DialogueTrigger.After => manager.IsCompleted(quest),
        _ => throw new System.NotImplementedException(),
    };
}