using Animocity;
using BlueprintSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestLoadSound : MonoBehaviour
{
    private void Awake()
    {
        DataLoader.OnDataLoaded += this.LoadSound;
    }

    private void LoadSound(PlayerProfile profile, DataLoader.LoadStatus Status)
    {
        var sound = this.GetComponent<AudioSource>();

        sound.clip = BlueprintDatabase<SoundBlue>.FetchAll().FirstOrDefault().clip;
    }
}
