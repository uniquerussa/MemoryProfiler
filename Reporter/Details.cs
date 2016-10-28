using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Reporter
{
    public class Details
    {
        public string objectName;
        public bool leak;
        public bool dublicate;

        public Details()
        {

        }

        public Dictionary<int, T> GetRemovedItems<T>(Dictionary<int, T> preScanedObject,
                                                     Dictionary<int, T> newScanedOject) where T : class // n
        {
            var returnRemoveObjDictionary = new Dictionary<int, T>();

            foreach (KeyValuePair<int, T> kvp in preScanedObject) 
            {
                if (!newScanedOject.ContainsKey(kvp.Key))
                {
                    returnRemoveObjDictionary.Add(kvp.Key, kvp.Value);
                }
            }

            return returnRemoveObjDictionary;
        }

        public Dictionary<int, T> GetRemainingItems<T>(Dictionary<int, T> preScanedObject,
                                                       Dictionary<int, T> newScanedOject) where T : class // n
        {
            var returnRemoveObjDictionary = new Dictionary<int, T>();

            foreach (KeyValuePair<int, T> kvp in newScanedOject) 
            {
                if (preScanedObject.ContainsKey(kvp.Key))
                {
                    returnRemoveObjDictionary.Add(kvp.Key, kvp.Value);
                }
            }

            return returnRemoveObjDictionary;
        }

        public Dictionary<int, T> GetNewItems<T>(Dictionary<int, T> preScanedObject,
                                                 Dictionary<int, T> newScanedOject) where T : class // n
        {
            var returnRemoveObjDictionary = new Dictionary<int, T>();

            foreach (KeyValuePair<int, T> kvp in newScanedOject)
            {
                if (!preScanedObject.ContainsKey(kvp.Key))
                {
                    returnRemoveObjDictionary.Add(kvp.Key, kvp.Value);
                }
            }

            return returnRemoveObjDictionary;
        }

        public Dictionary<int, T> GetRealRemainItems<T>(Dictionary<int, T> inputItem) where T : class // n
        {
            Dictionary<int, T> copy = new Dictionary<int, T>(inputItem);
            return copy;
        }

        public Dictionary<int, T> GetRealAllItems<T>(Dictionary<int, T> inputItem) where T : class // n
        {
            Dictionary<int, T> copy = new Dictionary<int, T>(inputItem);
            return copy;
        }


    }
}
