using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookAtTargetAnimationRigging : MonoBehaviour
{
    private Rig rig;
    public Transform headAimTarget;
    public float targetWeight = 0;

    private void Awake()
    {
        rig = GetComponent<Rig>();
        rig.weight = 0;
    }

    private void Update()
    {
        // rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * 10f);
    }
}
