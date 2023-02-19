using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ResizingCam : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private BoxCollider _boxBound;
    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        SetCamera();
    }

    private Vector3[] GetMinMaxVertices()
    {
        if (_meshFilter.sharedMesh == null)
            return null;

        Vector3[] vertices = _meshFilter.sharedMesh.vertices;

        if (vertices.Length <= 0)
            return null;

        Vector3[] minMaxVertices = new Vector3[2];
        //Debug.Log($"{vertices[0]}, {vertices.Length}");

        float xMax = vertices[0].x;
        float yMax = vertices[0].y;
        float zMax = vertices[0].z;
        float xMin = vertices[0].x;
        float yMin = vertices[0].y;
        float zMin = vertices[0].z;

        for (int i = 1; i < vertices.Length; i++) 
        {
            xMax = Mathf.Max(xMax, vertices[i].x);
            yMax = Mathf.Max(yMax, vertices[i].y);
            zMax = Mathf.Max(zMax, vertices[i].z);
            xMin = Mathf.Min(xMin, vertices[i].x);
            yMin = Mathf.Min(yMin, vertices[i].y);
            zMin = Mathf.Min(zMin, vertices[i].z);
        }
        
        minMaxVertices[0] = new Vector3(xMin, yMin, zMin); // [0] min값
        minMaxVertices[1] = new Vector3(xMax, yMax, zMax); // [1] max값

        return minMaxVertices;
    }

    private void SetBound()
    {
        Vector3[] minMaxVertices = GetMinMaxVertices();

        if (minMaxVertices == null)
            return;

        Vector3 targetVector = _meshFilter.GetComponent<Transform>().position;
        Vector3 targetCubeVector = new Vector3(targetVector.x + ((minMaxVertices[1].x + minMaxVertices[0].x) / 2)
                                                , targetVector.y + ((minMaxVertices[1].y + minMaxVertices[0].y) / 2)
                                                , targetVector.z + ((minMaxVertices[1].z + minMaxVertices[0].z)) / 2);
        float xScale = Mathf.Abs(minMaxVertices[1].x - minMaxVertices[0].x);
        float yScale = Mathf.Abs(minMaxVertices[1].y - minMaxVertices[0].y);
        float zScale = Mathf.Abs(minMaxVertices[1].z - minMaxVertices[0].z);
        _boxBound.size = new Vector3(xScale, yScale, zScale);
        _boxBound.center = Vector3.up * yScale / 2.0f;
    }

    private void SetCamera()
    {
        SetBound();
        float xzMagnitude = Vector3.Magnitude(new Vector3(_boxBound.size.x, 0, _boxBound.size.z));
        _cam.transform.localPosition = _boxBound.transform.localPosition + (new Vector3(1, 0, 1) * xzMagnitude)
                                                                      + (Vector3.up * Vector3.Magnitude(new Vector3(xzMagnitude, (_boxBound.transform.localScale.y/1.5f) - Mathf.Abs(_boxBound.transform.localPosition.y), xzMagnitude)));
        _cam.orthographicSize = Mathf.Clamp(Vector3.Magnitude(_boxBound.transform.localScale) / 2f, 0.4f, 30.0f);
    }
}
