using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float distanceFix = 0.2f;
    private float width;

    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= -width)
        {
            transform.position += Vector3.right * (width - distanceFix) * 2f;
        }
    }
}
