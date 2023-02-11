using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCCH
{
    public class SepTextProfilier : MonoBehaviour
    {

        /// <summary>
        /// LEGACY METHOD FOR GETTING IP ADDRESS, DO NOT USE
        /// </summary>
        /// <param name="input">input string that need to check</param>
        /// <param name="startingWord">the keyword that telling this method to start checking content</param>
        /// <param name="endingWord">the keyword that telling this method to stop checking content</param>
        /// <returns></returns>
        public string TextProfilier(string input, string startingWord, string endingWord)
        {
            //Debug.Log(input);
            string targetWord;
            char[] startingWordArray = startingWord.ToCharArray();
            char[] endingWordArray = endingWord.ToCharArray();
            char[] inputArray = input.ToCharArray();
            List<char> targetWordList = new List<char>();
            char[] targetWordArray;

            int checkTimeStartingWord = 0;
            int checkTimeEndingWord = 0;

            bool checkedStartingWord = false;
            bool checkedEndingWord = false;


            //Debug.Log(inputArray[0].ToString() + inputArray[1].ToString() + inputArray[2].ToString());

            for (int i = 0; i < inputArray.Length; i++)
            {
                //Detect wether it is starting keyword or not
                if (!checkedStartingWord && inputArray[i] == startingWordArray[checkTimeStartingWord])
                {
                    checkTimeStartingWord++;
                    if (checkTimeStartingWord == startingWordArray.Length)
                    {
                        Debug.Log("starting Word Checked");
                        checkedStartingWord = true;
                    }
                }
                else
                {
                    checkTimeStartingWord = 0;
                }

                //Detect wether it is ending keyword or not
                if (checkedStartingWord && !checkedEndingWord && checkTimeStartingWord == 0)
                {
                    if (inputArray[i] == endingWordArray[checkTimeEndingWord])
                    {
                        checkTimeEndingWord++;
                        if (checkTimeEndingWord == endingWordArray.Length)
                        {
                            checkedEndingWord = true;
                        }
                    }
                    else
                    {
                        checkTimeEndingWord = 0;
                        targetWordList.Add(inputArray[i]);
                    }
                }

                //Detect if we found everything and exit the traverse
                if (checkedStartingWord && checkedEndingWord)
                {
                    Debug.Log("Traverse complete");
                    i = inputArray.Length;
                }
            }

            //converting target word that we get into string and output
            targetWordArray = new char[targetWordList.Count];
            for (int j = 0; j < targetWordArray.Length; j++)
            {
                Debug.Log("CONVERTING char to string" + j);
                targetWordArray[j] = targetWordList[j];
            }

            //targetWord = targetWordArray.ToString();
            targetWord = new string(targetWordArray);

            Debug.Log(targetWord);
            return targetWord;
        }

        /// <summary>
        /// LEGACY METHOD FOR GETTING FILE INDEX, DO NOT USE
        /// </summary>
        /// <param name="input">input string that need to check</param>
        /// <param name="startingWord">the keyword that telling this method to start checking content</param>
        /// <param name="endingWord">the keyword that telling this method to stop checking content</param>
        /// <param name="seperatingWord">the keyword that telling this method to stop and store a name to the list and start checking following content</param>
        /// <returns></returns>
        public string[] TextProfilier(string input, string startingWord, string endingWord, char seperatingWord)
        {
            Debug.Log(input);
            string[] targetWords;
            //to hold entire list of words that allows to turn into array
            List<string> targetWordsList = new List<string>();

            char[] startingWordArray = startingWord.ToCharArray();
            char[] endingWordArray = endingWord.ToCharArray();
            char[] inputArray = input.ToCharArray();

            //to hold one word in entire list of words
            List<char> targetWordList = new List<char>();

            int checkTimeStartingWord = 0;
            int checkTimeEndingWord = 0;

            bool checkedStartingWord = false;
            bool checkedEndingWord = false;
            bool checkedSeperateWord = false;

            //Debug.Log(inputArray[0].ToString() + inputArray[1].ToString() + inputArray[2].ToString());

            for (int i = 0; i < inputArray.Length; i++)
            {
                //Detect wether it is starting keyword or not
                if (!checkedStartingWord && inputArray[i] == startingWordArray[checkTimeStartingWord])
                {
                    checkTimeStartingWord++;
                    if (checkTimeStartingWord == startingWordArray.Length)
                    {
                        Debug.Log("starting Word Checked");
                        checkedStartingWord = true;
                    }
                }
                else
                {
                    checkTimeStartingWord = 0;
                }


                if (checkedStartingWord && !checkedEndingWord && checkTimeStartingWord == 0)
                {
                    //Check if end of target word
                    if (inputArray[i] == endingWordArray[checkTimeEndingWord])
                    {
                        checkTimeEndingWord++;
                        if (checkTimeEndingWord == endingWordArray.Length)
                        {
                            checkedEndingWord = true;
                        }
                    }
                    else
                    {
                        checkTimeEndingWord = 0;
                        if (inputArray[i] == seperatingWord)
                        {
                            checkedSeperateWord = true;
                        }

                        //detect weather it is seperate word or not
                        if (checkedSeperateWord)
                        {
                            //stopping point
                            char[] targetWordCharHolder = new char[targetWordList.Count];
                            for (int j = 0; j < targetWordCharHolder.Length; j++)
                            {
                                targetWordCharHolder[j] = targetWordList[j];
                            }
                            //targetWordsList.Add(targetWordCharHolder.ToString());
                            targetWordsList.Add(new string(targetWordCharHolder));
                            targetWordList.Clear();
                            checkedSeperateWord = false;
                        }
                        else
                        {
                            targetWordList.Add(inputArray[i]);
                        }
                    }
                }

                //Detect if we found everything and exit the traverse
                if (checkedStartingWord && checkedEndingWord)
                {
                    Debug.Log("Traverse complete");
                    i = inputArray.Length;
                }
            }

            targetWords = new string[targetWordsList.Count];

            //put string list into string array in order to output
            for (int i = 0; i < targetWordsList.Count; i++)
            {
                targetWords[i] = targetWordsList[i];
            }
            Debug.Log(targetWords);
            return targetWords;
        }

        /// <summary>
        /// This method is for checking input file name's format, when using this code, DO NOT type ".XXX" cause it is already hard coded,
        /// instead, type "xxx" (e.g. fbx,txt,xml)
        /// </summary>
        /// <param name="input">input file name that need to be checked</param>
        /// <param name="keyword_format">input file format</param>
        /// <returns></returns>
        public bool TextProfilier(string input, string keyword_format)
        {
            bool keywordMatch = false;
            bool dotMatch = false;

            char keyword_Dot = ".".ToCharArray()[0];
            char[] inputArray = input.ToCharArray();
            char[] keywordArray_format = keyword_format.ToCharArray();
            List<char> aquiredList_format = new List<char>();
            char[] aquiredArray_format;

            //converting every char we got from input into this string, then compare with keyword_format that we want inspect
            string aquiredCompare_format;

            int checkInt = 0;

            for (int i = 0; i < inputArray.Length; i++)
            {
                //Detect wether it is starting keyword or not
                if (!dotMatch && inputArray[i] == keyword_Dot)
                {
                    Debug.Log("Dot match: " + input);
                    dotMatch = true;
                }

                //Detect wether it is ending keyword or not
                if (dotMatch)
                {
                    if (checkInt == 0)
                    {
                        checkInt++;
                    }
                    else
                    {
                        aquiredList_format.Add(inputArray[i]);
                    }

                }
            }

            //if there is no dot detected, return false and alarm
            if (!dotMatch)
            {
                Log.Text(SimulationManager.instance.label, "No format info found on input string", "No format info found on input string", Log.Level.Error);
                return false;
            }

            //converting target word that we get into string and output
            aquiredArray_format = new char[aquiredList_format.Count];
            for (int j = 0; j < aquiredArray_format.Length; j++)
            {
                Debug.Log("CONVERTING char to string" + j);
                aquiredArray_format[j] = aquiredList_format[j];
            }

            aquiredCompare_format = new string(aquiredArray_format);

            if (aquiredCompare_format == keyword_format)
            {
                Debug.Log("Format matched: " + input);
                keywordMatch = true;
            }
            else
            {
                aquiredCompare_format = new string(aquiredArray_format);
                string checkKeywordArray = new string(keywordArray_format);
                Debug.Log("Format dismatched: " + input + "\nFormat: " + aquiredCompare_format + "\nChecking keyword array: " + checkKeywordArray);
                keywordMatch = false;
            }

            return keywordMatch;
        }


        /// <summary>
        /// This metod is for returning file extension name
        /// </summary>
        /// <param name="input">input file name that need to be checked</param>
        /// <returns></returns>
        public string GetFormat(string input)
        {
            bool dotMatch = false;

            char keyword_Dot = ".".ToCharArray()[0];
            char[] inputArray = input.ToCharArray();

            List<char> aquiredList_format = new List<char>();
            char[] aquiredArray_format;


            string aquiredFormat = null;

            int checkInt = 0;

            for (int i = 0; i < inputArray.Length; i++)
            {
                //Detect wether it is starting keyword or not
                if (!dotMatch && inputArray[i] == keyword_Dot)
                {
                    //Debug.Log("Dot match: " + input);
                    dotMatch = true;
                }

                //Detect wether it is ending keyword or not
                if (dotMatch)
                {
                    if (checkInt == 0)
                    {
                        checkInt++;
                    }
                    else
                    {
                        aquiredList_format.Add(inputArray[i]);
                    }

                }
            }

            //if there is no dot detected, return false and alarm
            if (!dotMatch)
            {
                Log.Text(SimulationManager.instance.label, "No format info found on input string", "No format info found on input string", Log.Level.Error);
                return null;
            }

            //converting target word that we get into string and output
            aquiredArray_format = new char[aquiredList_format.Count];
            for (int j = 0; j < aquiredArray_format.Length; j++)
            {
                //Debug.Log("CONVERTING char to string" + j);
                aquiredArray_format[j] = aquiredList_format[j];
            }

            aquiredFormat = new string(aquiredArray_format);

            return aquiredFormat;
        }

        public string GetNameWithoutFormat(string input)
        {
            bool dotMatch = false;

            string output = null;

            char keyword_Dot = ".".ToCharArray()[0];
            char[] inputArray = input.ToCharArray();

            List<char> aquiredList_Output = new List<char>();
            char[] aquiredArray_Output;


            for (int i = 0; i < inputArray.Length; i++)
            {
                //Detect wether it is starting keyword or not
                if (inputArray[i] == keyword_Dot)
                {
                    //Debug.Log("Dot match: " + input);
                    dotMatch = true;
                    i = inputArray.Length;
                }
                else
                {
                    aquiredList_Output.Add(inputArray[i]);
                }
            }

            if (dotMatch)
            {
                aquiredArray_Output = new char[aquiredList_Output.Count];
                for (int i = 0; i < aquiredArray_Output.Length; i++)
                {
                    //Debug.Log("CONVERTING char to string" + i);
                    aquiredArray_Output[i] = aquiredList_Output[i];
                }
                output = new string(aquiredArray_Output);
                return output;
            }
            else
            {
                Debug.Log("ERROR: dot not found");
                return null;
            }


        }
    }
}

