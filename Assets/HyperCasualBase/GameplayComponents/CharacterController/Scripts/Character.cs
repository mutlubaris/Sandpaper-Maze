using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public enum CharacterControlType { None, Player, AI }
public enum CharacterPlayerType { Platformer}
public enum CharacterAIType { Petrol, Shooter }
public enum BodyType { CharacterController, Rigidbody}


public class Character : InterfaceBase, IPoolable
{
    [HideInInspector]
    public UnityEvent OnCharacterRevive = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnCharacterDie = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnCharacterHit = new UnityEvent();
    [HideInInspector]
    public HitEvent OnCharacterReciveDamage = new HitEvent();
    [HideInInspector]
    public UnityEvent OnCharacterJump = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnCharacterSet = new UnityEvent();
    [HideInInspector]
    public SkinEvent OnSkinChange = new SkinEvent();
    [HideInInspector]
    public InitilizeEvent OnCharacterInitilize = new InitilizeEvent();

    #region WeaponEvents
    [HideInInspector]
    public UnityEvent OnCharacterAttack = new UnityEvent();
    [HideInInspector]
    public WeaponEvent OnWeaponAcquired = new WeaponEvent();
    [HideInInspector]
    public UnityEvent OnWeaponReload = new UnityEvent();
    #endregion

    private CharacterData characterData;
    public CharacterData CharacterData { get { return characterData; } set { characterData = value; } }

    System.Type interfaceType;
    private ICharacterController characterController;
    public ICharacterController CharacterController { get { return Utilities.IsNullOrDestroyed(characterController, out interfaceType) ? characterController = GetComponent<ICharacterController>() : characterController; } }

    public bool IsGrounded { get { return (CharacterController == null) ? false : CharacterController.IsGrounded(); } }
    public float CurrentSpeed { get { return (CharacterController == null) ? 0 : CharacterController.CurrentSpeed(); } }

    private CharacterMovementData movementData;
    public CharacterMovementData CharacterMovementData { get { return (movementData == null) ? movementData = CharacterData.CharacterMovementData : movementData; } }

    [ReadOnly]
    public bool isDead;

    [ShowInInspector]
    [ReadOnly]
    private bool isControlable;
    public bool IsControlable { get { return isControlable; } set { isControlable = value; } }

    private void OnEnable()
    {
        if (Managers.Instance == null)
            return;


        LevelManager.Instance.OnLevelStart.AddListener(() => {
            ReviveCharacter();
        });
        LevelManager.Instance.OnLevelFinish.AddListener(() => IsControlable = false);
    }

    private void OnDisable()
    {
        if (Managers.Instance == null)
            return;


        LevelManager.Instance.OnLevelStart.RemoveListener(() => {
            ReviveCharacter();
        });
        LevelManager.Instance.OnLevelFinish.RemoveListener(() => IsControlable = false);
    }

    public void InitilizeCharacter(CharacterData characterData)
    {
        CharacterData = characterData;
        OnCharacterInitilize.Invoke(CharacterData);

        if(!LevelManager.Instance.IsLevelStarted)
        {
            IsControlable = false;
            isDead = true;
        }
        else
        {
            IsControlable = true;
            isDead = false;
        }

        CharacterManager.Instance.AddCharacter(this);
        SetCharacter();
    }

    public void KillCharacter()
    {
        if (isDead)
            return;

        IsControlable = false;
        isDead = true;
        CharacterManager.Instance.RemoveCharacter(this);
        OnCharacterDie.Invoke();
        Destroy(gameObject, 2f);
    }

    [Button]
    public void ReviveCharacter()
    {
        if (!isDead)
            return;


        IsControlable = true;
        isDead = false;
        //Reset Character values here
        OnCharacterRevive.Invoke();
    }

    [Button]
    public void DamageCharacter(int damage)
    {
        OnCharacterReciveDamage.Invoke(damage);
    }

    private void SetCharacter()
    {
        StartCoroutine(SetCharacterCo());
    }

    IEnumerator SetCharacterCo()
    {
        if (CharacterData.CharacterControlType == CharacterControlType.None)
        {
            OnCharacterSet.Invoke();
            yield break;
        }

        CameraTarget cameraTarget = GetComponent<CameraTarget>();
        List<ICharacterBrain> characterBrains = new List<ICharacterBrain>(GetComponentsInChildren<ICharacterBrain>());

        //Clear Character Brains
        foreach (var item in characterBrains)
        {
            item.Dispose();
        }

        yield return new WaitForEndOfFrame();

        
        switch (CharacterData.CharacterControlType)
        {
            case CharacterControlType.Player:
                //Add player bain
                var brainType = CharacterData.CharacterPlayerType.GetBehavior();
                var brain = gameObject.AddComponent(brainType);
                brain.GetComponent<ICharacterBrain>().Initialize();

                //Add camera target to make this object trackable by camera
                if (!cameraTarget)
                    gameObject.AddComponent<CameraTarget>();
                break;

            case CharacterControlType.AI:
                //Remove camera target
                if (cameraTarget)
                    Utilities.DestroyExtended(cameraTarget);

                //Add AI brain based on the type in character data
                var aiType = CharacterData.CharacterAIType.GetBehevior();
                var aiBrain = gameObject.AddComponent(aiType);
                aiBrain.GetComponent<ICharacterBrain>().Initialize();
                break;

        }
        OnCharacterSet.Invoke();
    }

    public void Initilize()
    {
        ReviveCharacter();
    }

    public void Dispose()
    {
        KillCharacter();
    }
}

public class SkinEvent : UnityEvent<Skin> { }
public class InitilizeEvent : UnityEvent<CharacterData> { }
public class HitEvent : UnityEvent<int> { }
public class WeaponEvent : UnityEvent<WeaponData> { }
