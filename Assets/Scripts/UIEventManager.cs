using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class UIEventManager : MonoBehaviour
{
    public static UIEventManager Instance;
    [SerializeField]
    RewardOptions options;
    [SerializeField]
    Transform contentTransform;
    [SerializeField]
    GameObject optionPrefab;

    int optionCount = 2;
    public UnityEvent onButtonClick;

    private void Awake()
    {
        UIEventManager.Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        var rewardOptions = options.rewards.GetRandomElements(optionCount);
        foreach (var opt in rewardOptions)
        {
            var card = Instantiate(optionPrefab, contentTransform).GetComponent<UIOptionButton>();
            card.gameObject.SetActive(true);
            card.SetupOption(opt);
        }
    }

    public void OnSelectOption(UnitData unitData, int count)
    {
        PlayerInventory.Instance.AddToInventory(unitData, count);
        SceneManager.LoadScene("SampleSceneMap");

    }

}
