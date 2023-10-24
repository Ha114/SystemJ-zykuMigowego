using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Vector3 targetPosition;
    public Vector3 startPosition;

    public void AnimatePanelMenu(bool sighn)
    {
        startPosition = transform.position;
        switch (sighn)
        {
            case true: targetPosition = transform.position + Vector3.right * 254; break;
            case false: targetPosition = transform.position - Vector3.right * 254; break;
        }
        StartCoroutine(MoveCoroutine());
    }
    IEnumerator MoveCoroutine()
    {
        for(float i = 0; i <1; i += Time.deltaTime*3)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, i);
            yield return null;
        }
        transform.position = targetPosition;
    }


}
