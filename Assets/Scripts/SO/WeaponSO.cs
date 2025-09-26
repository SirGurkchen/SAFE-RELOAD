using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObjects/Weapon Data")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public int maxAmmo;
    public float reloadThreshhold;
}