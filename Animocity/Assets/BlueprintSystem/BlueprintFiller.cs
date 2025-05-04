 using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using UnityEngine;


namespace BlueprintSystem
{
	internal static class BlueprintFiller<T> where T : Blueprint, new()
	{
		private static Dictionary<T, List<XmlNode>> xmlRefs = new Dictionary<T, List<XmlNode>> ();

		private static T FetchOrCreateBlueprintByLabel(string label){
        	T blue = GetBlueprintByLabel(label);
        	if(blue!=null) return blue;
        	else{
        		blue = new T();
        		blue.label = label;
        		return blue;
        	} 
        }

        public static T GetBlueprintByLabel(string label){

			IEnumerable<T> hits = (from key in xmlRefs.Keys 
        	where key.label==label
        	select key);

        	if(hits.Count()>0) 	return hits.FirstOrDefault();
        	else return null;
        }

        public static T GetParentBlueprint(XmlNode node){
			if(node.Attributes==null) return null;
			if(node.Attributes["parent"]==null) return null;

			string parentLabel = node.Attributes["parent"].Value;

			foreach(T blue in xmlRefs.Keys){
				if(blue.label==parentLabel){
					return blue;
				}
			}
			return null;
        }

		public static void FillInitBlueprints(){
			foreach(T key in xmlRefs.Keys){

				//Yggdrasil.Log.Message(string.Format("Filling fields of {0} with label {1}", key.GetType().Name, key.label));

				foreach(XmlNode node in xmlRefs[key]){

					// Skip any abstract nodes- they do not need entering.
					// May wish to check for keys that only have abstract nodes, but this would be a major user error.
					if(node.Attributes["abstract"]!=null && node.Attributes["abstract"].Value=="true") continue;

					// First parse parent node, if any...
					T parentBlue = GetParentBlueprint(node);
					if(parentBlue!=null){
						foreach(XmlNode parentNode in xmlRefs[parentBlue]){ 
							XMLToBlueprint.ParseObjectFields(key, parentNode);
						}
					}

					XMLToBlueprint.ParseObjectFields<T>(key,node);
					/*IEnumerable<FieldInfo> fields = (from field in typeof(T).GetFields()
					where field.MemberType == MemberTypes.Field && field.IsPublic
					select field);

 					foreach(XmlNode child in node){
 				
 						string childName = child.Name;
						FieldInfo field = (from f in fields				// Take the last field
 								    where f.Name==childName
 								    select f).Reverse().First();

 						if(field==null){ 
 							Yggdrasil.Log.Message(string.Format("Could not find field in type {0} named {1}.", typeof(T).Name, childName));
 							continue;
 						}
						var val = XMLToBlueprint.ParseField(field, child);
						field.SetValue(key, val);

						Yggdrasil.Log.Message(string.Format("Field {0} assigned value of {1} in {2} {3}.", field.Name, field.GetValue(key), typeof(T).Name, key.label));
 					}*/
				}

				//DebugReadFieldsFrom(key);
			}
		}

		public static void TransferBlueprintsAndClearCrossrefDictionary(){

			// First remove all refs that ONLY have abstract entries- frequently parent nodes
			List<T> removeKeys = new List<T>();
			foreach(T key in xmlRefs.Keys){
				if(xmlRefs[key].All((node)=>node.Attributes["abstract"]!=null && node.Attributes["abstract"].Value=="true")){
					removeKeys.Add(key);
				}
			}
			foreach(T key in removeKeys){
				xmlRefs.Remove(key);
			}

			//Then, pass to blueprint dictionary.
			BlueprintDatabase<T>.CreateDictionary(xmlRefs.Keys);
			xmlRefs.Clear();
		}

		private static void DebugReadFieldsFrom(object o){
			Type t = o.GetType();

			#if DEBUG
				MonoBehaviour.print(string.Format("Reading fields from object of type {0}", t.Name));
			#endif	
			foreach(FieldInfo fi in t.GetFields()){
				if(fi.GetValue(o)!=null && fi.GetValue(o).ToString()!=null){
					#if DEBUG
						MonoBehaviour.print(string.Format("Field {0} of type {1} has value {2}", fi.Name,fi.FieldType.ToString(), fi.GetValue(o).ToString()));
					#endif
				}
			}
		}

        public static void AddXmlRef(string label, XmlNode node){

			T blue = FetchOrCreateBlueprintByLabel(label);
			if(blue.ParentMod==null) blue.ParentMod = DataLoader.CurrentLoadingMod;

        	if(!xmlRefs.ContainsKey(blue)){
        		xmlRefs.Add(blue, new List<XmlNode>());
        	}
	        xmlRefs[blue].Add(node);
	    }
	
	}
}

