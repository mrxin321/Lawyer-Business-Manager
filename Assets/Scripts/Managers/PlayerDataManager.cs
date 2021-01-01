using System.Collections;
using System.Collections.Generic;

public  class PlayerDataManager
{
    private static PlayerDataManager _instance;

    public static PlayerDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerDataManager(); 
            }

            return _instance;
        }
    }

    private int UserId;
    private string UserName;
    private int Permission;

    public void SetUserData(int userId,string userName,int permission)
    {
        UserId = userId;
        UserName = userName;
        Permission = permission;
    }

    public int GetUserId()
    {
        return UserId;
    }

    public string GetUserName()
    {
        return UserName;
    }

    // public List<CaseData> MyCaseData = new List<CaseData>();
    // public List<StageData> MyStageData = new List<StageData>();
    // public List<TaskData> MyTaskData = new List<TaskData>();

    // public void SetMyCaseData(List<TaskData> myCaseData)
    // {
    //     MyCaseData.Clear();
    //     MyStageData.Clear();
    //     MyTaskData.Clear();

    //     var dic = new Dictinary<int,bool>();
    //     foreach(var data in myCaseData)
    //     {
    //         if(!dic.ContainsKey(data.caseid))
    //         {
    //             var caseData = new CaseData();
    //             caseData.id = data.caseid;
    //             caseData.id = data.caseid;
    //             caseData.id = data.caseid;
                
    //         }
    //     }
    // }
}