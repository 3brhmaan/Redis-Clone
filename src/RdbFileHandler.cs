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
    public static Dictionary<string , string> LoadKeysAndValues(string directoryPath , string fileName)
    {
        var result = new Dictionary<string , string>();

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
                    var tableSize = Convert.ToInt32(fileContent[i + 1]);
                    i += 3;

                    while (tableSize-- > 0)
                    {
                        bool expired = false;

                        currentByte = $"{fileContent[i]:X2}";
                        if (currentByte == "FC")
                        {
                            int expireStart = i + 1;
                            int expireEnd = i + 9;
                            var expireBytes = fileContent[expireStart..expireEnd];

                            ulong expiryMs = BitConverter.ToUInt64(expireBytes , 0);
                            ulong currentTimeMs = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                            if (expiryMs < currentTimeMs)
                                expired = true;

                            i += 9;
                        }

                        if (currentByte == "FD")
                        {
                            int expireStart = i + 1;
                            int expireEnd = i + 5;
                            var expireBytes = fileContent[expireStart..expireEnd];

                            ulong expirySec = BitConverter.ToUInt64(expireBytes , 0);
                            ulong currentTimeSec = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                            if (expirySec < currentTimeSec)
                                expired = true;

                            i += 5;
                        }

                        var keySize = Convert.ToInt32(fileContent[i + 1]);
                        var keyStart = i + 2;
                        var keyEnd = keyStart + keySize;

                        var keyBytes = fileContent[keyStart..keyEnd];
                        var key = Encoding.UTF8.GetString(keyBytes);

                        var valueSize = Convert.ToInt32(fileContent[keyEnd]);
                        var valueStart = keyEnd + 1;
                        var valueEnd = valueStart + valueSize;

                        var valueBytes = fileContent[valueStart..valueEnd];
                        var value = Encoding.ASCII.GetString(valueBytes);

                        result[key] = value;

                        if (expired)
                        {
                            result[key] = "-1";
                        }

                        i += keySize + valueSize + 3;
                    }

                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }
}