using System.Collections;
using UnityEngine;

namespace Entities
{
    public class DestroyWarrior : MonoBehaviour
    {
        private Warrior _warrior;

        private void Start()
        {
            _warrior = GetComponent<Warrior>();
            _warrior.onDestroy.AddListener(x => StartCoroutine(RemoveWarrior()));
        }

        private IEnumerator RemoveWarrior()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }
    }
}