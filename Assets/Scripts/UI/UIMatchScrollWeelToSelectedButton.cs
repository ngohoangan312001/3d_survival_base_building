using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AN
{
    public class UIMatchScrollWeelToSelectedButton : MonoBehaviour
    {
        [SerializeField] private GameObject currentSelected;
        [SerializeField] private GameObject previouslySelected;
        [SerializeField] private RectTransform currentSelectdTranform;

        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private ScrollRect scrollRect;

        private void Update()
        {
            currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected != null)
            {
                if (currentSelected != previouslySelected)
                {
                    previouslySelected = currentSelected;
                    currentSelectdTranform = currentSelected.GetComponent<RectTransform>();
                    SnapTo(currentSelectdTranform);
                }
            }
        }

        private void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();

            Vector2 newPosition = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) -
                                  (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
            //Lock position on y axis only
            newPosition.x = 0;

            contentPanel.anchoredPosition = newPosition;


        }
    }
}
