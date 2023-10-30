using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WeaponList
{
    public WeaponBase WeaponPrefab;
    public Transform WeaponCreatePoint;
}

public class WeaponHolder : MonoBehaviour
{
    public List<WeaponList> WeaponList;

    private WeaponBase currentWeapon;

    private Character character;
    public Character Character { get { return (character == null) ? character = transform.root.GetComponent<Character>() : character; } }


    private void OnEnable()
    {
        Character.OnCharacterInitilize.AddListener(Initialize);
    }


    private void OnDisable()
    {
        Character.OnCharacterInitilize.RemoveListener(Initialize);
    }


    private void Initialize(CharacterData characterData)
    {

        if (string.Equals(characterData.CharacterWeaponData.EqquiepedWeapon, "None"))
            return;

        for (int i = 0; i < WeaponList.Count; i++)
        {
            if(string.Equals(WeaponList[i].WeaponPrefab.WeaponData.WeaponName, characterData.CharacterWeaponData.EqquiepedWeapon))
            {
                if (currentWeapon != null)
                    Destroy(currentWeapon.gameObject);

                currentWeapon = Instantiate(WeaponList[i].WeaponPrefab.gameObject, WeaponList[i].WeaponCreatePoint.position, WeaponList[i].WeaponCreatePoint.rotation, WeaponList[i].WeaponCreatePoint).GetComponent<WeaponBase>();
                Character.OnWeaponAcquired.Invoke(currentWeapon.WeaponData);
                break;
            }
        }
    }

    public void NotifyReload()
    {
        Character.OnWeaponReload.Invoke();
    }

    public void EquipWeapon(WeaponData weaponData)
    {
        for (int i = 0; i < WeaponList.Count; i++)
        {
            if (string.Equals(WeaponList[i].WeaponPrefab.WeaponData.WeaponName, weaponData.WeaponName))
            {
                if (currentWeapon != null)
                    Destroy(currentWeapon.gameObject);

                currentWeapon = Instantiate(WeaponList[i].WeaponPrefab.gameObject, WeaponList[i].WeaponCreatePoint.position, WeaponList[i].WeaponCreatePoint.rotation, WeaponList[i].WeaponCreatePoint).GetComponent<WeaponBase>();
                Character.OnWeaponAcquired.Invoke(currentWeapon.WeaponData);
                break;
            }
        }
    }
}
