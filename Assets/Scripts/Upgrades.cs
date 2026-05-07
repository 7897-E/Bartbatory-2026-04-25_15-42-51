using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Data")]
public class Upgrades : ScriptableObject
{
    public string upgradeName;

    [TextArea]
    public string description;

    public UpgradeChange[] changes;

    public void Apply(PlayerController playerController, Weapons weapon, int level)
    {
        foreach (var change in changes)
        {
            if(level <= 0)
            {
                level = 1;
            }
            ApplyChange(playerController, change, weapon, level);
        }
    }

    private void ApplyChange(PlayerController player, UpgradeChange change, Weapons weapon, int level)
    {
        switch (change.attribute)
        {
            case UpgradeAttribute.MoveSpeed:
                player.moveSpeed = ApplyValue(player.moveSpeed, change, level);
                break;
            case UpgradeAttribute.MaxHealth:
                player.maxHealth = (int)ApplyValue(player.maxHealth, change, level);
                break;
            case UpgradeAttribute.CurrentHealth:
                player.currentHealth = (int)ApplyValue(player.currentHealth, change, level);
                break;
        }

        if (weapon != null && player.weaponInstances.TryGetValue(weapon, out BatScript batInstance))
        {
            switch (change.attribute)
            {
                case UpgradeAttribute.FireRate:
                    batInstance.fireRate = Mathf.Max(0.05f, batInstance.fireRate - 0.1f);
                    batInstance.currentLevel = level;
                    break;
                case UpgradeAttribute.Damage:
                    batInstance.projectileDamage = (int)ApplyValue(batInstance.projectileDamage, change, level);
                    batInstance.currentLevel = level;
                    break;
                case UpgradeAttribute.ProjectileSpeed:
                    batInstance.bulletSpeed = ApplyValue(batInstance.bulletSpeed, change, level);
                    batInstance.currentLevel = level;
                    break;
                case UpgradeAttribute.MaxHits:
                    batInstance.MaxHits = (int)ApplyValue(batInstance.MaxHits, change, level);
                    batInstance.currentLevel = level;
                    break;
                case UpgradeAttribute.Bounces:
                    batInstance.bounces = (int)ApplyValue(batInstance.bounces, change, level);
                    batInstance.currentLevel = level;
                    break;
                case UpgradeAttribute.Range:
                    batInstance.meleeRange = ApplyValue(batInstance.meleeRange, change, level);
                    batInstance.currentLevel = level;
                    break;
                case UpgradeAttribute.Spread:
                    batInstance.shotgunSpread = ApplyValue(batInstance.shotgunSpread, change, level);
                    batInstance.currentLevel = level;
                    break;
                case UpgradeAttribute.PelletCount:
                    batInstance.shotgunPellets = (int)ApplyValue(batInstance.shotgunPellets, change, level);
                    batInstance.currentLevel = level;
                    break;
            }
        }
    }

    private float ApplyValue(float currentValue, UpgradeChange change, int level)
    {
        float scaledValue = change.value * Math.Max(0.2f * level, 1f);
        switch (change.changeType)
        {
            case UpgradeChangeType.Add:
                return currentValue + scaledValue;

            case UpgradeChangeType.Multiply:
                return currentValue * change.value;

            case UpgradeChangeType.Set:
                return scaledValue;

            default:
                return currentValue;
        }
    }
}

public enum UpgradeAttribute
{
    MoveSpeed,
    Damage,
    MaxHealth,
    CurrentHealth,
    FireRate,
    ProjectileSpeed,
    MaxHits,
    Bounces,
    Range,
    Spread,
    PelletCount

}

public enum UpgradeChangeType
{
    Add,
    Multiply,
    Set
}

[System.Serializable]
public class UpgradeChange
{
    public UpgradeAttribute attribute;
    public UpgradeChangeType changeType;
    public float value;
}
