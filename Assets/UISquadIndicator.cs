using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class UISquadIndicator : MonoBehaviour
{
    public RectTransform canvasRect;

    public RectTransform rectTransform;

    public Image indicatorImage;
    public TMP_Text text;

    [SerializeField]
    private List<Transform> targetTransforms;
    
    // Start is called before the first frame update
    public void StartIndicator(List<Transform> targets, int squadNumber)
    {
        targetTransforms = targets;
        text.text = "#" + squadNumber;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransforms.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        var centerTransform = targetTransforms[targetTransforms.Count / 2];
        if(centerTransform == null)
        {
            targetTransforms.Remove(centerTransform);
            return;
        }

        if (IsOffScreen(centerTransform.position))
        {
            indicatorImage.enabled = true;
            text.enabled = true;

            var screenPos = Camera.main.WorldToScreenPoint(centerTransform.position);

            var croppedMax = canvasRect.rect.max * 0.9f;
            var croppedMin = canvasRect.rect.min * 0.9f;

            //Adjust position to match the canvas (since screen center == bottom left corner, while canvas center == middle)
            screenPos -= new Vector3(canvasRect.rect.max.x, canvasRect.rect.max.y, 0);

            if (screenPos.x > croppedMax.x) screenPos.x = croppedMax.x;
            else if(screenPos.x < croppedMin.x) screenPos.x = croppedMin.x;

            if (screenPos.y > croppedMax.y) screenPos.y = croppedMax.y;
            else if (screenPos.y < croppedMin.y) screenPos.y = croppedMin.y;

            rectTransform.anchoredPosition3D = screenPos;
        }
        else
        {
            indicatorImage.enabled = false;
            text.enabled = false;
        }
    }

    public bool IsOffScreen(Vector3 position)
    {
        var screenPos = Camera.main.WorldToScreenPoint(position);
        var onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f && screenPos.y < Screen.height;
        return !onScreen;
    }
}
