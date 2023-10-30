using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Skill;

public class Skill
{
    public enum ID
    {
        DOUBLE_JUMP,
        DASH,
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

    public void Unlock() => Unlocked = true;
}

public static class Skills
{
    public static bool CanDoubleJump { get => _Skills[ID.DOUBLE_JUMP].Unlocked; }
    public static bool CanDash { get => _Skills[ID.DASH].Unlocked; }

    static Dictionary<ID, Skill> _Skills = new Dictionary<ID, Skill> {
        { ID.DASH, new Skill(ID.DASH, "Dash", 2, false) },
        { ID.DOUBLE_JUMP, new Skill(ID.DOUBLE_JUMP, "Double Jump", 4, false) },
    };

    public static Skill Get(ID id) => _Skills[id];
}
