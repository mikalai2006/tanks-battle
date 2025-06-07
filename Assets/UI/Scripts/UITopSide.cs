using System;
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
    [SerializeField] private Image gerbImage;
    private BaseMachine target;
    public BaseMachine Target => target;
    private float maxWidth = 280;
    [SerializeField] private SerializedDictionary<TypeBonus, BonusLayoutItem> gameObjectsBonus;

    void Awake()
    {
        _levelManager = GameObject.FindGameObjectWithTag("LevelManager")?.GetComponent<LevelManager>();
    }

    void Update()
    {
        if (target != null)
        {
            // for (int i = 0; i < Target.Data.bonuses.Count; i++)
            // {
            //     Target.Data.bonuses.ElementAt(i).Value.time -= Time.deltaTime;

            //     if (Target.Data.bonuses.ElementAt(i).Value.time <= 0)
            //     {
            //         TypeBonus key = Target.Data.bonuses.ElementAt(i).Key;
            //         Destroy(gameObjectsBonus[key].gameObject);
            //         Target.Data.bonuses.Remove(key);
            //         gameObjectsBonus.Remove(key);
            //     }
            // }
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
            obj.Init(configBonus, target);
            gameObjectsBonus.Add(configBonus.typeBonus, obj);
        }
    }

    public void OnRemoveUIBonus(TypeBonus key)
    {
        Destroy(gameObjectsBonus[key].gameObject);
        gameObjectsBonus.Remove(key);
    }

    public void OnSetTarget(BaseMachine bm)
    {
        target = bm;

        Init(bm.MachineLevelData);
    }

    public void Init(MachineLevelData _machineLevelData)
    {
        textName.text = _machineLevelData.name;

        var configRank = _gameManager.Settings.ranks.Find(r => r.name.ToString() == _machineLevelData.rank.ToString());

        rankImage.sprite = configRank.sprite;

        // установка герба.
        Sprite gerb = _gameManager.Settings.gerbs.Find(l => l.name == _machineLevelData.gerbId);
        if (gerb)
        {
            gerbImage.sprite = gerb;
        }
    }
    
    public void OnChangeData(BaseMachine machine)
    {
        var oneProcentHP = maxWidth / machine.Config.hp;
        progressHP.sizeDelta = new Vector2(oneProcentHP * machine.Data.hp, progressHP.sizeDelta.y);
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
