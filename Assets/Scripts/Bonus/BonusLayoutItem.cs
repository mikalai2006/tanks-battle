using UnityEngine;
using UnityEngine.UI;

public class BonusLayoutItem : MonoBehaviour
{
    [SerializeField] private Image logo;
    [SerializeField] private TMPro.TextMeshProUGUI nameBonus;
    [SerializeField] private RectTransform progressTime;
    private BaseMachine Target;
    private GameBonus Config;
    private float maxWidth = 280f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null && Config != null)
        {
            DataBonus bonusData;
            if (Target.Data.bonuses.TryGetValue(Config.typeBonus, out bonusData))
            {
                var oneProcentTime = maxWidth / Config.time;
                progressTime.sizeDelta = new Vector2(oneProcentTime * bonusData.time, progressTime.sizeDelta.y);
            };
        }
    }

    public void Init(GameBonus config, BaseMachine _target)
    {
        Target = _target;
        Config = config;
        logo.sprite = config.sprite;
        nameBonus.text = config.text.title.GetLocalizedString();

    }
}
