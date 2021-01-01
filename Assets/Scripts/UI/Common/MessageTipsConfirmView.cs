using UnityEngine;
using UnityEngine.UI;
using System;

public class MessageTipsConfirmView : BaseView
{
	[SerializeField] public Text TipsView;

	private Action OkAction;

	public override void Refresh()
	{
		var _params = GetParams();
		var tips = (string)_params[0];

		if(_params.Length >= 2)OkAction = (Action)_params[1];
		TipsView.text = tips;
	}

	public void OnConfirmClick()
	{
		Close();
		OkAction();
	}

	public void OnCancelClick()
	{
		Close();
	}
}