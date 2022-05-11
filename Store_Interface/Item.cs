using UnityEngine;

public class Item : MonoBehaviour
{
   [SerializeField] private string ID;

   public string GetID() => ID;
}
