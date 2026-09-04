using System;
using System.Collections;
using UnityEngine;
public class FoodMover : MonoBehaviour
{
    [Header("Move Setting")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float bottomOffset = 50f;


    public void Move(RectTransform target,RectTransform destination)
    {
        StartCoroutine(MoveRoutine(target, destination));
    }

    private IEnumerator MoveRoutine(RectTransform target,RectTransform destination)
    {
        // 현재 위치
        Vector3 startPosition = target.position;

        // 목적지 패널의 밑부분
        Vector3 endPosition = GetBottomCenter(destination);

        float elapsedTime = 0f;

        // 이동 연출
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / moveDuration;

            target.position = Vector3.Lerp(startPosition,endPosition,t);

            yield return null;
        }
        target.position = endPosition;
        target.SetParent(destination);
    }

    private Vector3 GetBottomCenter(RectTransform panel)
    {
        Vector3[] corners = new Vector3[4];

        panel.GetWorldCorners(corners);

        // 0 = Bottom Left
        // 3 = Bottom Right
        Vector3 bottomCenter = (corners[0] + corners[3]) * 0.5f;
        // 패널 안쪽으로 살짝 올림
        return bottomCenter + panel.up * bottomOffset;
    }
}