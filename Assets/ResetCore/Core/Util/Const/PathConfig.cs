using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using ResetCore.ModuleControl;
using ResetCore.Util;
using ResetCore.Asset;

public class PathConfig
{

    #region 全局
    //插件统一文件夹名
    public static string PluginsFolderName = "Plugins";

    //ResetCore根目录
    public static string ResetCorePath = Application.dataPath + "/ResetCore/";
    //ResetCore备份目录
    public static string ResetCoreTempPath = Path.Combine(projectPath, "ResetCoreTemp");
    //ResetCore备份根目录
    public static string ResetCoreBackUpPath = Path.Combine(ResetCoreTempPath, "Backup");

    //Extra工具包内目录
    public static string ExtraToolPathInPackage = Path.Combine(ResetCorePath, "ExtraTool.zip");
    //Extra工具根目录
    public static string ExtraToolPath = Path.Combine(ResetCoreTempPath, "ExtraTool");

    //SDK工具包内目录
    public static string SDKPathInPackage = Path.Combine(ResetCorePath, "SDK.zip");
    //SDK工具备份目录
    public static string SDKBackupPath = Path.Combine(ResetCoreTempPath, "SDK");
    //SDK工具安装目录
    public static string SDKPath = Path.Combine(Application.dataPath, "SDK");

    //Plugins目录
    public static readonly string pluginPath = Path.Combine(Application.dataPath, PluginsFolderName); 

    //工程目录
    public static string projectPath
    {
        get
        {
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            return directory.Parent.FullName.Replace("\\", "/");
        }
    }
    //沙盒目录
    public static readonly string persistentDataPath = 
        Path.Combine(Application.persistentDataPath, Application.productName);

    public static readonly string assetResourcePath = "Assets/Resources/";

    #endregion

    #region AssetBundle 相关
    //AssetBundleManifest的文件名
    public static readonly string assetBundleManifest_FileName = "AssetBundleManifest";
    //AssetBundleManifest的扩展名
    public static readonly string assetBundleManifest_ExName = ".manifest";
    //Resources路径
    public static readonly string resourcePath = Application.dataPath + "/Resources/";
    //Bundle根目录
    public static readonly string resourceBundlePath = "Data/BundleData/";
    //资源列表目录
    public static readonly string resourceListDocPath = resourceBundlePath + "ResourcesList";
    //场景记录文件储存目录
    public static readonly string sceneXmlRootPath = resourceBundlePath + "SceneData/";
    //预置组件信息文件储存目录
    public static readonly string compInfoObjRootPath = resourceBundlePath + "PrefabCompData/";
    //相对于Assets到Resources文件夹的相对路径
    public static readonly string assetsToResources = "Asset/Resources";

    //AssetBundle导出文件夹
    public static readonly string bundleFolderName = "AssetBundle";
#if UNITY_EDITOR    
    public static string bundleRootPath = Path.Combine(projectPath, bundleFolderName).Replace("\\", "/");
#else
    public static string bundleRootPath = Path.Combine(persistentDataPath, bundleFolderName).Replace("\\", "/");
#endif

    public static readonly string AssetRootBundleFilePath = PathConfig.bundleRootPath + "/" + bundleFolderName;


    //Bundle包导出文件夹名称
    public static readonly string bundleExportFolderName = "AssetBundleExport";
    //BundlePkg包导出路径
    public static readonly string bundlePkgExportPath = Path.Combine(projectPath, bundleExportFolderName).Replace("\\", "/");

    //版本信息储存地址
    public static readonly string VersionDataName = "VersionData";
    public static readonly string VersionDataPathInResources = "VersionData/" + VersionDataName;
    public static readonly string VersionDataResourcesPath = PathEx.Combine(ModuleConst.GetSymbolPath(MODULE_SYMBOL.ASSET), "Resources").Replace("\\", "/");

    //沙盒目录下版本信息
    public static readonly string LocalVersionDataInPersistentDataPath = Path.Combine(persistentDataPath, VersionDataName + ".xml");

    //资源文件夹名
    public static readonly string bundleResourcesFolderPath = "Resources";

#endregion

#region GameData相关

    public enum DataType
    {
        Xml,
        Obj,
        Protobuf,
        Json,
        Pref,
        Localization,
        Core
    }

    //Excelm默认存放地址
    public static readonly string localGameDataExcelPath = Application.dataPath + "/Excel/";

    //Data专用的Resources
    public static readonly string localDataResourcesPath = ResetCorePath + "Core/GameDatas/Resources/";
    public static readonly string loacalDataPathInResources = "GameData/";

    //游戏数据根目录
    public static readonly string localGameDataSourceRoot = localDataResourcesPath + loacalDataPathInResources;
    //游戏数据类文件根目录
    public static readonly string localGameDataClassRoot = ResetCorePath + "Core/GameDatas/DataClasses/";

    //存放游戏数据源文件的目录
    public static string GetLocalGameDataPath(DataType type)
    {
        return localGameDataSourceRoot + type.ToString() + "/";
    }
    //存放GameDataClass的路径
    public static string GetLoaclGameDataClassPath(DataType type)
    {
        return localGameDataClassRoot + type.ToString() + "/";
    }
    //获取相对于Resources的路径
    public static string GetLocalGameDataResourcesPath(DataType type)
    {
        return loacalDataPathInResources + type.ToString() + "/";
    }

    //存放核心数据备份的地址
    public static readonly string localCoreDataBackupPath = ResetCorePath + "Core/GameDatas/CoreData/Datas/";

    /// <summary>
    /// 本地化数据存放地址
    /// </summary>
    public static readonly string LanguageDataExcelPath = 
        PathEx.Combine(ResetCorePath, ModuleConst.SymbolFoldNames[MODULE_SYMBOL.DATA_GENER], "Localization/Excel/LocalizationData.xlsx");

    //存放本地化数据的地址
    public static readonly string LanguageDataPath = 
        PathConfig.GetLocalGameDataPath(DataType.Localization) + "LocalizationData.xml";
    //存放PrefData GameData类的地址
    public static readonly string localLanguageDataClassPath = 
        PathConfig.GetLoaclGameDataClassPath(DataType.Localization) + "LocalizationData.cs";

#endregion

#region 工具
    //Lua模板资源路径
    public static readonly string luaScriptAssetPath = ResetCorePath + "Lua/Editor/LuaAsset.lua".Replace(projectPath, "");
    //Xml模板资源路径
    public static readonly string xmlScriptAssetPath = ResetCorePath + "Core/DataSupport/Xml/Editor/XmlAsset.xml".Replace(projectPath, "");
    //Json模板资源路径
    public static readonly string jsonScriptAssetPath = ResetCorePath + "Core/DataSupport/Json/Editor/JsonAsset.json".Replace(projectPath, "");

    public static readonly string csToolPath = ExtraToolPath + "C#Tools/ExcelDataManager.exe";
    public static readonly string csTool_GameDataViaExcel = "GameDataGen";
#endregion
}
