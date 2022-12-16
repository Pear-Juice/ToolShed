using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools.BasicControls
{
    public class Tabs
    {
        /// <summary>
        /// string is name of item
        /// </summary>
        public Action<string> clicked;

        private Button lastClickedButton;
        private List<Button> buttons = new List<Button>();
        public Tabs(VisualElement baseElement, List<string> items, ScaleStyle buttonScaleStyle, int? preColorIndex = null)
        {
            var tabContainer = ControlAssets.getControl(ControlAssets.Control.Tabs, out Action tabRelease).Instantiate().Q("Container");
            tabRelease.Invoke();
        
            baseElement.Add(tabContainer);

            foreach (var item in items)
            {
                var button = new Button(tabContainer, Button.Type.Normal, item, buttonScaleStyle);
                buttons.Add(button);

                button.OnClick += () =>
                {
                    clicked?.Invoke(item);

                    lastClickedButton?.setType(Button.Type.Normal);
                    button.setType(Button.Type.Accent);

                    lastClickedButton = button;
                };
            }
            
            if (preColorIndex != null && preColorIndex < items.Count)
            {
                buttons[preColorIndex.Value].setType(Button.Type.Accent);
                lastClickedButton = buttons[preColorIndex.Value];
            }
        }
    }
}

