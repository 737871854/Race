using UnityEngine;
using System.Collections;
using Need.Mx;
/// <summary>
/// 复制资源到Document文件夹下 等待动画;
/// </summary>
public class GameStartLoadingView{

    #region 控件;
    private GameObject view;
    #endregion

    #region 单例;
    private GameStartLoadingView()
    {
        //this.view = UITool.FindUIGameObject("GameStartLoading", UITool.CENTER_PANEL_COMMON);
        this.view.SetActive(false);
    }
    private static GameStartLoadingView _instance;
    public static GameStartLoadingView Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStartLoadingView();
            }
            return _instance;
        }
    }
    #endregion

    #region 公开方法;
    public void Start()
    {
        this.view.SetActive(true);
    }
    public void Close()
    {
        GameObject.Destroy(this.view);
    }
    #endregion
}
