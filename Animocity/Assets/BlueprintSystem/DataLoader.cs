using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

namespace BlueprintSystem
{
    public class DataLoader : MonoBehaviour
    {
        public List<string> coreNamespaces;
        public List<string> coreModNames;
        public string targetSceneFollowingLoad;
		public string modFolder;

		private static string _ModFolder;

		public TMP_Text displayOutput;

		public delegate void DataLoaderEvent(PlayerProfile profile, DataLoader.LoadStatus Status);
		public static event DataLoaderEvent OnDataLoaded;

		public void HotReaload()
		{
			Clear();
			Setup();
		}

		public void Clear()
		{
			XMLToBlueprint.ClearCoreNamespaces();
			// Possibly more clearing needed?
		}

        // Use this for initialization
        public void Setup () {
			if(coreNamespaces == null || coreNamespaces.Count == 0)
			{
				print("No core namespaces to load!");
				return;
			}
			if (modFolder.Equals(""))
			{
                print("No mod folder specified!");
                return;
            }
			else
			{
                _ModFolder = modFolder;
            }
			foreach(var ns in coreNamespaces)
			{
                AddCoreNamespace(ns);
            }
            print(Assembly.GetExecutingAssembly().FullName);
           
            Initialise(Application.dataPath, Application.persistentDataPath, Assembly.GetExecutingAssembly());
            StartCoroutine(LoadAllMods());
			StartCoroutine(AwaitDataLoad());
		}

        private void OnLoaded()
        {
			OnDataLoaded?.Invoke(PlayerProfile.Current, DataLoader.Status);
			UpdateDisplayString("");
        }

        private void UpdateDisplayString(string text)
		{
            displayString = text;
			if (displayOutput)
			{
				displayOutput.text = displayString;
			}

        }

		private IEnumerator AwaitDataLoad(){
			while(DataLoader.Status != DataLoader.LoadStatus.LOADED) yield return new WaitForSecondsRealtime(0.2f);
			if (targetSceneFollowingLoad != null && (!targetSceneFollowingLoad.Equals("")))
			{
				SceneManager.LoadScene("MainMenu");
			}
			OnLoaded();
        }


    	private static string persistentDataPath;
    	private static string dataPath;

		private static Dictionary<string, AudioClip> audioClips = new Dictionary<string,AudioClip>();
		public static Dictionary<string, AudioClip> AudioClips{
        	get{
        		return audioClips;
        	}
        }

        private static Dictionary<string, ModInfo> mods = new Dictionary<string,ModInfo>();
        public static Dictionary<string, ModInfo> Mods{
        	get{
        		return mods;
        	}
        }

		private static ModInfo currentLoadingMod;
		internal static ModInfo CurrentLoadingMod{
			get{
				return currentLoadingMod;
			}
		}
       	internal static List<Assembly> assemblies = new List<Assembly>();
       	private static List<string> profiles = new List<string>();
       	public static List<string> Profiles{
       		get{
       			return profiles;
       		}
       	}
        private static string displayString = "Loading...";

        public enum LoadStatus {PRELOADING, LOADING, CROSSREFERENCING, LOADED};

        private static LoadStatus status = LoadStatus.PRELOADING;

        public static LoadStatus Status{
        	get{
        		return status;
        	}
        }

		public static string DisplayString{
        	get{
        		return displayString;
        	}
        }

		public static string profilesPath{
        	get{
        		return Path.Combine(persistentDataPath, "Profiles");
        	}
        }
		public static string modsPath{
        	get{
        		return Path.Combine(dataPath, _ModFolder);
        	}
        }
		internal static List<string> AllActiveTexturePaths{
        	get{
        		List<string> paths = new List<string>();
        		foreach(ModInfo mi in mods.Values){
					string texturePath = Path.Combine(mi.Path, "Textures");
					paths.Add(texturePath);
        		}
        		return paths;
        	}
        }
		internal static List<string> AllActiveSoundPaths{
        	get{
        		List<string> paths = new List<string>();
        		foreach(ModInfo mi in mods.Values){
					string soundPath = Path.Combine(mi.Path, "Sounds");
					paths.Add(soundPath);
        		}
        		return paths;
        	}
        }

        /*  Creates list of Mod directories ready for importing
        *
        */
        public void AddCoreNamespace(string namespaceToAdd){
        	XMLToBlueprint.AddCoreNamespace(namespaceToAdd);
        }

        public void Initialise(string setDataPath, string setPersistentDataPath, Assembly coreAssembly){

        	dataPath = setDataPath;
        	persistentDataPath = setPersistentDataPath;
        	assemblies.Add(coreAssembly);

        	InitialiseRequiredDirectories();
        	RegisterAllModInfos();
			RegisterProfiles();
        }

		internal void RegisterAllModInfos(){
        	mods.Clear();

			string[] modslist  = Directory.GetDirectories(modsPath,"*",SearchOption.TopDirectoryOnly);

			foreach(string s in modslist){
				#if DEBUG
				MonoBehaviour.print("Importing mod at "+s);
				#endif
            	if(Directory.Exists(s)){
               		bool errored = false;
                	ModInfo mi = new ModInfo(s, ref errored);
                	if(errored){
						MonoBehaviour.print("Error!");
                	}
                	else{
						#if DEBUG
							MonoBehaviour.print("Now adding mod with label "+mi.label);
						#endif
						mods.Add(mi.label, mi);
                	}
            	}
        	}

        	ModInfo.CurrentMods = new List<ModInfo>();
        	ModInfo.CurrentMods.AddRange(mods.Values.Where((mod)=> coreModNames.Contains(mod.label)));
        }

