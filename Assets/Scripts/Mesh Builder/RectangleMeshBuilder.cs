using UnityEngine;

namespace BrawlShooter
{
    [RequireComponent(typeof(MeshFilter))]
    public class RectangleMeshBuilder : MonoBehaviour
    {
        public float viewWidth;
        public float viewLength;

        private Vector3[] vertices;
        private int[] triangles;
        private Vector2[] uvs;

        private Mesh viewMesh;
        private MeshFilter viewMeshFilter;

        protected MeshFilter MeshFilter
        {
            get
            {
                if (viewMeshFilter == null)
                {
                    viewMeshFilter = GetComponent<MeshFilter>();
                    //cannot be null since MeshFilter is required. 
                }
                return viewMeshFilter;
            }
        }

        void Start()
        {
            CreateMesh();
        }

        void LateUpdate()
        {
            UpdateMesh();
        }

        [ContextMenu("Update Mesh")]
        protected void UpdateMeshTest()
        {
            CreateMesh();
            UpdateMesh();
        }

        public void CreateMesh()
        {
            viewMesh = new Mesh();
            viewMesh.name = "Rectangle Mesh";
            MeshFilter.mesh = viewMesh;
        }

        public void UpdateMesh()
        {
            float halfWidth = viewWidth / 2f;
            var leftBottom = new Vector3(-halfWidth, 0, 0);
            var rightBottom = new Vector3(halfWidth, 0, 0);
            var leftTop = new Vector3(-halfWidth, 0, viewLength);
            var rightTop = new Vector3(halfWidth, 0, viewLength);

            vertices = new Vector3[]
            {
                leftBottom, leftTop, rightBottom, rightTop
            };

            triangles = new int[]
            {
                0,1,2, 1,3,2
            };

            uvs = new Vector2[vertices.Length];

            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(0, 1);
            uvs[2] = new Vector2(1, 0);
            uvs[3] = new Vector2(1, 1);

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.uv = uvs;

            viewMesh.RecalculateNormals();
        }
    }
}