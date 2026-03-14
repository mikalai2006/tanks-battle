using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorMachine : MonoBehaviour
{
    private GameManager _gameManager => GameManager.Instance;
    private BaseMachine Machine;
    [SerializeField] private GameObject Indicator;
    public BaseMachine Target;
    [SerializeField]private RectTransform rectTransform;
    [SerializeField] private Image bg;
    [SerializeField] private SpriteRenderer progressHP;
    [SerializeField] private float startSize;
    [SerializeField] private float startScale;
    [SerializeField] private float distance;
    [SerializeField] private float oneProcentScale;
    [SerializeField] private Vector3 offset;
    [SerializeField] private TMPro.TextMeshProUGUI text;
    private CancellationTokenSource cancelTokenSource;
    // private bool isRunningCoroutine;
    // Camera _camera;

    void Awake()
    {
        cancelTokenSource = new CancellationTokenSource();
        bg = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // // Фиксируем начальный размер прогрессбара.
        // startSize = progressHP.size.y;

        // // Фиксируем начальный масштаб маркера.
        // startScale = transform.localScale.x;

        // // рассчитываем масштаб для одной единицы сетки игрового мира.
        // oneProcentScale = startScale / _gameManager.LevelConfig.levelData.size.x;

        OnInit();
    }
    private void OnDestroy()
    {
        if (!cancelTokenSource.Token.IsCancellationRequested)
        {
        cancelTokenSource.Cancel();
        cancelTokenSource.Dispose();
        }
    }

    void Update()
    {
        if (!Target || !Machine) //  || Target == Machine
        {
            return;
        }

        // Vector2 direction = Target.transform.position - Machine.transform.position;

        // RaycastHit2D ray = Physics2D.Raycast(Machine.transform.position, direction, float.PositiveInfinity, 1 << 6);

        // if (ray.collider != null)
        // {
        //     Indicator.transform.position = ray.point;
        // }

        // distance = Vector3.Distance(Target.transform.position, Machine.transform.position);
        // transform.localScale = new Vector3(startScale - oneProcentScale * distance, startScale - oneProcentScale * distance, 1);

        float minX = bg.GetPixelAdjustedRect().width / 2.5f;
        float maxX = Screen.width - minX;

        float minY = bg.GetPixelAdjustedRect().height / 2.5f;
        float maxY = Screen.height - minY;

        Vector2 pos = Target.LevelManager.Camera.WorldToScreenPoint(Machine.transform.position + offset);

        if (Vector3.Dot((Machine.transform.position - Target.transform.position), Target.Towers[0].transform.forward) < 0)
        {
            // if (pos.x < Screen.width / 2)
            // {
            //     pos.x = maxX;
            // } else
            // {
            //     pos.x = minX;
            // }
            pos.y = minY;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rectTransform.position = pos;

    }


    async UniTask Refresh(CancellationToken token)
    {

        while(!token.IsCancellationRequested) {

        text.text = ((int)(Vector3.Distance(Machine.transform.position, Target.transform.position) / (_gameManager.Settings.scaleObjects * 8))).ToString();
    
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.05f), cancellationToken: token);
    }

    // public void SetStatus(bool status)
    // {
    //     if (status == true && !isRunningCoroutine)
    //     {
    //         StartCoroutine(CheckVisibleMarker());
    //     }
    // }

    // IEnumerator CheckVisibleMarker()
    // {
    //     isRunningCoroutine = true;

    //     Debug.Log("Корутина запущена");
        
    //     // Ключевое условие: работает, пока переменная true
    //     while (Machine.isVisible)
    //     {
    //     Debug.Log("Работает...");
    //         // // Проводим линию от наблюдателя к цели
    //         // // Linecast возвращает true, если что-то попалось на пути
    //         // if (Physics.Linecast(Target.transform.position, Machine.transform.position, out RaycastHit hit))
    //         // {
    //         //     // Если объект, в который попали, - это наша цель, значит, она видна
    //         //     if (hit.transform == Machine.transform)
    //         //     {
    //         //         return true;
    //         //     }
    //         //     // Если попали во что-то другое, цель скрыта
    //         //     return false;
    //         // }
    //         // // Если ничего не попалось, цель видна
    //         // return true;

    //         // Задержка или возврат управления, чтобы не зависнуть
    //         yield return new WaitForSeconds(1.0f);
    //     }

    //     Debug.Log("Корутина остановлена");
    //     isRunningCoroutine = false;
    // }
    }

    public void OnInit()
    {
        // progressHP.color = _gameManager.Settings.colorMarkerProgress;
        bg.color = _gameManager.Settings.colorMarkerBg;
        transform.localPosition = Vector3.zero;

        Refresh(cancelTokenSource.Token).Forget();
    }

    public void OnChangeData()
    {
        // // var oneProcentHP = startSize / Machine.Config.hp;

        // progressHP.size = new Vector2(progressHP.size.x, Mathf.Min(1, startSize * (1 - Machine.Data.ContainerData.levelDestruction)));
    }

    public void OnSetMachine(BaseMachine bm)
    {
        Machine = bm;
        if (!Machine.MachineLevelData.isBot)
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void OnSetTarget(BaseMachine bm)
    {
        Target = bm;
        
        // _camera = Target.LevelManager.Camera.isActiveAndEnabled ? Target.LevelManager.Camera : Target.Camera;
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
