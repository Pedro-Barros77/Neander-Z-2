using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    private float oldPosition;

    void Start()
    {
        oldPosition = transform.position.x;
    }

    void Update()
    {
        if (transform.position.x == oldPosition)
            return;

        float delta = oldPosition - transform.position.x;
        onCameraTranslate?.Invoke(delta);

        oldPosition = transform.position.x;
    }
}