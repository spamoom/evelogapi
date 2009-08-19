using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace EVELogApi
{
    public class API
    {
		private string logPath;
        private string chatPath;
        private string gamePath;

        private bool validPath = false;

        /// <summary>
        /// Initialiser for the API class
        /// </summary>
        /// <param name="thePath">The path which points to the logs directory.</param>
        public API(string thePath)
        {
            if (thePath.Length > 2)
            {

                // Check if the log folder exists
                if (Directory.Exists(thePath))
                {
                    // Set the global vars
                    logPath = thePath;
                    chatPath = thePath + "Chatlogs\\";
                    gamePath = thePath + "Gamelogs\\";

                    validPath = true;
                }
                else
                {
                    // Directory doesn't exist
                    validPath = false;
                }
            }
            else
            {
                // Length is too small
                validPath = false;
            }

            // If the path isn't valid
            if (!validPath)
            {
                throw new Exception("Not a valid path");
            }
        }

        /// <summary>
        /// Lists all private chats in the log direcory - must be called after api
        /// </summary>
        /// <returns>Array of private chat usernames</returns>
        public string[] getPrivateChatList()
        {
            // Check if we have valid logs
            if (validPath)
            {
                // Set counters
                int namesCount = 0;
                int indexCount = 0;

                // Point to the directory..
                DirectoryInfo di = new DirectoryInfo(chatPath);

                // .. and get a list of the files
                FileInfo[] rgFiles = di.GetFiles("Private_Chat_(*)_*.txt");

                // Count roughly how many files we need for the array
                foreach (FileInfo fi in rgFiles)
                {
                    namesCount++;
                }

                // Create the array based on this count
                string[] arrayOfNames = new string[namesCount];

                // Loop through each file
                foreach (FileInfo fi in rgFiles)
                {
                    // Trim it down
                    string temp = fi.Name.Remove(0, 14);
                    temp = temp.Remove(temp.Length - 21);

                    // Check if it is already there
                    int found = Array.BinarySearch(arrayOfNames, temp);

                    // If it isn't, add it to the array
                    if (found <= 0)
                    {
                        arrayOfNames[indexCount] = temp;
                        indexCount++;
                    }
                }

                // Now make a smaller array to the size of the new one
                string[] items = new string[indexCount];

                // Move the items to the smaller array
                for (int x = 0; x < indexCount; x++)
                {
                    items[x] = arrayOfNames[x];
                }

                // Sort in alphabetical order (soon to be optional?)
                Array.Sort(items);

                // Finally return the items
                return items;
            }
            else
            {
                // Logpath was not valid, return an empty array
                string[] blank = new string[0];
                return blank;
            }
        }

        /// <summary>
        /// Lists all group chats in the log direcory - must be called after api
        /// </summary>
        /// <returns>Array of group chat usernames</returns>
        public string[] getGroupChatList()
        {
            // Check if we have valid logs
            if (validPath)
            {
                // Set counters
                int namesCount = 0;
                int indexCount = 0;

                DirectoryInfo di = new DirectoryInfo(chatPath);
                FileInfo[] rgFiles = di.GetFiles("Group_Chat_(*)_*.txt");

                // Count roughly how many files we need for the array
                foreach (FileInfo fi in rgFiles)
                {
                    namesCount++;
                }

                // Create the array based on this count
                string[] arrayOfNames = new string[namesCount];

                // Loop through each file
                foreach (FileInfo fi in rgFiles)
                {
                    // Check if it is already there
                    int found = Array.BinarySearch(arrayOfNames, fi.Name);

                    // If it isn't, add it to the array
                    if (found <= 0)
                    {
                        arrayOfNames[indexCount] = fi.Name;
                        indexCount++;
                    }
                }

                // Now make a smaller array to the size of the new one
                string[] items = new string[indexCount];

                // Move the items to the smaller array
                for (int x = 0; x < indexCount; x++)
                {
                    arrayOfNames[x] = arrayOfNames[x].Remove(0, 14);
                    arrayOfNames[x] = arrayOfNames[x].Remove(arrayOfNames[x].Length - 21);
                    items[x] = arrayOfNames[x];
                }

                // Sort in alphabetical order (soon to be optional?)
                Array.Sort(items);

                // Finally return the items
                return items;
            }
            else
            {
                // Logpath was not valid, return an empty array
                string[] blank = new string[0];
                return blank;
            }
        }

        /// <summary>
        /// Lists all other chats in the log direcory - must be called after api
        /// </summary>
        /// <returns>Array of other chat usernames</returns>
        public string[] getOtherChannelsList()
        {
            // Check if we have valid logs
            if (validPath)
            {

                DirectoryInfo di = new DirectoryInfo(chatPath);
                FileInfo[] rgFiles = di.GetFiles("*.txt");

                // Make an array hoping that they haven't joined more than 50 channels
                // in the past day
                string[] channels = new string[50];


                // Set 
                int indexCount = 0;

                foreach (FileInfo fi in rgFiles)
                {
                    // Take off the date, time and file extention
                    string cutDown = fi.Name.Remove(fi.Name.Length - 20);

                    // Check to see if it is already there
                    int found = Array.LastIndexOf(channels, cutDown);

                    // If it isn't..
                    if (found < 0)
                    {
                        // Check length and cut off bits for private chat checking
                        string cutDownPriv;
                        if (cutDown.Length > 14)
                        {
                            cutDownPriv = cutDown.Remove(14);
                        }
                        else
                        {
                            cutDownPriv = "";
                        }

                        // Array of things to check :)
                        string[] checkForOthers = new string[] { "Local", "Alliance", "Corp", "Constellation", "EVEHQ_PUBLIC", "Fleet" };

                        // Var to check problems
                        bool problem = false;

                        // Loop through to check for matches
                        foreach (string name in checkForOthers)
                        {
                            if (cutDown == name)
                            {
                                problem = true;
                            }
                        }

                        // If it is a Private chat
                        if (cutDownPriv == "Private_Chat_(")
                        {
                            problem = true;
                        }

                        // If it is a Group chat
                        if (fi.Name.Contains("Group_Chat_("))
                        {
                            problem = true;
                        }

                        // If there were no problems
                        if (!problem)
                        {
                            // Add to the array
                            channels[indexCount] = cutDown;
                            // Add to the count, used for the new array size
                            indexCount++;
                        }
                    }
                }


                // Create a new array based on the size of the items
                string[] channelitems = new string[indexCount];
                
                // Loop through moving the items to a smaller array
                for (int x = 0; x < indexCount; x++)
                {
                    channelitems[x] = channels[x];
                }

                // Sort in alphabetical order :)
                Array.Sort(channelitems);

                // Return the list
                return channelitems;
            }
            else
            {
                // Logpath was not valid, return an empty array
                string[] blank = new string[0];
                return blank;
            }
        }

        /// <summary>
        /// Get the latest chat from the logs
        /// </summary>
        /// <param name="logType">The channel/private chat username</param>
        /// <param name="lastLine">The last name to prevent high amount of file reading</param>
        /// <param name="showTimeStamp">True returns the chats with the timestamp attached</param>
        /// <returns>A multidimentional array of the chats from the lastLine onwards</returns>
        public string[,] getChat(string logType, string lastLine, bool showTimeStamp)
        {
            // New array for the text
            string[,] text;
            
            // Get the directory info
            DirectoryInfo di = new DirectoryInfo(chatPath);

            // TODO: Correct variable so it is safer and more accurate
            // Temporary variable as a change in the eve logs broke it
            string a = "*" + logType + "*_*.txt";
            FileInfo[] rgFiles = di.GetFiles(a);

            // Temp variables
            long largestTick = 0;
            string fileName = "";
            int fileCount = 0;

            // Fetch the latest file
            foreach (FileInfo fi in rgFiles)
            {
                // Check if it is the latest file
                if (fi.LastWriteTime.Ticks > largestTick)
                {
                    largestTick = fi.LastWriteTime.Ticks;
                    fileName = fi.Name.ToString();
                }
                fileCount++;
            }

            // If at least one file was returned
            if (fileCount != 0)
            {

                // Open a new filestream to count how many lines the document has
                FileStream countfs = new FileStream(chatPath + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader countFile = new StreamReader(countfs);

                // Temp variables
                string line;
                int counter = 0;

                //Get Line Count
                while ((line = countFile.ReadLine()) != null)
                {
                    counter++;
                }

                // Close the stream
                countFile.Close();

                // Used for problem checking
                bool problem = false;

                // See if the file is empty / has no lines after the 'header'
                if (counter <= 11)
                {
                    problem = true;
                }

                // TODO: Add regex checking for the line
                // Using regex to check it is a chat line....
                // Example line:
                //  [ 2009.02.09 16:07:05 ] SpamooM > Testing testing!
                // Regex lineFormat = new Regex("[^a-zA-Z]");

                // If all went okay
                if (!problem)
                {
                    // Add a new array with the amount of lines
                    text = new string[counter - 11, 2];

                    // Reset the counter
                    counter = 0;

                    // New filestream to get the lines of text
                    FileStream fs = new FileStream(chatPath + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader file = new StreamReader(fs);

                    // While there are lines to read...
                    while ((line = file.ReadLine()) != null)
                    {
                        // Make sure we don't read the header
                        if (counter >= 11)
                        {
                            // Check if they want the timestamp :)
                            if (!showTimeStamp)
                            {
                                text[counter - 11, 1] = line.Remove(0, 25);
                            }
                            else
                            {
                                text[counter - 11, 1] = line;
                            }

                            // Chop off unwanted parts of the date time & add to the array
                            text[counter - 11, 0] = line.Remove(24, line.Length - 24);
                            text[counter - 11, 0] = text[counter - 11, 0].Remove(0, 3);
                            text[counter - 11, 0] = text[counter - 11, 0].Remove(text[counter - 11, 0].Length - 2, 2);
                        }

                        // Increment the count
                        counter++;
                    }

                    // Temp Variable
                    int cutOff = 0;

                    // Okay now to see if a line was specified!
                    if (lastLine != "")
                    {
                        //Loop through newly created array to look for it!
                        for (int i = 0; i < (text.Length / 2); i++)
                        {
                            if (text[i, 1].Contains(lastLine))
                            {
                                cutOff = i;
                                continue;
                            }
                        }

                        // Add 1 so that we don't send back the specified string
                        cutOff++;

                        // Now to re-create the array without the stuff they already have
                        string[,] temptext = new string[text.Length / 2 - cutOff, 2];

                        for (int i = 0; i < (temptext.Length / 2); i++)
                        {
                            temptext[i, 1] = text[i + cutOff, 1];
                            temptext[i, 0] = text[i + cutOff, 0];
                        }

                        text = new string[temptext.Length / 2, 2];

                        for (int i = 0; i < (temptext.Length / 2); i++)
                        {
                            text[i, 1] = temptext[i, 1];
                            text[i, 0] = temptext[i, 0];
                        }

                        if ((temptext.Length / 2) == 0)
                        {
                            text = new string[1, 2];
                            text[0, 0] = "error";
                        }
                    }
                    // Close the stream
                    file.Close();
                    
                    return text;
                }
            }
            // If there was a problem and the return didn't exit before this line
            // return an empty string with the error tag
            string[,] blank = new string[1, 2];
            blank[0, 1] = "";
            blank[0, 0] = "error";
            return blank;
          
        }
    }
}