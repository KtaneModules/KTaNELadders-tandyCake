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
    public KMColorblindMode Colorblind;

    public KMSelectable submitButton;
    public GameObject[] ladderArms;
    public Material[] ladderColorOptions; // ROYGBCPA
    public GameObject topShutter;
    public GameObject bottomShutter;
    public bool TwitchPlaysActive;
    public bool cbON;

    public MeshRenderer[] armColors = new MeshRenderer[6];
    public KMSelectable[] ladder1Selectables, ladder2Selectables, ladder3Selectables;
    public GameObject[] ladder1Rungs, ladder1Solid, ladder1Broke;
    public GameObject[] ladder2Rungs, ladder2Solid, ladder2Broke;
    public GameObject[] ladder3Rungs, ladder3Solid, ladder3Broke;
    public TextMesh[] ladder1CB, ladder2CB, ladder3CB;
    private KMSelectable[][] allSelectables = new KMSelectable[3][];
    private GameObject[][] allRungs = new GameObject[3][] { new GameObject[8], new GameObject[9], new GameObject[7] };
    private GameObject[][] allSolid = new GameObject[3][] { new GameObject[8], new GameObject[9], new GameObject[7] };
    private GameObject[][] allBroke = new GameObject[3][] { new GameObject[8], new GameObject[9], new GameObject[7] };
    private TextMesh[][] allCB = new TextMesh[3][] { new TextMesh[8], new TextMesh[9], new TextMesh[7] };

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

    int[,][] stage1Table = new int[,][]
    {
        { new int[]{6,5,3}, new int[]{1,2,3}, new int[]{0,5,0}, new int[]{7,4,3} },
        { new int[]{5,5,7}, new int[]{3,0,6}, new int[]{2,4,1}, new int[]{0,5,7} },
        { new int[]{6,1,4}, new int[]{1,0,4}, new int[]{2,7,3}, new int[]{6,7,5} }
    };
    List<int[]> stage1Intersections = new List<int[]>();
    int[] targetCombination = new int[8];

    int[,] stage2Table = new int[,]
    {
        { -1,1,0,3,4,0,0,7 },
        { 1,-1,2,3,1,5,1,1 },
        { 0,2,-1,2,4,2,6,7 },
        { 3,3,2,-1,3,5,6,7 },
        { 4,1,4,3,-1,4,6,4 },
        { 0,5,2,5,4,-1,5,5 },
        { 0,1,6,6,6,5,-1,6 },
        { 7,1,7,7,4,5,6,-1 },
    };
    int oneColor;
    int zeroColor;
    string initBinary;
    int initDecimal;
    string finalBinary;
    int finalDecimal;

    int[,] stage3Table = new int[,]
    {
        { 2,4,7,6,5,1,3 },
        { 3,5,6,4,7,0,2 },
        { 1,7,4,5,6,3,0 },
        { 0,6,5,2,4,7,1 },
        { 7,2,3,1,0,5,6 },
        { 6,0,1,3,2,4,7 },
        { 5,3,0,7,1,2,4 },
        { 4,1,2,0,3,6,5 }
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
    string possibleLetters = string.Empty;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable rung in ladder1Selectables)
            rung.OnInteract += delegate () { rungBreakAny(0, Array.IndexOf(ladder1Selectables, rung)); return false; };
        foreach (KMSelectable rung in ladder2Selectables)
            rung.OnInteract += delegate () { rungBreakAny(1, Array.IndexOf(ladder2Selectables, rung)); return false; };
        foreach (KMSelectable rung in ladder3Selectables)
            rung.OnInteract += delegate () { rungBreakAny(2, Array.IndexOf(ladder3Selectables, rung)); return false; };
        submitButton.OnInteract += delegate () { Submit(); return false; };
        GetComponent<KMBombModule>().OnActivate += delegate () { cbON = Colorblind.ColorblindModeActive; };

        allRungs[0] = ladder1Rungs; allRungs[1] = ladder2Rungs; allRungs[2] = ladder3Rungs;
        allSolid[0] = ladder1Solid; allSolid[1] = ladder2Solid; allSolid[2] = ladder3Solid;
        allBroke[0] = ladder1Broke; allBroke[1] = ladder2Broke; allBroke[2] = ladder3Broke;
        allSelectables[0] = ladder1Selectables; allSelectables[1] = ladder2Selectables; allSelectables[2] = ladder3Selectables;
        allCB[0] = ladder1CB; allCB[1] = ladder2CB; allCB[2] = ladder3CB;

    }
    void Start ()
    {
        GetArmColors();
    }

    void GetArmColors()
    {

        Material[] armPossibilities = ladderColorOptions.ToArray();
        armPossibilities.Shuffle();
        for (int i = 0; i < 3; i++)
        {
            armColors[i].material = armPossibilities[i];
            armColors[i + 3].material = armPossibilities[i];
        }
    }
    void Submit()
    {
        //Debug.Log(stage);
        submitButton.AddInteractionPunch(1f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (moduleSolved)

            return;
        if (stage == 0)
        //if (true)
            StartCoroutine(StageProgress(stage));
        else
        {
            bool valid;
            if (stage == 3)
                valid = pressedOrder.SequenceEqual(correctOrder);
            else
                valid = allLadderStatuses[stage - 1].SequenceEqual(correctStatuses[stage - 1]);
            if (valid)
            {
                Debug.LogFormat("[Ladders #{0}] You pressed submit. That was correct.", moduleId);
                StartCoroutine(StageProgress(stage));
            }
            else
            {
                Debug.Log(allLadderStatuses[stage - 1].Join());
                Debug.Log(correctStatuses[stage - 1].Join());

                Debug.LogFormat("[Ladders #{0}] You pressed submit. That was incorrect, stage reset.", moduleId);
                GetComponent<KMBombModule>().HandleStrike();
                pressedOrder.Clear();
                StartCoroutine(Reset());
            }
        }
    }

    void rungBreakAny(int ladderIndex, int pressedIndex)
    {
        if (allLadderStatuses[ladderIndex][pressedIndex] || !laddersActive[ladderIndex] || isResetting)
            return;
        Audio.PlaySoundAtTransform("target", transform);

        if (ladderIndex + 1 == stage)
            Debug.LogFormat("[Ladders #{0}] You broke rung {1}!", moduleId, pressedIndex + 1);
        if ((stage == 3) && ladderIndex == 2)
            pressedOrder.Add(ladderColors[2][pressedIndex]);
        allLadderStatuses[ladderIndex][pressedIndex] = true;
        allSolid[ladderIndex][pressedIndex].SetActive(false);
        allBroke[ladderIndex][pressedIndex].SetActive(true);
    }

    void GenerateStage(int stage)
    {
        if (stage == 0) //All code for stage 1
        {
            Debug.LogFormat("[Ladders #{0}] ::STAGE 1::", moduleId);
            int row;
            if (Bomb.GetBatteryCount() <= 1)
                row = 0;
            else if (Bomb.GetBatteryCount() >= 4)
                row = 2;
            else row = 1;

            bool[] colConditions = new[] { Bomb.GetSerialNumberLetters().Count() >= 3, Bomb.GetSerialNumberNumbers().Count() >= 3, Bomb.GetSerialNumber().Any(x => "AEIOU".Contains(x)), Bomb.GetSerialNumber().Any(x => "CL1MB".Contains(x)) };
            for (int i = 0; i < 4; i++)
                if (colConditions[i])
                    stage1Intersections.Add(stage1Table[row, i]);

            Debug.LogFormat("[Ladders #{0}] The valid patterns in the table are {1}", moduleId, stage1Intersections.Select(seq => seq.Select(color => colorNames[color]).Join()).Join(", "));
            targetCombination = stage1Intersections.PickRandom();
            Debug.LogFormat("[Ladders #{0}] The module will choose {1} to be the pattern that is shown in the ladder.", moduleId, targetCombination.Select(x => colorNames[x]).Join());
            GenStage1Ladders();
            int cycleNum = UnityEngine.Random.Range(0, 8);
            ladderColors[0] = CycleRight(ladderColors[0], cycleNum);
            correctStatuses[0] = CycleRight(correctStatuses[0], cycleNum);
            SetRungColors(0, ladderColors[0]);
            Debug.LogFormat("[Ladders #{0}] The displayed sequence of colors is {1}.", moduleId, ladderColors[0].Select(x => colorNames[x]).Join());
        }
        else if (stage == 1) //All code for stage 2
        {
            Debug.LogFormat("[Ladders #{0}] ::STAGE 2::", moduleId);
            var colors = Enumerable.Range(0, 8).ToArray().Shuffle().Take(3);
            int firstCol  = colors.ElementAt(0);
            int secondCol = colors.ElementAt(1);
            int separator = colors.ElementAt(2);
            oneColor = stage2Table[firstCol, secondCol];
            zeroColor = oneColor == firstCol ? secondCol : firstCol;
            
            ladderColors[1][0] = separator;
            ladderColors[1][1] = oneColor;
            ladderColors[1][2] = oneColor;
            ladderColors[1][3] = zeroColor;
            ladderColors[1][4] = zeroColor;
            for (int i = 0; i < 4; i++)
                ladderColors[1][i + 5] = (UnityEngine.Random.Range(0, 2) == 0) ? oneColor : zeroColor;
            ladderColors[1].Shuffle();
            while (ladderColors[1][0] != separator)
            {
                ladderColors[1] = CycleRight(ladderColors[1], 1);
            }
            for (int i = 1; i < 9; i++)
                initBinary += (ladderColors[1][i] == oneColor) ? "1" : "0";

            initDecimal = Convert.ToInt32(initBinary, 2);
            Debug.LogFormat("[Ladders #{0}] The color representing 1 will be {1}, the color representing 0 will be {2}, and the separator color will be {3}", moduleId, ladderColorOptions[oneColor].name, ladderColorOptions[zeroColor].name, ladderColorOptions[separator].name);
            Debug.LogFormat("[Ladders #{0}] The generated binary string is {1}, which is {2} in decimal", moduleId, initBinary, initDecimal);
            switch (separator)
            {
                case 0: finalDecimal = 3 * initDecimal; break;
                case 1: finalDecimal = initDecimal * ((Bomb.GetPortCount() == 0) ? 2 : Bomb.GetPortCount()); break;
                case 2: finalDecimal = (int)Math.Pow((initDecimal - 1) % 9 + 1, 3); break;
                case 3: finalDecimal = initDecimal + Bomb.GetModuleNames().Count(); break;
                case 4: finalDecimal = 500 - initDecimal; break;
                case 5: finalDecimal = initDecimal * ((Bomb.GetSerialNumberNumbers().Max() == 0) ? 2 : Bomb.GetSerialNumberNumbers().Max()); break;
                case 6: finalDecimal = initDecimal + Bomb.GetSerialNumberNumbers().Sum(); break;
                case 7: finalDecimal = (initDecimal + 1) / 2; break;
            }
            finalDecimal %= 256;
            finalBinary = Convert.ToString(finalDecimal, 2).PadLeft(8, '0');
            Debug.LogFormat("[Ladders #{0}] After the separator modifications, the value to submit is {1}, which is {2} in binary.", moduleId, finalDecimal, finalBinary);
            for (int i = 1; i < 9; i++)
                correctStatuses[1][i] = finalBinary[i - 1] == '1';
            int cycleNum = UnityEngine.Random.Range(0, 9);
            ladderColors[1] = CycleRight(ladderColors[1], cycleNum);
            correctStatuses[1] = CycleRight(correctStatuses[1], cycleNum);
            SetRungColors(1, ladderColors[1]);
            Debug.LogFormat("[Ladders #{0}] The displayed sequence of colors is {1}.", moduleId, ladderColors[1].Select(x => colorNames[x]).Join());
        }
        else if (stage == 2) //All code for stage 3 :(
        {
            Debug.LogFormat("[Ladders #{0}] ::STAGE 3::", moduleId);
            int[] colors = Enumerable.Range(0, 8).ToArray().Shuffle();
            for (int i = 0; i < 7; i++)
                ladderColors[2][i] = colors[i];
            missingColor = colors[7];
            SNIndex = Bomb.GetSerialNumber()[3] - 'A';
            chosenOrder = alphaOrders[SNIndex];
            for (int i = 0; i < 7; i++)
                correctOrder.Add(stage3Table[missingColor, chosenOrder[i]]); 
            Debug.LogFormat("[Ladders #{0}] The ladder colors are {1}, which leaves {2} as the missing color.", moduleId, ladderColors[2].Select(x => colorNames[x]).Join(), ladderColorOptions[missingColor].name);
            Debug.LogFormat("[Ladders #{0}] The chosen word is {1}, and so the breaking order is {2}", moduleId, wordList[SNIndex], correctOrder.Select(x => colorNames[x]).Join());
            SetRungColors(2, ladderColors[2]);
        }
    }
    void GenStage1Ladders()
    {
        if (UnityEngine.Random.Range(0, 2) == 0)
            Array.Reverse(targetCombination);
        for (int i = 0; i < 3; i++)
        {
            ladderColors[0][i] = targetCombination[i];
            correctStatuses[0][i] = true;
        }
        for (int i = 3; i < 8; i++)
            ladderColors[0][i] = UnityEngine.Random.Range(0, 8);
        int cnt = 0;
        for (int i = 0; i < 8; i++)
        {
            int[] threeAdj = new int[] { ladderColors[0][i % 8], ladderColors[0][(i + 1) % 8], ladderColors[0][(i + 2) % 8], };
            foreach (int[] posCombin in stage1Intersections)
                if (posCombin.SequenceEqual(threeAdj) || posCombin.SequenceEqual(threeAdj.Reverse())) 
                    cnt++;
        }
        if (cnt != 1)
            GenStage1Ladders();
    }

    int[] CycleRight(int[] input, int amount)
    {
        int[] temp = new int[input.Length];
        for (int i = 0; i < input.Length; i++)
            temp[i] = input[(i + amount) % input.Length];
        return temp;
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

    void SetRungColors(int ladderNum, int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Material chosenColor = ladderColorOptions[array[i]];
            allSolid[ladderNum][i].GetComponent<MeshRenderer>().material = chosenColor;
            allBroke[ladderNum][i].GetComponent<MeshRenderer>().material = chosenColor;
            allCB[ladderNum][i].text = colorNames[array[i]];
            allBroke[ladderNum][i].SetActive(false);
            allRungs[ladderNum][i].GetComponentInChildren<TextMesh>().text = string.Empty;

            if (TwitchPlaysActive)
            {
                allRungs[ladderNum][i].GetComponentInChildren<TextMesh>().text += alphabet[tpCounter];
                tpCounter++;
            }
        }
        SetCB();
    }

    void SetCB()
    {
        for (int ladderPos = 0; ladderPos < 3; ladderPos++)
            foreach (TextMesh text in allCB[ladderPos])
                text.gameObject.SetActive(cbON);
    }

    IEnumerator StageProgress(int ladderToMove)
    {
        if (moduleSolved)
            yield break;
        if (stage == 3)
        {
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
            isResetting = true;
            while (topShutter.transform.localScale.x < 0.007)
            {
                topShutter.transform.localPosition -= new Vector3(0, 0, 0.05f * Time.deltaTime);
                bottomShutter.transform.localPosition += new Vector3(0, 0, 0.05f * Time.deltaTime);
                topShutter.transform.localScale += new Vector3(0.01f * Time.deltaTime, 0, 0);
                bottomShutter.transform.localScale += new Vector3(0.01f * Time.deltaTime, 0, 0);
                yield return null;
            }
        }
        else
        {
            laddersActive[stage] = true;
            StartCoroutine(RungsMove(stage, stage));
            GenerateStage(stage);
            stage++;
            Transform arm = ladderArms[ladderToMove].transform;
            while (arm.localScale.y < 21)
            {
                arm.localPosition -= new Vector3(0, 4 * Time.deltaTime, 0);
                arm.localScale -= new Vector3(0, 5.33f* Time.deltaTime, 0);
                yield return null;
            }
            yield return null;
        }
    }
    IEnumerator RungsMove(int inputNum, int stageNum)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Transform movedRung = allRungs[stageNum][inputNum % allRungs[stageNum].Length].transform;
        while (movedRung.localPosition.y > -1.5f)
        { 
            movedRung.localPosition -= new Vector3(0, 8*Time.deltaTime, 0);
            yield return null;
        }
        StartCoroutine(RungsMove(inputNum + 1, stageNum));
        while (allRungs[stageNum][inputNum % allRungs[stageNum].Length].transform.localPosition.y > -33)
        {
            movedRung.localPosition -= new Vector3(0, 8*Time.deltaTime, 0);
            yield return null;
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
        while (topShutter.transform.localScale.x < 0.007)
        {
            topShutter.transform.localPosition -= new Vector3(0, 0, 0.05f * Time.deltaTime);
            bottomShutter.transform.localPosition += new Vector3(0, 0, 0.05f * Time.deltaTime);
            topShutter.transform.localScale += new Vector3(0.01f * Time.deltaTime, 0, 0);
            bottomShutter.transform.localScale += new Vector3(0.01f * Time.deltaTime, 0, 0);
            yield return null;
        }
        for (int i = 0; i < allLadderStatuses[stage - 1].Length; i++)
        {
            allLadderStatuses[stage - 1][i] = false;
            allSolid[stage - 1][i].SetActive(true);
            allBroke[stage - 1][i].SetActive(false);
            pressedOrder.Clear();

        }
        yield return new WaitForSecondsRealtime(0.5f);
        while (topShutter.transform.localScale.x > 0.0001f)
        {
            topShutter.transform.localPosition += new Vector3(0, 0, 0.05f * Time.deltaTime);
            bottomShutter.transform.localPosition -= new Vector3(0, 0, 0.05f * Time.deltaTime);
            topShutter.transform.localScale -= new Vector3(0.01f * Time.deltaTime, 0, 0);
            bottomShutter.transform.localScale -= new Vector3(0.01f * Time.deltaTime, 0, 0);
            yield return null;
        }
        isResetting = false;
        yield return null;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} submit> to press the submit button. Use <!{0} break A B C> to break the rungs labelled A, B, and C. Use <!{0} colorblind> to toggle colorblind mode.";
    #pragma warning restore 414
    
    IEnumerator ProcessTwitchCommand (string Command)
    {
        string[] parameters = Command.Trim().ToUpperInvariant().Split(' ');

        if (parameters.Length == 1 && new string[] { "COLORBLIND", "COLOURBLIND", "COLOR-BLIND", "COLOUR-BLIND", "CB" }.Contains(parameters[0]))
        {
            yield return null;
            cbON = !cbON;
            SetCB();
        }

        if ((parameters.Length == 1) && (parameters[0] == "SUBMIT"))
        {
            yield return null;
            submitButton.OnInteract();
            yield return new WaitForSecondsRealtime(0.1f);
        }
        else if ((parameters.Length > 1) && (parameters[0] == "BREAK"))
        {
            List<string> rungsToBreak = new List<string>();
            switch (stage)
            {
                case 0: yield return "sendtochaterror You can't break any ladders yet you silly goose!"; yield break    ;
                case 1: possibleLetters = " ABCDEFGH"; break;
                case 2: possibleLetters = " ABCDEFGHIJKLMNOPQ"; break;
                case 3: possibleLetters = " ABCDEFGHIJKLMNOPQRSTUVWX"; break;
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
                                if (!allLadderStatuses[Array.IndexOf(allRungs, array)][Array.IndexOf(array, rung)])
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
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (stage == 0)
        {
            submitButton.OnInteract();
            yield return true;
        }
        while (!moduleSolved)
        {
            switch (stage)
            {
                case 1: case 2:
                    int cnt = 0;
                    foreach (KMSelectable rung in allSelectables[stage - 1])
                    {
                        if (correctStatuses[stage - 1][cnt] && !allLadderStatuses[stage - 1][cnt])
                        {
                            rung.OnInteract();
                            yield return new WaitForSeconds(0.2f);
                        }
                        cnt++;
                    }
                    break;
                case 3:
                    if (pressedOrder.Count > 0) { StartCoroutine(Reset()); yield return new WaitForSeconds(3); }
                    for (int i = 0; i < 7; i++)
                    {
                        if (!allLadderStatuses[2][Array.IndexOf(ladderColors[2], correctOrder[i])])
                        {
                            allSelectables[2][Array.IndexOf(ladderColors[2], correctOrder[i])].OnInteract();
                            yield return new WaitForSeconds(0.2f);
                        }
                    }
                    break;
            }
            if (correctStatuses[stage - 1].SequenceEqual(allLadderStatuses[stage - 1]))
            {
                submitButton.OnInteract();
            }
            else { StartCoroutine(Reset()); yield return new WaitForSeconds(3); }
            yield return true;
        }
    }
}
