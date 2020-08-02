using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public PlayerController PlayerController;

    private void Awake()
    {
        PlayerController = FindObjectOfType<PlayerController>();
        Instance = this;
    }
}
