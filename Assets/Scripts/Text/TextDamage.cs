using UnityEngine;

public class TextDamage : MonoBehaviour
{
    private GameManager _gameManager => GameManager.Instance;
    [SerializeField] private TextMesh textMesh;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();

        // Animator anim = gameObject.GetComponentInChildren<Animator>();
        // if (anim)
        // {
        //     anim.Rebind();
        //     anim.Update(0f);
        // }
    }

    // void Start()
    // {
    //     // DestroyDelay().Forget();
    //     // Destroy(gameObject, 1f);
    //     Lean.Pool.LeanPool.Despawn(gameObject, 1f);
    // }

    // private async UniTask DestroyDelay()
    // {
    //     await UniTask.Delay(1000);

    //     Lean.Pool.LeanPool.Despawn(gameObject);
    // }

    public void Init(BaseMachine baseMachine)
    {
        gameObject.transform.localPosition = baseMachine.transform.position;
        
        Lean.Pool.LeanPool.Despawn(gameObject, 1f);
    }

    public void OnSetText(string str)
    {
        textMesh.text = str.ToString();

        if (str.StartsWith("-"))
        {
            textMesh.color = _gameManager.Settings.colorTextDamage;
        }
        else
        {
            textMesh.color = _gameManager.Settings.colorTextDamagePlus;
        }
    }
}
