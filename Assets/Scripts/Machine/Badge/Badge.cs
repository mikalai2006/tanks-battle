using UnityEngine;
using UnityEngine.UI;

public class Badge : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private RectTransform progressHP;
    [SerializeField] private RectTransform progressShot;
    [SerializeField] public TMPro.TextMeshProUGUI textName;
    [SerializeField] private Image rankImage;
    private float maxWidth = 260;

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

    public void OnSetNameText(string text)
    {
        textName.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
