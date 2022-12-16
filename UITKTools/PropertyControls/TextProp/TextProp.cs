using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools.PropertyControls
{
    public class TextProp
    {
        public readonly TextType type;
        private VisualElement textPropElement;
        private TextField textField;

        private string previousText;

        public string value { get; private set; }
        public Action<string> OnChange;
        
        public enum TextType
        {
            String,
            Int,
            Float
        }
        
        public TextProp(VisualElement baseElement, string label, TextType type, string defaultValue = "")
        {
            this.type = type;

            textPropElement = ControlAssets.getControl(ControlAssets.Control.TextProp, out Action release).Instantiate();
            release.Invoke();
            textField = textPropElement.Q<TextField>("TextField");

            textPropElement.Q<Label>("Label").text = label;
            
            baseElement.Add(textPropElement);

            textField.value = defaultValue != "" ? defaultValue : getBlankValue(true);

            textField.RegisterValueChangedCallback((e) =>
            {
                previousText = e.previousValue;
                setValue(e.newValue, true, false);
            });
        }

        public void setValue(string text, bool notify = false, bool changeOnElement = true)
        {
            if (checkValid(text))
            {
                string newValue = text != "" ? text : getBlankValue();

                value = newValue;
                if (notify) OnChange?.Invoke(newValue);
                if (changeOnElement) textField.SetValueWithoutNotify(newValue);
            }
            else
            {
                textField.SetValueWithoutNotify(previousText);
            }
        }

        private string getBlankValue(bool emptyIsDot = false)
        {
            switch (this.type)
            {
                case TextType.String:
                    return emptyIsDot ? "..." : "";
                case TextType.Int:
                    return "0";
                case TextType.Float:
                    return "0.0";
            }

            return "";
        }

        private bool checkValid(string text)
        {
            if (text == "") return true;            
            
            switch (type)
            {
                case TextType.String:
                    return true;
                case TextType.Int:
                    return int.TryParse(text, out _);
                case TextType.Float:
                    return float.TryParse(text, out _);
            }

            return false;
        }
    }
}