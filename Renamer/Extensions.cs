using System;
using System.Collections.Generic;

namespace Renamer
{
    static class Extensions
    {
        /// <summary>
        /// Execute action inside a dictionary when the key is present. When key is not present do nothing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict">Dictionary</param>
        /// <param name="key">key to look up action</param>
        /// <returns>true when action was found, false otherwise</returns>
        public static bool OnValue<T>(this Dictionary<T, Action> dict, T key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Action acc;
            if (dict.TryGetValue(key, out acc))
            {
                acc();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Execute action inside a dictionary with given parameter. When key is not present do nothing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict">Dictionary</param>
        /// <param name="key">key to look up action in dictionary.</param>
        /// <param name="parameter">Passed parameter to action.</param>
        /// <returns>true if action was found. false when parameter was null. null when no action for given key was found.</returns>
        /// <exception cref="ArgumentNullException">When key is null.</exception>
        /// <remarks>When the parameter is null the action will NOT be called since we expect some data to work with. 
        /// Besides this it makes error handline in the actions easier when they can rely on a non null input argument.</remarks>
        public static bool? OnValueAndParameterNotNull<T>(this Dictionary<T, Action<T>> dict, T key, T parameter)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Action<T> acc = null;
            if (dict.TryGetValue(key, out acc))
            {
                if (parameter == null)
                {
                    return false;
                }

                acc(parameter);
                return true;
            }

            return null;
        }
    }
}
