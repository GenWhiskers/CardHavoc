using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject playerHUDPrefab;

    private GameObject currentHUD;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: persist between scenes
    }

    public void SpawnHUD()
    {
        if (currentHUD != null) return;

        currentHUD = Instantiate(playerHUDPrefab);
    }

    public void DestroyHUD()
    {
        if (currentHUD != null)
            Destroy(currentHUD);
    }
}