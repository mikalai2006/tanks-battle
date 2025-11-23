public static class GameSceneEvents
{
    public static System.Action<AppInfoDamageData> AddInfoDamage;
    public static System.Action<BaseMachine> RefreshHP;
    public static System.Action<BaseMachine> SetHP;
}

/// <summary>
/// Структура данных для подготовки уведомления о нанесенном ущербе.
/// </summary>
[System.Serializable]
public struct AppInfoDamageData
{
    public BaseMachine kto;
    public BaseMachine komy;
    public string userText;
    public float duration;
}
