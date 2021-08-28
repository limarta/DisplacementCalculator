using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PositionWriter : MonoBehaviour
{
    private TMP_Text _textComponent;
    [SerializeField] private ClientSocket _client;

    // Start is called before the first frame update
    void Start()
    {
        _textComponent = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_client.Writer != null)
        {
            Vector3 pos = Camera.main.transform.position;
            double time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            time /= 1000;
            string output = time.ToString("##########.000") + ", " + 
                            pos.x.ToString("##.0000000") + ", " +
                            pos.y.ToString("##.0000000") + ", " +
                            pos.z.ToString("##.0000000");
            output = output.PadRight(53);
            _client.WriterQueue.Enqueue(output);
        }
    }
}
