namespace FileWorkers.FileReaders;

public class TxtFileReader
{
    public List<string> ReadTxtFile(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        return lines.ToList();
    }
}