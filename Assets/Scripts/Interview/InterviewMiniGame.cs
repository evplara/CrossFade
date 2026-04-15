using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InterviewMiniGame : MonoBehaviour
{
    [SerializeField] private List<InterviewQuestionSO> interviewQuestions;
    [SerializeField] private float timeToReadQuestion = 2.5f;
    [SerializeField] private float timeToAnswer = 3f;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;
    [Range(0, 1)]
    [SerializeField] private float distortion;
    private float timeRemaining;
    private int currentIndex;
    private int questionsAnswered;

    private bool active;
    private float score;

    private void Start()
    {
        ApplyPotionContextFromSession();
        StartMiniGame();
    }

    // Reads <see cref="PlayerPotionStats"/> (persists via DontDestroyOnLoad on that component's GameObject) and applies VFX hooks + minigame tuning.
    void ApplyPotionContextFromSession()
    {
        PotionEffectVfxHooks.ApplyAllFromPlayerStats();

        var stats = PlayerPotionStats.Instance;
        if (stats != null)
        {
            float d = PotionEffectVfxHooks.ComputeInterviewDistortion(stats);
            if (d > 0f)
            {
                distortion = Mathf.Clamp01(Mathf.Max(distortion, d));
            }

            float timeMul = PotionEffectVfxHooks.ComputeInterviewTimeMultiplier(stats);
            timeToAnswer *= timeMul;
            Debug.Log("Time to answer: " + timeToAnswer);
        }
    }

    void Update()
    {
        if (!active) return;

        timeRemaining -= Time.deltaTime;
        timeText.text = $"Time Remaining: {timeRemaining:F2}";

        if (timeRemaining <= 0)
        {
            SubmitAnswer(null);
        }
    }

    public void SetMiniGameParameters(float timeToAnswer, float distortion)
    {
        this.timeToAnswer = timeToAnswer;
        this.distortion = distortion;
    }

    public void StartMiniGame()
    {
        currentIndex = 0;
        questionsAnswered = 0;
        score = 0;
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (currentIndex >= interviewQuestions.Count)
        {
            EndMiniGame();
            return;
        }

        var q = interviewQuestions[currentIndex];

        questionText.text = ApplyDistortion(q.question);

        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        Invoke(nameof(ShowQuestions), 2.5f);
    }

    void ShowQuestions()
    {
        var q = interviewQuestions[currentIndex];

        foreach (InterviewResponse r in q.responses)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);

            button.GetComponentInChildren<TextMeshProUGUI>().text = ApplyDistortion(r.response);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                SubmitAnswer(r);
            });
        }

        StartTimer();
    }

    
    string ApplyDistortion(string input)
    {
        if (distortion >= 1f) return input;

        string[] words = input.Split(' ');

        //for testing: more forgiving scale
        float chance = Mathf.Pow(distortion, 0.5f);

        for (int i = 0; i < words.Length; i++)
        {
            char[] chars = words[i].ToCharArray();

            for (int j = 0; j < chars.Length; j++)
            {
                float letterKeepChance = chance;

                if (Random.value > letterKeepChance)
                {
                    chars[j] = '*'; 
                }
            }

            words[i] = new string(chars);
        }

        return string.Join(" ", words);
    }

    public void SubmitAnswer(InterviewResponse response)
    {
        foreach (Transform child in buttonContainer)
        {
            child.GetComponent<Button>().interactable = false;
        }

        score += response != null ? response.score : 0f;

        timeText.text = "";
        questionsAnswered++;
        currentIndex++;
        active = false;

        Invoke(nameof(ShowQuestion), 0.5f);
    }

    void StartTimer()
    {
        timeRemaining = timeToAnswer;
        active = true;
    }

    private void EndMiniGame()
    {
        float finalScore = questionsAnswered > 0 ? score / questionsAnswered : 0f;
        
        Debug.Log("Interview Complete. Final Score: " + finalScore);
        resultText.gameObject.SetActive(true);

        bool playerWon = false;
        if (finalScore > 0.75f)
        {
            playerWon = true;
            resultText.text = "Congrats you passed.";
        }else
        {
            resultText.text = "You failed.";
        }

        if (playerWon)
        {
            int moneyEarnedAmount = 100;

            if (finalScore > 0.95f)
            {
                moneyEarnedAmount = 750;
            }else if (finalScore > 0.9f)
            {
                moneyEarnedAmount = 600;
            }else if (finalScore > 0.85f)
            {
                moneyEarnedAmount = 400;
            }
            else if (finalScore > 0.8f)
            {
                moneyEarnedAmount = 250;
            }

            PlayerPotionStats.Instance.ChangeMoney(moneyEarnedAmount);
        }

        StartCoroutine(ExitScene());
    }

    private IEnumerator ExitScene()
    {
        yield return new WaitForSeconds(1.5f);

        HandleSceneManager.instance.LoadPotionScene();
    }
}
