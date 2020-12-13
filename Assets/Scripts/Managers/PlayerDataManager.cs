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
}