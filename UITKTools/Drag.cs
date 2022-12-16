using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools
{
    public class Drag
    {
        private readonly VisualElement root;
        private Vector2 dist;
        private VisualElement element;
        
        /// <summary>
        /// (Position, Delta Position)
        /// </summary>
        public Action<Vector2, Vector2> OnDrag = null;
        /// <summary>
        /// (Position)
        /// </summary>
        public Action<Vector2> OnStartDrag = null;
        /// <summary>
        /// (Position)
        /// </summary>
        public Action<Vector2> OnPointerDown = null;
        /// <summary>
        /// (Position)
        /// </summary>
        public Action<Vector2> OnPointerUp = null;
        
        private bool isDragging;
        private bool startedDrag;

        public Drag(VisualElement element, VisualElement root, bool startOnCreation = false)
        {
            this.element = element;
            this.root = root;
            //Register click down callback on element and then click up outside on root so you dont have to click up on the element
            element.RegisterCallback<PointerDownEvent>(clickDown);
            root.RegisterCallback<PointerUpEvent>(clickUp, TrickleDown.TrickleDown);

            if (startOnCreation)
                clickDown(null);
        }

        //Start dragging, register move pointer callback with root in order to be able to drag anywhere
        private void clickDown(PointerDownEvent e)
        {
            OnPointerDown?.Invoke(e.position);
            root.RegisterCallback<PointerMoveEvent>(update, TrickleDown.TrickleDown);
            dist = new Vector2(Pointer.current.position.x.ReadValue(), Pointer.current.position.y.ReadValue());
            
            startedDrag = false;
            isDragging = true;
        }

        //Stop dragging
        private void clickUp(PointerUpEvent e)
        {
            if (isDragging)
                OnPointerUp?.Invoke(e.position);

            root.UnregisterCallback<PointerMoveEvent>(update, TrickleDown.TrickleDown);

            isDragging = false;
        }

        //called when dragging
        private void update(PointerMoveEvent e)
        {
            if (isDragging)
            {
                if (startedDrag == false)
                {
                    startedDrag = true;
                    OnStartDrag?.Invoke(e.position);
                }
                
                OnDrag?.Invoke(e.position, e.deltaPosition);
            }
        }
        
        public void kill()
        {
            element.UnregisterCallback<PointerDownEvent>(clickDown);
            element.UnregisterCallback<PointerUpEvent>(clickUp);
            root.UnregisterCallback<PointerMoveEvent>(update, TrickleDown.TrickleDown);
        }
    }
}