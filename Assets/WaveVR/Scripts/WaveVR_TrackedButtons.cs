// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using wvr;
using WaveVR_Log;

public struct ClickedEventArgs
{
    public WVR_DeviceType device;
    public uint flags;
    public Vector2 axis;
}

public delegate void ClickedEventHandler(object sender, ClickedEventArgs e);

public class WaveVR_TrackedButtons : MonoBehaviour
{
    public const string LOG_TAG = "WaveVR_TrackedButtons";

    public WVR_DeviceType device;
    public bool triggerPressed = false;
    public bool menuPressed = false;
    public bool padPressed = false;
    public bool gripPressed = false;
    public bool padTouched = false;

    public event ClickedEventHandler MenuButtonClicked;
    public event ClickedEventHandler MenuButtonUnclicked;
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerUnclicked;
    public event ClickedEventHandler PadClicked;
    public event ClickedEventHandler PadUnclicked;
    public event ClickedEventHandler PadTouched;
    public event ClickedEventHandler PadUntouched;
    public event ClickedEventHandler Gripped;
    public event ClickedEventHandler Ungripped;

    // Use this for initialization
    void Start()
    {
        /*if (this.GetComponent<WaveVR_TrackedObject>() == null)
        {
            gameObject.AddComponent<WaveVR_TrackedObject>();
        }

        if (device != 0)
        {
            this.GetComponent<WaveVR_TrackedObject>().index = device;
            if (this.GetComponent<WaveVR_RenderModel>() != null)
            {
                this.GetComponent<WaveVR_RenderModel>().index = device;
            }
        }
        else
        {
            device = (uint) this.GetComponent<WaveVR_TrackedObject>().index;
        }*/

        Log.i(LOG_TAG, "Start, device: " + device);
    }

    public virtual void OnTriggerClicked(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnTriggerClicked!");
        if (TriggerClicked != null)
            TriggerClicked(this, e);
    }

    public virtual void OnTriggerUnclicked(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnTriggerUnclicked!");
        if (TriggerUnclicked != null)
            TriggerUnclicked(this, e);
    }

    public virtual void OnMenuClicked(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnMenuClicked!");
        if (MenuButtonClicked != null)
            MenuButtonClicked(this, e);
    }

    public virtual void OnMenuUnclicked(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnMenuUnclicked!");
        if (MenuButtonUnclicked != null)
            MenuButtonUnclicked(this, e);
    }

    public virtual void OnPadClicked(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnPadClicked!");
        if (PadClicked != null)
            PadClicked(this, e);
    }

    public virtual void OnPadUnclicked(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnPadUnclicked!");
        if (PadUnclicked != null)
            PadUnclicked(this, e);
    }

    public virtual void OnPadTouched(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnPadTouched!");
        if (PadTouched != null)
            PadTouched(this, e);
    }

    public virtual void OnPadUntouched(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnPadUntouched!");
        if (PadUntouched != null)
            PadUntouched(this, e);
    }

    public virtual void OnGripped(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnGripped!");
        if (Gripped != null)
            Gripped(this, e);
    }

    public virtual void OnUngripped(ClickedEventArgs e)
    {
        Log.i (LOG_TAG, "OnUngripped!");
        if (Ungripped != null)
            Ungripped(this, e);
    }

    private Vector2 GetAxis(WVR_AnalogState_t[] analogArray, WVR_InputId _id, WVR_AnalogType aType, int _count)
    {
        Vector2 axis = Vector2.zero;

        if (_count > 0)
        {
            for (int i = 0; i < _count; i++)
            {
                if (analogArray [i].id == _id &&
                    analogArray [i].type == aType)
                {
                    axis.x = analogArray [i].axis.x;
                    axis.y = _id == WVR_InputId.WVR_InputId_Alias1_Trigger ? 0 : analogArray [i].axis.y;    // trigger does NOT have y value
                    //if (Log.FLAG_BUTTON)
                    //    Log.d (LOG_TAG, "button " + _id + " x=" + axis.x + ", y=" + axis.y);
                    break;
                } else
                {
                    Log.e (LOG_TAG, "GetAxis() states unsynchronized! device: " + device
                        + ", analogArray[" + i + "].id: " + analogArray [i].id
                        + ", analogArray[" + i + "].type]: " + analogArray [i].type);
                }
            }
        } else
        {
            Log.e (LOG_TAG, "GetAxis() no axis!!");
        }

        return axis;
    }

