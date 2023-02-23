﻿using System;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools.BasicControls
{
    public class Toggle
    {
        private VisualElement baseElement;
        public readonly VisualElement toggleElement;
        public readonly UnityEngine.UIElements.Button toggleButton;
        public Action off;
        public Action on;

        public bool value;

        /// <summary>
        /// Instantiate scale button
        /// </summary>
        /// <param name="baseElement">Element to create toggle on</param>
        /// <param name="scaleStyle">Scale of new toggle</param>
        /// <param name="backgroundStyle">Background of new toggle</param>
        public Toggle(VisualElement baseElement, ScaleStyle scaleStyle = null, BackgroundStyle backgroundStyle = null)
        {
            this.baseElement = baseElement;
            toggleElement = ControlAssets.getControl(ControlAssets.Control.Toggle, out Action release).Instantiate();
            release.Invoke();
            
            toggleButton = toggleElement.Q<UnityEngine.UIElements.Button>("Button");

            scaleStyle?.applyStyle(toggleElement);
            backgroundStyle?.applyStyle(toggleButton);
            
            baseElement.Add(toggleElement);
            
            addCallbacks();
        }

        /// <summary>
        /// Load toggle from existing toggle element
        /// </summary>
        /// <param name="toggleElement">Element to create toggle with</param>
        /// /// <param name="backgroundStyle">Background of new toggle</param>
        public Toggle(VisualElement toggleElement, BackgroundStyle backgroundStyle = null)
        {
            this.toggleElement = toggleElement;
            this.toggleButton = toggleElement.Q<UnityEngine.UIElements.Button>("Button");

            backgroundStyle?.applyStyle(toggleButton);

            addCallbacks();
        }
        
        public Toggle(UnityEngine.UIElements.Button toggleButton, BackgroundStyle backgroundStyle = null)
        {
            this.toggleButton = toggleButton;
            backgroundStyle?.applyStyle(toggleButton);

            addCallbacks();
        }

        private void addCallbacks()
        {
            toggleButton.clickable.clicked += () =>
            {
                if (!value)
                {
                    setOn(true);
                }
                else
                {
                    setOff(true);
                }
            };
        }

        public void setOn(bool notify = false)
        {
            if (notify)
                on?.Invoke();
            value = true;
            
            toggleButton.AddToClassList("button-accent");
            toggleButton.AddToClassList("text-accent");
            toggleButton.RemoveFromClassList("button");
            toggleButton.RemoveFromClassList("text");
        }

        public void setOff(bool notify = false)
        {
            if (notify)
                off?.Invoke();
            value = false;
            
            toggleButton.AddToClassList("button");
            toggleButton.AddToClassList("text");
            toggleButton.RemoveFromClassList("button-accent");
            toggleButton.RemoveFromClassList("text-accent");
        }

        public void setActive()
        {
            toggleElement?.setActive();
            toggleButton?.setActive();
        }

        public void setInactive()
        {
            toggleElement?.setInactive();
            toggleButton?.setInactive();
        }
    }
}