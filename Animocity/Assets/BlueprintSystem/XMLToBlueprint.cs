using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Reflection;
using UnityEngine;

namespace BlueprintSystem
{
	internal static class XMLToBlueprint
	{

		const string listItemID  = "li";
		const string dictItemID  = "di";
		const string dictKeyID   = "dk";
		const string dictValueID = "dv";
		private static List<string> coreNamespaces = new List<string>();

		internal static List<Type> blueprintTypes = new List<Type>();

		/* 
		* Called during loading. Checks that the node is a blueprint,
		* and if it is passes it to a generic dictionary, where the blueprint is either given 
		* a reference to the node, or is created and THEN given the reference. 
		*/

		internal static void AddCoreNamespace(string ns){
			coreNamespaces.Add(ns);
		}

        internal static void ClearCoreNamespaces()
        {
            coreNamespaces.Clear();
        }

        internal static void CreateEmptyBlueprintsFromDocument(XmlDocument doc){

			foreach(XmlNode node in doc.DocumentElement.ChildNodes){
				if(node.NodeType==XmlNodeType.Comment || node.NodeType==XmlNodeType.CDATA) continue;
				XmlAttribute att = node.Attributes["label"];
				if(att==null) continue;

				string label = att.Value;
				if(label==null) continue;

				string typeName = node.Name;
				Type type = ParseType(typeName);

				if(type==null){
					MonoBehaviour.print(string.Format("Could not find type of {0}", typeName));
					continue;
				}
				if(!(type.IsSubclassOf(typeof(Blueprint)) || type.IsAssignableFrom(typeof(Blueprint)))){
					MonoBehaviour.print(string.Format("Type {0} is not a blueprint! Stopping import", typeName));
					continue;
				}
				if(!blueprintTypes.Contains(type)) blueprintTypes.Add(type);


				MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("AddNodeToInitBlueprintDictionary");
				MethodInfo generic 		= method.MakeGenericMethod(type);

				generic.Invoke(null, new object[]{node, label});

			}
		}

		public static void InitialiseAllBlueprints(){

			foreach(Type type in blueprintTypes){

				MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("InitialiseBlueprints");
				MethodInfo generic 		= method.MakeGenericMethod(type);
				generic.Invoke(null, new object[]{});

			}
		}

		public static void InitialiseBlueprints<T>() where T : Blueprint, new()
		{
			foreach( Blueprint blue in BlueprintDatabase<T>.FetchAll()){
				blue.Init();
			}
		}

		public static void PopulateAllBlueprintFields(){

			foreach(Type type in blueprintTypes){

				MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("FillBlueprintFields");
				MethodInfo generic 		= method.MakeGenericMethod(type);
				generic.Invoke(null, new object[]{});

			}
		}

		public static void FillBlueprintFields<T>() where T : Blueprint, new()
		{
			BlueprintFiller<T>.FillInitBlueprints();
		}

		public static void TransferAllBlueprintsToDatabases(){
			foreach(Type type in blueprintTypes){
				MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("TransferBlueprintsToDatabase");
				MethodInfo generic 		= method.MakeGenericMethod(type);
				generic.Invoke(null, new object[]{});
			}
		}

		public static void TransferBlueprintsToDatabase<T>() where T : Blueprint, new()
		{
			BlueprintFiller<T>.TransferBlueprintsAndClearCrossrefDictionary();
		}

		public static T FindReferenceBlueprint<T>(string s) where T : Blueprint, new(){

			return BlueprintFiller<T>.GetBlueprintByLabel(s);
		}

		public static void AddNodeToInitBlueprintDictionary<T>(XmlNode node, String label) where T: Blueprint, new() 
		{
			BlueprintFiller<T>.AddXmlRef(label, node);
		}

