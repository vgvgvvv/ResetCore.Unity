using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ResetCore.Xml;
using System.Xml.Linq;
using ResetCore.Util;

namespace ResetCore.PlatformHelper
{
    public class BuildAndroid
    {
        [MenuItem("Tools/Export")]
        public static void ExportAndroid()
        {
            string path = Application.dataPath + "/../Export";
            Export(path, new Dictionary<string, string>()
            {
                { "scenes", ""},
                { "develop", "false"},
                { "profile", "false"},
                { "configPath", @"D:\Documents\Unity5Projects\EscapeSDK\Reset\Build.xml"},
            });
        }
        
        public static void Export(string outputPath, Dictionary<string, string> args)
        {
            
            

            var scenes = args["scenes"].GetValue<string[]>();//导出的场景
            var develop = args["develop"].TryGetValue<bool>(false);//是否开发者模式
            HandleDevelop(develop);
            var profile = args["profile"].TryGetValue<bool>(false);//是否打开profile
            HandleProfile(profile);
            var configPath = args["configPath"];

            XDocument xDoc = XDocument.Load(configPath);
            BuildFile config = XmlUtil.Deserialize<BuildFile>(xDoc);

            PlayerSettings.applicationIdentifier = config.packageName;
            PlayerSettings.productName = config.appName;
            PlayerSettings.bundleVersion = config.versionCode;
            PlayerSettings.iOS.buildNumber = config.versionCode;

            if(Application.platform != RuntimePlatform.Android)
            {
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
            }
            
            BuildPipeline.BuildPlayer(scenes, outputPath, BuildTarget.Android, BuildOptions.None 
                | BuildOptions.AcceptExternalModificationsToPlayer);
        }
        
        private static void HandleDevelop(bool develop)
        {

        }

        private static void HandleProfile(bool profile)
        {

        }
    }

}
