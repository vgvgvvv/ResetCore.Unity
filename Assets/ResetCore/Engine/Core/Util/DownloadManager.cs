using UnityEngine;
using System.Collections;
using System.IO;
using System;
using ResetCore.Util;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

public class DownloadTask
{
    //下载地址
    public string Url { get; set; }
    //储存路径
    public string FileName { get; set; }
    public Action<float> Progress { get; set; }
    public String MD5 { get; set; }
    public Action<bool> Finished { get; set; }
    public Action<Exception> Error { get; set; }

    public bool hasMD5 = true;
    public bool bFineshed = false;//文件是否下载完成
    public bool bDownloadAgain = false;//是否需要从新下载，如果下载出错的时候会从新下

    public int tryTimes = 0;//重试次数

    public void OnProgress(float p)
    {
        if (Progress != null)
            Progress(p);
    }

    public void OnFinished(bool comp)
    {
        if (Finished != null)
            Finished(comp);
    }

    public void OnError(Exception ex)
    {
        if (Error != null)
            Error(ex);
    }

}


public class DownloadManager : Singleton<DownloadManager> {

    private readonly WebClient webClient = new WebClient ();

    public List<DownloadTask> taskList { get; set; }

    public static readonly int MaxTryTime = 5;

    public override void Init()
    {
        base.Init();
        taskList = new List<DownloadTask>();
    }

    private Action finishedAct;
    private Action<int, int, string> progressAct;
    /// <summary>
    /// 添加新的下载任务
    /// </summary>
    /// <param name="url">下载地址</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="md5">MD5（为空时则没有MD5检查）</param>
    /// <param name="progress">下载进度回调</param>
    /// <param name="finished">结束回调</param>
    public DownloadManager AddNewDownloadTask(string url, string filePath, string md5 = null
        , Action<float> progress = null, Action<bool> finished = null)
    {
        DownloadTask task = new DownloadTask();
        task.Url = url;
        task.FileName = filePath;
        task.MD5 = md5;
        task.hasMD5 = (md5 != null);
        task.Progress = progress;
        task.Finished = finished;
        taskList.Add(task);
        return this;
    }
    /// <summary>
    /// 检查当前下载列表
    /// </summary>
    public void CheckDownLoadList(Action finishedAct = null, Action<int, int, string> progressAct = null)
    {
        this.finishedAct = finishedAct;
        this.progressAct = progressAct;

        if (taskList.Count == 0)
            return;

        var finishedCount = 0;//已经完成的数目

        //移动到还未下载完的第一个任务
        foreach (var task in taskList)
        {
            if (task.bFineshed && !task.bDownloadAgain)
            {
                finishedCount++;
            }
            else
            {
                if (progressAct != null)
                {
                    var filename = task.FileName.Substring(task.FileName.LastIndexOf("/") + 1);
                    progressAct(taskList.Count, finishedCount, filename);
                }

                //判断下载文件的文件夹是否存在
                var dirName = Path.GetDirectoryName(task.FileName);
                PathEx.MakeFileDirectoryExist(dirName);

                Action downloadAct = () => 
                {
                    DoDownloadTask(task);
                };

                Thread thread = new Thread(downloadAct.Invoke);
                thread.Start();

                break;
            }

        }
        if (finishedCount > taskList.Count - 1)
        {
            taskList.Clear();
            taskList = null;
            if (finishedAct != null)
            {
                finishedAct();
                finishedAct = null;
            }
        }

    }

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="task"></param>
    private void DoDownloadTask(DownloadTask task)
    {
        string fileName = task.FileName;
        string address = task.Url;

        try
        {
            var resquestUrl = new Uri(address);
            var request = (HttpWebRequest)WebRequest.Create(resquestUrl);
            var response = (HttpWebResponse)request.GetResponse();

            var contentLength = response.ContentLength;
            response.Close();
            request.Abort();

            //剩余文件长度
            var leftSize = contentLength;
            //开始读写的位置
            long position = 0;

            ///检查是否已经有下载过的文件
            CheckIfHasDownloadedFile(fileName, contentLength, ref leftSize, ref position);

            ////从response中读取字节流
            ReadBytesFromResponse(task, position, leftSize);
           
            
            ////下载完成
            TaskFinish(task);


        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            this.TaskFinish(task, ex);
        }
    }

