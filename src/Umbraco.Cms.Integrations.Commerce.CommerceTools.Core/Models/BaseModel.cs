using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models
{
    public abstract class BaseModel
    {
        /// <summary>
        /// Returns the <typeparamref name="TValue"/> value from <paramref name="dictionary"/> that corresponds to <paramref name="key"/>.
        /// If no value exist for <paramref name="key"/> in <paramref name="dictionary"/>, or if <paramref name="dictionary"/> is null, returns the default value of <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected static TValue SafelyGetDictionaryValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                return default;
            }
            return dictionary.TryGetValue(key, out var value) ? value : default;
        }
    }
}
