using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BlueprintSystem
{
    public static class BlueprintDatabase<T> where T : Blueprint, new()
    {

        //private static List<T> blueprintList = new List<T> ();
        private static Dictionary<string, T> blueprintDictionary = new Dictionary<string, T> ();

        public static T Fetch(string label){
            T t;
            if(TryFetch(label, out t)) return t;
            return null;
        }

        public static bool TryFetch(string label, out T t){
            return blueprintDictionary.TryGetValue(label, out t);
        }
       
        public static IEnumerable<T> FetchAll(){
            return blueprintDictionary.Values;
        }

		public static void CreateDictionary(IEnumerable<T> newBlueprints){
			blueprintDictionary.Clear();
			foreach(T blue in newBlueprints){
				blueprintDictionary.Add(blue.label, blue);
			}
		}

        public static IEnumerable<T> FetchAllWhere(Predicate<T> match){
            return  (from val in blueprintDictionary.Values
                    where match(val)
                    select val);
        }

        public static void RemoveAllExcept(IEnumerable<T> exceptions){

			#if DEBUG
			MonoBehaviour.print(string.Format("# blueprints of type {0} before clearing :: {1}.",typeof(T).Name, blueprintDictionary.Count));
			#endif
        	HashSet<string> exceptionLabels = new HashSet<string>(from T blue in exceptions select blue.label);
        	IEnumerable<string> removeLabels = (from string key in blueprintDictionary.Keys
        										where !exceptionLabels.Contains(key)
        										select key);

        	foreach(string key in removeLabels){
        		blueprintDictionary.Remove(key);
        	}

			#if DEBUG
			MonoBehaviour.print(string.Format("# blueprints of type {0} after clearing :: {1}.",typeof(T).Name, blueprintDictionary.Count));
			#endif
        }
    }
}



