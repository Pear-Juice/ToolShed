using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools.BasicControls
{
    public class ScaleStyle
    {
        public ScaleType widthScaleType { get; private set; }
        public ScaleType heightScaleType { get; private set; }

        public float widthScale { get; private set; }
        public float heightScale { get; private set; }

        public Float4? margin;
        public Float4? padding;
        
        private const float small = 40;
        private const float large = 50;
        
        public enum ScaleType
        {
            Pixel,
            Percent,
            Auto,
        }

        public enum ScalePreset
        {
            Auto,
            SmallBox,
            LargeBox,
            MaxBox,
            MaxSmallRect,
            MaxLargeRect,
            AutoSmallRect,
            AutoLargeRect,
        }
        
        public ScaleStyle(ScaleType widthScaleType, float widthScale, ScaleType heightScaleType, float heightScale, Float4? margin = null, Float4? padding = null)
        {
            setScale(widthScaleType, widthScale, heightScaleType, heightScale);
            this.margin = margin;
            this.padding = padding;
        }

        public ScaleStyle(ScalePreset scalePreset, Float4? margin = null, Float4? padding = null)
        {
            switch (scalePreset)
            {
                case ScalePreset.Auto:
                    setScale(ScaleType.Auto, 0, ScaleType.Auto, 0); break;
                case ScalePreset.SmallBox:
                    setScale(ScaleType.Pixel, small, ScaleType.Pixel, small); break;
                case ScalePreset.LargeBox:
                    setScale(ScaleType.Pixel, large, ScaleType.Pixel, large); break;
                case ScalePreset.MaxBox:
                    setScale(ScaleType.Percent, 100, ScaleType.Percent, 100); break;
                case ScalePreset.MaxSmallRect:
                    setScale(ScaleType.Percent, 100, ScaleType.Pixel, small); break;
                case ScalePreset.MaxLargeRect:
                    setScale(ScaleType.Percent, 100, ScaleType.Pixel, large); break;
                case ScalePreset.AutoSmallRect:
                    setScale(ScaleType.Auto, 0, ScaleType.Pixel, small); break;
                case ScalePreset.AutoLargeRect:
                    setScale(ScaleType.Auto, 0, ScaleType.Pixel, large); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scalePreset), scalePreset, null);
            }
            
            this.margin = margin;
            this.padding = padding;
        }

        private void setScale(ScaleType widthScaleType, float widthScale, ScaleType heightScaleType, float heightScale)
        {
            this.widthScaleType = widthScaleType;
            this.heightScaleType = heightScaleType;

            if (widthScaleType == ScaleType.Percent)
                Mathf.Clamp(widthScale, 0, 100);

            this.widthScale = widthScale;
            
            if (heightScaleType == ScaleType.Percent)
                Mathf.Clamp(heightScale, 0, 100);

            this.heightScale = heightScale;
        }

        public void applyStyle(VisualElement element)
        {

            if (widthScaleType == ScaleType.Auto)
                element.style.width = new StyleLength(StyleKeyword.Auto);
            else
                element.style.width = new Length(widthScale, widthScaleType == ScaleType.Pixel ? LengthUnit.Pixel : LengthUnit.Percent);

            if (heightScaleType == ScaleType.Auto)
                element.style.height = new StyleLength(StyleKeyword.Auto);
            else 
                element.style.height = new Length(heightScale, heightScaleType == ScaleType.Pixel ? LengthUnit.Pixel : LengthUnit.Percent);

            

            if (margin != null)
            {
                if (margin.Value.all != null)
                {
                    element.style.marginTop = new Length(margin.Value.all.Value);
                    element.style.marginBottom = new Length(margin.Value.all.Value);
                    element.style.marginLeft = new Length(margin.Value.all.Value);
                    element.style.marginRight = new Length(margin.Value.all.Value);
                }
                else
                {
                    element.style.marginTop = new Length(margin.Value.top);
                    element.style.marginBottom = new Length(margin.Value.bottom);
                    element.style.marginLeft = new Length(margin.Value.left);
                    element.style.marginRight = new Length(margin.Value.right);
                }
            }

            if (padding != null)
            {
                if (padding.Value.all != null)
                {
                    element.style.paddingTop = new Length(padding.Value.all.Value);
                    element.style.paddingBottom = new Length(padding.Value.all.Value);
                    element.style.paddingLeft = new Length(padding.Value.all.Value);
                    element.style.paddingRight = new Length(padding.Value.all.Value);
                }
                else
                {
                    element.style.paddingTop = new Length(padding.Value.top);
                    element.style.paddingBottom = new Length(padding.Value.bottom);
                    element.style.paddingLeft = new Length(padding.Value.left);
                    element.style.paddingRight = new Length(padding.Value.right);
                }
            }
        }
        
        public struct Float4
        {
            public float? all;
            public float top;
            public float bottom;
            public float left;
            public float right;
        }

    }
}