using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialogueEntry", menuName = "Game/Dialogue Entry")]
public class DialogueEntry : ScriptableObject
{
    public QuestData Quest { get; set; }
    public QuestData QuestToGrantAfter { get; set; }
    public DialogueTrigger Trigger { get; set; }
    public List<string> Lines { get; set; }
    public UnityEvent OnExhausted { get; set; }

    public bool Matches(QuestManager manager) => Trigger switch
    {
        DialogueTrigger.Before => manager.ActiveQuest?.Data != Quest && manager.IsCompleted(Quest),
        DialogueTrigger.During => manager.ActiveQuest?.Data == Quest,
        DialogueTrigger.After => manager.IsCompleted(Quest),
        _ => throw new System.NotImplementedException(),
    };
}