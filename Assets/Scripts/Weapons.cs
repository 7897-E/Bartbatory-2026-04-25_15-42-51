using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon")]
public class Weapons : ScriptableObject
{
    public string weaponName;

    [TextArea]
    public string description;
    public GameObject weapon;
    public enum WeaponType { Bat, Minigun, Shotgun }

    public WeaponType weaponType;
    [Header("Firing")]
    public float fireRate = 0.3f;
    public int meleeDamage = 1;
    public int projectileDamage = 1;
    public float meleeRange = 1f;
    public float weaponRadius = 1f;
    public int levelup = 5;


    [Header("Bullet")]
    public Baseball levelUpBulletPrefab;
    public float bulletSpeed = 10f;
    public int MaxHits = 1;
    public int bounces = 0;

    [Header("Shotgun")]
    public int shotgunPellets = 5;
    public float shotgunSpread = 30f;
    public float ShotgunRange = 10f;

    [Header("Flamethrower (Shotgun Level Up)")]
    [Tooltip("Flamethrower damage multiplier based on shotgun damage (e.g., 0.4 = 40% of shotgun damage)")]
    public float flamethrowerDamageMultiplier = 0.4f;
    [Tooltip("Flamethrower fire rate (independent of shotgun)")]
    public float flamethrowerFireRate = 0.1f;
    [Tooltip("Flamethrower projectile speed multiplier based on shotgun speed")]
    public float flamethrowerSpeedMultiplier = 0.8f;
    [Tooltip("Flamethrower spread angle (independent of shotgun)")]
    public float flamethrowerSpread = 25f;
    [Tooltip("Flamethrower range (independent of shotgun)")]
    public float flamethrowerRange = 8f;
    [Tooltip("Flamethrower fire duration on enemies")]
    public float flamethrowerFireDuration = 3f;
    [Tooltip("Flamethrower damage per second")]
    public int flamethrowerFireDPS = 2;
    [Tooltip("Number of flames per shot for flamethrower")]
    public int flamethrowerFlameCount = 8;

    [Header("Swing (Bat only)")]
    public float swingAngle = 90f;
    public float swingDuration = 0.15f;
    public int levelsUntilRanged = 5;
    
    [Header("Upgrades")]
    public List<Upgrades> Compatibleupgrades = new List<Upgrades>();
    
    // Flamethrower-specific upgrades (shown when shotgun reaches levelup)
    [Header("Flamethrower Upgrades")]
    public List<Upgrades> FlamethrowerUpgrades = new List<Upgrades>();
    
    // Check if an upgrade is compatible with this weapon at the given level
    public bool IsUpgradeCompatible(Upgrades upgrade, int currentLevel)
    {
        // For shotgun: if below levelup, use regular compatible upgrades
        // If at or above levelup (flamethrower mode), use flamethrower upgrades
        if (weaponType == WeaponType.Shotgun)
        {
            if (currentLevel >= levelup)
            {
                // In flamethrower mode - check flamethrower upgrades
                return FlamethrowerUpgrades.Contains(upgrade);
            }
            else
            {
                // Regular shotgun mode - check regular upgrades
                return Compatibleupgrades.Contains(upgrade);
            }
        }
        
        // For other weapons, just check regular compatible upgrades
        return Compatibleupgrades.Contains(upgrade);
    }
    
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
        bat.shotgunPellets = shotgunPellets;
        bat.shotgunSpread = shotgunSpread;
        bat.ShotgunRange = ShotgunRange;
        bat.levelsUntilRanged = levelsUntilRanged;
        bat.levelup = levelup;
        bat.swingAngle = swingAngle;
        bat.swingDuration = swingDuration;
        bat.weaponType = (BatScript.WeaponType)weaponType;
        bat.levelUpBulletPrefabObject = levelUpBulletPrefab;
        
        bat.flamethrowerDamageMultiplier = flamethrowerDamageMultiplier;
        bat.flamethrowerFireRate = flamethrowerFireRate;
        bat.flamethrowerSpeedMultiplier = flamethrowerSpeedMultiplier;
        bat.flamethrowerSpread = flamethrowerSpread;
        bat.flamethrowerRange = flamethrowerRange;
        bat.flamethrowerFireDuration = flamethrowerFireDuration;
        bat.flamethrowerFireDPS = flamethrowerFireDPS;
        bat.flamethrowerFlameCount = flamethrowerFlameCount;

        if (playerCamera != null)
        {
            bat.cam = playerCamera;
        }
        return weaponPivot;
    }
}