using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Animocity.Utilities
{
    public static class CustomCollectionExtensions
    {
        private static System.Random _random = new System.Random();

        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> data, Func<T, U> map)
        {
            foreach (T t in data)
            {
                yield return map(t);
            }
        }
        public static IEnumerable<double> CumulativeSum(this IEnumerable<double> sequence)
        {
            double sum = 0;
            foreach (var item in sequence)
            {
                sum += item;
                yield return sum;
            }
        }
        public static IEnumerable<double> CumulativeSum<T>(this IEnumerable<T> sequence, Func<T, double> map)
        {
            double sum = 0;
            foreach (var item in sequence)
            {
                sum += map(item);
                yield return sum;
            }
        }
        public static bool WhereAny<T>(this IEnumerable<T> data, Func<T, bool> predicate)
        {
            foreach(T t in data)
            {
                if(predicate(t)) return true;
            }
            return false;
        }

        public static bool WhereAll<T>(this IEnumerable<T> data, Func<T, bool> predicate)
        {
            foreach (T t in data)
            {
                if (!predicate(t)) return false;
            }
            return true;
        }

        public static T WeightedRandom<T>(this IEnumerable<T> opts, Func<T, float> weightFunc)
        {
            var weights = Map(opts, weightFunc);
    
            var totalWeight = weights.Sum();

            var choiceWeight = _random.NextDouble()*totalWeight;

            float cumSum = 0;
            int choiceIndex = 0;
            foreach (float w in weights) 
            {
                cumSum += w;
                if (cumSum >= choiceWeight) break;
                choiceIndex++;
            }

            return opts.ToArray()[choiceIndex];
        }
    }
}
