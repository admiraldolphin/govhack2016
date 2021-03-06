﻿using UnityEngine;
using System.Collections;
using InControl;

public class Grabbing : MonoBehaviour
{
    private Player _player = null;
    private Player player {
        get { 
            if (_player == null)
                _player = GetComponentInParent<Player> (); 
            return _player;
        } 
    }

    private GameObject grabbedObject;

    public float maximumGrabbingRange = 0.5f;

    void OnDrawGizmos ()
    {

        if (isGrabbing) {
            Gizmos.color = Color.red;
        } else {
            Gizmos.color = Color.green;
        }


        var lineStartPoint = this.transform.position;
        var lineEndPoint = this.transform.position + (-this.transform.up) * maximumGrabbingRange;

        Gizmos.DrawLine (lineStartPoint, lineEndPoint);
    }

    private bool isGrabbing = false;

    private HingeJoint2D grabbingJoint = null;

    // Update is called once per frame
    void Update ()
    {

        if (grabbingJoint != null && grabbedObject == null) {
            ReleaseGrab();
        }

        if (player.Actions.Grab.WasPressed) {
            // try to create a joint if we don't already have one

            if (grabbingJoint != null) {
                ReleaseGrab ();
            } else {
                AttemptGrab ();
            }

        }


    }

    void AttemptGrab ()
    {
        
        var hit = Physics2D.Raycast (this.transform.position, 
                      this.transform.up, maximumGrabbingRange, ~LayerMask.GetMask ("Player"));

        if (hit != null) {

            if (hit.collider == null) {
                return;
            }

            var answer = hit.collider.GetComponent<AnswerImage>();

            // we only care about AnswerImages
            if (answer == null) {
                return;
            }

            answer.lastHoldingPlayer = GetComponentInParent<Player>();

            var connectedBody = hit.collider.GetComponent<Rigidbody2D> ();

            if (connectedBody == null) {
                isGrabbing = false;
                return;
            }

            // Is anything else grabbing this?
            foreach (var grabber in FindObjectsOfType<Grabbing>()) {
                if (grabber == this) continue;
                if (grabber.grabbedObject == hit.collider.gameObject) {
                    grabber.ReleaseGrab();
                }
            }

            grabbedObject = hit.collider.gameObject;

            Debug.LogFormat ("Collected {0}", hit.collider.gameObject);
            isGrabbing = true;

            grabbingJoint = gameObject.AddComponent<HingeJoint2D> ();

            var connectedAnchor = hit.collider.gameObject.transform.InverseTransformPoint (hit.point);

            grabbingJoint.enableCollision = false;
            grabbingJoint.connectedBody = connectedBody;
            grabbingJoint.anchor = new Vector2 (0, 0);
            grabbingJoint.autoConfigureConnectedAnchor = false;
            grabbingJoint.connectedAnchor = connectedAnchor;

            grabbingJoint.breakForce = 0.1f;

        }
    }

    void ReleaseGrab ()
    {
        grabbedObject = null;
        isGrabbing = false;
        
        Destroy (grabbingJoint);
        
        grabbingJoint = null;

    }


}









