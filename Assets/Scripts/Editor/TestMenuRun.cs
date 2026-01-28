using UnityEditor;
using UnityEngine;

public class TestMenuRun
{
    [MenuItem("Tools/Test - Run Player1 Setup")]
    static void TestRun()
    {
        Debug.Log("TEST: Menu item clicked!");
        CreatePlayer1Animator.CreateAnimator();
    }
}
