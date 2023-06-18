using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "mystic-militia/UnitData", order = 0)]
public class UnitData : ScriptableObject {
    public string unitName;

    public int movementSpeed = 50;
    public int healthPoints = 100;
    public int damage = 10;
    public int shootingRange = 3;
    public GameObject rangedWeaponPrefab;

    public Sprite unitSprite;
}