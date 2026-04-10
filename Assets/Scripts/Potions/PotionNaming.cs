using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrossFade.Potions
{
    // JSON shape for Resources/PotionNaming.json (affixes + suffixes word lists).
    [Serializable]
    public class PotionNamingData
    {
        public string[] affixes = Array.Empty<string>();
        public string[] suffixes = Array.Empty<string>();
    }

    // Loads naming data from Resources and picks random affix/suffix for rolled potions.
    public static class PotionNaming
    {
        private const string ResourcesName = "PotionNaming";

        public static PotionNamingData LoadFromResources()
        {
            var textAsset = Resources.Load<TextAsset>(ResourcesName);
            if (textAsset == null || string.IsNullOrWhiteSpace(textAsset.text))
            {
                return CreateFallback();
            }

            try
            {
                var data = JsonUtility.FromJson<PotionNamingData>(textAsset.text);
                if (data == null || !HasAnyWords(data))
                {
                    return CreateFallback();
                }

                return data;
            }
            catch (Exception)
            {
                return CreateFallback();
            }
        }

        public static string PickRandomAffix(PotionNamingData data)
        {
            return PickRandomFrom(data?.affixes, "Unnamed");
        }

        public static string PickRandomSuffix(PotionNamingData data)
        {
            return PickRandomFrom(data?.suffixes, "Potion");
        }

        private static string PickRandomFrom(string[] words, string fallback)
        {
            if (words == null || words.Length == 0)
            {
                return fallback;
            }

            var list = new List<string>();
            for (var i = 0; i < words.Length; i++)
            {
                var w = words[i];
                if (!string.IsNullOrWhiteSpace(w))
                {
                    list.Add(w.Trim());
                }
            }

            if (list.Count == 0)
            {
                return fallback;
            }

            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        private static bool HasAnyWords(PotionNamingData data)
        {
            if (data.affixes != null)
            {
                for (var i = 0; i < data.affixes.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(data.affixes[i]))
                    {
                        return true;
                    }
                }
            }

            if (data.suffixes != null)
            {
                for (var i = 0; i < data.suffixes.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(data.suffixes[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static PotionNamingData CreateFallback()
        {
            return new PotionNamingData
            {
                affixes = new[] { "Strange", "Wild", "Quiet" },
                suffixes = new[] { "Tonic", "Brew", "Elixir" }
            };
        }
    }
}
