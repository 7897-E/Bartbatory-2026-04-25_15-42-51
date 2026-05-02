using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Data")]
public class Upgrades : ScriptableObject
{
    public string upgradeName;

    [TextArea]
    public string description;

    public UpgradeChange[] changes;

    public void Apply(PlayerController playerController, Weapons weapon, int level = 1)
    {
        foreach (var change in changes)
        {
            ApplyChange(playerController, change, weapon, level);
        }
    }

    private void ApplyChange(PlayerController player, UpgradeChange change, Weapons weapon, int level)
    {
        // Update player stats (not weapon-specific)
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

        // Update the active weapon instance in the scene if it exists
        if (player.weaponInstances.TryGetValue(weapon, out BatScript batInstance))
        {
            switch (change.attribute)
            {
                case UpgradeAttribute.FireRate:
                    // Fire rate decreases by 0.1 per upgrade (faster fire)
                    batInstance.fireRate = Mathf.Max(0.05f, batInstance.fireRate - 0.1f * level);
                    break;
                case UpgradeAttribute.Damage:
                    batInstance.damage = (int)ApplyValue(batInstance.damage, change, level);
                    break;
                case UpgradeAttribute.ProjectileSpeed:
                    batInstance.bulletSpeed = ApplyValue(batInstance.bulletSpeed, change, level);
                    break;
                case UpgradeAttribute.MaxHits:
                    batInstance.MaxHits = (int)ApplyValue(batInstance.MaxHits, change, level);
                    break;
                case UpgradeAttribute.Bounces:
                    batInstance.bounces = (int)ApplyValue(batInstance.bounces, change, level);
                    break;
            }
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