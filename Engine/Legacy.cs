using System;
using System.Text.Json;

namespace Adofai.Engine; 

/// <summary>
/// This class is for dealing with older versions of .ADOFAI Files
/// </summary>
public static class Legacy {
    public static float[] GetAngleData(JsonElement element) {
        string rawData = element.GetString();
        
        Console.WriteLine(rawData);
        
        // Adofai file values divided by 180, and a 0 added at the start of the array
        float[] angleData = new float[rawData.Length + 1];
        angleData[0] = 0;

        for (int i = 0; i < rawData.Length; i++) {
            angleData[i + 1] = GetAngleFromPathChar(rawData[i]);
            Console.WriteLine(angleData[i + 1]);
        }

        return angleData;
    }

    private static float GetAngleFromPathChar(char path) {
        Console.Write(path + ": ");
        
        switch (path) {
            case 'R': return 0f;
            case 'U': return 0.5f;
            case 'L': return 1f;
            case 'D': return 1.5f;
            case 'E': return 0.25f;
            case 'H': return 0.833333333f;
            case 'M': return 1.833333333f;
            case 'T': return 0.666666667f;
            
            case 'F': return 0f; // Finish (fix)
            case '!': return 5.55f; // Midspin
        }

        return -1;
    }
}