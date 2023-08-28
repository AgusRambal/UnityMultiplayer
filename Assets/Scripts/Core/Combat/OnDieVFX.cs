using UnityEngine;

public class OnDieVFX : MonoBehaviour
{
    [SerializeField] private GameObject onDie;
    [SerializeField] private GameObject levelUp;

    [HideInInspector] public int health;

    private void OnDestroy()
    {
        if (health <= 0)
        {
            Instantiate(onDie, transform.position, Quaternion.identity);
        }

        else
        {
            Instantiate(levelUp, transform.position, Quaternion.identity);
        }
    }
}
