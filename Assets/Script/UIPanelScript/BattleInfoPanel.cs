using GJFramework;
using System;
using UnityEngine.UI;

public class BattleInfoPanel : UIPanelBase
{
    public Image imgEnemyIcon;
    public Text txtEnemyDesc;
    public GridLayoutGroup instrumentLayoutGroup;
    public Text txtRecommondTitle;
    public HorizontalLayoutGroup recommondLayoutGroup;
    public Button btnStart;

    private BattleLevelRefObj _battleLevelRefObj;
    protected override void OnShow()
    {


        RefreshShow();
    }

    protected override void OnHide(Action onHideFinished)
    {
        
    }

    private void RefreshShow()
    {

    }


}
