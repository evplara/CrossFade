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
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;
    [Range(0, 1)]
    [SerializeField] private float distortion;
    private float timeRemaining;
    private int currentIndex;

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
        Shuffle(interviewQuestions);
        currentIndex = 0;
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

        Invoke(nameof(ShowQuestions), 0f);
    }

    void ShowQuestions()
    {
        var q = interviewQuestions[currentIndex];

        List<InterviewResponse> shuffledResponses = new List<InterviewResponse>(q.responses);
        Shuffle(shuffledResponses);

        foreach (InterviewResponse r in shuffledResponses)
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

        //TODO: REmove X health

        timeText.text = "";
        currentIndex++;
        active = false;

        Invoke(nameof(ShowQuestion), 0f);
    }

    void StartTimer()
    {
        timeRemaining = timeToAnswer;
        active = true;
    }

    private void EndMiniGame()
    {
        StartCoroutine(ExitScene());
    }

    //if end early
    private IEnumerator ExitScene()
    {
        yield return new WaitForSeconds(1.5f);

        HandleSceneManager.instance.LoadRandomMiniGameScene();
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
