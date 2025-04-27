using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Game.DataPersistence
{
    public class JsonDataService : IDataService
    {
        private const string KEY = "4XVr0yRu4ef+dY1uoJGjKy21EuwUb2mqbnNdg9wxCMU=";
        private const string IV = "VOQUwIW9jhGWLvs+LM92/w==";
        public bool SaveData<T>(string relativePath, T data, bool encrypted)
        {
            string path = Application.persistentDataPath + relativePath;
            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Deleting old file and writing a new one");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log("Writing a file for a first time");
                }
                using FileStream stream = File.Create(path);

                if(encrypted)
                {
                    WriteEncryptedData<T>(data, stream);
                }
                else
                {
                    stream.Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(data));
                }


                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Unable to save a file.\n Reason: \n {exception.Message}" +
                    $"\n{exception.StackTrace}");
                return false;
            }
        }
        
        public T LoadData<T>(string relativePath, bool encrypted)
        {
            string path = Application.persistentDataPath + relativePath;
            
            if(!File.Exists(path))
            {
                Debug.LogError($"Cannot load file at {path}. File does not exist!");
                throw new FileNotFoundException($"{path} does not exist");
            }

            try
            {
                T data; 
                if(encrypted)
                {
                    data = ReadEncryptedData<T>(path);
                }
                else
                {
                    data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                }
                    
                return data;
            }
            catch(Exception exception)
            {
                Debug.LogError($"Unable to load a file.\n Reason: \n {exception.Message}" +
                    $"\n{exception.StackTrace}");

                throw exception;
            }
        }

        public bool IsFileExists(string relativePath)
        {
            string path = Application.persistentDataPath + relativePath;
            return File.Exists(path);
        }

        private void WriteEncryptedData<T>(T data, FileStream stream)
        {
            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(stream,
                                                               cryptoTransform, 
                                                               CryptoStreamMode.Write);

            cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));
        }
    
        private T ReadEncryptedData<T>(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            using Aes aesProvider = Aes.Create();

            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(
                aesProvider.Key,
                aesProvider.IV);

            using MemoryStream decryptionStream = new MemoryStream(fileBytes);
            using CryptoStream stream = new CryptoStream(
                                                         decryptionStream,
                                                         cryptoTransform,
                                                         CryptoStreamMode.Read
                                                        );

            using StreamReader reader = new StreamReader(stream);

            string result = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<T>(result);
        }
    
    }
}
