using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
    public class ParadoxEffect : MonoBehaviour 
    {
        public ParticleSystem particleSystem;

        public ParticleSystem.MainModule main;
        public ParticleSystem.EmissionModule emission;

        void Awake()
        {
            main = particleSystem.main;
            emission = particleSystem.emission;
        }
    }
}
