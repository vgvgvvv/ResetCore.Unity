using UnityEngine;
using System.Collections;
using System;

public class GPSUtil {

    /// <summary>
    /// 计算两个GPS数据之间的距离
    /// 算法来自 http://blog.csdn.net/yueqinglkong/article/details/10163739
    /// </summary>
    /// <param name="lat1"></param>
    /// <param name="log1"></param>
    /// <param name="lat2"></param>
    /// <param name="log2"></param>
    /// <returns></returns>
    public static double GPSDistance(double lat1, double log1, double lat2, double log2)
    {
         //地球半径   
        double R=6378137.0;   
        //将角度转化为弧度  
        double radLat1=(lat1*Math.PI/180.0);  
        double radLat2=(lat2*Math.PI/180.0);  
        double radLog1=(log1*Math.PI/180.0);  
        double radLog2=(log2*Math.PI/180.0);  
        //纬度的差值  
        double a=radLat1-radLat2;  
        //经度差值  
        double b=radLog1-radLog2;  
        //弧度长度  
        double s=2*Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a/2), 2)+Math.Cos(radLat1)*Math.Cos(radLat2)*Math.Pow(Math.Sin(b/2), 2)));  
        //获取长度  
        s=s*R;  
        //返回最接近参数的 long。结果将舍入为整数：加上 1/2  
        s=s*10000/10000;
        return s; 
    }
}
