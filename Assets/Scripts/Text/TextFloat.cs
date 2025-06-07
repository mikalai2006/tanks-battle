using UnityEngine;

public class TextFloat : MonoBehaviour
{
    private GameManager _gameManager => GameManager.Instance;
    [SerializeField] private TextMesh textMesh;
    [SerializeField] private Animator animator;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        animator = GetComponentInChildren<Animator>();
    }


    public void Init(BaseMachine baseMachine, bool isAnimation)
    {
        gameObject.transform.localPosition = baseMachine.transform.position;

        Lean.Pool.LeanPool.Despawn(gameObject, 1f);

        if (!isAnimation)
        {
            animator.gameObject.SetActive(false);
        }
    }

    public void OnSetColor(Color color)
    {
        textMesh.color = color;
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
