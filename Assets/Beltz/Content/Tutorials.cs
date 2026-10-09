using Beltz.Core;

namespace Beltz.Content
{
    public static class Tutorials
    {
        public static LevelDefinition Get(int chapter)
        {
            var mode=(GameMode)chapter;
            return new LevelDefinition("tutorial-"+chapter,mode,new[] {
                new ShaftDef(-1.6f,.5f,true,chapter==2),
                new ShaftDef(0,-.5f,true,chapter>=2),
                new ShaftDef(1.6f,.5f,true,chapter>=2)
            },new[] {
                new BeltDef(0,1,true,true),
                new BeltDef(1,2,false,chapter==0)
            });
        }
    }
}
