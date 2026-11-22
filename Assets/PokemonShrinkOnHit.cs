using System.Collections;
using UnityEngine;

public class PokemonShrinkOnHit : MonoBehaviour
{
    [Header("時間設定")]
    [SerializeField] private float pauseBeforeShrink = 0.5f; // 碰到後先停頓幾秒
    [SerializeField] private float shrinkDuration = 0.25f;   // 移動並縮到不見的時間

    [Header("碰撞與結束設定")]
    [SerializeField] private string pokeballTag = "Pokeball";
    [SerializeField] private bool destroyAfterShrink = false;

    private bool isShrinking = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isShrinking) return;

        if (collision.gameObject.CompareTag(pokeballTag) ||
            collision.gameObject.GetComponent<Pokemonball1>() != null)
        {
            // 取碰撞接觸點；若取不到接觸點則退回使用精靈球中心位置
            Vector3 hitPoint = collision.contactCount > 0
                ? collision.GetContact(0).point
                : collision.transform.position;

            StartCoroutine(ShrinkTowardsRoutine(hitPoint));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isShrinking) return;

        if (other.CompareTag(pokeballTag) ||
            other.GetComponent<Pokemonball1>() != null)
        {
            // Trigger 沒有 ContactPoint，用精靈球中心點或最近點
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            StartCoroutine(ShrinkTowardsRoutine(hitPoint));
        }
    }

    private IEnumerator ShrinkTowardsRoutine(Vector3 targetPosition)
    {
        isShrinking = true;

        // 停用碰撞體，避免被其他球重複觸發
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // 若寶可夢本身有 Rigidbody，縮小移動期間先停用物理力避免拉扯
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 1. 先停頓 0.5 秒
        yield return new WaitForSeconds(pauseBeforeShrink);

        // 2. 在 shrinkDuration 內，同時往目標點移動並縮放到 (0, 0, 0)
        float elapsed = 0f;
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.localScale = Vector3.zero;

        if (destroyAfterShrink)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}