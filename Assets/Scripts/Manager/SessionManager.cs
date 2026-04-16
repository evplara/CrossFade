using UnityEngine;

public class SessionManager : MonoBehaviour
{
    [System.Serializable] 
    public struct MoneyRange
    {
        public int minMoney;
        public int maxMoney;
    }

    [SerializeField] private MoneyRange healthMoney;
    [SerializeField] private MoneyRange timeMoney;
    [SerializeField] private MoneyRange effectMoney;

    public static SessionManager Instance { get; private set; }

    private float effectValue;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnDisable()
    {
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.PlayerDied -= PlayerDies;
        }

        if (SessionTimer.Instance != null)
        {
            SessionTimer.Instance.SessionOver -= SessionEnds;
        }
    }

    private void Start()
    {
        HealthManager.Instance.PlayerDied += PlayerDies;
        SessionTimer.Instance.SessionOver += SessionEnds;
    }

    private void PlayerDies()
    {
        SessionTimer.Instance.StopSession();
    }

    //get moeny and go back to potion scene
    private void SessionEnds()
    {
        MinigameTimer.Instance.StopTimer();
        HandleMoney();
        HandleSceneManager.instance.LoadPotionScene();
    }

    //get your money reward: based on health, time, and effect value
    private void HandleMoney()
    {
        float healthScore = (float)HealthManager.Instance.CurrentHealth / HealthManager.Instance.MaxHealth;
        float timeScore = SessionTimer.Instance.ElapsedSeconds / SessionTimer.Instance.MaxRoundTime;
        float effectScore = effectValue / PlayerPotionStats.Instance.MaxEffectTotal;

        Debug.Log("Health score: " + healthScore);
        Debug.Log("Time score: " + timeScore);
        Debug.Log("Effect score: " + effectScore);

        int totalMoney = 0;

        totalMoney += (int)Mathf.Lerp(healthMoney.minMoney, healthMoney.maxMoney, healthScore);
        totalMoney += (int)Mathf.Lerp(timeMoney.minMoney, timeMoney.maxMoney, timeScore);
        totalMoney += (int)Mathf.Lerp(effectMoney.minMoney, effectMoney.maxMoney, effectScore);


        MoneyManager.Instance.ChangeMoney(totalMoney);
    }

    public void SetHighScore(float value)
    {
        effectValue = value;
    }

    
}
