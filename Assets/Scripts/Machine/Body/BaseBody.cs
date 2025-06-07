using UnityEngine;

public class BaseBody : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _bodySprite;
    [SerializeField] private SpriteRenderer _bodyGerbSprite;
    [SerializeField] private SpriteRenderer _damageSprite;
    protected BaseMachine Machine;
    public void Init(BaseMachine _machine)
    {
        Machine = _machine;

        OnChangeData();

        _bodySprite.color = Machine.Config.colorBody;
    }

    public void OnChangeData()
    {
        Color col = Color.white;
        col.a = 1f - Mathf.Min(1f, Machine.Data.hp * 100f / Machine.Config.hp * 0.01f);

        _damageSprite.color = col;
    }

    public void OnSetSpriteGerb(Sprite sprite)
    {
        _bodyGerbSprite.sprite = sprite;
    }
}
