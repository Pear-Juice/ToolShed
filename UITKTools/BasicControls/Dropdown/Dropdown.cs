using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools.BasicControls
{
    public class Dropdown<TEnum>
    {
        private readonly List<Item> itemList = new();
        private readonly VisualElement itemListElement;
        private readonly Toggle toggle;
        private VisualElement baseElement;
        public VisualElement dropdownElement { get; }

        public Action<Enum> changed;
        private Enum en;

        public Dropdown(VisualElement baseElement, ScaleStyle toggleScale = null, ScaleStyle itemScale = null, string defaultText = "Select")
        {
            var enumItems = Enum.GetNames(typeof(TEnum));

            dropdownElement = ControlAssets.getControl(ControlAssets.Control.Dropdown, out Action release).Instantiate();
            release.Invoke();
            baseElement.Add(dropdownElement);
            
            itemListElement = dropdownElement.Q("ItemList");

            toggleScale ??= new ScaleStyle(ScaleStyle.ScalePreset.MaxSmallRect);
            toggle = new Toggle(dropdownElement.Q("ToggleContainer"), toggleScale, new BackgroundStyle(text: defaultText));

            itemScale ??= new ScaleStyle(ScaleStyle.ScalePreset.MaxSmallRect);
            foreach (var enumItem in enumItems)
            {
                itemList.Add(new Item(enumItem, itemListElement, itemScale, () =>
                {
                    toggle.toggleButton.text = enumItem;
                    toggle.setOff(true);

                    if (EnumExtensions.TryParse(typeof(TEnum), enumItem, out en)) changed?.Invoke(en);
                }));
            }

            IVisualElementScheduledItem opacityScheduledItem = null;

            toggle.on += () =>
            {
                itemListElement.setActive();
                opacityScheduledItem = itemListElement.schedule.Execute(() =>
                {
                    itemListElement.style.opacity = itemListElement.style.opacity.value + 10;
                }).Every(10).ForDuration(10);
            };

            toggle.off += () =>
            {
                itemListElement.setInactive();
                itemListElement.style.opacity = 0;

                opacityScheduledItem?.Pause();
            };
        }

        public void setValue(string value, bool notify = false)
        {
            if (EnumExtensions.TryParse(typeof(TEnum), value, out en))
            {
                if (notify)
                    changed?.Invoke(en);
                toggle.toggleButton.text = value;
            }
        }

        private class Item
        {
            private readonly UnityEngine.UIElements.Button buttonElement;
            private readonly VisualElement element;

            public Item(string label, VisualElement itemList, ScaleStyle scale, Action clicked)
            {
                element = ControlAssets.getControl(ControlAssets.Control.DropdownItem, out Action release).Instantiate();
                release.Invoke();
                
                buttonElement = element.Q<UnityEngine.UIElements.Button>("Item");
                buttonElement.clickable.clicked += () => { clicked?.Invoke(); };
                
                element.style.alignItems = new StyleEnum<Align>(Align.Center);
                scale.applyStyle(buttonElement);
                scale.applyStyle(element);

                itemList.Add(element);

                setLabel(label);
            }

            public void setLabel(string text)
            {
                buttonElement.text = text;
            }
        }
    }
}