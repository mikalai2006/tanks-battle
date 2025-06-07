using System;
using System.Linq;
using UnityEngine;

public class StateManager
{
  public static event Action<StatePlayer> OnChangeState;
 
  private GameManager _gameManager => GameManager.Instance;
  private GameSetting _gameSetting => GameManager.Instance.Settings;
  public StatePlayer statePlayer;
  public StateLevel stateLevel;

 public StatePlayer Init(StatePlayer _stateGame, bool reset = false)
  {
    if (_stateGame == null && !reset)
    {

      // Debug.Log("Get stateGame from playPref");
      _stateGame = JsonUtility.FromJson<StatePlayer>(PlayerPrefs.GetString(_gameManager.Settings.nameSaveData));
    }
    if (_stateGame == null)
    {
      // Debug.Log("Create new stateGame");
      _stateGame = new()
      {
        gerbId = _gameSetting.gerbs.ElementAt(UnityEngine.Random.Range(0, _gameSetting.gerbs.Count)).name,
        machines = new(),
      };
      _gameManager.DataManager.Save(true);
    }

    statePlayer = _stateGame;



    return statePlayer;
  }

  public void InitDataAloneLevel()
  {
    stateLevel = new();

    // генерация случайных имен.
    var listRandomNames = _gameSetting.names.OrderBy(t => UnityEngine.Random.value);

    for (int i = 0; i < _gameManager.LevelConfig.countPlayers; i++)
    {
      MachineLevelData machine = new()
      {
        id = _gameSetting.machines[UnityEngine.Random.Range(0, _gameSetting.machines.Count - 1)].name,
        logo = i == 0 ? statePlayer.gerbId : _gameSetting.gerbs[UnityEngine.Random.Range(0, _gameSetting.gerbs.Count - 1)].name,
        isBot = i != 0,
        name = i == 0 ? _gameManager.AppInfo.UserInfo.name : listRandomNames.ElementAt(i),
      };

      stateLevel.machines.Add(machine);
    }
  }

  public void InitDataCommandLevel()
  {
    stateLevel = new();

    for (int i = 0; i < _gameManager.LevelConfig.countTeam; i++)
    {
      TeamData team = new TeamData()
      {
        index = i,
        logo = i == 0 ? statePlayer.gerbId : _gameSetting.gerbs[UnityEngine.Random.Range(0, _gameSetting.gerbs.Count)].name,
      };

      // TODO

      stateLevel.teams.Add(team);
    }
  }

  public void RefreshData(bool saveDb)
  {

    OnChangeState.Invoke(statePlayer);
  }

  public StatePlayer GetData()
  {
    return statePlayer;
  }
  
  public void Reset()
  {
    _gameManager.StateManager.statePlayer = new StatePlayer();
    _gameManager.StateManager.Init(null);

    OnChangeState?.Invoke(statePlayer);
  }
}