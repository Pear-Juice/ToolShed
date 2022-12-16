using System;
using System.Collections.Generic;
using System.Linq;
using LevelEditor.LineEditor;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToolShed.UITKTools
{
    public class PenTool
    {
        private bool debug;
        public Mode mode;
        public enum Mode
        {
            Edit,
            Select
        }

        private LinePointStyle _linePointStyle;
        
        private readonly VisualElement _baseElement;
        private VisualElement _workingElement;

        private LinePoint lastLinePoint;
        private Connection lastConnection;
        private LinePoint dragLinePoint;
        private Action<Vector2> dragLinePointEvent;
        private Vector2 startDragLineLoc;

        public LinePoint selectedLinePoint;
        public Action<LinePoint> OnSelectPoint;
        public Action OnDeselectPoint;

        public Action<LinePoint> OnCreatePoint;
        public Action<LinePoint, Vector2, Vector2> OnMovedPoint;

        public readonly Dictionary<Vector2, LinePoint> linePoints = new();

        public PenTool(VisualElement baseElement, LinePointStyle linePointStyle = null, bool debug = false)
        {
            this.debug = debug;
            if (linePointStyle == null)
                _linePointStyle = new LinePointStyle
                {
                    pointScale = new Vector2(15, 15),
                    lineThicknes = 2,
                    lineColor = Color.black,
                    pointColor = Color.white,
                    selectedPointColor = Color.cyan
                };
            else
                _linePointStyle = linePointStyle;
            _baseElement = baseElement;

            initWorkingElement();

            ClickDrag clickDrag = new ClickDrag(_workingElement, _workingElement);
            clickDrag.OnClick += (loc) =>
            {
                if (mode == Mode.Edit)
                {
                    if (tryGetPointAtLoc(loc, out LinePoint linePoint))
                    {
                        clickPoint(linePoint);
                    }
                    else
                    {
                        clickBackground(loc);
                    }
                }
                else if (mode == Mode.Select)
                {
                    if (tryGetPointAtLoc(loc, out LinePoint linePoint))
                    {
                        if (selectedLinePoint != null) Primatives.setPointColor(selectedLinePoint.point, selectedLinePoint.color);
                        Primatives.setPointColor(linePoint.point, Color.cyan);

                        selectedLinePoint = linePoint;
                        OnSelectPoint?.Invoke(linePoint);
                    }
                    else
                    {
                        OnDeselectPoint?.Invoke();
                    }
                }
            };

            clickDrag.OnStartDrag += startDragPoint;

            clickDrag.OnDrag += dragPoint;

            clickDrag.OnStopDrag += stopDragPoint;
        }
        
        private void initWorkingElement()
        {
            _workingElement = new VisualElement();

            _workingElement.style.width = new Length(100, LengthUnit.Percent);
            _workingElement.style.height = new Length(100, LengthUnit.Percent);
            _workingElement.style.position = Position.Absolute;
            _baseElement.Add(_workingElement);
        }

        private void clickBackground(Vector2 loc)
        {
            LinePoint newPoint = createPoint(loc);
            //Connect this point to potential next point
            Connection connectionToNewPoint = extendPoint(newPoint);
            newPoint.connections.Add(connectionToNewPoint);

            //fill in last connections other point and connect it if it exists
            if (lastConnection != null)
            {
                lastConnection.otherPoint = newPoint;
                newPoint.connections.Add(new Connection(newPoint, lastConnection.line, lastLinePoint));
            }
            
            OnCreatePoint?.Invoke(newPoint);

            lastLinePoint = newPoint;
            lastConnection = connectionToNewPoint;
        }

        private void clickPoint(LinePoint linePoint)
        {
            //If clicked on same point, stop. Else start extending from clicked point
            if (lastLinePoint != null && lastLinePoint == linePoint)
            {
                if (debug) Debug.Log("Stop Drawing");
                cancelExtension(lastConnection);
                lastLinePoint.connections.Remove(lastConnection);
                lastLinePoint = null;
                lastConnection = null;
            }
            else if (lastLinePoint != null)
            {
                if (debug) Debug.Log("Connect Points");
                
                //Create backwards connection from this point to previous point
                linePoint.connections.Add(new Connection(linePoint, lastConnection.line, lastLinePoint));
                lastConnection.otherPoint = linePoint;
                
                if (debug) lastConnection.line.color = Color.magenta;
                //Shift line to align with point
                lastConnection.line.moveSecondPoint(Primatives.getPointPosition(linePoint.point.spriteElement, _linePointStyle.pointScale));
                linePoint.point.templateElement.BringToFront();
                
                lastLinePoint = null;
                lastConnection = null;
            }
            else
            {
                if (debug) Debug.Log("Extend From Point");
                Connection newConnection = extendPoint(linePoint);
                linePoint.connections.Add(newConnection);
                
                if (debug) newConnection.line.color = Color.blue;
                newConnection.line.regenerate();
                
                lastLinePoint = linePoint;
                lastConnection = newConnection;
            }
        }

        private void startDragPoint(Vector2 loc)
        {
            if (mode != Mode.Edit) return;
            
            //get line point that's being dragged on start
            if (tryGetPointAtLoc(loc, out LinePoint linePoint))
            {
                startDragLineLoc = linePoint.point.Loc;
                dragLinePoint = linePoint;
                linePoints.Remove(linePoint.point.Loc);

                foreach (var connection in dragLinePoint.connections)
                {
                    float distanceToP1 = Vector2.Distance(loc, connection.line.points[0]);
                    float distanceToP2 = Vector2.Distance(loc, connection.line.points[1]);

                    if (distanceToP1 < distanceToP2)
                    {
                        if (debug) connection.line.color = Color.red;
                        dragLinePointEvent += connection.line.moveFirstPoint;
                    }

                    if (distanceToP1 > distanceToP2)
                    {
                        if (debug) connection.line.color = Color.yellow;
                        dragLinePointEvent += connection.line.moveSecondPoint;
                    }
                }
            }
        }

        private void dragPoint(Vector2 pos, Vector2 dPos)
        {
            if (dragLinePoint != null)
            {
                dragLinePoint.point.Loc += dPos;
                dragLinePointEvent?.Invoke(pos);
            }
        }
        
        private void stopDragPoint(Vector2 pos)
        {
            if (dragLinePoint != null)
            {
                linePoints.Add(dragLinePoint.point.Loc, dragLinePoint);
                OnMovedPoint?.Invoke(dragLinePoint, startDragLineLoc, dragLinePoint.point.Loc);
                
                dragLinePoint = null;
                dragLinePointEvent = null;
            }
        }
        
        
        private LinePoint createPoint(Vector2 loc)
        {
            LinePoint linePoint = new()
            {
                point = new Point(_linePointStyle.pointScale, loc),
                color = _linePointStyle.pointColor
            };

            _workingElement.Add(linePoint.point.templateElement);
            linePoints.Add(linePoint.point.Loc, linePoint);

            return linePoint;
        }
        
        /// <summary>
        /// Creates line and extends from first line point
        /// </summary>
        /// <param name="firstLinePoint">Point to extend from</param>
        /// <returns></returns>
        public Connection extendPoint(LinePoint firstLinePoint)
        {
            //Create line
            var line = new Primatives.Line(new List<Vector3> { firstLinePoint.point.Loc, firstLinePoint.point.Loc }, 10, Color.black);
            _workingElement.Add(line);

            //Move point to front to fix overlapping
            firstLinePoint.point.templateElement.BringToFront();
            
            Connection connection = new(firstLinePoint, line, null, Connection.Direction.ThisFirst);

            _workingElement.RegisterCallback<PointerMoveEvent>(extendSecondPoint);
            _workingElement.RegisterCallback<PointerDownEvent>(stopExtension);

            void extendSecondPoint(PointerMoveEvent e)
            {
                line.moveSecondPoint(e.position);
            }

            void stopExtension(PointerDownEvent e)
            {
                _workingElement.UnregisterCallback<PointerMoveEvent>(extendSecondPoint);
                _workingElement.UnregisterCallback<PointerDownEvent>(stopExtension);
                
                connection.unregisterMoveLineCallbacks();
            }
            
            return connection;
        }
        
        public void cancelExtension(Connection connection)
        {
            _workingElement.Remove(connection.line);
            //if (_workingElement.Contains(connection.line))

            connection.line.mgc = null;
            //May need to set lines points to null too
            connection.line = null;
            connection.thisPoint.connections.Remove(connection);
        }

        private VisualElement getElementAtLoc(Vector2 loc)
        {
            return _workingElement.panel.Pick(loc);
        }

        private bool tryGetPointAtLoc(Vector2 loc, out LinePoint point)
        {
            if (debug) Debug.Log("Try at: " + loc);
            
            var element = getElementAtLoc(loc);
            if (element.name == "Point")
            {
                if (debug) Debug.Log("Succeed");
                
                point = linePoints[Primatives.getPointPosition(element, _linePointStyle.pointScale)];
                return true;
            }

            point = null;
            return false;
        }
        
        public List<LinePoint> getPath()
        {
            var keyValuePairs = linePoints.ToList();
            var points = new List<LinePoint>();

            foreach (var pair in keyValuePairs) points.Add(pair.Value);

            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linePoints"> List of points to add</param>
        /// <returns>Key: inputted line point, Value: created line point</returns>
        public Dictionary<LinePoint, LinePoint> loadPoints(List<LinePoint> linePoints)
        {
            List<LinePoint> passedLinePoints = new List<LinePoint>();
            List<Connection> passedConnections = new List<Connection>();

            Dictionary<LinePoint, LinePoint> createdLinePoints = new Dictionary<LinePoint, LinePoint>();

            foreach (var point in linePoints)
            {
                if (!passedLinePoints.Contains(point))
                    recurse(point);
            }

            foreach (var createdLinePoint in createdLinePoints)
            {
                createdLinePoint.Value.point.templateElement.BringToFront();
            }

            void recurse(LinePoint linePoint, LinePoint lastCreatedPoint = null, Connection lastConnection = null)
            {
                passedLinePoints.Add(linePoint);
                LinePoint newPoint = createPoint(linePoint.point.Loc);
                createdLinePoints.Add(linePoint, newPoint);

                //Set new line point's paramaters to given point's
                newPoint.id = linePoint.id;
                newPoint.color = linePoint.color;
                Primatives.setPointColor(newPoint.point, linePoint.color);

                foreach (var connection in linePoint.connections)
                {
                    if (passedConnections.Contains(connection)) continue;
                    passedConnections.Add(connection);
                    
                    Primatives.Line line;
                    if ((lastConnection != null) && (lastConnection.line != null) && (connection.line == lastConnection.line))
                    {
                        line = lastConnection.line;
                    }
                    else
                    {
                        line = new Primatives.Line(
                            new List<Vector3>() { connection.line.points[0], connection.line.points[1] },
                            _linePointStyle.lineThicknes * 5, _linePointStyle.lineColor);
                        _workingElement.Add(line);
                    }
                    
                    Connection newConnection = new Connection(linePoint, line, lastCreatedPoint);
                    newPoint.connections.Add(newConnection);

                    if (!passedLinePoints.Contains(connection.otherPoint))
                        recurse(connection.otherPoint,  newPoint, connection);
                }
            }

            return createdLinePoints;
        }

        public void activate()
        {
            _workingElement.style.display = DisplayStyle.Flex;
        }

        public void deactivate()
        {
            _workingElement.style.display = DisplayStyle.None;
        }
        
        public class LinePointStyle
        {
            public Vector2 pointScale;
            public float lineThicknes;
            public Color lineColor;
            public Color pointColor;
            public Color selectedPointColor;
        }
        
        public class LinePoint
        {
            public List<Connection> connections = new();
            public Point point;
            public Color color;
            public string id;

            public LinePoint()
            {
                id = Guid.NewGuid().ToString();
            }
        }

        public class Connection
        {
            public Primatives.Line line;
            public LinePoint otherPoint;
            public LinePoint thisPoint;

            public Direction? direction;
            public enum Direction
            {
                ThisFirst,
                ThisSecond
            }
            
            public Connection(LinePoint thisPoint, Primatives.Line line, LinePoint otherPoint = null, Direction? direction = null)
            {
                this.line = line;
                this.otherPoint = otherPoint;
                this.thisPoint = thisPoint;
                this.direction = direction;
                
                registerMoveLineCallback(direction);
            }


            private List<Direction> directions = new();
            public void registerMoveLineCallback(Direction? direction)
            {
                switch (direction)
                {
                    case Direction.ThisFirst:
                        thisPoint.point.OnMove += line.moveFirstPoint;
                        directions.Add(Direction.ThisFirst);
                        break;
                    case Direction.ThisSecond:
                        thisPoint.point.OnMove += line.moveSecondPoint;
                        directions.Add(Direction.ThisSecond);
                        break;
                    case null:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }
            }

            public void unregisterMoveLineCallbacks()
            {
                foreach (var dir in directions)
                {
                    if (dir == Direction.ThisFirst)
                        thisPoint.point.OnMove -= line.moveFirstPoint;
                    else if (dir == Direction.ThisSecond)
                        thisPoint.point.OnMove -= line.moveSecondPoint;
                }
            }
        }
    }
}