namespace GJFramework
{
    /// <summary>
    /// 配表数据管理器 在这里写所有的配表数据
    /// </summary>
    public class SCRefDataMgr : Singleton<SCRefDataMgr>
    {

        public InstrumentRefObj instrumentRefObj = new InstrumentRefObj(InstrumentRefObj.assetPath, InstrumentRefObj.sheetName);
        public void Init()
        {
            instrumentRefObj.readFromTxt();
        }
    }
}
