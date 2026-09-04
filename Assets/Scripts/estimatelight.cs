
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;


public class estimatelight : MonoBehaviour
{
    public ARCameraManager arcaman;
    public TMP_Text brightness;
    Light our_light;
    
    void OnEnable(){
        arcaman.frameReceived += getlight;
    }
    void OnDisable(){
        arcaman.frameReceived -= getlight;

    }
    // Start is called before the first frame update
    void Start()
    {
        our_light=GetComponent<Light>();
    }

    void getlight(ARCameraFrameEventArgs args){
        if(args.lightEstimation.mainLightColor.HasValue){
            //brightness.text = $"Color_Value:{args.lightEstimation.mainLightColor.Value}";
            our_light.color=args.lightEstimation.mainLightColor.Value;
            double average_brightness = 0.2126f * our_light.color.r + 0.7152 * our_light.color.g + 0.0722 * our_light.color.b;
            brightness.text=average_brightness.ToString();
        }
    }
}
