using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Reporter
{
    public class AudioDetails : Details
    {
        bool[] array;

        public AudioDetails()
        {

        }

        public string getLogContent(AudioDetails item)
        {
            string returnValue = null;

            returnValue = item.objectName + "    ";

            return returnValue;
        }

        public Dictionary<int, AudioDetails> GetNewObjectInstanceIdList(IEnumerable<AudioClip> scanedAllObject)
        {
            Dictionary<int, AudioDetails> returnValues = new Dictionary<int, AudioDetails>();

            foreach (var newItem in scanedAllObject)
            {
                AudioClip tAudio = newItem as AudioClip;
                AudioDetails tMeshDetails = new AudioDetails();

                tMeshDetails.objectName = tAudio.name;


                returnValues.Add(tAudio.GetInstanceID(), tMeshDetails);
            }
            return returnValues;
        }
        private bool IsReferencedObject(AudioSource component, int index, KeyValuePair<int, AudioDetails> kvp)
        {
            bool isExist = true;
            if (!array[index])
            {
                if (component.clip != null)
                {
                    if (component.clip.name.Contains(kvp.Value.objectName))
                    {
                        array[index] = true;
                        return !isExist;
                    }
                }
            }


            return isExist;
        }
        private bool IsReferencedObject(MonoBehaviour component, int index, KeyValuePair<int, AudioDetails> kvp)
        {
            bool isExist = true;

            Type temp = component.GetType();

            //temp.FindMembers()
            //if (!array[index])
            //{
            //    if (component.transform.) != null)
            //    {
            //        if (component.clip.name.Contains(kvp.Value.AudioName))
            //        {
            //            array[index] = true;
            //            return !isExist;
            //        }
            //    }
            //}


            return isExist;
        }

        public System.Object GetPropValue(System.Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public void CheckDublicateValue(Dictionary<int, AudioDetails> inputItem)
        {
            var duplicateValues = inputItem.GroupBy(x => x.Value.objectName).Where(x => x.Count() > 1);
            foreach (var item in duplicateValues)
            {
                foreach (KeyValuePair<int, AudioDetails> pair in inputItem)
                {
                    if (item.Key.Equals(pair.Value.objectName))
                    {
                        pair.Value.dublicate = true;
                    }
                }
            }
        }
        public void CheckLeak(Dictionary<int, AudioDetails> inputItem)
        {
            Component[] allComponent = Resources.FindObjectsOfTypeAll(typeof(Component)) as Component[];
            array = new bool[allComponent.Length];

            foreach (KeyValuePair<int, AudioDetails> kvp in inputItem)
            {
                bool flag = true;

                for (int i = 0; i < allComponent.Length; i++)
                {
                    string am = allComponent[i].GetType().ToString();

                    if (am.Equals("QuestAnimationSet"))
                    {
                        FieldInfo[] fields = allComponent[i].GetType().GetFields(BindingFlags.Public |
                                                                                 BindingFlags.NonPublic |
                                                                                 BindingFlags.Instance);

                        foreach (FieldInfo m in fields)
                        {
                            if (m.ToString().Contains("AudioClip"))
                            {
                                string inputName = null;
                                inputName = m.GetValue(allComponent[i]).ToString();
                                Debug.Log("");
                            }
                        }
                    }
                    //if (am.Equals("UnityEngine.AudioSource")) flag = IsReferencedObject(allComponent[i] as AudioSource, i, kvp); if (!flag) break;
                    if (am.Equals("MonoBehaviour")) flag = IsReferencedObject(allComponent[i] as MonoBehaviour, i, kvp); if (!flag) break;


                }
                if (flag) kvp.Value.leak = true;
            }
        }

        public Dictionary<int, AudioDetails> GetRealNewAudio(Dictionary<int, AudioDetails> newObject,
                                                             Dictionary<int, AudioDetails> removeObject)
        {
            var copy = new Dictionary<int, AudioDetails>(newObject);
            foreach (KeyValuePair<int, AudioDetails> kvp in removeObject) 
            {
                var keys = copy.Where(kvp2 => kvp2.Value.objectName == kvp.Value.objectName
                                            ).Select(kvp2 => kvp2.Key).Take(1).ToList();
                if (keys.Count > 0)
                {
                    int getKey = keys[0];
                    copy.Remove(getKey);
                }
            }
            return copy;
        }

        public Dictionary<int, AudioDetails> GetRealRemoveAudio(Dictionary<int, AudioDetails> newObject,
                                                                Dictionary<int, AudioDetails> removeObject)
        {
            var copy = new Dictionary<int, AudioDetails>(removeObject);
            foreach (KeyValuePair<int, AudioDetails> kvp in newObject) 
            {
                var keys = copy.Where(kvp2 => kvp2.Value.objectName == kvp.Value.objectName
                                           ).Select(kvp2 => kvp2.Key).Take(1).ToList();
                if (keys.Count > 0)
                {
                    int getKey = keys[0];
                    copy.Remove(getKey);
                }
            }
            return copy;
        }

    };

}
