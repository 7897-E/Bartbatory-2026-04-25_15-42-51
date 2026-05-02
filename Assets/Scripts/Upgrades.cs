using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Data")]
public class Upgrades : ScriptableObject
{
    public string upgradeName;

    [TextArea]
    public string description;

    public UpgradeChange[] changes;

    public void Apply(PlayerController playerController, BatScript weapon, int level = 1)
    {
        foreach (var change in changes)
        {
            ApplyChange(playerController, change, weapon, level);
        }
    }

    private void ApplyChange(PlayerController player, UpgradeChange change, BatScript weapon, int level)
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
            case UpgradeAttribute.FireRate:
                weapon.fireRate = ApplyValue(weapon.fireRate, change, level);
                break;
            case UpgradeAttribute.Damage:
                weapon.damage = (int)ApplyValue(weapon.damage, change, level);
                break;
            case UpgradeAttribute.ProjectileSpeed:
                weapon.bulletSpeed = ApplyValue(weapon.bulletSpeed, change, level);
                break;
            case UpgradeAttribute.MaxHits:
                weapon.MaxHits = (int)ApplyValue(weapon.MaxHits, change, level);
                break;
            case UpgradeAttribute.Bounces:
                weapon.bounces = (int)ApplyValue(weapon.bounces, change, level);
                break;
        }
    }

    private float ApplyValue(float currentValue, UpgradeChange change, int level)
    {
        float scaledValue = change.value * level;
        switch (change.changeType)
        {
            case UpgradeChangeType.Add:
                return currentValue + scaledValue;

            case UpgradeChangeType.Multiply:
                return currentValue * scaledValue;

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
    Bounces

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