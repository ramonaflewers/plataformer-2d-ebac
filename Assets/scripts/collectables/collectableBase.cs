using UnityEngine;

public class collectableBase : MonoBehaviour
{
    [Header("Collect Settings")]
    public string compareTag = "Player";

    [Header("Particle Effect (Optional)")]
    public GameObject collectParticlePrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag(compareTag))
        {
            Collect();
        }
    }

    protected virtual void Collect()
    {
        SpawnCollectParticle();
        onCollect();
        gameObject.SetActive(false);
    }

    protected virtual void onCollect() { }

    private void SpawnCollectParticle()
    {
        if (collectParticlePrefab != null)
        {
            GameObject particle = Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);


            ParticleSystem ps = particle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(particle, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(particle, 2f);
            }
        }
    }
}
