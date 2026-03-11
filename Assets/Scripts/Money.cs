using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI resource_text;

    const float interval = 1; // Should in later versions be 20, is 1 now to test things
    const float base_resource_gain = 20;

    float resource_aquisition_time;
    float resources;
    float resource_gain;

    int controlled_flags; //TODO: implement in other scripts.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resource_text = GetComponent<TextMeshProUGUI>();
        resources = 0;
        controlled_flags = 1;
        SetResourceGain();
        SetCountText();
    }

    // Update is called once per frame
    void Update()
    {
        resource_aquisition_time += Time.deltaTime;

        if (resource_aquisition_time > interval)
        {
            resource_aquisition_time -= interval;
            resources += resource_gain;

            SetCountText();
        }
    }

    void SetResourceGain()
    {
        resource_gain = base_resource_gain * controlled_flags;
    }

    void SetCountText()
    {
        string resourceText = $"Money: {resources} (<color=#556B2F>+{resource_gain}</color>)";
        resource_text.text = resourceText;
    }


}
