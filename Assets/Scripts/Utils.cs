using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils
{
    public static Vector3 GetMouseWorldDirection(Vector3 sourcePosition)
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 playerPositionInScreen = Camera.main.WorldToScreenPoint(sourcePosition);
        Vector3 directionInScreen = mousePosition - playerPositionInScreen;
        Vector3 direction = new Vector3(0, directionInScreen.y, directionInScreen.x);

        return direction.normalized;
    }

    public static int NextSceneToPlay = 0;

    public static string Instructions = "";
    public static Dictionary<string, bool> IsIntructionShown = new Dictionary<string, bool>();

    // create a list of string to store the name of the scenes
    public static List<string> InstructionShownScenes = new List<string>();
    public static string GetInstructions()
    {
        string Instructions = "";
        string scene_name = SceneManager.GetActiveScene().name.ToLower();
        if (scene_name.Contains("skilltree") && !InstructionShownScenes.Contains(scene_name))
        {
            if (Skills.Points >= Skills.Get(Skill.ID.DASH).Cost)
            {
                if (!Skills.Get(Skill.ID.DASH).Unlocked)
                {
                    Instructions = "W-" + "Click on the button to unlock the dash.\n";
                    InstructionShownScenes.Add(scene_name);
                }
            }
            else
            {
                Instructions = "W-" + "Finish a level to gain spectral units.\n";
            }
            
        }
        else if (scene_name.Contains("0") && !InstructionShownScenes.Contains(scene_name))
        {
            Instructions = "W-";
            Instructions += "Press ESCAPE to pause the game and check the controls.\n";
            Instructions += "Attack the ennemy to win spectral units and unlock skills.\n";
            
            InstructionShownScenes.Add(scene_name);
            
        }
        else if (scene_name.Contains("1") && !InstructionShownScenes.Contains(scene_name))
        {
            Instructions = "W-";
            Instructions += "Possess an enemy and use its powers by casting your soul on it.\n";
            Instructions += "You can't stay in the same enemy for too long !\n";
            Instructions += "/!\\ Be careful, you can't take possession of an enemy that is attacking you !\n";
            if (Skills.Get(Skill.ID.DASH).Unlocked)
            {
                Instructions += "Use F to dash\n";
            }
            InstructionShownScenes.Add(scene_name);
        }
        else if (scene_name.Contains("2") && !InstructionShownScenes.Contains(scene_name))
        {
            Instructions = "W-";
            Instructions += "Some enemies have a shield that protects them from frontal attacks.\n";
            Instructions += "Destroy the boxes and step on the heals to regain health.\n";
            InstructionShownScenes.Add(scene_name);
        }

        return Instructions;
    }


}
