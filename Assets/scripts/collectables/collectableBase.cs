using UnityEngine;

public class collectableBase : MonoBehaviour
{
    public string compareTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag(compareTag))
        {
            Collect();
        }
    }

    protected virtual void Collect()
    {
        onCollect();
        gameObject.SetActive(false);
    }

    protected virtual void onCollect() { }
}
