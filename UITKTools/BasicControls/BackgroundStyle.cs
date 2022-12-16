using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools.BasicControls
{
    public class BackgroundStyle
    {
        public readonly Texture2D texture;
        public readonly Color? color;
        public readonly string text;

        public BackgroundStyle(Texture2D texture = null, Color? color = null, string text = "")
        {
            this.texture = texture;
            this.color = color;
            this.text = text;
        }

        public void applyStyle(VisualElement visualElement)
        {
            if (texture != null)
                visualElement.style.backgroundImage = new StyleBackground(texture);
            if (color != null)
                visualElement.style.backgroundColor = color.Value;
        }
        
        public void applyStyle(UnityEngine.UIElements.Button button)
        {
            if (texture != null)
                button.style.backgroundImage = new StyleBackground(texture);
            if (color != null)
               button.style.backgroundColor = color.Value;
            if (text != null)
                button.text = text;
        }
    }
}