using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GluttonyEnemy : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator animator;
    public Quaternion angulo;
    public float grado;

    public Transform target;
    public bool attack;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Comportamiento_Enemigo()
    {
        // Si está lejos del jugador (modo patrulla)
        if (Vector3.Distance(transform.position, target.position) > 5)
        {
            animator.SetBool("run", false);
            cronometro += Time.deltaTime;

            if (cronometro >= 4)
            {
                rutina = Random.Range(0, 3);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    animator.SetBool("walk", false);
                    break;

                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina = 2;
                    break;

                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1f * Time.deltaTime);
                    animator.SetBool("walk", true);
                    break;
            }
        }
        else
        {
            // MODO PERSECUCIÓN
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(lookPos);

            // ERROR que tenías: escribiste "Transform.rotation"
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2f);

            animator.SetBool("walk", false);
            animator.SetBool("run", true);

            transform.Translate(Vector3.forward * 3f * Time.deltaTime);
        }
    }

    void Update()
    {
        Comportamiento_Enemigo();
    }
}
