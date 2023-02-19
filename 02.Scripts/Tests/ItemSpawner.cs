using System;
using UnityEngine;
using HTH.WorldElements;

/// <summary>
/// 성능 테스트용 
/// </summary>
namespace HTH.Tests
{
    [Obsolete]
    public class ItemSpawner : MonoBehaviour
    {
        public Item prefab;
        public float term;
        public int column;
        public int row;
        private Item[] items;

        private void Start()
        {
            Spawn();
        }

        public void ActiveAll()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].gameObject.SetActive(true);
            }
        }

        public void DeactiveAll()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].gameObject.SetActive(false);
            }
        }

        public void PlayAnimationAll()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].PlayPointAnimation();
            }
        }

        public void Spawn()
        {
            items = new Item[column * row];
            for (int i = 0; i < row * column; i++)
            {
                items[i] = Instantiate(prefab, new Vector3(i % column, 0.0f, i / row) * term, Quaternion.identity);
            }
        }

    }
}