using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools
{
    public class Point
    {
        private Vector2 loc;

        public Action<Vector2> OnMove;
        private Vector2 size;
        public VisualElement spriteElement;
        public VisualElement templateElement;

        public Point(Vector2 size, Vector2 loc)
        {
            Size = size;
            Loc = loc;
        }

        public Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                update();
            }
        }

        public Vector2 Loc
        {
            get => loc;
            set
            {
                loc = value;
                update();
                OnMove?.Invoke(value);
            }
        }

        public void update()
        {
            if (templateElement == null)
            {
                templateElement = Primatives.createPoint(Size, Loc);
                spriteElement = templateElement.Q<VisualElement>("Point");
            }

            else
            {
                Primatives.setElementTransform(spriteElement, Size, Loc);
            }
        }
    }
}