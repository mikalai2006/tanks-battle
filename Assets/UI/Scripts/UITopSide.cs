using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UITopSide : MonoBehaviour
{
    GameManager _gameManager => GameManager.Instance;
    LevelManager _levelManager;
    [SerializeField] private GameObject _objectSpawnBonuses;
    [SerializeField] private RectTransform progressHP;
    [SerializeField] public TMPro.TextMeshProUGUI textName;
    [SerializeField] private Image rankImage;
    [SerializeField] private BaseMachine Target;
    [SerializeField] private SerializedDictionary<TypeBonus, BonusLayoutItem> gameObjectsBonus;

    void Awake()
    {
        _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();
    }

    void Update()
    {
        if (Target != null)
        {
            for (int i = 0; i < Target.Data.bonuses.Count; i++)
            {
                Target.Data.bonuses.ElementAt(i).Value.time -= Time.deltaTime;

                if (Target.Data.bonuses.ElementAt(i).Value.time <= 0)
                {
                    TypeBonus key = Target.Data.bonuses.ElementAt(i).Key;
                    Destroy(gameObjectsBonus[key].gameObject);
                    Target.Data.bonuses.Remove(key);
                    gameObjectsBonus.Remove(key);
                }
            }
        }
        else
        {
            BaseMachine bm = _levelManager.machines.Find(m => !m.MachineLevelData.isBot);
            if (bm != null)
            {
                OnSetTarget(bm);
            }
        }
    }

    public void OnCreateUIBonus(GameBonus configBonus)
    {
        if (!gameObjectsBonus.ContainsKey(configBonus.typeBonus))
        {
            var obj = Instantiate(configBonus.prefabUI, _objectSpawnBonuses.transform);
            obj.Init(configBonus, Target);
            gameObjectsBonus.Add(configBonus.typeBonus, obj);
        }
    }

    public void OnSetTarget(BaseMachine bm)
    {
        Target = bm;
    }

    public void OnToStartMenu()
    {

        if (_levelManager == null)
        {
            // CloseSettings();
        }
        else
        {
            AudioManager.Instance.Click();

            _gameManager.ChangeState(GameState.CloseLevel);

            var dashBoard = new StartUIOperation();
            dashBoard.ShowAndHide().Forget();
        }

    }
}
