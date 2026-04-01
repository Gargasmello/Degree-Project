using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{

    public static Money instance;

    [SerializeField] private TextMeshProUGUI resource_text;

    const float interval = 1; // Should in later versions be 20, is 1 now to test things
    const float base_resource_gain = 20;

    float resource_aquisition_time;
    public float resources;
    float resource_gain;

    int controlled_flags; //TODO: implement in other scripts.

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resource_text = GetComponent<TextMeshProUGUI>();
        resources = 0;
        controlled_flags = 1;
        SetResourceGain();
        SetCountText();
    }

    public void GainResources()
    {
        resources += resource_gain;

        SetCountText();
    }

    void SetResourceGain()
    {
        resource_gain = base_resource_gain * controlled_flags;
    }

    public void SetCountText()
    {
        string resourceText = $"Resources: {resources} (<color=#556B2F>+{resource_gain}</color>)";
        resource_text.text = resourceText;
    }


}
