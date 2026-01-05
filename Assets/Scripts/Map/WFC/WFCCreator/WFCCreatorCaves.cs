#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

public class WFCCreatorCaves : WFCCreator
{
    public override void InitializeGrid(Tile3D[] prefabTiles, ParserHeight _parserHeight, TypeEntity typeCell)
    {
        _parserHeight._settings = wFCBuilder.wFCManager.settingMapCaves;

        base.InitializeGrid(prefabTiles, _parserHeight, typeCell);
    }
   

    public override List<LevelDataGroup> OnSaveTiled()
    {
        List<LevelDataGroup> levelDataGroups = base.OnSaveTiled();

        // сохраняем в файл уровня.
        // записываем сохраненные данные для тайлов.
        var levelData = wFCBuilder.wFCManager.gameLevel.levelData;
        
        levelData.caves = levelDataGroups;

        wFCBuilder.wFCManager.gameLevel.levelData = levelData;

        EditorUtility.SetDirty(wFCBuilder.wFCManager.gameLevel);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return levelDataGroups;
    }

    public override void Load()
    {
        LoadTiles(wFCBuilder.wFCManager.gameLevel.levelData.caves);
    }
}
#endif