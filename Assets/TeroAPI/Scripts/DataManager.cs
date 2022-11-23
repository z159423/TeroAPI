using UnityEngine;
using System.IO;

/// <summary>
/// 로컬에 유저 데이터를 저장하는 클래스
/// </summary>
public class DataManager
{
    //로컬에 저장할 데이터파일 이름
    private const string userDataName = "Data";

    /// <summary>
    /// 로컬에 있는 유저 데이터를 가져옵니다.
    /// </summary>
    public static UserData LoadUserData()
    {
        string filePath = Application.persistentDataPath + userDataName;

        if (File.Exists(filePath))
        {
            Debug.Log("로컬 데이터를 불러왔습니다. : " + filePath);
            string JsonData = File.ReadAllText(filePath);
            UserData userData = JsonUtility.FromJson<UserData>(JsonData);

            return userData;
        }
        else
        {
            Debug.LogWarning("로컬 데이터가 없어 새로 생성합니다. : " + filePath);
            UserData userData = new UserData();

            SaveUserData(userData);

            return userData;
        }
    }

    /// <summary>
    /// 해당 데이터를 로컬에 저장합니다.
    /// </summary>
    public static void SaveUserData(UserData data)
    {
        string filePath = Application.persistentDataPath + userDataName;

        string JsonData = JsonUtility.ToJson(data);

        File.WriteAllText(filePath, JsonData);
    }

    /// <summary>
    /// 존재하는 로컬 데이터를 삭제하는 함수
    /// </summary>
    public static void DeleteUserData()
    {
        string filePath = Application.persistentDataPath + userDataName;

        if (!File.Exists(filePath))
        {
            Debug.LogWarning(filePath + " 에 삭제할 유저 데이터 파일이 없습니다.");
        }
        else
        {
            File.Delete(filePath);
            Debug.Log(filePath + " 에 존재하는 로컬데이터를 삭제하였습니다.");
            RefreshEditorProjectWindow();
        }
    }

    static void RefreshEditorProjectWindow()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}