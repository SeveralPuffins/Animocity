using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace BlueprintSystem
{
	[System.Serializable]
	public class PlayerProfile
	{
	    public string   profilePath;
	    public string   profileName;

    	public string[] modules;

		private static PlayerProfile cur;
	
		public static PlayerProfile Current{
    		get{
    			return cur;
    		}
    	}

	    public static bool TryLoadFromJSON(string path, out PlayerProfile profile){
			var profilePath = Path.Combine(path, "ProfileInfo.txt");
			if (File.Exists(profilePath))
			{
				string jsonText = File.ReadAllText(profilePath);
				cur = JsonUtility.FromJson<PlayerProfile>(jsonText);
				profile = cur;
				return true;
			}
			else
			{
				profile = null;
				return false;
			}
	    }

	    public static void SaveCurrent(){
	        string jsonProfile = JsonUtility.ToJson(Current);
	        File.WriteAllText(Path.Combine(Current.profilePath, "ProfileInfo.txt"), jsonProfile);
			MonoBehaviour.print(string.Format("Saving profile {0} to path {1}.", Current.profileName, Current.profilePath));
	    }

	    public void AddModule(string moduleName){
	    	List<string> temp = this.modules.ToList();
	    	temp.Add(moduleName);
	    	this.modules = temp.ToArray();
	    }

		public void RemoveModule(string moduleName){
			List<string> temp = this.modules.ToList();
	    	if(temp.Contains(moduleName)) temp.Remove(moduleName);
	    	this.modules = temp.ToArray();
	    }

		public static PlayerProfile GetDefault()
		{
            string profileName = "DEFAULT_USER";
            string profilePath = Path.Combine(DataLoader.profilesPath, profileName);

            if (TryLoadFromJSON(profilePath, out var user))
			{
				return user;
			}
			else
			{
				return MakeNewProfile(profileName);
			}
        }

	    public static PlayerProfile MakeNewProfile(string newProfileName){

	    	PlayerProfile newProfile = new PlayerProfile();

	    	newProfile.modules 			= new string[] {ModInfo.CoreLabel};

	    	newProfile.profileName = newProfileName;
	    	newProfile.profilePath = Path.Combine(DataLoader.profilesPath,newProfileName);

			Directory.CreateDirectory(newProfile.profilePath);
			Directory.CreateDirectory(Path.Combine(newProfile.profilePath, "Saves"));

	    	cur = newProfile;
	    	SaveCurrent();
	    	DataLoader.RegisterProfiles();
	    	return newProfile;
	    }
	}
}

