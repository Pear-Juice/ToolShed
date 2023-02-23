﻿using System;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools.BasicControls
{
    public class Button
    {
        private VisualElement baseElement;
        private VisualElement buttonElement;
        public UnityEngine.UIElements.Button button;
        private ScaleStyle scaleStyle;

        public Action OnClick;

        public enum Type
        {
            Normal,
            Accent
        }

        public Button(VisualElement baseElement, Type type, string label, ScaleStyle scaleStyle = null, BackgroundStyle backgroundStyle = null)
        {
            this.baseElement = baseElement;
            this.scaleStyle = scaleStyle;

            buttonElement = ControlAssets.getControl(ControlAssets.Control.Button, out Action release).Instantiate();
            release.Invoke();
            
            baseElement.Add(buttonElement);

            button = buttonElement.Q<UnityEngine.UIElements.Button>("Button");
            button.text = label;
            
            scaleStyle?.applyStyle(buttonElement);
            new ScaleStyle(ScaleStyle.ScalePreset.MaxBox).applyStyle(button);
            backgroundStyle?.applyStyle(button);

            setType(type);

            button.clickable.clicked += (() => OnClick?.Invoke());
        }

        public void setType(Type type)
        {
            switch (type)
            {
                case Type.Normal:
                    button.AddToClassList("button");
                    button.AddToClassList("text");
                    button.RemoveFromClassList("button-accent");
                    button.RemoveFromClassList("text-accent");
                    break;
                case Type.Accent:
                    button.RemoveFromClassList("button");
                    button.RemoveFromClassList("text");
                    button.AddToClassList("button-accent");
                    button.AddToClassList("text-accent");
                    break;
            }
        }

        public void setText(string text)
        {
            button.text = text;
        }
    }
}