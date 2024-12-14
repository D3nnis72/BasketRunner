using UnityEngine;

public abstract class AttackType : MonoBehaviour
{
    public string Name { get; set; }
    public string AnimationTrigger { get; set; }
    public float ComboTime { get; set; }

    public abstract void TriggerAttack(SwordVFXManager vfxManager);
}



