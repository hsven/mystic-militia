using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventory : MonoBehaviour
{
    public bool newGame;

    [Serializable]
    public class UnitEntry
    {
        public UnitData unitData;
        public int quantity;
    }

    [Serializable]
    public class SquadEntry 
    {
        public List<UnitEntry> unitEntries = new List<UnitEntry>();
    }

    public static PlayerInventory Instance;

    public List<UnitEntry> playerUnits = new List<UnitEntry>();

    public List<SquadEntry> playerSquads = new List<SquadEntry>();

    public int battlesFought = 0;
    public int battlesTotal = 0;
    public bool finalBoss = false;

    void Awake() {
        PlayerInventory.Instance = this; 
        //TODO: Make this object permanent regardless of scene, when the overall gameplay loop is in place   
        DontDestroyOnLoad(this.gameObject);
    }

    public void CreateSquad() {
        playerSquads.Add(new SquadEntry());
    }

    public bool MoveUnitFromInventoryToSquad(UnitEntry unit, SquadEntry squad) {
        if(unit.quantity == 0) return false;

        var unitEntryPosInSquad = squad.unitEntries.FindIndex(0, x => x.unitData.unitName == unit.unitData.unitName);
        if (unitEntryPosInSquad != -1) {
            squad.unitEntries[unitEntryPosInSquad].quantity++;
        } 
        else {
            var newEntry = new UnitEntry();
            newEntry.unitData = unit.unitData;
            newEntry.quantity++;
            squad.unitEntries.Add(newEntry);
        }
        unit.quantity--;
        return true;
    }

    public bool MoveUnitFromSquadToInventory(UnitEntry unit, SquadEntry squad) {
        if(unit.quantity == 0) return false;

        var unitEntryPosInSquad = squad.unitEntries.FindIndex(0, x => x.unitData.unitName == unit.unitData.unitName);
        var unitEntryPosInInventory = playerUnits.FindIndex(0, x => x.unitData.unitName == unit.unitData.unitName);
        if (unitEntryPosInInventory != -1) {
            playerUnits[unitEntryPosInInventory].quantity++;
            squad.unitEntries[unitEntryPosInSquad].quantity--;

            if (squad.unitEntries[unitEntryPosInSquad].quantity == 0) 
            { 
                squad.unitEntries.RemoveAt(unitEntryPosInSquad);
            }
        } 
        return true;
    }

    public void AddToInventory(UnitData unit, int quantity){
        var unitEntryPosInInventory = playerUnits.FindIndex(0, x => x.unitData.unitName == unit.unitName);
        if (unitEntryPosInInventory != -1) {
            playerUnits[unitEntryPosInInventory].quantity+=quantity;
        }
        else {
            var newEntry = new UnitEntry();
            newEntry.unitData = unit;
            newEntry.quantity+=quantity;
            playerUnits.Add(newEntry);
        } 
    }
}