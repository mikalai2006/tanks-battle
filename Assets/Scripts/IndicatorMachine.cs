using UnityEngine;

public class IndicatorMachine : MonoBehaviour
{
    private GameManager _gameManager => GameManager.Instance;
    private BaseMachine Machine;
    [SerializeField] private GameObject Indicator;
    [SerializeField] private BaseMachine Target;
    [SerializeField] private SpriteRenderer bg;
    [SerializeField] private SpriteRenderer progressHP;
    [SerializeField] private float startSize;
    [SerializeField] private float startScale;
    [SerializeField] private float distance;
    [SerializeField] private float oneProcentScale;

    void Start()
    {
        // Фиксируем начальный размер прогрессбара.
        startSize = progressHP.size.y;

        // Фиксируем начальный масштаб маркера.
        startScale = transform.localScale.x;

        // рассчитываем масштаб для одной единицы сетки игрового мира.
        oneProcentScale = startScale / _gameManager.LevelConfig.gridSize.x;

        OnInit();
    }

    void Update()
    {
        if (!Target || !Machine || Target == Machine)
        {
            return;
        }

        Vector2 direction = Target.transform.position - Machine.transform.position;

        RaycastHit2D ray = Physics2D.Raycast(Machine.transform.position, direction, float.PositiveInfinity, 1 << 6);

        if (ray.collider != null)
        {
            Indicator.transform.position = ray.point;
        }

        distance = Vector3.Distance(Target.transform.position, Machine.transform.position);
        transform.localScale = new Vector3(startScale - oneProcentScale * distance, startScale - oneProcentScale * distance, 1);
    }

    public void OnInit()
    {
        progressHP.color = _gameManager.Settings.colorMarkerProgress;
        bg.color = _gameManager.Settings.colorMarkerBg;
    }

    public void OnChangeData()
    {
        var oneProcentHP = startSize / Machine.Config.hp;

        progressHP.size = new Vector2(progressHP.size.x, Mathf.Min(1, oneProcentHP * Machine.Data.hp));
    }

    public void OnSetMachine(BaseMachine bm)
    {
        Machine = bm;
    }

    public void OnSetTarget(BaseMachine bm)
    {
        Target = bm;
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
