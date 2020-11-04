using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private Type _type = Type.Gem;
    public Type type
    {
        get
        {
            return _type; // same as =>
        }
    }
    public enum Type
    {
        Gem,
        Cherry
    }

    [SerializeField] private int _quantity = 1;
    public int quantity => _quantity;
}