    private uint inputMask = (uint)(
        WVR_InputType.WVR_InputType_Button |
        WVR_InputType.WVR_InputType_Touch |
        WVR_InputType.WVR_InputType_Analog);

    // Update is called once per frame
    void Update()
    {
        if (WaveVR.Instance == null)
            return;

        if (!WaveVR_Controller.Input (device).connected)
            return;

        uint buttons = 0, touches = 0;

        int analogArrayCount = Interop.WVR_GetInputTypeCount(device, WVR_InputType.WVR_InputType_Analog);
        WVR_AnalogState_t[] analogArray = new WVR_AnalogState_t[analogArrayCount];
        if (Interop.WVR_GetInputDeviceState (device, inputMask, ref buttons, ref touches, analogArray, analogArrayCount))
        {
            ClickedEventArgs e;
            e.device = device;
            e.flags = buttons;
            e.axis = Vector2.zero;

            /**
             * for Button
             **/
            if (buttons != 0)
            {
                ulong btnState = (ulong)buttons;

                //if (Log.FLAG_BUTTON)
                //    Log.d (LOG_TAG, "device: " + device + " btnState: " + btnState);

                if ((btnState & WaveVR_Controller.Device.Input_Mask_Trigger) != 0)
                {
                    if (triggerPressed == false)    // trigger false -> true
                    {
                        triggerPressed = true;
                        e.axis = GetAxis (
                            analogArray,
                            WVR_InputId.WVR_InputId_Alias1_Trigger,
                            WVR_AnalogType.WVR_AnalogType_Trigger,
                            analogArrayCount);
                        OnTriggerClicked (e);
                    }
                }

                if ((btnState & WaveVR_Controller.Device.Input_Mask_Grip) != 0)
                {
                    if (gripPressed == false)   // grep false -> true
                    {
                        gripPressed = true;
                        OnGripped (e);
                    }
                }

                if ((btnState & WaveVR_Controller.Device.Input_Mask_Touchpad) != 0)
                {
                    if (padPressed == false)    // touchpad false -> true
                    {
                        e.axis = GetAxis (
                            analogArray,
                            WVR_InputId.WVR_InputId_Alias1_Touchpad,
                            WVR_AnalogType.WVR_AnalogType_TouchPad,
                            analogArrayCount);
                        padPressed = true;
                        OnPadClicked (e);
                    }
                }

                if ((btnState & WaveVR_Controller.Device.Input_Mask_Menu) != 0)
                {
                    if (menuPressed == false)   // menu false -> true
                    {
                        menuPressed = true;
                        OnMenuClicked (e);
                    }
                }
            } else
            {
                if (triggerPressed == true)
                {
                    triggerPressed = false;
                    OnTriggerUnclicked (e);
                }
                if (gripPressed == true)
                {
                    gripPressed = false;
                    OnUngripped (e);
                }
                if (padPressed == true)
                {
                    padPressed = false;
                    OnPadUnclicked (e);
                }
                if (menuPressed == true)
                {
                    menuPressed = false;
                    OnMenuUnclicked (e);
                }
            }   // if (buttons != 0)

            /**
             *  for Touch
             **/
            e.flags = touches;
            if (touches != 0)
            {

                ulong touchState = (ulong)touches;

                //if (Log.FLAG_BUTTON)
                //    Log.d (LOG_TAG, "device: " + device + " touchState: " + touchState);

                if ((touchState & WaveVR_Controller.Device.Input_Mask_Touchpad) != 0)
                {
                    if (padTouched == false)    // touchpad false -> true
                    {
                        padTouched = true;
                        e.axis = GetAxis (
                            analogArray,
                            WVR_InputId.WVR_InputId_Alias1_Touchpad,
                            WVR_AnalogType.WVR_AnalogType_TouchPad,
                            analogArrayCount);
                        OnPadTouched (e);
                    }
                } else
                {
                }
            } else
            {
                if (padTouched == true)
                {
                    padTouched = false;
                    OnPadUntouched (e);
                }
            }   // if (touches != 0)
        } else
        {
            Log.e (LOG_TAG, "device: " + device + " WVR_GetInputDeviceState failed!");
        }   // WVR_GetInputDeviceState
    }   // Update
}
