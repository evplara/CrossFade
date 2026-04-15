using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scriptable Objects/Interview Q")]
public class InterviewQuestionSO : ScriptableObject
{
    public string question;
    public List<InterviewResponse> responses = new();
}

[System.Serializable]
public class InterviewResponse
{
    public string response;
    [Header("0 being awful, 1 being perfect answer, 0.5 neutral")]
    [Range(0, 1)]
    public float score;
}
