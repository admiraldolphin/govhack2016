using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SlicedMesh : MonoBehaviour {

    [SerializeField]
    [HideInInspector]
    private Mesh mesh;

    public Vector2 size;
    public Vector2 borderSize;

	// Update is called once per frame
	void Update () {
        mesh.Clear();

        List<Vector3> verts = new List<Vector3>();

        //var xPositions = new float[]{ -size.x/2.0f, -borderSize, borderSize, size.x/2.0f };
        //var yPositions = new float[]{ -size.y/2.0f, -borderSize, borderSize, size.y/2.0f };

        //foreach 

	}
}
