using UnityEngine;
public class FireAttack : AttackType
{
    public override void TriggerAttack(SwordVFXManager vfxManager)
    {
        vfxManager.PlayVFX("FireAttack"); // Use a tag or identifier for fire VFX
    }
}