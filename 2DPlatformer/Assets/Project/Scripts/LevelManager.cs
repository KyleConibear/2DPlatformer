using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // [SerializeField] private PolygonCollider2D levelConfine = null;

    [Header("x:left, y:bottom, z:right")]
    [SerializeField] private Vector3 levelConfine;

    private static LevelManager _instance = null; // Setter
    public static LevelManager instance => _instance; // Getter

    private int gemCount = 0;
    private int cherryCount = 0;

    public bool IsXPositionWithinLevel(float xPos)
    {
        bool isWithinLevel = true;

        // Left
        if (xPos < levelConfine.x)
        {
            isWithinLevel = false;
        }
        // Right
        else if (xPos > levelConfine.z)
        {
            isWithinLevel = false;
        }

        Debug.Log($"isWithinLevel: {isWithinLevel}");

        return isWithinLevel;
    }
  
    public void IncrementGemCount()
    {

    }

    public void IncrementCherryCount()
    {

    }

    private void Awake()
    {
        // Create singleton
        if (_instance != null && _instance != this) // There must be a duplicate
        {
            Destroy(this.gameObject);
        }
        else // There is only one
        {
            _instance = this; // This object which is an instance of the class
            DontDestroyOnLoad(this);
        }
    }
}
