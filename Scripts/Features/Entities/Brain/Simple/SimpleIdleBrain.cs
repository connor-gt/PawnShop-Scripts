using UnityEngine;

public class SimpleIdleBrain : Brain, IEntityInteractionHandler
{
    private readonly SimpleIdleBrainSO brainData;

    private readonly IdleIndefinitelyState idleIndefinitelyState;

    private readonly InteractionModule interactionModule;
    private readonly DialogueModule dialogueModule;

    private int alternateDialogue = 0;

    public SimpleIdleBrain(Entity entity, SimpleIdleBrainSO brainData) : base(entity)
    {
        this.brainData = brainData;

        interactionModule = entity.GetComponent<InteractionModule>();
        dialogueModule = entity.GetComponent<DialogueModule>();

        idleIndefinitelyState = new IdleIndefinitelyState(entity);
    }

    public override void StartThinking()
    {
        TransitionTo(idleIndefinitelyState);
    }

    public string GetHoverMessage()
    {
        if (entity.StateMachine.ActiveState is IEntityInteractionHandler stateInteraction)
        {
            return stateInteraction.GetHoverMessage();
        }

        return interactionModule.BuildPrompt("Talk");
    }

    public void InteractWithEntity()
    {
        if (entity.StateMachine.ActiveState is IEntityInteractionHandler stateInteraction)
        {
            stateInteraction.InteractWithEntity();
            return;
        }

        if (dialogueModule.IsPaused)
        {
            dialogueModule.ResumeConversation();
        }
        else if (!dialogueModule.InConversation && alternateDialogue == 0)
        {
            alternateDialogue = 1;
            dialogueModule.StartConversation(brainData.ConversationDialogue);
        }
        else if (!dialogueModule.InConversation)
        {
            alternateDialogue = 0;
            dialogueModule.StartConversation(brainData.GenericDialogue);
        }
    }
}
