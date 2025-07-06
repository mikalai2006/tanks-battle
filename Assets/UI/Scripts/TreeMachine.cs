using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Loader;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

public class TreeMachine : UILocaleBase
{
  // Функция для рекурсивного вывода структуры дерева
    static void PrintNode(TreeNode node, int level, TreeNode root)
    {
        Debug.Log($"{new string(' ', level * 2)}{node.Name}/{node.GetNodeLevel(node, root)}");
        foreach (var child in node.Children)
        {
            PrintNode(child, level + 1, root);
        }
    }
  static class ClassNames
  {
    public static string TreeMachineRow = "tree-machine__row";
    public static string TreeMachineItem = "tree-machine__item";
  }
  // [DllImport("__Internal")]
  // private static extern void GetLeaderBoard();
  private UIDocument doc;
  [SerializeField] private VisualTreeAsset _treeAssetItem;
  [SerializeField] private Button _closeButton;
  private TaskCompletionSource<DataDialogResult> _processCompletionSource;
  private DataDialogResult _result;

  // private void Awake()
  // {
  //   UISettings.OnChangeLocale += RefreshMenu;
  //   GameManager.OnChangeTheme += RefreshMenu;
  //   StateManager.OnChangeState += SetValue;
  //   // DataManager.OnLoadLeaderBoard += DrawLeaderListBlok;
  // }

  // private void OnDestroy()
  // {
  //   UISettings.OnChangeLocale -= RefreshMenu;
  //   GameManager.OnChangeTheme -= RefreshMenu;
  //   StateManager.OnChangeState -= SetValue;
  //   // DataManager.OnLoadLeaderBoard -= DrawLeaderListBlok;
  // }

  public virtual void Start()
  {
    doc = GetComponent<UIDocument>();
    doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);

    var treeBox = doc.rootVisualElement.Q<VisualElement>("TreeMachine");
    treeBox.Clear();

    // // Create boxes machines.
    // List<TreeNode> items = new List<TreeNode>();
    // var allMachineConfig = _gameSetting.machines;
    // for (int i = 0; i < allMachineConfig.Count; i++)
    // {
    //   var config = allMachineConfig[i];
    //   // if (config.parent != null)
    //   // {
    //   var parentName = config.parent ? config.parent.name : "";
    //   items.Add(new TreeNode(config.name, parentName, config.name));
    //   Debug.Log($"{config.name}, {parentName}, {config.name}");
    //   // }
    // }
    // Tree tree = new Tree();
    // tree.BuildTree(items);
    // foreach (var root in tree.Roots)
    //     {
    //         PrintNode(root, 0, root);
    //     }

    _closeButton = doc.rootVisualElement.Q<Button>("BtnClose");
    _closeButton.clickable.clicked += () =>
    {
      Close();
    };

    base.Initialize(doc.rootVisualElement);
  }

  private void GeometryChangedCallback(GeometryChangedEvent evt)
  {
    doc.rootVisualElement.UnregisterCallback<GeometryChangedEvent>(GeometryChangedCallback);

    var allBoxs = doc.rootVisualElement.Query<VisualElement>(className: ClassNames.TreeMachineRow).ToList();

    for (int i = 0; i < allBoxs.Count; i++)
    {
      var treeItems = allBoxs[i].Query<VisualElement>(className: ClassNames.TreeMachineItem).ToList();
      for (int j = 0; j < treeItems.Count; j++)
      {
        treeItems[j].generateVisualContent += Draw;
        Debug.Log($"item0: {treeItems[j].parent.worldBound.position}");
      }
    }
  }

  void Draw(MeshGenerationContext ctx)
  {

    Debug.Log($"item: {ctx.visualElement.worldBound.position}");
    var painter = ctx.painter2D;
    painter.lineWidth = 2.0f;
    painter.lineCap = LineCap.Round;
    painter.strokeColor = Color.red;

    painter.BeginPath();
    painter.MoveTo(new Vector2(10, 10));
    painter.BezierCurveTo(new Vector2(100, 100), new Vector2(200, 0), new Vector2(300, 100));
    painter.Stroke();
  }


  public async UniTask<DataDialogResult> ProcessAction()
  {
    _result = new DataDialogResult();


    // #if ysdk
    //         GetLeaderBoard();
    // #endif


    _processCompletionSource = new TaskCompletionSource<DataDialogResult>();

    return await _processCompletionSource.Task;
  }


  private void Close()
  {
    AudioManager.Instance.Click();

    _result.isOk = true;

    _processCompletionSource.SetResult(_result);

    // _gameManager.InputManager.Enable();
  }
}

public class TreeNode
{
    public string Id { get; set; }
    public string ParentId { get; set; }
    public string Name { get; set; }
    public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    public int GetNodeLevel(TreeNode node, TreeNode root)
    {
        if (node == null || root == null)
        {
            return -1;
        }

        if (node == root)
        {
            return 0;
        }

        int level = -1;
        foreach (TreeNode child in root.Children)
        {
            level = GetNodeLevel(node, child);
            if (level != -1)
            {
                return level + 1;
            }
        }

        return -1;
    }

  public TreeNode(string id, string parentId, string name)
  {
    Id = id;
    ParentId = parentId;
    Name = name;
  }
}

public class Tree
{
    public List<TreeNode> Roots { get; set; } = new List<TreeNode>();

    public void BuildTree(List<TreeNode> items)
    {
        // Сначала находим корневые узлы (у которых нет родителя)
        Roots = items.Where(i => i.ParentId == "").ToList();

        // Рекурсивно строим поддеревья для каждого корневого узла
        foreach (var root in Roots)
        {
            BuildSubtree(root, items);
        }
    }

    private void BuildSubtree(TreeNode parent, List<TreeNode> items)
    {
        // Находим детей текущего родителя
        var children = items.Where(i => i.ParentId == parent.Id).ToList();

        // Добавляем детей к текущему родителю
        parent.Children.AddRange(children);

        // Рекурсивно строим поддеревья для каждого ребенка
        foreach (var child in children)
        {
            BuildSubtree(child, items);
        }
    }
    
}
