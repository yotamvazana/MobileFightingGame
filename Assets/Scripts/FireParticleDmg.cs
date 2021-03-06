﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticleDmg : MonoBehaviour
{
    private int partDamage = 5;
    public Enemy enemy;
    private bool doDmg = true;
    private ParticleSystem ps;

    void Start()
    {
       ps = GetComponent<ParticleSystem>();
    }
    //void OnParticleCollision(GameObject collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        StartCoroutine(ParticleDamage());
    //    }
    //}

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
         ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

        if (inside.Count >= 1)
        {
            StartCoroutine(ParticleDamage());

        }
    }
    IEnumerator ParticleDamage()
    {
        if (doDmg)
        {
            doDmg = !doDmg;
            enemy.TakeDamage(partDamage);
            Debug.Log("ayaaaaaaaaaaaaaa");
            yield return new WaitForSeconds(0.65f);
            enemy.TakeDamage(partDamage);
            yield return new WaitForSeconds(0.65f);
            enemy.TakeDamage(partDamage);
            yield return new WaitForSeconds(0.65f);
            doDmg = !doDmg;
        }

        yield return null;
    }
}
