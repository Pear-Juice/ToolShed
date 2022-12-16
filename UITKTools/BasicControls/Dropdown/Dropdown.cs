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

        public Action<Enum> changed;
        private Enum en;

        public Dropdown(VisualElement baseElement)
        {
            var enumItems = Enum.GetNames(typeof(TEnum));

            dropdownElement = ControlAssets.getControl(ControlAssets.Control.Dropdown, out Action release).Instantiate();
            release.Invoke();
            baseElement.Add(dropdownElement);
            itemListElement = dropdownElement.Q("ItemList");

            foreach (var enumItem in enumItems)
            {
                itemList.Add(new Item(enumItem, itemListElement, () =>
                {
                    toggle.toggleButton.text = enumItem;
                    toggle.setOff();

                    if (EnumExtensions.TryParse(typeof(TEnum), enumItem, out en)) changed?.Invoke(en);
                }));
            }

            toggle = new Toggle(dropdownElement.Q("Toggle"));

            IVisualElementScheduledItem opacityScheduledItem = null;

            toggle.on += () =>
            {
                itemListElement.style.display = DisplayStyle.Flex;
                opacityScheduledItem = itemListElement.schedule.Execute(() =>
                {
                    itemListElement.style.opacity = itemListElement.style.opacity.value + 10;
                }).Every(10).ForDuration(10);
            };

            toggle.off += () =>
            {
                itemListElement.style.display = DisplayStyle.None;
                itemListElement.style.opacity = 0;

                opacityScheduledItem?.Pause();
            };
        }

        public void setValue(string value)
        {
            if (EnumExtensions.TryParse(typeof(TEnum), value, out en))
            {
                changed?.Invoke(en);
                toggle.toggleButton.text = value;
            }
        }

        public VisualElement dropdownElement { get; }

        private class Item
        {
            private readonly UnityEngine.UIElements.Button buttonElement;
            private readonly VisualElement element;

            public Item(string label, VisualElement itemList, Action clicked)
            {
                element = ControlAssets.getControl(ControlAssets.Control.DropdownItem, out Action release).Instantiate();
                release.Invoke();
                
                element.style.width = new Length(100, LengthUnit.Percent);
                element.style.alignItems = new StyleEnum<Align>(Align.Center);

                buttonElement = element.Q<UnityEngine.UIElements.Button>("Item");
                buttonElement.clickable.clicked += () => { clicked?.Invoke(); };

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