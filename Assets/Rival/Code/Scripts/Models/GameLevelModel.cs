public class GameLevelModel
{

    public GameLevelModel(int segmentsPerPlayer, float missDamage, float hitDamage)
    {
        _segmentsPerPlayer = segmentsPerPlayer;
        _missDamage = missDamage;
        _hitDamage = hitDamage;
    }
    
    private int _segmentsPerPlayer;
    private readonly float _missDamage;
    private readonly float _hitDamage;

    public int GetSegmentsRemaining()
    {
        return _segmentsPerPlayer;
    }
    
    public void DecrementSegment()
    {
        _segmentsPerPlayer--;
    }

    public bool IsCompleted()
    {
        return _segmentsPerPlayer <= 0;
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