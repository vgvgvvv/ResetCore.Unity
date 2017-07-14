using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using ResetCore.Util;
using System.Xml.Linq;
using ResetCore.Xml;
using System.Collections.Generic;
using ResetCore.Asset;

namespace ResetCore.UGUI
{
    public class SpritePackerWindow : EditorWindow
    {
        //记录了所有种类的sprite集合
        private static readonly string spriteNameListFilePath = "SpritePacker/PackageList";
        //绝对路径
        private static readonly string spriteNameListFileAbstractPath = Path.Combine(UIConst.ResourcesPath, spriteNameListFilePath + ".xml");

        //当前sprite包名
        private string currentPackageName
        {
            get { return packageList[currentPkgIndex]; }
        }

        int currentPkgIndex = 0;

        //包列表
        private string[] _packageList;
        private string[] packageList
        {
            get 
            {
                XDocument xDoc = XDocumentEx.LoadOrCreate(spriteNameListFileAbstractPath);
                string[] result = xDoc.ReadValueFromXML<string[]>(new string[] { "PackageListName" });
                _packageList = result == null ? new string[0] : result;
                return _packageList;
            }
            set
            {
                XDocument xDoc = XDocumentEx.LoadOrCreate(spriteNameListFileAbstractPath);
                xDoc.WriteValue<string[]>(new string[] { "PackageListName" }, value)
                    .SafeSaveWithoutDeclaration(spriteNameListFileAbstractPath);
            }
        }
        
        //当前包内sprite列表
        private List<string> _spriteList;
        private XDocument xDoc;
        private List<string> spriteList
        {
            get
            {
                if (packageList.Length <= 0) return new List<string>();
                XDocument xDoc = XDocumentEx.LoadOrCreate(spriteNameListFileAbstractPath);
                List<string> result = xDoc.ReadValueFromXML<List<string>>(new string[] { "SpriteList", currentPackageName });
                _spriteList = result == null ? new List<string>() : result;
                return _spriteList;
            }
            set
            {
                XDocument xDoc = XDocumentEx.LoadOrCreate(spriteNameListFileAbstractPath);
                xDoc.WriteValue<List<string>>(new string[] { "SpriteList", currentPackageName }, value)
                    .SafeSaveWithoutDeclaration(spriteNameListFileAbstractPath);
            }
        }

        //private List<Sprite> spriteTexList = new List<Sprite>();



        //显示窗口的函数
        [MenuItem("Tools/UGUI/Extra/Sprite Packer")]
        static void ShowMainWindow()
        {
            SpritePackerWindow window =
                EditorWindow.GetWindow(typeof(SpritePackerWindow), false, "Sprite Packer") as SpritePackerWindow;
            
            window.Show();
        }

        bool hasInit = false;
        void Init()
        {
            if (hasInit) return;
            hasInit = true;
        }


        #region OnGUI
        void OnGUI()
        {
            Init();
            ShowSelectSprite();
            ShowSpriteList();
            ShowSpritePackageTools();
        }

