using System;
using System.Collections.Generic;

[Serializable]
public class StatePlayer
{
  public int playDay;
  public string lastDay;
  public string gerbId;
  public int coutBattle;
  public int countWin;
  public int rank;
  public int score;
  public int indexActiveMachine;
  public List<StateMachinePlayer> machines;
  public int coin;

  public StatePlayer()
  {
    machines = new();
  }
}

[Serializable]
public class StateMachinePlayer
{
    public string name;
    public List<ColorsModify> colorsModifies;
      
}

[Serializable]
public class StateLevel
{
  public List<TeamData> teams;
  public List<MachineLevelData> machines;

  public StateLevel()
  {
    teams = new();
    machines = new();
  }
}

[Serializable]
public class MachineLevelData
{
  public string id;
  public string gerbId;
  public string name;
  public int rank;
  public bool isBot;
  public List<ColorsModify> colorsModify;
}


[Serializable]
public class TeamData
{
    public int index;
    public string logo;
}
