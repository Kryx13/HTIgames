using UnityEngine;

/// <summary>
/// Represents a segment of the narrow path in Stage 6.
/// Can be straight, curved, or with obstacles.
/// </summary>
public class PathSegment : MonoBehaviour
{
    [Header("Segment Type")]
    [SerializeField] private SegmentType segmentType = SegmentType.Straight;
    [SerializeField] private float segmentLength = 5f;
    [SerializeField] private float pathWidth = 1.5f;
    [SerializeField] private float pathHeight = 0.3f;

    [Header("Curve Settings (for Curved segments)")]
    [SerializeField] private float curveAngle = 90f; // Degrees
    [SerializeField] private CurveDirection curveDirection = CurveDirection.Right;

    [Header("Obstacle Settings (for Obstacle segments)")]
    [SerializeField] private ObstacleType obstacleType = ObstacleType.None;
    [SerializeField] private float obstacleHeight = 1f;

    [Header("Visual")]
    [SerializeField] private Material pathMaterial;
    [SerializeField] private Color pathColor = new Color(0.3f, 0.3f, 0.35f);

    [Header("Safety")]
    [SerializeField] private bool addInvisibleWalls = false; // Prevent falling off sides
    [SerializeField] private float wallHeight = 0.5f;

    public enum SegmentType
    {
        Straight,
        Curved,
        WithObstacle
    }

    public enum CurveDirection
    {
        Left,
        Right
    }

    public enum ObstacleType
    {
        None,
        LowWall,    // Duck under
        HighGap,    // Jump over
        Pillar      // Walk around
    }

    private void Start()
    {
        BuildSegment();
    }

    /// <summary>
    /// Builds the path segment based on type
    /// </summary>
    private void BuildSegment()
    {
        switch (segmentType)
        {
            case SegmentType.Straight:
                BuildStraightPath();
                break;

            case SegmentType.Curved:
                BuildCurvedPath();
                break;

            case SegmentType.WithObstacle:
                BuildStraightPath();
                AddObstacle();
                break;
        }

        if (addInvisibleWalls)
        {
            AddInvisibleWalls();
        }
    }

    /// <summary>
    /// Builds a straight path segment
    /// </summary>
    private void BuildStraightPath()
    {
        GameObject path = GameObject.CreatePrimitive(PrimitiveType.Cube);
        path.name = "PathFloor";
        path.transform.SetParent(transform);
        path.transform.localPosition = new Vector3(0f, 0f, segmentLength / 2f);
        path.transform.localScale = new Vector3(pathWidth, pathHeight, segmentLength);

        ApplyPathMaterial(path);
    }

    /// <summary>
    /// Builds a curved path segment (approximated with multiple pieces)
    /// </summary>
    private void BuildCurvedPath()
    {
        int segments = 8; // Number of pieces to approximate curve
        float angleStep = curveAngle / segments;
        float pieceLength = (curveAngle * Mathf.Deg2Rad * segmentLength) / segments;

        for (int i = 0; i < segments; i++)
        {
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piece.name = $"PathPiece_{i}";
            piece.transform.SetParent(transform);

            float angle = angleStep * i * Mathf.Deg2Rad;
            float direction = (curveDirection == CurveDirection.Right) ? 1f : -1f;

            float x = Mathf.Sin(angle) * segmentLength * direction;
            float z = (1f - Mathf.Cos(angle)) * segmentLength;

            piece.transform.localPosition = new Vector3(x, 0f, z);
            piece.transform.localRotation = Quaternion.Euler(0f, angleStep * i * direction, 0f);
            piece.transform.localScale = new Vector3(pathWidth, pathHeight, pieceLength);

            ApplyPathMaterial(piece);
        }
    }

    /// <summary>
    /// Adds an obstacle to the path
    /// </summary>
    private void AddObstacle()
    {
        GameObject obstacle = null;

        switch (obstacleType)
        {
            case ObstacleType.LowWall:
                obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obstacle.name = "LowWall_Obstacle";
                obstacle.transform.localScale = new Vector3(pathWidth, obstacleHeight, 0.3f);
                obstacle.transform.localPosition = new Vector3(0f, obstacleHeight / 2f, segmentLength / 2f);
                break;

            case ObstacleType.HighGap:
                // Create a gap by removing middle section
                GameObject leftPiece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leftPiece.name = "LeftPiece";
                leftPiece.transform.SetParent(transform);
                leftPiece.transform.localPosition = new Vector3(0f, 0f, segmentLength * 0.25f);
                leftPiece.transform.localScale = new Vector3(pathWidth, pathHeight, segmentLength / 2f);
                ApplyPathMaterial(leftPiece);

                GameObject rightPiece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rightPiece.name = "RightPiece";
                rightPiece.transform.SetParent(transform);
                rightPiece.transform.localPosition = new Vector3(0f, 0f, segmentLength * 0.75f);
                rightPiece.transform.localScale = new Vector3(pathWidth, pathHeight, segmentLength / 2f);
                ApplyPathMaterial(rightPiece);
                return; // Don't create main path for gap

            case ObstacleType.Pillar:
                obstacle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                obstacle.name = "Pillar_Obstacle";
                obstacle.transform.localScale = new Vector3(0.5f, obstacleHeight, 0.5f);
                obstacle.transform.localPosition = new Vector3(0f, obstacleHeight / 2f, segmentLength / 2f);
                break;
        }

        if (obstacle != null)
        {
            obstacle.transform.SetParent(transform);
            obstacle.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.25f);
        }
    }

    /// <summary>
    /// Adds invisible walls to prevent falling off
    /// </summary>
    private void AddInvisibleWalls()
    {
        // Left wall
        GameObject leftWall = new GameObject("InvisibleWall_Left");
        leftWall.transform.SetParent(transform);
        leftWall.transform.localPosition = new Vector3(-pathWidth / 2f, wallHeight / 2f, segmentLength / 2f);

        BoxCollider leftCollider = leftWall.AddComponent<BoxCollider>();
        leftCollider.size = new Vector3(0.1f, wallHeight, segmentLength);

        // Right wall
        GameObject rightWall = new GameObject("InvisibleWall_Right");
        rightWall.transform.SetParent(transform);
        rightWall.transform.localPosition = new Vector3(pathWidth / 2f, wallHeight / 2f, segmentLength / 2f);

        BoxCollider rightCollider = rightWall.AddComponent<BoxCollider>();
        rightCollider.size = new Vector3(0.1f, wallHeight, segmentLength);
    }

    /// <summary>
    /// Applies material to path piece
    /// </summary>
    private void ApplyPathMaterial(GameObject pathPiece)
    {
        Renderer renderer = pathPiece.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (pathMaterial != null)
            {
                renderer.material = pathMaterial;
            }
            else
            {
                renderer.material.color = pathColor;
            }
        }
    }

    /// <summary>
    /// Gizmo to visualize path segment
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + new Vector3(0f, 0.1f, segmentLength / 2f),
            new Vector3(pathWidth, pathHeight, segmentLength));

        // Show direction arrow
        Gizmos.color = Color.green;
        Vector3 start = transform.position;
        Vector3 end = transform.position + transform.forward * segmentLength;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.2f);
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f,
            $"{segmentType}\nLength: {segmentLength}m\nWidth: {pathWidth}m");
#endif
    }

    // Public getters
    public float GetSegmentLength() => segmentLength;
    public float GetPathWidth() => pathWidth;
    public SegmentType GetSegmentType() => segmentType;
}
