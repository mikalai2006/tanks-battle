using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBonus : MonoBehaviour
{
    protected GameManager _gameManager = GameManager.Instance;
    [SerializeField] protected SpriteRenderer sprite;
    [SerializeField] protected GameBonus _config;
    public GameBonus Config => _config;
    protected BoxCollider2D Collider;

    public virtual void Init(GameBonus config)
    {
        _config = config;

        sprite.sprite = Config.sprite;
        sprite.color = Config.color;
    }

    public virtual void OnDrawText(BaseMachine bm)
    {
        // Создаем текст о поднятии бонуса.
        TextDamage obText = Lean.Pool.LeanPool.Spawn(_gameManager.Settings.prefabTextDamage, bm.LevelManager.objectSpawnText.transform);
        
        if (obText)
        {
            obText.Init(bm, false);
            obText.OnSetColor(Config.color);
            if (Config.value > 0)
            {
                obText.OnSetText(string.Concat(
                    Config.text.title.GetLocalizedString(),
                    Helpers.GetLocalizedPluralString("addbonus",
                    new Dictionary<string, object> {
                        {"value",  Config.value.ToString()},
                    }
                    )
                    )
                );
            }
            else if (Config.value == 0)
            {
                obText.OnSetText(string.Concat(
                    Config.text.title.GetLocalizedString(),
                    Helpers.GetLocaledString("fullBonusValue")
                )
                );
                
            }
        }
    }
}
