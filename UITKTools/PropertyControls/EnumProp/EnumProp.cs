using System;
using ToolShed.UITKTools.BasicControls;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools.PropertyControls
{
    public class EnumProp<T>
    {
        private readonly Dropdown<T> dropdown;
        private readonly VisualElement dropdownContainerElement;
        private readonly VisualElement element;
        public Action<Enum> changed;

        public EnumProp(VisualElement baseElement)
        {
            element = ControlAssets.getControl(ControlAssets.Control.EnumProp, out Action release).Instantiate();
            release.Invoke();
            baseElement.Add(element);

            dropdownContainerElement = element.Q("DropdownContainer");
            dropdown = new Dropdown<T>(dropdownContainerElement);
            dropdown.dropdownElement.style.width = new Length(100, LengthUnit.Percent);

            dropdown.changed += en => changed?.Invoke(en);
        }

        public void setValue(string value)
        {
            dropdown.setValue(value);
        }
    }
}