using Beltz.Core;

namespace Beltz.Content
{
    public static class Campaign
    {
        static readonly LevelDefinition[] Levels=Create();
        public static LevelDefinition Get(int index)=>Levels[index];
        static LevelDefinition[] Create()
        {
            var result=new LevelDefinition[CampaignProgress.LevelCount];var legacy=LegacyLevels.Create();
            for(int c=0;c<CampaignProgress.ChapterCount;c++)for(int n=0;n<CampaignProgress.ChapterSize;n++)
            {
                int index=c*CampaignProgress.ChapterSize+n;
                result[index]=c<2&&n<8?legacy[c*8+n]:LevelGenerator.Generate((GameMode)c,n<10?0:n<20?1:2,20261010+index*7919);
            }
            return result;
        }
    }
}
