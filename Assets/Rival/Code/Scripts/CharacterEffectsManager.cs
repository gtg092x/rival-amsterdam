using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefab;

    [SerializeField] private GameManager _gameManager;

    private void OnValidate()
    {
        _gameManager ??= FindAnyObjectByType<GameManager>();
    }
}
