using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PawnShop : MonoBehaviour
{
    public static PawnShop Instance { get; private set; }

    public event Action OnOpen;
    public event Action OnClose;

    public bool IsOpen { get; private set; }

    private GameObject _player = null;
    public GameObject Player
    {
        get
        {
            if (_player == null && SpawnPlayer.Instance != null && SpawnPlayer.Instance.Player != null)
            {
                _player = SpawnPlayer.Instance.Player;
            }

            return _player;
        }
    }

    [field: Title("Shelves")]
    [field: Required]
    [field: SerializeField]
    public ShelfController ShelfController { get; private set; }

    [field: Title("Entities")]
    [field: Required]
    [field: SerializeField]
    public EntityController EntityController { get; private set; }

    [field: Title("Tasks")]
    [field: Required]
    [field: SerializeField]
    public TaskController TaskController { get; private set; }

    [field: Title("UI")]
    [field: SerializeField]
    public ItemInfoPopup ItemInfoPopup { get; private set; }

    /// <summary>
    /// Gets the counter that NPCs should go to when they are ready to purchase items from the shop.
    /// </summary>
    [field: Title("Counters")]
    [field: Tooltip("The counter NPCs go to to purchase items from the shop.")]
    [field: Required]
    [field: SerializeField]
    public Counter PurchaseCounter { get; private set; }

    /// <summary>
    /// Gets the counter that NPCs should go to when they want to sell items to the player.
    /// </summary>
    [field: Tooltip("The counter NPCs go to to sell items to the player to potentially be sold in the shop.")]
    [field: Required]
    [field: SerializeField]
    public Counter SellCounter { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SpawnPlayer.OnPlayerLoaded += OnPlayerLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (ItemInfoPopup != null && ItemInfoPopup.IsActive)
        {
            ItemInfoPopup.Hide();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SpawnPlayer.OnPlayerLoaded -= OnPlayerLoaded;
            Instance = null;
        }
    }

    private void OnPlayerLoaded(GameObject player)
    {
        // Player and gameplay UI scenes have finished loading, theoretically no more objects need to register
        SceneSwapManager.Instance.CompleteSceneRegistration();
    }

    public void OpenShop()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;
        OnOpen?.Invoke();
    }

    public void CloseShop()
    {
        if (!IsOpen)
        {
            return;
        }

        IsOpen = false;
        OnClose?.Invoke();
    }
}
