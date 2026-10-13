using System;
using UnityEngine.InputSystem;

/// <summary>
/// Creates the standard temporary input bindings used by sell-item counter interactions.
/// </summary>
public class NegotiationInputFactory
{
    private readonly INegotiationDialogueProvider negotiationDialogueProvider;

    /// <summary>
    /// Creates a helper that builds the standard input bindings for the supplied dialogue provider.
    /// </summary>
    /// <param name="negotiationDialogueProvider">The provider that provides the dialogue choices and input
    /// actions used during sell-item negotiations.</param>
    public NegotiationInputFactory(INegotiationDialogueProvider negotiationDialogueProvider)
    {
        this.negotiationDialogueProvider = negotiationDialogueProvider;
    }
    
    /// <summary>
    /// Creates a temporary input binding that listens for the standard sell-item negotiation inputs and invokes
    /// the supplied callbacks when the corresponding input actions are performed.
    /// </summary>
    /// <param name="onAccept">Callback to invoke when the accept offer input is received.</param>
    /// <param name="onDecline">Callback to invoke when the decline offer input is received.</param>
    /// <param name="onLower">Callback to invoke when the lower offer input is received.</param>
    /// <param name="onRaise">Callback to invoke when the raise offer input is received.</param>
    /// <param name="onSubmit">Callback to invoke when the submit offer input is received.</param>
    /// <returns>A <see cref="TemporaryInputBinding"/> for handling sell-item negotiation inputs.</returns>
    public TemporaryInputBinding CreateNegotiationInputBinding(
        Action<InputAction.CallbackContext> onAccept,
        Action<InputAction.CallbackContext> onDecline,
        Action<InputAction.CallbackContext> onLower,
        Action<InputAction.CallbackContext> onRaise,
        Action<InputAction.CallbackContext> onSubmit)
    {
        InputAction[] offerInputs =
        {
            negotiationDialogueProvider.Choices.Accept.InputAction,
            negotiationDialogueProvider.Choices.Decline.InputAction,
            negotiationDialogueProvider.Choices.Lower.InputAction,
            negotiationDialogueProvider.Choices.Raise.InputAction,
            negotiationDialogueProvider.Choices.Submit.InputAction
        };

        Action<InputAction.CallbackContext>[] offerActions =
        {
            onAccept,
            onDecline,
            onLower,
            onRaise,
            onSubmit
        };

        return new TemporaryInputBinding(offerInputs, offerActions);
    }
}
