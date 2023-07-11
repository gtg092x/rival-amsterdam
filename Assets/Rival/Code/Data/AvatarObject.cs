using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Avatar")]
public class AvatarObject : ScriptableObject
{
    public GameObject Prefab;
    public string AvatarName;
    public string BoneFocus = "RigPelvis";
    public AudioClip[] Taunts;
    public AudioClip[] Ouches;
    public AudioClip[] Hits;
    public AudioClip LoseSound;
    public AnimationClip Ouch;
    public AnimationClip Die;
    public AnimationClip Hit;
    public AnimationClip Idle;
    public AnimationClip Taunt;
}
