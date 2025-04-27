using UnityEngine;
using System.Security.Cryptography;
using System;

namespace Game.DataPersistence
{
    public class RandomKeyAndIVGenerator : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                byte[] key = aes.Key;
                byte[] iv = aes.IV;

                Debug.Log($"Key: {Convert.ToBase64String(key)}");
                Debug.Log($"IV: {Convert.ToBase64String(iv)}");
            }
        }
    }
}
