/// <summary>
/// Keeps the guide at the sell counter until the player finishes the introduction conversation.
/// </summary>
public class IntroductionCounterInteraction : CounterInteraction
{
    private readonly DialogueSO introductionDialogue;

    public IntroductionCounterInteraction(DialogueSO introductionDialogue, Counter counter, Entity entity)
        : base(counter, entity)
    {
        this.introductionDialogue = introductionDialogue;
    }

    public override void Begin()
    {
        base.Begin();

        dialogueModule.OnConversationEnded += OnConversationEnded;
        dialogueModule.OnConversationPaused += UpdateHoverMessage;
        dialogueModule.OnConversationResumed += UpdateHoverMessage;

        UpdateHoverMessage();
    }

    public override void InteractWithEntity()
    {
        base.InteractWithEntity();

        if (dialogueModule.IsPaused)
        {
            dialogueModule.ResumeConversation();
        }
        else if (!dialogueModule.InConversation)
        {
            dialogueModule.StartConversation(introductionDialogue);
            UpdateHoverMessage();
        }
    }

    public override void Finish()
    {
        dialogueModule.OnConversationEnded -= OnConversationEnded;
        dialogueModule.OnConversationPaused -= UpdateHoverMessage;
        dialogueModule.OnConversationResumed -= UpdateHoverMessage;

        HoverMessage = "";
        interactionModule.RequestRefresh();

        base.Finish();
    }

    private void UpdateHoverMessage()
    {
        if (dialogueModule.IsPaused)
        {
            HoverMessage = interactionModule.BuildPrompt("Continue Talking");
        }
        else if (!dialogueModule.InConversation)
        {
            HoverMessage = interactionModule.BuildPrompt("Talk");
        }
        else
        {
            HoverMessage = "";
        }

        interactionModule.RequestRefresh();
    }

    private void OnConversationEnded(DialogueSO dialogue)
    {
        if (dialogue == introductionDialogue)
        {
            Finish();
        }
        else
        {
            UpdateHoverMessage();
        }
    }
}