		private static Type ParseType(string name){
			Type type = null;
			type = Type.GetType(name);
			if(type!=null) return type;

			type = Type.GetType("System."+name);
			if(type!=null) return type;

			foreach(Assembly a in DataLoader.assemblies){
				//MonoBehaviour.print(string.Format("Checking naively in assembly {0}...", a.GetName()));
				//MonoBehaviour.print(string.Format("Checking Assembly {0} for type {1}", a.GetName(), name));
				a.GetType(name);
				if(type!=null) return type;
			}

			foreach(Assembly a in DataLoader.assemblies){
				foreach(string s in coreNamespaces){
					//MonoBehaviour.print(string.Format("Checking in namespace {0} :: {1}.", a.GetName(), s));
					//MonoBehaviour.print(string.Format("Checking Assembly {0} for type {1}.{2}", a.GetName(), s, name));
					type = a.GetType(string.Format("{0}.{1}", s, name));
					if(type!=null) return type;
				}
			}

			foreach(Assembly a in DataLoader.assemblies){
				//MonoBehaviour.print(string.Format("Checking Assembly via:: GetType({0}, {1})", name,a.GetName()));
				type = Type.GetType(string.Format("{0}, {1}", name, a.GetName()));
				if(type!=null) break;	
			}
				
			foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies()){
				#if DEBUG
				MonoBehaviour.print(string.Format("Searching appdomain assembly {0}...", a.GetName()));
				#endif
				type = Type.GetType(string.Format("{0}, {1}", name, a.GetName()));
				if(type!=null) break;		
			}
			return type;
		}

		public static T ParseObjectFields<T>(T obj, XmlNode node){
			if(obj==null) return default(T);
			IEnumerable<FieldInfo> fields = (from field in typeof(T).GetFields()
					where field.MemberType == MemberTypes.Field && field.IsPublic
					select field);

			foreach(XmlNode child in node){

 				string childName = child.Name;

				IEnumerable<FieldInfo> matchingFields = (from f in fields				// Take the last field
 											where f.Name==childName
 											select f);

 				if(matchingFields.Count()==0){ 
					Console.WriteLine(string.Format("Could not find field in type {0} named {1}.", typeof(T).Name, childName));
 					continue;
 				}
 				FieldInfo field = matchingFields.Reverse().First();

 				if(field==null){ 
 					Console.WriteLine(string.Format("Could not find field in type {0} named {1}.", typeof(T).Name, childName));
 					continue;
 				}
				object current = field.GetValue(obj);
				var val = XMLToBlueprint.ParseField(field, child, current);

				field.SetValue(obj, val);

				//Yggdrasil.Log.Message(string.Format("Field {0} assigned value of {1} in {2}.", field.Name, field.GetValue(obj), obj));
 			}
 			return obj;
 		}


		public static object ParseField(FieldInfo field, XmlNode node, object current = null){

			Type fieldType = field.FieldType;

			if(field.Name=="label"){ 
				Console.WriteLine("Parser was passed field named label, which should be filled from attribute");
				return null;
			}

			// Check for alternative derived class to parse...
			if(node.Attributes!=null && node.Attributes["Class"]!=null && node.Attributes["Class"].Value!=null){
				#if DEBUG
				MonoBehaviour.print("Class specified for field type "+fieldType.Name);
				#endif
				Type newType = ParseType(node.Attributes["Class"].Value);
				if(newType!=null && newType.IsSubclassOf(fieldType)){
					fieldType = newType;
					#if DEBUG
					MonoBehaviour.print("New parsing as type "+fieldType.Name);
					#endif
				}
			}

			return ParseByType(fieldType, node, current);
		}

		public static object ParseByType(Type type, XmlNode node, object obj = null){

			//MonoBehaviour.print(string.Format("Now parsing field {0} of type {1}", node.Name, type.Name));

			if(node.Attributes!=null && node.Attributes["Class"]!=null && node.Attributes["Class"].Value!=null){
				#if DEBUG
				MonoBehaviour.print("Class specified for field type "+type.Name);
				#endif
				Type newType = ParseType(node.Attributes["Class"].Value);
				if(newType!=null && newType.IsSubclassOf(type)){
					type = newType;
					#if DEBUG
					MonoBehaviour.print("New parsing as type "+type.Name);
					#endif
				}
			}


			string text = "";
			bool overwrite = node.Attributes!=null && node.Attributes["new"]!=null && node.Attributes["new"].Value!=null && node.Attributes["new"].Value=="true";
			//List<XmlNode> children = new List<XmlNode>();

			for(int i=0; i<node.ChildNodes.Count; i++){
				XmlNode child = node.ChildNodes[i];
				if(child.NodeType==XmlNodeType.Text) text+=child.Value;

				//if(child.NodeType==XmlNodeType.Element) children.Add(child);
			}

			if(type==typeof(string)){
				return text;
			}

