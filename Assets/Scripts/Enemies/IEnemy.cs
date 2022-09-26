using UnityEngine;
public interface IEnemy
{
    EnemyState state { get; }
    Vector3? moveDestination { get; }
    float radius { get; }
    int score { get; }
    int power { get; }

    void Init();
    void ReactToShot();
    void ReactToKick(Vector3 lastDestination);
}
