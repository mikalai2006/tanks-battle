using UnityEngine;

public class SpeedTowerBonus : BaseBonus
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
            bm.OnAddBonus(Config);
            if (!bm.MachineLevelData.isBot)
            {
                bm.LevelManager.UiTopSide.OnCreateUIBonus(Config);
            }
            Destroy(gameObject);
        }
    }
}
