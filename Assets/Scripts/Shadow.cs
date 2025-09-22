using UnityEngine;

public class Shadow : MonoBehaviour
{
    private SpriteRenderer parentSr;
    private SpriteRenderer sr;

    private void Awake()
    {
        parentSr = transform.parent.GetComponent<SpriteRenderer>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        
    }
}
