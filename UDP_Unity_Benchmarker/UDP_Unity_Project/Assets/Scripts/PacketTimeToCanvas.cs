using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PacketTimeToCanvas : MonoBehaviour
{
    [SerializeField] string CurrentLine = "default";
    public UDP_Listener udp_listener_script;
    public Text ValueText;

    // Start is called before the first frame update
    void Start()
    {
        udp_listener_script = GetComponent<UDP_Listener>();
    }

    // Update is called once per frame
    void Update()
    {

        CurrentLine = udp_listener_script.debug_string;
        ValueText.text = CurrentLine;
    }
}
