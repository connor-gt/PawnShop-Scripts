using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleIdleBrain", menuName = "Scriptable Objects/Brains/Simple Idle Brain")]
public class SimpleIdleBrainSO : BrainSO
{
    [field: Title("Dialogue")]
    [field: SerializeField]
    public DialogueSO GenericDialogue { get; private set; }

    [field: SerializeField]
    public DialogueSO ConversationDialogue { get; private set; }
    
    public override Brain CreateBrain(Entity entity)
    {
        return new SimpleIdleBrain(entity, this);
    }
}
