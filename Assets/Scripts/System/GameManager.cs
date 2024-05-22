using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> arrows = new List<GameObject>();

    // 화살 추가
    public void AddArrow(GameObject arrow)
    {
        arrows.Add(arrow);
    }

    // 화살 제거
    public void RemoveArrow(GameObject arrow)
    {
        if (arrows.Contains(arrow))
        {
            arrows.Remove(arrow);
            Destroy(arrow);
        }
    }
    public int ArrowCount()
    {
        return arrows.Count;
    }

    // 가장 오래된 화살을 반환합니다.
    public GameObject GetOldestArrow()
    {
        if (arrows.Count > 0)
        {
            return arrows[0];
        }
        else
        {
            return null;
        }
    }
}
