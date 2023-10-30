using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;

public class CharacterManager : Singleton<CharacterManager>
{
    public GameObject CharacterPrefab;

    [HideInInspector]
    public UnityEvent OnCharactersLoaded = new UnityEvent();

    [HideInInspector]
    public CharacterEvent OnCharacterAdded = new CharacterEvent();

    [HideInInspector]
    public CharacterEvent OnCharacterRemoved = new CharacterEvent();

    [HideInInspector]
    public PlacementEvent OnPlacementChange = new PlacementEvent();
    [ReadOnly]
    public List<Character> Characters = new List<Character>();

    public List<Character> AICharacters = new List<Character>();

    public Character Player { get; private set; }

    public Character GetRandomAI
    {
        get
        {
            return AICharacters[Random.Range(0, Characters.Count)];
        }
    }


    public void AddCharacter(Character character)
    {
        if (!Characters.Contains(character))
        {
            if (character.CharacterData.CharacterControlType == CharacterControlType.Player)
                Player = character;

            Characters.Add(character);
            if (character.CharacterData.CharacterControlType == CharacterControlType.AI)
                AICharacters.Add(character);

            OnCharacterAdded.Invoke(character);
        }
    }

    public void RemoveCharacter(Character character)
    {
        if (Characters.Contains(character))
        {
            if (character.CharacterData.CharacterControlType == CharacterControlType.Player)
                Player = null;

            Characters.Remove(character);
            if (character.CharacterData.CharacterControlType == CharacterControlType.AI)
                AICharacters.Remove(character);

            OnCharacterRemoved.Invoke(character);
        }
    }

    

    public Character CreateCharacter(CharacterData characterData, Vector3 position, Quaternion rotation)
    {
        Character character = Instantiate(CharacterPrefab, position, rotation).GetComponent<Character>();
        character.InitilizeCharacter(characterData);
        return character;
    }

    [Button]
    private void KillAllAI()
    {
        List<Character> characters = new List<Character>(Characters);
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].CharacterData.CharacterControlType == CharacterControlType.Player)
                continue;

            characters[i].KillCharacter();
        }
    }

    [Button]
    private void KillPlayer()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].CharacterData.CharacterControlType == CharacterControlType.Player)
                Characters[i].KillCharacter();

        }
    }
}


public class CharacterEvent : UnityEvent<Character> { }
public class PlacementEvent : UnityEvent<int> { }
