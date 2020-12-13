using UnityEngine;
using UnityEngine.UI;

public class MessageTipsView : BaseView
{
	[SerializeField] public Text TipsView;

	public override void Refresh()
	{
		var _params = GetParams();
		var tips = (string)_params[0];
		TipsView.text = tips;

		Utility.DoWait(()=>{
			Close();
		},2f,this);
	}
}