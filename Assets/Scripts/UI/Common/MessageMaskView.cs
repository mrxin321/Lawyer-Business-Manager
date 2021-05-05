using UnityEngine;
using UnityEngine.UI;

public class MessageMaskView : BaseView
{
	[SerializeField] public Transform Root;

	private long ShowTime = 0;
	private long CloseTime = 0;
	public override void Refresh()
	{
		var _params = GetParams();
		var showNetMask = (bool)_params[0];

		if(showNetMask)
		{
			ShowTime = Utility.GetServerTime() + (long)(1000 * 1f);
			return;
		}
		Root.gameObject.SetActive(true);
	}	

	private void Update()
	{
		if(Utility.GetServerTime() > ShowTime)
		{
			if(CloseTime == 0)CloseTime = Utility.GetServerTime() + (long)(1000 * 10f);
			Root.gameObject.SetActive(true);
		}

		if(CloseTime > 0 && Utility.GetServerTime() > CloseTime)
		{
			Close();
		}
	}
}