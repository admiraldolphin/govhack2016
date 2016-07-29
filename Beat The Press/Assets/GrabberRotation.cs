using UnityEngine;
using System.Collections;

public class GrabberRotation : MonoBehaviour {

    private Player _player = null;
    private Player player {
        get { 
            if (_player == null)
                _player = GetComponentInParent<Player> (); 
            return _player;
        } 
    }

	float lastAngle = 0.0f;

	[SerializeField]
	float offsetAngle = 0.0f;

	// Update is called once per frame
	void Update () {
		
        if (player.Actions.GrabberRotation.Vector.magnitude > 0.5) {
            lastAngle = player.Actions.GrabberRotation.Angle;
			// Offset because the gun is at a right angle
			lastAngle += offsetAngle;
		}

		var rotation = Quaternion.AngleAxis(lastAngle, -Vector3.forward);

		this.transform.rotation = rotation;
	}
}
