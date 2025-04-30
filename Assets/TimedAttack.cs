using System.Collections;
using UnityEngine;

public class TimedAttack : MonoBehaviour
{
    [SerializeField] private string attackTrigger = "DoAttack";
    [SerializeField] private float delay = 4f;

    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animator.Play("idle");
        StartCoroutine(WaitAndAttack());
    }

    IEnumerator WaitAndAttack()
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger(attackTrigger);
    }
}


