using UnityEngine;

public class PlayerProjectile : MonoBehaviour, IPoolable
{
    private float lifetime = 3f;
    private float deactivateTime;
    private Transform ownerRoot;

    public void SetOwner(GameObject owner)
    {
        ownerRoot = owner.transform.root;
    }

    private void OnEnable()
    {
        deactivateTime = Time.time + lifetime;
    }

    private void Update()
    {
        if (Time.time >= deactivateTime)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.transform.root == ownerRoot)
            return;

        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.ChangeStateTo(EnemyScript.EnemyState.Dazed);
                Debug.Log("EnemyHit");
                ReturnToPool();
            }
        }
    }
}