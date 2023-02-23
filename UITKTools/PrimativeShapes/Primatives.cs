using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools
{
    public class Primatives : MonoBehaviour
    {
        public static Primatives Instance;
        public VisualTreeAsset point;

        private void Awake()
        {
            Instance = this;
        }

        public static VisualElement createPoint(Vector2 size, Vector2 loc)
        {
            VisualElement element = Instance.point.Instantiate();
            var visualElement = element.Q<VisualElement>("Point");
            setElementTransform(visualElement, size, loc);

            return element;
        }

        public static void setPointColor(Point point, Color color)
        {
            point.spriteElement.style.backgroundColor = color;
        }

        public static void setElementTransform(VisualElement element, Vector2 size, Vector2 loc)
        {
            element.style.width = size.x;
            element.style.height = size.y;

            var newLoc = new Vector2(loc.x - size.x / 2, loc.y - size.y / 2);

            element.style.left = newLoc.x;
            element.style.top = newLoc.y;
        }

        public static Vector2 getElementPosition(VisualElement element)
        {
            return new Vector2(element.style.left.value.value, element.style.top.value.value);
        }

        public static Vector2 getPointPosition(VisualElement element, Vector2 pointScale)
        {
            return new Vector2(element.style.left.value.value + pointScale.x / 2,
                element.style.top.value.value + pointScale.y / 2);
        }


        // Drawing a line with UITK https://forum.unity.com/threads/drawing-a-line-with-uitk.1193470/

        public class Line : VisualElement
        {
            public Color color;
            public MeshGenerationContext mgc;
            public List<Vector3> points;

            public float thickness;

            public Line(List<Vector3> points, float thickness, Color color)
            {
                this.thickness = thickness;
                this.points = points;
                this.color = color;

                generateVisualContent += OnGenerateVisualContent;
            }

            private void OnGenerateVisualContent(MeshGenerationContext mgc)
            {
                this.mgc = mgc;
                if (points != null) DrawCable(points.ToArray(), thickness, color, mgc);
            }

            public void moveFirstPoint(Vector2 loc)
            {
                points[0] = loc;
                regenerate();
            }
            
            public void moveSecondPoint(Vector2 loc)
            {
                points[1] = loc;
                regenerate();
            }

            //regeneration could be faster by not redrawing the entire line but adding the right vertices onto the end
            public void regenerate()
            {
                DrawCable(points.ToArray(), thickness, color, mgc);

                style.width = new StyleLength(style.width.value.value + 1);
                style.display = DisplayStyle.None;
                style.display = DisplayStyle.Flex;
            }

            public MeshWriteData DrawCable(Vector3[] points, float thickness, Color color, MeshGenerationContext context)
            {
                if (context == null) return null;

                var vertices = new List<Vertex>();
                var indices = new List<ushort>();

                for (var i = 0; i < points.Length - 1; i++)
                {
                    var pointA = points[i];
                    var pointB = points[i + 1];

                    var angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x);
                    var offsetX = thickness / 2 * Mathf.Sin(angle);
                    var offsetY = thickness / 2 * Mathf.Cos(angle);

                    vertices.Add(new Vertex
                    {
                        position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ),
                        tint = color
                    });
                    vertices.Add(new Vertex
                    {
                        position = new Vector3(pointB.x + offsetX, pointB.y - offsetY, Vertex.nearZ),
                        tint = color
                    });
                    vertices.Add(new Vertex
                    {
                        position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ),
                        tint = color
                    });
                    vertices.Add(new Vertex
                    {
                        position = new Vector3(pointB.x - offsetX, pointB.y + offsetY, Vertex.nearZ),
                        tint = color
                    });
                    vertices.Add(new Vertex
                    {
                        position = new Vector3(pointA.x - offsetX, pointA.y + offsetY, Vertex.nearZ),
                        tint = color
                    });
                    vertices.Add(new Vertex
                    {
                        position = new Vector3(pointA.x + offsetX, pointA.y - offsetY, Vertex.nearZ),
                        tint = color
                    });

                    ushort indexOffset(int value)
                    {
                        return (ushort)(value + i * 6);
                    }

                    indices.Add(indexOffset(0));
                    indices.Add(indexOffset(1));
                    indices.Add(indexOffset(2));
                    indices.Add(indexOffset(3));
                    indices.Add(indexOffset(4));
                    indices.Add(indexOffset(5));
                }

                var mesh = context.Allocate(vertices.Count, indices.Count);
                mesh.SetAllVertices(vertices.ToArray());
                mesh.SetAllIndices(indices.ToArray());

                return mesh;
            }

            public string toString()
            {
                return $"{ColorUtility.ToHtmlStringRGBA(color)}({points[0]})({points[1]})";
            }
        }
    }
}