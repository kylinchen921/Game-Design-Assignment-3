﻿using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class PieceTrigger : MonoBehaviour
{

    private Animator animator;
	// Use this for initialization
	void Start ()
	{
	    animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("IsTriggered", true);   
        }
    }

    
}
