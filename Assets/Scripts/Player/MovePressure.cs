using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

using UnityEngine.Audio;

public class MovePressure : MonoBehaviour
{
    public float pressTimeMs = 100f;
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] Transform effectSrc;
    [SerializeField] GameObject effect;

    public AudioSource audioSource;
    public AudioMixer mixer;
    private float pitch;

    private Vector3 startPos, endPos;
    private float count = 0f;

    SerialPort serial;
    private bool isExecuting = false;
    private float execTimer = 0f;
    private float estimatedDuration = 0.0f;

    private Thread readThread;
    private bool keepReading = true;

    private bool countStart = false;

    void Start()
    {
        startPos = start.position;
        endPos = end.position;
        this.transform.position = startPos;
        effect.SetActive(false);

        serial = new SerialPort("COM10", 9600);
        serial.NewLine = "\n";
        serial.Open();

        // 別スレッドで受信
        readThread = new Thread(ReadSerial);
        readThread.Start();
    }

    void Update()
    {
        audioSource.pitch = 1f + (float)(100f / pressTimeMs);
        pitch = 1f / audioSource.pitch;
        mixer.SetFloat("Shifter",pitch);

        startPos = start.position;
        endPos = end.position;
        
        if(Input.GetKeyDown("up")) pressTimeMs += 5f; ;
        if(Input.GetKeyDown("down")) pressTimeMs -= 5f;

        if(Input.GetKeyUp("space")) StartCoroutine("EffectDelay");//countStart = true;

        if (countStart)
        {

            if (count <= 4 * pressTimeMs / 1000f) count += Time.deltaTime;
            else
            {
                effect.SetActive(false);
                countStart = false;
                count = 0f;
            }
            this.transform.position = Vector3.Lerp(start.position, end.position, count / (4 * pressTimeMs / 1000f));
            this.transform.forward = (end.position - start.position).normalized;
        }
        
    

        if (isExecuting)
        {
            execTimer += Time.deltaTime * 1000f;
            if (execTimer >= estimatedDuration) isExecuting = false;
        }

        if ((Input.GetKeyUp("space") || OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger)) && !isExecuting)
        {
            count = 0f;
            //this.transform.position = startPos;
            //effect.SetActive(true);


            GameObject obj2 = (GameObject)Resources.Load("CabEffect");
            //Instantiate(obj2, effectSrc.position, Quaternion.identity);

            audioSource.Play();

            if (serial != null && serial.IsOpen)
            {
                string msg = $"TRIGGER:{(int)pressTimeMs}\n";
                serial.Write(msg);
                Debug.Log($"[Unity] Sent: {msg.Trim()}");

                isExecuting = true;
                execTimer = 0f;
                int pinCount = 4;
                estimatedDuration = pressTimeMs * pinCount * 1.2f;
            }
        }
    }

    IEnumerator EffectDelay()
    {

        effect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        countStart = true;
    }

    void ReadSerial()
    {
        while (keepReading && serial != null && serial.IsOpen)
        {
            try
            {
                string line = serial.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    Debug.Log($"[Arduino] {line}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Serial read error: {e.Message}");
            }
        }
    }

    
    void OnApplicationQuit()
    {
        keepReading = false;
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }

        if (serial != null && serial.IsOpen)
        {
            serial.Close();
        }
    }
}
