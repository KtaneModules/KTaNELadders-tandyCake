using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class LaddersScript : MonoBehaviour {
    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable submitButton;
    public GameObject[] ladderArms;
    public Material[] ladderColorOptions; // ROYGBCPA
    private Material[] ladderColorsFluid = new Material[8];
    public GameObject topShutter;
    public GameObject bottomShutter;
    bool TwitchPlaysActive;


    public MeshRenderer[] armColors = new MeshRenderer[6];
    public KMSelectable[] ladder1Selectables, ladder2Selectables, ladder3Selectables;
    public GameObject[] ladder1Rungs, ladder1Solid, ladder1Broke;
    public GameObject[] ladder2Rungs, ladder2Solid, ladder2Broke;
    public GameObject[] ladder3Rungs, ladder3Solid, ladder3Broke;
    private KMSelectable[][] allSelectables = new KMSelectable[3][];
    public GameObject[][] allRungs = new GameObject[3][] { new GameObject[8], new GameObject[9], new GameObject[7] };
    public GameObject[][] allSolid = new GameObject[3][] { new GameObject[8], new GameObject[9], new GameObject[7] };
    public GameObject[][] allBroke = new GameObject[3][] { new GameObject[8], new GameObject[9], new GameObject[7] };

    bool[][] allLadderStatuses = new bool[3][] { new bool[8], new bool[9], new bool[7] };
    bool[][] correctStatuses = new bool[3][] { new bool[8], new bool[9], new bool[7] };
    int[][] ladderColors = new int[3][] { new int[8], new int[9], new int[7] };


    bool[] laddersActive = new bool[3];
    bool isResetting;
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    int stage = 0;
    int pressedIndex;
    string[] colorNames = new string[] { "R", "O", "Y", "G", "B", "C", "P", "A" };

    int[][][] stage1Table = new int[3][][]
    {
        new int[4][] { new int[]{6,5,3}, new int[]{1,2,3}, new int[]{0,5,0}, new int[]{7,4,3} },
        new int[4][] { new int[]{5,5,7}, new int[]{3,0,6}, new int[]{2,4,1}, new int[]{0,5,7} },
        new int[4][] { new int[]{6,1,4}, new int[]{1,0,4}, new int[]{2,7,3}, new int[]{6,7,5} }
    };
    List<int[]> stage1Intersections = new List<int[]>();
    int[] targetCombination = new int[8];

    int[][] stage2Table = new int[8][]
    {
    new int[] { -1,1,0,3,4,0,0,7 },
    new int[] { 1,-1,2,3,1,5,1,1 },
    new int[] { 0,2,-1,2,4,2,6,7 },
    new int[] { 3,3,2,-1,3,5,6,7 },
    new int[] { 4,1,4,3,-1,4,6,4 },
    new int[] { 0,5,2,5,4,-1,5,5 },
    new int[] { 0,1,6,6,6,5,-1,6 },
    new int[] { 7,1,7,7,4,5,6,-1 },
    };
    int oneColor;
    int zeroColor;
    string initBinary;
    int initDecimal;
    string finalBinary;
    int finalDecimal;

    int[][] stage3Table = new int[8][]
    {
        new int[] { 2,4,7,6,5,1,3 },
        new int[] { 3,5,6,4,7,0,2 },
        new int[] { 1,7,4,5,6,3,0 },
        new int[] { 0,6,5,2,4,7,1 },
        new int[] { 7,2,3,1,0,5,6 },
        new int[] { 6,0,1,3,2,4,7 },
        new int[] { 5,3,0,7,1,2,4 },
        new int[] { 4,1,2,0,3,6,5 }
    };
    char[] alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    string[] wordList = new string[] { "ABOLISH", "BREVITY", "CRAFTED", "DRAGONS", "EVOKING", "FLICKER", "GHOSTLY", "HAYWIRE", "INSPECT", "JAILORS", "KERATIN", "LEOPARD", "MASTERY", "NUCLEAR", "OPTIMAL", "PAINFUL", "QUALIFY", "REBUILD", "SCOPING", "TEMPURA", "UNCLEAR", "VIBRATE", "WEDLOCK", "YOUNGER", "YOUNGER", "ZODIACS" };
    int[][] alphaOrders = new int[][] { new int[] { 0, 1, 6, 4, 3, 2, 5 }, new int[] { 0, 2, 4, 1, 5, 3, 6 }, new int[] { 2, 0, 6, 5, 3, 1, 4 }, new int[] { 2, 0, 3, 5, 4, 1, 6 }, new int[] { 0, 6, 4, 3, 5, 2, 1 }, new int[] { 3, 5, 0, 2, 4, 1, 6 }, new int[] { 0, 1, 5, 2, 3, 4, 6 }, new int[] { 1, 6, 0, 4, 5, 3, 2 }, new int[] { 5, 4, 0, 1, 3, 2, 6 }, new int[] { 1, 2, 0, 3, 4, 5, 6 }, new int[] { 3, 1, 5, 0, 6, 2, 4 }, new int[] { 4, 6, 1, 0, 2, 3, 5 }, new int[] { 1, 4, 0, 5, 2, 3, 6 }, new int[] { 5, 2, 4, 3, 0, 6, 1 }, new int[] { 5, 3, 6, 4, 0, 1, 2 }, new int[] { 1, 4, 2, 6, 3, 0, 5 }, new int[] { 2, 5, 4, 3, 0, 1, 6 }, new int[] { 2, 6, 1, 4, 5, 0, 3 }, new int[] { 1, 6, 4, 5, 2, 3, 0 }, new int[] { 6, 1, 2, 3, 5, 0, 4 }, new int[] { 5, 2, 4, 3, 1, 6, 0 }, new int[] { 4, 2, 6, 1, 3, 5, 0 }, new int[] { 5, 2, 1, 6, 3, 4, 0 }, new int[] { 5, 4, 3, 1, 6, 2, 0 }, new int[] { 5, 4, 3, 1, 6, 2, 0 }, new int[] { 4, 5, 2, 3, 1, 6, 0 } };
    int missingColor;
    int SNIndex;
    string chosenWord;
    int[] chosenOrder;
    List<int> correctOrder = new List<int>();
    List<int> pressedOrder = new List<int>();

    int tpCounter = 0;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable rung in ladder1Selectables)
        {
            rung.OnInteract += delegate () { rungBreak1(rung); return false; };
        }
        foreach (KMSelectable rung in ladder2Selectables)
        {
            rung.OnInteract += delegate () { rungBreak2(rung); return false; };
        }
        foreach (KMSelectable rung in ladder3Selectables)
        {
            rung.OnInteract += delegate () { rungBreak3(rung); return false; };
        }
        submitButton.OnInteract += delegate () { Submit(); return false; };

        allRungs[0] = ladder1Rungs; allRungs[1] = ladder2Rungs; allRungs[2] = ladder3Rungs;
        allSolid[0] = ladder1Solid; allSolid[1] = ladder2Solid; allSolid[2] = ladder3Solid;
        allBroke[0] = ladder1Broke; allBroke[1] = ladder2Broke; allBroke[2] = ladder3Broke;
        allSelectables[0] = ladder1Selectables; allSelectables[1] = ladder2Selectables; allSelectables[2] = ladder3Selectables;
        for (int i = 0; i < 8; i++)
        {
            ladderColorsFluid[i] = ladderColorOptions[i]; 
        }

    }

    void Start ()
    {
        GetArmColors();
    }

    void GetArmColors()
    {
        ladderColorsFluid.Shuffle();
        for (int i = 0; i < 3; i++)
        {
            armColors[i].material = ladderColorsFluid[i];
            armColors[i + 3].material = ladderColorsFluid[i];
        }
    }
    void Submit()
    {
        submitButton.AddInteractionPunch(1f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (stage == 0)
        {
            StartCoroutine(StageProgress(stage));
        }
        else
        {
            bool invalid = false;
            if (stage == 3)
            {
                Debug.Log(pressedOrder.Join());
                for (int i = 0; i < 7; i++)
                {
                    if (pressedOrder[i] != correctOrder[i])
                    {
                        invalid = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < allLadderStatuses[stage - 1].Length; i++)
                {
                    if (allLadderStatuses[stage - 1][i] != correctStatuses[stage - 1][i])
                    {
                        invalid = true;
                    }
                }
            }
            if (!invalid)
            {
                invalid = false;
                Debug.LogFormat("[Ladders #{0}] You pressed submit. That was correct.", moduleId);
                StartCoroutine(StageProgress(stage));
            }
            else
            {
                Debug.LogFormat("[Ladders #{0}] You pressed submit. That was incorrect, stage reset.", moduleId);
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine(Reset());
            }
        }
    }

    void rungBreak1(KMSelectable rung)
    {
        pressedIndex = Array.IndexOf(ladder1Selectables, rung);
        if (allLadderStatuses[0][pressedIndex] || !laddersActive[0] || isResetting)
        {return;}
        rungBreakAny(0, pressedIndex);
    }
    void rungBreak2(KMSelectable rung)
    {
        pressedIndex = Array.IndexOf(ladder2Selectables, rung);
        if (allLadderStatuses[1][pressedIndex] || !laddersActive[1] || isResetting)
        {return;}
        rungBreakAny(1, pressedIndex);
    }
    void rungBreak3(KMSelectable rung)
    {
        pressedIndex = Array.IndexOf(ladder3Selectables, rung);
        if (allLadderStatuses[2][pressedIndex] || !laddersActive[2] || isResetting)
        {return;}
        rungBreakAny(2, pressedIndex);
    }
    void rungBreakAny(int ladderIndex, int pressedIndex)
    {
        if (ladderIndex + 1 == stage)
        {
            Debug.LogFormat("[Ladders #{0}] You broke rung {1}!", moduleId, pressedIndex + 1);
        }
        if ((stage == 3) && allRungs[2].Contains(allRungs[ladderIndex][pressedIndex]))
        {
            pressedOrder.Add(ladderColors[2][pressedIndex]);
        }
        allSolid[ladderIndex][pressedIndex].SetActive(false);
        allBroke[ladderIndex][pressedIndex].SetActive(true);
        allLadderStatuses[ladderIndex][pressedIndex] = true;
        allSelectables[ladderIndex][pressedIndex].AddInteractionPunch(0.05f);
        Audio.PlaySoundAtTransform("target", transform);
    }

    void GenerateStage(int stage)
    {
        if (stage == 0) //All code for stage 1
        {
            Debug.LogFormat("[Ladders #{0}] ::STAGE 1::", moduleId);
            int row;
            if (Bomb.GetBatteryCount() <= 1)
            {
                row = 0;
            }
            else if (Bomb.GetBatteryCount() >= 4)
            {
                row = 2;
            }
            else row = 1;

            if (Bomb.GetSerialNumberLetters().Count() >= 3)
            {
                stage1Intersections.Add(stage1Table[row][0]);
            }
            if (Bomb.GetSerialNumberNumbers().Count() >= 3)
            {
                stage1Intersections.Add(stage1Table[row][1]);
            }
            if (Bomb.GetSerialNumber().Any(x => "AEIOU".Contains(x)))
            {
                stage1Intersections.Add(stage1Table[row][2]);
            }
            if (Bomb.GetSerialNumber().Any(x => "CL1MB".Contains(x)))
            {
                stage1Intersections.Add(stage1Table[row][3]);
            }
            List<string> loggingSequences = new List<string>();
            foreach (int[] array in stage1Intersections)
            {
                loggingSequences.Add(arrayToColors(array) + ", ");
            }
            Debug.LogFormat("[Ladders #{0}] The valid patterns in the table are{1}", moduleId, loggingSequences.Join());
            targetCombination = stage1Intersections[UnityEngine.Random.Range(0, stage1Intersections.Count())];
            Debug.LogFormat("[Ladders #{0}] The module will choose {1} to be the pattern that is shown in the ladder.", moduleId, arrayToColors(targetCombination));
            GenStage1Ladders();
            int cycleNum = UnityEngine.Random.Range(0, 8);
            ladderColors[0] = CycleRight(ladderColors[0], cycleNum);
            correctStatuses[0] = CycleRight(correctStatuses[0], cycleNum);
            SetRungColors(0, ladderColors[0]);
            Debug.LogFormat("[Ladders #{0}] The displayed sequence of colors is {1}.", moduleId, arrayToColors(ladderColors[0]));
        }
        else if (stage == 1) //All code for stage 2
        {
            Debug.LogFormat("[Ladders #{0}] ::STAGE 2::", moduleId);
            int[] colors = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            colors.Shuffle();
            int firstCol = colors[0];
            int secondCol = colors[1];
            int separator = colors[2];

            oneColor = stage2Table[firstCol][secondCol];
            if (oneColor == firstCol)
            {
                zeroColor = secondCol;
            }
            else zeroColor = firstCol;

            
            ladderColors[1][0] = separator;
            ladderColors[1][1] = oneColor; ladderColors[1][2] = oneColor;
            ladderColors[1][3] = zeroColor; ladderColors[1][4] = zeroColor;
            for (int i = 5; i < 9; i++)
            {
                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    ladderColors[1][i] = oneColor;
                }
                else ladderColors[1][i] = zeroColor;
            }
            ladderColors[1].Shuffle();
            for (int i = 0; i < 9; i++)
            {
                if (ladderColors[1][0] == separator)
                {
                    break;
                }
                else ladderColors[1] = CycleRight(ladderColors[1], 1);
            }
            for (int i = 1; i < 9; i++)
            {
                if (ladderColors[1][i] == oneColor)
                {
                    initBinary += "1";
                }
                else if (ladderColors[1][i] == zeroColor)
                {
                    initBinary += "0";
                }
            }

            initDecimal = Convert.ToInt32(initBinary, 2);
            Debug.LogFormat("[Ladders #{0}] The color representing 1 will be {1}, the color representing 0 will be {2}, and the separator color will be {3}", moduleId, ladderColorOptions[oneColor].name, ladderColorOptions[zeroColor].name, ladderColorOptions[separator].name);
            Debug.LogFormat("[Ladders #{0}] The generated binary string is {1}, which is {2} in decimal", moduleId, initBinary, initDecimal);
            switch (separator)
            {
                case 0:
                    finalDecimal = 3 * initDecimal; break;
                case 1:
                    if (Bomb.GetPortCount() == 0)
                    {
                        finalDecimal = 2 * initDecimal;
                    }
                    else finalDecimal = Bomb.GetPortCount() * initDecimal; break;
                case 2:
                    finalDecimal = Convert.ToInt32(Math.Pow(((Convert.ToDouble(initDecimal) - 1) % 9) + 1, 3)); break;
                case 3:
                    finalDecimal = initDecimal + Bomb.GetModuleNames().Count(); break;
                case 4:
                    finalDecimal = 500 - initDecimal; break;
                case 5:
                    if (Bomb.GetSerialNumberNumbers().Max() == 0)
                    {
                        finalDecimal = initDecimal * 2;
                    }
                    else finalDecimal = initDecimal * Bomb.GetSerialNumberNumbers().Max(); break;
                case 6:
                    finalDecimal = initDecimal + Bomb.GetSerialNumberNumbers().Sum(); break;
                case 7:
                    finalDecimal = (initDecimal + 1) / 2; break;
                default: break;
            }
            finalDecimal %= 256;
            finalBinary = Convert.ToString(finalDecimal, 2);
            while (finalBinary.Length < 8)
            {
                finalBinary = "0" + finalBinary;
            }
            Debug.LogFormat("[Ladders #{0}] After the separator modifications, the value to submit is {1}, which is {2} in binary.", moduleId, finalDecimal, finalBinary);
            for (int i = 0; i < 9; i++)
            {
                string finalFinalBinary = "0" + finalBinary;
                if (finalFinalBinary[i] == '1')
                {
                    correctStatuses[1][i] = true;
                }
            }
            int cycleNum = UnityEngine.Random.Range(0, 9);
            CycleRight(ladderColors[1], cycleNum);
            CycleRight(correctStatuses[1], cycleNum);
            Debug.LogFormat("[Ladders #{0}] The displayed sequence of colors is {1}.", moduleId, arrayToColors(ladderColors[1]));
            SetRungColors(1, ladderColors[1]);
        }
        else if (stage == 2) //All code for stage 3 :)
        {
            Debug.LogFormat("[Ladders #{0}] ::STAGE 3::", moduleId);
            int[] colors = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            colors.Shuffle();
            for (int i = 0; i < 7; i++)
            {
                ladderColors[2][i] = colors[i];
            }
            missingColor = colors[7];
            SNIndex = Array.IndexOf(alphabet, Bomb.GetSerialNumber()[3]);
            chosenWord = wordList[SNIndex];
            chosenOrder = alphaOrders[SNIndex];
            for (int i = 0; i < 7; i++)
            {
                correctOrder.Add(stage3Table[missingColor][chosenOrder[i]]);
            }
            Debug.LogFormat("[Ladders #{0}] The ladder colors are {1}, which leaves {2} as the missing color.", moduleId, arrayToColors(ladderColors[2]), colorNames[missingColor]);
            Debug.LogFormat("[Ladders #{0}] The chosen word is {1}, and so the breaking order is {2}", moduleId, chosenWord, arrayToColors(correctOrder.ToArray()));
            SetRungColors(2, ladderColors[2]);
        }
    }
    void GenStage1Ladders()
    {
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            Array.Reverse(targetCombination);
        }
        for (int i = 0; i < 3; i++)
        {
            ladderColors[0][i] = targetCombination[i];
            correctStatuses[0][i] = true;
        }
        for (int i = 3; i < 8; i++)
        {
            ladderColors[0][i] = UnityEngine.Random.Range(0, 8);
        }
        for (int i = 0; i < 8; i++)
        {
            int[] threeAdj = new int[] { ladderColors[0][i % 8], ladderColors[0][(i + 1) % 8], ladderColors[0][(i + 2) % 8], };

            int cnt = 0;
            foreach (int[] possibleCombination in stage1Intersections)
            {
                
                if (((threeAdj[0] == possibleCombination[0]) && (threeAdj[1] == possibleCombination[1]) && (threeAdj[2] == possibleCombination[2])) || ((threeAdj[2] == possibleCombination[0]) && (threeAdj[1] == possibleCombination[1]) && (threeAdj[0] == possibleCombination[2])))
                {
                    cnt++;
                }
            }
            if (cnt > 1)
            {
                GenStage1Ladders();
            }
        }
    }

    int[] CycleRight(int[] array, int amount)
    {
        int[] temp = new int[array.Length];
        int[] output = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            temp[i] = array[(i + amount) % array.Length];
        }
        for (int i = 0; i < array.Length; i++)
        {
            output[i] = temp[i];
        }
        return output;
    }
    bool[] CycleRight(bool[] array, int amount)
    {
        bool[] temp = new bool[array.Length];
        bool[] output = new bool[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            temp[i] = array[(i + amount) % array.Length];
        }
        for (int i = 0; i < array.Length; i++)
        {
            output[i] = temp[i];
        }
        return output;
    }
    string arrayToColors(int[] array)
    {
        string template = string.Empty;
        foreach (int number in array)
        {
            template += colorNames[number] + " ";
        }
        return template;
    }
    

    void SetRungColors(int ladderNum, int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Material chosenColor = ladderColorOptions[array[i]];
            allSolid[ladderNum][i].GetComponent<MeshRenderer>().material = chosenColor;
            allBroke[ladderNum][i].GetComponent<MeshRenderer>().material = chosenColor;
            allBroke[ladderNum][i].SetActive(false);
            allRungs[ladderNum][i].GetComponentInChildren<TextMesh>().text = string.Empty;

            if (TwitchPlaysActive)
            {
                allRungs[ladderNum][i].GetComponentInChildren<TextMesh>().text += alphabet[tpCounter];
                tpCounter++;
            }
        }
    }

    IEnumerator StageProgress(int ladderToMove)
    {
        if (moduleSolved)
        {
            yield break;
        }
        if (stage == 3)
        {
            if (moduleSolved)
            {
                yield break;
            }
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
            isResetting = true;
            float elapsed = 0f;
            float duration = 1.2f;
            while (elapsed < duration)
            {
                Vector3 topPos = topShutter.transform.localPosition;
                Vector3 bottomPos = bottomShutter.transform.localPosition;
                Vector3 topScl = topShutter.transform.localScale;
                Vector3 bottomScl = bottomShutter.transform.localScale;
                topShutter.transform.localPosition = new Vector3(topPos.x, topPos.y, topPos.z - 0.0005f);
                bottomShutter.transform.localPosition = new Vector3(bottomPos.x, bottomPos.y, bottomPos.z + 0.0005f);
                topShutter.transform.localScale = new Vector3(topScl.x - 0.0001f, topScl.y, topScl.z);
                bottomShutter.transform.localScale = new Vector3(bottomScl.x + 0.0001f, bottomScl.y, bottomScl.z);
                yield return null;
                elapsed += Time.deltaTime;
            }
            StopAllCoroutines();    
        }
        else
        {
            laddersActive[stage] = true;
            StartCoroutine(RungsMove(stage, stage));
            GenerateStage(stage);
            stage++;
            GameObject arm = ladderArms[ladderToMove];
            float elapsed = 0f;
            float duration = 3.75f;
            while (elapsed < duration)
            {
                Transform ladTran = arm.transform;

                ladTran.localPosition = new Vector3(ladTran.localPosition.x, -0.075f + ladTran.localPosition.y, ladTran.localPosition.z);
                ladTran.localScale = new Vector3(ladTran.localScale.x, 0.1f + ladTran.localScale.y, ladTran.localScale.z);
                yield return null;
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
    }
    IEnumerator RungsMove(int inputNum, int stageNum)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        float elapsed = 0f;
        const float duration = 0.1f;
        while (elapsed < duration)
        { 
            Transform movedRung = allRungs[stageNum][inputNum % allRungs[stageNum].Length].transform;
            float x = movedRung.localPosition.x;
            float y = movedRung.localPosition.y;
            float z = movedRung.localPosition.z;

            movedRung.localPosition = new Vector3(x, y - 0.15f, z);
            yield return null;
            elapsed += Time.deltaTime;
        }
        StartCoroutine(RungsMove(inputNum + 1, stageNum));
        elapsed = 0f;
        float duration2 = 3.6f;
        while (elapsed < duration2)
        { 
            Transform movedRung = allRungs[stageNum][inputNum % allRungs[stageNum].Length].transform;
            float x = movedRung.localPosition.x;
            float y = movedRung.localPosition.y;
            float z = movedRung.localPosition.z;

            movedRung.localPosition = new Vector3(x, y - 0.15f, z);
            yield return null;
            elapsed += Time.deltaTime;
        }
        allRungs[stageNum][inputNum % allRungs[stageNum].Length].transform.localPosition = new Vector3(0.25f, 0f, -0.75f);

    }
    IEnumerator Reset()
    {
        if (isResetting)
        {
            yield break;
        }
        isResetting = true;
        float elapsed = 0f;
        float duration = 1.1f;
        while (elapsed < duration)
        {
            Vector3 topPos = topShutter.transform.localPosition;
            Vector3 bottomPos = bottomShutter.transform.localPosition;
            Vector3 topScl = topShutter.transform.localScale;
            Vector3 bottomScl = bottomShutter.transform.localScale;
            topShutter.transform.localPosition = new Vector3(topPos.x, topPos.y, topPos.z - 0.0005f);
            bottomShutter.transform.localPosition = new Vector3(bottomPos.x, bottomPos.y, bottomPos.z + 0.0005f);
            topShutter.transform.localScale = new Vector3(topScl.x - 0.0001f, topScl.y, topScl.z );
            bottomShutter.transform.localScale = new Vector3(bottomScl.x + 0.0001f, bottomScl.y, bottomScl.z);
            yield return null;
            elapsed += Time.deltaTime;
        }
        for (int i = 0; i < allLadderStatuses[stage - 1].Length; i++)
        {
            allLadderStatuses[stage - 1][i] = false;
            allSolid[stage - 1][i].SetActive(true);
            allBroke[stage - 1][i].SetActive(false);
            pressedOrder.Clear();

        }
        elapsed = 0f;
        yield return new WaitForSecondsRealtime(0.5f);
        while (elapsed < duration)
        {
            Vector3 topPos = topShutter.transform.localPosition;
            Vector3 bottomPos = bottomShutter.transform.localPosition;
            Vector3 topScl = topShutter.transform.localScale;
            Vector3 bottomScl = bottomShutter.transform.localScale;
            topShutter.transform.localPosition = new Vector3(topPos.x, topPos.y, topPos.z + 0.0005f);
            bottomShutter.transform.localPosition = new Vector3(bottomPos.x, bottomPos.y, bottomPos.z - 0.0005f);
            topShutter.transform.localScale = new Vector3(topScl.x + 0.0001f, topScl.y, topScl.z);
            bottomShutter.transform.localScale = new Vector3(bottomScl.x - 0.0001f, bottomScl.y, bottomScl.z);
            yield return null;
            elapsed += Time.deltaTime;
        }
        isResetting = false;
        yield return null;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} submit> to press the submit button. Use <!{0} break A B C> to break the rungs labelled A, B, and C.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand (string Command)
    {
        string[] parameters = Command.Trim().ToUpperInvariant().Split(' ');

        if ((parameters.Length == 1) && (parameters[0] == "SUBMIT"))
        {
            yield return null;
            submitButton.OnInteract();
            yield return new WaitForSecondsRealtime(0.1f);
        }
        else if ((parameters.Length > 1) && (parameters[0] == "BREAK"))
        {
            string possibleLetters = string.Empty;
            List<string> rungsToBreak = new List<string>();
            switch (stage)
            {
                case 0: yield return "sendtochaterror You can't break any ladders yet you silly goose!"; break;
                case 1: possibleLetters += " ABCDEFGH"; break;
                case 2: possibleLetters += " IJKLMNOPQ"; break;
                case 3: possibleLetters += " RSTUVWX"; break;
            }
            for (int i = 1; i < parameters.Length; i++)
            {
                rungsToBreak.Add(parameters[i]);
            }
            if (!rungsToBreak.All(x => possibleLetters.Contains(x)))
            {
                yield return "sendtochaterror You cannot break that rung yet.";
            }
            else
            {
                yield return null;
                foreach (string letter in rungsToBreak)
                {
                    foreach (GameObject[] array in allRungs)
                    {
                        foreach (GameObject rung in array)
                        {
                            if (rung.GetComponentInChildren<TextMesh>().text == letter)
                            {
                                rung.GetComponentInChildren<KMSelectable>().OnInteract();
                                yield return new WaitForSecondsRealtime(0.2f);
                            }
                        }
                    }
                }
            }
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (!moduleSolved)
        {
            stage = 3;
            StartCoroutine(StageProgress(0));
            yield return null;
        }
    }
}
