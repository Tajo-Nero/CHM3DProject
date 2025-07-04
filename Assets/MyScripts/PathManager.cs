using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private List<Vector3> mainPath = new List<Vector3>();

    public void SetMainPath(List<Vector3> path)
    {
        mainPath = new List<Vector3>(path);
        Debug.Log("메인 경로가 설정되었습니다!");
    }

    public List<Vector3> GetMainPath()
    {
        return new List<Vector3>(mainPath);
    }

    public bool HasValidPath()
    {
        return mainPath.Count > 1;
    }
}