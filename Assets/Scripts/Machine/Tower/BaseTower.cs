using UnityEngine;
using UnityEngine.UI;

public class BaseTower : MonoBehaviour
{
    protected GameManager _gameManager = GameManager.Instance;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Image _spriteSector;
    [SerializeField] private RectTransform _rectSector;
    [SerializeField] private SpriteRenderer _damageSprite;
    protected BaseMachine Machine;
    protected float distanceAttack;
    public float DistanceAttack => distanceAttack;

    public void Init(BaseMachine _machine)
    {
        Machine = _machine;

        OnChangeData();

        _sprite.color = Machine.Config.colorTower;

        distanceAttack = Machine.Config.distanceAttack;

        OnSetSizeSector(distanceAttack);
    }

    public void OnChangeData()
    {
        Color col = Color.white;
        col.a = 1f - Mathf.Min(1f, Machine.Data.hp * 100f / Machine.Config.hp * 0.01f);

        _damageSprite.color = col;
    }

    public void OnSetAngleSector(float angle)
    {
        // fillAmount: 1 - 360град.
        // fillAmount: x - angle
        // fillAmount: x = 1 * angle / 360.
        // rectTransform = -(fillAmount * 360) / 2
        var fillAmount = angle / 360;
        _spriteSector.fillAmount = fillAmount;
        _rectSector.localEulerAngles = new Vector3(_rectSector.localEulerAngles.x,_rectSector.localEulerAngles.y, -fillAmount * 360 / 2);
    }

    public void OnSetColorSector(Color color)
    {
        _spriteSector.color = color;
    }

    public void OnSetSizeSector(float size)
    {
        _rectSector.sizeDelta = Vector2.Lerp(_rectSector.sizeDelta, new Vector2(size * 2, size * 2), _gameManager.Settings.speedChangeAreaSize * Time.deltaTime);
    
        // _rectSector.sizeDelta = new Vector2(size, size);
    }

    void Update()
    {
        if (Machine && Machine.Data.angleTower != Machine.Data.currentAngleTower)
        {
            Machine.OnSetCurrentAngleTower(transform.localEulerAngles.z);
        }

        // проверяем наличие бонуса дистанции атаки.
        DataBonus bonusDistanceAttack = null;
        Machine.Data.bonuses.TryGetValue(TypeBonus.DistanceAttack, out bonusDistanceAttack);
        if (bonusDistanceAttack != null)
        {
            distanceAttack = Machine.Config.distanceAttack + bonusDistanceAttack.value;
            OnSetSizeSector(distanceAttack);
        }
        else
        {
            distanceAttack = Machine.Config.distanceAttack;
            OnSetSizeSector(distanceAttack);
        }


        // изменяем угол сектора и его размер.
        if (Machine.ObjectTarget != null && (!Machine.MachineLevelData.isBot || !Machine.ObjectTarget.MachineLevelData.isBot))
        {
            OnSetAngleSector(Mathf.Max(5, Mathf.Abs(Mathf.DeltaAngle(Machine.Data.angleTower, Machine.Data.currentAngleTower))));
        }
    }
}
