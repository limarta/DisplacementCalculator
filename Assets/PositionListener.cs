using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class PositionListener : MonoBehaviour
{
    private TMP_Text _textComponent;
    private readonly Regex rx = new Regex(@"\(\s*(?<x>\d+\.\d+)\s*,\s*(?<y>\d+\.\d+)\s*,\s*(?<z>\d+\.\d+)\s*\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private Vector3 currentPos;
    private bool matched;
    [SerializeField] private ClientSocket _client;
    void Start()
    {
        currentPos = transform.position;
        matched = false;
    }

    // Update is called once per frame
    void Update()
    {
        string text = ""; 
        if (_client.Reader != null && !_client.ReaderQueue.IsEmpty)
        {
            text += currentPos + "\nMatched: " + matched + "\nReader established";
            try
            {
                string msg;
                bool success = _client.ReaderQueue.TryDequeue(out msg);
                if (success)
                {
                    text = msg.Length + "\n" + msg;
                    MatchCollection matches = rx.Matches(msg);
                    // Report on each match.
                    foreach (Match match in matches)
                    {
                        matched = true;
                        GroupCollection groups = match.Groups;
                        float x = float.Parse(groups["x"].Value);
                        float y = float.Parse(groups["y"].Value);
                        float z = float.Parse(groups["z"].Value);
                        text += "\n" + x + ", " + y + ", " + z;
                        currentPos = new Vector3(x, y, z);
                        transform.position = currentPos;
                    }
                }
            }
            catch (Exception e)
            {
                text = "Couldn't get from queue";
                text += e.Message;
            }

        }
        else
        {
            text += "\nReader not established";
        }

        _textComponent.text = text;
    }
}
