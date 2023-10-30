using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterBrainDataHolder
{
    private static Dictionary<CharacterAIType, System.Type> characterAIDictionary;
    public static Dictionary<CharacterAIType, System.Type> CharacterAIDictionary
    {
        get
        {
            if(characterAIDictionary == null)
            {
                characterAIDictionary = new Dictionary<CharacterAIType, System.Type>();
                characterAIDictionary[CharacterAIType.Petrol] = typeof(AIPetrolBrain);
                characterAIDictionary[CharacterAIType.Shooter] = typeof(AIShooterBrain);
            }

            return characterAIDictionary;
        }
    }


    public static System.Type GetBehevior(this CharacterAIType characterAIType)
    {
        return CharacterAIDictionary[characterAIType];
    }

    private static Dictionary<CharacterPlayerType, System.Type> characterPlayerDictionary;
    public static Dictionary<CharacterPlayerType, System.Type> CharacterPlayerDictionary
    {
        get
        {
            if(characterPlayerDictionary == null)
            {
                characterPlayerDictionary = new Dictionary<CharacterPlayerType, System.Type>();
                characterPlayerDictionary[CharacterPlayerType.Platformer] = typeof(PlayerPlatformerBrain);
            }
            return characterPlayerDictionary;
        }
    }

    public static System.Type GetBehavior(this CharacterPlayerType characterPlayerType)
    {
        return CharacterPlayerDictionary[characterPlayerType];
    }

}
