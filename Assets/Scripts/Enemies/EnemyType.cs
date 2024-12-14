using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyType", menuName = "Enemy/EnemyType")]
public class EnemyType : ScriptableObject
{
    public string enemyName;
    public Color enemyColor;
    public string effectOnHit;
    public string[] canDestroyedBy;
}
