using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISquadManager : MonoBehaviour
{
    public static UISquadManager Instance;
    // public EventSystem eventSys;


    public GameObject unitUIEntry;

    public UISquad selectedSquad;
    
    public List<UIUnitCard> units = new List<UIUnitCard>();

    void Awake() {
        UISquadManager.Instance = this;    

    }

    // Start is called before the first frame update
    void Start()
    {
        BattleManager.Instance.PauseGame();
    }

    void Update() {
        var cur = EventSystem.current.currentSelectedGameObject;
        if (!cur) return;
        var sqd = cur.GetComponent<UISquad>();
        if(!sqd) return;

        selectedSquad = sqd;
        // if (!cur) return;

        // selectedSquad = cur;
    }

    public bool RegisterUnitToSquad(UIUnitCard unit) {
        // var cur = EventSystem.current.currentSelectedGameObject;
        if (!selectedSquad) return false;

        // var sqd = cur.GetComponent<UISquad>();
        // if (!sqd) return false;

        selectedSquad.AddUnit(unit);
        return true;
    }
}
