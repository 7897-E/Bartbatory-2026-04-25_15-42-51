using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon")]
public class Weapons : ScriptableObject
{
    public string weaponName;

    [TextArea]
    public string description;
    public GameObject weapon;

    [Header("Weapon Stats")]
    public float fireRate = 0.3f;
    public int meleeDamage = 1;
    public int projectileDamage = 5;
     public float bulletSpeed = 10f;
    public int MaxHits = 1;
    public int bounces = 0;
    public float weaponRadius = 1f;

    public GameObject Apply(PlayerController playerController, Transform weaponHolder, Camera playerCamera, int weaponIndex = 0)
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
        
        float radius = weaponRadius;
        int totalWeapons = playerController.weaponCount;
        float angle = (weaponIndex / (float)totalWeapons) * 360f * Mathf.Deg2Rad;
        weaponPivot.transform.localPosition = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0); 

        BatScript bat = weaponPivot.GetComponentInChildren<BatScript>();
        if (bat == null)
        {
            Debug.LogError($"Weapon prefab '{weapon.name}' does not have a BatScript on its children.");
            return weaponPivot;
        }

        bat.fireRate = fireRate;
        bat.meleeDamage = meleeDamage;
        bat.projectileDamage = projectileDamage;
        bat.bulletSpeed = bulletSpeed;
        bat.MaxHits = MaxHits;
        bat.bounces = bounces;

        if (playerCamera != null)
        {
            bat.cam = playerCamera;
        }
        return weaponPivot;
    }
}