			else if(type==typeof(Vector2)){
				return ParseVector2(text);
			}
			else if(type==typeof(Vector3)){
				return ParseVector3(text);
			}
			else if(type==typeof(Vector4)){
				return ParseVector4(text);
			}
            else if (type == typeof(Vector2Int))
            {
                return ParseVector2Int(text);
            }
            else if (type == typeof(Vector3Int))
            {
                return ParseVector3Int(text);
            }
            else if(type==typeof(Color)){
				return ParseColor(text);
			}

			else if(type==typeof(Texture2D)){
     			Texture2D tex = null;
     			byte[] fileData;
 				foreach(string pathToTextureFolder in DataLoader.AllActiveTexturePaths){
					string completeTexturePath = Path.Combine(pathToTextureFolder, text);

					if (File.Exists(completeTexturePath))
    	 			{
    	 				//MonoBehaviour.print("Importing texture at "+completeTexturePath);
						fileData = File.ReadAllBytes(completeTexturePath);
         				tex = new Texture2D(2, 2);
						tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
         				return tex;
     				}
     				else{
     					MonoBehaviour.print("No texture at "+completeTexturePath);
     				}
				}
				return new Texture2D(10,10);
			}

			else if(type==typeof(AudioClip)){

				if(DataLoader.AudioClips.ContainsKey(text)){
					return DataLoader.AudioClips[text];
				}
				else{
					return null;
				}
			/*
				MonoBehaviour.print("Importing audio");
 				foreach(string pathToSoundFolder in DataLoader.AllActiveSoundPaths){

					string completeSoundPath = Path.Combine(pathToSoundFolder, text);

					if (File.Exists(completeSoundPath))
    	 			{
    	 				//MonoBehaviour.print("Importing texture at "+completeTexturePath);
    	 				WWW path = new WWW("file://"+completeSoundPath);
    	 				AudioClip clip = WWWAudioExtensions.GetAudioClip(path);

         				return clip;
     				}
     				else{
     					MonoBehaviour.print("No sound clip at "+completeSoundPath);
     				}
				}
				return null;*/
			}

			else if(type.IsArray){
				Type innerType = type.GetElementType();
				XmlNodeList children = node.SelectNodes(listItemID);
				if(obj==null || overwrite){
					Array newArray = Array.CreateInstance(innerType, children.Count);

					for(int i=0; i<children.Count; i++){
						newArray.SetValue(ParseByType(innerType, children[i]), i);
					}
					return newArray;
				}
				else{
					Array oldArray = obj as Array;
					int oldLength = oldArray.GetLength(0);

					Array newArray = Array.CreateInstance(innerType, children.Count+oldLength);
					for(int i=0; i<oldArray.GetLength(0); i++){
						newArray.SetValue(oldArray.GetValue(i), i);
					}
					for(int i=oldLength; i<oldLength+children.Count; i++){
						newArray.SetValue(ParseByType(innerType, children[i]), i);
					}
					return newArray;
				}
			}

			// If it's a blueprint, find it in the blueprintfiller of the same type!
			else if(type.IsSubclassOf(typeof(Blueprint)) || type.IsAssignableFrom(typeof(Blueprint))){
				MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("FindReferenceBlueprint");
				MethodInfo generic 		= method.MakeGenericMethod(type);
				var value = generic.Invoke(null, new object[]{text});
				return value;
			}

