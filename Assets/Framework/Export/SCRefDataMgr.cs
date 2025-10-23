namespace GJFramework
{
    /// <summary>
    /// 配表数据管理器 在这里写所有的配表数据
    /// </summary>
    public class SCRefDataMgr : Singleton<SCRefDataMgr>
    {

        public SCRefDataList<InstrumentRefObj> instrumentRefList = new SCRefDataList<InstrumentRefObj>(InstrumentRefObj.assetPath, InstrumentRefObj.sheetName);
        public SCRefDataList<BattleLevelRefObj> battleLevelRefList = new SCRefDataList<BattleLevelRefObj>(BattleLevelRefObj.assetPath, BattleLevelRefObj.sheetName);
        public SCRefDataList<InstrumentStoreRefObj> instrumentStoreRefList = new SCRefDataList<InstrumentStoreRefObj>(InstrumentStoreRefObj.assetPath, InstrumentStoreRefObj.sheetName);
        public SCRefDataList<SkillRefObj> skillRefList = new SCRefDataList<SkillRefObj>(SkillRefObj.assetPath, SkillRefObj.sheetName);

        public void Init()
        {
            instrumentRefList.readFromTxt();
            battleLevelRefList.readFromTxt();
            instrumentStoreRefList.readFromTxt();
            skillRefList.readFromTxt();
        }
    }
}
