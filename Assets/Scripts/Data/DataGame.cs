using System;
using System.Collections.Generic;
using UnityEngine;

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
  public List<string> machines;
  public int coin;

  public StatePlayer()
  {
    machines = new();
  }
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
  public string logo;
  public string name;
  public bool isBot; 
}


[Serializable]
public class TeamData
{
    public int index;
    public string logo;
}