			// If it's a generic type, create a new object and iterate.
 			else if(type.IsGenericType){
 				Type[] typeArgs = type.GetGenericArguments();
 						
 				if(type.GetGenericTypeDefinition()==typeof(List<>)){

					MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("ParseAsList");
					MethodInfo generic 		= method.MakeGenericMethod(typeArgs);

  					var list = generic.Invoke(null, new object[]{node});

					if(obj==null || (obj as List<object>) == null 
					|| (obj as List<object>).Count==0 || overwrite) return list;
  					else{
  						List<object> oldList = obj  as List<object>;
  						List<object> newList = list as List<object>; 
  						oldList.AddRange(newList);

  						MethodInfo cast = typeof(XMLToBlueprint).GetMethod("CastListAs");
  						MethodInfo castGeneric = cast.MakeGenericMethod(typeArgs);

						return castGeneric.Invoke(null, new object[]{oldList});
  					}
 				}
				else if(type.GetGenericTypeDefinition()==typeof(Dictionary<,>)){
					if(obj==null || overwrite){ 
						MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("ParseAsDictionary");
						MethodInfo generic 		= method.MakeGenericMethod(typeArgs);
						return generic.Invoke(null, new object[]{node});
					}
					else{
						MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("AddToDictionary");
						MethodInfo generic 		= method.MakeGenericMethod(typeArgs);
						return generic.Invoke(null, new object[]{node, obj});
					}
 				}
 				else return null;
 			}
			// If it's a type, find the type within all imported assemblies and assign. 
			// Troublesome if two assemblies use the same name! Here we take the first that does.
			else if(type==typeof(Type)){

				Type t2 = ParseType(text);

				if(t2==null){ 
					//Yggdrasil.Log.Message(string.Format("No such type when parsing {0}.", text));
				}
				return t2;
			}
			// If it's an enum, check that it's a defined value before calling parse, or we'll end up with an unexpected enum value.
 			else if(type.IsEnum){
 					
				if(!Enum.IsDefined(type, text)){
					Console.WriteLine(string.Format("Enum of type {0} cannot have value {1}", type.ToString(), text));
				} 
				var value = Enum.Parse(type, text);
	 			return value;
 			}
	 		else{
	 			//For other types we must consider inheritance. 
	 			//we may want a subtype, but have been offered a base type.
	 			//For example, we may have requested an IEnumerable, but want to 
	 			//automatically make a list.
	 			//In that case, we first switch the variable "type" to the subtype.
				XmlAttribute sub = node.Attributes["subclass"];
				Type subType = null;
				if(sub!=null && sub.Value!=null){
					subType = ParseType(sub.Value);
					if(subType!=null && type.IsAssignableFrom(subType)){
						type = subType;
					}
	 			}

				//Else let us try to parse it with its own defined (static) parse function.
			    //Note that this works with all non-string primitives.
				MethodInfo mi = type.GetMethod("Parse", new Type[] { typeof(string) });
 				if(mi!=null){
					return mi.Invoke(null, new object[]{text});
	 			}

	 			// Else see if we can populate a new struct or class instance directly
 				else{
					// This should be a struct
					if(type.IsValueType && !type.IsPrimitive){
						MonoBehaviour.print(string.Format("Type {0} is a struct", type.Name));
						object newStruct = obj;
						if(obj==null || overwrite) newStruct = Activator.CreateInstance(type);
						MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("ParseObjectFields");
						MethodInfo generic 		= method.MakeGenericMethod(type);
						newStruct = generic.Invoke(null, new object[]{newStruct, node});
						#if DEBUG
						MonoBehaviour.print("Finished parsing struct  ::"+newStruct);
						#endif
						return newStruct;
					}
					else{
						// Field is an object of some class- make one and fill it if it has a null constructor.

						var constructor = type.GetConstructor(Type.EmptyTypes);
						if(constructor!=null){
							//MonoBehaviour.print(string.Format("Type {0} is a class", type.Name));
							object newObj = obj;
							if(obj==null || overwrite) newObj = Activator.CreateInstance(type);
							MethodInfo method 		= typeof(XMLToBlueprint).GetMethod("ParseObjectFields");
							MethodInfo generic 		= method.MakeGenericMethod(type);
							generic.Invoke(null, new object[]{newObj, node});
							//MonoBehaviour.print("Finished parsing struct  ::"+newObj);
							return newObj;
						}
						else return null;
					}
 				}
			}
		}

		public static List<T> CastListAs<T>(IEnumerable<object> items){
				return items.Cast<T>().ToList();
		}


		public static T CastToType<T>(object obj){
			return (T) obj;
		}

		public static List<T> ParseAsList<T>(XmlNode node){

			List<T> list = new List<T>();

			XmlNodeList children = node.SelectNodes(listItemID);

			foreach(XmlNode item in children){
				T value = (T) ParseByType(typeof(T), item);
				list.Add(value);	
			}

			return list;
		}

		private static float[] ParseVectorN(string sIn){

			
			string[] strings = sIn.Split(',');
			float[] parsed = new float[strings.Length];
			for(int i=0; i<strings.Length; i++){
				string s = strings[i];
				s.Replace(" ", "");
				if(!float.TryParse(s, out parsed[i])) return null;
			}
			return parsed;
		}
        private static int[] ParseVectorNInt(string sIn)
        {


            string[] strings = sIn.Split(',');
            int[] parsed = new int[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                string s = strings[i];
                s.Replace(" ", "");
                if (!int.TryParse(s, out parsed[i])) return null;
            }
            return parsed;
        }

