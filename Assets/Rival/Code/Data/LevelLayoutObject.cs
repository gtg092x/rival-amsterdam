using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Level")]
public class LevelLayoutObject : ScriptableObject
{
    public int Segments;
    public float MissDamage = 0.2f;
    public float BossDamage = 0.1f;

    public GameLevelModel GetModel()
    {
        return new GameLevelModel(Segments, MissDamage, BossDamage);
    }
}
