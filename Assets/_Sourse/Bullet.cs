using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 60f;
    [HideInInspector] public Vector2 direction = Vector2.right;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90);
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}