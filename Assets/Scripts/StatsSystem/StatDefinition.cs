[System.Serializable]
public class StatDefinition
{
    public StatId id;
    public StatType type;

    // Used when type == Numeric
    public float baseValue;

    // Used when type == Bool
    public bool defaultBool;
}
