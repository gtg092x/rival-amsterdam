using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerSessionModel
{
    public class PlayerEntry
    {
        public AvatarObject Avatar;
        public int Index;
        public bool IsActive;
    }

    public event Action<PlayerEntry> OnPlayerRemove;
    public event Action<PlayerEntry> OnPlayerAdd;
    private PlayerEntry[] _players;
    
    private AvatarObject _boss;
    public void SetBossAvatar(AvatarObject bossAvatar)
    {
        _boss = bossAvatar;
    }

    public AvatarObject BossAvatar => _boss; 
    public PlayerSessionModel()
    {
        _players = new PlayerEntry[] { };
    }

    public void AddPlayer(AvatarObject avatar)
    {
        var count = _players.Length;
        var entry = new PlayerEntry
        {
            Avatar = avatar,
            Index = count,
            IsActive = true
        };
        _players = _players.Append(entry).ToArray();
        OnPlayerAdd?.Invoke(entry);
    }

    public PlayerEntry[] GetPlayers()
    {
        return _players;
    }
    
    public void RemovePlayer(int index)
    {
        _players[index].IsActive = false;
        OnPlayerRemove?.Invoke(_players[index]);
    }

    public int GetPlayerCount()
    {
        return _players.Length;
    }

    public PlayerEntry GetPlayer(int currentPlayerIndex)
    {
        return _players[currentPlayerIndex];
    }
    
    int GetNextActivePlayerIndex(int currentPlayerIndex)
    {
        for (int i = currentPlayerIndex + 1; i < _players.Length; i++)
        {
            if (_players[i].IsActive)
            {
                return i;
            }
        }
        
        for (int i = 0; i < currentPlayerIndex; i++)
        {
            if (_players[i].IsActive)
            {
                return i;
            }
        }

        throw new Exception("No active players");
    }


}
