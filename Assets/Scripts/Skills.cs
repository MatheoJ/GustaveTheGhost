using System.Collections.Generic;
using static Skill;

public class Skill
{
    public enum ID
    {
        DASH,
        JUMP,
        DOUBLE_JUMP,
        TRIPLE_JUMP,
        POSSESSION,
        FIVE_SEC_POSSESSION,
        TEN_SEC_POSSESSION,
        ONE_BOUNCE,
        TWO_BOUNCE
    }

    public ID Id { get; private set; }
    public string Name { get; private set; }
    public int Cost { get; private set; }
    public bool Unlocked { get; private set; }

    public Skill(ID id, string name, int cost, bool unlocked)
    {
        Id = id;
        Name = name;
        Cost = cost;
        Unlocked = unlocked;
    }

    public void Unlock()
    {
        Unlocked = true;
        AudioManager.Instance.PlaySound("unlocked");
    }
}

public static class Skills
{
    public static int Points { get; private set; } = 0;
    public static bool CanDash { get => _Skills[ID.DASH].Unlocked; }
    public static bool CanDoubleJump { get => _Skills[ID.DOUBLE_JUMP].Unlocked; }
    public static bool CanTripleJump { get => _Skills[ID.TRIPLE_JUMP].Unlocked; }
    public static bool HasOneBounce { get => _Skills[ID.ONE_BOUNCE].Unlocked; }
    public static bool HasTwoBounce { get => _Skills[ID.TWO_BOUNCE].Unlocked; }
    public static bool HasPossession { get => _Skills[ID.POSSESSION].Unlocked; }
    public static bool HasFiveSecPossession { get => _Skills[ID.FIVE_SEC_POSSESSION].Unlocked; }
    public static bool HasTenSecPossession { get => _Skills[ID.TEN_SEC_POSSESSION].Unlocked; }


    static Dictionary<ID, Skill> _Skills = new Dictionary<ID, Skill> {
        { ID.DASH, new Skill(ID.DASH, "Dash", 5, false) },

        { ID.JUMP, new Skill(ID.JUMP, "Jump", 0, true) },
        { ID.DOUBLE_JUMP, new Skill(ID.DOUBLE_JUMP, "Double Jump", 8, false) },
        { ID.TRIPLE_JUMP, new Skill(ID.TRIPLE_JUMP, "Triple Jump", 12, false) },

        { ID.POSSESSION, new Skill(ID.POSSESSION, "Possession", 0, true) },
        { ID.FIVE_SEC_POSSESSION, new Skill(ID.FIVE_SEC_POSSESSION, "+5 Seconds\nPossession", 5, false) },
        { ID.TEN_SEC_POSSESSION, new Skill(ID.TEN_SEC_POSSESSION, "+10 Seconds\nPossession", 12, false) },

        { ID.ONE_BOUNCE, new Skill(ID.ONE_BOUNCE, "+1 Bounce", 4, false) },
        { ID.TWO_BOUNCE, new Skill(ID.TWO_BOUNCE, "+2 Bounces", 6, false) }
    };

    public static Skill Get(ID id) => _Skills[id];
    public static void GainPoints(int amount) => Points += amount;
    public static bool SpendPoints(int amount)
    {
        if (Points < amount) return false;

        Points -= amount;
        return true;
    }
}
