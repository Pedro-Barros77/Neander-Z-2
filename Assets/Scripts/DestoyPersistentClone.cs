using UnityEngine;

public class DestoyPersistentClone : MonoBehaviour
{
    void Start()
    {
        if (!gameObject.CompareTag("Original"))
        {
            Destroy(gameObject);
        }
    }
}
