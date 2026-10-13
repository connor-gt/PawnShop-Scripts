using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles phase-specific setup for the player after they load.
/// </summary>
public class PlayerSpawnPositionHandler : MonoBehaviour
{
    private void Awake()
    {
        // The screen should remain black until the player is moved to whatever position they need to be in
        // and is ready to go
        SceneSwapManager.Instance.RegisterToBeAwaited(this);
    }

    /// <summary>
    /// Sets up the player for the current chapter phase.
    /// <para>
    /// During the <see cref="ChapterPhase.Opening"/> and <see cref="ChapterPhase.Night"/> phases, this initializes
    /// and starts the bed wake-up sequence. During the <see cref="ChapterPhase.Day"/> and
    /// <see cref="ChapterPhase.Closing"/> phases, this uses the normal spawn path.
    /// </para>
    /// </summary>
    /// <param name="phase">The current <see cref="ChapterPhase"/>.</param>
    /// <param name="bedSequenceHandler">The bed sequence handler used during phases that begin with the player
    /// waking up from bed.</param>
    public async UniTask SetPlayer(ChapterPhase phase, BedSequenceHandler bedSequenceHandler)
    {
        switch (phase)
        {
            // The player wakes up from the bed at the start of these phases
            case ChapterPhase.Opening:
            case ChapterPhase.Night:
                await WakeUpFromBed(bedSequenceHandler);
                break;
            // The player is already in the shop.
            // TODO: Move player to the position they were in at the start of the phase
            case ChapterPhase.Day:
            case ChapterPhase.Closing:
                // TODO: Method to move player
                SpawnNormally();
                break;
        }
    }

    private async UniTask WakeUpFromBed(BedSequenceHandler bedSequenceHandler)
    {
        await bedSequenceHandler.InitializeWakeUpSequence();

        SceneSwapManager.Instance.NotifySceneReady(this);
        bedSequenceHandler.WakeUpSequence();
    }

    private void SpawnNormally()
    {
        Debug.Log("Defaulting player to spawn point since the phase is Day or Closing.");
        SceneSwapManager.Instance.NotifySceneReady(this);
    }
}
