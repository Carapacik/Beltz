using System;

namespace Beltz.Core
{
    public sealed class CampaignProgress
    {
        public const int ChapterSize=30, ChapterCount=4, LevelCount=ChapterSize*ChapterCount, UnlockThreshold=10;
        const uint ValidBits=(1u<<ChapterSize)-1;
        readonly uint[] completed=new uint[ChapterCount];
        public int IntroSeen {get;private set;}
        public int CompletedCount {get {int total=0;for(int c=0;c<ChapterCount;c++)total+=ChapterCompleted(c);return total;}}
        public CampaignProgress(uint[] masks=null,int introSeen=0)
        {
            if(masks!=null)for(int c=0;c<Math.Min(masks.Length,ChapterCount);c++)completed[c]=masks[c]&ValidBits;
            IntroSeen=introSeen&15;
        }
        public int ChapterCompleted(int chapter)
        {
            if(chapter<0||chapter>=ChapterCount)return 0;
            uint bits=completed[chapter];int count=0;while(bits!=0){bits&=bits-1;count++;}return count;
        }
        public bool IsChapterUnlocked(int chapter)
        {
            if(chapter<0||chapter>=ChapterCount)return false;
            for(int c=0;c<chapter;c++)if(ChapterCompleted(c)<UnlockThreshold)return false;
            return true;
        }
        public int FirstIncomplete(int chapter)
        {
            int offset=0;while(offset<ChapterSize&&IsCompleted(chapter*ChapterSize+offset))offset++;return offset;
        }
        public int ContinueIndex
        {
            get {for(int c=ChapterCount-1;c>=0;c--)if(IsChapterUnlocked(c)&&FirstIncomplete(c)<ChapterSize)return c*ChapterSize+FirstIncomplete(c);return LevelCount-1;}
        }
        public bool IsCompleted(int index)=>index>=0&&index<LevelCount&&(completed[index/ChapterSize]&(1u<<(index%ChapterSize)))!=0;
        public bool IsUnlocked(int index)=>index>=0&&index<LevelCount&&IsChapterUnlocked(index/ChapterSize)&&(IsCompleted(index)||index%ChapterSize<=FirstIncomplete(index/ChapterSize));
        public bool Complete(int index){if(!IsUnlocked(index))return false;completed[index/ChapterSize]|=1u<<(index%ChapterSize);return true;}
        public bool HasSeenIntro(int chapter)=>chapter>=0&&chapter<ChapterCount&&(IntroSeen&(1<<chapter))!=0;
        public void SeeIntro(int chapter){if(chapter<0||chapter>=ChapterCount)throw new ArgumentOutOfRangeException(nameof(chapter));IntroSeen|=1<<chapter;}
        public string Export()=>string.Join(",",completed);
        public static CampaignProgress Import(string value,int introSeen=0)
        {
            var masks=new uint[ChapterCount];var parts=(value??"").Split(',');
            if(parts.Length==ChapterCount)for(int c=0;c<ChapterCount;c++)uint.TryParse(parts[c],out masks[c]);
            return new CampaignProgress(masks,introSeen);
        }
        public static CampaignProgress FromLegacy(uint bits,int introSeen=0)
        {
            var masks=new uint[ChapterCount];for(int c=0;c<ChapterCount;c++)masks[c]=(bits>>(c*8))&255;
            return new CampaignProgress(masks,introSeen);
        }
    }
}
