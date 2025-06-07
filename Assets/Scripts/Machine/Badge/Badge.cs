using UnityEngine;
using UnityEngine.UI;

public class Badge : MonoBehaviour
{
    GameManager _gameManager => GameManager.Instance;
    [SerializeField] private Image bg;
    [SerializeField] private RectTransform progressHP;
    [SerializeField] private RectTransform progressShot;
    [SerializeField] public TMPro.TextMeshProUGUI textName;
    [SerializeField] private Image rankImage;
    private float maxWidth = 260;
    private MachineLevelData machineLevelData;
    private GameRank configRank;

    // void Awake()
    // {
    //     BaseMachine.OnChangeData += OnChangeData;
    // }

    // void OnDestroy()
    // {
    //     BaseMachine.OnChangeData -= OnChangeData;
    // }

    public void OnChangeData(BaseMachine machine)
    {
        var oneProcentHP = maxWidth / machine.Config.hp;
        progressHP.sizeDelta = new Vector2(oneProcentHP * machine.Data.hp, progressHP.sizeDelta.y);

        // var oneProcentShot = maxWidth / machine.Config.Muzzle.timeBetweenShot;
        // progressShot.sizeDelta = new Vector2((machine.Config.Muzzle.timeBetweenShot - machine.Data.timeBeforeShot) * oneProcentShot, progressShot.sizeDelta.y);
    }

    public void Init(MachineLevelData _machineLevelData)
    {
        machineLevelData = _machineLevelData;

        OnSetNameText(machineLevelData.name);

        configRank = _gameManager.Settings.ranks.Find(r => r.name.ToString() == _machineLevelData.rank.ToString());

        rankImage.sprite = configRank.sprite;
    }

    public void OnSetNameText(string text)
    {
        textName.text = text;
    }
}