        public static void RegisterProfiles(){
        	profiles.Clear();

			string[] proList    = Directory.GetDirectories(profilesPath,"*",SearchOption.TopDirectoryOnly);

			foreach(string s in proList){

				string fN = Path.Combine(Path.Combine(profilesPath, s), "ProfileInfo.txt");

				if(File.Exists(fN)){
					string sP = Path.GetFileName(Path.GetDirectoryName( fN ));

					profiles.Add(sP);
				}
			}
        }

		internal IEnumerable<ModInfo> ActiveMods{
        	get{
        		return (from mod in mods.Values
        				where mod.Active
        				select mod);
        	}
        }

		/*  If initialised, loads mods!
        *
        */
		public IEnumerator LoadAllMods(){
			print("Loading");
			status = LoadStatus.LOADING;

			if(ModInfo.CurrentMods==null){
				print("ModInfo class not initialised! Aborting!");
				yield break;
			}
			if(ModInfo.CurrentMods.Count==0){ 
				print("No mods detected! Aborting!");
				yield break;
			}
			foreach(ModInfo mod in mods.Values){
			 
				mod.Active = ModInfo.CurrentMods.Contains(mod);
			}

			foreach(ModInfo m in ModInfo.CurrentMods){
				string mod = m.label;
				if(!mods.ContainsKey(mod)){
					print(string.Format("No mods with key {0} in database", mod));
					continue;
				}
				foreach(var e in ImportModule(mods[mod])){
					#if DEBUG
					displayString = "Importing mod "+mods[mod].label;
					#endif
					yield return 0;
				}
			}
			yield return StartCoroutine(MakeAudioDatabase());
			yield return 0;
			XMLToBlueprint.PopulateAllBlueprintFields();
			yield return 0;
			XMLToBlueprint.TransferAllBlueprintsToDatabases();
			yield return 0;
			XMLToBlueprint.InitialiseAllBlueprints();
			status = LoadStatus.LOADED;
		}

		public IEnumerator MakeAudioDatabase(){
			foreach(string s in AllActiveSoundPaths){
				string[] allFiles = Directory.GetFiles(s,"*.wav",SearchOption.AllDirectories);
				foreach(string file in allFiles){
					string newKey = file.Substring(s.Length+1);
					#if DEBUG
					print("Importing wav at "+newKey);
					#endif

					//MonoBehaviour.print("Importing texture at "+completeTexturePath);
    	 			UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.WAV);

    	 			yield return www.SendWebRequest();

    	 			while(!www.isDone){
    	 				yield return 0;
    	 			}
    	 			AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
					AudioClips.Add(newKey,clip);
				}
			}
		}

		private IEnumerable ImportModule(ModInfo mod){

			#if DEBUG
			print("Importing module "+mod.label);
			#endif
			currentLoadingMod = mod;

			string assemblyPath  = Path.Combine(mod.Path, "Assemblies");
			string blueprintPath = Path.Combine(mod.Path, "Blueprints");
	
			if(Directory.Exists(assemblyPath)){
				string[] files = Directory.GetFiles(assemblyPath);
				foreach(string file in files){
					if(Path.GetExtension(file)==".dll") TryImportAssembly(file);
				}
			}

			if(Directory.Exists(blueprintPath)){
				#if DEBUG
				print("Blueprint path at ::"+blueprintPath);
				#endif
				string[] files = Directory.GetFiles(blueprintPath,"*.xml" ,SearchOption.AllDirectories);
				foreach(string file in files){
					if(Path.GetExtension(file)==".xml"){ 
						UpdateDisplayString($"Loading mod {mod.label} blueprint {file}");
						print(file);
						TryCreateBlueprints(file);
						yield return 0;
					}
				}
			}

			currentLoadingMod = null;
		}

		private bool TryImportAssembly(string path){
			if(File.Exists(path)) return false;

			UpdateDisplayString($"Importing assembly {path}.");


            try{
				Assembly loaded = Assembly.LoadFile(path);
				assemblies.Add(loaded);
			}
			catch(Exception e){
				if(e is FileNotFoundException){
					print("No file at "+path);
				}
				if(e is FileLoadException){
					print("Error loading assembly file at "+path);
				}
				return false;
			}

			return true;
		}

		private bool TryCreateBlueprints(string path){
			if(!File.Exists(path)) return false;

			displayString = path;
			#if DEBUG
			print("Importing "+path);
			#endif
			XmlDocument doc = new XmlDocument();
			doc.Load(path);
			XMLToBlueprint.CreateEmptyBlueprintsFromDocument(doc);

			return true;
		}

        private void InitialiseRequiredDirectories(){
			Directory.CreateDirectory(@profilesPath);
			Directory.CreateDirectory(@modsPath);
        }
    }
}