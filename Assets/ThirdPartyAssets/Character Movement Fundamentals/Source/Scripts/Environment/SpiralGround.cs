using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
    public class SpiralGround : MonoBehaviour
    {
        private List<Character> characters = new List<Character>();
        private void FixedUpdate()
        {
            for (int i = 0; i < characters.Count; i++)
            {
                RaycastHit hit;
                Physics.Raycast(characters[i].transform.position, transform.position, out hit);

                characters[i].transform.rotation = Quaternion.FromToRotation(transform.forward, hit.normal);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Character character = other.GetComponent<Character>();
            if(character)
            {
                if(!characters.Contains(character))
                    characters.Add(character);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Character character = other.GetComponent<Character>();
            if (character)
            {
                if (characters.Contains(character))
                    characters.Remove(character);
            }
        }


    }
}


