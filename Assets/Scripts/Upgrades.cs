using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Data")]
public class Upgrades : ScriptableObject
{
    public string upgradeName;

    [TextArea]
    public string description;

    public UpgradeChange[] changes;

    public void Apply(PlayerController playerController, BatScript weapon)
    {
        foreach (var change in changes)
        {
            ApplyChange(playerController, change, weapon);
        }
    }

    private void ApplyChange(PlayerController player, UpgradeChange change, BatScript weapon)
    {
        switch (change.attribute)
        {
            case UpgradeAttribute.MoveSpeed:
                player.moveSpeed = ApplyValue(player.moveSpeed, change);
                break;
            case UpgradeAttribute.MaxHealth:
                player.maxHealth = (int)ApplyValue(player.maxHealth, change);
                break;
            case UpgradeAttribute.CurrentHealth:
                player.currentHealth = (int)ApplyValue(player.currentHealth, change);
                break;
            case UpgradeAttribute.FireRate:
                weapon.fireRate = ApplyValue(weapon.fireRate, change);
                break;
            case UpgradeAttribute.Damage:
                weapon.damage = (int)ApplyValue(weapon.damage, change);
                break;
            case UpgradeAttribute.ProjectileSpeed:
                weapon.bulletSpeed = ApplyValue(weapon.bulletSpeed, change);
                break;
            case UpgradeAttribute.MaxHits:
                weapon.MaxHits = (int)ApplyValue(weapon.MaxHits, change);
                break;
        }
    }

    private float ApplyValue(float currentValue, UpgradeChange change)
    {
        switch (change.changeType)
        {
            case UpgradeChangeType.Add:
                return currentValue + change.value;

            case UpgradeChangeType.Multiply:
                return currentValue * change.value;

            case UpgradeChangeType.Set:
                return change.value;

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
    MaxHits

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