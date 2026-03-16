using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitLibrary
{
    public class UIGameView: MonoBehaviour
    {
        public UIDocument m_UIDoc;
        VisualElement m_Wrapper;
        List<VisualElement> indicators;

        void Awake()
        {
            indicators = new();
        }

        void Start()
        {
            m_Wrapper = m_UIDoc.rootVisualElement.Q<VisualElement>(UINames.VisualElementWrapper);
        }

        public VisualElement AddIndicator()
        {
           VisualElement el = new VisualElement() {
            name = $"Indicator_{indicators.Count}"
           };

           el.style.width = 50;
           el.style.height = 50;
           el.style.flexGrow = 0;
           el.style.flexShrink = 0;
           el.style.position = Position.Absolute;
           el.AddToClassList("panel-primary");

           Label textField = new Label()
           {
               text = indicators.Count.ToString(),
           };
           el.Add(textField);

           m_Wrapper.Add(el);

           indicators.Add(el);

           return el;
        }

        public void RemoveIndicator(VisualElement element)
        {
            if (indicators.Contains(element))
            {
                indicators.Remove(element);
            }
        }
    }
}