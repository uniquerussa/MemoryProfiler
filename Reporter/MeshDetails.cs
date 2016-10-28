using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Reporter
{
    public class MeshDetails : Details
    {
        public int verticesCount;

        public string getLogContent(MeshDetails item)
        {
            string returnValue = null;

            returnValue = item.objectName + "    ";

            string sizeLabel = null;

            sizeLabel += "vertexCount " + item.verticesCount;

            returnValue += sizeLabel;

            return returnValue;
        }

        public Mesh FindMesh(string name)
        {
            Mesh[] allTextures = Resources.FindObjectsOfTypeAll<Mesh>();

            Mesh returnValue = null;

            for (int i = 0; i < allTextures.Length; i++)
            {
                if (allTextures[i].name == name)
                {
                    returnValue = allTextures[i];
                    break;
                }
            }
            return returnValue;
        }

        public Dictionary<int, MeshDetails> GetNewObjectInstanceIdList(IEnumerable<Mesh> scanedAllObject)
        {
            Dictionary<int, MeshDetails> returnValues = new Dictionary<int, MeshDetails>(); //새로 스캔된 녀석의 Dictionary

            foreach (var newItem in scanedAllObject)
            {
                Mesh tMesh = newItem as Mesh;
                MeshDetails tMeshDetails = new MeshDetails();

                tMeshDetails.objectName = tMesh.name;
                tMeshDetails.verticesCount = tMesh.vertexCount;

                returnValues.Add(tMesh.GetInstanceID(), tMeshDetails);
            }
            return returnValues;
        }

        public Dictionary<int, MeshDetails> GetRealNewMesh(Dictionary<int, MeshDetails> newObject,
                                                           Dictionary<int, MeshDetails> removeObject)
        {
            var copy = new Dictionary<int, MeshDetails>(newObject);
            foreach (KeyValuePair<int, MeshDetails> kvp in removeObject) 
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

        public Dictionary<int, MeshDetails> GetRealRemoveMesh(Dictionary<int, MeshDetails> newObject,
                                                              Dictionary<int, MeshDetails> removeObject)
        {
            var copy = new Dictionary<int, MeshDetails>(removeObject);
            foreach (KeyValuePair<int, MeshDetails> kvp in newObject) 
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
