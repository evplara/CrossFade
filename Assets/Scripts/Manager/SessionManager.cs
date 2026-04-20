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

    private int interviewMoneyEarned;
    //money from interview questions
    //how much damage you took that round
    private float effectValue;
    private int damageTakenThisRound;
    private bool gameOver;
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
            HealthManager.Instance.HealthChanged -= PlayerTakesDamage;
        }

        if (SessionTimer.Instance != null)
        {
            SessionTimer.Instance.SessionOver -= SessionEnds;
        }
    }

    private void Start()
    {
        HealthManager.Instance.PlayerDied += PlayerDies;
        HealthManager.Instance.HealthChanged += PlayerTakesDamage;
        SessionTimer.Instance.SessionOver += SessionEnds;
    }

    private void PlayerTakesDamage(int current, int max)
    {
        damageTakenThisRound += 1;
    }

    private void PlayerDies()
    {
        gameOver = true;
        SessionTimer.Instance.StopSession();  
    }

    //get moeny and go back to potion scene
    private void SessionEnds()
    {
        MinigameTimer.Instance.StopTimer();

        if (gameOver)
        {
            interviewMoneyEarned = 0;
            damageTakenThisRound = 0;
            HandleSceneManager.instance.LoadGameOverScene();
            return;
        }

        HandleMoney();
        HandleSceneManager.instance.LoadPotionScene();
    }

    public void InterviewMoney(int money)
    {
        interviewMoneyEarned += money;
    }

    //get your money reward: based on health, time, and effect value
    private void HandleMoney()
    {
        float healthScore = (float)(HealthManager.Instance.MaxHealth - damageTakenThisRound)/ HealthManager.Instance.MaxHealth;
        float timeScore = SessionTimer.Instance.ElapsedSeconds / SessionTimer.Instance.MaxRoundTime;
        float effectScore = effectValue / PlayerPotionStats.Instance.MaxEffectTotal;

        Debug.Log("Health score: " + healthScore + ", Damage taken this round: " + damageTakenThisRound);
        Debug.Log("Time score: " + timeScore);
        Debug.Log("Effect score: " + effectScore);

        int totalMoney = 0;

        totalMoney += (int)Mathf.Lerp(healthMoney.minMoney, healthMoney.maxMoney, healthScore);
        totalMoney += (int)Mathf.Lerp(timeMoney.minMoney, timeMoney.maxMoney, timeScore);
        totalMoney += (int)Mathf.Lerp(effectMoney.minMoney, effectMoney.maxMoney, effectScore);
        totalMoney += interviewMoneyEarned;

        MoneyManager.Instance.ChangeMoney(totalMoney);
        interviewMoneyEarned = 0;
        damageTakenThisRound = 0;
    }

    public void SetHighScore(float value)
    {
        effectValue = value;
    }

    
}
