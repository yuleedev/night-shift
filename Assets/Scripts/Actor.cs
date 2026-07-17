using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] private bool isRobber;

    public bool IsRobber => isRobber;
}