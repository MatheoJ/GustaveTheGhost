using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillNode
{
    public const string ASSET_PATH = "Prefabs/ui/SkillNode";

    public Vector2 Position { get; private set; }
    public Skill.ID Id { get; private set; }
    public Skill.ID[] Requirements { get; private set; }

    public string Name { get => _Skill.Name; }
    public bool Unlocked { get => _Skill.Unlocked; }
    public int Cost { get => _Skill.Cost; }

    Skill _Skill { get; set; }
    RectTransform _Transform { get; set; }
    Button _Button { get; set; }
    TextMeshProUGUI _Label {  get; set; }
    Image _Image { get; set; }

    public SkillNode(Skill.ID skillId, Vector2 position, Skill.ID[] requirements)
    {
        Id = skillId;
        Position = position;
        Requirements = requirements;

        _Skill = Skills.Get(skillId);
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
    const string TITLE = "Purchase New Skills";

    [SerializeField] RectTransform Content;
    [SerializeField] TextMeshProUGUI TitleText;

    // find back button 
    public GameObject backButton;
    public GameObject game;

    Dictionary<Skill.ID, SkillNode> NodesDict = new Dictionary<Skill.ID, SkillNode>();

    SkillNode[] Nodes = {
        new SkillNode(Skill.ID.DASH, new Vector2(1, 0), new Skill.ID[] {}),

        new SkillNode(Skill.ID.JUMP, new Vector2(2, 0), new Skill.ID[] {}),
        new SkillNode(Skill.ID.DOUBLE_JUMP, new Vector2(2, 1), new Skill.ID[] { Skill.ID.JUMP }),
        new SkillNode(Skill.ID.TRIPLE_JUMP, new Vector2(2, 2), new Skill.ID[] { Skill.ID.DOUBLE_JUMP }),

        new SkillNode(Skill.ID.POSSESSION, new Vector2(3, 0), new Skill.ID[] {}),
        new SkillNode(Skill.ID.FIVE_SEC_POSSESSION, new Vector2(3, 1), new Skill.ID[] { Skill.ID.POSSESSION }),
        new SkillNode(Skill.ID.TEN_SEC_POSSESSION, new Vector2(3, 2), new Skill.ID[] { Skill.ID.FIVE_SEC_POSSESSION }),

        new SkillNode(Skill.ID.ONE_BOUNCE, new Vector2(4, 1), new Skill.ID[] { Skill.ID.POSSESSION }),
        new SkillNode(Skill.ID.TWO_BOUNCE, new Vector2(4, 2), new Skill.ID[] { Skill.ID.ONE_BOUNCE })
    };
    
    Vector2 _OutMargin = new Vector2(80, 80);
    Vector2 _InMargin = new Vector2(40, 80);

    private void Awake()
    {
        if (Content == null) throw new MissingReferenceException("Missing 'content' in SkillTree.");
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject asset = Resources.Load<GameObject>(SkillNode.ASSET_PATH);

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
            NodesDict.Add(node.Id, node);
        }

        // Update nodes
        UpdateData();

        // Draw lines between nodes
        foreach (SkillNode node in Nodes)
        {
            foreach (Skill.ID requirement in node.Requirements)
            {
                if (!NodesDict.ContainsKey(requirement)) throw new KeyNotFoundException("Missing ability node '" + requirement.ToString() + "' in tree");
                MakeLine(NodesDict[requirement].GetBottomPosition(), node.GetTopPosition(), Color.white);
            }
        }

        // Set back button
        backButton.GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Level_" + Utils.NextSceneToPlay); 
        });

    }

    void PurchaseAbility(Skill.ID id)
    {
        if (NodesDict[id].Unlocked) return;
        if (!AreRequirementsMet(id)) return;
        if (!IsAbilityAffordable(id)) return;

        bool spent = Skills.SpendPoints(NodesDict[id].Cost);
        if (!spent) return;

        FindObjectOfType<Game>().SkillPoints = Skills.Points;
        NodesDict[id].Unlock();

        UpdateData();
    }

    bool AreRequirementsMet(Skill.ID id)
    {
        SkillNode node = NodesDict[id];

        foreach (Skill.ID requirement in node.Requirements)
        {
            if (NodesDict[requirement].Unlocked)
                continue;

            return false;
        }

        return true;
    }

    bool IsAbilityAffordable(Skill.ID id)
    {
        SkillNode node = NodesDict[id];

        return Skills.Points >= node.Cost;
    }

    void UpdateData()
    {
        // Set labels
        TitleText.text = TITLE;

        // Define colors
        Color colorUnlocked = new Color(0.63f, 0.62f, 0.54f);
        Color colorAvailable = new Color(0.32f, 0.46f, 0.31f);
        Color colorTooExpensive = new Color(0.15f, 0.27f, 0.20f);
        Color colorRestricted = new Color(0.09f, 0.16f, 0.18f);

        // Draw nodes
        foreach (SkillNode node in Nodes)
        {
            bool requirementsOk = AreRequirementsMet(node.Id);
            bool costOk = IsAbilityAffordable(node.Id);

            Color color =
                node.Unlocked ? colorUnlocked :
                requirementsOk && costOk ? colorAvailable :
                requirementsOk ? colorTooExpensive :
                colorRestricted;

            node.Update(_OutMargin, _InMargin, color, () => PurchaseAbility(node.Id));
        }
    }


    void MakeLine(Vector2 a, Vector2 b, Color col, float size = 5)
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
