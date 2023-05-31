using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "mystic-militia/UnitData", order = 0)]
public class UnitData : ScriptableObject {
    public string unitName;

    public int healthPoints;
    public int damage;
}