using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Provides the shared functionality for directing a pawn shop chapter.
/// </summary>
[RequireComponent(typeof(ChapterStateHandler), typeof(CustomerHandler), typeof(PlayerSpawnPositionHandler))]
[RequireComponent(typeof(NightTransitionHandler))]
public abstract class ChapterDirector : MonoBehaviour
{
    public static ChapterDirector Instance { get; private set; }

    /// <summary>
    /// Invoked whenever the <see cref="CurrentPhase"/> gets updated.
    /// </summary>
    public event Action<ChapterPhase> OnChapterPhaseUpdated;

    /// <inheritdoc cref="ChapterStateHandler.CurrentPhase"/>
    public ChapterPhase CurrentPhase => chapterStateHandler.CurrentPhase;

    [Title("Pawn Shop Scene")]
    [Tooltip("The pawn shop scene is additively loaded in on Start() if it isn't already loaded.")]
    [SerializeField]
    private SceneField pawnShopScene;

    protected ChapterStateHandler chapterStateHandler;
    protected CustomerHandler customerHandler;
    protected PlayerSpawnPositionHandler playerSpawnPositionHandler;
    protected NightTransitionHandler nightTransitionHandler;

    private BedSequenceHandler bedSequenceHandler;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            chapterStateHandler = GetComponent<ChapterStateHandler>();
            customerHandler = GetComponent<CustomerHandler>();
            playerSpawnPositionHandler = GetComponent<PlayerSpawnPositionHandler>();
            nightTransitionHandler = GetComponent<NightTransitionHandler>();

            chapterStateHandler.OnChapterPhaseUpdated += OnEnterNewChapterPhase;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // If children need to override, yield return base.Start() should be used
    protected virtual IEnumerator Start()
    {
        SpawnPlayer.OnPlayerLoaded += OnPlayerLoaded;

        if (!SceneHandler.IsSceneLoaded(pawnShopScene))
        {
            yield return SceneHandler.LoadSceneAsync(pawnShopScene, additive: true, setActive: false);
        }

        while (PawnShop.Instance == null)
        {
            yield return null;
        }

        PawnShop.Instance.OnOpen += OnPawnShopOpened;

        // Quick and dirty way but I'd rather not make BedSequenceHandler a singleton
        bedSequenceHandler = FindAnyObjectByType<BedSequenceHandler>();
        bedSequenceHandler.OnPlayerAwake += OnPlayerAwake;
        bedSequenceHandler.OnPlayerAsleep += OnPlayerAsleep;
    }

    protected virtual void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        Instance = null;
        SpawnPlayer.OnPlayerLoaded -= OnPlayerLoaded;

        if (PawnShop.Instance != null)
        {
            PawnShop.Instance.OnOpen -= OnPawnShopOpened;
        }

        if (bedSequenceHandler != null)
        {
            bedSequenceHandler.OnPlayerAwake -= OnPlayerAwake;
            bedSequenceHandler.OnPlayerAsleep -= OnPlayerAsleep;
        }

        chapterStateHandler.OnChapterPhaseUpdated -= OnEnterNewChapterPhase;
    }

    /// <summary>
    /// When the player scene loads, handle moving the player to the appropriate location.
    /// </summary>
    /// <param name="player">The player game object that's loaded in.</param>
    protected virtual void OnPlayerLoaded(GameObject player)
    {
        playerSpawnPositionHandler.SetPlayer(CurrentPhase, bedSequenceHandler).Forget();
    }

    /// <summary>
    /// Called when the pawn shop is opened and begins the active day sequence.
    /// <para>
    /// This starts the day timer and allows customers to begin entering the shop.
    /// </para>
    /// </summary>
    protected virtual void OnPawnShopOpened()
    {
        chapterStateHandler.BeginDaySequence();
        customerHandler.SpawnCustomers(true);
    }

    /// <summary>
    /// Called when the <see cref="ChapterStateHandler.CurrentPhase"/> is updated and invokes
    /// <see cref="OnChapterPhaseUpdated"/>.
    /// <para>
    /// If the new phase is <see cref="ChapterPhase.Closing"/>, this will stop customers from spawning and
    /// instructs existing customers whose brains use <see cref="IExitRequester"/> to finish what they are
    /// doing and leave the shop.
    /// </para>
    /// </summary>
    private void OnEnterNewChapterPhase(ChapterPhase newPhase)
    {
        if (newPhase == ChapterPhase.Closing)
        {
            customerHandler.EndCustomerService();
        }

        OnChapterPhaseUpdated?.Invoke(newPhase);
    }

    /// <summary>
    /// Called after the player has finished waking up from bed.
    /// </summary>
    protected virtual void OnPlayerAwake()
    {
        
    }

    /// <summary>
    /// Called after the player has closed their eyes in bed.
    /// </summary>
    protected virtual void OnPlayerAsleep()
    {
        if (CurrentPhase == ChapterPhase.Closing)
        {
            StartCoroutine(CompleteDay());
        }
    }

    private IEnumerator CompleteDay()
    {
        yield return nightTransitionHandler.SetToNight();

        // For testing, just wait like 5 seconds
        yield return new WaitForSecondsRealtime(5f);

        Debug.Log("Transition complete");
        bedSequenceHandler.WakeUpSequence();
    }
}
