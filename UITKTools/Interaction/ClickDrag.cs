using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools
{
    public class ClickDrag
    {
        public Action<Vector2> OnClick;
        /// <summary>
        /// Vector2: Position of mouse, Vector2: Change in position of mouse
        /// </summary>
        public Action<Vector2, Vector2> OnDrag;
        public Action<Vector2> OnStartDrag;
        public Action<Vector2> OnStopDrag;
        private Drag drag;

        /// <summary>
        /// Event registration separation of clicking and dragging on an element.
        /// </summary>
        /// <param name="element">Element to read events on</param>
        /// <param name="root">Element that read events from outside areas</param>
        /// <param name="startDragOnCreation">Changes whether drag should start immediatly on instantiation</param>
        public ClickDrag(VisualElement element, VisualElement root, bool startDragOnCreation = false)
        {
            var moved = false;
            drag = new Drag(element, root, startDragOnCreation);

            var lastClickedPos = new Vector2();

            drag.OnPointerDown += pos => { lastClickedPos = pos; };

            drag.OnStartDrag += (loc) => OnStartDrag?.Invoke(loc);

            drag.OnDrag += (pos, dPos) =>
            {
                moved = true;
                OnDrag?.Invoke(pos, dPos);
            };

            drag.OnPointerUp += loc =>
            {
                if (!moved)
                    OnClick?.Invoke(lastClickedPos);
                else
                    OnStopDrag?.Invoke(loc);

                moved = false;
            };
        }

        public void kill()
        {
            drag.kill();
            drag = null;
        }
    }
}