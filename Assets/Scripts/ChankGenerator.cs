using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChankGenerator : MonoBehaviour
{
    [SerializeField] private ChankGenerator[] chankPrefab;
    [SerializeField] private Vector3 nextChankOffset;
    public ChankGenerator previousChank;
    private ChankGenerator nextChank;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" || nextChank != null)
            return;

        if(previousChank != null && previousChank.previousChank != null)
        {
            previousChank.previousChank.DestroyChank();
        }
        SpawnNextChank();
    }

    public void DestroyChank()
    {
        Destroy(gameObject);
    }

    public void SpawnNextChank()
    {
        int randIndex = Random.Range(0, chankPrefab.Length);
        nextChank = Instantiate(chankPrefab[randIndex], transform.position + nextChankOffset, transform.rotation);
        nextChank.previousChank = this;
    }
}
