using System.Text;

namespace codecrafters_redis.src;
public class RdbFileHandler
{
    public static void EnsureFileExist(string directoryPath , string fileName)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Console.WriteLine("Directory Created Succefully");
            }
            else
            {
                Console.WriteLine("Directory Already Exist");
            }

            string filePath = Path.Combine(directoryPath , fileName);

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                Console.WriteLine("File Create Succefully");
            }
            else
            {
                Console.WriteLine("File Aready Exist");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error ensuring directory and file exist: {ex.Message}");
        }
    }
    public static string[] LoadKeys(string directoryPath , string fileName)
    {
        var result = new List<string>();
        try
        {
            var filePath = Path.Combine(directoryPath , fileName);
            var fileContent = File.ReadAllBytes(filePath);

            for (int i = 0 ; i < fileContent.Length ; i++)
            {
                string currentByte = $"{fileContent[i]:X2}";

                // Indicates that hash table size information follows.
                if (currentByte == "FB")
                {
                    var keySize = Convert.ToInt32(fileContent[i + 4]);
                    var keyStart = i + 5;
                    var keyEnd = keyStart + keySize;

                    var keyBytes = fileContent[keyStart..keyEnd];
                    var keyValue = Encoding.UTF8.GetString(keyBytes);

                    result.Add(keyValue);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result.ToArray();
    }
}
