using System;
using System.Collections.Generic;
using ToolShed.UITKTools.BasicControls;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools
{
    public class StateMachine
    {
        readonly List<VisualElement> stateElements = new List<VisualElement>();
        private VisualElement currentStateElement;
        
        public StateMachine(VisualElement baseElement, List<Action<VisualElement>> states)
        {
            foreach (var action in states)
            {
                var stateElement = new VisualElement();
                new ScaleStyle(ScaleStyle.ScalePreset.Auto).applyStyle(stateElement);
                
                stateElements.Add(stateElement);
                baseElement.Add(stateElement);
                stateElement.style.display = DisplayStyle.None;
                
                action.Invoke(stateElement);
            }
        }

        public void setState(int index)
        {
            if (index >= stateElements.Count) return;
                
                
            if (currentStateElement != null)
                currentStateElement.style.display = DisplayStyle.None;
            
            stateElements[index].style.display = DisplayStyle.Flex;
            currentStateElement = stateElements[index];
        }
    }
}