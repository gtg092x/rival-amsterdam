public class GameLevelModel
{

    public GameLevelModel(int turnsPerPlayer, float missDamage, float hitDamage)
    {
        _turnsPerPlayer = turnsPerPlayer;
        _missDamage = missDamage;
        _hitDamage = hitDamage;
    }
    
    private int _turnsPerPlayer;
    private readonly float _missDamage;
    private readonly float _hitDamage;

    public int GetDefaultHitsPerPlayer()
    {
        return _turnsPerPlayer;
    }
    
    public void DecrementSegment()
    {
        _turnsPerPlayer--;
    }

    public bool IsCompleted()
    {
        return _turnsPerPlayer <= 0;
    }

    public float GetMissDamage()
    {
        return _missDamage;
    }
    
    public float GetHitDamage()
    {
        return _hitDamage;
    }
}