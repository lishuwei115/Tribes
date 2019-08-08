using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinderManager : MonoBehaviour
{
    public static PathFinderManager Instance;
    public List<PathPoint> Points;
    public List<Transform> KeyPoints;
    public List<PathLine> Lines;
    public int PointsPerSegment = 10;
    public LayerMask CollisionMask;

    private void Awake()
    {
        Instance = this;
        List<Transform> v = GetComponentsInChildren<Transform>().ToList();
        v.Remove(transform);
        for (int i = 0; i < v.Count; i++)
        {
            PathPoint p = new PathPoint();
            p.Position = v[i];
            Points.Add(p);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CollisionMask = LayerMask.GetMask("ObstacleItem");
        //CollisionMask = LayerMask.NameToLayer("ObstacleItem");
        CreateNet();
    }
    public Vector3 GetCloserPoint(Vector3 position)
    {
        Vector3 CloserPoint = Vector3.zero;

        foreach (PathLine line in Lines)
        {
            foreach (Vector3 pos in line.Points)
            {
                if (CloserPoint == Vector3.zero)//primo punto
                {
                    CloserPoint = pos;
                }
                else
                if (Vector3.Distance(pos, position) < Vector3.Distance(position, CloserPoint))
                {
                    CloserPoint = pos;
                }
            }
        }


        return CloserPoint;
    }
    public PathLine GetCloserLine(Vector3 position)
    {
        Vector3 CloserPoint = new Vector3(50000,0,50000);
        bool b = false;
        PathLine returnLine = new PathLine();
        foreach (PathLine line in Lines)
        {
            if (line.Points.Count == 0)
                Debug.Log("line missing");
            foreach (Vector3 pos in line.Points)
            {
                if (line.Points.Count == 0)
                    Debug.Log("line missing");
                if (b == false)//primo punto
                {
                    CloserPoint = pos;
                    returnLine = line;

                    b = true;
                }
                else
                if (Vector3.Distance(pos, position) < Vector3.Distance(position, CloserPoint))
                {
                    CloserPoint = pos;
                    if (line.Points.Count == 0)
                        Debug.Log("line missing");
                    returnLine = line;
                }
            }
        }

        if (b)
        {
            if (returnLine.Points.Count == 0)
                Debug.Log("line missing");
            return returnLine;

        }
        return null;

    }
    private void CreateNet()
    {
        List<PathPoint> Points2 = Points;
        List<PathPoint> support = Points;
        foreach (PathPoint point in Points)
        {
            support = Points2.Where(r => !r.Equals(Points)).ToList();
            Points2 = support;
            foreach (PathPoint point2 in Points2)
            {


                if (point.Position == point2.Position)
                {
                    //Points2.Remove(point2);
                }
                else
                {
                    Vector3 cache = point.Position.position;
                    Ray r = new Ray(point.Position.position, point2.Position.position);
                    //Debug.DrawRay(p.Target.position, p2.Target.position, Color.white, 30);
                    if (!Physics.Raycast(point.Position.position, point2.Position.position - point.Position.position, Vector3.Distance(point.Position.position, point2.Position.position), CollisionMask))
                    {
                        point.Neighbors.Add(point2.Position);
                        PathLine newPath = new PathLine();
                        newPath.MainPoints.Add(point.Position.position);
                        newPath.MainPoints.Add(point2.Position.position);
                        newPath.weight = Vector3.Distance(point.Position.position, point2.Position.position);
                        //create sub points
                        for (float t = 0; t <= 1 + 1f / PointsPerSegment; t += 1f / PointsPerSegment)
                        {

                            Vector3 v = Vector3.Lerp(point.Position.position, point2.Position.position, t);
                            newPath.Points.Add(v);
                            Debug.DrawLine(v, cache, Color.Lerp(Color.red, Color.blue, t), 3000);
                            cache = v;
                        }
                        if (newPath.Points.Count > 0)
                        {
                            newPath.ID = "" + Lines.Count;

                            Lines.Add(newPath);

                        }

                    }
                }

            }

        }
        FindPaths();
    }
    private void FindPaths()
    {
        List<PathPoint> Points2 = Points;
        List<PathPoint> support = Points;

        foreach (PathPoint point in Points)
        {
            foreach (Transform t in KeyPoints)
            {
                support = Points.Where(r => r.Position == t).ToList();
                CalculatePath(point, support[0]);
            }

        }

    }
    public PathPoint TransformToPathPoint(Transform t)
    {
        foreach (PathPoint p in Points)
        {
            if (p.Position == t)
            {
                return p;
            }
        }
        return null;
    }
    public PathPoint VtoPoint(Vector3 t)
    {
        foreach (PathPoint p in Points)
        {
            if (p.Position.position == t)
            {
                return p;
            }
        }
        return null;
    }
    public float CalculateListV3Weight(List<Vector3> v)
    {
        float distance = 0;
        for (int i = 1; i < v.Count; i++)
        {
            distance += Vector3.Distance(v[0], v[1]);
        }
        return distance;
    }
    public List<Vector3> TransformListToVectorList(List<Transform> transforms)
    {
        List<Vector3> resultList = new List<Vector3>();
        foreach (Transform t in transforms)
        {
            resultList.Add(t.position);
        }
        return resultList;
    }
    public float DistanceToKeyPoint(Vector3 StartingPoint, Transform destination)
    {
        /*if (Vector3.Distance(StartingPoint, destination.position) < 20)
        {
            return 0;
        }*/
        Vector3 closerPoint = GetCloserPoint(StartingPoint);
        Vector3 closerPathPoint;
        PathLine closerLine = GetCloserLine(StartingPoint);
        if (closerLine.Points.Count <= 0)
        {
            Debug.Log("point missing" + closerLine.weight);
        }
        if (Vector3.Distance(closerLine.Points[0], destination.position) < Vector3.Distance(closerLine.Points[closerLine.Points.Count - 1], destination.position))
        {
            closerPathPoint = closerLine.Points[0];
            PathPoint p = VtoPoint(closerPathPoint);
            List<Transform> path = p.Paths.Where(t => t.destination == destination).ToList()[0].Path;
            List<Vector3> path1 = TransformListToVectorList(path);
            List<Vector3> path2 = new List<Vector3> { StartingPoint, closerPoint, closerLine.Points[0] };
            path1 = path2.Union(path1).ToList();
            float dist = CalculateListV3Weight(path1);
            return dist;
        }
        else
        {
            closerPathPoint = closerLine.Points[closerLine.Points.Count - 1];
            PathPoint p = VtoPoint(closerPathPoint);
            List<Transform> path = p.Paths.Where(t => t.destination == destination).ToList()[0].Path;
            List<Vector3> path1 = TransformListToVectorList(path);
            List<Vector3> path2 = new List<Vector3> { StartingPoint, closerPoint, closerLine.Points[closerLine.Points.Count - 1] };
            path1 = path2.Union(path1).ToList();
            float dist = CalculateListV3Weight(path1);
            return dist;
        }
    }
    public List<Vector3> GetPathToDestination(Vector3 StartingPoint, Transform destination)
    {
        Vector3 closerPoint = GetCloserPoint(StartingPoint);
        Vector3 closerPathPoint;
        PathLine closerLine = GetCloserLine(StartingPoint);
        if (Vector3.Distance(closerLine.Points[0], destination.position) < Vector3.Distance(closerLine.Points[closerLine.Points.Count - 1], destination.position))
        {
            closerPathPoint = closerLine.Points[0];
            PathPoint p = VtoPoint(closerPathPoint);
            List<Transform> path = p.Paths.Where(t => t.destination == destination).ToList()[0].Path;
            List<Vector3> path1 = TransformListToVectorList(path);
            List<Vector3> path2 = new List<Vector3> { StartingPoint, closerPoint, closerLine.Points[0] };
            path1 = path2.Union(path1).ToList();
            return path1;
        }
        else
        {
            closerPathPoint = closerLine.Points[closerLine.Points.Count - 1];
            PathPoint p = VtoPoint(closerPathPoint);
            List<Transform> path = p.Paths.Where(t => t.destination == destination).ToList()[0].Path;
            List<Vector3> path1 = TransformListToVectorList(path);
            List<Vector3> path2 = new List<Vector3> { StartingPoint, closerPoint, closerLine.Points[closerLine.Points.Count - 1] };
            path1 = path2.Union(path1).ToList();
            return path1;
        }
    }

    internal Transform GetHousePos(HousesTypes houseType)
    {
        switch (houseType)
        {
            case HousesTypes.North:
                return KeyPoints[0];
                break;
            case HousesTypes.South:
                return KeyPoints[1];
                break;
            case HousesTypes.East:
                return KeyPoints[2];
                break;
            case HousesTypes.West:
                return KeyPoints[3];
                break;
        }
        return null;
    }

    public void CalculatePath(PathPoint startingPoint, PathPoint endingPoint)
    {
        PathToPoint pathToDest = new PathToPoint();
        pathToDest.destination = endingPoint.Position;
        if (startingPoint.Position == endingPoint.Position)
        {
            pathToDest.Path = new List<Transform> { startingPoint.Position, endingPoint.Position };
            startingPoint.Paths.Add(pathToDest);
            return;
        }

        if (startingPoint.Neighbors.Contains(endingPoint.Position))
        {
            pathToDest.Path = new List<Transform> { startingPoint.Position, endingPoint.Position };
            startingPoint.Paths.Add(pathToDest);
            return;
        }
        else
        {
            List<Transform> pathsTried = new List<Transform>();
            //List<PathPoint> n1 = startingPoint.Neighbors.; 
            pathToDest.Path = new List<Transform> { startingPoint.Position };
            foreach (Transform v in startingPoint.Neighbors)
            {
                pathToDest.Path = new List<Transform> { startingPoint.Position, TransformToPathPoint(v).Position };
                if (!pathsTried.Contains(v))
                {
                    pathsTried.Add(v);
                    foreach (Transform v2 in TransformToPathPoint(v).Neighbors)
                    {
                        if (!pathsTried.Contains(v2))
                        {
                            pathToDest.Path = new List<Transform> { startingPoint.Position, TransformToPathPoint(v).Position, TransformToPathPoint(v2).Position };
                            if (v2 == endingPoint.Position) // found path
                            {
                                startingPoint.Paths.Add(pathToDest);
                                return;
                            }
                            pathsTried.Add(v2);
                            foreach (Transform v3 in TransformToPathPoint(v2).Neighbors)
                            {
                                pathToDest.Path = new List<Transform> { startingPoint.Position, TransformToPathPoint(v).Position, TransformToPathPoint(v2).Position, TransformToPathPoint(v3).Position };
                                if (v3 == endingPoint.Position) // found path
                                {
                                    startingPoint.Paths.Add(pathToDest);
                                    return;
                                }
                                if (!pathsTried.Contains(v3))
                                {
                                    pathsTried.Add(v3);
                                    foreach (Transform v4 in TransformToPathPoint(v3).Neighbors)
                                    {
                                        pathToDest.Path = new List<Transform> { startingPoint.Position, TransformToPathPoint(v).Position, TransformToPathPoint(v2).Position, TransformToPathPoint(v3).Position, TransformToPathPoint(v4).Position };
                                        if (v4 == endingPoint.Position) // found path
                                        {
                                            startingPoint.Paths.Add(pathToDest);
                                            return;
                                        }
                                        if (!pathsTried.Contains(v4))
                                        {
                                            pathsTried.Add(v4);
                                            foreach (Transform v5 in TransformToPathPoint(v4).Neighbors)
                                            {
                                                pathToDest.Path = new List<Transform> { startingPoint.Position, TransformToPathPoint(v).Position, TransformToPathPoint(v2).Position, TransformToPathPoint(v3).Position, TransformToPathPoint(v4).Position, TransformToPathPoint(v5).Position };
                                                if (v5 == endingPoint.Position) // found path
                                                {
                                                    startingPoint.Paths.Add(pathToDest);
                                                    return;
                                                }
                                                if (!pathsTried.Contains(v5))
                                                {
                                                    pathsTried.Add(v5);
                                                    foreach (Transform v6 in TransformToPathPoint(v5).Neighbors)
                                                    {
                                                        pathToDest.Path = new List<Transform> { startingPoint.Position, TransformToPathPoint(v).Position, TransformToPathPoint(v2).Position, TransformToPathPoint(v3).Position, TransformToPathPoint(v4).Position, TransformToPathPoint(v5).Position, TransformToPathPoint(v6).Position };
                                                        if (v6 == endingPoint.Position) // found path
                                                        {
                                                            startingPoint.Paths.Add(pathToDest);
                                                            return;
                                                        }
                                                        if (!pathsTried.Contains(v6))
                                                        {
                                                            pathsTried.Add(v6);
                                                            foreach (Transform v7 in TransformToPathPoint(v6).Neighbors)
                                                            {
                                                                pathToDest.Path = new List<Transform> { startingPoint.Position, TransformToPathPoint(v).Position, TransformToPathPoint(v2).Position, TransformToPathPoint(v3).Position, TransformToPathPoint(v4).Position, TransformToPathPoint(v5).Position, TransformToPathPoint(v6).Position };
                                                                if (v7 == endingPoint.Position) // found path
                                                                {
                                                                    startingPoint.Paths.Add(pathToDest);
                                                                    return;
                                                                }
                                                                if (!pathsTried.Contains(v7))
                                                                {
                                                                    pathsTried.Add(v7);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

    }

}
[System.Serializable]
public class PathLine
{
    public List<Vector3> Points = new List<Vector3>();
    public string ID = "";
    public float weight = 0;
    public List<Vector3> MainPoints = new List<Vector3>();
}
[System.Serializable]
public class PathPoint
{
    public Transform Position;
    public List<Transform> Neighbors = new List<Transform>();
    public List<PathToPoint> Paths = new List<PathToPoint>();
    public string ID = "";
}
[System.Serializable]

public class PathToPoint
{
    public Transform destination;
    public List<Transform> Path;
}