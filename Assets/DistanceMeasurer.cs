using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceMeasurer : MonoBehaviour
{
    private Vector3 initialPosition;
    private TMP_Text m_TextComponent;
    private float totalDistance;
    private Vector3 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 pos = Camera.main.transform.position;
        totalDistance += (transform.position - prevPos).magnitude;
        prevPos = transform.position;
        m_TextComponent = gameObject.GetComponent<TMP_Text>();
        m_TextComponent.text = "Initial Position: " + initialPosition + "\n" +
                                "Current Position: (" + transform.position.x + ", " + transform.position.y + ", " + transform.position.z  + ")\n" +
                                "Displacement: " + (transform.position - initialPosition).magnitude + "\n" +
                                "Total Distance: " + totalDistance;
    } 
}
