using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISquadManager : MonoBehaviour
{
    class UISquadInfo {
        public UISquadCard squadCard;
        public List<UIUnitCard> unitCards = new List<UIUnitCard>();
    }

    public static UISquadManager Instance;
    // public EventSystem eventSys;


    public GameObject unitUIEntry;
    public RectTransform unitInventoryRect;
    //Solely for easier updating
    private List<UIUnitCard> unitInventoryCards = new List<UIUnitCard>();

    public GameObject squadUIEntry;
    public RectTransform squadInventoryRect;
    //Solely for easier updating
    private List<UISquadInfo> squadCards = new List<UISquadInfo>();
    public UISquadCard selectedSquad;


    void Awake() {
        UISquadManager.Instance = this;    

    }

    // Start is called before the first frame update
    void Start()
    {    
        RefreshUnitInventoryPanel();
        RefreshSquadPanel();
    }

    void Update() {
        var cur = EventSystem.current.currentSelectedGameObject;
        if (!cur) return;
        var sqd = cur.GetComponent<UISquadCard>();
        if(!sqd) return;

        selectedSquad = sqd;
    }

    public bool RegisterUnitToSquad(UIUnitCard unit) {
        if (!selectedSquad) return false;

        if(PlayerInventory.Instance.MoveUnitFromInventoryToSquad(unit.entry, selectedSquad.entry)) {
            unit.UpdateCardData();
            selectedSquad.UpdateCardData();
            return true;
        }
        return false;
    }

    public bool RemoveUnitFromSquad(UIUnitCard unit) {
        var squad = unit.GetComponentInParent<UISquadCard>();
        if (!squad) return false;

        if(PlayerInventory.Instance.MoveUnitFromSquadToInventory(unit.entry, squad.entry)) {
            if(unit.entry.quantity <= 0) {
                Destroy(unit.gameObject);
            } else unit.UpdateCardData();

            foreach (var item in unitInventoryCards)
            {
                item.UpdateCardData();
            }
            return true;
        }
        return false;
    }

    public void OnCreateNewSquad() {
        PlayerInventory.Instance.CreateSquad();
        RefreshSquadPanel();
    }

    void RefreshUnitInventoryPanel() {
        for (int i = 0; i < unitInventoryRect.childCount; i++)
        {
            Destroy(unitInventoryRect.GetChild(i).gameObject);
        }

        foreach (var entry in PlayerInventory.Instance.playerUnits)
        {
            var unitUI = Instantiate(unitUIEntry, unitInventoryRect).GetComponent<UIUnitCard>();
            unitUI.CreateUICard(entry, true);
            unitInventoryCards.Add(unitUI);
        }
    }

    void RefreshSquadPanel() {
        for (int i = 0; i < squadInventoryRect.childCount; i++)
        {
            //TODO: Revisit this
            if(squadInventoryRect.GetChild(i).gameObject.CompareTag("GameController")) continue;

            Destroy(squadInventoryRect.GetChild(i).gameObject);
        }

        int cnt = 0;
        foreach (var squad in PlayerInventory.Instance.playerSquads)
        {
            var squadUI = Instantiate(squadUIEntry, squadInventoryRect).GetComponent<UISquadCard>();
            squadUI.SetName("Squad " + cnt.ToString());
            squadUI.CreateUICard(squad);
            cnt++;
        }
        DataManager.Instance.gameMetrics.squadCount=cnt;
    }
}
