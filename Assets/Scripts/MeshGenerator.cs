using UnityEngine;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;
    List<Vector3> vertices;
    List<int> tringles; 

    public void GenerateMesh(int[,] map, float squareSize)
    {
        vertices = new List<Vector3>();
        tringles = new List<int>();

        squareGrid = new SquareGrid(map, squareSize);

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tringles.ToArray();
        mesh.RecalculateNormals();
    }
    public void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;

            // 1 point
            case 1:
                MeshFromPoints(square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 2:
                MeshFromPoints(square.centreRight, square.bottomRight, square.centreBottom);
                break;
            case 4:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
                break;

            //2 points
            case 3:
                MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 6:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                break;
            case 5:
                MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreLeft, square.bottomLeft, square.centreLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            //3 points
            case 7:
                MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            //4 points 
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
        }
    }
    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);

    }
    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if(points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }
    void CreateTriangle(Node a,Node b, Node c)
    {
        tringles.Add(a.vertexIndex);
        tringles.Add(b.vertexIndex);
        tringles.Add(c.vertexIndex);
    }
    private void OnDrawGizmos()
    {
        return;
        if (squareGrid != null)
        {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                {
                    Gizmos.color = (squareGrid.squares[x, y].topLeft.isActive) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector3.one * 0.4f);

                    Gizmos.color = (squareGrid.squares[x, y].topRight.isActive) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector3.one * 0.4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomRight.isActive) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.position, Vector3.one * 0.4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomLeft.isActive) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.position, Vector3.one * 0.4f);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreTop.position, Vector3.one * 0.15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreRight.position, Vector3.one * 0.15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreBottom.position, Vector3.one * 0.15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centreLeft.position, Vector3.one * 0.15f);

                }
            }
        }
    }
    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];

            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }
    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreLeft, centreRight, centreTop, centreBottom;
        public int configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            centreTop = topLeft.right;
            centreLeft = bottomLeft.above;
            centreBottom = bottomLeft.right;
            centreRight = bottomRight.above;

            if (topLeft.isActive) configuration += 8;
            if (topRight.isActive) configuration += 4;
            if (bottomRight.isActive) configuration += 2;
            if (bottomLeft.isActive) configuration += 1;
        }
    }
    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 position)
        {
            this.position = position;
        }
    }
    public class ControlNode : Node
    {
        public bool isActive = false;
        public Node above, right;
        public ControlNode(Vector3 position, bool isActive, float squareSize) : base(position)
        {
            this.isActive = isActive;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
