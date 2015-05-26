﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System;

public class PlayerSightController : MonoBehaviour {

    public static Action<Vector3> PlayerMovedEvent;

    public float movementSpeed;

    private CardboardHead playerHead;
    private Transform _t;

    private MapWayPoint nextTarget;
    private LeverController targetLever;
    private PlayerState state;
    private FirstPersonController fpController;

    public enum PlayerState
    {
        moving,
        idle,
        activating,
        acting
    }

	// Use this for initialization
	void Start () {
        playerHead = GetComponentInChildren<CardboardHead>();
        _t = GetComponent<Transform>();
        fpController = GetComponent<FirstPersonController>();
        state = PlayerState.idle;

        PlayerMovedEvent(transform.position);
	}
	
	// Update is called once per frame
	void Update () {

		switch (state){
			case PlayerState.idle:
				Look();
				break;
			case PlayerState.moving:
				MoveTowardsWaypoint();
				break;
		}
	}

    private void MoveTowardsWaypoint()
    {
        Vector3 dir = (nextTarget.transform.position - transform.position).normalized;
        transform.position = transform.position + dir * movementSpeed * Time.deltaTime;

        if (Vector3.Magnitude(transform.position - nextTarget.transform.position) < 0.3f)
        {
            transform.position = nextTarget.transform.position;
            state = PlayerState.idle;
        }
    }

    private void Look()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(playerHead.Gaze, out hitInfo))
        {
            MapWayPoint wp = hitInfo.collider.gameObject.GetComponent<MapWayPoint>();
			LeverController lc = hitInfo.collider.gameObject.GetComponent<LeverController>();
            if (wp){
                if (wp.LookAt(_t))
                    SetMoveTarget(wp);
            }
            else if (lc){
                if (lc.LookAt())
                    lc.Use();
            }
        }
    }

    private void SetMoveTarget(MapWayPoint wp)
    {
        nextTarget = wp;
        state = PlayerState.moving;
        PlayerMovedEvent(wp.transform.position);
    }
}