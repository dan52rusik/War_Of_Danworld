public class Castle : BattleEntity
{
    protected override void Start()
    {
        base.Start();
        _gameManager.OnGameStart.AddListener(InitializeCastle);
    }

    private void InitializeCastle()
    {
        currentHealth = maxHealth;
    }
}