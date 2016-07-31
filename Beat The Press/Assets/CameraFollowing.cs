using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollowing : MonoBehaviour {

	public float minimumSize = 5.35f;
	public float maximumSize = 10.0f;


	[Range(0.0f, 1.0f)]
	public float cameraEasingSpeed = 0.9f;

	void LateUpdate() {

		var targets = new List<Component>();

        targets.AddRange(FindObjectsOfType<Player>());

		if (targets.Count > 0) {

			var bounds = new Bounds();

			Vector3 averagePosition = Vector3.zero;

			var hasCreatedBounds = false;

			foreach (var target in targets) {
				var targetTransform = target.transform;
				averagePosition += targetTransform.position;

				if (hasCreatedBounds == false) {
					bounds = new Bounds(targetTransform.position, Vector3.zero);
					hasCreatedBounds = true;
				} else {
					bounds.Encapsulate(targetTransform.position);
				}
			}

            bounds.size += Vector3.one * 1.5f;

			var maximumDistance = 0.0f;
            maximumDistance = Mathf.Max(bounds.size.x, bounds.size.y);

			var destinationCameraSize = Mathf.Clamp(maximumDistance, minimumSize, maximumSize);
			destinationCameraSize /= 2.0f;

			var camera = this.GetComponent<Camera>();
			var currentCameraSize = camera.orthographicSize;

			var newSize = Mathf.Lerp(currentCameraSize, destinationCameraSize, cameraEasingSpeed);

			this.GetComponent<Camera>().orthographicSize = newSize;


			averagePosition /= targets.Count;
			
			averagePosition.z = this.transform.position.z;

			var currentPosition = this.transform.position;

			var newPosition = Vector3.Lerp(currentPosition, averagePosition, cameraEasingSpeed);

			this.transform.position = newPosition;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;

        var targets = FindObjectsOfType<Player>();
		
		if (targets.Length > 0) {

			var hasCreatedBounds = false;
			var bounds = new Bounds();

			foreach (var target in targets) {
				var targetTransform = target.transform;

				if (hasCreatedBounds == false) {
					bounds = new Bounds(targetTransform.position, Vector3.zero);
					hasCreatedBounds = true;
				} else {
					bounds.Encapsulate(targetTransform.position);
				}

			}

            bounds.size += Vector3.one * 1.5f;

			Gizmos.DrawWireCube(bounds.center, bounds.size);

		}

	}

}
