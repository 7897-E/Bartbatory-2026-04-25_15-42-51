using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon")]
public class Weapons : ScriptableObject
{
    public string weaponName;

    [TextArea]
    public string description;
    public GameObject weapon;

    public GameObject Apply(PlayerController playerController, Transform weaponHolder, Camera playerCamera)
    {
        if (weapon == null)
        {
            Debug.LogError($"Weapon ScriptableObject '{name}' has no prefab assigned.");
            return null;
        }

        if (weaponHolder == null)
        {
            Debug.LogError("Apply called with no weaponHolder Transform.");
            return null;
        }
        GameObject weaponPivot = Instantiate(weapon);

        weaponPivot.transform.SetParent(weaponHolder, worldPositionStays: false);

        BatScript bat = weaponPivot.GetComponentInChildren<BatScript>();
        if (bat == null)
        {
            Debug.LogError($"Weapon prefab '{weapon.name}' does not have a BatScript on its children.");
            return weaponPivot;
        }

        if (playerCamera != null)
        {
            bat.cam = playerCamera;
        }
        return weaponPivot;
    }
}