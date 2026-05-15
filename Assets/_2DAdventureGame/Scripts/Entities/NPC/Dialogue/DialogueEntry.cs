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
}
