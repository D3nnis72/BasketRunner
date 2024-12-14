using UnityEngine;
public class WaterAttack : AttackType
{
    public override void TriggerAttack(SwordVFXManager vfxManager)
    {
        vfxManager.PlayVFX("WaterAttack"); // Use a tag or identifier for lightning VFX
    }
}
