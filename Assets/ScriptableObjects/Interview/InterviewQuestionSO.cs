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
    public int damage;
}
