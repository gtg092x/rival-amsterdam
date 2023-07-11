using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Level")]
public class LevelLayoutObject : ScriptableObject
{
    public int HitsPerPlayer;
    public float MissDamage = 0.2f;
    public float BossDamage = 0.1f;

    public GameLevelModel GetModel()
    {
        return new GameLevelModel(HitsPerPlayer, MissDamage, BossDamage);
    }
}
