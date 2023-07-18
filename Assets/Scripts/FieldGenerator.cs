using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FieldGenerator : MonoBehaviour
{
    public int VerticalPointsCount = 100;
    public int HorizontalPointsCount = 100;

    private int hexSize = Metrics.hexSize;

    public float triangleSize = 0.2f;
    public float hillHeight = 0.4f;
    public float rockHeight = 1.0f;
    public float swampHeight = -0.2f;
    public float step = 0.1f;

    public FieldMesh fieldMesh;
    private List<HexMapElement> hexMap = new List<HexMapElement>();
    private List<Vertice> vertices = new List<Vertice>();
    private List<Vector2> uvs = new List<Vector2>();
    private Vertice[,] verticesArray;


    private void Start()
    {
        fieldMesh.Clear();
        //GenerateHexMap();
        GenerateRandomHexMap();
        GenerateInitialPoints();
        UpdateHeights();
        
        //NormalizeEdgesHeights();
        //NormalizeHeights();
        PopulateMeshVertices();
        GenerateTriangles();
        AddUVs();
        fieldMesh.Apply();
        fieldMesh.ApplyNormalNoise(fieldMesh.Filter.mesh);
        fieldMesh.Smooth();
    }

    private void GenerateInitialPoints()
    {
        //int i = 0;
        verticesArray = new Vertice[HorizontalPointsCount, VerticalPointsCount];
        for (int z = 0; z < VerticalPointsCount; z++)
        {
            for (int x = 0; x < HorizontalPointsCount; x++)
            {
                Vector3 position;
                position.x = z % 2 == 0 ? x * triangleSize : x * triangleSize + triangleSize / 2;
                position.z = z * triangleSize * 0.86602540378f;
                position.y = 0f;
                Vertice v = new Vertice(position, x, z);
                vertices.Add(v);
                verticesArray[x, z] = v;
                uvs.Add(new Vector2((float)x / HorizontalPointsCount, (float)z / VerticalPointsCount));
                //fieldMesh.AddVertice(position);
                //i++;
            }
        }
    }

    private void PopulateMeshVertices()
    {
        foreach (Vertice v in vertices)
        {
            fieldMesh.AddVertice(v.position);
        }
    }

    private void AddUVs()
    {
        fieldMesh.AddUVs(uvs);
    }

    private void GenerateRandomHexMap()
    {
        int vc = VerticalPointsCount / hexSize;
        int hc = HorizontalPointsCount / hexSize;
        for (int z = 0; z < vc; z++)
        {
            for (int x = 0; x < hc; x++)
            {
                var hextype = (HexType)UnityEngine.Random.Range(0, 4);
                hexMap.Add(new HexMapElement(x, z, hextype));

            }
        }
    }

    private void GenerateHexMap()
    {
        hexMap.Add(new HexMapElement(0, 0, HexType.hill));
        hexMap.Add(new HexMapElement(0, 1, HexType.hill));
        //hexMap.Add(new HexMapElement(0, 2, HexType.plain));
        //hexMap.Add(new HexMapElement(0, 3, HexType.plain));
        hexMap.Add(new HexMapElement(0, 4, HexType.hill));
        //hexMap.Add(new HexMapElement(0, 5, HexType.plain));
        hexMap.Add(new HexMapElement(1, 0, HexType.hill));
        hexMap.Add(new HexMapElement(1, 1, HexType.hill));
        //hexMap.Add(new HexMapElement(1, 2, HexType.rock));
        hexMap.Add(new HexMapElement(1, 3, HexType.rock));
        hexMap.Add(new HexMapElement(1, 4, HexType.rock));
        //hexMap.Add(new HexMapElement(1, 5, HexType.rock));
        //hexMap.Add(new HexMapElement(2, 0, HexType.plain));
        //hexMap.Add(new HexMapElement(2, 1, HexType.plain));
        //hexMap.Add(new HexMapElement(2, 2, HexType.plain));
        //hexMap.Add(new HexMapElement(2, 3, HexType.rock));
        hexMap.Add(new HexMapElement(2, 4, HexType.swamp));
        //hexMap.Add(new HexMapElement(2, 5, HexType.plain));
        //hexMap.Add(new HexMapElement(3, 0, HexType.plain));
        //hexMap.Add(new HexMapElement(3, 1, HexType.plain));
        //hexMap.Add(new HexMapElement(3, 2, HexType.plain));
        hexMap.Add(new HexMapElement(3, 3, HexType.swamp));
        //hexMap.Add(new HexMapElement(3, 4, HexType.plain));
        //hexMap.Add(new HexMapElement(3, 5, HexType.plain));
        //hexMap.Add(new HexMapElement(4, 0, HexType.plain));
        hexMap.Add(new HexMapElement(4, 1, HexType.hill));
        //hexMap.Add(new HexMapElement(4, 2, HexType.plain));
        //hexMap.Add(new HexMapElement(4, 3, HexType.plain));
        //hexMap.Add(new HexMapElement(4, 4, HexType.plain));
        //hexMap.Add(new HexMapElement(4, 5, HexType.plain));
        //hexMap.Add(new HexMapElement(5, 0, HexType.plain));
        //hexMap.Add(new HexMapElement(5, 1, HexType.rock));
        //hexMap.Add(new HexMapElement(5, 2, HexType.hill));
        //hexMap.Add(new HexMapElement(5, 3, HexType.plain));
        //hexMap.Add(new HexMapElement(5, 4, HexType.plain));
        //hexMap.Add(new HexMapElement(5, 5, HexType.plain));
    }


    private void UpdateHeights()
    {
        var hexCenters = vertices.Where(v => v.IsHexCenter());
        foreach (var hexCenter in hexCenters)
        {
            var hexCoords = hexCenter.ToHexCoordinates();
            //Debug.Log("tX: "+ hexCenter.tX.ToString() + " tZ: " + hexCenter.tZ.ToString() + "/ hexX: " + hexCoords.Item1.ToString() + " hexZ: " + hexCoords.Item2.ToString());
            var hex = hexMap.FirstOrDefault(e => e.X == hexCoords.Item1 && e.Z == hexCoords.Item2);
            var height = hex.hexType switch
            {
                HexType.plain => 0f,
                HexType.hill => hillHeight,
                HexType.rock => rockHeight,
                HexType.swamp => swampHeight,
                _ => throw new ArgumentException("wrong hex type")
            };
            vertices.First(v => v.tX == hexCenter.tX && v.tZ == hexCenter.tZ).position.y = height;
<<<<<<< Updated upstream
            //Debug.Log(hexCenter.ToString());
            for (int d = 1; d < Metrics.hexSize; d++)
=======

            int typeNumber = hex.textureType;
            int textureindex = vertices.IndexOf(hexCenter);
            texturevector.x = texturevector.z = texturevector.y = typeNumber;
            texturelist[textureindex] = texturevector;

            Debug.Log(hexCenter.ToString());

            for (int d = 1; d <= Metrics.hexSize; d++)
>>>>>>> Stashed changes
            {
                var neighbours = vertices.Where(v => v.DiscreteDistance(hexCenter) == d);
                foreach (var n in neighbours)
                {
                    float newHeight = 0f;
                    if (hexCenter.position.y == 0f)
                        newHeight = 0f;
                    else
                    {
                        newHeight = hex.hexType switch
                        {
                            HexType.plain => 0f,
                            HexType.hill => HillHeight(hexCenter, n),
                            HexType.rock => RockHeight(hexCenter, n),
                            HexType.swamp => SwampHeight(hexCenter, n),
                            _ => throw new ArgumentException("wrong hex type")
                        };
                    }
                    n.position.y = newHeight;
                }
            }
        }


    }

    private float RockHeight(Vertice hexCenter, Vertice target)
    {
        var h = hexCenter.position.y;
        Vector2 start, end;
        start.x = hexCenter.position.x;
        start.y = hexCenter.position.z;
        end.x = target.position.x;
        end.y = target.position.z;
        var relativeDistance = Vector2.Distance(start, end) / (hexSize * triangleSize);
        Debug.Log(relativeDistance.ToString());
        if (relativeDistance < 0.2f)
            return Mathf.Lerp(h, 0.9f * h, relativeDistance * 5);
        else if (relativeDistance < 0.4f)
            return Mathf.Lerp(0.9f * h, 0.7f * h, relativeDistance * 2 / 5);
        else if (relativeDistance < 0.6f)
            return Mathf.Lerp(0.7f * h, 0.4f * h, relativeDistance * 3 / 5);
        else if (relativeDistance < 0.8f)
            return Mathf.Lerp(0.4f * h, 0.1f * h, relativeDistance * 4 / 5);
        else
            return Mathf.Lerp(0.2f * h, 0, relativeDistance);
    }





    private float HillHeight(Vertice hexCenter, Vertice target)
    {
        Vector2 start, end;
        start.x = hexCenter.position.x;
        start.y = hexCenter.position.z;
        end.x = target.position.x;
        end.y = target.position.z;
        //var relativeDistance = Vector3.Distance(hexCenter.position, target.position) / (hexSize * triangleSize);
        var relativeDistance = Vector2.Distance(start, end) / (hexSize * triangleSize);
        return Mathf.Lerp(hexCenter.position.y, 0, Mathf.Pow(relativeDistance, 2f));
    }

    private float SwampHeight(Vertice hexCenter, Vertice target)
    {
        var h = hexCenter.position.y;
        Vector2 start, end;
        start.x = hexCenter.position.x;
        start.y = hexCenter.position.z;
        end.x = target.position.x;
        end.y = target.position.z;
        var relativeDistance = Vector2.Distance(start, end) / (hexSize * triangleSize);
        if (relativeDistance < 0.8f)
            return h;
        else
            return Mathf.Lerp(h, 0, relativeDistance);
    }

    private void NormalizeHeights()
    {
        for (int z = 0; z < VerticalPointsCount; z++)
        {
            for (int x = 0; x < HorizontalPointsCount; x++)
            {
                var vertice = verticesArray[x, z];
                var neighbours = new List<Vertice>();
                neighbours.Add(verticesArray[x - 1 < 0 ? 0 : 1, z]);
                neighbours.Add(verticesArray[x + 1 > HorizontalPointsCount - 1 ? x : x + 1, z]);

                neighbours.Add(verticesArray[x - 1 + z % 2 < 0 ? 0 : x - 1 + z % 2, z + 1 > VerticalPointsCount - 1 ? z : z + 1]);
                neighbours.Add(verticesArray[x + z % 2 > HorizontalPointsCount -1 ? HorizontalPointsCount - 1 : x + z % 2, z + 1 > VerticalPointsCount - 1 ? z : z + 1]);
                neighbours.Add(verticesArray[x - 1 + z % 2 < 0 ? 0 : x - 1 + z % 2, z - 1 < 0 ? 0 : z - 1]);
                neighbours.Add(verticesArray[x + z % 2 > HorizontalPointsCount - 1 ? HorizontalPointsCount - 1 : x + z % 2, z - 1 < 0 ? 0 : z - 1]);
                var averageHeight = neighbours.Select(e => e.position.y).Average();
                vertice.position.y = averageHeight;
            }
        }
    }

    //private void updateEdgeHeights()
    //{
    //    var edgetable = new Dictionary<Vertice, HashSet<HexType>>();
    //    var hexCenters = vertices.Where(v => v.IsHexCenter());
    //    foreach (var hexCenter in hexCenters)
    //    {
    //        var hexCoords = hexCenter.ToHexCoordinates();
    //        var edgeVertices = vertices.Where(v => v.DiscreteDistance(hexCenter) == hexSize);
    //        foreach (Vertice edgeVertice in edgeVertices)
    //        {
    //            if (!edgetable.ContainsKey(edgeVertice))
    //            {
    //                var type = hexMap.FirstOrDefault(e => e.X == hexCoords.Item1 && e.Z == hexCoords.Item2).hexType;
    //                edgetable.Add(edgeVertice, new type);
    //            }
    //        }
    //    }
        
    //}

    private void NormalizeEdgesHeights()
    {
        var hexCenters = vertices.Where(v => v.IsHexCenter());
        foreach(Vertice hexCenter in hexCenters)
        {
            var edgeVertices = vertices.Where(v => v.DiscreteDistance(hexCenter) == hexSize);
            foreach (Vertice v in edgeVertices)
            {
                var averageNeighboursHeight = vertices.Where(e => e.DiscreteDistance(v) == 1).Select(e => e.position.y).Average();
                v.position.y = averageNeighboursHeight;
            }
            

        }
        

    }


    private void GenerateTriangles()
    {
        for (int z = 0; z < (VerticalPointsCount - 1) / 2 ; z++)
        {
            for (int x = 0; x < HorizontalPointsCount - 1; x++)
            {


                fieldMesh.AddTriangle(
                    z * 2 * HorizontalPointsCount + x,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + 1);
                fieldMesh.AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount + 1,
                    z * 2 * HorizontalPointsCount + x + 1);
                fieldMesh.AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2 + 1);
                fieldMesh.AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2 + 1,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount + 1);
            }
        }
    }
}

