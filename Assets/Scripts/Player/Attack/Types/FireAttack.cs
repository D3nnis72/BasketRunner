using UnityEngine;
public class FireAttack : AttackType
{
    public override void TriggerAttack(SwordVFXManager vfxManager)
    {
        vfxManager.PlayVFX("FireAttack");
    }
}