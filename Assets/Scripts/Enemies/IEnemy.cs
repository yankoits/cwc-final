public interface IEnemy
{
    EnemyState state { get; }

    void ReactToShot();
}
