using UnityEngine;

public class MedicineBonus : BaseBonus
{
    public override void Init(GameBonus config)
    {
        base.Init(config);

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        BaseMachine bm = collision.gameObject.GetComponentInParent<BaseMachine>();
        AreaMove am = collision.gameObject.GetComponent<AreaMove>();
        if (bm && am)
        {
            bm.OnSetHP(bm.Config.hp);

            base.OnDrawText(bm);
            
            Destroy(gameObject);
        }
    }
}