public struct HexMapElement
{
    public int X;
    public int Z;
    public HexType hexType;

    public HexMapElement(int x, int z, HexType type)
    {
        X = x;
        Z = z;
        hexType = type;
    }

    public override string ToString()
    {
        return "X " + X.ToString() + "   Z " + Z.ToString();
    }
}

public static class Metrics
{
    public const int hexSize = 10;
}

public class Vertice
{
    //private const int initialOffset = 

    public Vector3 position;
    public int x;
    public int z;

    public int tX => x - z / 2;
    public int tZ => z;
    public int tY => -tX - tZ;
    

    public Vertice(Vector3 pos, int X, int Z)
    {
        position = pos;
        x = X;
        z = Z;
    }

    public (int, int) ToHexCoordinates()
    {
        var hexZ = (tZ / Metrics.hexSize) / 2;
        var hexX = (((tX + tZ / 2) / Metrics.hexSize) * 2 + 2) / 3;
        return (hexX, hexZ);
    }

    public bool IsHexCenter()
    {
        if (tX % Metrics.hexSize == 0)
        {
            if ((tZ - tX) % (Metrics.hexSize * 3) == 0)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public int DiscreteDistance(Vertice other)
    {
        return (Mathf.Abs(tX - other.tX) + Mathf.Abs(tZ - other.tZ) + Mathf.Abs(tY - other.tY)) / 2;
    }

    public override string ToString()
    {
        string res = "Vertice: x: " + x.ToString() + " z: " + z.ToString();
        return res;
    }

}


public enum HexType
{
    plain, hill, rock, swamp
}

//public interface IHexHeightsProvider {
//    float Height(Vertice hexCenter, int distanceFromHexCenter);
//}

//public class 