        string newPackageName = string.Empty;
        private void ShowSelectSprite()
        {
            if (packageList.Length > 0)
            {
                GUILayout.Label("Select Your Sprite Package", GUIHelper.MakeHeader(30));
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                currentPkgIndex = EditorGUILayout.Popup(currentPkgIndex, packageList, GUILayout.MaxWidth(200));
                GUILayout.EndHorizontal();
                GUILayout.Label("Current package has " + spriteList.Count + " sprites", GUIHelper.MakeHeader(30));
            }
            else
            {
                List<string> temp = new List<string>(packageList);
                if (string.IsNullOrEmpty(UIConst.defaultPackage.Trim())) return;
                temp.Add(UIConst.defaultPackage);
                packageList = (temp.ToArray());
                Refresh();
            }
          

            GUILayout.BeginHorizontal();
            newPackageName = EditorGUILayout.TextField("New Package Name", newPackageName, GUILayout.MaxWidth(200));
            if (GUILayout.Button("Create", GUILayout.MaxWidth(200)))
            {
                List<string> temp = new List<string>(packageList);
                if (string.IsNullOrEmpty(newPackageName.Trim())) return;
                temp.Add(newPackageName);
                packageList = (temp.ToArray());
                Refresh();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        int currentPage = 0;
        int totalPages;

        int line = 5;
        int row = 5;

        Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
        private void ShowSpriteList()
        {
            GUILayout.Label("Sprite List", GUIHelper.MakeHeader(30));
            GUILayout.Space(10);
            
            totalPages = Mathf.CeilToInt(spriteList.Count / (float)(line * row));
            for (int curRow = 0; curRow < row; curRow++)
            {
                GUILayout.BeginHorizontal();
                for (int curLine = 0; curLine < line; curLine++)
                {
                    int num = currentPage * line * row + curRow * line + curLine;
                    if (num >= spriteList.Count || num < 0)
                    {
                        continue;
                    }
                    string spritePath = spriteList[num];

                    Sprite spriteTex;
                    if (!spriteDict.ContainsKey(spritePath))
                    {
                        spriteTex = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                        spriteDict.Add(spritePath, spriteTex);
                    }
                    else
                    {
                        spriteTex = spriteDict[spritePath];
                    }

                    if (spriteTex == null)
                    {
                        spriteList.Remove(spritePath);
                        spriteList = _spriteList;
                        continue;
                    }

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(spriteTex.texture, GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        AssetDatabase.OpenAsset(spriteTex);
                    }

                    GUILayout.BeginVertical();
                    if (GUILayout.Button("Remove", GUILayout.Width(100), GUILayout.Height(23)))
                    {
                        spriteList.Remove(spritePath);
                        spriteList = _spriteList;
                    }
                    if (GUILayout.Button("ReBuild", GUILayout.Width(100), GUILayout.Height(23)))
                    {
                        GenSpritePrefab(spritePath);
                        AssetDatabase.Refresh();
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }//for (int curLine = 0; curLine < line; line++)
                GUILayout.EndHorizontal();
            }//for (int curRow = 0; curRow < row; row++ )

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev Page", GUILayout.Width(100), GUILayout.Height(50)))
            {
                if (currentPage-1 >= 0)
                    currentPage--;
            }
            if (GUILayout.Button("Next Page", GUILayout.Width(100), GUILayout.Height(50)))
            {
                if (currentPage + 1 < totalPages)
                    currentPage++;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void ShowSpritePackageTools()
        {
            GUILayout.Label("Handle Your Sprite Package", GUIHelper.MakeHeader(30));
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            //添加所选
            if (GUILayout.Button("Add Select", GUILayout.MaxWidth(200)))
            {
                string[] paths = SelectionHelper.SelectWithCondition((obj) =>
                {
                    if (obj == null) return false;
                    string path = AssetDatabase.GetAssetPath(obj);
                    Texture2D tex = obj as Texture2D;
                    if (tex == null) return false;
                    return true;
                });

                for (int i = 0; i < paths.Length; i++ )
                {
                    string texAssetPath = paths[i];
                    EditorUtility.DisplayProgressBar("Add Sprite Prefab", i + "/" + paths.Length + " " + texAssetPath, i / (float)paths.Length);
                    if (spriteList.Contains(texAssetPath)) continue;
                    if (GenSpritePrefab(texAssetPath))
                    {
                        spriteList.Add(texAssetPath);
                        spriteList = _spriteList;
                    }
                }
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
            //全部重新生成
            if (GUILayout.Button("ReBuild All", GUILayout.MaxWidth(200)))
            {
                string path = PathEx.Combine(UIConst.spritePrefabPathAbstractPath, currentPackageName);
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
                for (int i = 0; i < spriteList.Count; i++)
                {
                    string texAssetPath = spriteList[i];
                    EditorUtility.DisplayProgressBar("Gen Sprite Prefab", i + "/" + spriteList.Count + " " + texAssetPath, i / (float)spriteList.Count);
                    GenSpritePrefab(texAssetPath);
                }
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
            //全部重新生成
            if (GUILayout.Button("Clean All", GUILayout.MaxWidth(200)))
            {
                string path = PathEx.Combine(UIConst.spritePrefabPathAbstractPath, currentPackageName);
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();
        }
        #endregion



        private void Refresh()
        {
            hasInit = false;
        }

        //创建SpritePrefab
        private bool GenSpritePrefab(string texPath)
        {
            string spriteName = Path.GetFileNameWithoutExtension(texPath);

            Object[] spriteList = AssetDatabase.LoadAllAssetsAtPath(texPath);

            foreach (Object sp in spriteList)
            {
                if (!(sp is Sprite)) 
                    continue;

                GameObject go = new GameObject(sp.name);
                SpriteRenderer spriterenderer = go.AddComponent<SpriteRenderer>();

                spriterenderer.sprite = sp as Sprite;

                string savePath = GetSpritePrefabPath(currentPackageName + "-" + sp.name);
                PathEx.MakeDirectoryExist(savePath);
                PrefabUtility.CreatePrefab(savePath, go);
                DestroyImmediate(go);
            }
            return true;
        }
        //获取SpritePrefab的路径
        private string GetSpritePrefabPath(string spriteName)
        {
            return PathEx.ConvertAbstractToAssetPath(PathEx.Combine(UIConst.spritePrefabPathAbstractPath, currentPackageName, spriteName + ".prefab"));
        }

        private bool ContainSpriteOPrefab(string spriteName)
        {
            return AssetDatabase.LoadAssetAtPath<Object>(GetSpritePrefabPath(spriteName)) != null;
        }
    }

}
