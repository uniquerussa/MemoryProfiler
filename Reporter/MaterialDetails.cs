using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Reporter
{
    public class MaterialDetails : Details
    {
        public string mainTextureName;
        public Shader materialShader;

        public MaterialDetails()
        {

        }

        public string getLogContent(MaterialDetails item)
        {
            string returnValue = null;

            returnValue = item.objectName + "    ";

            string sizeLabel = null;

            //sizeLabel += " color " + item.materialColor.ToString();
            sizeLabel += "\n" + "Shader " + item.materialShader.name;

            returnValue += sizeLabel;

            return returnValue;
        }

        public Dictionary<int, MaterialDetails> GetNewObjectInstanceIdList2(IEnumerable<Material> scanedAllObject)
        {
            Dictionary<int, MaterialDetails> returnValues = new Dictionary<int, MaterialDetails>(); //새로 스캔된 녀석의 Dictionary

            foreach (var newItem in scanedAllObject)
            {

                Material tMaterial = newItem as Material;
                MaterialDetails tMaterialDetails = new MaterialDetails();

                if (!tMaterial.name.Contains("(Instance)"))
                {
                    tMaterialDetails.objectName = tMaterial.name;

                    if (tMaterial.mainTexture != null)
                    {
                        tMaterialDetails.mainTextureName = tMaterial.mainTexture.name;
                    }
                    if (tMaterial.shader != null)
                    {
                        tMaterialDetails.materialShader = tMaterial.shader;
                    }

                    returnValues.Add(tMaterial.GetInstanceID(), tMaterialDetails);
                }
            }
            return returnValues;
        }

        public Dictionary<int, MaterialDetails> GetRealNewMaterial(Dictionary<int, MaterialDetails> newObject,
                                                                   Dictionary<int, MaterialDetails> removeObject)
        {
            var copy = new Dictionary<int, MaterialDetails>(newObject);
            foreach (KeyValuePair<int, MaterialDetails> kvp in removeObject)
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

        public Dictionary<int, MaterialDetails> GetRealRemoveMaterial(Dictionary<int, MaterialDetails> newObject,
                                                                      Dictionary<int, MaterialDetails> removeObject)
        {
            var copy = new Dictionary<int, MaterialDetails>(removeObject);
            foreach (KeyValuePair<int, MaterialDetails> kvp in newObject) 
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

    }
}
