using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float timeUntilDestroy = 5;

        private void Awake()
        {
            Destroy(gameObject,timeUntilDestroy);
        }
    }
}
