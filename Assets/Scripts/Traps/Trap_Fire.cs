using System.Collections;
using UnityEngine;

public class Trap_Fire : MonoBehaviour
{
    [SerializeField] private float offDuration;
    [SerializeField] private Trap_FireButton fireButton;
    private Animator anim;
    private CapsuleCollider2D fireCollider;
    private bool isActive;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        fireCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Start()
    {
        if (fireButton == null)
            Debug.LogWarning("You don't have fire button on " + gameObject.name + "!");

        SetFire(true);
    }

    public void SwitchOffFire()
    {
        if (isActive == false)
            return;

        StartCoroutine(FireCourtine());
    }
    private IEnumerator FireCourtine()
    {
        SetFire(false);
        yield return new WaitForSeconds(offDuration);
        SetFire(true);
    }

    private void SetFire(bool active)
    {
        anim.SetBool("active", active);
        fireCollider.enabled = active;
        isActive = active;
    }
}
