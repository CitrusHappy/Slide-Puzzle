using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class block : MonoBehaviour
{
    public event System.Action<block> OnBlockPressed;
    public event System.Action OnFinishedMoving;

    public Vector2Int coord;
    Vector2Int startingCoord;

    public void Init(Vector2Int startingCoord, Texture2D image)
    {
        this.startingCoord = startingCoord;
        coord = startingCoord;

        GetComponent<MeshRenderer>().material = Resources.Load<Material>("block");
        GetComponent<MeshRenderer>().material.mainTexture = image;
    }

    public void MoveToPosition(Vector2 target, float duration) 
    {
        StartCoroutine(AnimateMove(target, duration));
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            OnBlockPressed?.Invoke(this);
        }
    }

    private void OnMouseDown()
    {
        OnBlockPressed?.Invoke(this);
    }


    IEnumerator AnimateMove(Vector2 target, float duration)
    {
        Vector2 initialPos = transform.position;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(initialPos, target, percent);
            yield return null;
        }

        if (OnFinishedMoving != null)
        {
            OnFinishedMoving();
        }
    }

    public bool IsAtStartingCoord()
    {
        return coord == startingCoord;
    }

}