    #region 私有函数
    //从断点开始继续下载
    private static void CheckIfHasDownloadedFile(string fileName, long contentLength, ref long leftSize, ref long position)
    {
        if (File.Exists(fileName))
        {
            Debug.unityLogger.Log("需要下载的文件存在：" + fileName);
            using (
                var sw = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                                    FileShare.ReadWrite))
            {
                leftSize = contentLength - sw.Length;
                position = sw.Length;
            }
        }
    }

    private void ReadBytesFromResponse(DownloadTask task, long position, long leftSize)
    {
        if (leftSize <= 0)
        {
            return;
        }

        string requestURL = task.Url;
        string fileName = task.FileName;
        long totalSize = position + leftSize;

        var partRequest = (HttpWebRequest)WebRequest.Create(new Uri(requestURL));

        partRequest.AddRange((int)position, (int)(position + leftSize));
        var partResponse = (HttpWebResponse)partRequest.GetResponse();

        try
        {
            var bufferLength = (int)leftSize;
            var buffer = new byte[bufferLength];

            //本块的位置指针
            var currentChunkPointer = 0;
            var offset = 0;
            using (var respStream = partResponse.GetResponseStream())
            {
                int receivedBytesCount;
                do
                {
                    receivedBytesCount = respStream.Read(buffer, offset, bufferLength - offset);

                    offset += receivedBytesCount;

                    if (receivedBytesCount > 0)
                    {
                        var bufferCopyed = new byte[receivedBytesCount];
                        Buffer.BlockCopy(buffer, currentChunkPointer, bufferCopyed, 0, bufferCopyed.Length);

                        //写数据流到文件中
                        using (var sw = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            sw.Position = position;
                            sw.Write(bufferCopyed, 0, bufferCopyed.Length);
                            sw.Close();
                        }

                        var progress = ((int)position + bufferCopyed.Length) /
                            ((float)totalSize);

                        //触发数据到达事件
                        task.OnProgress(progress);



                        currentChunkPointer += receivedBytesCount; //本块的位置指针 
                        position += receivedBytesCount; //整个文件的位置指针 
                    }
                } while (receivedBytesCount != 0);
            }
        }
        catch (Exception e)
        {
            Debug.unityLogger.LogError("", "ReadBytesFromResponse Error：" + e.Message);
            TaskFinish(task, e);
        }
        finally
        {
            partResponse.Close();
            partRequest.Abort();
        }
    }

    private void TaskFinish(DownloadTask task, Exception e = null)
    {
        if (task == null)
            return;
        if (e != null)
        {
            Debug.Log("下载出错" + e.Message);
            Debug.LogException(e);
            task.OnError(e);
            task.Finished(false);
        }
        else
        {
            DownloadFinishWithMd5(task);
        }


    }

    static void HandleNetworkError(Exception e, Action mycontinue, Action again, Action finished = null)
    {
        if (e.Message.Contains("ConnectFailure") //连接失败
                || e.Message.Contains("NameResolutionFailure") //域名解析失败
                || e.Message.Contains("No route to host")) //找不到主机
        {
            Debug.unityLogger.LogError("下载失败", "无法找到服务器");
        }
        else if (e.Message.Contains("(404) Not Found") || e.Message.Contains("403"))
        {
            Debug.unityLogger.LogError("下载失败", "404错误");
        }
        else if (e.Message.Contains("Disk full"))
        {
            Debug.unityLogger.LogError("下载失败", "磁盘已满");
        }
        else if (e.Message.Contains("timed out") || e.Message.Contains("Error getting response stream"))
        {
            Debug.unityLogger.LogError("下载失败", "下载超时");
        }
        else if (e.Message.Contains("Sharing violation on path"))
        {
            Debug.unityLogger.LogError("下载失败", "Sharing violation on path");
        }
        else
        {
            Debug.unityLogger.LogError("下载失败", "未知错误");
        }
    }

    private void DownloadFinishWithMd5(DownloadTask task)
    {


#if UNITY_IPHONE
		//ios下如果封装该方法在一个函数中，调用该函数来产生文件的MD5的时候，就会抛JIT异常。
		//如果直接把这个产生MD5的方法体放在直接执行，就可以正常执行，这个原因还不清楚。
		string md5Compute = null;
		using(System.IO.FileStream fileStream = System.IO.File.OpenRead(task.FileName))
		{
			System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
			byte[] fileMD5Bytes = md5.ComputeHash(fileStream);
			md5Compute = System.BitConverter.ToString(fileMD5Bytes).Replace("-", "").ToLower();
		}
#else
        var md5Compute = MD5Utils.BuildFileMd5(task.FileName);
#endif

        if (task.hasMD5 && md5Compute.Trim() != task.MD5.Trim())
        {
            task.tryTimes++;
            if (task.tryTimes > MaxTryTime)
            {
                Debug.unityLogger.Log("重试" + MaxTryTime + "无效, 停止重试");
                task.bDownloadAgain = false;
                task.bFineshed = true;
                task.OnError(new Exception("md5验证失败，下载失败"));
                task.OnFinished(false);
            }
            else
            {
                if (File.Exists(task.FileName))
                    File.Delete(task.FileName);

                Debug.Log("断点MD5验证失败，第" + task.tryTimes + "次重试，从新下载：" + task.FileName + "--" + md5Compute + " vs " + task.MD5);

                task.bDownloadAgain = true;
                task.bFineshed = false;
            }
        }
        else
        {
            task.bDownloadAgain = false;
            task.bFineshed = true;
            task.OnFinished(true);
        }

        CheckDownLoadList(finishedAct, progressAct);
    }
    #endregion
    /// <summary>
    /// 清空下载列表
    /// </summary>
    public void ClearTaskList()
    {
        taskList = new List<DownloadTask>();
    }

    /// <summary>
    /// 异步方式下载文本
    /// </summary>
    /// <param name="url"></param>
    /// <param name="asynResult"></param>
    /// <param name="OnError"></param>
    public void AsynDownLoadText(string url, Action<string> asynResult, Action OnError)
    {
        Action action = () =>
        {
            var u = url;
            var result = DownLoadText(u);
            if (String.IsNullOrEmpty(result))
            {
                if (OnError != null)
                    OnError();
                Debug.Log(u + " 文件下载失败");

            }
            else
            {
                Debug.Log(u + " 文件下载成功并返回");
                if (asynResult != null)
                    asynResult(result);
            }
        };

        action.BeginInvoke(null, null);
       

    }

    /// <summary>
    /// 同步方式下载文本
    /// </summary>
    /// <param name="url"></param>
    /// <param name="OnError"></param>
    /// <returns></returns>
    public string DownLoadText(String url, Action<Exception> OnError = null)
    {
        try
        {
            string str = webClient.DownloadString(url);

            return str;
        }
        catch (Exception ex)
        {
            OnError(ex);
            Debug.LogException(ex);
            return String.Empty;
        }
    }

}
