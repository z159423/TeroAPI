using UnityEngine;
using System;

/// <summary>
/// 유용한 기능의 함수를 모아둠
/// </summary>
public class Util
{
    /// <summary>
    /// 유용한 기능의 함수를 모아둠
    /// </summary>   
    public static Vector2 GetDirection(Vector2 origin, Vector2 target)
    {
        return (target - origin).normalized;
    }

    /// <summary>
    /// 반올림 하는 함수
    /// </summary>   
    public static int RountToInt(float value)
    {
        return Mathf.RoundToInt(value);
    }

    /// <summary>
    /// 확률 계산 함수 램덤값보다 크면 true 반환, 작으면 false 반환
    /// </summary>   
    public static bool PercentageCalculator(int percent, int maxValue)
    {
        var random = UnityEngine.Random.Range(0, maxValue);

        if (percent > random)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 시간 차이 계산 time 매개변수에 현재 시간을 뺀값을 반환함
    /// </summary>   
    public static TimeSpan GetTimeDiff(DateTime time)
    {
        TimeSpan timeDiff = DateTime.Now - time;

        return timeDiff;
    }

    /// <summary>
    /// 초를 "시간 : 분 : 초" 문자열로 변환하여 반환하는 함수
    /// </summary>   
    public static string GetFormatedStringFromSecond(int second)
    {
        int min = second / 60;
        int hour = min / 60;
        int sec = second % 60;

        return hour + " : " + min + " : " + sec;
    }
}