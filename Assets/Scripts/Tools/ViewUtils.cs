using UnityEngine;

public static class ViewUtils
{
    public static void Print(string message,params object[] args)
    {
        string msg = string.Format(message, args);
        Debug.Log(msg);
    }

    public static void MessageTips(string tips)
    {
    	UIManager.Instance.OpenWindow("MessageTipsView",tips);
    }
}