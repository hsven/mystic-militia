using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//using UnityEngine.UIElements;

public class UIToggleSquadCard : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private TMP_Text squadName;
    [SerializeField]
    private Toggle toggle;

    private Vector2 originalSize;
    private int cardIndex;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;

        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(toggle);
        });
    }

    public void SetProperties(string name, int index)
    {
        squadName.text = name;
        cardIndex = index;
    }

    public bool GetIsOn()
    {
        return toggle.isOn;
    }

    public void SetIsOn(bool newOnValue)
    {
        toggle.isOn = newOnValue;
    }
    // Update is called once per frame
    void ToggleValueChanged(Toggle change)
    {
        if (change.isOn) 
        {
            Debug.Log(rectTransform.gameObject.name);
            rectTransform.DOSizeDelta(originalSize + new Vector2(0, 75), 0.5f).SetUpdate(true);

        }
        else
        {
            rectTransform.DOSizeDelta(originalSize, 0.5f).SetUpdate(true);
        }

        UIBattleSquadSelector.Instance.OnToggleChanges(cardIndex);
    }

}
