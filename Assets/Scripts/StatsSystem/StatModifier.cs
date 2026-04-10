[System.Serializable]
public class StatModifier
{
    public StatId target;
    public ModifierType modifierType;

    // Used when modifierType == Numeric
    public float flatDelta;

    // Used when modifierType == Bool
    public bool boolValue;
}
