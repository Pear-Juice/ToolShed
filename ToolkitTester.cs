using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed
{
    public class ToolkitTester : MonoBehaviour
    {
        // Start is called before the first frame update
        public UIDocument doc;

        public Vector2 loc;
        public Vector2 size;

        private void Start()
        {
            var baseElement = doc.rootVisualElement.Q<VisualElement>("Base");

            //var renderer = new Renderer(baseElement);
            //var penTool = new PenTool(baseElement, new Vector2(10, 10), 10, Color.black, Color.white);

            //     var clock = new Clock(2);
            //     clock.pass += () =>
            //     {
            //         if (Keyboard.current.tabKey.isPressed)
            //         {
            //             var grid = penTool.getPathAsNodeGraph();
            //
            //             var nodes = grid.nodes.ToList();
            //
            //             var seeker = new Seeker();
            //
            //             var soughtNodes = seeker.seek(nodes[0].Value, nodes[^1].Value);
            //
            //             foreach (var soughtNode in soughtNodes)
            //                 if (penTool.linePoints.TryGetValue(soughtNode.location, out var linePoint))
            //                     linePoint.point.element.Q<VisualElement>("Point").style.unityBackgroundImageTintColor =
            //                         new StyleColor(Color.green);
            //         }
            //     };
            //
            //     clock.start();
            //
            //
            //     // bool isFirstPointDown = false;
            //     // PenTool dragableLine = new PenTool(baseElement, new Vector2(10, 10), 50, Color.black);fe
            //     // renderer.Add(ref dragableLine.pointElements);
            //     //
            //     // baseElement.RegisterCallback<PointerDownEvent>((e) =>
            //     // {
            //     //     if (!e.target.ToString().Contains("Base")) return;
            //     //     
            //     //     if (!isFirstPointDown)
            //     //     {
            //     //         dragableLine.addFirstPoint(e.position);
            //     //         isFirstPointDown = true;
            //     //     }
            //     // });
        }
    }
}