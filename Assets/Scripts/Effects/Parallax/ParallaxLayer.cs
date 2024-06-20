using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;

    [SerializeField]
    float CenterOffsetX = 0;

    private void Start()
    {
        transform.localPosition = transform.localPosition.WithX(CenterOffsetX);
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;

        transform.localPosition = newPos;
    }
}