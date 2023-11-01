using CampusManager;
using System.Collections.Generic;
using System.IO;

internal static class ProgrammesHelpers
{

    public static List<Progra> GetProgras()
    {

        var file = @"C:\Users\eddin\OneDrive\Desktop\Informatique,PROG-001,4 ans.txt";
        var lines = File.ReadAllLines(file);
        var list = new List<Progra>();
        for (int i = 0; 1 < lines.Length; i++)
        {
            var line = lines[i].Split(',');
            var progr = new Progra()
            {
                NomDuProg = line[0],
                NumDuProg = line[1],
                DureeDuProg = line[3]
            };
            list.Add(progr);
        }
        return list;
    }
}