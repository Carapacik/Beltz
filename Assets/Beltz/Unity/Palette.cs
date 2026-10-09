using UnityEngine;

namespace Beltz.Unity
{
    public static class Palette
    {
        // Six sampled colors per original 2x3 image. BG6's 2x2 midpoint is interpolated.
        static readonly string[] Presets = {
            "A18CD1 A18CD1 7F4CF1 C7BAE4 FBC2EB DF86C6", "FBB8A5 FAD0C4 00A8F3 C7BAE4 FFA1E5 FFD1FF",
            "5A55A9 827EC6 DA61B7 9461DA EE98D5 EE98D5", "7F7FD5 6969A9 86A8E7 86A8E7 86A8E7 91EAE4",
            "B82CB4 8A2387 E94057 E94057 B46A3C F27121", "F054FF FF2DB2 D374FF FF46C0 B593FF FF60CF",
            "189EE8 BF52A7 5277D8 ACA9E0 E190B7 A533A1", "70D4F2 37E74F F5FC79 70F282 D4FC79 96E6A1",
            "77BAE5 8BC6EC 7E83E4 6E75F0 8084C4 9599E2", "A5ABFF A6ACFF 85C9FF C43CF3 CB8CE4 D299E6",
            "2AF598 2AF56D 3FBBAD 71BEC2 44AFEF 4470EB", "25A049 4D69D4 35C6CE 79D5F0 33B486 12CC44",
            "F12711 F5AF19 F5AF19 F5AF19 F12711 F12711", "C471ED C471ED C471ED 12C2E9 F64F59 F64F59",
            "373B44 373B44 373B44 4286F4 4286F4 4286F4", "DAE44D D3E000 00B73D DAE44D 008500 00B73D"
        };
        static readonly string[] Names = {"_TopLeft","_TopRight","_MidLeft","_MidRight","_BottomLeft","_BottomRight"};
        public static void Apply(Material material,int index)
        {
            var colors=Presets[(index%Presets.Length+Presets.Length)%Presets.Length].Split(' ');
            for(int i=0;i<6;i++){ColorUtility.TryParseHtmlString("#"+colors[i],out var c);material.SetColor(Names[i],c);Shader.SetGlobalColor("_Beltz"+Names[i].Substring(1),c.linear);}
        }
    }
}
