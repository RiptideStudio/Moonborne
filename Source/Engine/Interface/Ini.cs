using System.Collections.Generic;
using System;
using System.IO;
public class IniFile
{
    private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
    private string PathName;

    public IniFile(string path)
    {
        PathName = path;
        Load(path);
    }

    public bool IsValid()
    {
        return File.Exists(PathName);
    }

    private void Load(string path)
    {
        string[] lines = File.ReadAllLines(path);
        string currentSection = "";

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
                continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                currentSection = line.Substring(1, line.Length - 2);
                data[currentSection] = new Dictionary<string, string>();
            }
            else
            {
                string[] keyValue = line.Split('=');
                if (keyValue.Length == 2)
                {
                    data[currentSection][keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }
        }
    }

    public string GetValue(string section, string key)
    {
        return data.ContainsKey(section) && data[section].ContainsKey(key) ? data[section][key] : null;
    }
}
