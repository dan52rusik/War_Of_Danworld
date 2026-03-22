using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MenuAnimController : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        // Включаем анимацию рубки дерева (Chopping)
        _animator.SetBool("Chopping", true);
        
        // Опционально: можно также сразу запустить нужный стейт, чтобы не ждать перехода из Idle
        _animator.Play("Chop");
    }
}
