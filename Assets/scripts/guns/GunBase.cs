using UnityEngine;
using System.Collections;

public class GunBase : MonoBehaviour
{
    public Transform playerTransform;
    public player playerScript;
    public Transform shootingPosition;
    public ProjectileBase prefabProjectile;
    public Vector3 offset;
    public float shootDelay = 0.3f;

    private Coroutine _currentCoroutine;
    private float _lastShootTime;

    private void Update()
    {
        FollowPlayer();

        if (Input.GetKeyDown(KeyCode.C))
        {
            TryShoot();
        }

        if (Input.GetKey(KeyCode.S) && _currentCoroutine == null)
        {
            _currentCoroutine = StartCoroutine(AutoShoot());
        }
        else if (Input.GetKeyUp(KeyCode.S) && _currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    private void FollowPlayer()
    {
        if (playerTransform == null || playerScript == null) return;

        transform.position = playerTransform.position + offset;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(playerScript.facingDirection);
        transform.localScale = scale;
    }

    private IEnumerator AutoShoot()
    {
        while (true)
        {
            TryShoot();
            yield return new WaitForSeconds(shootDelay);
        }
    }

    private void TryShoot()
    {
        if (Time.time - _lastShootTime < shootDelay) return;

        Shoot();
        _lastShootTime = Time.time;
    }

    public void Shoot()
    {
        var projectile = Instantiate(prefabProjectile);
        projectile.transform.position = shootingPosition.position;
        projectile.direction = Vector3.right * playerScript.facingDirection;
    }
}
