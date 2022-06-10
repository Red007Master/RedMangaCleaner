namespace RedsTools
{
    namespace Strings
    {
        public static class RStrings
        {
            static public string[] Split(string inputSplited, string inputSpliter)
            {
                string[] result = new string[2];

                int endIndexOfSpliter = 0, startIndexOfSpliter = 0;
                bool spliterEncountered = false;
                for (int i = 0; i < inputSplited.Length; i++)
                {
                    for (int j = 0; j < inputSpliter.Length; j++)
                    {
                        if (!(inputSplited[i + j] == inputSpliter[j]))
                        {
                            break;
                        }
                        else if (j == inputSpliter.Length - 1)
                        {
                            spliterEncountered = true;
                            endIndexOfSpliter = i + j;
                            startIndexOfSpliter = endIndexOfSpliter - inputSpliter.Length;
                        }
                    }
                }

                if (spliterEncountered)
                {
                    string str1 = "", str2 = "";

                    for (int i = 0; i < startIndexOfSpliter + 1; i++)
                    {
                        str1 = str1 + inputSplited[i];
                    }

                    for (int i = endIndexOfSpliter + 1; i < inputSplited.Length; i++)
                    {
                        str2 = str2 + inputSplited[i];
                    }

                    result[0] = str1;
                    result[1] = str2;
                }

                return result;
            }
        }
    }
}


