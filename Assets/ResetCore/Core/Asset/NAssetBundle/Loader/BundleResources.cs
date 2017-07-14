using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace ResetCore.NAsset
{
    /// <summary>
    /// 资源包管理器
    /// </summary>
    public class BundleResources
    {
        /// <summary>
        /// 资源包管理器
        /// </summary>
        public AssetBundle Bundle;

        /// <summary>
        /// 资源表
        /// </summary>
        private Hashtable assetTable = new Hashtable();

        /// <summary>
        /// 生成新的资源包管理器
        /// </summary>
        /// <param name="bundle">资源包</param>
        public BundleResources(AssetBundle bundle)
        {
            Bundle = bundle;

            //解析资源包
            //ParseAssetBundle();

            assetTable[typeof(Sprite)] = new Hashtable();
            assetTable[typeof(TextAsset)] = new Hashtable();
            assetTable[typeof(AudioClip)] = new Hashtable();
            assetTable[typeof(Material)] = new Hashtable();
            assetTable[typeof(Shader)] = new Hashtable();
            assetTable[typeof(GameObject)] = new Hashtable();
        }

        /// <summary>
        /// 解析资源包
        /// </summary>
        public void ParseAssetBundle()
        {
            //解析图片
            Sprite[] sprites = Bundle.LoadAllAssets<Sprite>();
            Hashtable spriteTable = new Hashtable();
            foreach (Sprite sprite in sprites)
                spriteTable[sprite.name] = sprite;
            assetTable[typeof(Sprite)] = spriteTable;

            //解析文本
            TextAsset[] texts = Bundle.LoadAllAssets<TextAsset>();
            Hashtable textTable = new Hashtable();
            foreach (TextAsset text in texts)
                textTable[text.name] = text;
            assetTable[typeof(TextAsset)] = textTable;

            //解析音频
            AudioClip[] audios = Bundle.LoadAllAssets<AudioClip>();
            Hashtable audioTable = new Hashtable();
            foreach (AudioClip audio in audios)
                audioTable[audio.name] = audio;
            assetTable[typeof(AudioClip)] = audioTable;

            //解析材质
            Material[] mats = Bundle.LoadAllAssets<Material>();
            Hashtable matTable = new Hashtable();
            foreach (Material mat in mats)
                matTable[mat.name] = mat;
            assetTable[typeof(Material)] = matTable;

            //解析Shader
            Shader[] shaders = Bundle.LoadAllAssets<Shader>();
            Hashtable shaderTable = new Hashtable();
            foreach (Shader shader in shaders)
                shaderTable[shader.name] = shader;
            assetTable[typeof(Shader)] = shaderTable;

            //解析GameObject
            GameObject[] objs = Bundle.LoadAllAssets<GameObject>();
            Hashtable objTable = new Hashtable();
            foreach (GameObject obj in objs)
                objTable[obj.name] = obj;
            assetTable[typeof(GameObject)] = objTable;
        }

        /// <summary>
        /// 获取精灵图片
        /// </summary>
        /// <param name="name">图片名称</param>
        /// <returns>精灵图片</returns>
        public Sprite GetSprite(string name, string fatherName = null)
        {
            Hashtable spriteTable = assetTable[typeof(Sprite)] as Hashtable;

            if (spriteTable[name] != null)
                return spriteTable[name] as Sprite;

            Sprite sprite = Bundle.LoadAsset<Sprite>(name);

            if (sprite == null && fatherName != null)
            {
                Sprite[] sps = Bundle.LoadAssetWithSubAssets<Sprite>(fatherName);
                foreach (Sprite sp in sps)
                    if (sp.name == name)
                    {
                        sprite = sp;
                        break;
                    }
            }

            if (sprite != null)
                spriteTable[name] = sprite;

            return sprite;
        }


        /// <summary>
        /// 获取Texture
        /// </summary>
        /// <param name="name">图片名称</param>
        /// <returns>Texture</returns>
        public Texture GetTexture(string name, string fatherName = null)
        {
            Hashtable spriteTable = assetTable[typeof(Texture)] as Hashtable;

            if (spriteTable[name] != null)
                return spriteTable[name] as Texture;

            Texture sprite = Bundle.LoadAsset<Texture>(name);

            if (sprite == null && fatherName != null)
            {
                Texture[] sps = Bundle.LoadAssetWithSubAssets<Texture>(fatherName);
                foreach (Texture sp in sps)
                    if (sp.name == name)
                    {
                        sprite = sp;
                        break;
                    }
            }

            if (sprite != null)
                spriteTable[name] = sprite;

            return sprite;
        }

        /// <summary>
        /// 获取文本对象
        /// </summary>
        /// <param name="name">文本名</param>
        /// <returns>文本对象</returns>
        public TextAsset GetText(string name)
        {
            Hashtable spriteTable = assetTable[typeof(TextAsset)] as Hashtable;

            if (spriteTable[name] != null)
                return spriteTable[name] as TextAsset;

            TextAsset text = Bundle.LoadAsset<TextAsset>(name);

            if (text != null)
                spriteTable[name] = text;

            return text;
        }

        /// <summary>
        /// 获取声音对象
        /// </summary>
        /// <param name="name">对象名</param>
        /// <returns>声音对象</returns>
        public AudioClip GetAudio(string name)
        {
            Hashtable spriteTable = assetTable[typeof(AudioClip)] as Hashtable;

            if (spriteTable[name] != null)
                return spriteTable[name] as AudioClip;

            AudioClip audio = Bundle.LoadAsset<AudioClip>(name);

            if (audio != null)
                spriteTable[name] = audio;

            return audio;
        }

        /// <summary>
        /// 获取材质对象
        /// </summary>
        /// <param name="name">对象名</param>
        /// <returns>对象</returns>
        public Material GetMaterial(string name)
        {
            Hashtable matTable = assetTable[typeof(Material)] as Hashtable;

            if (matTable[name] != null)
                return matTable[name] as Material;

            Material mat = Bundle.LoadAsset<Material>(name);

            if (mat != null)
                matTable[name] = mat;

            return mat;
        }

        /// <summary>
        /// 获取Shader对象
        /// </summary>
        /// <param name="name">对象名</param>
        /// <returns>对象</returns>
        public Shader GetShader(string name)
        {
            Hashtable shaderTable = assetTable[typeof(Shader)] as Hashtable;

            if (shaderTable[name] != null)
                return shaderTable[name] as Shader;

            Shader shader = Bundle.LoadAsset<Shader>(name);

            if (shader != null)
                shaderTable[name] = shader;

            return shader;
        }

        /// <summary>
        /// 获取GameObject对象
        /// </summary>
        /// <param name="name">对象名</param>
        /// <returns>对象</returns>
        public GameObject GetGameObject(string name)
        {
            Hashtable objTable = assetTable[typeof(GameObject)] as Hashtable;

            if (objTable[name] != null)
                return objTable[name] as GameObject;

            GameObject obj = Bundle.LoadAsset<GameObject>(name);

            if (obj != null)
                objTable[name] = obj;

            return obj;
        }

        public void Reset()
        {
            Hashtable spriteTable = assetTable[typeof(Sprite)] as Hashtable;
            Hashtable audioTable = assetTable[typeof(AudioClip)] as Hashtable;
            Hashtable textureTable = assetTable[typeof(Texture)] as Hashtable;
            Hashtable textTable = assetTable[typeof(TextAsset)] as Hashtable;
            Hashtable matTable = assetTable[typeof(Material)] as Hashtable;
            Hashtable shaderTable = assetTable[typeof(Shader)] as Hashtable;
            Hashtable objTable = assetTable[typeof(GameObject)] as Hashtable;

            if (spriteTable != null)
                spriteTable.Clear();
            if (audioTable != null)
                audioTable.Clear();
            if (textureTable != null)
                textureTable.Clear();
            if (textTable != null)
                textTable.Clear();
            if (matTable != null)
                matTable.Clear();
            if (shaderTable != null)
                shaderTable.Clear();
            if (objTable != null)
                objTable.Clear();

            assetTable.Clear();
        }
    }
}