        public static Vector2 ParseVector2(string sIn){

			float[] parsed = ParseVectorN(sIn);
			if(parsed==null || parsed.Length!=2) return default(Vector2);

			return new Vector2(parsed[0], parsed[1]);
		}

        public static Vector2Int ParseVector2Int(string sIn)
        {

            int[] parsed = ParseVectorNInt(sIn);
            if (parsed == null || parsed.Length != 2) return default(Vector2Int);

            return new Vector2Int(parsed[0], parsed[1]);
        }

        public static Vector3 ParseVector3(string sIn){

			float[] parsed = ParseVectorN(sIn);
			if(parsed==null || parsed.Length!=3) return default(Vector3);

			return new Vector3(parsed[0], parsed[1], parsed[2]);
		}
        public static Vector3Int ParseVector3Int(string sIn)
        {

            int[] parsed = ParseVectorNInt(sIn);
            if (parsed == null || parsed.Length != 3) return default(Vector3Int);

            return new Vector3Int(parsed[0], parsed[1],parsed[3]);
        }

        public static Vector4 ParseVector4(string sIn){

			float[] parsed = ParseVectorN(sIn);
			if(parsed==null || parsed.Length!=3) return default(Vector4);

			return new Vector4(parsed[0], parsed[1], parsed[2], parsed[3]);
		}

		public static Color ParseColor(string sIn){

			float[] parsed = ParseVectorN(sIn);
			//MonoBehaviour.print(string.Format("parsed:: {0}:{1}:{2}", parsed[0],parsed[1],parsed[2]));
			Color c = default(Color);
			if(parsed.Length==3) c = new Color(parsed[0],parsed[1],parsed[2]);
			if(parsed.Length==4) c = new Color(parsed[0],parsed[1],parsed[2], parsed[3]);
			return c;
		}

		public static Dictionary<T1, T2> ParseAsDictionary<T1, T2>(XmlNode node){
			//MonoBehaviour.print(string.Format("parsing node {0} as Dict<{1},{2}>",node.Name,typeof(T1).Name,typeof(T2).Name));
			Dictionary<T1, T2> dict = new Dictionary<T1, T2>();
			XmlNodeList children = node.SelectNodes(dictItemID);
			foreach(XmlNode item in children){
				XmlNode key = item.SelectSingleNode(dictKeyID);
				XmlNode val = item.SelectSingleNode(dictValueID);

				//MonoBehaviour.print("Parsing child node...");

				if(key==null || val==null) continue;

				//MonoBehaviour.print(string.Format("Adding pair from text mapping {0}->{1}", key.InnerText, val.InnerText));

				T1 keyObj = (T1) ParseByType(typeof(T1), key);
				T2 valObj = (T2) ParseByType(typeof(T2), val);
			
				if(keyObj!=null){ 
					if(typeof(T1).IsAssignableFrom(typeof(Blueprint)) || typeof(T1).IsSubclassOf(typeof(Blueprint))){

						//Blueprint print = keyObj as Blueprint;

						//MonoBehaviour.print("Blueprint name:: "+print.DisplayName);	
						//MonoBehaviour.print("Blueprint label:: "+print.label);	
					}
				}
				/*if(valObj!=null){ 
					Yggdrasil.Log.Message("Value:: "+valObj.ToString());	
				}*/

				if(keyObj!=null && valObj!=null){
					dict.Add(keyObj, valObj);
				}
			}

			return dict;
		}

		public static Dictionary<T1, T2> AddToDictionary<T1, T2>(XmlNode node, Dictionary<T1, T2> dict){

			XmlNodeList children = node.SelectNodes(dictItemID);
			foreach(XmlNode item in children){
				XmlNode key = item.SelectSingleNode(dictKeyID);
				XmlNode val = item.SelectSingleNode(dictValueID);

				if(key==null || val==null) continue;

				T1 keyObj = (T1) ParseByType(typeof(T1), key);
				T2 valObj = (T2) ParseByType(typeof(T2), val);
				if(keyObj!=null && valObj!=null){
					dict.Add(keyObj, valObj);
				}
			}

			return dict;
		}
	}
}

