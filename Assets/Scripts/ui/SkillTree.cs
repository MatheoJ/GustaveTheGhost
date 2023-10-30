using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode
{
    public Vector2 Position { get; private set; }
    public int Index { get; private set; }
    public int[] Requirements { get; private set; }

    public string Name { get => _Skill.Name; }
    public bool Unlocked { get => _Skill.Unlocked; }
    public int Cost { get => _Skill.Cost; }

    Skill _Skill { get; set; }
    RectTransform _Transform { get; set; }
    Button _Button { get; set; }
    TextMeshProUGUI _Label {  get; set; }
    Image _Image { get; set; }

    public SkillNode(int index, Skill skill, Vector2 position, int[] requirements)
    {
        Index = index;
        Position = position;
        Requirements = requirements;

        _Skill = skill;
    }

    public void Unlock() => _Skill.Unlock();

    public void Initialize(RectTransform parent, GameObject o, Vector2Int gridOffset)
    {
        _Transform = o.GetComponent<RectTransform>();
        _Button = o.GetComponent<Button>();
        _Label = o.GetComponentInChildren<TextMeshProUGUI>();
        _Image = o.GetComponent<Image>();

        _Transform.SetParent(parent);
        _Transform.anchorMin = Vector2.up;
        _Transform.anchorMax = Vector2.up;

        Position -= gridOffset;
    }

    public void Update(Vector2 outMargin, Vector2 inMargin, Color color, UnityEngine.Events.UnityAction onClick)
    {
        string label = Name + (Unlocked ? "" : " (" + Cost.ToString() + ")");
        _Label.text = label;

        _Image.color = color;

        float x = outMargin.x + _Transform.sizeDelta.x / 2 + Position.x * (_Transform.sizeDelta.x + inMargin.x);
        float y = outMargin.y + _Transform.sizeDelta.y / 2 + Position.y * (_Transform.sizeDelta.y + inMargin.y);
        _Transform.localPosition = new Vector2(x, -y);

        _Button.onClick.RemoveAllListeners();
        _Button.onClick.AddListener(onClick);
    }

    public Vector2 GetTopPosition() => GetPosition(Vector2.up * _Transform.sizeDelta.y / 2);
    public Vector2 GetBottomPosition() => GetPosition(Vector2.down * _Transform.sizeDelta.y / 2);

    Vector2 GetPosition(Vector2 offset) => (Vector2)_Transform.localPosition + offset;
}

public class SkillTree : MonoBehaviour
{
    const string TITLE = "Compétences";
    int POINTS_AVAILABLE = 9;

    [SerializeField] RectTransform Content;
    [SerializeField] TextMeshProUGUI TitleText;
    [SerializeField] TextMeshProUGUI PointsText;

    Dictionary<int, SkillNode> NodesDict = new Dictionary<int, SkillNode>();
    SkillNode[] Nodes = {
        new SkillNode(0, Skills.Get(Skill.ID.DASH), new Vector2(1, 0), new int[] {}),
        new SkillNode(1, Skills.Get(Skill.ID.DOUBLE_JUMP), new Vector2(2, 0), new int[] {}),
        //new SkillNode(2, "Ability A.1", false, 2, new Vector2(0, 1), new int[] { 0 }),
        //new SkillNode(3, "Ability A.2", true, 3, new Vector2(1, 1), new int[] { 0 }),
        //new SkillNode(4, "Ability B.1", false, 2, new Vector2(2, 1), new int[] { 1 }),
        //new SkillNode(5, "Ability B.2", false, 3, new Vector2(3, 1), new int[] { 1 }),
        //new SkillNode(6, "Ability C", false, 5, new Vector2(2, 2), new int[] { 3, 4 }),
    };
    
    Vector2 _OutMargin = new Vector2(20, 20);
    Vector2 _InMargin = new Vector2(10, 30);

