using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetNavigation : MonoBehaviour
{
    [SerializeField] public Transform targetTransform;

    [SerializeField] public GameObject targetClone;
    [SerializeField] private RectTransform rootRectTransform;
    private Image icon;
    private Vector2 localPoint;

    //private Transform player;
    private Transform playerEye;

    private Rect viewRect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        viewRect = GetComponent<RectTransform>().rect;
        playerEye = GameObject.Find("CenterEyeAnchor").transform;

        //GameObject enemy = GameObject.FindGameObjectWithTag("Target");
        //targetTransform = enemy.transform.Find("target").transform;
        rootRectTransform = transform.parent.GetComponent<RectTransform>();
        icon = GetComponent<Image>();
        icon.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetClone == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 offset = targetTransform.position - playerEye.position;  //相対的な位置関係をとりたい
        offset = playerEye.InverseTransformPoint(offset);
        //Debug.Log("offsetX: " + offset.x + " offsetY: " + offset.z + " tan: " + offset.z / offset.x);

        /*
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootRectTransform,
            screenPosition,
            Camera.main,
            out Vector2 localPoint);
        */
        localPoint.x = offset.x;
        localPoint.y = offset.z;

        icon.enabled = true;

        if (localPoint.x >= 0 && localPoint.y >= 0) //++
        {
            if (localPoint.y > 0.7f * localPoint.x) icon.enabled = false;

            localPoint.x = rootRectTransform.rect.xMax - viewRect.width / 2;
            localPoint.y = rootRectTransform.rect.yMin + rootRectTransform.rect.height * offset.z / (offset.x) + viewRect.height / 2;
        }
        else if (localPoint.x < 0 && localPoint.y >= 0) //-+
        {
            if (localPoint.y > -0.7f * localPoint.x) icon.enabled = false;

            localPoint.x = rootRectTransform.rect.xMin + viewRect.width / 2;
            localPoint.y = rootRectTransform.rect.yMin + rootRectTransform.rect.height * offset.z / (-offset.x) + viewRect.height / 2;
        }
        else if (localPoint.x < 0 && localPoint.y < 0) //--
        {
            if (localPoint.y > 0.2f * localPoint.x) localPoint.x = rootRectTransform.rect.xMin + viewRect.width / 2;

            else localPoint.x = ((rootRectTransform.rect.width / 2) + viewRect.width / 2) * -offset.x / (5 * offset.z);
            localPoint.y = rootRectTransform.rect.yMin + viewRect.height / 2;
        }
        else if (localPoint.x >= 0 && localPoint.y < 0) //+-
        {
            if (localPoint.y > -0.2f * localPoint.x) localPoint.x = rootRectTransform.rect.xMax - viewRect.width / 2;

            else localPoint.x = ((rootRectTransform.rect.width / 2) - viewRect.width / 2) * -offset.x / (5 * offset.z);
            localPoint.y = rootRectTransform.rect.yMin + viewRect.height / 2;
        }

        //Vector3 Scale = GetComponent<RectTransform>().localScale;
        var maxScale = 3.5f;
        float distance = offset.magnitude;
        float size = maxScale / distance;
        if (size < 0.25f) icon.enabled = false;

        //Debug.Log(size);
        Vector3 Scale = size * Vector3.one;
        GetComponent<RectTransform>().localScale = Scale;

        //transform.position = screenPosition;

        // Debug.Log("screenPosition: " + screenPosition);


        // Debug.Log("localPoint: " + localPoint);
        /*
                localPoint.x = Mathf.Clamp(
                    localPoint.x,
                    rootRectTransform.rect.xMin + viewRect.width / 2,
                    rootRectTransform.rect.xMax - viewRect.width / 2
                );

                if (0 < screenPosition.z)
                {
                    localPoint.y = Mathf.Clamp(
                        localPoint.y,
                        rootRectTransform.rect.yMin + viewRect.height / 2,
                        rootRectTransform.rect.yMax - viewRect.height / 2
                    );
                }
                else
                {
                    localPoint.y = rootRectTransform.rect.yMin + viewRect.height / 2;
                }
                */

        //Vector2 icon = GetComponent<RectTransform>().anchoredPosition;



        GetComponent<RectTransform>().anchoredPosition = localPoint;
        // GetComponent<RectTransform>().sizeDelta = (maxScale / size) * 100 * Vector2.one;
        //transform.position = localPoint;
        
        
    }
}
