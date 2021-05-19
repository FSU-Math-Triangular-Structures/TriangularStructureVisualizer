using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public void beginBranching()
    {

        References.TrungleManager.branchReflections = !References.TrungleManager.branchReflections;

        if (References.TrungleManager.branchReflections)
        {
            References.StepsManager.current = 1;
            References.TrungleManager.AddGenerations();
        }
    }
}
