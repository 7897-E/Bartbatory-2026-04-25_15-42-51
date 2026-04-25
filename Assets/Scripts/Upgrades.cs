using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Data")]
public class Upgrades :ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
}
