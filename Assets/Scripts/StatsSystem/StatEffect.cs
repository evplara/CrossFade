using System.Collections.Generic;

[System.Serializable]
public class StatEffect
{
    // Label for debugging and UI (e.g. "Nausea Brew")
    public string sourceLabel;

    public List<StatModifier> modifiers = new List<StatModifier>();
}
