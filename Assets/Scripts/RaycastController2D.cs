using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController2D : MonoBehaviour
{
    public const float skinWidth = 0.015f;

    public float distanceBetweenRays = 0.25f;
    public LayerMask collisionMask;

    public Transform topLeft, topRight;
    public Transform bottomLeft, bottomRight;

    protected BoxCollider2D _collider;
    internal RaycastOrigins _raycastOrigins;

    protected int _horizontaRayCount;
    protected int _verticalRayCount;
    protected float _horizontaRaySpacing;
    protected float _verticalRaySpacing;

    protected void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    public void CalculateRaySpacing()
    {
        var boundsWidth = Vector2.Distance(topLeft.position, topRight.position) - 2 * skinWidth;
        var boundsHeight = Vector2.Distance(bottomLeft.position, topLeft.position) - 2 * skinWidth;

        _horizontaRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
        _verticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

        _horizontaRaySpacing = boundsHeight / (_horizontaRayCount - 1);
        _verticalRaySpacing = boundsWidth / (_verticalRayCount - 1);
    }

    public void UpdateRaycastOrigins()
    {
        _raycastOrigins.topLeft = topLeft.position + (bottomRight.position - topLeft.position).normalized * skinWidth;
        _raycastOrigins.topRight = topRight.position + (bottomLeft.position - topRight.position).normalized * skinWidth;
        _raycastOrigins.bottomLeft = bottomLeft.position + (topRight.position - bottomLeft.position).normalized * skinWidth;
        _raycastOrigins.bottomRight = bottomRight.position + (topLeft.position - bottomRight.position).normalized * skinWidth;
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
