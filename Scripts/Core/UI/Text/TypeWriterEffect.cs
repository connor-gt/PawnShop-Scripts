using System;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine;

public class TypeWriterEffect : MonoBehaviour
{
    /// <summary>
    /// Invoked when the typewriter effect has finished revealing all characters.
    /// </summary>
    public event Action OnFinished;

    [Title("Text")]
    [SerializeField]
    private TextMeshProUGUI textBox;

    private int currentVisibleCharIndex;
    private Coroutine typeWriterRoutine;

    /// <summary>
    /// Gets whether or not the typewriter effect is currently running.
    /// </summary>
    public bool InProgress => typeWriterRoutine != null;

    // To avoid creating new WaitForSeconds objects in coroutines
    private WaitForSeconds charDelay;
    private WaitForSeconds puncDelay;

    [Title("Settings")]
    [Tooltip("How many characters should appear per second during the typewriter effect.")]
    [SerializeField]
    [Min(0f)]
    private float charactersPerSecond = 20f;

    [Tooltip("Extra delay applied after punctuation characters (.,?!).")]
    [SerializeField]
    [Min(0f)]
    private float interpunctuationDelay = 0.25f;

    private void Awake()
    {
        charDelay = new WaitForSeconds(1f / charactersPerSecond);
        puncDelay = new WaitForSeconds(interpunctuationDelay);
    }

    /// <summary>
    /// Begins the typewriter effect using the provided text.
    /// <para>
    /// If an effect is already running, it will be stopped and restarted.
    /// </para>
    /// </summary>
    /// <param name="text">The text to reveal character by character.</param>
    public void BeginEffect(string text)
    {
        if (typeWriterRoutine != null)
        {
            StopCoroutine(typeWriterRoutine);
        }

        textBox.maxVisibleCharacters = 0;
        currentVisibleCharIndex = 0;
        textBox.text = text;

        typeWriterRoutine = StartCoroutine(TypeWriterRoutine());
    }

    /// <summary>
    /// Coroutine that reveals the text one character at a time by applying a delay after revealing each
    /// character and applies an extra delay after punctuation characters.
    /// </summary>
    private IEnumerator TypeWriterRoutine()
    {
        TMP_TextInfo textInfo = textBox.textInfo;

        // 1 is subtracted to avoid the delay after the very last character (especially for punctuation)
        while (currentVisibleCharIndex < textInfo.characterCount - 1)
        {
            char character = textInfo.characterInfo[currentVisibleCharIndex].character;
            textBox.maxVisibleCharacters++;
            
            if (character == '?' || character == '.' || character == ',' || character == '!')
            {
                yield return puncDelay;
            }
            else
            {
                yield return charDelay;
            }

            currentVisibleCharIndex++;
        }

        Finish();
    }

    /// <summary>
    /// If the typewriter effect is active, this instantly completes the typewriter effect which reveals all
    /// characters and invokes the <see cref="OnFinished"/> event.
    /// </summary>
    public void Finish()
    {
        if (InProgress)
        {
            StopCoroutine(typeWriterRoutine);
            textBox.maxVisibleCharacters = textBox.textInfo.characterCount;
            typeWriterRoutine = null;
            OnFinished?.Invoke();
        }
    }
}
