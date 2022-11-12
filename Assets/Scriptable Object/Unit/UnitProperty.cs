using UnityEngine;

[CreateAssetMenu(fileName = "UnitProperty", menuName = "ScriptableObject/UnitProperty")]
public class UnitProperty : ScriptableObject
{
    public int maxHp = 10;
    public int armor = 5;
    public int damage = 3;
    public int movement = 6;
}
