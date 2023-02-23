using UnityEngine.UIElements;

namespace ToolShed.UITKTools
{
    public static class Extensions
    {
        public static void setActive(this VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
        }
        
        public static void setInactive(this VisualElement element)
        {
            element.style.display = DisplayStyle.None;
        }
    }
}