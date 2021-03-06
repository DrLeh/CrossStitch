﻿using System.Collections.Generic;

namespace CrossStitch.Core.Utility.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            if (dict.ContainsKey(key))
                return dict[key];
            return defaultValue;
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue addValue)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            dict.Add(key, addValue);
            return addValue;
        }
    }
}
