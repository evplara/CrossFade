using System.Linq;
using UnityEngine;

public class ResetSingletonsManager : MonoBehaviour
{
    private void Start()
    {
        var singletons = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IResetable>();

        foreach (var r in singletons)
        {
            r.OnReset();
        }
    }
}
