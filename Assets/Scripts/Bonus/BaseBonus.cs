using UnityEngine;

public abstract class BaseBonus : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] protected GameBonus _config;
    public GameBonus Config => _config;
    protected BoxCollider2D Collider;

    public virtual void Init(GameBonus config)
    {
        _config = config;

        sprite.sprite = Config.sprite;
    }
}
