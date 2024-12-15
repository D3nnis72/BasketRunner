using UnityEngine;
public class IceAttack : AttackType
{
    public override void TriggerAttack(SwordVFXManager vfxManager)
    {
        vfxManager.PlayVFX("IceAttack");
    }

}
