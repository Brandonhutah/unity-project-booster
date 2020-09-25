using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movement = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 3f;
    float movementFactor;
    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period == 0)
        {
            return;
        }

        float cycles = Time.time / period;
        float rawSin = Mathf.Sin(cycles * (Mathf.PI * 2f)); // number of cycles times tau
        movementFactor = (rawSin / 2f) + 0.5f; // raw sign / 2 gives values between -.5 and .5. add .5 to shift values to be between 0 and 1

        Vector3 offset = movement * movementFactor;
        transform.position = startingPos + offset;
    }
}