    private void Awake()
    {
        if (Content == null) throw new MissingReferenceException("Missing 'content' in SkillTree.");
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject asset = Resources.Load<GameObject>("Prefabs/SkillNode");

        // Set boundaries and create nodes
        Vector2Int gridSize = Vector2Int.zero;
        Vector2Int gridOffset = Vector2Int.one * int.MaxValue;
        foreach (SkillNode node in Nodes)
        {
            if (node.Position.x < gridOffset.x) gridOffset.x = (int)node.Position.x;
            if (node.Position.y < gridOffset.y) gridOffset.y = (int)node.Position.y;
            if (node.Position.x >= gridSize.x) gridSize.x = (int)node.Position.x + 1;
            if (node.Position.y >= gridSize.y) gridSize.y = (int)node.Position.y + 1;
        }
        gridSize -= gridOffset;

        if (gridSize.x == 0) return;
        if (gridSize.y == 0) return;

        // Set content size
        Vector2 assetSize = asset.GetComponent<RectTransform>().sizeDelta;
        Content.sizeDelta = new Vector2(
            2 * _OutMargin.x + assetSize.x + (gridSize.x - 1) * (assetSize.x + _InMargin.x),
            2 * _OutMargin.y + assetSize.y + (gridSize.y - 1) * (assetSize.y + _InMargin.y)
        );

        // Initialize nodes
        foreach (SkillNode node in Nodes)
        {
            node.Initialize(Content, Instantiate(asset), gridOffset);
            NodesDict.Add(node.Index, node);
        }

        // Update nodes
        UpdateData();

        // Draw lines between nodes
        foreach (SkillNode node in Nodes)
        {
            foreach (int requirement in node.Requirements)
            {
                if (!NodesDict.ContainsKey(requirement)) throw new KeyNotFoundException("Missing ability node '" + requirement.ToString() + "' in tree");

                MakeLine(NodesDict[requirement].GetBottomPosition(), node.GetTopPosition(), Color.black);
            }
        }
    }

    void PurchaseAbility(int index)
    {
        if (NodesDict[index].Unlocked) return;
        if (!AreRequirementsMet(index)) return;
        if (!IsAbilityAffordable(index)) return;

        NodesDict[index].Unlock();
        POINTS_AVAILABLE -= NodesDict[index].Cost;

        UpdateData();
    }

    bool AreRequirementsMet(int index)
    {
        SkillNode node = NodesDict[index];

        foreach (int requirement in node.Requirements)
        {
            if (NodesDict[requirement].Unlocked)
                continue;

            return false;
        }

        return true;
    }

    bool IsAbilityAffordable(int index)
    {
        SkillNode node = NodesDict[index];

        return POINTS_AVAILABLE >= node.Cost;
    }

    void UpdateData()
    {
        // Set labels
        TitleText.text = TITLE;
        PointsText.text = "Points : " + POINTS_AVAILABLE;

        // Define colors
        Color colorUnlocked = new Color(0, 0.8f, 0);
        Color colorAvailable = new Color(0.95f, 0.95f, 0.95f);
        Color colorTooExpensive = new Color(0.7f, 0.7f, 0.7f);
        Color colorRestricted = new Color(0.4f, 0.4f, 0.4f);

        // Draw nodes
        foreach (SkillNode node in Nodes)
        {
            bool requirementsOk = AreRequirementsMet(node.Index);
            bool costOk = IsAbilityAffordable(node.Index);

            Color color =
                node.Unlocked ? colorUnlocked :
                requirementsOk && costOk ? colorAvailable :
                requirementsOk ? colorTooExpensive :
                colorRestricted;

            node.Update(_OutMargin, _InMargin, color, () => PurchaseAbility(node.Index));
        }
    }

    void MakeLine(Vector2 a, Vector2 b, Color col, float size = 2)
    {
        GameObject line = new GameObject();
        line.name = "line from (" + a.x + ", " + a.y + ") to (" + b.x + ", " + b.y + ")";
        
        Image image = line.AddComponent<Image>();
        image.color = col;

        RectTransform rect = line.GetComponent<RectTransform>();
        rect.SetParent(Content);
        rect.anchorMin = Vector2.up;
        rect.anchorMax = Vector2.up;
        rect.localPosition = (a + b) / 2;
        Vector3 diff = a - b;
        rect.sizeDelta = new Vector3(diff.magnitude, size);
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(diff.y / diff.x) / Mathf.PI));
    }
}
