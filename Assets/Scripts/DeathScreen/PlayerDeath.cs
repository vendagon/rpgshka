using UnityEngine;

public class PlayerDeathAnimation : MonoBehaviour
{
    private Animator animator;
    private bool hasPlayed = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayDeathAnimation();
    }

    void PlayDeathAnimation()
    {
        if (animator != null && !hasPlayed)
        {
            animator.Play("Death");
            hasPlayed = true;
        }
    }

    public void StopAnimation()
    {
        if (animator != null)
        {
            animator.speed = 0f; // ������ ��������� ��������
        }
    }

    void Update()
    {
        // �������������� ��������� ����� ���������� ��������
        if (hasPlayed && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            StopAnimation();
        }
    }
}