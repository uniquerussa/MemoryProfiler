using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Reporter
{
    public class TextureDetails : Details
    {
        public bool isCubeMap;
        public int memSizeKB;
        public int textureWidth;
        public int textureHeight;
        public TextureFormat format;
        public int mipMapCount;

        private readonly string[] _shaderTextures = new[] { "_SpecularTex", "_Cube", "_EnvTex", "_AlphaMap", "_AlphaTex", "_Blurred" };//Coc","_ReflectionTex", "_PatternTex", "_Occluder", "_Silhouette", "_TapHigh", "_TapLow", "_Coc", "_Blurred", "_LowRez", "_FgOverlap", "_TapMedium", "_TapLowForeground", "_TapLowBackground", "_Source", "_NonBlurredTex", "_BlurTex", "_VignetteTex", "_Curve", "_SmallTex", "_ColorBuffer", "_EdgeTex", "_RandomTexture", "_SSAO", "_Skybox","_GrainTex", "_ScratchTex", "_LrDepthTex", "_HrDepthTex","_MainTexBlurred", "_AdaptTex", "_CurTex", "_RampTex", "_RgbDepthTex", "_ZCurve", "_RgbTex","_Overlay","_GlossTex","_SliceGuide","_Ramp","_SpecTex", "starTexture", "noisetex","_WiggleTex", "_MaskTex", "_TransparencyLM","_ParallaxMap","_Spec_Gloss_Reflec_Masks", "_Mask", "_BumpMap2","_Texture2", "_BumpMap1","_Texture1", "_NoiseTex" };

        public TextureDetails()
        {

        }

        private int CalculateTextureSizeBytes(Texture tTexture)
        {
            int tWidth = tTexture.width;
            int tHeight = tTexture.height;
            if (tTexture is Texture2D)
            {
                Texture2D tTex2D = tTexture as Texture2D;
                int bitsPerPixel = GetBitsPerPixel(tTex2D.format);
                int mipMapCount = tTex2D.mipmapCount;
                int mipLevel = 1;
                int tSize = 0;
                while (mipLevel <= mipMapCount)
                {
                    tSize += tWidth * tHeight * bitsPerPixel / 8;
                    tWidth = tWidth / 2;
                    tHeight = tHeight / 2;
                    mipLevel++;
                }
                return tSize;
            }

            if (tTexture is Cubemap)
            {
                Cubemap tCubemap = tTexture as Cubemap;
                int bitsPerPixel = GetBitsPerPixel(tCubemap.format);
                return tWidth * tHeight * 6 * bitsPerPixel / 8;
            }
            return 0;
        }

        private int GetBitsPerPixel(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8:	
                    return 8;
                case TextureFormat.ARGB4444: 
                    return 16;
                case TextureFormat.RGB24:   
                    return 24;
                case TextureFormat.RGBA32:  
                    return 32;
                case TextureFormat.ARGB32: 
                    return 32;
                case TextureFormat.RGB565: 
                    return 16;
                case TextureFormat.DXT1: 
                    return 4;
                case TextureFormat.DXT5:    
                    return 8;
                case TextureFormat.PVRTC_RGB2:
                    return 2;
                case TextureFormat.PVRTC_RGBA2:
                    return 2;
                case TextureFormat.PVRTC_RGB4:
                    return 4;
                case TextureFormat.PVRTC_RGBA4:
                    return 4;
                case TextureFormat.ETC_RGB4:
                    return 4;
                case TextureFormat.ATC_RGB4:
                    return 4;
                case TextureFormat.ATC_RGBA8:
                    return 8;
                case TextureFormat.BGRA32:
                    return 32;
                case TextureFormat.ATF_RGB_DXT1:	 
                case TextureFormat.ATF_RGBA_JPG:
                case TextureFormat.ATF_RGB_JPG: 
                    return 0;
            }
            return 0;
        }

        private string FormatSizeString(int memSizeKB)
        {
            if (memSizeKB < 1024) return "" + memSizeKB + "k";
            else
            {
                float memSizeMB = ((float)memSizeKB) / 1024.0f;
                return memSizeMB.ToString("0.00") + "Mb";
            }
        }

        public Dictionary<int, TextureDetails> GetNewObjectInstanceIdList(IEnumerable<Texture> scanedAllObject)
        {
            Dictionary<int, TextureDetails> returnValues = new Dictionary<int, TextureDetails>(); 

            foreach (var newItem in scanedAllObject)
            {
                Texture tTexture = newItem as Texture;
                TextureDetails tTextureDetails = new TextureDetails();

                tTextureDetails.isCubeMap = tTexture is Cubemap;
                tTextureDetails.objectName = tTexture.name;
                tTextureDetails.textureWidth = tTexture.width;
                tTextureDetails.textureHeight = tTexture.height;

                int memSize = tTextureDetails.CalculateTextureSizeBytes(tTexture);
                tTextureDetails.memSizeKB = memSize / 1024;
                TextureFormat tFormat = TextureFormat.RGBA32;
                int tMipMapCount = 1;

                if (tTexture is Texture2D)
                {
                    tFormat = (tTexture as Texture2D).format;
                    tMipMapCount = (tTexture as Texture2D).mipmapCount;
                }
                if (tTexture is Cubemap)
                {
                    tFormat = (tTexture as Cubemap).format;
                }

                tTextureDetails.format = tFormat;
                tTextureDetails.mipMapCount = tMipMapCount;
                tTextureDetails.dublicate = false;
                tTextureDetails.leak = false;
                returnValues.Add(tTexture.GetInstanceID(), tTextureDetails);
            }
            return returnValues;
        }

        public string GetLogContent(TextureDetails item)
        {
            string returnValue = null;

            returnValue = item.objectName+ "    ";

            string sizeLabel = "" + item.textureWidth + "x" + item.textureHeight;
            if (item.isCubeMap) sizeLabel += "x6";
            sizeLabel += " - " + item.mipMapCount + "mip";
            sizeLabel += "\n" + FormatSizeString(item.memSizeKB) + " - " + item.format + "";


            returnValue += sizeLabel;

            return returnValue;
        }

        private bool IsReferencedObject(UISprite component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;

            if (component.mainTexture != null &&
                component.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }

            }
            return isExist;
        }
        private bool IsReferencedObject(UITexture component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;

            if (component.mainTexture != null &&
                component.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }

            }
            return isExist;
        }
        private bool IsReferencedObject(ParticleSystemRenderer component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }
            }

            return isExist;
        }
        private bool IsReferencedObject(ParticleRenderer component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }
            }

            return isExist;
        }
        private bool IsReferencedObject(SkinnedMeshRenderer component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }
            }

            return isExist;
        }
        private bool IsReferencedObject(MeshRenderer component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }
            }

            return isExist;
        }
        private bool IsReferencedObject(SpriteRenderer component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }
            }

            return isExist;
        }
        private bool IsReferencedObject(UnityEngine.UI.Graphic component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }
            }

            if (component.mainTexture != null &&
                component.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

            return isExist;
        }
        private bool IsReferencedObject(UIFont component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }
            }
            return isExist;
        }
        private bool IsReferencedObject(TrailRenderer component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.material != null)
            {
                if (component.material.mainTexture != null &&
                    component.material.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.material.GetTexture(input) != null &&
                        component.material.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }

            }

            return isExist;
        }
        private bool IsReferencedObject(UIAtlas component, KeyValuePair<int, TextureDetails> kvp)
        {
            bool isExist = true;
            if (component.spriteMaterial != null)
            {
                if (component.spriteMaterial.mainTexture != null &&
                    component.spriteMaterial.mainTexture.name.Contains(kvp.Value.objectName)) return !isExist;

                for (int index = 0; index < _shaderTextures.Length; index++)
                {
                    string input = _shaderTextures[index];
                    if (component.spriteMaterial.GetTexture(input) != null &&
                        component.spriteMaterial.GetTexture(input).name.Contains(kvp.Value.objectName)) return !isExist;
                }

            }

            if (component.texture != null &&
                component.texture.name.Contains(kvp.Value.objectName)) return !isExist;

            return isExist;
        }

        public void CheckLeak(Dictionary<int, TextureDetails> inputItem)
        {
            Component[] allComponent = Resources.FindObjectsOfTypeAll(typeof(Component)) as Component[];

            foreach (KeyValuePair<int, TextureDetails> kvp in inputItem)
            {
                bool flag = true;

                for (int i = 0; i < allComponent.Length; i++)
                {
                    string am = allComponent[i].GetType().ToString();

                    if (am.Equals("UISprite")) flag = IsReferencedObject(allComponent[i] as UISprite, kvp); if (!flag) break;
                    else if (am.Equals("UITexture")) flag = IsReferencedObject(allComponent[i] as UITexture, kvp); if (!flag) break;
                    else if (am.Equals("UnityEngine.ParticleSystemRenderer")) flag = IsReferencedObject(allComponent[i] as ParticleSystemRenderer, kvp); if (!flag) break;
                    else if (am.Equals("UnityEngine.ParticleRenderer")) flag = IsReferencedObject(allComponent[i] as ParticleRenderer, kvp); if (!flag) break;
                    else if (am.Equals("UnityEngine.SkinnedMeshRenderer")) flag = IsReferencedObject(allComponent[i] as SkinnedMeshRenderer, kvp); if (!flag) break;
                    else if (am.Equals("UnityEngine.MeshRenderer")) flag = IsReferencedObject(allComponent[i] as MeshRenderer, kvp); if (!flag) break;
                    else if (am.Equals("UnityEngine.SpriteRenderer")) flag = IsReferencedObject(allComponent[i] as SpriteRenderer, kvp); if (!flag) break;
                    else if (am.Equals("UnityEngine.UI.Graphic")) flag = IsReferencedObject(allComponent[i] as UnityEngine.UI.Graphic, kvp); if (!flag) break;
                    else if (am.Equals("UIFont")) flag = IsReferencedObject(allComponent[i] as UIFont, kvp); if (!flag) break;
                    else if (am.Equals("UnityEngine.TrailRenderer")) flag = IsReferencedObject(allComponent[i] as TrailRenderer, kvp); if (!flag) break;
                    else if (am.Equals("UIAtlas")) flag = IsReferencedObject(allComponent[i] as UIAtlas, kvp); if (!flag) break;
                }
                if (flag) kvp.Value.leak = true;
            }
        }

        public void CheckDublicateValue(Dictionary<int, TextureDetails> inputItem)
        {
            var duplicateValues = inputItem.GroupBy(x => x.Value.objectName).Where(x => x.Count() > 1);
            foreach (var item in duplicateValues)
            {
                foreach (KeyValuePair<int, TextureDetails> pair in inputItem)
                {
                    if (item.Key.Equals(pair.Value.objectName))
                    {
                        pair.Value.dublicate = true;
                    }
                }
            }
        }

        public Dictionary<int, TextureDetails> GetRealNewTexture(Dictionary<int, TextureDetails> newObject,
                                                                 Dictionary<int, TextureDetails> removeObject)
        {
            var copy = new Dictionary<int, TextureDetails>(newObject);
            foreach (KeyValuePair<int, TextureDetails> kvp in removeObject) 
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

        public Dictionary<int, TextureDetails> GetRealRemoveTextue(Dictionary<int, TextureDetails> newObject,
                                                                   Dictionary<int, TextureDetails> removeObject)
        {
            var copy = new Dictionary<int, TextureDetails>(removeObject);
            foreach (KeyValuePair<int, TextureDetails> kvp in newObject)
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

        public Texture FindTexture(string name)
        {
            Texture[] allTextures = Resources.FindObjectsOfTypeAll<Texture>();

            Texture returnValue = null;

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
    };
}
