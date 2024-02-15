using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using File = System.IO.File;
// Jak překládat
// 1, source sentence:      "Dal jsi to vedoucímu, ten spis?"
// 2, detect varible words: "Dal jsi to <id=1,vedouc|ímu>, ten <id=2,spis>?"
// 3, more info:            "Dal jsi to <id=1,3.pád,č. j.,rod muž.,vzor. průvodčí>, <id=2,4.pád,č j.,rod muž> <id=3,4.pád,č j.,rod muž,vzor: hrad>?"
// 4, change to pattern:    "Dals <id=1,vedoucí,3.pád,č. j.,rod muž.,vzor. průvodčí> <id=2,4.pád,č j.,rod muž> <id=3,spis,4.pád,č j.,rod muž,vzor: hrad>?"
// Find if match... if not try alternative out pattern
// 5, alternative:          "Dals <id=1,vedóc|i,3.pád,č. j.,rod muž.,vzor. průvodčí> <id=2,4.pád,rod+č.=(id=3)> <id=3,lejstr|o,4.pád>?"
// 6, translate words:      "Dals <id=1,vedócimô> <id=2,to> <id=3,lejstro>?"
// 7, display:              "Dals vedócimô to lejstro?"

namespace TranslatorWritter {
    public partial class FormMain :Form{
        // Poslední verze projektu
        public const int CurrentVersion=2;

        #region Lists
        List<ItemSimpleWord> itemsSimpleWords, itemsSimpleWordsFiltered;
        ItemSimpleWord CurrentSimpleWord;
        List<ItemPhrase> itemsPhrases, itemsPhrasesFiltered;
        ItemPhrase CurrentPhrase;
        List<ItemSentencePart> itemsSentenceParts, itemsSentencePartsFiltered;
        ItemSentencePart CurrentSentencePart;
        List<ItemSentence> itemsSentences, itemsSentencesFiltered;
        ItemSentence CurrentSentence;
        List<ItemSentencePatternPart> itemsSentencePatternParts, itemsSentencePatternPartsFiltered;
        ItemSentencePatternPart CurrentSentencePatternPart;
        List<ItemSentencePattern> itemsSentencePatterns, itemsSentencePatternsFiltered;
        ItemSentencePattern CurrentSentencePattern;
        List<ItemReplaceS> itemsReplaceS, itemsReplaceSFiltered;
        ItemReplaceS CurrentReplaceS;
        List<ItemReplaceG> itemsReplaceG, itemsReplaceGFiltered;
        ItemReplaceG CurrentReplaceG;
        List<ItemReplaceE> itemsReplaceE, itemsReplaceEFiltered;
        ItemReplaceE CurrentReplaceE;
        List<ItemConjunction> itemsConjunctions, itemsConjunctionsFiltered;
        ItemConjunction CurrentConjunction;
        List<ItemParticle> itemsParticles, itemsParticlesFiltered;
        ItemParticle CurrentParticle;
        List<ItemInterjection> itemsInterjections, itemsInterjectionsFiltered;
        ItemInterjection CurrentInterjection;
        List<ItemPhrasePattern> itemsPhrasePattern, itemsPhrasePatternFiltered;
        ItemPhrasePattern CurrentPhrasePattern;
        List<ItemAdverb> itemsAdverbs, itemsAdverbsFiltered;
        ItemAdverb CurrentAdverb;

        List<ItemPatternNoun> itemsPatternNounFrom, itemsPatternNounTo, itemsPatternNounFromFiltered, itemsPatternNounToFiltered;
        ItemPatternNoun CurrentPatternNounFrom, CurrentPatternNounTo;

        List<ItemNoun> itemsNouns, itemsNounsFiltered;
        ItemNoun CurrentNoun;
        List<ItemPatternPronoun> itemsPatternPronounFrom, itemsPatternPronounTo, itemsPatternPronounsFromFiltered, itemsPatternPronounsToFiltered;
        ItemPatternPronoun CurrentPatternPronounFrom, CurrentPatternPronounTo;
        List<ItemPronoun> itemsPronouns, itemsPronounsFiltered;
        ItemPronoun CurrentPronoun;
        List<ItemPatternAdjective> itemsPatternAdjectiveFrom, itemsPatternAdjectiveTo;
        List<ItemPatternAdjective> itemsPatternAdjectivesFromFiltered, itemsPatternAdjectivesToFiltered;
        ItemPatternAdjective CurrentPatternAdjectiveFrom, CurrentPatternAdjectiveTo;
        List<ItemAdjective> itemsAdjectives, itemsAdjectivesFiltered;
        ItemAdjective CurrentAdjective;
         List<ItemPatternVerb> itemsPatternVerbFrom, itemsPatternVerbTo;
        List<ItemPatternVerb> itemsPatternVerbsFromFiltered, itemsPatternVerbsToFiltered;
        ItemPatternVerb CurrentPatternVerbFrom, CurrentPatternVerbTo;
        List<ItemVerb> itemsVerbs, itemsVerbsFiltered;
        ItemVerb CurrentVerb;
        List<ItemPatternNumber> itemsPatternNumberFrom, itemsPatternNumberTo;
        List<ItemPatternNumber> itemsPatternNumbersFromFiltered, itemsPatternNumbersToFiltered;
        ItemPatternNumber CurrentPatternNumberFrom, CurrentPatternNumberTo;
        List<ItemNumber> itemsNumbers, itemsNumbersFiltered;
        ItemNumber CurrentNumber;
        List<ItemPreposition> itemsPrepositions, itemsPrepositionsFiltered;
        ItemPreposition CurrentPreposition;
        #endregion

        #region vars
        long loadfilesize;
        string CurrentFile;
        bool doingJob;
        bool Edited;
        bool BlockClosing;
        const string NewProject = "<New>";
      //  const string SaveVersion = "TW v0.2";
        public static string NewestSaveVersion = "TW v"+CurrentVersion;
        public static string LoadedSaveVersion;
        public static float LoadedVersionNumber;
        int timerIndex = 0;
        readonly System.Windows.Forms.Timer timerDownloading = new System.Windows.Forms.Timer();
        #endregion

        SimpleToWithPattern simpleUINouns, simpleUIAdjective, simpleUIPronouns, simpleUINumbers, simpleUIVerbs;
        SimpleTo simpleUISimpleWord, simpleUIPhrase, simpleUISentence, simpleUISentencePart, simpleUIAdverb, simpleUIPreposition, simpleUIConjuction, simpleUIParticle, simpleUIInterjection;
        bool uiLoaded=false;
        public FormMain(string[] args) {
            // Set items
            itemsSimpleWords = new List<ItemSimpleWord>();
            itemsPhrases = new List<ItemPhrase>();
            itemsSentences = new List<ItemSentence>();
            itemsSentencePatterns = new List<ItemSentencePattern>();
            itemsSentencePatternParts = new List<ItemSentencePatternPart>();

            itemsNouns = new List<ItemNoun>();
            itemsPatternNounFrom = new List<ItemPatternNoun>();
            itemsPatternNounTo = new List<ItemPatternNoun>();

            itemsAdjectives = new List<ItemAdjective>();
            itemsPatternAdjectiveFrom = new List<ItemPatternAdjective>();
            itemsPatternAdjectiveTo = new List<ItemPatternAdjective>();

            itemsPronouns = new List<ItemPronoun>();
            itemsPatternPronounFrom = new List<ItemPatternPronoun>();
            itemsPatternPronounTo = new List<ItemPatternPronoun>();

            itemsNumbers = new List<ItemNumber>();
            itemsPatternNumberFrom = new List<ItemPatternNumber>();
            itemsPatternNumberTo = new List<ItemPatternNumber>();

            itemsVerbs = new List<ItemVerb>();
            itemsPatternVerbFrom = new List<ItemPatternVerb>();
            itemsPatternVerbTo = new List<ItemPatternVerb>();

            itemsAdverbs = new List<ItemAdverb>();
            itemsPrepositions = new List<ItemPreposition>();
            itemsConjunctions = new List<ItemConjunction>();
            itemsParticles = new List<ItemParticle>();
            itemsInterjections = new List<ItemInterjection>();
            itemsReplaceS = new List<ItemReplaceS>();
            itemsReplaceG = new List<ItemReplaceG>();
            itemsReplaceE = new List<ItemReplaceE>();
            itemsSentenceParts = new List<ItemSentencePart>();
            itemsPhrasePattern = new List<ItemPhrasePattern>();

            // Controls
            InitializeComponent();
            uiLoaded=true;

            KeyPreview = true;
            timerSecond.Start();
            simpleUINouns=AddToToolsWithPattern(splitContainerNoun);
            simpleUIAdjective=AddToToolsWithPattern(splitContainerAdjective);
            simpleUIPronouns=AddToToolsWithPattern(splitContainerPronoun);
            simpleUINumbers=AddToToolsWithPattern(splitContainerNumber);
            simpleUIVerbs=AddToToolsWithPattern(splitContainerVerb);
            simpleUIAdverb=AddToTools(splitContainerAdverb);
            simpleUIPreposition=AddToTools(splitContainerPreposition);
            simpleUIConjuction=AddToTools(splitContainerConjunction);
            simpleUIParticle=AddToTools(splitContainerParticle);
            simpleUIInterjection=AddToTools(splitContainerInterjection);

            simpleUISentencePart=AddToTools(splitContainerSentencePart);
            simpleUISentence=AddToTools(splitContainerSentence);
            simpleUIPhrase=AddToTools(splitContainerPhrase);
            simpleUISimpleWord=AddToTools(splitContainerSimpleWord);
            //NounAddTools();
            //VerbAddTools();

            // Set new project
            CurrentFile = NewProject;
            SimpleWordSetNone();
            PhraseSetNone();
            SentencePartSetNone();
            SentenceSetNone();
            SentencePatternSetNone();
            SetNoneSentencePatternPart();

            NounSetNone();
            PatternNounFromSetNone();
            PatternNounToSetNone();

            AdjectiveSetNone();
            PatternAdjectiveFromSetNone();
            PatternAdjectiveToSetNone();

            PronounSetNone();
            PatternPronounFromSetNone();
            PatternPronounToSetNone();

            NumberSetNone();
            PatternNumberFromSetNone();
            PatternNumberToSetNone();

            VerbSetNone();
            PatternVerbFromSetNone();
            PatternVerbToSetNone();

            AdverbSetNone();
            PrepositionSetNone();
            ConjunctionSetNone();
            ParticleSetNone();
            InterjectionSetNone();
            ReplaceSSetNone();
            SetNoneReplaceG();
            SetNoneReplaceE();
            PhrasePatternSetNone();

            int splitterDistance = 200;
            splitContainerParticle.SplitterDistance = splitterDistance;
            splitContainerAdjective.SplitterDistance = splitterDistance;
            splitContainerAdverb.SplitterDistance = splitterDistance;
            splitContainerConjunction.SplitterDistance = splitterDistance;
            splitContainerInterjection.SplitterDistance = splitterDistance;
            splitContainerNoun.SplitterDistance = splitterDistance;
            splitContainerNumber.SplitterDistance = splitterDistance;
            splitContainerParticle.SplitterDistance = splitterDistance;
            splitContainerPatternAdjective.SplitterDistance = splitterDistance;
            splitContainerPatternNounFrom.SplitterDistance = splitterDistance;
            splitContainerPatternNounTo.SplitterDistance = splitterDistance;
            splitContainerPatternNumberFrom.SplitterDistance = splitterDistance;
            splitContainerPatternPronounFrom.SplitterDistance = splitterDistance;
            splitContainerPatternVerbFrom.SplitterDistance = splitterDistance;
            splitContainerPhrase.SplitterDistance = splitterDistance;
            splitContainerPreposition.SplitterDistance = splitterDistance;
            splitContainerPronoun.SplitterDistance = splitterDistance;
            splitContainerReplaceE.SplitterDistance = splitterDistance;
            splitContainerReplaceG.SplitterDistance = splitterDistance;
            splitContainerReplaceS.SplitterDistance = splitterDistance;
            splitContainerSentence.SplitterDistance = splitterDistance;
            splitContainerSentencePart.SplitterDistance = splitterDistance;
            splitContainerSentencePattern.SplitterDistance = splitterDistance;
            splitContainerSentencePatternPart.SplitterDistance = splitterDistance;
            splitContainerSimpleWord.SplitterDistance = splitterDistance;
            splitContainerVerb.SplitterDistance = splitterDistance;
            splitContainerPatternAdjectiveTo.SplitterDistance = splitterDistance;
            splitContainerPatternPronounTo.SplitterDistance = splitterDistance;
            splitContainerPatternNumberTo.SplitterDistance = splitterDistance;
            splitContainerPatternVerbTo.SplitterDistance = splitterDistance;
            splitContainerPhrasePattern.SplitterDistance = splitterDistance;

            Point locListBox = new Point(6, 40);
            PatternNumberFromlistBox.Location = locListBox;
            PatternNumberTolistBox.Location = locListBox;
            PatternVerbFromlistBox.Location = locListBox;

            // tab Inside tab
            Size sizListBox = new Size(188, Height-230);
            PatternNumberFromlistBox.Size = sizListBox;
            PatternNumberTolistBox.Size = sizListBox;
            PatternVerbFromlistBox.Size = sizListBox;
            //PatternVerbTolistBox.Size = sizListBox;
            //PatternAdjectiveFromlistBox.Size = sizListBox;
            //PatternAdjectiveTolistBox.Size = sizListBox;

            // Edit UI
            ChangeCaptionText();

            EditPosButtonsSimpleWord();
            EditPosButtonsPhrase();
            EditPosButtonsSentence();
            EditPosButtonsSentencePattern();
            EditPosButtonsSentencePatternPart();
            EditPosButtonsPatternNounFrom();
            EditPosButtonsNoun();
            EditPosButtonsPatternAdjective();
            EditPosButtonsAdjective();
            EditPosButtonsPatternPronounFrom();
            EditPosButtonsPatternPronounTo();
            EditPosButtonsPronoun();
            EditPosButtonsPatternNumberFrom();
            EditPosButtonsPatternNumberTo();
            EditPosButtonsNumber();
            EditPosButtonsPatternVerbFrom();
            EditPosButtonsPatternVerbTo();
            EditPosButtonsVerb();
            EditPosButtonsAdverb();
            EditPosButtonsPreposition();
            EditPosButtonsConjunction();
            EditPosButtonsParticle();
            EditPosButtonsInterjection();
            EditPosButtonsReplaceS();
            EditPosButtonsReplaceG();
            EditPosButtonsReplaceE();
            EditPosButtonsSentencePart();
            EditPosButtonsPhrasePattern();

            if (args.Length == 1) {
                Open(args[0]);
                CurrentFile = args[0];
                ChangeCaptionText();
            }
        }

        #region Open, Save, New
        void ToolStripMenuItem1_Click(object sender, EventArgs e) {
            Open();
        }

        void UložitJakoToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveAs();
        }

        void ZavřítToolStripMenuItem_Click(object sender, EventArgs e) {
            CloseProgram();
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (BlockClosing) e.Cancel = true;

            if (Edited){
                DialogResult dr= MessageBox.Show("Chcete uložit data?","Co s neuloženými daty?", MessageBoxButtons.YesNoCancel);
                switch (dr){
                    case DialogResult.Yes:
                        if (CurrentFile == NewProject) {
                            SaveAs();
                        } else {
                            SaveCurrentFile();
                        }
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        void NovýToolStripMenuItem_Click(object sender, EventArgs e) {
            if (Edited) {
                DialogResult dr = MessageBox.Show("Nechcete uložit stávající změny?", "Před otevřením", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                    return;
            }
        }

        void UložitToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentFile == NewProject) {
                SaveAs();
            } else {
                SaveCurrentFile();
            }
        }

        void Open() {
            OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Soubor Trans Writter|*.trw|Textové soubory|*.txt|Všechny soubory|*.*",
                Title = "Otevřít soubor překladů",
                CheckFileExists = true
            };
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    Open(ofd.FileName);
                    CurrentFile = ofd.FileName;
                    ChangeCaptionText();
                }
            }
        }

        bool SaveAs() {
            SaveFileDialog sfd = new SaveFileDialog {
                Filter = "Soubor Trans Writter|*.trw|Textové soubory|*.txt|Všechny soubory|*.*",
                Title = "Uložit soubor překladů",
                FileName = "Nový soubor překladů.trw",
            };
            DialogResult dr = sfd.ShowDialog();
            if (dr == DialogResult.OK) {
                //     if (!File.Exists(sfd.FileName)) {
                Save(sfd.FileName);
                CurrentFile = sfd.FileName;
                Edited = false;
                ChangeCaptionText();
                return true;
                //    }
            }
            return false;
        }

        bool SaveCurrentFile() {
            if (CurrentFile != NewProject) {
                Save(CurrentFile);
                return true;
            }
            return false;
        }

        void CloseProgram() {
            if (!Edited)
                Environment.Exit(0);
            if (Edited) {
                DialogResult dr = MessageBox.Show("Chcete uložit stávající změny?", "Uložit?", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                    return;
            }
            if (CurrentFile == NewProject) {
                if (SaveAs()) {
                    BlockClosing = false;
                    Environment.Exit(0);
                } else {
                    BlockClosing = true;
                }
                return;
            } else {
                if (SaveCurrentFile()) {
                    BlockClosing = false;
                    Environment.Exit(0);
                } else {
                    BlockClosing = true;
                }
                return;
            }
        }

        void Open(string file) {
            FileInfo fi=new FileInfo(file);
            long now=fi.Length;
            double a=(1d-(double)loadfilesize/now)*100;
            labelFileSize.Text="Velikost souboru: "+(now/1024f).ToString("0.00")+"kB ("+(a>0 ? "+":"")+a.ToString("0.000")+"%)";
            loadfilesize=now;

            doingJob = true;
            if (Edited) {
                DialogResult dr = MessageBox.Show("Chystáte se otevřít soubor. Chcete uložit stávající změny?", "Uložit?", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)  return;
            }

            SetNewProject();

            string[] lines = File.ReadAllLines(file);
            if (lines.Length > 0) {
                // Supporting versions
                if (lines[0] == "TW v0.1" || lines[0] == "TW v1.0" || lines[0] == NewestSaveVersion) {
                    // Current version
                    LoadedSaveVersion=lines[0];
                    if (LoadedSaveVersion.Length>4){
                        string num=LoadedSaveVersion.Substring(4);
                        if (num=="1.0") LoadedVersionNumber=1;
                        else if (num=="0.1") LoadedVersionNumber=0;
                        else{
                            if (float.TryParse(num, out LoadedVersionNumber)) { } else LoadedVersionNumber=-1;
                        }
                    }
                    // Load
                    LoadLines(lines);
                } else {
                    MessageBox.Show("Soubor není překládací projekt Translator Writteru");
                }
            } else {
                MessageBox.Show("Soubor není překládací projekt Translator Writteru");
            }


            doingJob = false;
        }

        void LoadLines(string[] lines) {
            // Head
            int i = 0;
            for (; i < lines.Length; i++) {
                string line = lines[i];

                if (line == "-") break;
                if (line=="") continue;
                string subtype = line.Substring(0, 1);
                switch (subtype) {
                    // Comment info
                    case "i":
                        textBoxInfo.Text = line.Substring(1).Replace("\\n", Environment.NewLine);
                        break;

                    case "a":
                        textBoxAuthor.Text = line.Substring(1);
                        break;

                    case "d":
                        textBoxLastDate.Text = line.Substring(1);
                        break;

                    case "f":
                        textBoxLangFrom.Text = line.Substring(1);
                        break;

                    case "t":
                        textBoxLangLocation.Text = line.Substring(1);
                        break;

                    case "e":
                        textBoxSelect.Text=line.Substring(1);
                        break;

                    case "c":
                        textBoxComment.Text=line.Substring(1).Replace("\\n", Environment.NewLine);
                        break;

                    case "z":
                        textBoxZachytne.Text = line.Substring(1);
                        break;

                    case "s":
                        textBoxSpadaPod.Text = line.Substring(1);
                        break;

                    case "l":
                        textBoxLang.Text = line.Substring(1);
                        break;

                    case "g":
                        textBoxGPS.Text = line.Substring(1);
                        break;

                    case "x":
                        textBoxtypeLang.Text = line.Substring(1);
                        break;

                    case "q":
                        numericUpDownQuality.Value = int.Parse(line.Substring(1));
                        break;

                    case "o":
                        textBoxOblast.Text = line.Substring(1);
                        break;

                    case "r":
                        textBoxLocOriginal.Text = line.Substring(1);
                        break;

                    case "#":
                        break;
                }
            }

            // SentencePattern
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsSentencePatterns.Add(ItemSentencePattern.Load(line));
            }

            // SentencePatternPart
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsSentencePatternParts.Add(ItemSentencePatternPart.Load(line));
            }

            // Sentences
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsSentences.Add(ItemSentence.Load(line));
            }

            // SentencePart
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsSentenceParts.Add(ItemSentencePart.Load(line));
            }

            // Phrase
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsPhrases.Add(ItemPhrase.Load(line));
            }

            // SimpleWords
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsSimpleWords.Add(ItemSimpleWord.Load(line));
            }

            // ReplaceS
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsReplaceS.Add(ItemReplaceS.Load(line));
            }

            // ReplaceG
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsReplaceG.Add(ItemReplaceG.Load(line));
            }

            // ReplaceE
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                itemsReplaceE.Add(ItemReplaceE.Load(line));
            }

            // PatternNounFrom
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsPatternNounFrom.Add(ItemPatternNoun.Load(line));
            }

            // PatternNounTo
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")   break;
                if (line == "")  continue;
                itemsPatternNounTo.Add(ItemPatternNoun.Load(line));
            }

            // Noun
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsNouns.Add(ItemNoun.Load(line));
            }

            // PatternAdjectives
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsPatternAdjectiveFrom.Add(ItemPatternAdjective.Load(line));
            }

            // PatternAdjectivesTo
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsPatternAdjectiveTo.Add(ItemPatternAdjective.Load(line));
            }

            // Adjectives
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsAdjectives.Add(ItemAdjective.Load(line));
            }

            // PatternPronounsFrom
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsPatternPronounFrom.Add(ItemPatternPronoun.Load(line));
            }

            // PatternPronounsTo
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsPatternPronounTo.Add(ItemPatternPronoun.Load(line));
            }

            // Pronouns
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsPronouns.Add(ItemPronoun.Load(line));
            }

            // PatternNumbersFrom
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                var item=ItemPatternNumber.Load(line);
                if (item!=null) itemsPatternNumberFrom.Add(item);
            }

            // PatternNumbersTo
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                var item=ItemPatternNumber.Load(line);
                if (item!=null) itemsPatternNumberTo.Add(item);
            }

            // Numbers
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                itemsNumbers.Add(ItemNumber.Load(line));
            }

            // PatternVerbsFrom
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsPatternVerbFrom.Add(ItemPatternVerb.Load(line));
            }

            // PatternVerbsTo
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsPatternVerbTo.Add(ItemPatternVerb.Load(line));
            }

            // Verb
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                itemsVerbs.Add(ItemVerb.Load(line));
            }

            // Adverb
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")   break;
                if (line == "")  continue;
                itemsAdverbs.Add(ItemAdverb.Load(line));
            }

            // Preposition
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsPrepositions.Add(ItemPreposition.Load(line));
            }

            // Conjunction
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsConjunctions.Add(ItemConjunction.Load(line));
            }

            // Particle
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-")  break;
                if (line == "")  continue;
                itemsParticles.Add(ItemParticle.Load(line));
            }

            // Interjection
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsInterjections.Add(ItemInterjection.Load(line));
            }

            // PhrasePattern
            for (i++; i < lines.Length; i++) {
                string line = lines[i];
                if (line == "-") break;
                if (line == "")  continue;
                itemsPhrasePattern.Add(ItemPhrasePattern.Load(line));
            }

            TextBoxPhrasePatternFilter_TextChanged(null, null);
            TextBoxSentencePatternFilter_TextChanged(null, null);
            TextBoxSentenceFilter_TextChanged(null, null);
            TextBoxSimpleWordFilter_TextChanged(null, null);
            TextBoxNounFilter_TextChanged(null, null);
            TextBoxPatternNounFromFilter_TextChanged(null, null);
            TextBoxPatternNounToFilter_TextChanged(null, null);
            PatternAdjectiveFromTextBoxFilter_TextChanged(null, null);
            TextBoxAdjectiveFilter_TextChanged(null, null);
            TextBoxNumberFilter_TextChanged(null, null);
            PatternNumberFromTextBoxFilter_TextChanged(null, null);
            PatternPronounFromTextBoxFilter_TextChanged(null, null);
            TextBoxPronounFilter_TextChanged(null, null);
            PatternVerbFromTextBoxFilter_TextChanged(null, null);
            TextBoxVerbFilter_TextChanged(null, null);
            TextBoxPrepositionFilter_TextChanged(null, null);
            TextBoxConjunctionFilter_TextChanged(null, null);
            TextBoxInterjectionFilter_TextChanged(null, null);
            TextBoxParticleFilter_TextChanged(null, null);
            TextBoxAdverbFilter_TextChanged(null, null);
            TextBoxSentencePatternPartFilter_TextChanged(null, null);
            TextBoxPhraseFilter_TextChanged(null, null);
            TextBoxReplaceSFilter_TextChanged(null, null);
            TextBoxReplaceEFilter_TextChanged(null, null);
            TextBoxReplaceGFilter_TextChanged(null, null);

            PatternNumberToTextBoxFilter_TextChanged(null, null);
            PatternVerbToTextBoxFilter_TextChanged(null, null);
            PatternAdjectiveToTextBoxFilter_TextChanged(null, null);
            PatternPronounToTextBoxFilter_TextChanged(null, null);
        }

        void Save(string file) {
            Edited = false;
            ChangeCaptionText();
            string data = NewestSaveVersion + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxLangFrom.Text)) data += "f" + textBoxLangFrom.Text + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxLangLocation.Text)) data += "t" + textBoxLangLocation.Text + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxAuthor.Text)) data += "a" + textBoxAuthor.Text + Environment.NewLine;
            data += "d" + (Edited ? DateTime.Now.ToString() : (string.IsNullOrEmpty(textBoxLastDate.Text) ? DateTime.Now.ToString() : textBoxLastDate.Text)) + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxInfo.Text)) data += "i" + textBoxInfo.Text.Replace(Environment.NewLine, "\\n") + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxSelect.Text)) data += "e" + textBoxSelect.Text + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxComment.Text)) data += "c" + textBoxComment.Text.Replace(Environment.NewLine, "\\n") + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxZachytne.Text)) data += "z" + textBoxZachytne.Text + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxSpadaPod.Text)) data += "s" + textBoxSpadaPod.Text + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxLang.Text)) data += "l" + textBoxLang.Text + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxGPS.Text)) data += "g" + textBoxGPS.Text + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxtypeLang.Text)) data += "x" + textBoxtypeLang.Text + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxLocOriginal.Text)) data += "r" + textBoxLocOriginal.Text + Environment.NewLine;
            data += "q" + numericUpDownQuality.Value + Environment.NewLine;
            if (!string.IsNullOrEmpty(textBoxOblast.Text)) data += "o" + textBoxOblast.Text + Environment.NewLine;
            data += "-" + Environment.NewLine;

            //foreach (ItemSentencePattern sp in itemsSentencePatterns) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemSentence sp in itemsSentences) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemSimpleWord sp in itemsSimpleWords) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;


            //foreach (ItemPatternNoun sp in itemsPatternNounFrom) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPatternNoun sp in itemsPatternNounTo) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemNoun sp in itemsNouns) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;


            //foreach (ItemPatternAdjective sp in itemsPatternAdjectiveFrom) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemAdjective sp in itemsAdjectives) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPatternPronoun sp in itemsPatternPronounFrom) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPronoun sp in itemsPronouns) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPatternNumber sp in itemsPatternNumberFrom) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemNumber sp in itemsNumbers) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPatternVerb sp in itemsPatternVerbFrom) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemVerb sp in itemsVerbs) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPreposition sp in itemsPrepositions) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemAdverb sp in itemsAdverbs) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemConjunction sp in itemsConjunctions) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemParticle sp in itemsParticles) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemInterjection sp in itemsInterjections) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPhrase sp in itemsPhrases) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemSentencePatternPart sp in itemsSentencePatternParts) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemReplaceS sp in itemsReplaceS) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemReplaceG sp in itemsReplaceG) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemReplaceE sp in itemsReplaceE) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemSentencePart sp in itemsSentenceParts) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPatternAdjective sp in itemsPatternAdjectiveTo) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPatternPronoun sp in itemsPatternPronounTo) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPatternNumber sp in itemsPatternNumberTo) {
            //    data += sp.Save() + Environment.NewLine;
            //}
            //data += "-" + Environment.NewLine;

            //foreach (ItemPatternVerb sp in itemsPatternVerbTo) {
            //    data += sp.Save() + Environment.NewLine;
            //}

            foreach (ItemSentencePattern sp in itemsSentencePatterns) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemSentencePatternPart sp in itemsSentencePatternParts) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;


            foreach (ItemSentence sp in itemsSentences) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemSentencePart sp in itemsSentenceParts) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemPhrase sp in itemsPhrases) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemSimpleWord sp in itemsSimpleWords) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;


            foreach (ItemReplaceS sp in itemsReplaceS) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemReplaceG sp in itemsReplaceG) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemReplaceE sp in itemsReplaceE) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;


            foreach (ItemPatternNoun sp in itemsPatternNounFrom) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemPatternNoun sp in itemsPatternNounTo) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemNoun sp in itemsNouns) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;


            foreach (ItemPatternAdjective sp in itemsPatternAdjectiveFrom) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemPatternAdjective sp in itemsPatternAdjectiveTo) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemAdjective sp in itemsAdjectives) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;


            foreach (ItemPatternPronoun sp in itemsPatternPronounFrom) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemPatternPronoun sp in itemsPatternPronounTo) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemPronoun sp in itemsPronouns) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;


            foreach (ItemPatternNumber sp in itemsPatternNumberFrom) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemPatternNumber sp in itemsPatternNumberTo) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemNumber sp in itemsNumbers) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;


            foreach (ItemPatternVerb sp in itemsPatternVerbFrom) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemPatternVerb sp in itemsPatternVerbTo) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemVerb sp in itemsVerbs) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;


            foreach (ItemAdverb sp in itemsAdverbs) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemPreposition sp in itemsPrepositions) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemConjunction sp in itemsConjunctions) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemParticle sp in itemsParticles) {
                data += sp.Save() + Environment.NewLine;
            }
            data += "-" + Environment.NewLine;

            foreach (ItemInterjection sp in itemsInterjections) {
                data += sp.Save() + Environment.NewLine;
            }
            //data += "-" + Environment.NewLine;
            File.WriteAllText(file, data);

            FileInfo fi=new FileInfo(file);
            long now=fi.Length;
            double a=(1d-(double)loadfilesize/now)*100;
            labelFileSize.Text="Velikost souboru: "+(now/1024f).ToString("0.00")+"kB ("+(a>0 ? "+":"")+a.ToString("0.000")+"%)";
            loadfilesize=now;
        }

        void SetNewProject() {
            ClearSimpleWord();
            ClearPhrase();
            ClearSentences();
            ClearSentencePattern();

            PatternNounFromClear();
            PatternNounToClear();
            ClearNoun();

            PatternAdjectiveFromClear();
            PatternAdjectiveToClear();
            ClearAdjective();

            PatternPronounFromClear();
            PatternPronounToClear();
            ClearPronoun();

            PatternNumberFromClear();
            PatternNumberToClear();
            ClearNumber();

            PatternVerbFromClear();
            PatternVerbToClear();
            ClearVerb();

            ClearParticle();
            ClearAdverb();
            ClearConjunction();
            ClearInterjection();
        }
        #endregion

        #region UI
        void ChangeCaptionText() {
            if (CurrentFile == NewProject) {
                Text = Application.ProductName + " " + Application.ProductVersion + "  " + "<Nový>" + (Edited ? "*" : "");
            } else {
                FileInfo fi = new FileInfo(CurrentFile);
                Text = Application.ProductName + " " + Application.ProductVersion + "  " + fi.Name + (Edited ? "*" : "");
            }

        }

        void SplitContainerSentencePart_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsSentencePattern();
        }

        void SplitContainerSentencePattern_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsSentencePattern();
        }

        void SplitContainerSentence_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsSentence();
        }

        void SplitContainerPatternNounFrom_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternNounFrom();
        }

        void SplitContainerPatternNounTo_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternNounTo();
        }

        void SplitContainerVerb_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsVerb();
        }

        void SplitContainerPatternVerb_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternVerbFrom();
        }

        void SplitContainerNumber_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsNumber();
        }

        void SplitContainerPatternNumber_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternNumberFrom();
        }

        void SplitContainerPronoun_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPronoun();
        }

        void SplitContainerPatternPronounFrom_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternPronounFrom();
        }

        void SplitContainerPatternAdjective_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternAdjective();
        }

        void SplitContainerAdjective_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsAdjective();
        }

        void SplitContainerSimpleWord_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsSimpleWord();
        }

        void SplitContainerNoun_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsNoun();
        }

        void SplitContainerInterjection_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsInterjection();
        }

        void SplitContainerAdverb_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsAdverb();
        }

        void SplitContainerParticle_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsParticle();
        }

        void SplitContainerConjunction_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsConjunction();
        }

        void SplitContainerPhrase_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPhrase();
        }

        void SplitContainerSentencePatternPart_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsSentencePatternPart();
        }

        void SplitContainerPreposition_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPreposition();
        }

        void SplitContainerReplaceS_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsReplaceS();
        }

        void SplitContainerReplaceG_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsReplaceG();
        }

        void SplitContainerReplaceE_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsReplaceE();
        }

        void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternAdjectiveTo();
        }

        void EditPosButtonsPatternAdjectiveTo() {
            int h = splitContainerPatternAdjectiveTo.Panel1.Height - 39;
            int w = (splitContainerPatternAdjectiveTo.Panel1.Width - 16 - 6) / 2;

            buttonPatternAdjectiveToAdd.Location = new Point(buttonPatternAdjectiveToAdd.Location.X, h);
            buttonPatternAdjectiveToAdd.Width = w;
            buttonPatternAdjectiveToRemove.Location = new Point(splitContainerPatternAdjectiveTo.Panel1.Width - 6 - w, h);
            buttonPatternAdjectiveToRemove.Width = w;
        }

        void EditPosButtonsSentencePattern() {
            int h = splitContainerSentencePattern.Panel1.Height - 39;
            int w = (splitContainerSentencePattern.Panel1.Width - 16 - 6) / 2;

            buttonSentencePatternAdd.Location = new Point(buttonSentencePatternAdd.Location.X, h);
            buttonSentencePatternAdd.Width = w;
            buttonSentencePatternRemove.Location = new Point(splitContainerSentencePattern.Panel1.Width - 6 - w, h);
            buttonSentencePatternRemove.Width = w;
        }

        void EditPosButtonsSentencePart() {
            int h = splitContainerSentencePart.Panel1.Height - 39;
            int w = (splitContainerSentencePart.Panel1.Width - 16 - 6) / 2;

            buttonSentencePartAdd.Location = new Point(buttonSentencePartAdd.Location.X, h);
            buttonSentencePartAdd.Width = w;
            buttonSentencePartRemove.Location = new Point(splitContainerSentencePart.Panel1.Width - 6 - w, h);
            buttonSentencePartRemove.Width = w;
        }

        void EditPosButtonsPhrasePattern() {
            int h = splitContainerPhrasePattern.Panel1.Height - 39;
            int w = (splitContainerPhrasePattern.Panel1.Width - 16 - 6) / 2;

            buttonPhrasePatternAdd.Location = new Point(buttonPhrasePatternAdd.Location.X, h);
            buttonPhrasePatternAdd.Width = w;
            buttonPhrasePatternRemove.Location = new Point(splitContainerPhrasePattern.Panel1.Width - 6 - w, h);
            buttonPhrasePatternRemove.Width = w;
        }

        void EditPosButtonsPatternPronounFrom() {
            int h = splitContainerPatternPronounFrom.Panel1.Height - 39;
            int w = (splitContainerPatternPronounFrom.Panel1.Width - 16 - 6) / 2;

            buttonPatternPronounFromAdd.Location = new Point(buttonPatternPronounFromAdd.Location.X, h);
            buttonPatternPronounFromAdd.Width = w;
            buttonPatternPronounFromRemove.Location = new Point(splitContainerPatternPronounFrom.Panel1.Width - 6 - w, h);
            buttonPatternPronounFromRemove.Width = w;
        }

        void EditPosButtonsPatternPronounTo() {
            int h = splitContainerPatternPronounTo.Panel1.Height - 39;
            int w = (splitContainerPatternPronounTo.Panel1.Width - 16 - 6) / 2;

            buttonPatternPronounToAdd.Location = new Point(buttonPatternPronounToAdd.Location.X, h);
            buttonPatternPronounToAdd.Width = w;
            buttonPatternPronounToRemove.Location = new Point(splitContainerPatternPronounTo.Panel1.Width - 6 - w, h);
            buttonPatternPronounToRemove.Width = w;
        }

        void EditPosButtonsPatternAdjective() {
            int h = splitContainerPatternAdjective.Panel1.Height - 39;
            int w = (splitContainerPatternAdjective.Panel1.Width - 16 - 6) / 2;

            buttonPatternAdjectiveFromAdd.Location = new Point(buttonPatternAdjectiveFromAdd.Location.X, h);
            buttonPatternAdjectiveFromAdd.Width = w;
            buttonPatternAdjectiveFromRemove.Location = new Point(splitContainerPatternAdjective.Panel1.Width - 6 - w, h);
            buttonPatternAdjectiveFromRemove.Width = w;
        }

        void EditPosButtonsAdjective() {
            int h = splitContainerAdjective.Panel1.Height - 39;
            int w = (splitContainerAdjective.Panel1.Width - 16 - 6) / 2;

            buttonAdjectiveAdd.Location = new Point(buttonAdjectiveAdd.Location.X, h);
            buttonAdjectiveAdd.Width = w;
            buttonAdjectiveRemove.Location = new Point(splitContainerAdjective.Panel1.Width - 6 - w, h);
            buttonAdjectiveRemove.Width = w;
        }

        void EditPosButtonsSentence() {
            int h = splitContainerSentence.Panel1.Height - 39;
            int w = (splitContainerSentence.Panel1.Width - 16 - 6) / 2;

            buttonSentenceAdd.Location = new Point(buttonSentenceAdd.Location.X, h);
            buttonSentenceAdd.Width = w;
            buttonSentenceRemove.Width = w;
            buttonSentenceRemove.Location = new Point(splitContainerSentence.Panel1.Width - 6 - w, h);
        }

        void EditPosButtonsSimpleWord() {
            int h = splitContainerSimpleWord.Panel1.Height - 39;
            int w = (splitContainerSimpleWord.Panel1.Width - 16 - 6) / 2;

            buttonSimpleWordAdd.Location = new Point(buttonSimpleWordAdd.Location.X, h);
            buttonSimpleWordAdd.Width = w;
            buttonSimpleWordRemove.Width = w;
            buttonSimpleWordRemove.Location = new Point(splitContainerSimpleWord.Panel1.Width - 6 - w, h);
        }

        void EditPosButtonsNoun() {
            int h = splitContainerNoun.Panel1.Height - 39;
            int w = (splitContainerNoun.Panel1.Width - 16 - 6) / 2;

            buttonNounAdd.Location = new Point(buttonNounAdd.Location.X, h);
            buttonNounAdd.Width = w;
            buttonNounRemove.Width = w;
            buttonNounRemove.Location = new Point(splitContainerNoun.Panel1.Width - 6 - w, h);
        }

        void EditPosButtonsNumber() {
            int h = splitContainerNumber.Panel1.Height - 39;
            int w = (splitContainerNumber.Panel1.Width - 16 - 6) / 2;

            buttonNumberAdd.Location = new Point(buttonNumberAdd.Location.X, h);
            buttonNumberAdd.Width = w;
            buttonNumberRemove.Width = w;
            buttonNumberRemove.Location = new Point(splitContainerNumber.Panel1.Width - 6 - w, h);
        }

        void EditPosButtonsVerb() {
            int h = splitContainerVerb.Panel1.Height - 39;
            int w = (splitContainerVerb.Panel1.Width - 16 - 6) / 2;

            buttonVerbAdd.Location = new Point(buttonVerbAdd.Location.X, h);
            buttonVerbAdd.Width = w;
            buttonVerbRemove.Width = w;
            buttonVerbRemove.Location = new Point(splitContainerVerb.Panel1.Width - 6 - w, h);
        }

        void EditPosButtonsPatternNumberFrom() {
            int h = splitContainerPatternNumberFrom.Panel1.Height - 39;
            int w = (splitContainerPatternNumberFrom.Panel1.Width - 16 - 6) / 2;

            buttonPatternNumberFromAdd.Location = new Point(buttonPatternNumberFromAdd.Location.X, h);
            buttonPatternNumberFromAdd.Width = w;
            buttonPatternNumberFromRemove.Width = w;
            buttonPatternNumberFromRemove.Location = new Point(splitContainerPatternNumberFrom.Panel1.Width - 6 - w, h);
        }

        void EditPosButtonsPatternNumberTo() {
            int h = splitContainerPatternNumberTo.Panel1.Height - 39;
            int w = (splitContainerPatternNumberTo.Panel1.Width - 16 - 6) / 2;

            buttonPatternNumberToAdd.Location = new Point(buttonPatternNumberToAdd.Location.X, h);
            buttonPatternNumberToAdd.Width = w;
            buttonPatternNumberToRemove.Width = w;
            buttonPatternNumberToRemove.Location = new Point(splitContainerPatternNumberTo.Panel1.Width - 6 - w, h);
        }

        void EditPosButtonsPronoun() {
            int h = splitContainerPronoun.Panel1.Height - 39;
            int w = (splitContainerPronoun.Panel1.Width - 16 - 6) / 2;

            buttonPronounAdd.Location = new Point(buttonPronounAdd.Location.X, h);
            buttonPronounAdd.Width = w;
            buttonPronounRemove.Width = w;
            buttonPronounRemove.Location = new Point(splitContainerPronoun.Panel1.Width - 6 - w, h);
        }

        void EditPosButtonsPatternNounFrom() {
            int h = splitContainerPatternNounFrom.Panel1.Height - 39;
            int w = (splitContainerPatternNounFrom.Panel1.Width - 16 - 6) / 2;

            buttonPatternNounFromAdd.Location = new Point(buttonPatternNounFromAdd.Location.X, h);
            buttonPatternNounFromAdd.Width = w;
            buttonPatternNounFromRemove.Location = new Point(splitContainerPatternNounFrom.Panel1.Width - 6 - w, h);
            buttonPatternNounFromRemove.Width = w;
        }

        void EditPosButtonsPatternNounTo() {
            int h = splitContainerPatternNounTo.Panel1.Height - 39;
            int w = (splitContainerPatternNounTo.Panel1.Width - 16 - 6) / 2;

            buttonPatternNounToAdd.Location = new Point(buttonPatternNounToAdd.Location.X, h);
            buttonPatternNounToAdd.Width = w;
            buttonPatternNounToRemove.Location = new Point(splitContainerPatternNounTo.Panel1.Width - 6 - w, h);
            buttonPatternNounToRemove.Width = w;
        }

        void EditPosButtonsPatternVerbFrom() {
            int w = (splitContainerPatternVerbFrom.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerPatternVerbFrom.Panel1.Height - 39;

            buttonPatternVerbFromAdd.Location = new Point(buttonPatternVerbFromAdd.Location.X, h);
            buttonPatternVerbFromAdd.Width = w;
            buttonPatternVerbFromRemove.Location = new Point(splitContainerPatternVerbFrom.Panel1.Width - 6 - w, h);
            buttonPatternVerbFromRemove.Width = w;
        }

        void EditPosButtonsPatternVerbTo() {
            int w = (splitContainerPatternVerbTo.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerPatternVerbTo.Panel1.Height - 39;

            buttonPatternVerbToAdd.Location = new Point(buttonPatternVerbToAdd.Location.X, h);
            buttonPatternVerbToAdd.Width = w;
            buttonPatternVerbToRemove.Location = new Point(splitContainerPatternVerbTo.Panel1.Width - 6 - w, h);
            buttonPatternVerbToRemove.Width = w;
        }

        void EditPosButtonsPreposition() {
            int w = (splitContainerPreposition.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerPreposition.Panel1.Height - 39;

            buttonPrepositionAdd.Location = new Point(buttonPrepositionAdd.Location.X, h);
            buttonPrepositionAdd.Width = w;
            buttonPrepositionRemove.Location = new Point(splitContainerPreposition.Panel1.Width - 6 - w, h);
            buttonPrepositionRemove.Width = w;
        }

        void EditPosButtonsParticle() {
            int w = (splitContainerParticle.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerParticle.Panel1.Height - 39;

            buttonParticleAdd.Location = new Point(buttonParticleAdd.Location.X, h);
            buttonParticleAdd.Width = w;
            buttonParticleRemove.Location = new Point(splitContainerParticle.Panel1.Width - 6 - w, h);
            buttonParticleRemove.Width = w;
        }

        void EditPosButtonsAdverb() {
            int w = (splitContainerAdverb.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerAdverb.Panel1.Height - 39;

            buttonAdverbAdd.Location = new Point(buttonAdverbAdd.Location.X, h);
            buttonAdverbAdd.Width = w;
            buttonAdverbRemove.Location = new Point(splitContainerAdverb.Panel1.Width - 6 - w, h);
            buttonAdverbRemove.Width = w;
        }

        void EditPosButtonsConjunction() {
            int w = (splitContainerConjunction.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerConjunction.Panel1.Height - 39;

            buttonConjunctionAdd.Location = new Point(buttonConjunctionAdd.Location.X, h);
            buttonConjunctionAdd.Width = w;
            buttonConjunctionRemove.Location = new Point(splitContainerConjunction.Panel1.Width - 6 - w, h);
            buttonConjunctionRemove.Width = w;
        }

        void EditPosButtonsInterjection() {
            int w = (splitContainerInterjection.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerInterjection.Panel1.Height - 39;

            buttonInterjectionAdd.Location = new Point(buttonInterjectionAdd.Location.X, h);
            buttonInterjectionAdd.Width = w;
            buttonInterjectionRemove.Location = new Point(splitContainerInterjection.Panel1.Width - 6 - w, h);
            buttonInterjectionRemove.Width = w;
        }

        void EditPosButtonsPhrase() {
            int w = (splitContainerPhrase.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerPhrase.Panel1.Height - 39;

            buttonPhraseAdd.Location = new Point(buttonPhraseAdd.Location.X, h);
            buttonPhraseAdd.Width = w;
            buttonPhraseRemove.Location = new Point(splitContainerPhrase.Panel1.Width - 6 - w, h);
            buttonPhraseRemove.Width = w;
        }

        void EditPosButtonsSentencePatternPart() {
            int w = (splitContainerSentencePatternPart.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerSentencePatternPart.Panel1.Height - 39;

            buttonSentencePatternPartAdd.Location = new Point(buttonSentencePatternPartAdd.Location.X, h);
            buttonSentencePatternPartAdd.Width = w;
            buttonSentencePatternPartRemove.Location = new Point(splitContainerSentencePatternPart.Panel1.Width - 6 - w, h);
            buttonSentencePatternPartRemove.Width = w;
        }

        void EditPosButtonsReplaceS() {
            int w = (splitContainerReplaceS.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerReplaceS.Panel1.Height - 39;

            buttonReplaceSAdd.Location = new Point(buttonReplaceSAdd.Location.X, h);
            buttonReplaceSAdd.Width = w;
            buttonReplaceSRemove.Location = new Point(splitContainerReplaceS.Panel1.Width - 6 - w, h);
            buttonReplaceSRemove.Width = w;
        }

        void EditPosButtonsReplaceG() {
            int w = (splitContainerReplaceG.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerReplaceG.Panel1.Height - 39;

            buttonReplaceGAdd.Location = new Point(buttonReplaceGAdd.Location.X, h);
            buttonReplaceGAdd.Width = w;
            buttonReplaceGRemove.Location = new Point(splitContainerReplaceG.Panel1.Width - 6 - w, h);
            buttonReplaceGRemove.Width = w;
        }

        void EditPosButtonsReplaceE() {
            int w = (splitContainerReplaceE.Panel1.Width - 16 - 6) / 2;
            int h = splitContainerReplaceE.Panel1.Height - 39;

            buttonReplaceEAdd.Location = new Point(buttonReplaceEAdd.Location.X, h);
            buttonReplaceEAdd.Width = w;
            buttonReplaceERemove.Location = new Point(splitContainerReplaceE.Panel1.Width - 6 - w, h);
            buttonReplaceERemove.Width = w;
        }

        void TextBoxLangFrom_TextChanged(object sender, EventArgs e) {
            if (doingJob)
                return;
            Edited = true;
            ChangeCaptionText();
        }

        void TextBoxLangTo_TextChanged(object sender, EventArgs e) {
            if (doingJob)
                return;
            Edited = true;
            ChangeCaptionText();
        }

        void TextBoxAuthor_TextChanged(object sender, EventArgs e) {
            if (doingJob)
                return;
            Edited = true;
            ChangeCaptionText();
        }

        void TextBoxInfo_TextChanged(object sender, EventArgs e) {
            if (doingJob)
                return;
            Edited = true;
            ChangeCaptionText();
        }
        #endregion

        void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode == Keys.S) {
                if (CurrentFile=="<New>")SaveAs();
                else Save(CurrentFile);
                Debug.WriteLine("Saved");
                e.SuppressKeyPress = true;
                return;
            }
            if (e.Control && e.KeyCode == Keys.O) {
                Open();
                Debug.WriteLine("Opened");
                e.SuppressKeyPress = true;
                return;
            }
        }

        void TextBoxPatternAdjectiveName_TextChanged(object sender, EventArgs e) {
            if (doingJob)
                return;
            Edited = true;
            ChangeCaptionText();
        }

        void ComboBoxPatternPronounType_SelectedIndexChanged(object sender, EventArgs e) {
            if (CurrentPatternPronounFrom != null) {
                CurrentPatternPronounFrom.Type = (PronounType)comboBoxPatternPronounFromType.SelectedIndex;
                ChangeTypePatternPronounFrom(CurrentPatternPronounFrom?.Type);
            }
        }

        void ChangeTypePatternPronounFrom(PronounType? type) {
            if (!type.HasValue)
                type = PronounType.Unknown;

            switch (type) {
                case PronounType.Unknown:
                    textBoxPatternPronounFromMuzJ1.Visible = false;
                    textBoxPatternPronounFromMuzJ2.Visible = false;
                    textBoxPatternPronounFromMuzJ3.Visible = false;
                    textBoxPatternPronounFromMuzJ4.Visible = false;
                    textBoxPatternPronounFromMuzJ5.Visible = false;
                    textBoxPatternPronounFromMuzJ6.Visible = false;
                    textBoxPatternPronounFromMuzJ7.Visible = false;
                    labelPatternPronounFromMuz.Visible = false;
                    labelPatternPronounFromMuzFall.Visible = false;
                    labelPatternPronounFromMuzMultiple.Visible = false;


                    labelPatternPronounFromZen.Visible = false;
                    labelPatternPronounFromZenFall.Visible = false;
                    labelPatternPronounFromZenSingle.Visible = false;
                    labelPatternPronounFromZenMultiple.Visible = false;
                    tableLayoutPanelPatternPronounFromZen.Visible = false;


                    labelPatternPronounFromMun.Visible = false;
                    labelPatternPronounFromMunFall.Visible = false;
                    labelPatternPronounFromMunSingle.Visible = false;
                    labelPatternPronounFromMunMultiple.Visible = false;
                    tableLayoutPanelPatternPronounFromMun.Visible = false;


                    labelPatternPronounFromStr.Visible = false;
                    labelPatternPronounFromStrFall.Visible = false;
                    labelPatternPronounFromStrSingle.Visible = false;
                    labelPatternPronounFromStrMultiple.Visible = false;
                    tableLayoutPanelPatternPronounStr.Visible = false;

                    tableLayoutPanelPatternPronounFromMuz.Visible = false;
                    return;

                case PronounType.NoDeklination:
                    textBoxPatternPronounFromMuzJ1.Visible = true;
                    textBoxPatternPronounFromMuzJ2.Visible = false;
                    textBoxPatternPronounFromMuzJ3.Visible = false;
                    textBoxPatternPronounFromMuzJ4.Visible = false;
                    textBoxPatternPronounFromMuzJ5.Visible = false;
                    textBoxPatternPronounFromMuzJ6.Visible = false;
                    textBoxPatternPronounFromMuzJ7.Visible = false;
                    labelPatternPronounFromMuz.Visible = false;
                    labelPatternPronounFromMuzFall.Visible = false;
                    labelPatternPronounFromMuzMultiple.Visible = false;

                    labelPatternPronounFromZen.Visible = false;
                    labelPatternPronounFromZenFall.Visible = false;
                    labelPatternPronounFromZenSingle.Visible = false;
                    labelPatternPronounFromZenMultiple.Visible = false;
                    tableLayoutPanelPatternPronounFromZen.Visible = false;


                    labelPatternPronounFromMun.Visible = false;
                    labelPatternPronounFromMunFall.Visible = false;
                    labelPatternPronounFromMunSingle.Visible = false;
                    labelPatternPronounFromMunMultiple.Visible = false;
                    tableLayoutPanelPatternPronounFromMun.Visible = false;


                    labelPatternPronounFromStr.Visible = false;
                    labelPatternPronounFromStrFall.Visible = false;
                    labelPatternPronounFromStrSingle.Visible = false;
                    labelPatternPronounFromStrMultiple.Visible = false;
                    tableLayoutPanelPatternPronounStr.Visible = false;

                    textBoxPatternPronounFromMuzM1.Visible = false;
                    textBoxPatternPronounFromMuzM2.Visible = false;
                    textBoxPatternPronounFromMuzM3.Visible = false;
                    textBoxPatternPronounFromMuzM4.Visible = false;
                    textBoxPatternPronounFromMuzM5.Visible = false;
                    textBoxPatternPronounFromMuzM6.Visible = false;
                    textBoxPatternPronounFromMuzM7.Visible = false;

                    tableLayoutPanelPatternPronounFromMuz.Visible = false;
                    return;

                case PronounType.DeklinationOnlySingle:
                    textBoxPatternPronounFromMuzJ1.Visible = true;
                    textBoxPatternPronounFromMuzJ2.Visible = true;
                    textBoxPatternPronounFromMuzJ3.Visible = true;
                    textBoxPatternPronounFromMuzJ4.Visible = true;
                    textBoxPatternPronounFromMuzJ5.Visible = true;
                    textBoxPatternPronounFromMuzJ6.Visible = true;
                    textBoxPatternPronounFromMuzJ7.Visible = true;
                    labelPatternPronounFromMuz.Visible = false;
                    labelPatternPronounFromMuzFall.Visible = true;
                    labelPatternPronounFromMuzSingle.Visible = false;
                    labelPatternPronounFromMuzMultiple.Visible = false;


                    labelPatternPronounFromZen.Visible = false;
                    labelPatternPronounFromZenFall.Visible = false;
                    labelPatternPronounFromZenSingle.Visible = false;
                    labelPatternPronounFromZenMultiple.Visible = false;
                    tableLayoutPanelPatternPronounFromZen.Visible = false;


                    labelPatternPronounFromMun.Visible = false;
                    labelPatternPronounFromMunFall.Visible = false;
                    labelPatternPronounFromMunSingle.Visible = false;
                    labelPatternPronounFromMunMultiple.Visible = false;
                    tableLayoutPanelPatternPronounFromMun.Visible = false;


                    labelPatternPronounFromStr.Visible = false;
                    labelPatternPronounFromStrFall.Visible = false;
                    labelPatternPronounFromStrSingle.Visible = false;
                    labelPatternPronounFromStrMultiple.Visible = false;
                    tableLayoutPanelPatternPronounStr.Visible = false;


                    textBoxPatternPronounFromMuzM1.Visible = false;
                    textBoxPatternPronounFromMuzM2.Visible = false;
                    textBoxPatternPronounFromMuzM3.Visible = false;
                    textBoxPatternPronounFromMuzM4.Visible = false;
                    textBoxPatternPronounFromMuzM5.Visible = false;
                    textBoxPatternPronounFromMuzM6.Visible = false;
                    textBoxPatternPronounFromMuzM7.Visible = false;


                    tableLayoutPanelPatternPronounFromMuz.Visible = true;
                    return;

                case PronounType.Deklination:
                    textBoxPatternPronounFromMuzJ1.Visible = true;
                    textBoxPatternPronounFromMuzJ2.Visible = true;
                    textBoxPatternPronounFromMuzJ3.Visible = true;
                    textBoxPatternPronounFromMuzJ4.Visible = true;
                    textBoxPatternPronounFromMuzJ5.Visible = true;
                    textBoxPatternPronounFromMuzJ6.Visible = true;
                    textBoxPatternPronounFromMuzJ7.Visible = true;
                    labelPatternPronounFromMuz.Visible = false;
                    labelPatternPronounFromMuzFall.Visible = true;
                    labelPatternPronounFromMuzSingle.Visible = true;
                    labelPatternPronounFromMuzMultiple.Visible = true;


                    labelPatternPronounFromZen.Visible = false;
                    labelPatternPronounFromZenFall.Visible = false;
                    labelPatternPronounFromZenSingle.Visible = false;
                    labelPatternPronounFromZenMultiple.Visible = false;
                    tableLayoutPanelPatternPronounFromZen.Visible = false;


                    labelPatternPronounFromMun.Visible = false;
                    labelPatternPronounFromMunFall.Visible = false;
                    labelPatternPronounFromMunSingle.Visible = false;
                    labelPatternPronounFromMunMultiple.Visible = false;
                    tableLayoutPanelPatternPronounFromMun.Visible = false;


                    labelPatternPronounFromStr.Visible = false;
                    labelPatternPronounFromStrFall.Visible = false;
                    labelPatternPronounFromStrSingle.Visible = false;
                    labelPatternPronounFromStrMultiple.Visible = false;
                    tableLayoutPanelPatternPronounStr.Visible = false;


                    textBoxPatternPronounFromMuzM1.Visible = true;
                    textBoxPatternPronounFromMuzM2.Visible = true;
                    textBoxPatternPronounFromMuzM3.Visible = true;
                    textBoxPatternPronounFromMuzM4.Visible = true;
                    textBoxPatternPronounFromMuzM5.Visible = true;
                    textBoxPatternPronounFromMuzM6.Visible = true;
                    textBoxPatternPronounFromMuzM7.Visible = true;
                    tableLayoutPanelPatternPronounFromMuz.Visible = true;
                    return;

                case PronounType.DeklinationWithGender:
                    textBoxPatternPronounFromMuzJ1.Visible = true;
                    textBoxPatternPronounFromMuzJ2.Visible = true;
                    textBoxPatternPronounFromMuzJ3.Visible = true;
                    textBoxPatternPronounFromMuzJ4.Visible = true;
                    textBoxPatternPronounFromMuzJ5.Visible = true;
                    textBoxPatternPronounFromMuzJ6.Visible = true;
                    textBoxPatternPronounFromMuzJ7.Visible = true;
                    labelPatternPronounFromMuz.Visible = true;
                    labelPatternPronounFromMuzFall.Visible = true;
                    labelPatternPronounFromMuzSingle.Visible = true;
                    labelPatternPronounFromMuzMultiple.Visible = true;


                    labelPatternPronounFromZen.Visible = true;
                    labelPatternPronounFromZenFall.Visible = true;
                    labelPatternPronounFromZenSingle.Visible = true;
                    labelPatternPronounFromZenMultiple.Visible = true;
                    tableLayoutPanelPatternPronounFromZen.Visible = true;


                    labelPatternPronounFromMun.Visible = true;
                    labelPatternPronounFromMunFall.Visible = true;
                    labelPatternPronounFromMunSingle.Visible = true;
                    labelPatternPronounFromMunMultiple.Visible = true;
                    tableLayoutPanelPatternPronounFromMun.Visible = true;


                    labelPatternPronounFromStr.Visible = true;
                    labelPatternPronounFromStrFall.Visible = true;
                    labelPatternPronounFromStrSingle.Visible = true;
                    labelPatternPronounFromStrMultiple.Visible = true;
                    tableLayoutPanelPatternPronounStr.Visible = true;

                    textBoxPatternPronounFromMuzM1.Visible = true;
                    textBoxPatternPronounFromMuzM2.Visible = true;
                    textBoxPatternPronounFromMuzM3.Visible = true;
                    textBoxPatternPronounFromMuzM4.Visible = true;
                    textBoxPatternPronounFromMuzM5.Visible = true;
                    textBoxPatternPronounFromMuzM6.Visible = true;
                    textBoxPatternPronounFromMuzM7.Visible = true;
                    tableLayoutPanelPatternPronounFromMuz.Visible = true;
                    return;
            }
        }

        void ChangeTypePatternPronounTo(PronounType? type) {
            if (!type.HasValue)
                type = PronounType.Unknown;

            switch (type) {
                case PronounType.Unknown:
                    textBoxPatternPronounToMuzJ1.Visible = false;
                    textBoxPatternPronounToMuzJ2.Visible = false;
                    textBoxPatternPronounToMuzJ3.Visible = false;
                    textBoxPatternPronounToMuzJ4.Visible = false;
                    textBoxPatternPronounToMuzJ5.Visible = false;
                    textBoxPatternPronounToMuzJ6.Visible = false;
                    textBoxPatternPronounToMuzJ7.Visible = false;
                    labelPatternPronounToMuz.Visible = false;
                    labelPatternPronounToMuzFall.Visible = false;
                    labelPatternPronounToMuzMultiple.Visible = false;


                    labelPatternPronounToZen.Visible = false;
                    labelPatternPronounToZenFall.Visible = false;
                    labelPatternPronounToZenSingle.Visible = false;
                    labelPatternPronounToZenMultiple.Visible = false;
                    tableLayoutPanelPatternPronounToZen.Visible = false;


                    labelPatternPronounToMun.Visible = false;
                    labelPatternPronounToMunFall.Visible = false;
                    labelPatternPronounToMunSingle.Visible = false;
                    labelPatternPronounToMunMultiple.Visible = false;
                    tableLayoutPanelPatternPronounToMun.Visible = false;


                    labelPatternPronounToStr.Visible = false;
                    labelPatternPronounToStrFall.Visible = false;
                    labelPatternPronounToStrSingle.Visible = false;
                    labelPatternPronounToStrMultiple.Visible = false;
                    tableLayoutPanelPatternPronounStr.Visible = false;

                    tableLayoutPanelPatternPronounToMuz.Visible = false;
                    return;

                case PronounType.NoDeklination:
                    textBoxPatternPronounToMuzJ1.Visible = true;
                    textBoxPatternPronounToMuzJ2.Visible = false;
                    textBoxPatternPronounToMuzJ3.Visible = false;
                    textBoxPatternPronounToMuzJ4.Visible = false;
                    textBoxPatternPronounToMuzJ5.Visible = false;
                    textBoxPatternPronounToMuzJ6.Visible = false;
                    textBoxPatternPronounToMuzJ7.Visible = false;
                    labelPatternPronounToMuz.Visible = false;
                    labelPatternPronounToMuzFall.Visible = false;
                    labelPatternPronounToMuzMultiple.Visible = false;

                    labelPatternPronounToZen.Visible = false;
                    labelPatternPronounToZenFall.Visible = false;
                    labelPatternPronounToZenSingle.Visible = false;
                    labelPatternPronounToZenMultiple.Visible = false;
                    tableLayoutPanelPatternPronounToZen.Visible = false;


                    labelPatternPronounToMun.Visible = false;
                    labelPatternPronounToMunFall.Visible = false;
                    labelPatternPronounToMunSingle.Visible = false;
                    labelPatternPronounToMunMultiple.Visible = false;
                    tableLayoutPanelPatternPronounToMun.Visible = false;


                    labelPatternPronounToStr.Visible = false;
                    labelPatternPronounToStrFall.Visible = false;
                    labelPatternPronounToStrSingle.Visible = false;
                    labelPatternPronounToStrMultiple.Visible = false;
                    tableLayoutPanelPatternPronounStr.Visible = false;

                    textBoxPatternPronounToMuzM1.Visible = false;
                    textBoxPatternPronounToMuzM2.Visible = false;
                    textBoxPatternPronounToMuzM3.Visible = false;
                    textBoxPatternPronounToMuzM4.Visible = false;
                    textBoxPatternPronounToMuzM5.Visible = false;
                    textBoxPatternPronounToMuzM6.Visible = false;
                    textBoxPatternPronounToMuzM7.Visible = false;

                    tableLayoutPanelPatternPronounToMuz.Visible = false;
                    return;

                case PronounType.DeklinationOnlySingle:
                    textBoxPatternPronounToMuzJ1.Visible = true;
                    textBoxPatternPronounToMuzJ2.Visible = true;
                    textBoxPatternPronounToMuzJ3.Visible = true;
                    textBoxPatternPronounToMuzJ4.Visible = true;
                    textBoxPatternPronounToMuzJ5.Visible = true;
                    textBoxPatternPronounToMuzJ6.Visible = true;
                    textBoxPatternPronounToMuzJ7.Visible = true;
                    labelPatternPronounToMuz.Visible = false;
                    labelPatternPronounToMuzFall.Visible = true;
                    labelPatternPronounToMuzSingle.Visible = false;
                    labelPatternPronounToMuzMultiple.Visible = false;


                    labelPatternPronounToZen.Visible = false;
                    labelPatternPronounToZenFall.Visible = false;
                    labelPatternPronounToZenSingle.Visible = false;
                    labelPatternPronounToZenMultiple.Visible = false;
                    tableLayoutPanelPatternPronounToZen.Visible = false;


                    labelPatternPronounToMun.Visible = false;
                    labelPatternPronounToMunFall.Visible = false;
                    labelPatternPronounToMunSingle.Visible = false;
                    labelPatternPronounToMunMultiple.Visible = false;
                    tableLayoutPanelPatternPronounToMun.Visible = false;


                    labelPatternPronounToStr.Visible = false;
                    labelPatternPronounToStrFall.Visible = false;
                    labelPatternPronounToStrSingle.Visible = false;
                    labelPatternPronounToStrMultiple.Visible = false;
                    tableLayoutPanelPatternPronounStr.Visible = false;


                    textBoxPatternPronounToMuzM1.Visible = false;
                    textBoxPatternPronounToMuzM2.Visible = false;
                    textBoxPatternPronounToMuzM3.Visible = false;
                    textBoxPatternPronounToMuzM4.Visible = false;
                    textBoxPatternPronounToMuzM5.Visible = false;
                    textBoxPatternPronounToMuzM6.Visible = false;
                    textBoxPatternPronounToMuzM7.Visible = false;


                    tableLayoutPanelPatternPronounToMuz.Visible = true;
                    return;

                case PronounType.Deklination:
                    textBoxPatternPronounToMuzJ1.Visible = true;
                    textBoxPatternPronounToMuzJ2.Visible = true;
                    textBoxPatternPronounToMuzJ3.Visible = true;
                    textBoxPatternPronounToMuzJ4.Visible = true;
                    textBoxPatternPronounToMuzJ5.Visible = true;
                    textBoxPatternPronounToMuzJ6.Visible = true;
                    textBoxPatternPronounToMuzJ7.Visible = true;
                    labelPatternPronounToMuz.Visible = false;
                    labelPatternPronounToMuzFall.Visible = true;
                    labelPatternPronounToMuzSingle.Visible = true;
                    labelPatternPronounToMuzMultiple.Visible = true;


                    labelPatternPronounToZen.Visible = false;
                    labelPatternPronounToZenFall.Visible = false;
                    labelPatternPronounToZenSingle.Visible = false;
                    labelPatternPronounToZenMultiple.Visible = false;
                    tableLayoutPanelPatternPronounToZen.Visible = false;


                    labelPatternPronounToMun.Visible = false;
                    labelPatternPronounToMunFall.Visible = false;
                    labelPatternPronounToMunSingle.Visible = false;
                    labelPatternPronounToMunMultiple.Visible = false;
                    tableLayoutPanelPatternPronounToMun.Visible = false;


                    labelPatternPronounToStr.Visible = false;
                    labelPatternPronounToStrFall.Visible = false;
                    labelPatternPronounToStrSingle.Visible = false;
                    labelPatternPronounToStrMultiple.Visible = false;
                    tableLayoutPanelPatternPronounStr.Visible = false;


                    textBoxPatternPronounToMuzM1.Visible = true;
                    textBoxPatternPronounToMuzM2.Visible = true;
                    textBoxPatternPronounToMuzM3.Visible = true;
                    textBoxPatternPronounToMuzM4.Visible = true;
                    textBoxPatternPronounToMuzM5.Visible = true;
                    textBoxPatternPronounToMuzM6.Visible = true;
                    textBoxPatternPronounToMuzM7.Visible = true;
                    tableLayoutPanelPatternPronounToMuz.Visible = true;
                    return;

                case PronounType.DeklinationWithGender:
                    textBoxPatternPronounToMuzJ1.Visible = true;
                    textBoxPatternPronounToMuzJ2.Visible = true;
                    textBoxPatternPronounToMuzJ3.Visible = true;
                    textBoxPatternPronounToMuzJ4.Visible = true;
                    textBoxPatternPronounToMuzJ5.Visible = true;
                    textBoxPatternPronounToMuzJ6.Visible = true;
                    textBoxPatternPronounToMuzJ7.Visible = true;
                    labelPatternPronounToMuz.Visible = true;
                    labelPatternPronounToMuzFall.Visible = true;
                    labelPatternPronounToMuzSingle.Visible = true;
                    labelPatternPronounToMuzMultiple.Visible = true;


                    labelPatternPronounToZen.Visible = true;
                    labelPatternPronounToZenFall.Visible = true;
                    labelPatternPronounToZenSingle.Visible = true;
                    labelPatternPronounToZenMultiple.Visible = true;
                    tableLayoutPanelPatternPronounToZen.Visible = true;


                    labelPatternPronounToMun.Visible = true;
                    labelPatternPronounToMunFall.Visible = true;
                    labelPatternPronounToMunSingle.Visible = true;
                    labelPatternPronounToMunMultiple.Visible = true;
                    tableLayoutPanelPatternPronounToMun.Visible = true;


                    labelPatternPronounToStr.Visible = true;
                    labelPatternPronounToStrFall.Visible = true;
                    labelPatternPronounToStrSingle.Visible = true;
                    labelPatternPronounToStrMultiple.Visible = true;
                    tableLayoutPanelPatternPronounStr.Visible = true;

                    textBoxPatternPronounToMuzM1.Visible = true;
                    textBoxPatternPronounToMuzM2.Visible = true;
                    textBoxPatternPronounToMuzM3.Visible = true;
                    textBoxPatternPronounToMuzM4.Visible = true;
                    textBoxPatternPronounToMuzM5.Visible = true;
                    textBoxPatternPronounToMuzM6.Visible = true;
                    textBoxPatternPronounToMuzM7.Visible = true;
                    tableLayoutPanelPatternPronounToMuz.Visible = true;
                    return;
            }
        }

        void ButtonWikidirectonaryAdjective_Click(object sender, EventArgs e) {
            WebClient client = new WebClient();
            var data = client.DownloadData("https://cs.wiktionary.org/wiki/" + Uri.EscapeDataString(textBoxPatternAdjectiveFromName.Text));
            string strz = Encoding.UTF8.GetString(data);
            if (strz.Contains("<table class=\"deklinace adjektivum\">")) {
                int start = strz.IndexOf("<table class=\"deklinace adjektivum\">");
                string _table = strz.Substring(start);
                string table = _table.Substring(0, _table.IndexOf("</table>"));
                List<string> tvary = new System.Collections.Generic.List<string>();
                Console.WriteLine("|" + table + "|");
                bool ignore = false;
                char[] arr = table.ToCharArray();
                string ac = "";
                for (int i = 0; i < arr.Length; i++) {
                    char ch = arr[i];
                    if (ch == '<') {
                        ignore = true;
                        if (ac != "" && ac != "\n" && ac != "\n\n") {
                            tvary.Add(Uri.UnescapeDataString(ac).Replace("\n", ""));
                            ac = "";
                        }
                    }

                    if (ch == '>') {
                        ignore = false;
                    } else if (!ignore) {
                        ac += ch;
                    }
                }
                if (!ignore) {
                    if (ac != "" && ac != "\n" && ac != "\n\n") {
                        tvary.Add(Uri.UnescapeDataString(ac).Replace("\n", ""));
                    }
                }
                if (tvary[0] == ""
                && tvary[1] == "Číslo"
                && tvary[2] == "singulár"
                && tvary[3] == "plurál"
                && tvary[4] == "Rod"
                && tvary[5] == "mužský"
                && tvary[6] == "životný"
                && tvary[7] == "mužský"
                && tvary[8] == "neživotný"
                && tvary[9] == "ženský"
                && tvary[10] == "střední"
                && tvary[11] == "mužský"
                && tvary[12] == "životný"
                && tvary[13] == "mužský") {
                    bool str = false;
                    List<string> arr2 = new List<string>();
                    for (int i = 0; i < tvary.Count; i++) {
                        var tvar = tvary[i];
                        if (tvar == "nominativ") { str = true; continue; }
                        if (str) {
                            if (tvar == "")
                                continue;
                            if (tvar == "nominativ")
                                continue;
                            if (tvar == "genitiv")
                                continue;
                            if (tvar == "dativ")
                                continue;
                            if (tvar == "lokativ")
                                continue;
                            if (tvar == "akuzativ")
                                continue;
                            if (tvar == "vokativ")
                                continue;
                            if (tvar == "instrumentál")
                                continue;
                            if (tvar == "lokál")
                                continue;
                            arr2.Add(tvar);
                        }
                    }
                    if (arr2.Count != 7 * 2 * 4) {
                        return;
                    }

                    int total = -1;
                    for (int step = 1; step < arr2[0].Length; step++) {
                        string same = arr2[0].Substring(0, step);
                        for (int i = 0; i < arr2.Count; i++) {
                            var a = arr2[i];
                            if (step <= a.Length) {
                                if (same == a.Substring(0, step)) {
                                    //ok
                                } else {
                                    total = step - 1;
                                    break;
                                }
                            } else { total = step - 1; break; }
                        }
                    }

                    for (int i = 0; i < arr2.Count; i++) {
                        arr2[i] = arr2[i].Substring(total);
                    }

                    textBoxPatternAdjectiveFromMuzJ1.Text = arr2[0];
                    textBoxPatternAdjectiveFromMuzJ2.Text = arr2[8];
                    textBoxPatternAdjectiveFromMuzJ3.Text = arr2[16];
                    textBoxPatternAdjectiveFromMuzJ4.Text = arr2[24];
                    textBoxPatternAdjectiveFromMuzJ5.Text = arr2[32];
                    textBoxPatternAdjectiveFromMuzJ6.Text = arr2[40];
                    textBoxPatternAdjectiveFromMuzJ7.Text = arr2[48];
                    textBoxPatternAdjectiveFromMuzJN.Text = "-";
                    textBoxPatternAdjectiveFromMuzJA.Text = "-";

                    textBoxPatternAdjectiveFromMuzM1.Text = arr2[4];
                    textBoxPatternAdjectiveFromMuzM2.Text = arr2[12];
                    textBoxPatternAdjectiveFromMuzM3.Text = arr2[20];
                    textBoxPatternAdjectiveFromMuzM4.Text = arr2[28];
                    textBoxPatternAdjectiveFromMuzM5.Text = arr2[36];
                    textBoxPatternAdjectiveFromMuzM6.Text = arr2[44];
                    textBoxPatternAdjectiveFromMuzM7.Text = arr2[52];
                    textBoxPatternAdjectiveFromMuzMN.Text = "-";
                    textBoxPatternAdjectiveFromMuzMA.Text = "-";


                    textBoxPatternAdjectiveFromMunJ1.Text = arr2[1];
                    textBoxPatternAdjectiveFromMunJ2.Text = arr2[9];
                    textBoxPatternAdjectiveFromMunJ3.Text = arr2[17];
                    textBoxPatternAdjectiveFromMunJ4.Text = arr2[25];
                    textBoxPatternAdjectiveFromMunJ5.Text = arr2[33];
                    textBoxPatternAdjectiveFromMunJ6.Text = arr2[41];
                    textBoxPatternAdjectiveFromMunJ7.Text = arr2[49];
                    textBoxPatternAdjectiveFromMunJN.Text = "-";
                    textBoxPatternAdjectiveFromMunJA.Text = "-";

                    textBoxPatternAdjectiveFromMunM1.Text = arr2[5];
                    textBoxPatternAdjectiveFromMunM2.Text = arr2[13];
                    textBoxPatternAdjectiveFromMunM3.Text = arr2[21];
                    textBoxPatternAdjectiveFromMunM4.Text = arr2[29];
                    textBoxPatternAdjectiveFromMunM5.Text = arr2[37];
                    textBoxPatternAdjectiveFromMunM6.Text = arr2[45];
                    textBoxPatternAdjectiveFromMunM7.Text = arr2[53];
                    textBoxPatternAdjectiveFromMunMN.Text = "-";
                    textBoxPatternAdjectiveFromMunMA.Text = "-";


                    textBoxPatternAdjectiveFromZenJ1.Text = arr2[2];
                    textBoxPatternAdjectiveFromZenJ2.Text = arr2[10];
                    textBoxPatternAdjectiveFromZenJ3.Text = arr2[18];
                    textBoxPatternAdjectiveFromZenJ4.Text = arr2[26];
                    textBoxPatternAdjectiveFromZenJ5.Text = arr2[34];
                    textBoxPatternAdjectiveFromZenJ6.Text = arr2[42];
                    textBoxPatternAdjectiveFromZenJ7.Text = arr2[50];
                    textBoxPatternAdjectiveFromZenJN.Text = "-";
                    textBoxPatternAdjectiveFromZenJA.Text = "-";

                    textBoxPatternAdjectiveFromZenM1.Text = arr2[6];
                    textBoxPatternAdjectiveFromZenM2.Text = arr2[14];
                    textBoxPatternAdjectiveFromZenM3.Text = arr2[22];
                    textBoxPatternAdjectiveFromZenM4.Text = arr2[30];
                    textBoxPatternAdjectiveFromZenM5.Text = arr2[38];
                    textBoxPatternAdjectiveFromZenM6.Text = arr2[46];
                    textBoxPatternAdjectiveFromZenM7.Text = arr2[54];
                    textBoxPatternAdjectiveFromZenMN.Text = "-";
                    textBoxPatternAdjectiveFromZenMA.Text = "-";


                    textBoxPatternAdjectiveFromStrJ1.Text = arr2[3];
                    textBoxPatternAdjectiveFromStrJ2.Text = arr2[11];
                    textBoxPatternAdjectiveFromStrJ3.Text = arr2[19];
                    textBoxPatternAdjectiveFromStrJ4.Text = arr2[27];
                    textBoxPatternAdjectiveFromStrJ5.Text = arr2[35];
                    textBoxPatternAdjectiveFromStrJ6.Text = arr2[43];
                    textBoxPatternAdjectiveFromStrJ7.Text = arr2[51];
                    textBoxPatternAdjectiveFromStrJN.Text = "-";
                    textBoxPatternAdjectiveFromStrJA.Text = "-";

                    textBoxPatternAdjectiveFromStrM1.Text = arr2[7];
                    textBoxPatternAdjectiveFromStrM2.Text = arr2[15];
                    textBoxPatternAdjectiveFromStrM3.Text = arr2[23];
                    textBoxPatternAdjectiveFromStrM4.Text = arr2[31];
                    textBoxPatternAdjectiveFromStrM5.Text = arr2[39];
                    textBoxPatternAdjectiveFromStrM6.Text = arr2[47];
                    textBoxPatternAdjectiveFromStrM7.Text = arr2[55];
                    textBoxPatternAdjectiveFromStrMN.Text = "-";
                    textBoxPatternAdjectiveFromStrMA.Text = "-";
                }
            }
        }

        void DuplicateToolStripMenuItem_Click_1(object sender, EventArgs e) {
            itemsAdjectives.Add(CurrentAdjective.Duplicate());
        }

        void ToolStripMenuItemPatternAdjectiveDup_Click(object sender, EventArgs e) {
            doingJob = true;
            PatternAdjectiveFromSaveCurrent();
            itemsPatternAdjectiveFrom.Add(CurrentPatternAdjectiveFrom.Duplicate());
            PatternAdjectiveFromSetCurrent();
            PatternAdjectiveFromSetListBox();
            doingJob = false;
        }

        void ZWikidirectonaryToolStripMenuItem_Click(object sender, EventArgs e) {
            string name = GetString("", "Název adjektiva");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;
            #if !DEBUG
            try{
            #endif
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    List<Table> tables=new List<Table>();
                    Computation.FindTableInHTML(html, "deklinace adjektivum", ref tables);


                   // bool future=false;
                    if (tables.Count>=1) {
                        Table table=tables[0];
                        if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
                            ItemPatternAdjective pattern = new ItemPatternAdjective {
                                Name = name,
                               // TypeShow = VerbTypeShow.Unknown,
                                Feminine=new string[18],
                                Middle=new string[18],
                                MasculineAnimate=new string[18],
                                MasculineInanimate=new string[18],
                            };

                            for (int r=0; r<7; r++) pattern.MasculineAnimate[r]=table.Rows[2+r].Cells[1+0].Text;
                            for (int r=0; r<7; r++) pattern.MasculineInanimate[r/*+7*2*/]=table.Rows[2+r].Cells[1+1].Text;
                            for (int r=0; r<7; r++) pattern.Feminine[r/*+7*4*/]=table.Rows[2+r].Cells[1+2].Text;
                            for (int r=0; r<7; r++) pattern.Middle[r/*+7*6*/]=table.Rows[2+r].Cells[1+3].Text;

                            for (int r=0; r<7; r++) pattern.MasculineAnimate[r+7+2/**1*/]=table.Rows[2+r].Cells[1+4].Text;
                            for (int r=0; r<7; r++) pattern.MasculineInanimate[r+7+2/**3*/]=table.Rows[2+r].Cells[1+5].Text;
                            for (int r=0; r<7; r++) pattern.Feminine[r+7/*+7*5*/+2]=table.Rows[2+r].Cells[1+6].Text;
                            for (int r=0; r<7; r++) pattern.Middle[r+7/*+7*7*/+2]=table.Rows[2+r].Cells[1+7].Text;

                            pattern.Optimize();
                            itemsPatternAdjectiveFrom.Add(pattern);
                            PatternAdjectiveFromRefreshFilteredList();
                            PatternAdjectiveFromSetListBox();
                            PatternAdjectiveFromListBoxSetCurrent();
                        }
                    }
                };
            #if !DEBUG
            } catch { MessageBox.Show("Error");}
            #endif
            Computation.DownloadString(ref handler, name);
            //string name = GetString("", "Název adjektiva");
            //if (name == null)
            //    return;
            //WebClient client = new WebClient();

            //bool downloading = false;

            //timerDownloading.Tick += delegate {
            //    if (downloading) {
            //        client.CancelAsync();
            //        MessageBox.Show("Nezle stáhnout");
            //    }
            //    timerDownloading.Stop();
            //};
            //timerDownloading.Interval = 5000;
            //timerDownloading.Start();

            //downloading = true;
            //client.DownloadDataAsync(new Uri("https://cs.wiktionary.org/wiki/" + Uri.EscapeDataString(name)));
            //client.DownloadDataCompleted += (sender2, e2) => {
            //    downloading = false;
            //    if (e2.Error!=null){
            //        MessageBox.Show(e2.Error.Message);
            //        return;
            //    }
            //    var data = e2.Result;
            //    string strz = Encoding.UTF8.GetString(data);

            //    if (strz.Contains("<table class=\"deklinace adjektivum\">")) {
            //        int start = strz.IndexOf("<table class=\"deklinace adjektivum\">");
            //        string _table = strz.Substring(start);
            //        string table = _table.Substring(0, _table.IndexOf("</table>"));
            //        List<string> tvary = new System.Collections.Generic.List<string>();
            //        Console.WriteLine("|" + table + "|");
            //        bool ignore = false;
            //        char[] arr = table.ToCharArray();
            //        string ac = "";
            //        for (int i = 0; i < arr.Length; i++) {
            //            char ch = arr[i];
            //            if (ch == '<') {

            //                ignore = true;
            //                if (ac != "" && ac != "\n" && ac != "\n\n") {
            //                    if (i + 3 < arr.Length) {
            //                        if ((arr[i + 1] == 't' && arr[i + 2] == 'd' && arr[i + 3] == '>') || (arr[i + 1] == 't' && arr[i + 2] == 'r' && arr[i + 3] == '>') || (arr[i + 1] == 't' && arr[i + 2] == 'h' && arr[i + 3] == '>')) {
            //                            Add();
            //                        }
            //                    } else
            //                        Add();
            //                    void Add() {
            //                        tvary.Add(Uri.UnescapeDataString(ac).Replace("\n", ""));
            //                        ac = "";
            //                    }
            //                }
            //            }

            //            if (ch == '>') {
            //                ignore = false;
            //            } else if (!ignore) {
            //                ac += ch;
            //            }
            //        }
            //        if (!ignore) {
            //            if (ac != "" && ac != "\n" && ac != "\n\n") {
            //                tvary.Add(Uri.UnescapeDataString(ac).Replace("\n", ""));
            //            }
            //        }

            //        if (tvary[0] == ""
            //        && tvary[1] == "Číslosingulárplurál"
            //        && tvary[2] == "Rod") {
            //            //bool str=false;
            //            List<string> arr2 = new List<string>();
            //            for (int i = 0; i < tvary.Count; i++) {
            //                var tvar = tvary[i];
            //                //if (tvar == "nominativ") { str = true; continue; }
            //                //if (str) {
            //                if (tvar == "")
            //                    continue;
            //                if (tvar == "nominativ")
            //                    continue;
            //                if (tvar == "genitiv")
            //                    continue;
            //                if (tvar == "dativ")
            //                    continue;
            //                if (tvar == "lokativ")
            //                    continue;
            //                if (tvar == "akuzativ")
            //                    continue;
            //                if (tvar == "vokativ")
            //                    continue;
            //                if (tvar == "instrumentál")
            //                    continue;
            //                if (tvar == "lokál")
            //                    continue;
            //                if (tvar == "plurál")
            //                    continue;
            //                if (tvar == "singulár")
            //                    continue;
            //                if (tvar == "Substantivum")
            //                    continue;
            //                if (tvar == "mužskýneživotný")
            //                    continue;
            //                if (tvar == "mužskýživotný")
            //                    continue;
            //                if (tvar == "ženský")
            //                    continue;
            //                if (tvar == "střední")
            //                    continue;
            //                if (tvar == "Číslosingulárplurál")
            //                    continue;
            //                if (tvar == "Rod")
            //                    continue;
            //                arr2.Add(tvar);
            //                //}
            //            }
            //            if (arr2.Count != 56) {
            //                MessageBox.Show("Špatné jméno");
            //                return;
            //            }

            //            int total = -1;
            //            string toplen = arr2.OrderBy(i => -i.Length).ToArray()[0];
            //            for (int step = 1; step < toplen.Length; step++) {
            //                string same = toplen/*arr2[0]*/.Substring(0, step);
            //                bool allOK = true;

            //                for (int i = 0; i < arr2.Count; i++) {
            //                    var a = arr2[i];
            //                    if (step <=/**/a.Length) {
            //                        string sub = a.Substring(0, step);
            //                        if (same == sub) {
            //                            //ok
            //                        } else {
            //                            // total=step-1;
            //                            allOK = false;
            //                            break;
            //                        }
            //                    } else {
            //                        total = step - 1/*-1*/;
            //                        allOK = false;
            //                        break;
            //                    }
            //                }
            //                if (!allOK) {
            //                    break;
            //                } else {

            //                    total = step;
            //                    /*  if (step+1==arr2[0].Length){
            //                          total=step+1;
            //                      }*/
            //                }
            //            }
            //            if (total != -1) {
            //                for (int i = 0; i < arr2.Count; i++) {
            //                    if (arr2[i].Contains(" / ")) {
            //                        string[] d = arr2[i].Split(new string[] { " / " }, StringSplitOptions.None);
            //                        string o = "";
            //                        foreach (string s in d) {
            //                            o += s.Substring(total);
            //                            o += ",";
            //                        }
            //                        if (o.Length > 0)
            //                            o = o.Substring(0, o.Length - 1);
            //                        arr2[i] = o;
            //                    } else {
            //                        arr2[i] = arr2[i].Substring(total);
            //                    }
            //                }
            //            }
            //            PatternAdjectiveFromSaveCurrent();
            //            ItemPatternAdjective item = new ItemPatternAdjective {
            //                MasculineAnimate = new string[14] { arr2[0], arr2[8], arr2[16], arr2[24], arr2[32], arr2[40], arr2[48], arr2[4], arr2[12], arr2[20], arr2[28], arr2[36], arr2[44], arr2[52] },
            //                MasculineInanimate = new string[14] { arr2[1], arr2[9], arr2[17], arr2[25], arr2[33], arr2[41], arr2[49], arr2[5], arr2[13], arr2[21], arr2[29], arr2[37], arr2[45], arr2[53] },
            //                Feminine = new string[14] { arr2[2], arr2[10], arr2[18], arr2[26], arr2[34], arr2[42], arr2[50], arr2[6], arr2[14], arr2[22], arr2[30], arr2[38], arr2[46], arr2[54] },
            //                Middle = new string[14] { arr2[3], arr2[11], arr2[19], arr2[27], arr2[35], arr2[43], arr2[51], arr2[7], arr2[15], arr2[23], arr2[31], arr2[39], arr2[47], arr2[55] }
            //            };
            //            if (total > 0)
            //                item.Name = name.Substring(0, total) + name.Substring(total).ToUpper();
            //            else
            //                item.Name = name;
            //            if (item.Name == "")
            //                throw new Exception();
            //            //  if (strz.Contains("rod mužský neživotný"))item.Gender=GenderNoun.MasculineInanimate;
            //            // else if (strz.Contains("rod ženský"))item.Gender=GenderNoun.Feminine;
            //            // else if (strz.Contains("rod střední") || strz.Contains("rod střední"))item.Gender=GenderNoun.Neuter;
            //            // else if (strz.Contains("rod mužský životný"))item.Gender=GenderNoun.MasculineAnimal;
            //            // else item.Gender=GenderNoun.Unknown;



            //            //   PatternNounFromRefreshFilteredList();
            //            //  PatternNounFromSetListBox();
            //            //  PatternNounFromSetCurrent();

            //            //  if (doingJob) return;
            //            doingJob = true;
            //            Edited = true;
            //            ChangeCaptionText();
            //            PatternAdjectiveFromSaveCurrent();
            //            itemsPatternAdjectiveFrom.Add(item);
            //            /* var newItem=new ItemPatternNoun();
            //            // newItem.ID=itemsNouns.Count;
            //             itemsPatternNounFrom.Add(newItem);
            //             CurrentPatternNounFrom=newItem;*/
            //            CurrentPatternAdjectiveFrom = item;
            //            PatternAdjectiveFromRefreshFilteredList();
            //            PatternAdjectiveFromSetListBox();
            //            PatternAdjectiveFromListBoxSetCurrent();
            //            PatternAdjectiveFromSetCurrent();

            //            doingJob = false;

            //        } else
            //            MessageBox.Show("Jiná tabulka");
            //    } else
            //        MessageBox.Show("Nenalezeno");
            //};
        }

        void TimerSecond_Tick(object sender, EventArgs e) {
            timerIndex++;

            if (tabControl1.SelectedTab.Text == "Verb") {
                if (CurrentVerb != null) {
                    // From
                    ItemPatternVerb patternFrom = itemsPatternVerbFrom.GetItemWithName(CurrentVerb.PatternFrom);// GetPatternVerb(CurrentVerb.PatternFrom);
                    if (patternFrom != null) {

                        var toforeach = new List<string[]> {
                            new string[] { patternFrom.Infinitive }
                        };
                        if (patternFrom.SContinous)         toforeach.Add(patternFrom.Continous);
                        if (patternFrom.SImperative)        toforeach.Add(patternFrom.Imperative);
                        if (patternFrom.SPastActive)        toforeach.Add(patternFrom.PastActive);
                        if (patternFrom.SPastPassive)       toforeach.Add(patternFrom.PastPassive);
                        if (patternFrom.SFuture)            toforeach.Add(patternFrom.Future);
                        if (patternFrom.STransgressiveCont) toforeach.Add(patternFrom.TransgressiveCont);
                        if (patternFrom.STransgressivePast) toforeach.Add(patternFrom.TransgressivePast);

                        int i=0;
                        bool br=false;
                        foreach (string[] v in toforeach) {
                            foreach (string u in v) {
                                if (i==timerIndex) {
                                    labelVerbShowFrom.Text = CurrentVerb.From + u;
                                //    goto endPatternVerbFrom;
                                    br=true;
                                    break;
                                }
                                i++;
                            }
                            if (br) break;
                        }
                    }

                    // To
                    foreach (var iverb in CurrentVerb.To) {
                        ItemPatternVerb patternTo = itemsPatternVerbTo.GetItemWithName(iverb.Pattern);
                        if (patternTo != null) {
                            var toforeach = new List<string[]> {
                                new string[] { patternTo.Infinitive }
                            };
                            if (patternTo.SContinous)         toforeach.Add(patternTo.Continous);
                            if (patternTo.SImperative)        toforeach.Add(patternTo.Imperative);
                            if (patternTo.SPastActive)        toforeach.Add(patternTo.PastActive);
                            if (patternTo.SPastPassive)       toforeach.Add(patternTo.PastPassive);
                            if (patternTo.SFuture)            toforeach.Add(patternTo.Future);
                            if (patternTo.STransgressiveCont) toforeach.Add(patternTo.TransgressiveCont);
                            if (patternTo.STransgressivePast) toforeach.Add(patternTo.TransgressivePast);
                        }
                    }
                }
            }

            if (tabControl1.SelectedTab.Text == "Noun") {
                if (CurrentNoun != null) {

                    // From
                    ItemPatternNoun patternF = itemsPatternNounFrom.GetItemWithName(CurrentNoun.PatternFrom);
                    if (patternF != null) {
                        if (timerIndex > 7 * 2)  timerIndex = 0;

                        if (timerIndex < 7 * 2) {
                            string[] shapes = patternF.Shapes[timerIndex].Split(',');
                            string final = "";
                            foreach (string s in shapes)
                                final += CurrentNoun.From + s + " ";
                            labelNounShowFrom.Text = final;
                            // return;
                        }
                    }
                    TranslatingToDataWithPattern[] to = simpleUINouns.GetData();
                    ItemPatternNoun[] patterns=new ItemPatternNoun[to.Length];
                    for (int i=0; i<to.Length; i++) {
                        patterns[i]= itemsPatternNounTo.GetItemWithName(to[i].Pattern);
                    }
                    simpleUINouns.SetShowNoun(timerIndex%14, patterns);

                    return;
                }
            }
        }

        //ItemPatternVerb GetPatternVerb(string name) {
        //    foreach (ItemPatternVerb item in itemsPatternVerbFrom) {
        //        if (item.Name == name) {
        //            return item;
        //        }
        //    }
        //    return null;
        //}

        //ItemPatternNoun GetPatternNounFrom(string name) {
        //    foreach (ItemPatternNoun item in itemsPatternNounFrom) {
        //        if (item.Name == name) {
        //            return item;
        //        }
        //    }
        //    return null;
        //}

        //ItemPatternNoun GetPatternNounTo(string name) {
        //    foreach (ItemPatternNoun item in itemsPatternNounTo) {
        //        if (item.Name == name) {
        //            return item;
        //        }
        //    }
        //    return null;
        //}

        void ComboBoxPatternNumberType_SelectedIndexChanged(object sender, EventArgs e) {
            if (CurrentPatternNumberFrom != null) {
                CurrentPatternNumberFrom.ShowType = (NumberType)comboBoxPatternNumberFromType.SelectedIndex;
                PatternNumberFromSetCurrent();
            }
        }

        void Button2_Click(object sender, EventArgs e) {
            if (CurrentPatternNumberFrom == null) return;
            string str = Clipboard.GetText();
            Debug.WriteLine(str);
            string[] lines = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            string[][] table = new string[lines.Length][];

            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                var cells = line.Split('\t');
                table[i] = cells;
            }

            int len = -1;
            bool allRawsSameLen = true;
            int start = 0;
            for (int i = 0; i < table.Length; i++) {
                string[] line = table[i];
                if (line[0] == "nominativ")
                    start = i;

                if (len == -1)
                    len = line.Length;
                else {
                    if (len != line.Length) {
                        if (i < 7)
                            allRawsSameLen = false;
                        break;
                    }
                }
            }

            if (!allRawsSameLen)
                return;

            if (len == 9) {
                string[][] etable = new string[7][];
                string same = null;
                for (int i = start; i < start + 7; i++) {
                    var line = table[i];
                    etable[i] = new string[8];

                    for (int j = 1; j < 9; j++) {
                        if (same == null) {
                            same = line[j];
                            Debug.WriteLine(same + " " + line[j]);
                        } else {
                            same = SaveStartsWith(same, line[j]);
                            Debug.WriteLine(same + " " + line[j]);
                        }
                        etable[i - start][j - 1] = line[j];
                    }
                }

                for (int i = 0; i < 7; i++) {
                    // var line=table[i];
                    for (int j = 0; j < 8; j++) {
                        etable[i][j] = etable[i][j].Substring(same.Length);
                    }
                }

                CurrentPatternNumberFrom.ShowType = NumberType.DeklinationWithGender;

                textBoxPatternNumberFromMuzS1.Text = etable[0][0];
                textBoxPatternNumberFromMuzS2.Text = etable[1][0];
                textBoxPatternNumberFromMuzS3.Text = etable[2][0];
                textBoxPatternNumberFromMuzS4.Text = etable[3][0];
                textBoxPatternNumberFromMuzS5.Text = etable[4][0];
                textBoxPatternNumberFromMuzS6.Text = etable[5][0];
                textBoxPatternNumberFromMuzS7.Text = etable[6][0];
                textBoxPatternNumberFromMuzM1.Text = etable[0][4];
                textBoxPatternNumberFromMuzM2.Text = etable[1][4];
                textBoxPatternNumberFromMuzM3.Text = etable[2][4];
                textBoxPatternNumberFromMuzM4.Text = etable[3][4];
                textBoxPatternNumberFromMuzM5.Text = etable[4][4];
                textBoxPatternNumberFromMuzM6.Text = etable[5][4];
                textBoxPatternNumberFromMuzM7.Text = etable[6][4];

                textBoxPatternNumberFromMunS1.Text = etable[0][1];
                textBoxPatternNumberFromMunS2.Text = etable[1][1];
                textBoxPatternNumberFromMunS3.Text = etable[2][1];
                textBoxPatternNumberFromMunS4.Text = etable[3][1];
                textBoxPatternNumberFromMunS5.Text = etable[4][1];
                textBoxPatternNumberFromMunS6.Text = etable[5][1];
                textBoxPatternNumberFromMunS7.Text = etable[6][1];
                textBoxPatternNumberFromMunM1.Text = etable[0][5];
                textBoxPatternNumberFromMunM2.Text = etable[1][5];
                textBoxPatternNumberFromMunM3.Text = etable[2][5];
                textBoxPatternNumberFromMunM4.Text = etable[3][5];
                textBoxPatternNumberFromMunM5.Text = etable[4][5];
                textBoxPatternNumberFromMunM6.Text = etable[5][5];
                textBoxPatternNumberFromMunM7.Text = etable[6][5];

                textBoxPatternNumberFromZenS1.Text = etable[0][2];
                textBoxPatternNumberFromZenS2.Text = etable[1][2];
                textBoxPatternNumberFromZenS3.Text = etable[2][2];
                textBoxPatternNumberFromZenS4.Text = etable[3][2];
                textBoxPatternNumberFromZenS5.Text = etable[4][2];
                textBoxPatternNumberFromZenS6.Text = etable[5][2];
                textBoxPatternNumberFromZenS7.Text = etable[6][2];
                textBoxPatternNumberFromZenM1.Text = etable[0][6];
                textBoxPatternNumberFromZenM2.Text = etable[1][6];
                textBoxPatternNumberFromZenM3.Text = etable[2][6];
                textBoxPatternNumberFromZenM4.Text = etable[3][6];
                textBoxPatternNumberfromZenM5.Text = etable[4][6];
                textBoxPatternNumberFromZenM6.Text = etable[5][6];
                textBoxPatternNumberFromZenM7.Text = etable[6][6];

                textBoxPatternNumberFromStrS1.Text = etable[0][3];
                textBoxPatternNumberFromStrS2.Text = etable[1][3];
                textBoxPatternNumberFromStrS3.Text = etable[2][3];
                textBoxPatternNumberFromStrS4.Text = etable[3][3];
                textBoxPatternNumberFromStrS5.Text = etable[4][3];
                textBoxPatternNumberFromStrS6.Text = etable[5][3];
                textBoxPatternNumberFromStrS7.Text = etable[6][3];
                textBoxPatternNumberFromStrM1.Text = etable[0][7];
                textBoxPatternNumberFromStrM2.Text = etable[1][7];
                textBoxPatternNumberFromStrM3.Text = etable[2][7];
                textBoxPatternNumberFromStrM4.Text = etable[3][7];
                textBoxPatternNumberFromStrM5.Text = etable[4][7];
                textBoxPatternNumberFromStrM6.Text = etable[5][7];
                textBoxPatternNumberFromStrM7.Text = etable[6][7];
            }
        }

        static string SaveStartsWith(string a, string b) {
            for (int i = 0; i < a.Length && i < b.Length; i++) {
                if (a[i] != b[i]) {
                    if (i == 0)
                        return "";
                    else
                        return a.Substring(0, i);
                }
            }

            if (a.Length < b.Length)
                return a;
            else
                return b;
        }

        #region ContextMenuStrip show
        void ListBoxAdjectives_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripAdjective.Show(MousePosition);
        }

        void ContextMenuStripPatternAdjective_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternAdjectiveFrom.Show(MousePosition);
        }

        void ListBoxNoun_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripNoun.Show(MousePosition);
        }

        void ListBoxPatternNoun_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternNounFrom.Show(MousePosition);
        }

        void ListBoxPatternAdjectiveFrom_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternAdjectiveFrom.Show(MousePosition);
        }

        void ListBoxPatternPronoun_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternPronounFrom.Show(MousePosition);
        }

        void ListBoxPatternNounTo_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternNounTo.Show(MousePosition);
        }

        void ListBoxSimpleWord_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripSimpleWord.Show(MousePosition);
        }

        void ListBoxPatternVerb_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternVerbFrom.Show(MousePosition);
        }

        void ListBoxPatternAdjectiveTo_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternAdjectiveTo.Show(MousePosition);
        }

        void ListBoxPatternPronounTo_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternPronounTo.Show(MousePosition);
        }

        void PatternNumberFromlistBox_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternNumberFrom.Show(MousePosition);
        }

        void PatternNumberTolistBox_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternNumberTo.Show(MousePosition);
        }

        void ListBoxNumbers_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripNumber.Show(MousePosition);
        }

        void ListBoxVerbs_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripVerb.Show(MousePosition);
        }

        void PatternVerbToListBox_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPatternVerbTo.Show(MousePosition);
        }

        void ListBoxAdverb_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripAdverb.Show(MousePosition);
        }

        void ListBoxPreposition_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPreposition.Show(MousePosition);
        }

        void ListBoxConjunction_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripConjuction.Show(MousePosition);
        }

        void ListBoxParticle_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripParticle.Show(MousePosition);
        }

        void ListBoxInterjection_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripInterjection.Show(MousePosition);
        }

        void ListBoxSentencePart_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripSentencePart.Show(MousePosition);
        }

        void ListBoxSentence_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripSentence.Show(MousePosition);
        }

        void ListBoxSentencePatterns_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripSentencePattern.Show(MousePosition);
        }

        void ListBoxSentencePatternPart_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripSentencePatternPart.Show(MousePosition);
        }

        void ListBoxReplaceG_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripReplaceG.Show(MousePosition);
        }

        void ListBoxReplaceE_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripReplaceE.Show(MousePosition);
        }

        void ListBoxReplaceS_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripReplaceS.Show(MousePosition);
        }

        void ListBoxPhrase_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right)
                contextMenuStripPhrase.Show(MousePosition);
        }
        #endregion

        #region contexstrip Noun
        void AbecedněToolStripMenuItem_Click(object sender, EventArgs e) {
            doingJob = true;
            SaveCurrentNoun();
            itemsNouns = itemsNouns.OrderBy(a => a.From).ToList();
            NounRefreshFilteredList();
            NounSetListBox();
            doingJob = false;
        }

        void DleVzorůToolStripMenuItem_Click(object sender, EventArgs e) {
            doingJob = true;
            SaveCurrentNoun();
            itemsNouns = itemsNouns.OrderBy(a => itemsPatternNounFrom.GetItemWithName(a.PatternFrom)/* FindPatternFrom(a.PatternFrom)*/?.Gender).ToList();
            NounRefreshFilteredList();
            NounSetListBox();
            doingJob = false;
        }

        //ItemPatternNoun FindPatternFrom(string name) {
        //    foreach (ItemPatternNoun p in itemsPatternNounFrom) {
        //        if (p.Name == name)
        //            return p;
        //    }
        //    return null;
        //}

        //ItemPatternNoun FindPatternTo(string name) {
        //    foreach (ItemPatternNoun p in itemsPatternNounTo) {
        //        if (p.Name == name)
        //            return p;
        //    }
        //    return null;
        //}

        void DleNázvuVzorceToolStripMenuItem_Click(object sender, EventArgs e) {
            doingJob = true;
            SaveCurrentNoun();
            itemsNouns = itemsNouns.OrderBy(a =>itemsPatternNounFrom.GetItemWithName(a.PatternFrom)/*  FindPatternFrom(a.PatternFrom)*/?.Name).ToList();
            NounRefreshFilteredList();
            NounSetListBox();
            doingJob = false;
        }

        void ToolStripMenuItem121_Click(object sender, EventArgs e) {
            ButtonSentencePatternPartAdd_Click(null, null);
        }

        void ToolStripMenuItem153_Click(object sender, EventArgs e) {
            ButtonSentencePartAdd_Click(null, null);
        }

        void ToolStripMenuItem139_Click(object sender, EventArgs e) {
            ButtonReplaceGAdd_Click(null, null);
        }

        void AddToolStripMenuItem4_Click(object sender, EventArgs e) {
            ButtonPatternNounFromAdd_Click(null, null);
        }

        void AddToolStripMenuItem1_Click(object sender, EventArgs e) => ButtonAdjectiveAdd_Click(null, null);


        void MoveToolStripMenuItem3_Click(object sender, EventArgs e) {
            // From pattternNounFrom to patternnounTo
            if (CurrentPatternNounFrom == null)  return;
            ItemPatternNoun PatternNounFrom = CurrentPatternNounFrom;
            itemsPatternNounTo.Add(PatternNounFrom);
            itemsPatternNounFrom.Remove(PatternNounFrom);

            PatternNounToRefreshFilteredList();
            PatternNounFromRefreshFilteredList();

            PatternNounFromSetListBox();
            PatternNounFromSetCurrent();


            PatternNounToSetListBox();
            PatternNounToSetCurrent();
        }

        void AddFromWikidirectonaryToolStripMenuItem_Click(object sender, EventArgs e) {
            string name = GetString("", "Název pronomenu");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;
            #if !DEBUG
            try{
            #endif
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    List<Table> tables=new List<Table>();
                    Computation.FindTableInHTML(html, "deklinace pronomen", ref tables);

                    if (tables.Count>=1) {
                        Table table=tables[0];
                        if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
                            ItemPatternPronoun pattern = new ItemPatternPronoun {
                                Name = name,
                                Type = PronounType.DeklinationWithGender,
                                Shapes = new string[8*7]
                            };

                            for (int r=0; r<7; r++) pattern.Shapes[r]=table.Rows[2+r].Cells[1+0].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*2]=table.Rows[2+r].Cells[1+1].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*4]=table.Rows[2+r].Cells[1+2].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*6]=table.Rows[2+r].Cells[1+3].Text;

                            for (int r=0; r<7; r++) pattern.Shapes[r+7*1]=table.Rows[2+r].Cells[1+4].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*3]=table.Rows[2+r].Cells[1+5].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*5]=table.Rows[2+r].Cells[1+6].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*7]=table.Rows[2+r].Cells[1+7].Text;

                            pattern.Optimize();
                            itemsPatternPronounFrom.Add(pattern);
                            PatternPronounFromRefreshFilteredList();
                            PatternPronounFromSetListBox();
                            PatternPronounFromListBoxSetCurrent();
                        } else if (table.Rows.Count==8 && table.Rows[3].Cells.Count==2) {
                            ItemPatternPronoun pattern = new ItemPatternPronoun {
                                Name = name,
                                Type = PronounType.DeklinationOnlySingle,
                                Shapes = new string[7]
                            };
                            for (int c=0; c<7; c++) {
                                pattern.Shapes[c]=table.Rows[1+c].Cells[1].Text;
                            }

                            pattern.Optimize();
                            itemsPatternPronounFrom.Add(pattern);
                            PatternPronounFromRefresh();
                        }
                    }
                };
            #if !DEBUG
            }catch{ MessageBox.Show("Error");}
            #endif
            Computation.DownloadString(ref handler, name);
        }
        #endregion

        #region StripMenu PatternNoun From
        void DleNázvuToolStripMenuItem_Click(object sender, EventArgs e) {
            doingJob = true;
            PatternNounFromSaveCurrent();
            itemsPatternNounFrom = itemsPatternNounFrom.OrderBy(a => a.Name).ToList();
            PatternNounFromRefreshFilteredList();
            //   SetListBoxPatternNoun();
            PatternNounFromSetListBox();
            doingJob = false;
        }

        void PřejmenovatSOdkazyToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentPatternNounFrom != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternNoun '" + CurrentPatternNounFrom.Name + "' s odkazy na...",
                    Input = CurrentPatternNounFrom.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternNounFrom.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternNounFromSaveCurrent();
                            SaveCurrentNoun();

                            foreach (ItemNoun noun in itemsNouns) {
                                if (noun.PatternFrom == CurrentPatternNounFrom.Name) {
                                    noun.PatternFrom = edit.ReturnString;
                                }
                            }
                            CurrentPatternNounFrom.Name = edit.ReturnString;
                            PatternNounFromSetCurrent();
                            SetCurrentNoun();
                        }
                    }
                }
            }
        }

        void DleRoduToolStripMenuItem_Click(object sender, EventArgs e) {
            PatternNounFromSaveCurrent();
            itemsPatternNounFrom = itemsPatternNounFrom.OrderBy(a => (int)a.Gender).ToList();
            PatternNounFromRefreshFilteredList();
            PatternNounFromSetListBox();
        }
        #endregion

        #region StripMenu PatternNounTo
        void PřejmenovatSOdkazyToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (CurrentPatternNounTo != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternNoun '" + CurrentPatternNounTo.Name + "' s odkazy na...",
                    Input = CurrentPatternNounTo.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternNounTo.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternNounToSaveCurrent();
                            SaveCurrentNoun();

                            foreach (ItemNoun noun in itemsNouns) {
                                for (int i=0; i<noun.To.Count; i++){
                                    TranslatingToDataWithPattern d = noun.To[i];
                                    if (d.Pattern == CurrentPatternNounTo.Name) {
                                        //if (d.Item1.Contains(d.Item2)) return true;
                                        noun.To[i] = new TranslatingToDataWithPattern{Body=d.Body, Pattern=edit.ReturnString };
                                    }
                                }
                                //if (noun.PatternTo == CurrentPatternNounTo.Name) {
                                //    noun.PatternTo = edit.ReturnString;
                                //}
                            }
                            CurrentPatternNounTo.Name = edit.ReturnString;
                            PatternNounToSetCurrent();
                            PatternNounToRefreshFilteredList();
                            SetCurrentNoun();
                        }
                    }
                }
            }
        }

        void DleRoduToolStripMenuItem1_Click(object sender, EventArgs e) {
            PatternNounToSaveCurrent();

            itemsPatternNounTo = itemsPatternNounTo.OrderBy(a => (int)a.Gender).ToList();
            PatternNounToRefreshFilteredList();
            PatternNounToSetListBox();
        }

        void MoveToToolStripMenuItem_Click(object sender, EventArgs e) {
            // From pattternpronounFrom to patternpronounTo
            if (CurrentPatternPronounFrom == null) return;
            ItemPatternPronoun PatternPronounFrom = CurrentPatternPronounFrom;
            itemsPatternPronounTo.Add(PatternPronounFrom);
            itemsPatternPronounFrom.Remove(PatternPronounFrom);

            PatternPronounToRefreshFilteredList();
            PatternPronounFromRefreshFilteredList();

            PatternPronounFromSetListBox();
            PatternPronounFromSetCurrent();


            PatternPronounToSetListBox();
            PatternPronounToSetCurrent();
        }

        void MoveToolStripMenuItem2_Click(object sender, EventArgs e) {
            // From pattternadjectiveFrom to patternAdjectiveTo
            if (CurrentPatternAdjectiveFrom == null)   return;
            ItemPatternAdjective PatternAdjectiveFrom = CurrentPatternAdjectiveFrom;
            itemsPatternAdjectiveTo.Add(PatternAdjectiveFrom);
            itemsPatternAdjectiveFrom.Remove(PatternAdjectiveFrom);

            PatternAdjectiveToRefreshFilteredList();
            PatternAdjectiveFromRefreshFilteredList();

            PatternAdjectiveFromSetListBox();
            PatternAdjectiveFromSetCurrent();

            PatternAdjectiveToSetListBox();
            PatternAdjectiveToSetCurrent();
        }

        void MoveToolStripMenuItem_Click(object sender, EventArgs e) {
            // From pattternVerbFrom to patternVerbTo
            if (CurrentPatternVerbFrom == null) return;
            ItemPatternVerb PatternVerbFrom = CurrentPatternVerbFrom;
            itemsPatternVerbTo.Add(PatternVerbFrom);
            itemsPatternVerbFrom.Remove(PatternVerbFrom);

            PatternVerbToRefreshFilteredList();
            PatternVerbFromRefreshFilteredList();

            PatternVerbFromSetListBox();
            PatternVerbFromSetCurrent();


            PatternVerbToSetListBox();
            PatternVerbToSetCurrent();
        }

        void DuplikovatToolStripMenuItem1_Click(object sender, EventArgs e) {
            PatternNounToSaveCurrent();
            itemsPatternNounTo.Add(CurrentPatternNounTo.Duplicate());
            PatternNounToRefreshFilteredList();
            PatternNounToSetListBox();
        }

        void toolStripMenuItem9_Click(object sender, EventArgs e) {
            ButtonVerbAdd_Click(null, null);
        }

        void ToolStripMenuItem21_Click(object sender, EventArgs e) {
            ButtonAdverbAdd_Click(null, null);
        }

        void ToolStripMenuItem39_Click(object sender, EventArgs e) {
            ButtonInterjectionAdd_Click(null, null);
        }

        void ToolStripMenuItem45_Click(object sender, EventArgs e) {
            buttonParticleAdd_Click(null, null);
        }

        void ToolStripMenuItem33_Click(object sender, EventArgs e) {
            ButtonConjunctionAdd_Click(null, null);
        }

        void AbecedněToolStripMenuItem1_Click(object sender, EventArgs e) {
            doingJob = true;
            PatternNounToSaveCurrent();
            itemsPatternNounTo = itemsPatternNounTo.OrderBy(a => a.Name).ToList();
            PatternNounToRefreshFilteredList();

            PatternNounToSetListBox();
            doingJob = false;
        }
        #endregion

        void DuplikovatToolStripMenuItem_Click(object sender, EventArgs e) {
            PatternPronounFromSaveCurrent();
            itemsPatternPronounFrom.Add(CurrentPatternPronounFrom.Duplicate());
            PatternPronounFromRefreshFilteredList();
            PatternPronounFromSetListBox();
        }

        void NovýZWikidirecnonaryToolStripMenuItem_Click(object sender, EventArgs e) {
            //string name = GetString("", "Název noun");
            //if (name == null)
            //    return;
            //WebClient client = new WebClient();

            //bool downloading = false;

            //timerDownloading.Tick += delegate {
            //    if (downloading) {
            //        client.CancelAsync();
            //        MessageBox.Show("Nezle stáhnout");
            //    }
            //    timerDownloading.Stop();
            //};
            //timerDownloading.Interval = 5000;
            //timerDownloading.Start();

            //downloading = true;
            //client.DownloadDataAsync(new Uri("https://cs.wiktionary.org/wiki/" + Uri.EscapeDataString(name)));
            //client.DownloadDataCompleted += (sender2, e2) => {
            //    downloading = false;
            //    if (e2.Error!=null) return;
            //    var data = e2.Result;
            //    string strz = Encoding.UTF8.GetString(data);

            //    if (strz.Contains("<table class=\"deklinace substantivum\">")) {
            //        int start = strz.IndexOf("<table class=\"deklinace substantivum\">");
            //        string _table = strz.Substring(start);
            //        string table = _table.Substring(0, _table.IndexOf("</table>"));
            //        List<string> tvary = new System.Collections.Generic.List<string>();
            //        Console.WriteLine("|" + table + "|");
            //        bool ignore = false;
            //        char[] arr = table.ToCharArray();
            //        string ac = "";
            //        for (int i = 0; i < arr.Length; i++) {
            //            char ch = arr[i];
            //            if (ch == '<') {

            //                ignore = true;
            //                if (ac != "" && ac != "\n" && ac != "\n\n") {
            //                    if (i + 3 < arr.Length) {
            //                        if ((arr[i + 1] == 't' && arr[i + 2] == 'd' && arr[i + 3] == '>') || (arr[i + 1] == 't' && arr[i + 2] == 'r' && arr[i + 3] == '>') || (arr[i + 1] == 't' && arr[i + 2] == 'h' && arr[i + 3] == '>')) {
            //                            Add();
            //                        }
            //                    } else
            //                        Add();
            //                    void Add() {
            //                        tvary.Add(Uri.UnescapeDataString(ac).Replace("\n", ""));
            //                        ac = "";
            //                    }
            //                }
            //            }

            //            if (ch == '>') {
            //                ignore = false;
            //            } else if (!ignore) {
            //                ac += ch;
            //            }
            //        }
            //        if (!ignore) {
            //            if (ac != "" && ac != "\n" && ac != "\n\n") {
            //                tvary.Add(Uri.UnescapeDataString(ac).Replace("\n", ""));
            //            }
            //        }
            //        if (tvary[0] == ""
            //        && tvary[1] == "Substantivum"
            //        && tvary[2] == "singulár"
            //        && tvary[3] == "plurál") {
            //            //bool str=false;
            //            List<string> arr2 = new List<string>();
            //            for (int i = 0; i < tvary.Count; i++) {
            //                var tvar = tvary[i];
            //                //if (tvar == "nominativ") { str = true; continue; }
            //                //if (str) {
            //                if (tvar == "")
            //                    continue;
            //                if (tvar == "nominativ")
            //                    continue;
            //                if (tvar == "genitiv")
            //                    continue;
            //                if (tvar == "dativ")
            //                    continue;
            //                if (tvar == "lokativ")
            //                    continue;
            //                if (tvar == "akuzativ")
            //                    continue;
            //                if (tvar == "vokativ")
            //                    continue;
            //                if (tvar == "instrumentál")
            //                    continue;
            //                if (tvar == "lokál")
            //                    continue;
            //                if (tvar == "plurál")
            //                    continue;
            //                if (tvar == "singulár")
            //                    continue;
            //                if (tvar == "Substantivum")
            //                    continue;
            //                arr2.Add(tvar);
            //                //}
            //            }
            //            if (arr2.Count != 14) {
            //                MessageBox.Show("Špatné jméno");
            //                return;
            //            }

            //            int total = -1;
            //            string toplen = arr2.OrderBy(i => -i.Length).ToArray()[0];
            //            for (int step = 1; step < toplen.Length; step++) {
            //                string same = toplen/*arr2[0]*/.Substring(0, step);
            //                bool allOK = true;

            //                for (int i = 0; i < arr2.Count; i++) {
            //                    var a = arr2[i];
            //                    if (step <=/**/a.Length) {
            //                        string sub = a.Substring(0, step);
            //                        if (same == sub) {
            //                            //ok
            //                        } else {
            //                            // total=step-1;
            //                            allOK = false;
            //                            break;
            //                        }
            //                    } else {
            //                        total = step - 1/*-1*/;
            //                        allOK = false;
            //                        break;
            //                    }
            //                }
            //                if (!allOK) {
            //                    break;
            //                } else {

            //                    total = step;
            //                    /*  if (step+1==arr2[0].Length){
            //                          total=step+1;
            //                      }*/
            //                }
            //            }
            //            if (total != -1) {
            //                for (int i = 0; i < arr2.Count; i++) {
            //                    if (arr2[i].Contains(" / ")) {
            //                        string[] d = arr2[i].Split(new string[] { " / " }, StringSplitOptions.None);
            //                        string o = "";
            //                        foreach (string s in d) {
            //                            o += s.Substring(total);
            //                            o += ",";
            //                        }
            //                        if (o.Length > 0)
            //                            o = o.Substring(0, o.Length - 1);
            //                        arr2[i] = o;
            //                    } else {
            //                        arr2[i] = arr2[i].Substring(total);
            //                    }
            //                }
            //            }
            //            PatternNounFromSaveCurrent();
            //            ItemPatternNoun item = new ItemPatternNoun {
            //                Shapes = new string[14]{
            //                    arr2[0], arr2[2], arr2[4], arr2[6], arr2[8], arr2[10], arr2[12],
            //                    arr2[1], arr2[3], arr2[5], arr2[7], arr2[9], arr2[11], arr2[13]
            //                },

            //            };
            //            if (total>0)item.Name = name.Substring(0, total) + name.Substring(total).ToUpper();
            //            else item.Name = name.ToUpper();
            //            if (item.Name == "")   throw new Exception();
            //            if (strz.Contains("rod mužský neživotný"))  item.Gender = GenderNoun.MasculineInanimate;
            //            else if (strz.Contains("rod ženský"))  item.Gender = GenderNoun.Feminine;
            //            else if (strz.Contains("rod střední") || strz.Contains("rod střední"))  item.Gender = GenderNoun.Neuter;
            //            else if (strz.Contains("rod mužský životný"))   item.Gender = GenderNoun.MasculineAnimal;
            //            else  item.Gender = GenderNoun.Unknown;



            //            //   PatternNounFromRefreshFilteredList();
            //            //  PatternNounFromSetListBox();
            //            //  PatternNounFromSetCurrent();

            //            //  if (doingJob) return;
            //            doingJob = true;
            //            Edited = true;
            //            ChangeCaptionText();
            //            PatternNounFromSaveCurrent();
            //            itemsPatternNounFrom.Add(item);
            //            /* var newItem=new ItemPatternNoun();
            //            // newItem.ID=itemsNouns.Count;
            //             itemsPatternNounFrom.Add(newItem);
            //             CurrentPatternNounFrom=newItem;*/
            //            CurrentPatternNounFrom = item;
            //            PatternNounFromRefreshFilteredList();
            //            PatternNounFromSetListBox();
            //            PatternNounFromListBoxSetCurrent();
            //            PatternNounFromSetCurrent();
            //            if (CurrentNoun!=null) SetCurrentNoun();
            //            doingJob = false;

            //        } else
            //            MessageBox.Show("Jiná tabulka");
            //    } else
            //        MessageBox.Show("Nenalezeno");
            //};
            string name = GetString("", "Název noun");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;

            try{
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    List<Table> tables=new List<Table>();
                    Computation.FindTableInHTML(html, "deklinace substantivum", ref tables);

                    ItemPatternNoun pattern = new ItemPatternNoun {
                        Name = name,
                       // TypeShow = VerbTypeShow.Unknown,
                        Shapes=new string[14]
                    };
                    bool found=false;
                   // bool future=false;
                    if (tables.Count>0){
                        Table table=tables[0];
                        if (table.Rows.Count==8) {
                            if (table.Rows[0].Cells[0].Text.ToLower()=="pád \\ číslo") {
                                found=true;
                                for (int i=0; i<7; i++) {
                                    pattern.Shapes[i]=table.Rows[i+1].Cells[1].Text;
                                }
                                for (int i=0; i<7; i++) {
                                    pattern.Shapes[i+7]=table.Rows[i+1].Cells[2].Text;
                                }
                            } else MessageBox.Show("Error, something else,non substantivnum");
                        } else MessageBox.Show("Error, something else,height");
                    } else MessageBox.Show("Error, nic");
                if (!found) return;
                    if (html.Contains("rod střední"))pattern.Gender=GenderNoun.Neuter;
                    else if (html.Contains("rod ženský"))pattern.Gender=GenderNoun.Feminine;
                    else if (html.Contains("rod mužský neživotný"))pattern.Gender=GenderNoun.MasculineInanimate;
                    else if (html.Contains("rod mužský životný"))pattern.Gender=GenderNoun.MasculineAnimal;
                    pattern.Optimize();

                    itemsPatternNounFrom.Add(pattern);
                    PatternNounFromRefreshFilteredList();
                  //  nounsPatterns.SetComboboxes();
                    PatternNounFromSetListBox();
                    PatternNounFromListBoxSetCurrent();
                };
            } catch { MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);

        }

        void ZálohovatToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentFile==NewProject)return;
            string data = File.ReadAllText(CurrentFile);
            DateTime now = DateTime.Now;
            File.WriteAllText(CurrentFile + " - zaloha " + now.Year + " " + now.Day + "-" + now.Month + ".trw", data);
        }

        void NačístRawToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Textové soubory|*.*txt|Všecky sóbore|*.*"
            };
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    doingJob = true;
                    Edited = true;
                    ChangeCaptionText();
                    SimpleWordSaveCurrent();
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines) {
                        string[] parts = line.Split('|');
                        if (parts[0] == "W" && parts.Length == 3) {
                            if (!parts[1].Contains(" ")) {
                                foreach (string s in parts[1].Split('#')) {
                                    ItemSimpleWord w = new ItemSimpleWord {
                                        From = s,
                                        To = Methods.LoadListTranslatingToDataWithPattern(parts[2], '#') // parts[2].Replace("#", ",")
                                    };
                                    itemsSimpleWords.Add(w);
                                }
                            }
                        }

                        if (parts[0] == "O" && parts.Length == 2) {
                            if (!parts[1].Contains(" ")) {
                                ItemSimpleWord w = new ItemSimpleWord();
                                w.From = parts[1];
                                w.To = new List<TranslatingToData>{new TranslatingToData{ Text=parts[1] }};
                                itemsSimpleWords.Add(w);
                            }
                        }
                    }

                    SimpleWordRefreshFilteredList();
                    SimpleWordSetListBox();
                    SimpleWordListBoxSetCurrent();
                    SimpleWordSetCurrent();

                    doingJob = false;
                }
            }
        }

        void OptimalizovatToolStripMenuItem_Click(object sender, EventArgs e) {
            for (int i = 0; i < itemsAdjectives.Count; i++) {
                ItemAdjective item = itemsAdjectives[i];
                if (item.IsBlanck()) {
                    itemsAdjectives.RemoveAt(i);
                    i--;
                    continue;
                }
                item.Optimize();
                //if (item.From.EndsWith(" ")) {
                //    item.From.Substring(item.From.Length - 1);
                //}
                //if (item.To.EndsWith(" ")) {
                //    item.To.Substring(item.From.Length - 1);
                //}
            }
            for (int i = 0; i < itemsSimpleWords.Count; i++) {
                ItemSimpleWord item = itemsSimpleWords[i];
                if (item.IsBlanck()) {
                    itemsSimpleWords.RemoveAt(i);
                    i--;
                    continue;
                }
                item.Optimize();
                //if (item.From.EndsWith(" ")) {
                //    item.From = item.From.Substring(item.From.Length - 1);
                //}
                //if (item.To.EndsWith(" ")) {
                //    item.To = item.To.Substring(item.From.Length - 1);
                //}
            }
            for (int i = 0; i < itemsPhrases.Count; i++) {
                ItemPhrase item = itemsPhrases[i];
                if (item.IsBlanck()) {
                    itemsPhrases.RemoveAt(i);
                    i--;
                    continue;
                }
                item.Optimize();
                //if (item.From.EndsWith(" ")) {
                //    item.From = item.From.Substring(item.From.Length - 1);
                //}
                //if (item.To.EndsWith(" ")) {
                //    item.To = item.To.Substring(item.From.Length - 1);
                //}
            }
        }

        void DuplikovatToolStripMenuItem2_Click(object sender, EventArgs e) {
            PatternVerbFromSaveCurrent();
            itemsPatternVerbFrom.Add(CurrentPatternVerbFrom.Duplicate());
            PatternVerbFromRefreshFilteredList();
            PatternVerbFromSetListBox();
        }

        void ButtonWikidirectonaryPronoun_Click(object sender, EventArgs e) {
            string name = GetString(CurrentPatternPronounFrom.Name, "Název pronounu");
            if (name == null)
                return;

            WebClient client = new WebClient();

            bool downloading = false;

            timerDownloading.Tick += delegate {
                if (downloading) {
                    client.CancelAsync();
                }
                timerDownloading.Stop();
            };
            timerDownloading.Interval = 5000;
            timerDownloading.Start();

            downloading = true;
            client.DownloadDataAsync(new Uri("https://cs.wiktionary.org/wiki/" + Uri.EscapeDataString(name)));
            client.DownloadDataCompleted += (sender2, e2) => {
                var data = e2.Result;
                string strz = Encoding.UTF8.GetString(data);

                if (strz.Contains("<table class=\"deklinace pronomen\">")) {
                    int start = strz.IndexOf("<table class=\"deklinace pronomen\">");
                    string _table = strz.Substring(start);
                    string table = _table.Substring(0, _table.IndexOf("</table>"));
                    List<string> tvary = new System.Collections.Generic.List<string>();
                    Console.WriteLine("|" + table + "|");
                    bool ignore = false;
                    char[] arr = table.ToCharArray();
                    string ac = "";
                    for (int i = 0; i < arr.Length; i++) {
                        char ch = arr[i];
                        if (ch == '<') {
                            ignore = true;
                            if (ac != "" && ac != "\n" && ac != "\n\n") {
                                tvary.Add(Uri.UnescapeDataString(ac).Replace("\n", ""));
                                ac = "";
                            }
                        }

                        if (ch == '>') {
                            ignore = false;
                        } else if (!ignore) {
                            ac += ch;
                        }
                    }
                    if (!ignore) {
                        if (ac != "" && ac != "\n" && ac != "\n\n") {
                            tvary.Add(Uri.UnescapeDataString(ac).Replace("\n", ""));
                        }
                    }
                    if (tvary[0] == ""
                    && tvary[1] == "Číslo"
                    && tvary[2] == "singulár"
                    && tvary[3] == "plurál"
                    && tvary[4] == "Rod"
                    && tvary[5] == "mužský"
                    && tvary[6] == "životný"
                    && tvary[7] == "mužský"
                    && tvary[8] == "neživotný"
                    && tvary[9] == "ženský"
                    && tvary[10] == "střední"
                    && tvary[11] == "mužský"
                    && tvary[12] == "životný"
                    && tvary[13] == "mužský") {
                        bool str = false;
                        List<string> arr2 = new List<string>();
                        for (int i = 0; i < tvary.Count; i++) {
                            var tvar = tvary[i];
                            if (tvar == "nominativ") { str = true; continue; }
                            if (str) {
                                if (tvar == "")
                                    continue;
                                if (tvar == "nominativ")
                                    continue;
                                if (tvar == "genitiv")
                                    continue;
                                if (tvar == "dativ")
                                    continue;
                                if (tvar == "lokativ")
                                    continue;
                                if (tvar == "akuzativ")
                                    continue;
                                if (tvar == "vokativ")
                                    continue;
                                if (tvar == "instrumentál")
                                    continue;
                                if (tvar == "lokál")
                                    continue;
                                arr2.Add(tvar);
                            }
                        }
                        if (arr2.Count != 7 * 2 * 4) {
                            return;
                        }

                        int total = -1;
                        for (int step = 1; step < arr2[0].Length; step++) {
                            string same = arr2[0].Substring(0, step);
                            bool allOK = true;
                            for (int i = 0; i < arr2.Count; i++) {
                                var a = arr2[i];
                                if (step <= a.Length) {
                                    if (same == a.Substring(0, step)) {
                                        //ok
                                    } else {
                                        total = step - 1;
                                        allOK = false;
                                        break;
                                    }
                                } else { total = step - 1; allOK = false; break; }
                            }
                            if (!allOK) {
                                break;
                            }
                        }

                        for (int i = 0; i < arr2.Count; i++) {
                            arr2[i] = arr2[i].Substring(total);
                        }

                        textBoxPatternPronounFromMuzJ1.Text = arr2[0];
                        textBoxPatternPronounFromMuzJ2.Text = arr2[8];
                        textBoxPatternPronounFromMuzJ3.Text = arr2[16];
                        textBoxPatternPronounFromMuzJ4.Text = arr2[24];
                        textBoxPatternPronounFromMuzJ5.Text = arr2[32];
                        textBoxPatternPronounFromMuzJ6.Text = arr2[40];
                        textBoxPatternPronounFromMuzJ7.Text = arr2[48];

                        textBoxPatternPronounFromMuzM1.Text = arr2[4];
                        textBoxPatternPronounFromMuzM2.Text = arr2[12];
                        textBoxPatternPronounFromMuzM3.Text = arr2[20];
                        textBoxPatternPronounFromMuzM4.Text = arr2[28];
                        textBoxPatternPronounFromMuzM5.Text = arr2[36];
                        textBoxPatternPronounFromMuzM6.Text = arr2[44];
                        textBoxPatternPronounFromMuzM7.Text = arr2[52];


                        textBoxPatternPronounFromMunJ1.Text = arr2[1];
                        textBoxPatternPronounFromMunJ2.Text = arr2[9];
                        textBoxPatternPronounFromMunJ3.Text = arr2[17];
                        textBoxPatternPronounFromMunJ4.Text = arr2[25];
                        textBoxPatternPronounFromMunJ5.Text = arr2[33];
                        textBoxPatternPronounFromMunJ6.Text = arr2[41];
                        textBoxPatternPronounFromMunJ7.Text = arr2[49];

                        textBoxPatternPronounFromMunM1.Text = arr2[5];
                        textBoxPatternPronounFromMunM2.Text = arr2[13];
                        textBoxPatternPronounFromMunM3.Text = arr2[21];
                        textBoxPatternPronounFromMunM4.Text = arr2[29];
                        textBoxPatternPronounFromMunM5.Text = arr2[37];
                        textBoxPatternPronounFromMunM6.Text = arr2[45];
                        textBoxPatternPronounFromMunM7.Text = arr2[53];


                        textBoxPatternPronounFromZenJ1.Text = arr2[2];
                        textBoxPatternPronounFromZenJ2.Text = arr2[10];
                        textBoxPatternPronounFromZenJ3.Text = arr2[18];
                        textBoxPatternPronounFromZenJ4.Text = arr2[26];
                        textBoxPatternPronounFromZenJ5.Text = arr2[34];
                        textBoxPatternPronounFromZenJ6.Text = arr2[42];
                        textBoxPatternPronounFromZenJ7.Text = arr2[50];

                        textBoxPatternPronounFromZenM1.Text = arr2[6];
                        textBoxPatternPronounFromZenM2.Text = arr2[14];
                        textBoxPatternPronounFromZenM3.Text = arr2[22];
                        textBoxPatternPronounFromZenM4.Text = arr2[30];
                        textBoxPatternPronounFromZenM5.Text = arr2[38];
                        textBoxPatternPronounFromZenM6.Text = arr2[46];
                        textBoxPatternPronounFromZenM7.Text = arr2[54];


                        textBoxPatternPronounFromStrJ1.Text = arr2[3];
                        textBoxPatternPronounFromStrJ2.Text = arr2[11];
                        textBoxPatternPronounFromStrJ3.Text = arr2[19];
                        textBoxPatternPronounFromStrJ4.Text = arr2[27];
                        textBoxPatternPronounFromStrJ5.Text = arr2[35];
                        textBoxPatternPronounFromStrJ6.Text = arr2[43];
                        textBoxPatternPronounFromStrJ7.Text = arr2[51];

                        textBoxPatternPronounFromStrM1.Text = arr2[7];
                        textBoxPatternPronounFromStrM2.Text = arr2[15];
                        textBoxPatternPronounFromStrM3.Text = arr2[23];
                        textBoxPatternPronounFromStrM4.Text = arr2[31];
                        textBoxPatternPronounFromStrM5.Text = arr2[39];
                        textBoxPatternPronounFromStrM6.Text = arr2[47];
                        textBoxPatternPronounFromStrM7.Text = arr2[55];
                    }
                }
            };
        }

        string GetString(string def, string txt) {
            FormString str = new FormString {
                Input = def,
                LabelText = txt
            };
            DialogResult dr = str.ShowDialog();
            if (dr == DialogResult.OK)
                return str.ReturnString;
            return null;
        }

        #region Sentence
        void ListBoxSentences_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSentence();

            int index=listBoxSentence.SelectedIndex;
            if (itemsSentences.Count==0) {
                SentenceSetNone();
                return;
            }
            if (index>=itemsSentences.Count)
                index=itemsSentences.Count-1;
            if (index<0)
                index=0;

            CurrentSentence=itemsSentences[index];
            SetCurrentSentence();
            SetListBoxSentence();
          //  SetCurrent();
            doingJob=false;
        }

        void buttonSentenceAdd_Click(object sender, EventArgs e) {
            AddNewItemSentence();
        }

        void ButtonSentenceRemove_Click(object sender, EventArgs e) {
            RemoveItemSentence(CurrentSentence);
            TextBoxSentenceFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxSentenceFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentSentence();

            // Získej aktuální prvek
            ItemSentence selectedId=null;
            if (listBoxSentence.SelectedIndex!=-1) {
                selectedId=itemsSentencesFiltered[listBoxSentence.SelectedIndex];
            }

            SentenceRefreshFilteredList();

            listBoxSentence.Items.Clear();
            for (int i=0; i<itemsSentencesFiltered.Count; i++) {
                ItemSentence item = itemsSentencesFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentence.Items.Add(textToAdd);
            }

            //SetListBoxSentence();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsSentencesFiltered.Count; i++){
                    if (itemsSentencesFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxSentence.SelectedIndex=-1;
                    CurrentSentence=null;
                } else listBoxSentence.SelectedIndex=outIndex;
            } else {
                listBoxSentence.SelectedIndex=-1;
                CurrentSentence=null;
            }
            SetCurrentSentence();
        }

        void RemoveCurrentSentence(object sender, EventArgs e) {
            itemsSentences.Remove(CurrentSentence);
        }

        void SetListBoxSentence() {
            //string filter=textBoxSentenceFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxSentence.SelectedIndex;
            listBoxSentence.Items.Clear();
            for (int i=0; i<itemsSentencesFiltered.Count; i++) {
                ItemSentence item = itemsSentencesFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentence.Items.Add(textToAdd);
            }

            if (index>=listBoxSentence.Items.Count)index=listBoxSentence.Items.Count-1;
            listBoxSentence.SelectedIndex=index;
        }

        void SentenceRefreshFilteredList() {
            if (itemsSentencesFiltered==null) itemsSentencesFiltered=new List<ItemSentence>();
            itemsSentencesFiltered.Clear();
            string filter=textBoxSentenceFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsSentences.Count; i++) {
                    ItemSentence item = itemsSentences[i];

                    if (item.Filter(filter)) {
                        itemsSentencesFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsSentences.Count; i++) {
                    ItemSentence item = itemsSentences[i];
                    itemsSentencesFiltered.Add(item);
                }
            }
        }

        void AddNewItemSentence() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSentence();

            var newItem=new ItemSentence();
            newItem.To=new List<TranslatingToData>{ new TranslatingToData()};
          //  newItem.ID=itemsSentences.Count;
            itemsSentences.Add(newItem);
            CurrentSentence=newItem;
            SentenceRefreshFilteredList();
            SetListBoxSentence();
            ListBoxSetCurrentSentence();
            SetCurrentSentence();

            doingJob=false;
        }

        void RemoveItemSentence(ItemSentence item) {
            Edited=true;
            ChangeCaptionText();
            itemsSentences.Remove(item);
            SentenceRefreshFilteredList();
            SetListBoxSentence();
            SetCurrentSentence();
        }

        void SetCurrentSentence(){
            if (itemsSentencesFiltered.Count==0) {
                SentenceSetNone();
                return;
            }

            int index=listBoxSentence.SelectedIndex;
            if (index>=itemsSentencesFiltered.Count) index=itemsSentencesFiltered.Count-1;
            if (index<0) index=0;
            CurrentSentence=itemsSentencesFiltered[index];

            textBoxSentenceSource.Visible=true;
            //textBoxSentenceTo.Visible=true;
            labelSentenceSource.Visible=true;
           // labelSentenceTo.Visible=true;
           //buttonSentence.Visible=true;
            textBoxSentenceSource.Text= CurrentSentence.From;
            //textBoxSentenceTo.Text= CurrentSentence.To;

            simpleUISentence.Visible=true;
            simpleUISentence.SetData(CurrentSentence.To.ToArray());

        }

        void ListBoxSetCurrentSentence() {
            for (int indexCur=0; indexCur<itemsSentencesFiltered.Count; indexCur++) {
                if (itemsSentences[indexCur]==CurrentSentence) {
                    int indexList=listBoxSentence.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSentence.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        //int GetListBoxSelectedIndexSentence() {
        //    if (listBoxSentences.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterSentence.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) {
        //        var item=itemsSentencesFiltered[listBoxSentences.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsSentences.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else {
        //        return listBoxSentences.SelectedIndex;
        //    }

        //    return -1;
        //}

        void SaveCurrentSentence() {
            if (CurrentSentence==null) return;

            CurrentSentence.From=textBoxSentenceSource.Text;
           // CurrentSentence.To=textBoxSentenceTo.Text;


            CurrentSentence.To=simpleUISentence.GetData().ToList();
        }

        void SentenceSetNone(){
            textBoxSentenceSource.Text="";
           // textBoxSentenceTo.Text="";
            textBoxSentenceSource.Visible=false;
           // textBoxSentenceTo.Visible=false;
            labelSentenceSource.Visible=false;
         //   labelSentenceTo.Visible=false;
         //buttonSentence.Visible=false;
            simpleUISentence.Visible=false;
            simpleUISentence.Clear();
        }

        void ClearSentences(){
            listBoxSentence.Items.Clear();
            SentenceSetNone();
            itemsSentencesFiltered?.Clear();
            itemsSentences?.Clear();
            CurrentSentence=null;
        }
        #endregion

        #region Phrase
        void ListBoxPhrases_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PhraseSaveCurrent();

            int index=listBoxPhrase.SelectedIndex;
            if (itemsPhrases.Count==0) {
                PhraseSetNone();
                return;
            }
            if (index>=itemsPhrases.Count)
                index=itemsPhrases.Count-1;
            if (index<0)
                index=0;

            CurrentPhrase=itemsPhrases[index];
            PhraseSetCurrent();
            PhraseSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonPhraseAdd_Click(object sender, EventArgs e) {
            PhraseAddNewItem();
        }

        void ButtonPhraseRemove_Click(object sender, EventArgs e) {
            PhraseRemoveItem(CurrentPhrase);
            TextBoxPhraseFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxPhraseFilter_TextChanged(object sender, EventArgs e) {
            if (!uiLoaded) return;
            PhraseSaveCurrent();

            // Získej aktuální prvek
            ItemPhrase selectedId=null;
            if (listBoxPhrase.SelectedIndex!=-1) {
                selectedId=itemsPhrasesFiltered[listBoxPhrase.SelectedIndex];
            }

            PhraseRefreshFilteredList();

            listBoxPhrase.Items.Clear();
            for (int i=0; i<itemsPhrasesFiltered.Count; i++) {
                ItemPhrase item = itemsPhrasesFiltered[i];

                string textToAdd=item.GetText();
           //     if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPhrase.Items.Add(textToAdd);
            }

            //SetListBoxPhrase();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPhrasesFiltered.Count; i++){
                    if (itemsPhrasesFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxPhrase.SelectedIndex=-1;
                    CurrentPhrase=null;
                } else listBoxPhrase.SelectedIndex=outIndex;
            } else {
                listBoxPhrase.SelectedIndex=-1;
                CurrentPhrase=null;
            }
            PhraseSetCurrent();
        }

        void RemoveCurrentPhrase(object sender, EventArgs e) {
            itemsPhrases.Remove(CurrentPhrase);
        }

        void PhraseSetListBox() {
            //string filter=textBoxPhraseFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxPhrase.SelectedIndex;
            listBoxPhrase.Items.Clear();
            for (int i=0; i<itemsPhrasesFiltered.Count; i++) {
                ItemPhrase item = itemsPhrasesFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPhrase.Items.Add(textToAdd);
            }

            if (index>=listBoxPhrase.Items.Count)index=listBoxPhrase.Items.Count-1;
            listBoxPhrase.SelectedIndex=index;
        }

        void PhraseRefreshFilteredList() {
            if (itemsPhrasesFiltered==null) itemsPhrasesFiltered=new List<ItemPhrase>();
            itemsPhrasesFiltered.Clear();
            string filter=textBoxPhraseFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPhrases.Count; i++) {
                    ItemPhrase item = itemsPhrases[i];

                    if (item.Filter(filter)) {
                        itemsPhrasesFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPhrases.Count; i++) {
                    ItemPhrase item = itemsPhrases[i];
                    itemsPhrasesFiltered.Add(item);
                }
            }
        }

        void PhraseAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PhraseSaveCurrent();

            var newItem=new ItemPhrase();
            newItem.To=new List<TranslatingToData>{ new TranslatingToData{ Text=""} };
          //  newItem.ID=itemsPhrases.Count;
            itemsPhrases.Add(newItem);
            CurrentPhrase=newItem;
            PhraseRefreshFilteredList();
            PhraseSetListBox();
            PhraseListBoxSetCurrent();
            PhraseSetCurrent();

            doingJob=false;
        }

        void PhraseRemoveItem(ItemPhrase item) {
            Edited=true;
            ChangeCaptionText();
            itemsPhrases.Remove(item);
            PhraseRefreshFilteredList();
            PhraseSetListBox();
            PhraseSetCurrent();
        }

        void PhraseSetCurrent(){
            if (itemsPhrasesFiltered.Count==0) {
                PhraseSetNone();
                return;
            }

            int index=listBoxPhrase.SelectedIndex;
            if (index>=itemsPhrasesFiltered.Count) index=itemsPhrasesFiltered.Count-1;
            if (index<0) index=0;
            CurrentPhrase=itemsPhrasesFiltered[index];

            textBoxPhraseFrom.Visible=true;
            textBoxPhraseFrom.Text= CurrentPhrase.From;
            //textBoxPhraseTo.Visible=true;
            labelPhraseFrom.Visible=true;

            //buttonAddToPhrase.Visible=true;
            //labelPhraseTo.Visible=true;
            checkBoxPhraseShow.Visible=true;
            checkBoxPhraseShow.Checked=CurrentPhrase.Show;
           // textBoxPhraseTo.Text= CurrentPhrase.To;
            simpleUIPhrase.Visible=true;
            simpleUIPhrase.SetData(CurrentPhrase.To.ToArray());
        }

        void PhraseListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPhrasesFiltered.Count; indexCur++) {
                if (itemsPhrases[indexCur]==CurrentPhrase) {
                    int indexList=listBoxPhrase.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPhrase.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        //int GetListBoxSelectedIndexPhrase() {
        //    if (listBoxPhrases.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterPhrase.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) {
        //        var item=itemsPhrasesFiltered[listBoxPhrases.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsPhrases.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else {
        //        return listBoxPhrases.SelectedIndex;
        //    }

        //    return -1;
        //}

        void PhraseSaveCurrent() {
            if (CurrentPhrase==null) return;

            CurrentPhrase.From=textBoxPhraseFrom.Text;
            //CurrentPhrase.To=textBoxPhraseTo.Text;
            CurrentPhrase.Show=checkBoxPhraseShow.Checked;
            CurrentPhrase.To=simpleUIPhrase.GetData().ToList();
        }

        void PhraseSetNone(){
            textBoxPhraseFrom.Visible=false;
            textBoxPhraseFrom.Text="";
       //     textBoxPhraseTo.Text="";
          //  textBoxPhraseTo.Visible=false;
            labelPhraseFrom.Visible=false;
        //    labelPhraseTo.Visible=false;
            checkBoxPhraseShow.Visible=false;
            //buttonAddToPhrase.Visible=false;
            simpleUIPhrase.Clear();
            simpleUIPhrase.Visible=false;
        }

        void ClearPhrase(){
            listBoxPhrase.Items.Clear();
            PhraseSetNone();
            itemsPhrasesFiltered?.Clear();
            itemsPhrases?.Clear();
            CurrentPhrase=null;
        }
        #endregion

        #region PhrasePattern
        void ListBoxPhrasePatternPatterns_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PhrasePatternSaveCurrent();

            int index=listBoxPhrasePattern.SelectedIndex;
            if (itemsPhrasePattern.Count==0) {
                PhrasePatternSetNone();
                return;
            }
            if (index>=itemsPhrasePattern.Count)
                index=itemsPhrasePattern.Count-1;
            if (index<0)
                index=0;

            CurrentPhrasePattern=itemsPhrasePattern[index];
            PhrasePatternSetCurrent();
            PhrasePatternSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonPhrasePatternAdd_Click(object sender, EventArgs e) {
            PhrasePatternAddNewItem();
        }

        void ButtonPhrasePatternRemove_Click(object sender, EventArgs e) {
            PhrasePatternRemoveItem(CurrentPhrasePattern);
            TextBoxPhrasePatternFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxPhrasePatternFilter_TextChanged(object sender, EventArgs e) {
            PhrasePatternSaveCurrent();

            // Získej aktuální prvek
            ItemPhrasePattern selectedId=null;
            if (listBoxPhrasePattern.SelectedIndex!=-1) {
                selectedId=itemsPhrasePatternFiltered[listBoxPhrasePattern.SelectedIndex];
            }

            PhrasePatternRefreshFilteredList();

            listBoxPhrasePattern.Items.Clear();
            for (int i=0; i<itemsPhrasePatternFiltered.Count; i++) {
                ItemPhrasePattern item = itemsPhrasePatternFiltered[i];

                string textToAdd=item.GetText();
           //     if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPhrasePattern.Items.Add(textToAdd);
            }

            //SetListBoxPhrasePattern();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPhrasePatternFiltered.Count; i++){
                    if (itemsPhrasePatternFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxPhrasePattern.SelectedIndex=-1;
                    CurrentPhrasePattern=null;
                } else listBoxPhrasePattern.SelectedIndex=outIndex;
            } else {
                listBoxPhrasePattern.SelectedIndex=-1;
                CurrentPhrasePattern=null;
            }
            PhrasePatternSetCurrent();
        }

        void RemoveCurrentPhrasePattern(object sender, EventArgs e) {
            itemsPhrasePattern.Remove(CurrentPhrasePattern);
        }

        void PhrasePatternSetListBox() {
            //string filter=textBoxPhrasePatternFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxPhrasePattern.SelectedIndex;
            listBoxPhrasePattern.Items.Clear();
            for (int i=0; i<itemsPhrasePatternFiltered.Count; i++) {
                ItemPhrasePattern item = itemsPhrasePatternFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPhrasePattern.Items.Add(textToAdd);
            }

            if (index>=listBoxPhrasePattern.Items.Count)index=listBoxPhrasePattern.Items.Count-1;
            listBoxPhrasePattern.SelectedIndex=index;
        }

        void PhrasePatternRefreshFilteredList() {
            if (itemsPhrasePatternFiltered==null) itemsPhrasePatternFiltered=new List<ItemPhrasePattern>();
            itemsPhrasePatternFiltered.Clear();
            string filter=textBoxPhrasePatternFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPhrasePattern.Count; i++) {
                    ItemPhrasePattern item = itemsPhrasePattern[i];

                    if (item.Filter(filter)) {
                        itemsPhrasePatternFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPhrasePattern.Count; i++) {
                    ItemPhrasePattern item = itemsPhrasePattern[i];
                    itemsPhrasePatternFiltered.Add(item);
                }
            }
        }

        void PhrasePatternAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PhrasePatternSaveCurrent();

            var newItem=new ItemPhrasePattern();
          //  newItem.ID=itemsPhrasePatterns.Count;
            itemsPhrasePattern.Add(newItem);
            CurrentPhrasePattern=newItem;
            PhrasePatternRefreshFilteredList();
            PhrasePatternSetListBox();
            PhrasePatternListBoxSetCurrent();
            PhrasePatternSetCurrent();

            doingJob=false;
        }

        void PhrasePatternRemoveItem(ItemPhrasePattern item) {
            Edited=true;
            ChangeCaptionText();
            itemsPhrasePattern.Remove(item);
            PhrasePatternRefreshFilteredList();
            PhrasePatternSetListBox();
            PhrasePatternSetCurrent();
        }

        void PhrasePatternSetCurrent(){
            if (itemsPhrasePatternFiltered.Count==0) {
                PhrasePatternSetNone();
                return;
            }

            int index=listBoxPhrasePattern.SelectedIndex;
            if (index>=itemsPhrasePatternFiltered.Count) index=itemsPhrasePatternFiltered.Count-1;
            if (index<0) index=0;
            CurrentPhrasePattern=itemsPhrasePatternFiltered[index];

            textBoxPhrasePatternFrom.Visible=true;
            textBoxPhrasePatternFrom.Text=CurrentPhrasePattern.From;

            textBoxPhrasePatternTo.Visible=true;
            textBoxPhrasePatternTo.Text=CurrentPhrasePattern.To;

            labelPhrasePatternFrom.Visible=true;
            labelPhrasePatternTo.Visible=true;
        }

        void PhrasePatternListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPhrasePatternFiltered.Count; indexCur++) {
                if (itemsPhrasePattern[indexCur]==CurrentPhrasePattern) {
                    int indexList=listBoxPhrasePattern.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPhrasePattern.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        //int GetListBoxSelectedIndexPhrasePattern() {
        //    if (listBoxPhrasePatterns.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterPhrasePattern.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) {
        //        var item=itemsPhrasePatternsFiltered[listBoxPhrasePatterns.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsPhrasePatterns.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else {
        //        return listBoxPhrasePatterns.SelectedIndex;
        //    }

        //    return -1;
        //}

        void PhrasePatternSaveCurrent() {
            if (CurrentPhrasePattern==null) return;

            CurrentPhrasePattern.From=textBoxPhrasePatternFrom.Text;
            CurrentPhrasePattern.To=textBoxPhrasePatternTo.Text;
        }

        void PhrasePatternSetNone(){
            textBoxPhrasePatternTo.Visible=false;
            textBoxPhrasePatternTo.Text="";
            textBoxPhrasePatternFrom.Text="";
            textBoxPhrasePatternFrom.Visible=false;
            labelPhrasePatternFrom.Visible=false;
            labelPhrasePatternTo.Visible=false;

            simpleUIPhrase.Visible=false;
            simpleUIPhrase.Clear();
        }

        //void ClearPhrasePattern(){
        //    listBoxPhrasePattern.Items.Clear();
        //    PhrasePatternSetNone();
        //    itemsPhrasePatternFiltered?.Clear();
        //    itemsPhrasePattern?.Clear();
        //    CurrentPhrasePattern=null;
        //}
        #endregion

        #region SimpleWord
        void ListBoxSimpleWord_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SimpleWordSaveCurrent();

            int index=listBoxSimpleWord.SelectedIndex;
            if (itemsSimpleWords.Count==0) {
                SimpleWordSetNone();
                return;
            }
            if (index>=itemsSimpleWords.Count)    index=itemsSimpleWords.Count-1;
            if (index<0) index=0;

            CurrentSimpleWord=itemsSimpleWords[index];
            SimpleWordSetCurrent();
            SimpleWordSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonSimpleWordAdd_Click(object sender, EventArgs e) {
            SimpleWordAddNewItem();
        }

        void ButtonSimpleWordRemove_Click(object sender, EventArgs e) {
            RemoveItemSimpleWord(CurrentSimpleWord);
            TextBoxSimpleWordFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxSimpleWordFilter_TextChanged(object sender, EventArgs e) {
            SimpleWordSaveCurrent();
            DrawingControl.SuspendDrawing(listBoxSimpleWord);
            // Získej aktuální prvek
            ItemSimpleWord selectedId=null;
            if (listBoxSimpleWord.SelectedIndex!=-1) {
                selectedId=itemsSimpleWordsFiltered[listBoxSimpleWord.SelectedIndex];
            }

            SimpleWordRefreshFilteredList();

            listBoxSimpleWord.Items.Clear();
            for (int i=0; i<itemsSimpleWordsFiltered.Count; i++) {
                ItemSimpleWord item = itemsSimpleWordsFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSimpleWord.Items.Add(textToAdd);
            }

            //SetListBoxSimpleWord();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsSimpleWordsFiltered.Count; i++){
                    if (itemsSimpleWordsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxSimpleWord.SelectedIndex=-1;
                    CurrentSimpleWord=null;
                } else listBoxSimpleWord.SelectedIndex=outIndex;
            } else {
                listBoxSimpleWord.SelectedIndex=-1;
                CurrentSimpleWord=null;
            }
            SimpleWordSetCurrent();
            DrawingControl.ResumeDrawing(listBoxSimpleWord);
        }

        void RemoveCurrentSimpleWord(object sender, EventArgs e) {
            itemsSimpleWords.Remove(CurrentSimpleWord);
        }

        void SimpleWordSetListBox() {
            DrawingControl.SuspendDrawing(listBoxSimpleWord);
            //string filter=textBoxSimpleWordFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxSimpleWord.SelectedIndex;
            listBoxSimpleWord.Items.Clear();
            for (int i=0; i<itemsSimpleWordsFiltered.Count; i++) {
                ItemSimpleWord item = itemsSimpleWordsFiltered[i];

                string textToAdd=item.GetText();
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSimpleWord.Items.Add(textToAdd);
            }

            if (index>=listBoxSimpleWord.Items.Count)index=listBoxSimpleWord.Items.Count-1;
            listBoxSimpleWord.SelectedIndex=index;

            DrawingControl.ResumeDrawing(listBoxSimpleWord);
        }

        void SimpleWordRefreshFilteredList() {
            if (itemsSimpleWordsFiltered==null) itemsSimpleWordsFiltered=new List<ItemSimpleWord>();
            itemsSimpleWordsFiltered.Clear();
            string filter=textBoxSimpleWordFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsSimpleWords.Count; i++) {
                    ItemSimpleWord item = itemsSimpleWords[i];

                    if (item.Filter(filter)) {
                        itemsSimpleWordsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsSimpleWords.Count; i++) {
                    ItemSimpleWord item = itemsSimpleWords[i];
                    itemsSimpleWordsFiltered.Add(item);
                }
            }
        }

        void SimpleWordAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SimpleWordSaveCurrent();

            var newItem=new ItemSimpleWord();
            newItem.To=new List<TranslatingToData>{new TranslatingToData{ Text="", Comment=""} };
            // newItem.ID=itemsSimpleWords.Count;
            itemsSimpleWords.Add(newItem);
            CurrentSimpleWord=newItem;
            SimpleWordRefreshFilteredList();
            SimpleWordSetListBox();
            SimpleWordListBoxSetCurrent();
            SimpleWordSetCurrent();

            doingJob=false;
        }

        void RemoveItemSimpleWord(ItemSimpleWord item) {
            Edited=true;
            ChangeCaptionText();
            itemsSimpleWords.Remove(item);
            SimpleWordRefreshFilteredList();
            SimpleWordSetListBox();
            SimpleWordSetCurrent();
        }

        void SimpleWordSetCurrent(){
            if (itemsSimpleWordsFiltered.Count==0) {
                SimpleWordSetNone();
                return;
            }

            int index=listBoxSimpleWord.SelectedIndex;
            if (index>=itemsSimpleWordsFiltered.Count) index=itemsSimpleWordsFiltered.Count-1;
            if (index<0) index=0;
            CurrentSimpleWord=itemsSimpleWordsFiltered[index];

            textBoxSimpleWordFrom.Visible=true;
           // textBoxSimpleWordTo.Visible=true;
            labelSimpleWordFrom.Visible=true;
         //  labelSimpleWordTo.Visible=true;

            //buttonAddToSimpleWord.Visible=true;
           textBoxSimpleWordFrom.Text= CurrentSimpleWord.From;
         //  textBoxSimpleWordTo.Text= CurrentSimpleWord.To;
            checkBoxSimpleWordShow.Visible=true;
            checkBoxSimpleWordShow.Checked=CurrentSimpleWord.Show;
            simpleUISimpleWord.Visible=true;
            simpleUISimpleWord.SetData(CurrentSimpleWord.To.ToArray());
        }

        void SimpleWordListBoxSetCurrent() {
            DrawingControl.SuspendDrawing(listBoxSimpleWord);
            for (int indexCur=0; indexCur<itemsSimpleWordsFiltered.Count; indexCur++) {
                if (itemsSimpleWords[indexCur]==CurrentSimpleWord) {
                    int indexList=listBoxSimpleWord.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSimpleWord.SelectedIndex=indexCur;
                    break;
                }
            }
            DrawingControl.ResumeDrawing(listBoxSimpleWord);
        }

        //int GetListBoxSelectedIndexSimpleWord() {
        //    if (listBoxSimpleWords.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterSimpleWord.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) {
        //        var item=itemsSimpleWordsFiltered[listBoxSimpleWords.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsSimpleWords.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else {
        //        return listBoxSimpleWords.SelectedIndex;
        //    }

        //    return -1;
        //}

        void SimpleWordSaveCurrent() {
            if (CurrentSimpleWord==null) return;

            CurrentSimpleWord.From=textBoxSimpleWordFrom.Text;
       //    CurrentSimpleWord.To=textBoxSimpleWordTo.Text;

             CurrentSimpleWord.Show=checkBoxSimpleWordShow.Checked;
            CurrentSimpleWord.To=simpleUISimpleWord.GetData().ToList();
        }

        void SimpleWordSetNone(){
            textBoxSimpleWordFrom.Text="";
            textBoxSimpleWordFrom.Visible=false;
           // textBoxSimpleWordTo.Text="";
           // textBoxSimpleWordTo.Visible=false;
            labelSimpleWordFrom.Visible=false;
           // labelSimpleWordTo.Visible=false;
            //buttonAddToSimpleWord.Visible=false;

            checkBoxSimpleWordShow.Visible=false;

            simpleUISimpleWord.Visible=false;
            simpleUISimpleWord.Clear();
        }

        void ClearSimpleWord(){
            listBoxSimpleWord.Items.Clear();
            SimpleWordSetNone();
            itemsSimpleWordsFiltered?.Clear();
            itemsSimpleWords?.Clear();
            CurrentSimpleWord=null;
        }
        #endregion

        #region SentencePattern
        void TextBoxSentencePatternFilter_TextChanged(object sender, EventArgs e) {
              SaveCurrentSentencePattern();

            // Získej aktuální prvek
            ItemSentencePattern selectedId=null;
            if (listBoxSentencePatterns.SelectedIndex!=-1) {
                selectedId=itemsSentencePatternsFiltered[listBoxSentencePatterns.SelectedIndex];
            }

            SentencePatternRefreshFilteredList();

            listBoxSentencePatterns.Items.Clear();
            for (int i=0; i<itemsSentencePatternsFiltered.Count; i++) {
                ItemSentencePattern item = itemsSentencePatternsFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePatterns.Items.Add(textToAdd);
            }

            //SetListBoxSimpleWord();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsSentencePatternsFiltered.Count; i++){
                    if (itemsSentencePatternsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxSentencePatterns.SelectedIndex=-1;
                    CurrentSentencePattern=null;
                } else listBoxSentencePatterns.SelectedIndex=outIndex;
            } else {
                listBoxSentencePatterns.SelectedIndex=-1;
                CurrentSentencePattern=null;
            }
            SetCurrentSentencePattern();
          //  SetListBoxSentencePattern();
        }

        void SentencePatternRefreshFilteredList() {
            if (itemsSentencePatternsFiltered==null) itemsSentencePatternsFiltered=new List<ItemSentencePattern>();
            itemsSentencePatternsFiltered.Clear();
            string filter=textBoxSentencePatternFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsSentencePatterns.Count; i++) {
                    ItemSentencePattern item = itemsSentencePatterns[i];

                    if (item.Filter(filter)) {
                        itemsSentencePatternsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsSentencePatterns.Count; i++) {
                    ItemSentencePattern item = itemsSentencePatterns[i];
                    itemsSentencePatternsFiltered.Add(item);
                }
            }
        }

        void ButtonSentencePatternAdd_Click(object sender, EventArgs e) {
            AddNewItemSentencePattern();
        }

        void ButtonSentencePatternRemove_Click(object sender, EventArgs e) {
            RemoveItemSentencePattern(CurrentSentencePattern);
            TextBoxSentencePatternFilter_TextChanged(null, new EventArgs());
        }

        //void SetListBoxSentencePattern() {
        //    string filter=textBoxFilterSentencePattern.Text;
        //    bool useFilter = filter!="" && filter!="*";

        //    int index=listBoxSentencePatterns.SelectedIndex;
        //    listBoxSentencePatterns.Items.Clear();
        //    foreach (ItemSentencePattern item in itemsSentencePatterns) {
        //        if (useFilter) {
        //            if (!item.Filter(filter)) {
        //                continue;
        //            }
        //        }
        //        string textToAdd=item.GetText();
        //        if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

        //        listBoxSentencePatterns.Items.Add(textToAdd);
        //    }
        //    listBoxSentencePatterns.SelectedIndex=index;
        //}
        void SetListBoxSentencePattern() {
            //string filter=textBoxSentencePatternFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxSentencePatterns.SelectedIndex;
            listBoxSentencePatterns.Items.Clear();
            for (int i=0; i<itemsSentencePatternsFiltered.Count; i++) {
                ItemSentencePattern item = itemsSentencePatternsFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePatterns.Items.Add(textToAdd);
            }

            if (index>=listBoxSentencePatterns.Items.Count)index=listBoxSentencePatterns.Items.Count-1;
            listBoxSentencePatterns.SelectedIndex=index;
        }

        void AddNewItemSentencePattern() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSentencePattern();

            var newItem=new ItemSentencePattern();
            itemsSentencePatterns.Add(newItem);
            CurrentSentencePattern=newItem;

            //SetListBoxSentencePattern();
            //ListBoxSetCurrentSentencePattern();
            //SetCurrentSentencePattern();

            SentencePatternRefreshFilteredList();
            SetListBoxSentencePattern();
            ListBoxSetCurrentSentencePattern();
            SetCurrentSentencePattern();

            doingJob=false;
        }

        void RemoveItemSentencePattern(ItemSentencePattern item) {
            Edited=true;
            ChangeCaptionText();
            itemsSentencePatterns.Remove(item);
            SetListBoxSentencePattern();
            SetCurrentSentencePattern();
        }

        void SetCurrentSentencePattern(){
            if (itemsSentencePatternsFiltered.Count==0) {
                SentencePatternSetNone();
                return;
            }

            int index=listBoxSentencePatterns.SelectedIndex;
            if (index>itemsSentencePatternsFiltered.Count) index=itemsSentencePatternsFiltered.Count-1;
            if (index<0) index=0;
            CurrentSentencePattern=itemsSentencePatternsFiltered[index];
          //  throw new Exception();

            textBoxSentencePatternFrom.Text=CurrentSentencePattern.From;
            textBoxSentencePatternTo.Text=CurrentSentencePattern.To;
            textBoxSentencePatternTo.Visible=true;
            textBoxSentencePatternFrom.Visible=true;
            labelSentencePatternInfo.Visible=true;
            labelSentencePatternFrom.Visible=true;
            labelSentencePatternTo.Visible=true;
        }

        void ListBoxSetCurrentSentencePattern() {
            for (int indexCur=0; indexCur<itemsSentencePatternsFiltered.Count; indexCur++) {
                if (itemsSentencePatterns[indexCur]==CurrentSentencePattern) {
                    int indexList=listBoxSentencePatterns.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSentencePatterns.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentSentencePattern() {
            if (CurrentSentencePattern==null) return;

            CurrentSentencePattern.From=textBoxSentencePatternFrom.Text;
            CurrentSentencePattern.To=textBoxSentencePatternTo.Text;
        }

        void RemoveCurrentSentencePattern(object sender, EventArgs e) {
            itemsSentencePatterns.Remove(CurrentSentencePattern);
        }

        void ListBoxSentencePattern_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSentencePattern();

            int index=listBoxSentencePatterns.SelectedIndex;
            if (itemsSentencePatterns.Count==0) {
                SentencePatternSetNone();
                return;
            }
            if (index>=itemsSentencePatterns.Count)  index=itemsSentencePatterns.Count-1;
            if (index<0) index=0;

            CurrentSentencePattern=itemsSentencePatterns[index];
            SetCurrentSentencePattern();
            SetListBoxSentencePattern();
          //  SetCurrent();
            doingJob=false;
        }

        void SentencePatternSetNone() {
            textBoxSentencePatternFrom.Text="";
            textBoxSentencePatternTo.Text="";
            textBoxSentencePatternTo.Visible=false;
            textBoxSentencePatternFrom.Visible=false;
            labelSentencePatternInfo.Visible=false;
            labelSentencePatternFrom.Visible=false;
            labelSentencePatternTo.Visible=false;
        }

        void ClearSentencePattern(){
            listBoxSentencePatterns.Items.Clear();
            SentencePatternSetNone();
            itemsSentencePatternsFiltered?.Clear();
            itemsSentencePatterns?.Clear();
            CurrentSentencePattern=null;
        }
        #endregion

        #region SentencePatternPart
        void TextBoxSentencePatternPartFilter_TextChanged(object sender, EventArgs e) {
              SaveCurrentSentencePatternPart();

            // Získej aktuální prvek
            ItemSentencePatternPart selectedId=null;
            if (listBoxSentencePatternPart.SelectedIndex!=-1) {
                selectedId=itemsSentencePatternPartsFiltered[listBoxSentencePatternPart.SelectedIndex];
            }

            SentencePatternPartRefreshFilteredList();

            listBoxSentencePatternPart.Items.Clear();
            for (int i=0; i<itemsSentencePatternPartsFiltered.Count; i++) {
                ItemSentencePatternPart item = itemsSentencePatternPartsFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePatternPart.Items.Add(textToAdd);
            }

            //SetListBoxSimpleWord();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsSentencePatternPartsFiltered.Count; i++){
                    if (itemsSentencePatternPartsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxSentencePatternPart.SelectedIndex=-1;
                    CurrentSentencePatternPart=null;
                } else listBoxSentencePatternPart.SelectedIndex=outIndex;
            } else {
                listBoxSentencePatternPart.SelectedIndex=-1;
                CurrentSentencePatternPart=null;
            }
            SetCurrentSentencePatternPart();
          //  SetListBoxSentencePatternPart();
        }

        void SentencePatternPartRefreshFilteredList() {
            if (itemsSentencePatternPartsFiltered==null) itemsSentencePatternPartsFiltered=new List<ItemSentencePatternPart>();
            itemsSentencePatternPartsFiltered.Clear();
            string filter=textBoxSentencePatternPartFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsSentencePatternParts.Count; i++) {
                    ItemSentencePatternPart item = itemsSentencePatternParts[i];

                    if (item.Filter(filter)) {
                        itemsSentencePatternPartsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsSentencePatternParts.Count; i++) {
                    ItemSentencePatternPart item = itemsSentencePatternParts[i];
                    itemsSentencePatternPartsFiltered.Add(item);
                }
            }
        }

        void ButtonSentencePatternPartAdd_Click(object sender, EventArgs e) {
            AddNewItemSentencePatternPart();
        }

        void ButtonSentencePatternPartRemove_Click(object sender, EventArgs e) {
            RemoveItemSentencePatternPart(CurrentSentencePatternPart);
            TextBoxSentencePatternPartFilter_TextChanged(null, new EventArgs());
        }

        //void SetListBoxSentencePatternPart() {
        //    string filter=textBoxFilterSentencePatternPart.Text;
        //    bool useFilter = filter!="" && filter!="*";

        //    int index=listBoxSentencePatternParts.SelectedIndex;
        //    listBoxSentencePatternParts.Items.Clear();
        //    foreach (ItemSentencePatternPart item in itemsSentencePatternParts) {
        //        if (useFilter) {
        //            if (!item.Filter(filter)) {
        //                continue;
        //            }
        //        }
        //        string textToAdd=item.GetText();
        //        if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

        //        listBoxSentencePatternParts.Items.Add(textToAdd);
        //    }
        //    listBoxSentencePatternParts.SelectedIndex=index;
        //}
        void SetListBoxSentencePatternPart() {
            //string filter=textBoxSentencePatternPartFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxSentencePatternPart.SelectedIndex;
            listBoxSentencePatternPart.Items.Clear();
            for (int i=0; i<itemsSentencePatternPartsFiltered.Count; i++) {
                ItemSentencePatternPart item = itemsSentencePatternPartsFiltered[i];

                string textToAdd=item.GetText();
           //     if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePatternPart.Items.Add(textToAdd);
            }

            if (index>=listBoxSentencePatternPart.Items.Count)index=listBoxSentencePatternPart.Items.Count-1;
            listBoxSentencePatternPart.SelectedIndex=index;
        }

        void AddNewItemSentencePatternPart() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSentencePatternPart();

            var newItem=new ItemSentencePatternPart();
            itemsSentencePatternParts.Add(newItem);
            CurrentSentencePatternPart=newItem;

            //SetListBoxSentencePatternPart();
            //ListBoxSetCurrentSentencePatternPart();
            //SetCurrentSentencePatternPart();

            SentencePatternPartRefreshFilteredList();
            SetListBoxSentencePatternPart();
            ListBoxSetCurrentSentencePatternPart();
            SetCurrentSentencePatternPart();

            doingJob=false;
        }

        void RemoveItemSentencePatternPart(ItemSentencePatternPart item) {
            Edited=true;
            ChangeCaptionText();
            itemsSentencePatternParts.Remove(item);
            SetListBoxSentencePatternPart();
            SetCurrentSentencePatternPart();
        }

        void SetCurrentSentencePatternPart(){
            if (itemsSentencePatternPartsFiltered.Count==0) {
                SetNoneSentencePatternPart();
                return;
            }

            int index=listBoxSentencePatternPart.SelectedIndex;
            if (index>itemsSentencePatternPartsFiltered.Count) index=itemsSentencePatternPartsFiltered.Count-1;
            if (index<0) index=0;
            CurrentSentencePatternPart=itemsSentencePatternPartsFiltered[index];
          //  throw new Exception();

            textBoxSentencePatternPartFrom.Text=CurrentSentencePatternPart.From;
            textBoxSentencePatternPartTo.Text=CurrentSentencePatternPart.To;
            textBoxSentencePatternPartTo.Visible=true;
            textBoxSentencePatternPartFrom.Visible=true;
            //labelSentencePatternPartInfo.Visible=true;
            labelSentencePatternPartFrom.Visible=true;
            labelSentencePatternPartTo.Visible=true;
        }

        void ListBoxSetCurrentSentencePatternPart() {
            for (int indexCur=0; indexCur<itemsSentencePatternPartsFiltered.Count; indexCur++) {
                if (itemsSentencePatternParts[indexCur]==CurrentSentencePatternPart) {
                    int indexList=listBoxSentencePatternPart.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSentencePatternPart.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentSentencePatternPart() {
            if (CurrentSentencePatternPart==null) return;

            CurrentSentencePatternPart.From=textBoxSentencePatternPartFrom.Text;
            CurrentSentencePatternPart.To=textBoxSentencePatternPartTo.Text;
        }

        void RemoveCurrentSentencePatternPart(object sender, EventArgs e) {
            itemsSentencePatternParts.Remove(CurrentSentencePatternPart);
        }

        void ListBoxSentencePatternPart_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSentencePatternPart();

            int index=listBoxSentencePatternPart.SelectedIndex;
            if (itemsSentencePatternParts.Count==0) {
                SetNoneSentencePatternPart();
                return;
            }
            if (index>=itemsSentencePatternParts.Count)  index=itemsSentencePatternParts.Count-1;
            if (index<0) index=0;

            CurrentSentencePatternPart=itemsSentencePatternParts[index];
            SetCurrentSentencePatternPart();
            SetListBoxSentencePatternPart();
          //  SetCurrent();
            doingJob=false;
        }

        void SetNoneSentencePatternPart() {
            textBoxSentencePatternPartFrom.Text="";
            textBoxSentencePatternPartTo.Text="";
            textBoxSentencePatternPartTo.Visible=false;
            textBoxSentencePatternPartFrom.Visible=false;
            //labelSentencePatternPartInfo.Visible=false;
            labelSentencePatternPartFrom.Visible=false;
            labelSentencePatternPartTo.Visible=false;
        }

        //void ClearSentencePatternPart(){
        //    listBoxSentencePatternPart.Items.Clear();
        //    SetNoneSentencePatternPart();
        //    itemsSentencePatternPartsFiltered?.Clear();
        //    itemsSentencePatternParts?.Clear();
        //    CurrentSentencePatternPart=null;
        //}
        #endregion

        #region SentencePart
        void TextBoxSentencePartFilter_TextChanged(object sender, EventArgs e) {
              SaveCurrentSentencePart();

            // Získej aktuální prvek
            ItemSentencePart selectedId=null;
            if (listBoxSentencePart.SelectedIndex!=-1) {
                selectedId=itemsSentencePartsFiltered[listBoxSentencePart.SelectedIndex];
            }

            SentencePartRefreshFilteredList();

            listBoxSentencePart.Items.Clear();
            for (int i=0; i<itemsSentencePartsFiltered.Count; i++) {
                ItemSentencePart item = itemsSentencePartsFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePart.Items.Add(textToAdd);
            }

            //SetListBoxSimpleWord();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsSentencePartsFiltered.Count; i++){
                    if (itemsSentencePartsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxSentencePart.SelectedIndex=-1;
                    CurrentSentencePart=null;
                } else listBoxSentencePart.SelectedIndex=outIndex;
            } else {
                listBoxSentencePart.SelectedIndex=-1;
                CurrentSentencePart=null;
            }
            SetCurrentSentencePart();
          //  SetListBoxSentencePart();
        }

        void SentencePartRefreshFilteredList() {
            if (itemsSentencePartsFiltered==null) itemsSentencePartsFiltered=new List<ItemSentencePart>();
            itemsSentencePartsFiltered.Clear();
            string filter=textBoxSentencePartFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsSentenceParts.Count; i++) {
                    ItemSentencePart item = itemsSentenceParts[i];

                    if (item.Filter(filter)) {
                        itemsSentencePartsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsSentenceParts.Count; i++) {
                    ItemSentencePart item = itemsSentenceParts[i];
                    itemsSentencePartsFiltered.Add(item);
                }
            }
        }

        void ButtonSentencePartAdd_Click(object sender, EventArgs e) {
            AddNewItemSentencePart();
        }

        void ButtonSentencePartRemove_Click(object sender, EventArgs e) {
            RemoveItemSentencePart(CurrentSentencePart);
            TextBoxSentencePartFilter_TextChanged(null, new EventArgs());
        }

        //void SetListBoxSentencePart() {
        //    string filter=textBoxFilterSentencePart.Text;
        //    bool useFilter = filter!="" && filter!="*";

        //    int index=listBoxSentenceParts.SelectedIndex;
        //    listBoxSentenceParts.Items.Clear();
        //    foreach (ItemSentencePart item in itemsSentenceParts) {
        //        if (useFilter) {
        //            if (!item.Filter(filter)) {
        //                continue;
        //            }
        //        }
        //        string textToAdd=item.GetText();
        //        if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

        //        listBoxSentenceParts.Items.Add(textToAdd);
        //    }
        //    listBoxSentenceParts.SelectedIndex=index;
        //}
        void SetListBoxSentencePart() {
            //string filter=textBoxSentencePartFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxSentencePart.SelectedIndex;
            listBoxSentencePart.Items.Clear();
            for (int i=0; i<itemsSentencePartsFiltered.Count; i++) {
                ItemSentencePart item = itemsSentencePartsFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePart.Items.Add(textToAdd);
            }

            if (index>=listBoxSentencePart.Items.Count)index=listBoxSentencePart.Items.Count-1;
            listBoxSentencePart.SelectedIndex=index;
        }

        void AddNewItemSentencePart() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSentencePart();

            var newItem=new ItemSentencePart();
            newItem.To=new List<TranslatingToData>{ new TranslatingToData()};
            itemsSentenceParts.Add(newItem);
            CurrentSentencePart=newItem;

            //SetListBoxSentencePart();
            //ListBoxSetCurrentSentencePart();
            //SetCurrentSentencePart();

            SentencePartRefreshFilteredList();
            SetListBoxSentencePart();
            ListBoxSetCurrentSentencePart();
            SetCurrentSentencePart();

            doingJob=false;
        }

        void RemoveItemSentencePart(ItemSentencePart item) {
            Edited=true;
            ChangeCaptionText();
            itemsSentenceParts.Remove(item);
            SetListBoxSentencePart();
            SetCurrentSentencePart();
        }

        void SetCurrentSentencePart(){
            if (itemsSentencePartsFiltered.Count==0) {
                SentencePartSetNone();
                return;
            }

            int index=listBoxSentencePart.SelectedIndex;
            if (index>itemsSentencePartsFiltered.Count) index=itemsSentencePartsFiltered.Count-1;
            if (index<0) index=0;
            CurrentSentencePart=itemsSentencePartsFiltered[index];
          //  throw new Exception();

            textBoxSentencePartFrom.Text=CurrentSentencePart.From;
           // textBoxSentencePartTo.Text=CurrentSentencePart.To;
            //textBoxSentencePartTo.Visible=true;
            textBoxSentencePartFrom.Visible=true;
            //labelSentencePartInfo.Visible=true;
            labelSentencePartFrom.Visible=true;
           // labelSentencePartTo.Visible=true;
            checkBoxSentencePart.Visible=true;
            checkBoxSentencePart.Checked=CurrentSentencePart.Show;
            //buttonAddToSentencePart.Visible=true;
            simpleUISentencePart.Visible=true;
            simpleUISentencePart.SetData(CurrentSentencePart.To.ToArray());
        }

        void ListBoxSetCurrentSentencePart() {
            for (int indexCur=0; indexCur<itemsSentencePartsFiltered.Count; indexCur++) {
                if (itemsSentenceParts[indexCur]==CurrentSentencePart) {
                    int indexList=listBoxSentencePart.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSentencePart.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentSentencePart() {
            if (CurrentSentencePart==null) return;

            CurrentSentencePart.From=textBoxSentencePartFrom.Text;
          //  CurrentSentencePart.To=textBoxSentencePartTo.Text;
            CurrentSentencePart.Show=checkBoxSentencePart.Checked;
            CurrentSentencePart.To=simpleUISentencePart.GetData().ToList();
        }

        void RemoveCurrentSentencePart(object sender, EventArgs e) {
            itemsSentenceParts.Remove(CurrentSentencePart);
        }

        void ListBoxSentencePart_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSentencePart();

            int index=listBoxSentencePart.SelectedIndex;
            if (itemsSentenceParts.Count==0) {
                SentencePartSetNone();
                return;
            }
            if (index>=itemsSentenceParts.Count)  index=itemsSentenceParts.Count-1;
            if (index<0) index=0;

            CurrentSentencePart=itemsSentenceParts[index];
            SetCurrentSentencePart();
            SetListBoxSentencePart();
          //  SetCurrent();
            doingJob=false;
        }

        void SentencePartSetNone() {
            textBoxSentencePartFrom.Text="";
            //textBoxSentencePartTo.Text="";
            //textBoxSentencePartTo.Visible=false;
            textBoxSentencePartFrom.Visible=false;
            //labelSentencePartInfo.Visible=false;
            labelSentencePartFrom.Visible=false;
          //  labelSentencePartTo.Visible=false;
            //buttonAddToSentencePart.Visible=false;
            simpleUISentencePart.Visible=false;
            checkBoxSentencePart.Visible=false;
            simpleUISentencePart.Clear();
        }

        #endregion

        #region NounPatternFrom
        void ListBoxPatternNounFrom_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternNounFromSaveCurrent();

            int index=listBoxPatternNounFrom.SelectedIndex;
            if (itemsPatternNounFrom.Count==0) {
                PatternNounFromSetNone();
                return;
            }
            if (index>=itemsPatternNounFrom.Count)
                index=itemsPatternNounFrom.Count-1;
            if (index<0)
                index=0;

            CurrentPatternNounFrom=itemsPatternNounFrom[index];
            PatternNounFromSetCurrent();
            PatternNounFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonPatternNounFromAdd_Click(object sender, EventArgs e) {
            PatternNounFromAddNewItem();
        }

        void ButtonPatternNounFromRemove_Click(object sender, EventArgs e) {
            PatternNounFromRemoveItem(CurrentPatternNounFrom);
            TextBoxPatternNounFromFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxPatternNounFromFilter_TextChanged(object sender, EventArgs e) {
            PatternNounFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternNoun selectedId=null;
            if (listBoxPatternNounFrom.SelectedIndex!=-1) {
                selectedId=itemsPatternNounFromFiltered[listBoxPatternNounFrom.SelectedIndex];
            }

            PatternNounFromRefreshFilteredList();

            listBoxPatternNounFrom.Items.Clear();
            for (int i=0; i<itemsPatternNounFromFiltered.Count; i++) {
                ItemPatternNoun item = itemsPatternNounFromFiltered[i];

                string textToAdd=item.GetText();
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternNounFrom.Items.Add(textToAdd);
            }

            //NounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternNounFromFiltered.Count; i++){
                    if (itemsPatternNounFromFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1) {
                    listBoxPatternNounFrom.SelectedIndex=-1;
                    CurrentPatternNounFrom=null;
                } else listBoxPatternNounFrom.SelectedIndex=outIndex;
            } else {
                listBoxPatternNounFrom.SelectedIndex=-1;
                CurrentPatternNounFrom=null;
            }
            PatternNounFromSetCurrent();
        }

        void PatternNounFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternNounFrom.Remove(CurrentPatternNounFrom);
        }

        void PatternNounFromSetListBox() {
            //string filter=textBoxPatternNounFromFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxPatternNounFrom.SelectedIndex;

            listBoxPatternNounFrom.SuspendLayout();
            listBoxPatternNounFrom.Items.Clear();
            for (int i=0; i<itemsPatternNounFromFiltered.Count; i++) {
                ItemPatternNoun item = itemsPatternNounFromFiltered[i];

                string textToAdd=item.GetText();
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternNounFrom.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternNounFrom.Items.Count)index=listBoxPatternNounFrom.Items.Count-1;
            listBoxPatternNounFrom.SelectedIndex=index;
            listBoxPatternNounFrom.ResumeLayout();
        }

        void PatternNounFromRefreshFilteredList() {
            if (itemsPatternNounFromFiltered==null) itemsPatternNounFromFiltered=new List<ItemPatternNoun>();
            itemsPatternNounFromFiltered.Clear();
            string filter=textBoxPatternNounFromFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternNounFrom.Count; i++) {
                    ItemPatternNoun item = itemsPatternNounFrom[i];

                    if (item.Filter(filter)) {
                        itemsPatternNounFromFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternNounFrom.Count; i++) {
                    ItemPatternNoun item = itemsPatternNounFrom[i];
                    itemsPatternNounFromFiltered.Add(item);
                }
            }
        }

        void PatternNounFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternNounFromSaveCurrent();

            var newItem=new ItemPatternNoun();
           // newItem.ID=itemsNouns.Count;
            itemsPatternNounFrom.Add(newItem);
            CurrentPatternNounFrom=newItem;
            PatternNounFromRefreshFilteredList();
            PatternNounFromSetListBox();
            PatternNounFromListBoxSetCurrent();
            PatternNounFromSetCurrent();

            doingJob=false;
        }

        void PatternNounFromRemoveItem(ItemPatternNoun item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternNounFrom.Remove(item);
            PatternNounFromRefreshFilteredList();
            PatternNounFromSetListBox();
            PatternNounFromSetCurrent();
        }

        void PatternNounFromSetCurrent(){
            if (itemsPatternNounFromFiltered.Count==0) {
                PatternNounFromSetNone();
                return;
            }

            int index=listBoxPatternNounFrom.SelectedIndex;
            if (index>=itemsPatternNounFromFiltered.Count) index=itemsPatternNounFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternNounFrom=itemsPatternNounFromFiltered[index];
            textBoxPatternNounFromName.Text=CurrentPatternNounFrom.Name;

            switch (CurrentPatternNounFrom.Gender){
                case GenderNoun.Neuter:
                    comboBoxPatternNounFromGender.SelectedIndex=1;
                    break;

                case GenderNoun.Feminine:
                    comboBoxPatternNounFromGender.SelectedIndex=2;
                    break;

                case GenderNoun.MasculineAnimal:
                    comboBoxPatternNounFromGender.SelectedIndex=3;
                    break;

                case GenderNoun.MasculineInanimate:
                   comboBoxPatternNounFromGender.SelectedIndex=4;
                   break;

                case GenderNoun.Unknown:
                    comboBoxPatternNounFromGender.SelectedIndex=0;
                   break;
            }
//            <Nenastaveno>
//Střední
//Ženský
//Mužský živ
//Mužský než

            //(int)CurrentPatternNounFrom.Gender;

            textBoxPatternNounFromJ1.Text=CurrentPatternNounFrom.Shapes[0];
            textBoxPatternNounFromJ2.Text=CurrentPatternNounFrom.Shapes[1];
            textBoxPatternNounFromJ3.Text=CurrentPatternNounFrom.Shapes[2];
            textBoxPatternNounFromJ4.Text=CurrentPatternNounFrom.Shapes[3];
            textBoxPatternNounFromJ5.Text=CurrentPatternNounFrom.Shapes[4];
            textBoxPatternNounFromJ6.Text=CurrentPatternNounFrom.Shapes[5];
            textBoxPatternNounFromJ7.Text=CurrentPatternNounFrom.Shapes[6];

            textBoxPatternNounFromM1.Text=CurrentPatternNounFrom.Shapes[7];
            textBoxPatternNounFromM2.Text=CurrentPatternNounFrom.Shapes[8];
            textBoxPatternNounFromM3.Text=CurrentPatternNounFrom.Shapes[9];
            textBoxPatternNounFromM4.Text=CurrentPatternNounFrom.Shapes[10];
            textBoxPatternNounFromM5.Text=CurrentPatternNounFrom.Shapes[11];
            textBoxPatternNounFromM6.Text=CurrentPatternNounFrom.Shapes[12];
            textBoxPatternNounFromM7.Text=CurrentPatternNounFrom.Shapes[13];

            labelPatternNounFromFall.Visible=true;
            labelPatternNounFromInfo.Visible=true;
            labelPatternNounFromMultiple.Visible=true;
            labelPatternNounFromGender.Visible=true;

            labelPatternNounFromSingle.Visible=true;
            labelPatternNounFromName.Visible=true;
            tableLayoutPanelPatternNounFrom.Visible=true;

            textBoxPatternNounFromName.Visible=true;
            comboBoxPatternNounFromGender.Visible=true;
        }

        void PatternNounFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternNounFromFiltered.Count; indexCur++) {
                if (itemsPatternNounFrom[indexCur]==CurrentPatternNounFrom) {
                    int indexList=listBoxPatternNounFrom.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternNounFrom.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternNounFromSaveCurrent() {
            if (CurrentPatternNounFrom==null) return;

            CurrentPatternNounFrom.Name=textBoxPatternNounFromName.Text;
            int index=comboBoxPatternNounFromGender.SelectedIndex;

            if (index==1) CurrentPatternNounFrom.Gender=GenderNoun.Neuter;
            else if (index==2) CurrentPatternNounFrom.Gender=GenderNoun.Feminine;
            else if (index==3) CurrentPatternNounFrom.Gender=GenderNoun.MasculineAnimal;
            else if (index==4) CurrentPatternNounFrom.Gender=GenderNoun.MasculineInanimate;
            else /*if (index<0)*/ CurrentPatternNounFrom.Gender=GenderNoun.Unknown;

            CurrentPatternNounFrom.Shapes[0]=textBoxPatternNounFromJ1.Text;
            CurrentPatternNounFrom.Shapes[1]=textBoxPatternNounFromJ2.Text;
            CurrentPatternNounFrom.Shapes[2]=textBoxPatternNounFromJ3.Text;
            CurrentPatternNounFrom.Shapes[3]=textBoxPatternNounFromJ4.Text;
            CurrentPatternNounFrom.Shapes[4]=textBoxPatternNounFromJ5.Text;
            CurrentPatternNounFrom.Shapes[5]=textBoxPatternNounFromJ6.Text;
            CurrentPatternNounFrom.Shapes[6]=textBoxPatternNounFromJ7.Text;

            CurrentPatternNounFrom.Shapes[7]=textBoxPatternNounFromM1.Text;
            CurrentPatternNounFrom.Shapes[8]=textBoxPatternNounFromM2.Text;
            CurrentPatternNounFrom.Shapes[9]=textBoxPatternNounFromM3.Text;
            CurrentPatternNounFrom.Shapes[10]=textBoxPatternNounFromM4.Text;
            CurrentPatternNounFrom.Shapes[11]=textBoxPatternNounFromM5.Text;
            CurrentPatternNounFrom.Shapes[12]=textBoxPatternNounFromM6.Text;
            CurrentPatternNounFrom.Shapes[13]=textBoxPatternNounFromM7.Text;
        }

        void PatternNounFromSetNone(){
            textBoxPatternNounFromName.Text="";
            comboBoxPatternNounFromGender.SelectedIndex=0;

            textBoxPatternNounFromJ1.Text="";
            textBoxPatternNounFromJ2.Text="";
            textBoxPatternNounFromJ3.Text="";
            textBoxPatternNounFromJ4.Text="";
            textBoxPatternNounFromJ5.Text="";
            textBoxPatternNounFromJ6.Text="";
            textBoxPatternNounFromJ7.Text="";

            textBoxPatternNounFromM1.Text="";
            textBoxPatternNounFromM2.Text="";
            textBoxPatternNounFromM3.Text="";
            textBoxPatternNounFromM4.Text="";
            textBoxPatternNounFromM5.Text="";
            textBoxPatternNounFromM6.Text="";
            textBoxPatternNounFromM7.Text="";
            textBoxPatternNounFromName.Visible=false;
            comboBoxPatternNounFromGender.Visible=false;
            labelPatternNounFromFall.Visible=false;
            labelPatternNounFromInfo.Visible=false;
            labelPatternNounFromMultiple.Visible=false;
            labelPatternNounFromGender.Visible=false;

            labelPatternNounFromSingle.Visible=false;
            labelPatternNounFromName.Visible=false;
            tableLayoutPanelPatternNounFrom.Visible=false;
        }

        void PatternNounFromClear() {
            listBoxPatternNounFrom.Items.Clear();
            PatternNounFromSetNone();
            itemsPatternNounFromFiltered?.Clear();
            itemsPatternNounFrom?.Clear();
            CurrentPatternNounFrom=null;
        }
        #endregion

        #region NounPatternTo
        void ListBoxPatternNounTo_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternNounToSaveCurrent();

            int index=listBoxPatternNounTo.SelectedIndex;
            if (itemsPatternNounTo.Count==0) {
                PatternNounToSetNone();
                return;
            }
            if (index>=itemsPatternNounTo.Count)
                index=itemsPatternNounTo.Count-1;
            if (index<0)
                index=0;

            CurrentPatternNounTo=itemsPatternNounTo[index];
            PatternNounToSetCurrent();
            PatternNounToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonPatternNounToAdd_Click(object sender, EventArgs e) {
            PatternNounToAddNewItem();
        }

        void ButtonPatternNounToRemove_Click(object sender, EventArgs e) {
            PatternNounToRemoveItem(CurrentPatternNounTo);
            TextBoxPatternNounToFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxPatternNounToFilter_TextChanged(object sender, EventArgs e) {
            PatternNounToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternNoun selectedId=null;
            if (listBoxPatternNounTo.SelectedIndex!=-1) {
                selectedId=itemsPatternNounToFiltered[listBoxPatternNounTo.SelectedIndex];
            }

            PatternNounToRefreshFilteredList();

            listBoxPatternNounTo.Items.Clear();
            for (int i=0; i<itemsPatternNounToFiltered.Count; i++) {
                ItemPatternNoun item = itemsPatternNounToFiltered[i];

                string textToAdd=item.GetText();
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternNounTo.Items.Add(textToAdd);
            }

            //NounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternNounToFiltered.Count; i++){
                    if (itemsPatternNounToFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1) {
                    listBoxPatternNounTo.SelectedIndex=-1;
                    CurrentPatternNounTo=null;
                } else listBoxPatternNounTo.SelectedIndex=outIndex;
            } else {
                listBoxPatternNounTo.SelectedIndex=-1;
                CurrentPatternNounTo=null;
            }
            PatternNounToSetCurrent();
        }

        void PatternNounToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternNounTo.Remove(CurrentPatternNounTo);
        }

        void PatternNounToSetListBox() {

            int index=listBoxPatternNounTo.SelectedIndex;
            listBoxPatternNounTo.Items.Clear();
            for (int i=0; i<itemsPatternNounToFiltered.Count; i++) {
                ItemPatternNoun item = itemsPatternNounToFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternNounTo.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternNounTo.Items.Count)index=listBoxPatternNounTo.Items.Count-1;
            listBoxPatternNounTo.SelectedIndex=index;
            //listBoxPatternNounTo.AutoScrollOffset=p;
        }

        void PatternNounToRefreshFilteredList() {
            if (itemsPatternNounToFiltered==null) itemsPatternNounToFiltered=new List<ItemPatternNoun>();
            itemsPatternNounToFiltered.Clear();
            string filter=textBoxPatternNounToFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternNounTo.Count; i++) {
                    ItemPatternNoun item = itemsPatternNounTo[i];

                    if (item.Filter(filter)) {
                        itemsPatternNounToFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternNounTo.Count; i++) {
                    ItemPatternNoun item = itemsPatternNounTo[i];
                    itemsPatternNounToFiltered.Add(item);
                }
            }
        }

        void PatternNounToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternNounToSaveCurrent();

            var newItem=new ItemPatternNoun();
           // newItem.ID=itemsNouns.Count;
            itemsPatternNounTo.Add(newItem);
            CurrentPatternNounTo=newItem;
            PatternNounToRefreshFilteredList();
            PatternNounToSetListBox();
            PatternNounToListBoxSetCurrent();
            PatternNounToSetCurrent();

            doingJob=false;
        }

        void PatternNounToRemoveItem(ItemPatternNoun item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternNounTo.Remove(item);
            PatternNounToRefreshFilteredList();
            PatternNounToSetListBox();
            PatternNounToSetCurrent();
        }

        void PatternNounToSetCurrent(){
            if (itemsPatternNounToFiltered.Count==0) {
                PatternNounToSetNone();
                return;
            }

            int index=listBoxPatternNounTo.SelectedIndex;
            if (index>=itemsPatternNounToFiltered.Count) index=itemsPatternNounToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternNounTo=itemsPatternNounToFiltered[index];
            textBoxPatternNounToName.Text=CurrentPatternNounTo.Name;


            if (CurrentPatternNounTo.Gender==GenderNoun.Neuter) comboBoxPatternNounToGender.SelectedIndex=1;
            else if (CurrentPatternNounTo.Gender==GenderNoun.Feminine) comboBoxPatternNounToGender.SelectedIndex=2;
            else if (CurrentPatternNounTo.Gender==GenderNoun.MasculineAnimal) comboBoxPatternNounToGender.SelectedIndex=3;
            else if (CurrentPatternNounTo.Gender==GenderNoun.MasculineInanimate) comboBoxPatternNounToGender.SelectedIndex=4;
            else /*if (CurrentPatternNounTo.Gender==GenderNoun.Unknown)*/ comboBoxPatternNounToGender.SelectedIndex=0;

            textBoxPatternNounToS1.Text=CurrentPatternNounTo.Shapes[0];
            textBoxPatternNounToS2.Text=CurrentPatternNounTo.Shapes[1];
            textBoxPatternNounToS3.Text=CurrentPatternNounTo.Shapes[2];
            textBoxPatternNounToS4.Text=CurrentPatternNounTo.Shapes[3];
            textBoxPatternNounToS5.Text=CurrentPatternNounTo.Shapes[4];
            textBoxPatternNounToS6.Text=CurrentPatternNounTo.Shapes[5];
            textBoxPatternNounToS7.Text=CurrentPatternNounTo.Shapes[6];

            textBoxPatternNounToM1.Text=CurrentPatternNounTo.Shapes[7];
            textBoxPatternNounToM2.Text=CurrentPatternNounTo.Shapes[8];
            textBoxPatternNounToM3.Text=CurrentPatternNounTo.Shapes[9];
            textBoxPatternNounToM4.Text=CurrentPatternNounTo.Shapes[10];
            textBoxPatternNounToM5.Text=CurrentPatternNounTo.Shapes[11];
            textBoxPatternNounToM6.Text=CurrentPatternNounTo.Shapes[12];
            textBoxPatternNounToM7.Text=CurrentPatternNounTo.Shapes[13];

            labelPatternNounToFall.Visible=true;
            labelPatternNounToInfo.Visible=true;
            labelPatternNounToMultiple.Visible=true;
            labelPatternNounToGender.Visible=true;

            labelPatternNounToSingle.Visible=true;
            labelPatternNounToName.Visible=true;
            tableLayoutPanelPatternNounTo.Visible=true;

            textBoxPatternNounToName.Visible=true;
            comboBoxPatternNounToGender.Visible=true;
        }

        void PatternNounToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternNounToFiltered.Count; indexCur++) {
                if (itemsPatternNounTo[indexCur]==CurrentPatternNounTo) {
                    int indexList=listBoxPatternNounTo.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternNounTo.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternNounToSaveCurrent() {
            if (CurrentPatternNounTo==null) return;

            CurrentPatternNounTo.Name=textBoxPatternNounToName.Text;

            if (comboBoxPatternNounToGender.SelectedIndex==1)CurrentPatternNounTo.Gender=GenderNoun.Neuter;
            else if (comboBoxPatternNounToGender.SelectedIndex==2)CurrentPatternNounTo.Gender=GenderNoun.Feminine;
            else if (comboBoxPatternNounToGender.SelectedIndex==3)CurrentPatternNounTo.Gender=GenderNoun.MasculineAnimal;
            else if (comboBoxPatternNounToGender.SelectedIndex==4)CurrentPatternNounTo.Gender=GenderNoun.MasculineInanimate;
            else /*if (comboBoxPatternNounToGender.SelectedIndex==0)*/CurrentPatternNounTo.Gender=GenderNoun.Unknown;

            CurrentPatternNounTo.Shapes[0]=textBoxPatternNounToS1.Text;
            CurrentPatternNounTo.Shapes[1]=textBoxPatternNounToS2.Text;
            CurrentPatternNounTo.Shapes[2]=textBoxPatternNounToS3.Text;
            CurrentPatternNounTo.Shapes[3]=textBoxPatternNounToS4.Text;
            CurrentPatternNounTo.Shapes[4]=textBoxPatternNounToS5.Text;
            CurrentPatternNounTo.Shapes[5]=textBoxPatternNounToS6.Text;
            CurrentPatternNounTo.Shapes[6]=textBoxPatternNounToS7.Text;

            CurrentPatternNounTo.Shapes[ 7]=textBoxPatternNounToM1.Text;
            CurrentPatternNounTo.Shapes[ 8]=textBoxPatternNounToM2.Text;
            CurrentPatternNounTo.Shapes[ 9]=textBoxPatternNounToM3.Text;
            CurrentPatternNounTo.Shapes[10]=textBoxPatternNounToM4.Text;
            CurrentPatternNounTo.Shapes[11]=textBoxPatternNounToM5.Text;
            CurrentPatternNounTo.Shapes[12]=textBoxPatternNounToM6.Text;
            CurrentPatternNounTo.Shapes[13]=textBoxPatternNounToM7.Text;
        }

        void PatternNounToSetNone(){
            textBoxPatternNounToName.Text="";
            comboBoxPatternNounToGender.SelectedIndex=0;

            textBoxPatternNounToS1.Text="";
            textBoxPatternNounToS2.Text="";
            textBoxPatternNounToS3.Text="";
            textBoxPatternNounToS4.Text="";
            textBoxPatternNounToS5.Text="";
            textBoxPatternNounToS6.Text="";
            textBoxPatternNounToS7.Text="";

            textBoxPatternNounToM1.Text="";
            textBoxPatternNounToM2.Text="";
            textBoxPatternNounToM3.Text="";
            textBoxPatternNounToM4.Text="";
            textBoxPatternNounToM5.Text="";
            textBoxPatternNounToM6.Text="";
            textBoxPatternNounToM7.Text="";
            textBoxPatternNounToName.Visible=false;
            comboBoxPatternNounToGender.Visible=false;
            labelPatternNounToFall.Visible=false;
            labelPatternNounToInfo.Visible=false;
            labelPatternNounToMultiple.Visible=false;
            labelPatternNounToGender.Visible=false;

            labelPatternNounToSingle.Visible=false;
            labelPatternNounToName.Visible=false;
            tableLayoutPanelPatternNounTo.Visible=false;
        }

        void PatternNounToClear() {
            listBoxPatternNounTo.Items.Clear();
            PatternNounToSetNone();
            itemsPatternNounToFiltered?.Clear();
            itemsPatternNounTo?.Clear();
            CurrentPatternNounTo=null;
        }
        #endregion

        #region Noun
        void ListBoxNoun_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentNoun();

            int index=listBoxNoun.SelectedIndex;
            if (itemsNouns.Count==0) {
                NounSetNone();
                return;
            }
            if (index>=itemsNouns.Count)
                index=itemsNouns.Count-1;
            if (index<0)
                index=0;

            CurrentNoun=itemsNouns[index];
            SetCurrentNoun();
            NounSetListBox();
            doingJob=false;
        }

        void ButtonNounAdd_Click(object sender, EventArgs e) {
            AddNewItemNoun();
        }

        void ButtonNounRemove_Click(object sender, EventArgs e) {
            RemoveItemNoun(CurrentNoun);
            TextBoxNounFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxNounFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentNoun();
            
            DrawingControl.SuspendDrawing(listBoxNoun);
            // Získej aktuální prvek
            ItemNoun selectedId=null;
            if (listBoxNoun.SelectedIndex!=-1) {
                selectedId=itemsNounsFiltered[listBoxNoun.SelectedIndex];
            }

            NounRefreshFilteredList();

            listBoxNoun.Items.Clear();
            for (int i=0; i<itemsNounsFiltered.Count; i++) {
                ItemNoun item = itemsNounsFiltered[i];

                string textToAdd=item.GetText(itemsPatternNounFrom.Cast<ItemTranslatingPattern>().ToList(), itemsPatternNounTo.Cast<ItemTranslatingPattern>().ToList());

                //if (string.IsNullOrEmpty(textToAdd)) {
                //    if (string.IsNullOrEmpty(item.PatternFrom)){
                //        textToAdd="<Neznámé>";
                //    } else textToAdd="{"+item.PatternFrom+"}";
                //}

                listBoxNoun.Items.Add(textToAdd);
            }

            //NounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsNounsFiltered.Count; i++){
                    if (itemsNounsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxNoun.SelectedIndex=-1;
                    CurrentNoun=null;
                } else listBoxNoun.SelectedIndex=outIndex;
            } else {
                listBoxNoun.SelectedIndex=-1;
                CurrentNoun=null;
            }
            SetCurrentNoun();
            
            DrawingControl.ResumeDrawing(listBoxNoun);
        }

        void RemoveCurrentNoun(object sender, EventArgs e) {
            itemsNouns.Remove(CurrentNoun);
        }

        void NounSetListBox() {
            int index=listBoxNoun.SelectedIndex;
            listBoxNoun.Items.Clear();
            for (int i=0; i<itemsNounsFiltered.Count; i++) {
                ItemNoun item = itemsNounsFiltered[i];

                string textToAdd=item.GetText(itemsPatternNounFrom.Cast<ItemTranslatingPattern>().ToList(), itemsPatternNounTo.Cast<ItemTranslatingPattern>().ToList());
                if (string.IsNullOrEmpty(textToAdd)) {
                    if (string.IsNullOrEmpty(item.PatternFrom)) {
                        textToAdd="<Neznámé>";
                    } else textToAdd="{"+item.PatternFrom+"}";
                }
                listBoxNoun.Items.Add(textToAdd);
            }

            if (index>=listBoxNoun.Items.Count)index=listBoxNoun.Items.Count-1;
            listBoxNoun.SelectedIndex=index;
        }

        void NounRefreshFilteredList() {
            if (itemsNounsFiltered==null) itemsNounsFiltered=new List<ItemNoun>();
            itemsNounsFiltered.Clear();
            string filter=textBoxNounFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsNouns.Count; i++) {
                    ItemNoun item = itemsNouns[i];

                    if (item.Filter(filter)) {
                        itemsNounsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsNouns.Count; i++) {
                    ItemNoun item = itemsNouns[i];
                    itemsNounsFiltered.Add(item);
                }
            }
        }

        void AddNewItemNoun() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentNoun();

            var newItem=new ItemNoun();
           // newItem.ID=itemsNouns.Count;
           newItem.To.Add(new TranslatingToDataWithPattern{Body="", Pattern="", Comment=""});
            itemsNouns.Add(newItem);
            CurrentNoun=newItem;
            NounRefreshFilteredList();
            NounSetListBox();
            NounListBoxSetCurrent();
            SetCurrentNoun();

            doingJob=false;
        }

        void RemoveItemNoun(ItemNoun item) {
            Edited=true;
            ChangeCaptionText();
            itemsNouns.Remove(item);
            NounRefreshFilteredList();
            NounSetListBox();
            SetCurrentNoun();
        }

        void SetCurrentNoun(){
            if (itemsNounsFiltered.Count==0) {
                NounSetNone();
                return;
            }

            int index=listBoxNoun.SelectedIndex;
            if (index>=itemsNounsFiltered.Count) index=itemsNounsFiltered.Count-1;
            if (index<0) index=0;
            CurrentNoun=itemsNounsFiltered[index];
            textBoxNounFrom.Visible=true;
            comboBoxNounType.Visible=true;
            labelNounFrom.Visible=true;
            //textBoxNounComment.Visible=true;
            //textBoxNounComment.Text=CurrentNoun.Comment;
            comboBoxNounType.SelectedIndex=(int)CurrentNoun.wordUpperCaseType;

            textBoxNounFrom.Text=CurrentNoun.From;

            if (CurrentNoun.PatternFrom=="") {
                comboBoxNounInputPatternFrom.SelectedIndex=-1;
                comboBoxNounInputPatternFrom.Text="";
            } else {
                comboBoxNounInputPatternFrom.Text=CurrentNoun.PatternFrom;
            }

            comboBoxNounInputPatternFrom.Items.Clear();

            foreach (ItemPatternNoun x in itemsPatternNounFrom) {
                comboBoxNounInputPatternFrom.Items.Add(x.Name);
            }

            simpleUINouns.Visible=true;
            simpleUINouns.SetData(CurrentNoun.To.ToArray());
            List<string> options=new List<string>();
            foreach (ItemPatternNoun x in itemsPatternNounTo) options.Add(x.Name);
            simpleUINouns.SetComboboxes(options.ToArray());

            comboBoxNounInputPatternFrom.Visible=true;
            labelNounInputPatternFrom.Visible=true;
            labelNounShowFrom.Visible=true;
            //buttonNounAddTo.Visible=true;
        }

        void NounListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsNounsFiltered.Count; indexCur++) {
                if (itemsNouns[indexCur]==CurrentNoun) {
                    int indexList=listBoxNoun.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxNoun.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentNoun() {
            if (CurrentNoun==null) return;
            Edited=true;

            CurrentNoun.From=textBoxNounFrom.Text;
            CurrentNoun.To=simpleUINouns.GetData().ToList();
            CurrentNoun.PatternFrom=comboBoxNounInputPatternFrom.Text;
            CurrentNoun.wordUpperCaseType = (WordUpperCaseType)comboBoxNounType.SelectedIndex;
        }

        void NounSetNone(){
            textBoxNounFrom.Text="";
            simpleUINouns.Visible=false;
            simpleUINouns.Clear();
            comboBoxNounType.Visible=false;
            comboBoxNounType.SelectedIndex=-1;
            comboBoxNounInputPatternFrom.SelectedIndex=-1;
            comboBoxNounType.Visible=false;
            textBoxNounFrom.Visible=false;
            labelNounFrom.Visible=false;
            comboBoxNounInputPatternFrom.Visible=false;
            labelNounShowFrom.Visible=false;
            labelNounInputPatternFrom.Visible=false;
            //buttonNounAddTo.Visible=false;
        }

        void ClearNoun(){
            listBoxNoun.Items.Clear();
            NounSetNone();
            itemsNounsFiltered?.Clear();
            itemsNouns?.Clear();
            CurrentNoun=null;

            comboBoxNounType.SelectedIndex=-1;
        }
        #endregion

        #region PatternAdjectiveFrom
        void PatternAdjectiveFromListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternAdjectiveFromSaveCurrent();

            int index=listBoxPatternAdjectiveFrom.SelectedIndex;
            if (itemsPatternAdjectiveFrom.Count==0) {
                PatternAdjectiveFromSetNone();
                return;
            }
            if (index>=itemsPatternAdjectiveFrom.Count)
                index=itemsPatternAdjectiveFrom.Count-1;
            if (index<0)
                index=0;

            CurrentPatternAdjectiveFrom=itemsPatternAdjectiveFrom[index];
            PatternAdjectiveFromSetCurrent();
            PatternAdjectiveFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternAdjectiveFromButtonAdd_Click(object sender, EventArgs e) {
            PatternAdjectiveFromAddNewItem();
        }

        void PatternAdjectiveFromButtonRemove_Click(object sender, EventArgs e) {
            PatternAdjectiveFromRemoveItem(CurrentPatternAdjectiveFrom);
            PatternAdjectiveFromTextBoxFilter_TextChanged(null, new EventArgs());
        }

        void PatternAdjectiveFromTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternAdjectiveFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternAdjective selectedId=null;
            if (listBoxPatternAdjectiveFrom.SelectedIndex!=-1) {
                selectedId=itemsPatternAdjectivesFromFiltered[listBoxPatternAdjectiveFrom.SelectedIndex];
            }

            PatternAdjectiveFromRefreshFilteredList();

            listBoxPatternAdjectiveFrom.Items.Clear();
            for (int i=0; i<itemsPatternAdjectivesFromFiltered.Count; i++) {
                ItemPatternAdjective item = itemsPatternAdjectivesFromFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternAdjectiveFrom.Items.Add(textToAdd);
            }

            //SetListBoxAdjective();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternAdjectivesFromFiltered.Count; i++){
                    if (itemsPatternAdjectivesFromFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxPatternAdjectiveFrom.SelectedIndex=-1;
                    CurrentPatternAdjectiveFrom=null;
                } else listBoxPatternAdjectiveFrom.SelectedIndex=outIndex;
            } else {
                listBoxPatternAdjectiveFrom.SelectedIndex=-1;
                CurrentPatternAdjectiveFrom=null;
            }
            PatternAdjectiveFromSetCurrent();
        }

        void PatternAdjectiveFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternAdjectiveFrom.Remove(CurrentPatternAdjectiveFrom);
        }

        void PatternAdjectiveFromSetListBox() {
            //string filter=textBoxPatternAdjectiveFromFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxPatternAdjectiveFrom.SelectedIndex;
            listBoxPatternAdjectiveFrom.Items.Clear();
            for (int i=0; i<itemsPatternAdjectivesFromFiltered.Count; i++) {
                ItemPatternAdjective item = itemsPatternAdjectivesFromFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternAdjectiveFrom.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternAdjectiveFrom.Items.Count)index=listBoxPatternAdjectiveFrom.Items.Count-1;
            listBoxPatternAdjectiveFrom.SelectedIndex=index;
        }

        void PatternAdjectiveFromRefreshFilteredList() {
            if (itemsPatternAdjectivesFromFiltered==null) itemsPatternAdjectivesFromFiltered=new List<ItemPatternAdjective>();
            itemsPatternAdjectivesFromFiltered.Clear();
            string filter=textBoxPatternAdjectiveFromFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternAdjectiveFrom.Count; i++) {
                    ItemPatternAdjective item = itemsPatternAdjectiveFrom[i];

                    if (item.Filter(filter)) {
                        itemsPatternAdjectivesFromFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternAdjectiveFrom.Count; i++) {
                    ItemPatternAdjective item = itemsPatternAdjectiveFrom[i];
                    itemsPatternAdjectivesFromFiltered.Add(item);
                }
            }
        }

        void PatternAdjectiveFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternAdjectiveFromSaveCurrent();

            var newItem=new ItemPatternAdjective();
           // newItem.ID=itemsAdjectives.Count;
            itemsPatternAdjectiveFrom.Add(newItem);
            CurrentPatternAdjectiveFrom=newItem;
            PatternAdjectiveFromRefreshFilteredList();
            PatternAdjectiveFromSetListBox();
            PatternAdjectiveFromListBoxSetCurrent();
            PatternAdjectiveFromSetCurrent();

            doingJob=false;
        }

        void PatternAdjectiveFromRemoveItem(ItemPatternAdjective item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternAdjectiveFrom.Remove(item);
            PatternAdjectiveFromRefreshFilteredList();
            PatternAdjectiveFromSetListBox();
            PatternAdjectiveFromSetCurrent();
        }

        void PatternAdjectiveFromSetCurrent(){
            if (itemsPatternAdjectivesFromFiltered.Count==0) {
                PatternAdjectiveFromSetNone();
                return;
            }

            int index=listBoxPatternAdjectiveFrom.SelectedIndex;
            if (index>=itemsPatternAdjectivesFromFiltered.Count) index=itemsPatternAdjectivesFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternAdjectiveFrom=itemsPatternAdjectivesFromFiltered[index];
            textBoxPatternAdjectiveFromName.Text=CurrentPatternAdjectiveFrom.Name;
            textBoxPatternAdjectiveFromStrJ1.Text=CurrentPatternAdjectiveFrom.Middle[0];
            textBoxPatternAdjectiveFromStrJ2.Text=CurrentPatternAdjectiveFrom.Middle[1];
            textBoxPatternAdjectiveFromStrJ3.Text=CurrentPatternAdjectiveFrom.Middle[2];
            textBoxPatternAdjectiveFromStrJ4.Text=CurrentPatternAdjectiveFrom.Middle[3];
            textBoxPatternAdjectiveFromStrJ5.Text=CurrentPatternAdjectiveFrom.Middle[4];
            textBoxPatternAdjectiveFromStrJ6.Text=CurrentPatternAdjectiveFrom.Middle[5];
            textBoxPatternAdjectiveFromStrJ7.Text=CurrentPatternAdjectiveFrom.Middle[6];
            textBoxPatternAdjectiveFromStrJN.Text=CurrentPatternAdjectiveFrom.Middle[7];
            textBoxPatternAdjectiveFromStrJA.Text=CurrentPatternAdjectiveFrom.Middle[8];

            textBoxPatternAdjectiveFromStrM1.Text=CurrentPatternAdjectiveFrom.Middle[9];
            textBoxPatternAdjectiveFromStrM2.Text=CurrentPatternAdjectiveFrom.Middle[10];
            textBoxPatternAdjectiveFromStrM3.Text=CurrentPatternAdjectiveFrom.Middle[11];
            textBoxPatternAdjectiveFromStrM4.Text=CurrentPatternAdjectiveFrom.Middle[12];
            textBoxPatternAdjectiveFromStrM5.Text=CurrentPatternAdjectiveFrom.Middle[13];
            textBoxPatternAdjectiveFromStrM6.Text=CurrentPatternAdjectiveFrom.Middle[14];
            textBoxPatternAdjectiveFromStrM7.Text=CurrentPatternAdjectiveFrom.Middle[15];
            textBoxPatternAdjectiveFromStrMN.Text=CurrentPatternAdjectiveFrom.Middle[16];
            textBoxPatternAdjectiveFromStrMA.Text=CurrentPatternAdjectiveFrom.Middle[17];

            textBoxPatternAdjectiveFromZenJ1.Text=CurrentPatternAdjectiveFrom.Feminine[0];
            textBoxPatternAdjectiveFromZenJ2.Text=CurrentPatternAdjectiveFrom.Feminine[1];
            textBoxPatternAdjectiveFromZenJ3.Text=CurrentPatternAdjectiveFrom.Feminine[2];
            textBoxPatternAdjectiveFromZenJ4.Text=CurrentPatternAdjectiveFrom.Feminine[3];
            textBoxPatternAdjectiveFromZenJ5.Text=CurrentPatternAdjectiveFrom.Feminine[4];
            textBoxPatternAdjectiveFromZenJ6.Text=CurrentPatternAdjectiveFrom.Feminine[5];
            textBoxPatternAdjectiveFromZenJ7.Text=CurrentPatternAdjectiveFrom.Feminine[6];
            textBoxPatternAdjectiveFromZenJN.Text=CurrentPatternAdjectiveFrom.Feminine[7];
            textBoxPatternAdjectiveFromZenJA.Text=CurrentPatternAdjectiveFrom.Feminine[8];

            textBoxPatternAdjectiveFromZenM1.Text=CurrentPatternAdjectiveFrom.Feminine[9];
            textBoxPatternAdjectiveFromZenM2.Text=CurrentPatternAdjectiveFrom.Feminine[10];
            textBoxPatternAdjectiveFromZenM3.Text=CurrentPatternAdjectiveFrom.Feminine[11];
            textBoxPatternAdjectiveFromZenM4.Text=CurrentPatternAdjectiveFrom.Feminine[12];
            textBoxPatternAdjectiveFromZenM5.Text=CurrentPatternAdjectiveFrom.Feminine[13];
            textBoxPatternAdjectiveFromZenM6.Text=CurrentPatternAdjectiveFrom.Feminine[14];
            textBoxPatternAdjectiveFromZenM7.Text=CurrentPatternAdjectiveFrom.Feminine[15];
            textBoxPatternAdjectiveFromZenMN.Text=CurrentPatternAdjectiveFrom.Feminine[16];
            textBoxPatternAdjectiveFromZenMA.Text=CurrentPatternAdjectiveFrom.Feminine[17];

            textBoxPatternAdjectiveFromMuzJ1.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[0];
            textBoxPatternAdjectiveFromMuzJ2.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[1];
            textBoxPatternAdjectiveFromMuzJ3.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[2];
            textBoxPatternAdjectiveFromMuzJ4.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[3];
            textBoxPatternAdjectiveFromMuzJ5.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[4];
            textBoxPatternAdjectiveFromMuzJ6.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[5];
            textBoxPatternAdjectiveFromMuzJ7.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[6];
            textBoxPatternAdjectiveFromMuzJN.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[7];
            textBoxPatternAdjectiveFromMuzJA.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[8];

            textBoxPatternAdjectiveFromMuzM1.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[9];
            textBoxPatternAdjectiveFromMuzM2.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[10];
            textBoxPatternAdjectiveFromMuzM3.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[11];
            textBoxPatternAdjectiveFromMuzM4.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[12];
            textBoxPatternAdjectiveFromMuzM5.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[13];
            textBoxPatternAdjectiveFromMuzM6.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[14];
            textBoxPatternAdjectiveFromMuzM7.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[15];
            textBoxPatternAdjectiveFromMuzMN.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[16];
            textBoxPatternAdjectiveFromMuzMA.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[17];

            textBoxPatternAdjectiveFromMunJ1.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[0];
            textBoxPatternAdjectiveFromMunJ2.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[1];
            textBoxPatternAdjectiveFromMunJ3.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[2];
            textBoxPatternAdjectiveFromMunJ4.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[3];
            textBoxPatternAdjectiveFromMunJ5.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[4];
            textBoxPatternAdjectiveFromMunJ6.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[5];
            textBoxPatternAdjectiveFromMunJ7.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[6];
            textBoxPatternAdjectiveFromMunJN.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[7];
            textBoxPatternAdjectiveFromMunJA.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[8];

            textBoxPatternAdjectiveFromMunM1.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[9];
            textBoxPatternAdjectiveFromMunM2.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[10];
            textBoxPatternAdjectiveFromMunM3.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[11];
            textBoxPatternAdjectiveFromMunM4.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[12];
            textBoxPatternAdjectiveFromMunM5.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[13];
            textBoxPatternAdjectiveFromMunM6.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[14];
            textBoxPatternAdjectiveFromMunM7.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[15];
            textBoxPatternAdjectiveFromMunMN.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[16];
            textBoxPatternAdjectiveFromMunMA.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[17];


            textBoxPatternAdjectiveFromName.Visible=true;
            tableLayoutPanelPatternAdjectiveFromStr.Visible=true;
            labelPatternAdjectiveFromStrFall.Visible=true;
            labelPatternAdjectiveFromStrMultiple.Visible=true;
            labelPatternAdjectiveFromStrSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromMuz.Visible=true;
            labelPatternAdjectiveFromMuzFall.Visible=true;
            labelPatternAdjectiveFromMuzMultiple.Visible=true;
            labelPatternAdjectiveFromMuzSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromMun.Visible=true;
            labelPatternAdjectiveFromMunFall.Visible=true;
            labelPatternAdjectiveFromMunMultiple.Visible=true;
            labelPatternAdjectiveFromMunSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromZen.Visible=true;
            labelPatternAdjectiveFromZenFall.Visible=true;
            labelPatternAdjectiveFromZenMultiple.Visible=true;
            labelPatternAdjectiveFromZenSingle.Visible=true;

            labelPatternAdjectiveFromName.Visible=true;
            //labelPatternAdjectiveType.Visible=true;
           // comboBoxPatternAdjectiveFromType.Visible=true;
            labelPatternAdjectiveFromStr.Visible=true;
            labelPatternAdjectiveFromZen.Visible=true;
            labelPatternAdjectiveFromMuz.Visible=true;
            labelPatternAdjectiveFromMun.Visible=true;
          //  comboBoxPatternAdjectiveFromType.SelectedIndex=(int)CurrentPatternAdjectiveFrom.adjectiveType;
        }

        void PatternAdjectiveFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternAdjectivesFromFiltered.Count; indexCur++) {
                if (itemsPatternAdjectiveFrom[indexCur]==CurrentPatternAdjectiveFrom) {
                    int indexList=listBoxPatternAdjectiveFrom.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternAdjectiveFrom.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternAdjectiveFromSaveCurrent() {
            if (CurrentPatternAdjectiveFrom==null) return;
            Edited=true;
            CurrentPatternAdjectiveFrom.Name=textBoxPatternAdjectiveFromName.Text;

            CurrentPatternAdjectiveFrom.Middle[0]=textBoxPatternAdjectiveFromStrJ1.Text;
            CurrentPatternAdjectiveFrom.Middle[1]=textBoxPatternAdjectiveFromStrJ2.Text;
            CurrentPatternAdjectiveFrom.Middle[2]=textBoxPatternAdjectiveFromStrJ3.Text;
            CurrentPatternAdjectiveFrom.Middle[3]=textBoxPatternAdjectiveFromStrJ4.Text;
            CurrentPatternAdjectiveFrom.Middle[4]=textBoxPatternAdjectiveFromStrJ5.Text;
            CurrentPatternAdjectiveFrom.Middle[5]=textBoxPatternAdjectiveFromStrJ6.Text;
            CurrentPatternAdjectiveFrom.Middle[6]=textBoxPatternAdjectiveFromStrJ7.Text;
            CurrentPatternAdjectiveFrom.Middle[7]=textBoxPatternAdjectiveFromStrJN.Text;
            CurrentPatternAdjectiveFrom.Middle[8]=textBoxPatternAdjectiveFromStrJA.Text;

            CurrentPatternAdjectiveFrom.Middle[ 9]=textBoxPatternAdjectiveFromStrM1.Text;
            CurrentPatternAdjectiveFrom.Middle[10]=textBoxPatternAdjectiveFromStrM2.Text;
            CurrentPatternAdjectiveFrom.Middle[11]=textBoxPatternAdjectiveFromStrM3.Text;
            CurrentPatternAdjectiveFrom.Middle[12]=textBoxPatternAdjectiveFromStrM4.Text;
            CurrentPatternAdjectiveFrom.Middle[13]=textBoxPatternAdjectiveFromStrM5.Text;
            CurrentPatternAdjectiveFrom.Middle[14]=textBoxPatternAdjectiveFromStrM6.Text;
            CurrentPatternAdjectiveFrom.Middle[15]=textBoxPatternAdjectiveFromStrM7.Text;
            CurrentPatternAdjectiveFrom.Middle[16]=textBoxPatternAdjectiveFromStrMN.Text;
            CurrentPatternAdjectiveFrom.Middle[17]=textBoxPatternAdjectiveFromStrMA.Text;

            CurrentPatternAdjectiveFrom.Feminine[0]=textBoxPatternAdjectiveFromZenJ1.Text;
            CurrentPatternAdjectiveFrom.Feminine[1]=textBoxPatternAdjectiveFromZenJ2.Text;
            CurrentPatternAdjectiveFrom.Feminine[2]=textBoxPatternAdjectiveFromZenJ3.Text;
            CurrentPatternAdjectiveFrom.Feminine[3]=textBoxPatternAdjectiveFromZenJ4.Text;
            CurrentPatternAdjectiveFrom.Feminine[4]=textBoxPatternAdjectiveFromZenJ5.Text;
            CurrentPatternAdjectiveFrom.Feminine[5]=textBoxPatternAdjectiveFromZenJ6.Text;
            CurrentPatternAdjectiveFrom.Feminine[6]=textBoxPatternAdjectiveFromZenJ7.Text;
            CurrentPatternAdjectiveFrom.Feminine[7]=textBoxPatternAdjectiveFromZenJN.Text;
            CurrentPatternAdjectiveFrom.Feminine[8]=textBoxPatternAdjectiveFromZenJA.Text;

            CurrentPatternAdjectiveFrom.Feminine[ 9]=textBoxPatternAdjectiveFromZenM1.Text;
            CurrentPatternAdjectiveFrom.Feminine[10]=textBoxPatternAdjectiveFromZenM2.Text;
            CurrentPatternAdjectiveFrom.Feminine[11]=textBoxPatternAdjectiveFromZenM3.Text;
            CurrentPatternAdjectiveFrom.Feminine[12]=textBoxPatternAdjectiveFromZenM4.Text;
            CurrentPatternAdjectiveFrom.Feminine[13]=textBoxPatternAdjectiveFromZenM5.Text;
            CurrentPatternAdjectiveFrom.Feminine[14]=textBoxPatternAdjectiveFromZenM6.Text;
            CurrentPatternAdjectiveFrom.Feminine[15]=textBoxPatternAdjectiveFromZenM7.Text;
            CurrentPatternAdjectiveFrom.Feminine[16]=textBoxPatternAdjectiveFromZenMN.Text;
            CurrentPatternAdjectiveFrom.Feminine[17]=textBoxPatternAdjectiveFromZenMA.Text;

            CurrentPatternAdjectiveFrom.MasculineAnimate[0]=textBoxPatternAdjectiveFromMuzJ1.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[1]=textBoxPatternAdjectiveFromMuzJ2.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[2]=textBoxPatternAdjectiveFromMuzJ3.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[3]=textBoxPatternAdjectiveFromMuzJ4.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[4]=textBoxPatternAdjectiveFromMuzJ5.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[5]=textBoxPatternAdjectiveFromMuzJ6.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[6]=textBoxPatternAdjectiveFromMuzJ7.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[7]=textBoxPatternAdjectiveFromMuzJN.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[8]=textBoxPatternAdjectiveFromMuzJA.Text;

            CurrentPatternAdjectiveFrom.MasculineAnimate[ 9]=textBoxPatternAdjectiveFromMuzM1.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[10]=textBoxPatternAdjectiveFromMuzM2.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[11]=textBoxPatternAdjectiveFromMuzM3.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[12]=textBoxPatternAdjectiveFromMuzM4.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[13]=textBoxPatternAdjectiveFromMuzM5.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[14]=textBoxPatternAdjectiveFromMuzM6.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[15]=textBoxPatternAdjectiveFromMuzM7.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[16]=textBoxPatternAdjectiveFromMuzMN.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[17]=textBoxPatternAdjectiveFromMuzMA.Text;

            CurrentPatternAdjectiveFrom.MasculineInanimate[0]=textBoxPatternAdjectiveFromMunJ1.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[1]=textBoxPatternAdjectiveFromMunJ2.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[2]=textBoxPatternAdjectiveFromMunJ3.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[3]=textBoxPatternAdjectiveFromMunJ4.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[4]=textBoxPatternAdjectiveFromMunJ5.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[5]=textBoxPatternAdjectiveFromMunJ6.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[6]=textBoxPatternAdjectiveFromMunJ7.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[7]=textBoxPatternAdjectiveFromMunJN.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[8]=textBoxPatternAdjectiveFromMunJA.Text;

            CurrentPatternAdjectiveFrom.MasculineInanimate[ 9]=textBoxPatternAdjectiveFromMunM1.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[10]=textBoxPatternAdjectiveFromMunM2.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[11]=textBoxPatternAdjectiveFromMunM3.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[12]=textBoxPatternAdjectiveFromMunM4.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[13]=textBoxPatternAdjectiveFromMunM5.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[14]=textBoxPatternAdjectiveFromMunM6.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[15]=textBoxPatternAdjectiveFromMunM7.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[16]=textBoxPatternAdjectiveFromMunMN.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[17]=textBoxPatternAdjectiveFromMunMA.Text;
        }

        void PatternAdjectiveFromSetNone(){
            textBoxPatternAdjectiveFromName.Text="";

            textBoxPatternAdjectiveFromStrJ1.Text="";
            textBoxPatternAdjectiveFromStrJ2.Text="";
            textBoxPatternAdjectiveFromStrJ3.Text="";
            textBoxPatternAdjectiveFromStrJ4.Text="";
            textBoxPatternAdjectiveFromStrJ5.Text="";
            textBoxPatternAdjectiveFromStrJ6.Text="";
            textBoxPatternAdjectiveFromStrJ7.Text="";
            textBoxPatternAdjectiveFromStrJN.Text="";
            textBoxPatternAdjectiveFromStrJA.Text="";

            textBoxPatternAdjectiveFromStrM1.Text="";
            textBoxPatternAdjectiveFromStrM2.Text="";
            textBoxPatternAdjectiveFromStrM3.Text="";
            textBoxPatternAdjectiveFromStrM4.Text="";
            textBoxPatternAdjectiveFromStrM5.Text="";
            textBoxPatternAdjectiveFromStrM6.Text="";
            textBoxPatternAdjectiveFromStrM7.Text="";
            textBoxPatternAdjectiveFromStrMN.Text="";
            textBoxPatternAdjectiveFromStrMA.Text="";

            textBoxPatternAdjectiveFromZenJ1.Text="";
            textBoxPatternAdjectiveFromZenJ2.Text="";
            textBoxPatternAdjectiveFromZenJ3.Text="";
            textBoxPatternAdjectiveFromZenJ4.Text="";
            textBoxPatternAdjectiveFromZenJ5.Text="";
            textBoxPatternAdjectiveFromZenJ6.Text="";
            textBoxPatternAdjectiveFromZenJ7.Text="";
            textBoxPatternAdjectiveFromZenJN.Text="";
            textBoxPatternAdjectiveFromZenJA.Text="";

            textBoxPatternAdjectiveFromZenM1.Text="";
            textBoxPatternAdjectiveFromZenM2.Text="";
            textBoxPatternAdjectiveFromZenM3.Text="";
            textBoxPatternAdjectiveFromZenM4.Text="";
            textBoxPatternAdjectiveFromZenM5.Text="";
            textBoxPatternAdjectiveFromZenM6.Text="";
            textBoxPatternAdjectiveFromZenM7.Text="";
            textBoxPatternAdjectiveFromZenMN.Text="";
            textBoxPatternAdjectiveFromZenMA.Text="";

            textBoxPatternAdjectiveFromMuzJ1.Text="";
            textBoxPatternAdjectiveFromMuzJ2.Text="";
            textBoxPatternAdjectiveFromMuzJ3.Text="";
            textBoxPatternAdjectiveFromMuzJ4.Text="";
            textBoxPatternAdjectiveFromMuzJ5.Text="";
            textBoxPatternAdjectiveFromMuzJ6.Text="";
            textBoxPatternAdjectiveFromMuzJ7.Text="";
            textBoxPatternAdjectiveFromMuzJN.Text="";
            textBoxPatternAdjectiveFromMuzJA.Text="";

            textBoxPatternAdjectiveFromMuzM1.Text="";
            textBoxPatternAdjectiveFromMuzM2.Text="";
            textBoxPatternAdjectiveFromMuzM3.Text="";
            textBoxPatternAdjectiveFromMuzM4.Text="";
            textBoxPatternAdjectiveFromMuzM5.Text="";
            textBoxPatternAdjectiveFromMuzM6.Text="";
            textBoxPatternAdjectiveFromMuzM7.Text="";
            textBoxPatternAdjectiveFromMuzMN.Text="";
            textBoxPatternAdjectiveFromMuzMA.Text="";

            textBoxPatternAdjectiveFromMunJ1.Text="";
            textBoxPatternAdjectiveFromMunJ2.Text="";
            textBoxPatternAdjectiveFromMunJ3.Text="";
            textBoxPatternAdjectiveFromMunJ4.Text="";
            textBoxPatternAdjectiveFromMunJ5.Text="";
            textBoxPatternAdjectiveFromMunJ6.Text="";
            textBoxPatternAdjectiveFromMunJ7.Text="";
            textBoxPatternAdjectiveFromMunJN.Text="";
            textBoxPatternAdjectiveFromMunJA.Text="";

            textBoxPatternAdjectiveFromMunM1.Text="";
            textBoxPatternAdjectiveFromMunM2.Text="";
            textBoxPatternAdjectiveFromMunM3.Text="";
            textBoxPatternAdjectiveFromMunM4.Text="";
            textBoxPatternAdjectiveFromMunM5.Text="";
            textBoxPatternAdjectiveFromMunM6.Text="";
            textBoxPatternAdjectiveFromMunM7.Text="";
            textBoxPatternAdjectiveFromMunMA.Text="";
            textBoxPatternAdjectiveFromMunMN.Text="";
            textBoxPatternAdjectiveFromName.Visible=false;
            labelPatternAdjectiveFromStr.Visible=false;
            tableLayoutPanelPatternAdjectiveFromStr.Visible=false;
            labelPatternAdjectiveFromStrFall.Visible=false;
            labelPatternAdjectiveFromStrMultiple.Visible=false;
            labelPatternAdjectiveFromStrSingle.Visible=false;

            tableLayoutPanelPatternAdjectiveFromMuz.Visible=false;
            labelPatternAdjectiveFromMuzFall.Visible=false;
            labelPatternAdjectiveFromMuzMultiple.Visible=false;
            labelPatternAdjectiveFromMuzSingle.Visible=false;
            labelPatternAdjectiveFromMuz.Visible=false;
            tableLayoutPanelPatternAdjectiveFromMun.Visible=false;
            labelPatternAdjectiveFromMunFall.Visible=false;
            labelPatternAdjectiveFromMunMultiple.Visible=false;
            labelPatternAdjectiveFromMunSingle.Visible=false;
            labelPatternAdjectiveFromZen.Visible=false;
            tableLayoutPanelPatternAdjectiveFromZen.Visible=false;
            labelPatternAdjectiveFromZenFall.Visible=false;
            labelPatternAdjectiveFromZenMultiple.Visible=false;
            labelPatternAdjectiveFromZenSingle.Visible=false;
            labelPatternAdjectiveFromMun.Visible=false;
            labelPatternAdjectiveFromName.Visible=false;
        }

        void PatternAdjectiveFromClear() {
            listBoxPatternAdjectiveFrom.Items.Clear();
            PatternAdjectiveFromSetNone();
            itemsPatternAdjectivesFromFiltered?.Clear();
            itemsPatternAdjectiveFrom?.Clear();
            CurrentPatternAdjectiveFrom=null;
        }
        #endregion

        #region PatternAdjectiveTo
        void PatternAdjectiveToListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternAdjectiveToSaveCurrent();

            int index=listBoxPatternAdjectiveTo.SelectedIndex;
            if (itemsPatternAdjectiveTo.Count==0) {
                PatternAdjectiveToSetNone();
                return;
            }
            if (index>=itemsPatternAdjectiveTo.Count)
                index=itemsPatternAdjectiveTo.Count-1;
            if (index<0)
                index=0;

            CurrentPatternAdjectiveTo=itemsPatternAdjectiveTo[index];
            PatternAdjectiveToSetCurrent();
            PatternAdjectiveToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternAdjectiveToButtonAdd_Click(object sender, EventArgs e) {
            PatternAdjectiveToAddNewItem();
        }

        void PatternAdjectiveToButtonRemove_Click(object sender, EventArgs e) {
            PatternAdjectiveToRemoveItem(CurrentPatternAdjectiveTo);
            PatternAdjectiveToTextBoxFilter_TextChanged(null, new EventArgs());
        }

        void PatternAdjectiveToTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternAdjectiveToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternAdjective selectedId=null;
            if (listBoxPatternAdjectiveTo.SelectedIndex!=-1) {
                selectedId=itemsPatternAdjectivesToFiltered[listBoxPatternAdjectiveTo.SelectedIndex];
            }

            PatternAdjectiveToRefreshFilteredList();

            listBoxPatternAdjectiveTo.Items.Clear();
            for (int i=0; i<itemsPatternAdjectivesToFiltered.Count; i++) {
                ItemPatternAdjective item = itemsPatternAdjectivesToFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternAdjectiveTo.Items.Add(textToAdd);
            }

            //SetListBoxAdjective();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternAdjectivesToFiltered.Count; i++){
                    if (itemsPatternAdjectivesToFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxPatternAdjectiveTo.SelectedIndex=-1;
                    CurrentPatternAdjectiveTo=null;
                } else listBoxPatternAdjectiveTo.SelectedIndex=outIndex;
            } else {
                listBoxPatternAdjectiveTo.SelectedIndex=-1;
                CurrentPatternAdjectiveTo=null;
            }
            PatternAdjectiveToSetCurrent();
        }

        void PatternAdjectiveToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternAdjectiveTo.Remove(CurrentPatternAdjectiveTo);
        }

        void PatternAdjectiveToSetListBox() {
            //string filter=textBoxPatternAdjectiveToFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxPatternAdjectiveTo.SelectedIndex;
            listBoxPatternAdjectiveTo.Items.Clear();
            for (int i=0; i<itemsPatternAdjectivesToFiltered.Count; i++) {
                ItemPatternAdjective item = itemsPatternAdjectivesToFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternAdjectiveTo.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternAdjectiveTo.Items.Count)index=listBoxPatternAdjectiveTo.Items.Count-1;
            listBoxPatternAdjectiveTo.SelectedIndex=index;
        }

        void PatternAdjectiveToRefreshFilteredList() {
            if (itemsPatternAdjectivesToFiltered==null) itemsPatternAdjectivesToFiltered=new List<ItemPatternAdjective>();
            itemsPatternAdjectivesToFiltered.Clear();
            string filter=textBoxPatternAdjectiveToFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternAdjectiveTo.Count; i++) {
                    ItemPatternAdjective item = itemsPatternAdjectiveTo[i];

                    if (item.Filter(filter)) {
                        itemsPatternAdjectivesToFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternAdjectiveTo.Count; i++) {
                    ItemPatternAdjective item = itemsPatternAdjectiveTo[i];
                    itemsPatternAdjectivesToFiltered.Add(item);
                }
            }
        }

        void PatternAdjectiveToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternAdjectiveToSaveCurrent();

            var newItem=new ItemPatternAdjective();
           // newItem.ID=itemsAdjectives.Count;
            itemsPatternAdjectiveTo.Add(newItem);
            CurrentPatternAdjectiveTo=newItem;
            PatternAdjectiveToRefreshFilteredList();
            PatternAdjectiveToSetListBox();
            PatternAdjectiveToListBoxSetCurrent();
            PatternAdjectiveToSetCurrent();

            doingJob=false;
        }

        void PatternAdjectiveToRemoveItem(ItemPatternAdjective item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternAdjectiveTo.Remove(item);
            PatternAdjectiveToRefreshFilteredList();
            PatternAdjectiveToSetListBox();
            PatternAdjectiveToSetCurrent();
        }

        void PatternAdjectiveToSetCurrent(){
            if (itemsPatternAdjectivesToFiltered.Count==0) {
                PatternAdjectiveToSetNone();
                return;
            }

            int index=listBoxPatternAdjectiveTo.SelectedIndex;
            if (index>=itemsPatternAdjectivesToFiltered.Count) index=itemsPatternAdjectivesToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternAdjectiveTo=itemsPatternAdjectivesToFiltered[index];
            textBoxPatternAdjectiveToName.Text=CurrentPatternAdjectiveTo.Name;
            textBoxPatternAdjectiveToStrJ1.Text=CurrentPatternAdjectiveTo.Middle[0];
            textBoxPatternAdjectiveToStrJ2.Text=CurrentPatternAdjectiveTo.Middle[1];
            textBoxPatternAdjectiveToStrJ3.Text=CurrentPatternAdjectiveTo.Middle[2];
            textBoxPatternAdjectiveToStrJ4.Text=CurrentPatternAdjectiveTo.Middle[3];
            textBoxPatternAdjectiveToStrJ5.Text=CurrentPatternAdjectiveTo.Middle[4];
            textBoxPatternAdjectiveToStrJ6.Text=CurrentPatternAdjectiveTo.Middle[5];
            textBoxPatternAdjectiveToStrJ7.Text=CurrentPatternAdjectiveTo.Middle[6];
            textBoxPatternAdjectiveToStrJN.Text=CurrentPatternAdjectiveTo.Middle[7];
            textBoxPatternAdjectiveToStrJA.Text=CurrentPatternAdjectiveTo.Middle[8];

            textBoxPatternAdjectiveToStrM1.Text=CurrentPatternAdjectiveTo.Middle[ 9];
            textBoxPatternAdjectiveToStrM2.Text=CurrentPatternAdjectiveTo.Middle[10];
            textBoxPatternAdjectiveToStrM3.Text=CurrentPatternAdjectiveTo.Middle[11];
            textBoxPatternAdjectiveToStrM4.Text=CurrentPatternAdjectiveTo.Middle[12];
            textBoxPatternAdjectiveToStrM5.Text=CurrentPatternAdjectiveTo.Middle[13];
            textBoxPatternAdjectiveToStrM6.Text=CurrentPatternAdjectiveTo.Middle[14];
            textBoxPatternAdjectiveToStrM7.Text=CurrentPatternAdjectiveTo.Middle[15];
            textBoxPatternAdjectiveToStrMN.Text=CurrentPatternAdjectiveTo.Middle[16];
            textBoxPatternAdjectiveToStrMA.Text=CurrentPatternAdjectiveTo.Middle[17];

            textBoxPatternAdjectiveToZenJ1.Text=CurrentPatternAdjectiveTo.Feminine[0];
            textBoxPatternAdjectiveToZenJ2.Text=CurrentPatternAdjectiveTo.Feminine[1];
            textBoxPatternAdjectiveToZenJ3.Text=CurrentPatternAdjectiveTo.Feminine[2];
            textBoxPatternAdjectiveToZenJ4.Text=CurrentPatternAdjectiveTo.Feminine[3];
            textBoxPatternAdjectiveToZenJ5.Text=CurrentPatternAdjectiveTo.Feminine[4];
            textBoxPatternAdjectiveToZenJ6.Text=CurrentPatternAdjectiveTo.Feminine[5];
            textBoxPatternAdjectiveToZenJ7.Text=CurrentPatternAdjectiveTo.Feminine[6];
            textBoxPatternAdjectiveToZenJN.Text=CurrentPatternAdjectiveTo.Feminine[7];
            textBoxPatternAdjectiveToZenJA.Text=CurrentPatternAdjectiveTo.Feminine[8];

            textBoxPatternAdjectiveToZenM1.Text=CurrentPatternAdjectiveTo.Feminine[ 9];
            textBoxPatternAdjectiveToZenM2.Text=CurrentPatternAdjectiveTo.Feminine[10];
            textBoxPatternAdjectiveToZenM3.Text=CurrentPatternAdjectiveTo.Feminine[11];
            textBoxPatternAdjectiveToZenM4.Text=CurrentPatternAdjectiveTo.Feminine[12];
            textBoxPatternAdjectiveToZenM5.Text=CurrentPatternAdjectiveTo.Feminine[13];
            textBoxPatternAdjectiveToZenM6.Text=CurrentPatternAdjectiveTo.Feminine[14];
            textBoxPatternAdjectiveToZenM7.Text=CurrentPatternAdjectiveTo.Feminine[15];
            textBoxPatternAdjectiveToZenMN.Text=CurrentPatternAdjectiveTo.Feminine[16];
            textBoxPatternAdjectiveToZenMA.Text=CurrentPatternAdjectiveTo.Feminine[17];

            textBoxPatternAdjectiveToMuzJ1.Text=CurrentPatternAdjectiveTo.MasculineAnimate[0];
            textBoxPatternAdjectiveToMuzJ2.Text=CurrentPatternAdjectiveTo.MasculineAnimate[1];
            textBoxPatternAdjectiveToMuzJ3.Text=CurrentPatternAdjectiveTo.MasculineAnimate[2];
            textBoxPatternAdjectiveToMuzJ4.Text=CurrentPatternAdjectiveTo.MasculineAnimate[3];
            textBoxPatternAdjectiveToMuzJ5.Text=CurrentPatternAdjectiveTo.MasculineAnimate[4];
            textBoxPatternAdjectiveToMuzJ6.Text=CurrentPatternAdjectiveTo.MasculineAnimate[5];
            textBoxPatternAdjectiveToMuzJ7.Text=CurrentPatternAdjectiveTo.MasculineAnimate[6];
            textBoxPatternAdjectiveToMuzJN.Text=CurrentPatternAdjectiveTo.MasculineAnimate[7];
            textBoxPatternAdjectiveToMuzJA.Text=CurrentPatternAdjectiveTo.MasculineAnimate[8];

            textBoxPatternAdjectiveToMuzM1.Text=CurrentPatternAdjectiveTo.MasculineAnimate[9];
            textBoxPatternAdjectiveToMuzM2.Text=CurrentPatternAdjectiveTo.MasculineAnimate[10];
            textBoxPatternAdjectiveToMuzM3.Text=CurrentPatternAdjectiveTo.MasculineAnimate[11];
            textBoxPatternAdjectiveToMuzM4.Text=CurrentPatternAdjectiveTo.MasculineAnimate[12];
            textBoxPatternAdjectiveToMuzM5.Text=CurrentPatternAdjectiveTo.MasculineAnimate[13];
            textBoxPatternAdjectiveToMuzM6.Text=CurrentPatternAdjectiveTo.MasculineAnimate[14];
            textBoxPatternAdjectiveToMuzM7.Text=CurrentPatternAdjectiveTo.MasculineAnimate[15];
            textBoxPatternAdjectiveToMuzMN.Text=CurrentPatternAdjectiveTo.MasculineAnimate[16];
            textBoxPatternAdjectiveToMuzMA.Text=CurrentPatternAdjectiveTo.MasculineAnimate[17];

            textBoxPatternAdjectiveToMunJ1.Text=CurrentPatternAdjectiveTo.MasculineInanimate[0];
            textBoxPatternAdjectiveToMunJ2.Text=CurrentPatternAdjectiveTo.MasculineInanimate[1];
            textBoxPatternAdjectiveToMunJ3.Text=CurrentPatternAdjectiveTo.MasculineInanimate[2];
            textBoxPatternAdjectiveToMunJ4.Text=CurrentPatternAdjectiveTo.MasculineInanimate[3];
            textBoxPatternAdjectiveToMunJ5.Text=CurrentPatternAdjectiveTo.MasculineInanimate[4];
            textBoxPatternAdjectiveToMunJ6.Text=CurrentPatternAdjectiveTo.MasculineInanimate[5];
            textBoxPatternAdjectiveToMunJ7.Text=CurrentPatternAdjectiveTo.MasculineInanimate[6];
            textBoxPatternAdjectiveToMunJN.Text=CurrentPatternAdjectiveTo.MasculineInanimate[7];
            textBoxPatternAdjectiveToMunJA.Text=CurrentPatternAdjectiveTo.MasculineInanimate[8];

            textBoxPatternAdjectiveToMunM1.Text=CurrentPatternAdjectiveTo.MasculineInanimate[9];
            textBoxPatternAdjectiveToMunM2.Text=CurrentPatternAdjectiveTo.MasculineInanimate[10];
            textBoxPatternAdjectiveToMunM3.Text=CurrentPatternAdjectiveTo.MasculineInanimate[11];
            textBoxPatternAdjectiveToMunM4.Text=CurrentPatternAdjectiveTo.MasculineInanimate[12];
            textBoxPatternAdjectiveToMunM5.Text=CurrentPatternAdjectiveTo.MasculineInanimate[13];
            textBoxPatternAdjectiveToMunM6.Text=CurrentPatternAdjectiveTo.MasculineInanimate[14];
            textBoxPatternAdjectiveToMunM7.Text=CurrentPatternAdjectiveTo.MasculineInanimate[15];
            textBoxPatternAdjectiveToMunMN.Text=CurrentPatternAdjectiveTo.MasculineInanimate[16];
            textBoxPatternAdjectiveToMunMA.Text=CurrentPatternAdjectiveTo.MasculineInanimate[17];


            textBoxPatternAdjectiveToName.Visible=true;
            tableLayoutPanelPatternAdjectiveToStr.Visible=true;
            labelPatternAdjectiveToStrFall.Visible=true;
            labelPatternAdjectiveToStrMultiple.Visible=true;
            labelPatternAdjectiveToStrSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveToMuz.Visible=true;
            labelPatternAdjectiveToMuzFall.Visible=true;
            labelPatternAdjectiveToMuzMultiple.Visible=true;
            labelPatternAdjectiveToMuzSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveToMun.Visible=true;
            labelPatternAdjectiveToMunFall.Visible=true;
            labelPatternAdjectiveToMunMultiple.Visible=true;
            labelPatternAdjectiveToMunSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveToZen.Visible=true;
            labelPatternAdjectiveToZenFall.Visible=true;
            labelPatternAdjectiveToZenMultiple.Visible=true;
            labelPatternAdjectiveToZenSingle.Visible=true;

            labelPatternAdjectiveToName.Visible=true;
            labelPatternAdjectiveToStr.Visible=true;
            labelPatternAdjectiveToZen.Visible=true;
            labelPatternAdjectiveToMuz.Visible=true;
            labelPatternAdjectiveToMun.Visible=true;
        }

        void PatternAdjectiveToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternAdjectivesToFiltered.Count; indexCur++) {
                if (itemsPatternAdjectiveTo[indexCur]==CurrentPatternAdjectiveTo) {
                    int indexList=listBoxPatternAdjectiveTo.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternAdjectiveTo.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternAdjectiveToSaveCurrent() {
            if (CurrentPatternAdjectiveTo==null) return;
                     Edited=true;
            CurrentPatternAdjectiveTo.Name=textBoxPatternAdjectiveToName.Text;

            CurrentPatternAdjectiveTo.Middle[0]=textBoxPatternAdjectiveToStrJ1.Text;
            CurrentPatternAdjectiveTo.Middle[1]=textBoxPatternAdjectiveToStrJ2.Text;
            CurrentPatternAdjectiveTo.Middle[2]=textBoxPatternAdjectiveToStrJ3.Text;
            CurrentPatternAdjectiveTo.Middle[3]=textBoxPatternAdjectiveToStrJ4.Text;
            CurrentPatternAdjectiveTo.Middle[4]=textBoxPatternAdjectiveToStrJ5.Text;
            CurrentPatternAdjectiveTo.Middle[5]=textBoxPatternAdjectiveToStrJ6.Text;
            CurrentPatternAdjectiveTo.Middle[6]=textBoxPatternAdjectiveToStrJ7.Text;
            CurrentPatternAdjectiveTo.Middle[7]=textBoxPatternAdjectiveToStrJN.Text;
            CurrentPatternAdjectiveTo.Middle[8]=textBoxPatternAdjectiveToStrJA.Text;

            CurrentPatternAdjectiveTo.Middle[ 9]=textBoxPatternAdjectiveToStrM1.Text;
            CurrentPatternAdjectiveTo.Middle[10]=textBoxPatternAdjectiveToStrM2.Text;
            CurrentPatternAdjectiveTo.Middle[11]=textBoxPatternAdjectiveToStrM3.Text;
            CurrentPatternAdjectiveTo.Middle[12]=textBoxPatternAdjectiveToStrM4.Text;
            CurrentPatternAdjectiveTo.Middle[13]=textBoxPatternAdjectiveToStrM5.Text;
            CurrentPatternAdjectiveTo.Middle[14]=textBoxPatternAdjectiveToStrM6.Text;
            CurrentPatternAdjectiveTo.Middle[15]=textBoxPatternAdjectiveToStrM7.Text;
            CurrentPatternAdjectiveTo.Middle[16]=textBoxPatternAdjectiveToStrMN.Text;
            CurrentPatternAdjectiveTo.Middle[17]=textBoxPatternAdjectiveToStrMA.Text;

            CurrentPatternAdjectiveTo.Feminine[0]=textBoxPatternAdjectiveToZenJ1.Text;
            CurrentPatternAdjectiveTo.Feminine[1]=textBoxPatternAdjectiveToZenJ2.Text;
            CurrentPatternAdjectiveTo.Feminine[2]=textBoxPatternAdjectiveToZenJ3.Text;
            CurrentPatternAdjectiveTo.Feminine[3]=textBoxPatternAdjectiveToZenJ4.Text;
            CurrentPatternAdjectiveTo.Feminine[4]=textBoxPatternAdjectiveToZenJ5.Text;
            CurrentPatternAdjectiveTo.Feminine[5]=textBoxPatternAdjectiveToZenJ6.Text;
            CurrentPatternAdjectiveTo.Feminine[6]=textBoxPatternAdjectiveToZenJ7.Text;
            CurrentPatternAdjectiveTo.Feminine[7]=textBoxPatternAdjectiveToZenJN.Text;
            CurrentPatternAdjectiveTo.Feminine[8]=textBoxPatternAdjectiveToZenJA.Text;

            CurrentPatternAdjectiveTo.Feminine[ 9]=textBoxPatternAdjectiveToZenM1.Text;
            CurrentPatternAdjectiveTo.Feminine[10]=textBoxPatternAdjectiveToZenM2.Text;
            CurrentPatternAdjectiveTo.Feminine[11]=textBoxPatternAdjectiveToZenM3.Text;
            CurrentPatternAdjectiveTo.Feminine[12]=textBoxPatternAdjectiveToZenM4.Text;
            CurrentPatternAdjectiveTo.Feminine[13]=textBoxPatternAdjectiveToZenM5.Text;
            CurrentPatternAdjectiveTo.Feminine[14]=textBoxPatternAdjectiveToZenM6.Text;
            CurrentPatternAdjectiveTo.Feminine[15]=textBoxPatternAdjectiveToZenM7.Text;
            CurrentPatternAdjectiveTo.Feminine[16]=textBoxPatternAdjectiveToZenMN.Text;
            CurrentPatternAdjectiveTo.Feminine[17]=textBoxPatternAdjectiveToZenMA.Text;

            CurrentPatternAdjectiveTo.MasculineAnimate[0]=textBoxPatternAdjectiveToMuzJ1.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[1]=textBoxPatternAdjectiveToMuzJ2.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[2]=textBoxPatternAdjectiveToMuzJ3.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[3]=textBoxPatternAdjectiveToMuzJ4.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[4]=textBoxPatternAdjectiveToMuzJ5.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[5]=textBoxPatternAdjectiveToMuzJ6.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[6]=textBoxPatternAdjectiveToMuzJ7.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[7]=textBoxPatternAdjectiveToMuzJN.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[8]=textBoxPatternAdjectiveToMuzJA.Text;

            CurrentPatternAdjectiveTo.MasculineAnimate[ 9]=textBoxPatternAdjectiveToMuzM1.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[10]=textBoxPatternAdjectiveToMuzM2.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[11]=textBoxPatternAdjectiveToMuzM3.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[12]=textBoxPatternAdjectiveToMuzM4.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[13]=textBoxPatternAdjectiveToMuzM5.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[14]=textBoxPatternAdjectiveToMuzM6.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[15]=textBoxPatternAdjectiveToMuzM7.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[16]=textBoxPatternAdjectiveToMuzMN.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[17]=textBoxPatternAdjectiveToMuzMA.Text;

            CurrentPatternAdjectiveTo.MasculineInanimate[0]=textBoxPatternAdjectiveToMunJ1.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[1]=textBoxPatternAdjectiveToMunJ2.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[2]=textBoxPatternAdjectiveToMunJ3.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[3]=textBoxPatternAdjectiveToMunJ4.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[4]=textBoxPatternAdjectiveToMunJ5.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[5]=textBoxPatternAdjectiveToMunJ6.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[6]=textBoxPatternAdjectiveToMunJ7.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[7]=textBoxPatternAdjectiveToMunJN.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[8]=textBoxPatternAdjectiveToMunJA.Text;

            CurrentPatternAdjectiveTo.MasculineInanimate[ 9]=textBoxPatternAdjectiveToMunM1.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[10]=textBoxPatternAdjectiveToMunM2.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[11]=textBoxPatternAdjectiveToMunM3.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[12]=textBoxPatternAdjectiveToMunM4.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[13]=textBoxPatternAdjectiveToMunM5.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[14]=textBoxPatternAdjectiveToMunM6.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[15]=textBoxPatternAdjectiveToMunM7.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[16]=textBoxPatternAdjectiveToMunMN.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[17]=textBoxPatternAdjectiveToMunMA.Text;
        }

        void PatternAdjectiveToSetNone(){
            textBoxPatternAdjectiveToName.Text="";

            textBoxPatternAdjectiveToStrJ1.Text="";
            textBoxPatternAdjectiveToStrJ2.Text="";
            textBoxPatternAdjectiveToStrJ3.Text="";
            textBoxPatternAdjectiveToStrJ4.Text="";
            textBoxPatternAdjectiveToStrJ5.Text="";
            textBoxPatternAdjectiveToStrJ6.Text="";
            textBoxPatternAdjectiveToStrJ7.Text="";
            textBoxPatternAdjectiveToStrJN.Text="";
            textBoxPatternAdjectiveToStrJA.Text="";

            textBoxPatternAdjectiveToStrM1.Text="";
            textBoxPatternAdjectiveToStrM2.Text="";
            textBoxPatternAdjectiveToStrM3.Text="";
            textBoxPatternAdjectiveToStrM4.Text="";
            textBoxPatternAdjectiveToStrM5.Text="";
            textBoxPatternAdjectiveToStrM6.Text="";
            textBoxPatternAdjectiveToStrM7.Text="";
            textBoxPatternAdjectiveToStrMN.Text="";
            textBoxPatternAdjectiveToStrMA.Text="";

            textBoxPatternAdjectiveToZenJ1.Text="";
            textBoxPatternAdjectiveToZenJ2.Text="";
            textBoxPatternAdjectiveToZenJ3.Text="";
            textBoxPatternAdjectiveToZenJ4.Text="";
            textBoxPatternAdjectiveToZenJ5.Text="";
            textBoxPatternAdjectiveToZenJ6.Text="";
            textBoxPatternAdjectiveToZenJ7.Text="";
            textBoxPatternAdjectiveToZenJN.Text="";
            textBoxPatternAdjectiveToZenJA.Text="";

            textBoxPatternAdjectiveToZenM1.Text="";
            textBoxPatternAdjectiveToZenM2.Text="";
            textBoxPatternAdjectiveToZenM3.Text="";
            textBoxPatternAdjectiveToZenM4.Text="";
            textBoxPatternAdjectiveToZenM5.Text="";
            textBoxPatternAdjectiveToZenM6.Text="";
            textBoxPatternAdjectiveToZenM7.Text="";
            textBoxPatternAdjectiveToZenMN.Text="";
            textBoxPatternAdjectiveToZenMA.Text="";

            textBoxPatternAdjectiveToMuzJ1.Text="";
            textBoxPatternAdjectiveToMuzJ2.Text="";
            textBoxPatternAdjectiveToMuzJ3.Text="";
            textBoxPatternAdjectiveToMuzJ4.Text="";
            textBoxPatternAdjectiveToMuzJ5.Text="";
            textBoxPatternAdjectiveToMuzJ6.Text="";
            textBoxPatternAdjectiveToMuzJ7.Text="";
            textBoxPatternAdjectiveToMuzJN.Text="";
            textBoxPatternAdjectiveToMuzJA.Text="";

            textBoxPatternAdjectiveToMuzM1.Text="";
            textBoxPatternAdjectiveToMuzM2.Text="";
            textBoxPatternAdjectiveToMuzM3.Text="";
            textBoxPatternAdjectiveToMuzM4.Text="";
            textBoxPatternAdjectiveToMuzM5.Text="";
            textBoxPatternAdjectiveToMuzM6.Text="";
            textBoxPatternAdjectiveToMuzM7.Text="";
            textBoxPatternAdjectiveToMuzMN.Text="";
            textBoxPatternAdjectiveToMuzMA.Text="";

            textBoxPatternAdjectiveToMunJ1.Text="";
            textBoxPatternAdjectiveToMunJ2.Text="";
            textBoxPatternAdjectiveToMunJ3.Text="";
            textBoxPatternAdjectiveToMunJ4.Text="";
            textBoxPatternAdjectiveToMunJ5.Text="";
            textBoxPatternAdjectiveToMunJ6.Text="";
            textBoxPatternAdjectiveToMunJ7.Text="";
            textBoxPatternAdjectiveToMunJN.Text="";
            textBoxPatternAdjectiveToMunJA.Text="";

            textBoxPatternAdjectiveToMunM1.Text="";
            textBoxPatternAdjectiveToMunM2.Text="";
            textBoxPatternAdjectiveToMunM3.Text="";
            textBoxPatternAdjectiveToMunM4.Text="";
            textBoxPatternAdjectiveToMunM5.Text="";
            textBoxPatternAdjectiveToMunM6.Text="";
            textBoxPatternAdjectiveToMunM7.Text="";
            textBoxPatternAdjectiveToMunMN.Text="";
            textBoxPatternAdjectiveToMunMA.Text="";

            textBoxPatternAdjectiveToName.Visible=false;
            labelPatternAdjectiveToStr.Visible=false;
            tableLayoutPanelPatternAdjectiveToStr.Visible=false;
            labelPatternAdjectiveToStrFall.Visible=false;
            labelPatternAdjectiveToStrMultiple.Visible=false;
            labelPatternAdjectiveToStrSingle.Visible=false;

            tableLayoutPanelPatternAdjectiveToMuz.Visible=false;
            labelPatternAdjectiveToMuzFall.Visible=false;
            labelPatternAdjectiveToMuzMultiple.Visible=false;
            labelPatternAdjectiveToMuzSingle.Visible=false;
            labelPatternAdjectiveToMuz.Visible=false;
            tableLayoutPanelPatternAdjectiveToMun.Visible=false;
            labelPatternAdjectiveToMunFall.Visible=false;
            labelPatternAdjectiveToMunMultiple.Visible=false;
            labelPatternAdjectiveToMunSingle.Visible=false;
            labelPatternAdjectiveToZen.Visible=false;
            tableLayoutPanelPatternAdjectiveToZen.Visible=false;
            labelPatternAdjectiveToZenFall.Visible=false;
            labelPatternAdjectiveToZenMultiple.Visible=false;
            labelPatternAdjectiveToZenSingle.Visible=false;
            labelPatternAdjectiveToMun.Visible=false;
            labelPatternAdjectiveToName.Visible=false;
        }

        void PatternAdjectiveToClear() {
            listBoxPatternAdjectiveTo.Items.Clear();
            PatternAdjectiveToSetNone();
            itemsPatternAdjectivesToFiltered?.Clear();
            itemsPatternAdjectiveTo?.Clear();
            CurrentPatternAdjectiveTo=null;
        }
        #endregion

        #region Adjective
        void ListBoxAdjective_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentAdjective();

            int index=listBoxAdjective.SelectedIndex;
            if (itemsAdjectives.Count==0) {
                AdjectiveSetNone();
                return;
            }
            if (index>=itemsAdjectives.Count)
                index=itemsAdjectives.Count-1;
            if (index<0)
                index=0;

            CurrentAdjective=itemsAdjectives[index];
            SetCurrentAdjective();
            AdjectiveSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonAdjectiveAdd_Click(object sender, EventArgs e) {
            AddNewItemAdjective();
        }

        void buttonAdjectiveRemove_Click(object sender, EventArgs e) {
            RemoveItemAdjective(CurrentAdjective);
            TextBoxAdjectiveFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxAdjectiveFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentAdjective();

            // Získej aktuální prvek
            ItemAdjective selectedId=null;
            if (listBoxAdjective.SelectedIndex!=-1) {
                selectedId=itemsAdjectivesFiltered[listBoxAdjective.SelectedIndex];
            }

            AdjectiveRefreshFilteredList();

            listBoxAdjective.Items.Clear();
            for (int i=0; i<itemsAdjectivesFiltered.Count; i++) {
                ItemAdjective item = itemsAdjectivesFiltered[i];

                string textToAdd=item.GetText(itemsPatternAdjectiveFrom.Cast<ItemTranslatingPattern>().ToList(),itemsPatternAdjectiveTo.Cast<ItemTranslatingPattern>().ToList());
           //     if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxAdjective.Items.Add(textToAdd);
            }

            //SetListBoxAdjective();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsAdjectivesFiltered.Count; i++){
                    if (itemsAdjectivesFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxAdjective.SelectedIndex=-1;
                    CurrentAdjective=null;
                } else listBoxAdjective.SelectedIndex=outIndex;
            } else {
                listBoxAdjective.SelectedIndex=-1;
                CurrentAdjective=null;
            }
            SetCurrentAdjective();
        }

        void RemoveCurrentAdjective(object sender, EventArgs e) {
            itemsAdjectives.Remove(CurrentAdjective);
        }

        void AdjectiveSetListBox() {
            //string filter=textBoxAdjectiveFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxAdjective.SelectedIndex;
            listBoxAdjective.Items.Clear();
            for (int i=0; i<itemsAdjectivesFiltered.Count; i++) {
                ItemAdjective item = itemsAdjectivesFiltered[i];

                string textToAdd=item.GetText(itemsPatternAdjectiveFrom.Cast<ItemTranslatingPattern>().ToList(),itemsPatternAdjectiveTo.Cast<ItemTranslatingPattern>().ToList());
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxAdjective.Items.Add(textToAdd);
            }

            if (index>=listBoxAdjective.Items.Count)index=listBoxAdjective.Items.Count-1;
            listBoxAdjective.SelectedIndex=index;
        }

        void AdjectiveRefreshFilteredList() {
            if (itemsAdjectivesFiltered==null) itemsAdjectivesFiltered=new List<ItemAdjective>();
            itemsAdjectivesFiltered.Clear();
            string filter=textBoxAdjectiveFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsAdjectives.Count; i++) {
                    ItemAdjective item = itemsAdjectives[i];
                    if (item.Filter(filter)) {
                        itemsAdjectivesFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsAdjectives.Count; i++) {
                    ItemAdjective item = itemsAdjectives[i];
                    itemsAdjectivesFiltered.Add(item);
                }
            }
        }

        void AddNewItemAdjective() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentAdjective();

            var newItem=new ItemAdjective();
            newItem.To=new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern()};
            itemsAdjectives.Add(newItem);
            CurrentAdjective=newItem;
            AdjectiveRefreshFilteredList();
            AdjectiveSetListBox();
            AdjectiveListBoxSetCurrent();
            SetCurrentAdjective();

            doingJob=false;
        }

        void RemoveItemAdjective(ItemAdjective item) {
            Edited=true;
            ChangeCaptionText();
            itemsAdjectives.Remove(item);
            AdjectiveRefreshFilteredList();
            AdjectiveSetListBox();
            SetCurrentAdjective();
        }

        void SetCurrentAdjective(){
            if (itemsAdjectivesFiltered.Count==0) {
                AdjectiveSetNone();
                return;
            }

            int index=listBoxAdjective.SelectedIndex;
            if (index>=itemsAdjectivesFiltered.Count) index=itemsAdjectivesFiltered.Count-1;
            if (index<0) index=0;
            CurrentAdjective=itemsAdjectivesFiltered[index];

            textBoxAdjectiveFrom.Visible=true;
            //textBoxAdjectiveTo.Visible=true;
            labelAdjectiveFrom.Visible=true;
            //labelAdjectiveTo.Visible=true;
           // textBoxAdjectiveComment.Visible=true;
            //textBoxAdjectiveComment.Text=CurrentAdjective.Comment;

            textBoxAdjectiveFrom.Text=CurrentAdjective.From;
            //textBoxAdjectiveTo.Text=CurrentAdjective.To;

            simpleUIAdjective.Visible=true;
            simpleUIAdjective.SetData(CurrentAdjective.To.ToArray());
            List<string> options=new List<string>();
            foreach (ItemPatternAdjective x in itemsPatternAdjectiveTo) options.Add(x.Name);
            simpleUIAdjective.SetComboboxes(options.ToArray());

            comboBoxAdjectiveInputPatternFrom.Text=CurrentAdjective.PatternFrom;

            comboBoxAdjectiveInputPatternFrom.Items.Clear();
          //  comboBoxAdjectiveInputPatternTo.Items.Clear();
            foreach (ItemPatternAdjective x in itemsPatternAdjectiveFrom) {
                comboBoxAdjectiveInputPatternFrom.Items.Add(x.Name);
            }

            //foreach (ItemPatternAdjective x in itemsPatternAdjectiveTo) {
            //    comboBoxAdjectiveInputPatternTo.Items.Add(x.Name);
            //}

           //comboBoxAdjectiveInputPatternTo.Text=CurrentAdjective.PatternTo;
         //buttonAddToAdjective.Visible=true;
            comboBoxAdjectiveInputPatternFrom.Visible=true;
            //comboBoxAdjectiveInputPatternTo.Visible=true;

            labelAdjectiveInputPatternFrom.Visible=true;
           // labelAdjectiveInputPatternTo.Visible=true;

            labelAdjectiveShowFrom.Visible=true;
           // labelAdjectiveShowTo.Visible=true;
        }

        void AdjectiveListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsAdjectivesFiltered.Count; indexCur++) {
                if (itemsAdjectives[indexCur]==CurrentAdjective) {
                    int indexList=listBoxAdjective.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxAdjective.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentAdjective() {
            if (CurrentAdjective==null) return;

            CurrentAdjective.From=textBoxAdjectiveFrom.Text;
          //  CurrentAdjective.To=textBoxAdjectiveTo.Text;

            CurrentAdjective.PatternFrom=comboBoxAdjectiveInputPatternFrom.Text;
         //   CurrentAdjective.PatternTo=comboBoxAdjectiveInputPatternTo.Text;

          //  CurrentAdjective.Comment=textBoxAdjectiveComment.Text;

            CurrentAdjective.To=simpleUIAdjective.GetData().ToList();
        }

        void AdjectiveSetNone(){
            textBoxAdjectiveFrom.Text="";
           // textBoxAdjectiveTo.Text="";

            comboBoxAdjectiveInputPatternFrom.Text="";
            //comboBoxAdjectiveInputPatternTo.Text="";

            textBoxAdjectiveFrom.Visible=false;
           // textBoxAdjectiveTo.Visible=false;
            labelAdjectiveFrom.Visible=false;
           // labelAdjectiveTo.Visible=false;
          //  comboBoxAdjectiveInputPatternTo.Visible=false;
            comboBoxAdjectiveInputPatternFrom.Visible=false;
            labelAdjectiveShowFrom.Visible=false;
          //  labelAdjectiveShowTo.Visible=false;
            labelAdjectiveInputPatternFrom.Visible=false;
          //  labelAdjectiveInputPatternTo.Visible=false;

           // textBoxAdjectiveComment.Text="";
          //  textBoxAdjectiveComment.Visible=false;
          //buttonAddToAdjective.Visible=false;

            simpleUIAdjective.Visible=false;
            simpleUIAdjective.Clear();
        }

        void ClearAdjective(){
            listBoxAdjective.Items.Clear();
            AdjectiveSetNone();
            itemsAdjectivesFiltered?.Clear();
            itemsAdjectives?.Clear();
            CurrentAdjective=null;
        }
        #endregion

        #region PronounPatternFrom
        void PatternPronounFromListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternPronounFromSaveCurrent();

            int index=listBoxPatternPronounFrom.SelectedIndex;
            if (itemsPatternPronounFrom.Count==0) {
                PatternPronounFromSetNone();
                return;
            }
            if (index>=itemsPatternPronounFrom.Count)
                index=itemsPatternPronounFrom.Count-1;
            if (index<0)
                index=0;

            CurrentPatternPronounFrom=itemsPatternPronounFrom[index];
            PatternPronounFromSetCurrent();
            PatternPronounFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternPronounFromButtonAdd_Click(object sender, EventArgs e) {
            PatternPronounFromAddNewItem();
        }

        void PatternPronounFromButtonRemove_Click(object sender, EventArgs e) {
            PatternPronounFromRemoveItem(CurrentPatternPronounFrom);
            PatternPronounFromTextBoxFilter_TextChanged(null, new EventArgs());
        }

        void PatternPronounFromTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternPronounFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternPronoun selectedId=null;
            if (listBoxPatternPronounFrom.SelectedIndex!=-1) {
                selectedId=itemsPatternPronounsFromFiltered[listBoxPatternPronounFrom.SelectedIndex];
            }

            PatternPronounFromRefreshFilteredList();

            listBoxPatternPronounFrom.Items.Clear();
            for (int i=0; i<itemsPatternPronounsFromFiltered.Count; i++) {
                ItemPatternPronoun item = itemsPatternPronounsFromFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternPronounFrom.Items.Add(textToAdd);
            }

            //PronounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternPronounsFromFiltered.Count; i++){
                    if (itemsPatternPronounsFromFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxPatternPronounFrom.SelectedIndex=-1;
                    CurrentPatternPronounFrom=null;
                } else listBoxPatternPronounFrom.SelectedIndex=outIndex;
            } else {
                listBoxPatternPronounFrom.SelectedIndex=-1;
                CurrentPatternPronounFrom=null;
            }
            PatternPronounFromSetCurrent();
        }

        void PatternPronounFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternPronounFrom.Remove(CurrentPatternPronounFrom);
        }

        void PatternPronounFromSetListBox() {
            //string filter=textBoxPatternPronounFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxPatternPronounFrom.SelectedIndex;
            listBoxPatternPronounFrom.Items.Clear();
            for (int i=0; i<itemsPatternPronounsFromFiltered.Count; i++) {
                ItemPatternPronoun item = itemsPatternPronounsFromFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternPronounFrom.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternPronounFrom.Items.Count)index=listBoxPatternPronounFrom.Items.Count-1;
            listBoxPatternPronounFrom.SelectedIndex=index;
        }

        void PatternPronounFromRefreshFilteredList() {
            if (itemsPatternPronounsFromFiltered==null) itemsPatternPronounsFromFiltered=new List<ItemPatternPronoun>();
            itemsPatternPronounsFromFiltered.Clear();
            string filter=textBoxPatternPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternPronounFrom.Count; i++) {
                    ItemPatternPronoun item = itemsPatternPronounFrom[i];

                    if (item.Filter(filter)) {
                        itemsPatternPronounsFromFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternPronounFrom.Count; i++) {
                    ItemPatternPronoun item = itemsPatternPronounFrom[i];
                    itemsPatternPronounsFromFiltered.Add(item);
                }
            }
        }

        void PatternPronounFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternPronounFromSaveCurrent();

            var newItem=new ItemPatternPronoun();
           // newItem.ID=itemsPronouns.Count;
            itemsPatternPronounFrom.Add(newItem);
            CurrentPatternPronounFrom=newItem;
            PatternPronounFromRefreshFilteredList();
            PatternPronounFromSetListBox();
            PatternPronounFromListBoxSetCurrent();
            PatternPronounFromSetCurrent();

            doingJob=false;
        }

        void PatternPronounFromRemoveItem(ItemPatternPronoun item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternPronounFrom.Remove(item);
            PatternPronounFromRefreshFilteredList();
            PatternPronounFromSetListBox();
            PatternPronounFromSetCurrent();
        }

        void PatternPronounFromSetCurrent(){
            if (itemsPatternPronounsFromFiltered.Count==0) {
                PatternPronounFromSetNone();
                return;
            }

            int index=listBoxPatternPronounFrom.SelectedIndex;
            if (index>=itemsPatternPronounsFromFiltered.Count) index=itemsPatternPronounsFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternPronounFrom=itemsPatternPronounsFromFiltered[index];

            textBoxPatternPronounFromName.Text=CurrentPatternPronounFrom.Name;
            comboBoxPatternPronounFromType.SelectedIndex=(int)CurrentPatternPronounFrom.Type;
          //  comboBoxPatternPronounGender.SelectedIndex=(int)CurrentPatternPronoun.Gender;

            //textBoxPatternPronounMuzJ1.Text=CurrentPatternPronoun.Shapes[0];
            //textBoxPatternPronounMuzJ2.Text=CurrentPatternPronoun.Shapes[1];
            //textBoxPatternPronounMuzJ3.Text=CurrentPatternPronoun.Shapes[2];
            //textBoxPatternPronounMuzJ4.Text=CurrentPatternPronoun.Shapes[3];
            //textBoxPatternPronounMuzJ5.Text=CurrentPatternPronoun.Shapes[4];
            //textBoxPatternPronounMuzJ6.Text=CurrentPatternPronoun.Shapes[5];
            //textBoxPatternPronounMuzJ7.Text=CurrentPatternPronoun.Shapes[6];

            //textBoxPatternPronounMuzM1.Text=CurrentPatternPronoun.Shapes[7];
            //textBoxPatternPronounMuzM2.Text=CurrentPatternPronoun.Shapes[8];
            //textBoxPatternPronounMuzM3.Text=CurrentPatternPronoun.Shapes[9];
            //textBoxPatternPronounMuzM4.Text=CurrentPatternPronoun.Shapes[10];
            //textBoxPatternPronounMuzM5.Text=CurrentPatternPronoun.Shapes[11];
            //textBoxPatternPronounMuzM6.Text=CurrentPatternPronoun.Shapes[12];
            //textBoxPatternPronounMuzM7.Text=CurrentPatternPronoun.Shapes[13];

             switch (CurrentPatternPronounFrom.Type) {
                case PronounType.Unknown:
                    break;

                case PronounType.NoDeklination:
                     textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounFrom.Shapes[0];
                    break;

                case PronounType.DeklinationOnlySingle:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounFrom.Shapes[0];
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounFrom.Shapes[1];
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounFrom.Shapes[2];
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounFrom.Shapes[3];
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounFrom.Shapes[4];
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounFrom.Shapes[5];
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounFrom.Shapes[6];
                    break;

                case PronounType.Deklination:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounFrom.Shapes[0];
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounFrom.Shapes[1];
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounFrom.Shapes[2];
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounFrom.Shapes[3];
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounFrom.Shapes[4];
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounFrom.Shapes[5];
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounFrom.Shapes[6];

                    textBoxPatternPronounFromMuzM1.Text=CurrentPatternPronounFrom.Shapes[7];
                    textBoxPatternPronounFromMuzM2.Text=CurrentPatternPronounFrom.Shapes[8];
                    textBoxPatternPronounFromMuzM3.Text=CurrentPatternPronounFrom.Shapes[9];
                    textBoxPatternPronounFromMuzM4.Text=CurrentPatternPronounFrom.Shapes[10];
                    textBoxPatternPronounFromMuzM5.Text=CurrentPatternPronounFrom.Shapes[11];
                    textBoxPatternPronounFromMuzM6.Text=CurrentPatternPronounFrom.Shapes[12];
                    textBoxPatternPronounFromMuzM7.Text=CurrentPatternPronounFrom.Shapes[13];
                    break;

                case PronounType.DeklinationWithGender:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounFrom.Shapes[0] ;
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounFrom.Shapes[1] ;
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounFrom.Shapes[2] ;
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounFrom.Shapes[3] ;
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounFrom.Shapes[4] ;
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounFrom.Shapes[5] ;
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounFrom.Shapes[6] ;
                    textBoxPatternPronounFromMuzM1.Text=CurrentPatternPronounFrom.Shapes[7] ;
                    textBoxPatternPronounFromMuzM2.Text=CurrentPatternPronounFrom.Shapes[8] ;
                    textBoxPatternPronounFromMuzM3.Text=CurrentPatternPronounFrom.Shapes[9] ;
                    textBoxPatternPronounFromMuzM4.Text=CurrentPatternPronounFrom.Shapes[10];
                    textBoxPatternPronounFromMuzM5.Text=CurrentPatternPronounFrom.Shapes[11];
                    textBoxPatternPronounFromMuzM6.Text=CurrentPatternPronounFrom.Shapes[12];
                    textBoxPatternPronounFromMuzM7.Text=CurrentPatternPronounFrom.Shapes[13];

                    textBoxPatternPronounFromMunJ1.Text=CurrentPatternPronounFrom.Shapes[14];
                    textBoxPatternPronounFromMunJ2.Text=CurrentPatternPronounFrom.Shapes[15];
                    textBoxPatternPronounFromMunJ3.Text=CurrentPatternPronounFrom.Shapes[16];
                    textBoxPatternPronounFromMunJ4.Text=CurrentPatternPronounFrom.Shapes[17];
                    textBoxPatternPronounFromMunJ5.Text=CurrentPatternPronounFrom.Shapes[18];
                    textBoxPatternPronounFromMunJ6.Text=CurrentPatternPronounFrom.Shapes[19];
                    textBoxPatternPronounFromMunJ7.Text=CurrentPatternPronounFrom.Shapes[20];
                    textBoxPatternPronounFromMunM1.Text=CurrentPatternPronounFrom.Shapes[21];
                    textBoxPatternPronounFromMunM2.Text=CurrentPatternPronounFrom.Shapes[22];
                    textBoxPatternPronounFromMunM3.Text=CurrentPatternPronounFrom.Shapes[23];
                    textBoxPatternPronounFromMunM4.Text=CurrentPatternPronounFrom.Shapes[24];
                    textBoxPatternPronounFromMunM5.Text=CurrentPatternPronounFrom.Shapes[25];
                    textBoxPatternPronounFromMunM6.Text=CurrentPatternPronounFrom.Shapes[26];
                    textBoxPatternPronounFromMunM7.Text=CurrentPatternPronounFrom.Shapes[27];

                    textBoxPatternPronounFromZenJ1.Text=CurrentPatternPronounFrom.Shapes[28];
                    textBoxPatternPronounFromZenJ2.Text=CurrentPatternPronounFrom.Shapes[29];
                    textBoxPatternPronounFromZenJ3.Text=CurrentPatternPronounFrom.Shapes[30];
                    textBoxPatternPronounFromZenJ4.Text=CurrentPatternPronounFrom.Shapes[31];
                    textBoxPatternPronounFromZenJ5.Text=CurrentPatternPronounFrom.Shapes[32];
                    textBoxPatternPronounFromZenJ6.Text=CurrentPatternPronounFrom.Shapes[33];
                    textBoxPatternPronounFromZenJ7.Text=CurrentPatternPronounFrom.Shapes[34];
                    textBoxPatternPronounFromZenM1.Text=CurrentPatternPronounFrom.Shapes[35];
                    textBoxPatternPronounFromZenM2.Text=CurrentPatternPronounFrom.Shapes[36];
                    textBoxPatternPronounFromZenM3.Text=CurrentPatternPronounFrom.Shapes[37];
                    textBoxPatternPronounFromZenM4.Text=CurrentPatternPronounFrom.Shapes[38];
                    textBoxPatternPronounFromZenM5.Text=CurrentPatternPronounFrom.Shapes[39];
                    textBoxPatternPronounFromZenM6.Text=CurrentPatternPronounFrom.Shapes[40];
                    textBoxPatternPronounFromZenM7.Text=CurrentPatternPronounFrom.Shapes[41];

                    textBoxPatternPronounFromStrJ1.Text=CurrentPatternPronounFrom.Shapes[42];
                    textBoxPatternPronounFromStrJ2.Text=CurrentPatternPronounFrom.Shapes[43];
                    textBoxPatternPronounFromStrJ3.Text=CurrentPatternPronounFrom.Shapes[44];
                    textBoxPatternPronounFromStrJ4.Text=CurrentPatternPronounFrom.Shapes[45];
                    textBoxPatternPronounFromStrJ5.Text=CurrentPatternPronounFrom.Shapes[46];
                    textBoxPatternPronounFromStrJ6.Text=CurrentPatternPronounFrom.Shapes[47];
                    textBoxPatternPronounFromStrJ7.Text=CurrentPatternPronounFrom.Shapes[48];
                    textBoxPatternPronounFromStrM1.Text=CurrentPatternPronounFrom.Shapes[49];
                    textBoxPatternPronounFromStrM2.Text=CurrentPatternPronounFrom.Shapes[50];
                    textBoxPatternPronounFromStrM3.Text=CurrentPatternPronounFrom.Shapes[51];
                    textBoxPatternPronounFromStrM4.Text=CurrentPatternPronounFrom.Shapes[52];
                    textBoxPatternPronounFromStrM5.Text=CurrentPatternPronounFrom.Shapes[53];
                    textBoxPatternPronounFromStrM6.Text=CurrentPatternPronounFrom.Shapes[54];
                    textBoxPatternPronounFromStrM7.Text=CurrentPatternPronounFrom.Shapes[55];
                    break;
            }

            labelPatternPronounFromName.Visible=true;
            textBoxPatternPronounFromName.Visible=true;
            labelPatternPronounType.Visible=true;
            comboBoxPatternPronounFromType.Visible=true;

            labelPatternPronounFromMuzFall.Visible=true;
            labelPatternPronounFromMuzSingle.Visible=true;
            labelPatternPronounFromMuzMultiple.Visible=true;
            tableLayoutPanelPatternPronounFromMuz.Visible=true;

           //abelPatternPronounInfo.Visible=true;
            ChangeTypePatternPronounFrom(CurrentPatternPronounFrom?.Type);
        }

        void PatternPronounFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternPronounsFromFiltered.Count; indexCur++) {
                if (itemsPatternPronounFrom[indexCur]==CurrentPatternPronounFrom) {
                    int indexList=listBoxPatternPronounFrom.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternPronounFrom.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternPronounFromSaveCurrent() {
            if (CurrentPatternPronounFrom==null) return;
            Edited=true;
            comboBoxPatternPronounFromType.SelectedIndex=(int) CurrentPatternPronounFrom.Type;
            CurrentPatternPronounFrom.Name=textBoxPatternPronounFromName.Text;
            switch (CurrentPatternPronounFrom.Type) {
                case PronounType.Unknown:
                    break;

                case PronounType.NoDeklination:
                    if (CurrentPatternPronounFrom.Shapes.Length>0) {
                    CurrentPatternPronounFrom.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    } else CurrentPatternPronounFrom.Shapes=new string[]{ "" };
                    break;

                case PronounType.DeklinationOnlySingle:
                    CurrentPatternPronounFrom.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounFrom.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounFrom.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounFrom.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounFrom.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounFrom.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounFrom.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;
                    break;

                case PronounType.Deklination:
                    CurrentPatternPronounFrom.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounFrom.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounFrom.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounFrom.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounFrom.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounFrom.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounFrom.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;

                    CurrentPatternPronounFrom.Shapes[7]=textBoxPatternPronounFromMuzM1.Text;
                    CurrentPatternPronounFrom.Shapes[8]=textBoxPatternPronounFromMuzM2.Text;
                    CurrentPatternPronounFrom.Shapes[9]=textBoxPatternPronounFromMuzM3.Text;
                    CurrentPatternPronounFrom.Shapes[10]=textBoxPatternPronounFromMuzM4.Text;
                    CurrentPatternPronounFrom.Shapes[11]=textBoxPatternPronounFromMuzM5.Text;
                    CurrentPatternPronounFrom.Shapes[12]=textBoxPatternPronounFromMuzM6.Text;
                    CurrentPatternPronounFrom.Shapes[13]=textBoxPatternPronounFromMuzM7.Text;
                    break;

                case PronounType.DeklinationWithGender:
                    CurrentPatternPronounFrom.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounFrom.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounFrom.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounFrom.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounFrom.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounFrom.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounFrom.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;
                    CurrentPatternPronounFrom.Shapes[7]=textBoxPatternPronounFromMuzM1.Text;
                    CurrentPatternPronounFrom.Shapes[8]=textBoxPatternPronounFromMuzM2.Text;
                    CurrentPatternPronounFrom.Shapes[9]=textBoxPatternPronounFromMuzM3.Text;
                    CurrentPatternPronounFrom.Shapes[10]=textBoxPatternPronounFromMuzM4.Text;
                    CurrentPatternPronounFrom.Shapes[11]=textBoxPatternPronounFromMuzM5.Text;
                    CurrentPatternPronounFrom.Shapes[12]=textBoxPatternPronounFromMuzM6.Text;
                    CurrentPatternPronounFrom.Shapes[13]=textBoxPatternPronounFromMuzM7.Text;

                    CurrentPatternPronounFrom.Shapes[14]=textBoxPatternPronounFromMunJ1.Text;
                    CurrentPatternPronounFrom.Shapes[15]=textBoxPatternPronounFromMunJ2.Text;
                    CurrentPatternPronounFrom.Shapes[16]=textBoxPatternPronounFromMunJ3.Text;
                    CurrentPatternPronounFrom.Shapes[17]=textBoxPatternPronounFromMunJ4.Text;
                    CurrentPatternPronounFrom.Shapes[18]=textBoxPatternPronounFromMunJ5.Text;
                    CurrentPatternPronounFrom.Shapes[19]=textBoxPatternPronounFromMunJ6.Text;
                    CurrentPatternPronounFrom.Shapes[20]=textBoxPatternPronounFromMunJ7.Text;
                    CurrentPatternPronounFrom.Shapes[21]=textBoxPatternPronounFromMunM1.Text;
                    CurrentPatternPronounFrom.Shapes[22]=textBoxPatternPronounFromMunM2.Text;
                    CurrentPatternPronounFrom.Shapes[23]=textBoxPatternPronounFromMunM3.Text;
                    CurrentPatternPronounFrom.Shapes[24]=textBoxPatternPronounFromMunM4.Text;
                    CurrentPatternPronounFrom.Shapes[25]=textBoxPatternPronounFromMunM5.Text;
                    CurrentPatternPronounFrom.Shapes[26]=textBoxPatternPronounFromMunM6.Text;
                    CurrentPatternPronounFrom.Shapes[27]=textBoxPatternPronounFromMunM7.Text;

                    CurrentPatternPronounFrom.Shapes[28]=textBoxPatternPronounFromZenJ1.Text;
                    CurrentPatternPronounFrom.Shapes[29]=textBoxPatternPronounFromZenJ2.Text;
                    CurrentPatternPronounFrom.Shapes[30]=textBoxPatternPronounFromZenJ3.Text;
                    CurrentPatternPronounFrom.Shapes[31]=textBoxPatternPronounFromZenJ4.Text;
                    CurrentPatternPronounFrom.Shapes[32]=textBoxPatternPronounFromZenJ5.Text;
                    CurrentPatternPronounFrom.Shapes[33]=textBoxPatternPronounFromZenJ6.Text;
                    CurrentPatternPronounFrom.Shapes[34]=textBoxPatternPronounFromZenJ7.Text;
                    CurrentPatternPronounFrom.Shapes[35]=textBoxPatternPronounFromZenM1.Text;
                    CurrentPatternPronounFrom.Shapes[36]=textBoxPatternPronounFromZenM2.Text;
                    CurrentPatternPronounFrom.Shapes[37]=textBoxPatternPronounFromZenM3.Text;
                    CurrentPatternPronounFrom.Shapes[38]=textBoxPatternPronounFromZenM4.Text;
                    CurrentPatternPronounFrom.Shapes[39]=textBoxPatternPronounFromZenM5.Text;
                    CurrentPatternPronounFrom.Shapes[40]=textBoxPatternPronounFromZenM6.Text;
                    CurrentPatternPronounFrom.Shapes[41]=textBoxPatternPronounFromZenM7.Text;

                    CurrentPatternPronounFrom.Shapes[42]=textBoxPatternPronounFromStrJ1.Text;
                    CurrentPatternPronounFrom.Shapes[43]=textBoxPatternPronounFromStrJ2.Text;
                    CurrentPatternPronounFrom.Shapes[44]=textBoxPatternPronounFromStrJ3.Text;
                    CurrentPatternPronounFrom.Shapes[45]=textBoxPatternPronounFromStrJ4.Text;
                    CurrentPatternPronounFrom.Shapes[46]=textBoxPatternPronounFromStrJ5.Text;
                    CurrentPatternPronounFrom.Shapes[47]=textBoxPatternPronounFromStrJ6.Text;
                    CurrentPatternPronounFrom.Shapes[48]=textBoxPatternPronounFromStrJ7.Text;
                    CurrentPatternPronounFrom.Shapes[49]=textBoxPatternPronounFromStrM1.Text;
                    CurrentPatternPronounFrom.Shapes[50]=textBoxPatternPronounFromStrM2.Text;
                    CurrentPatternPronounFrom.Shapes[51]=textBoxPatternPronounFromStrM3.Text;
                    CurrentPatternPronounFrom.Shapes[52]=textBoxPatternPronounFromStrM4.Text;
                    CurrentPatternPronounFrom.Shapes[53]=textBoxPatternPronounFromStrM5.Text;
                    CurrentPatternPronounFrom.Shapes[54]=textBoxPatternPronounFromStrM6.Text;
                    CurrentPatternPronounFrom.Shapes[55]=textBoxPatternPronounFromStrM7.Text;
                    break;
            }
        }

        void PatternPronounFromSetNone(){
            textBoxPatternPronounFromName.Text="";

            textBoxPatternPronounFromMuzJ1.Text="";
            textBoxPatternPronounFromMuzJ2.Text="";
            textBoxPatternPronounFromMuzJ3.Text="";
            textBoxPatternPronounFromMuzJ4.Text="";
            textBoxPatternPronounFromMuzJ5.Text="";
            textBoxPatternPronounFromMuzJ6.Text="";
            textBoxPatternPronounFromMuzJ7.Text="";

            textBoxPatternPronounFromMuzM1.Text="";
            textBoxPatternPronounFromMuzM2.Text="";
            textBoxPatternPronounFromMuzM3.Text="";
            textBoxPatternPronounFromMuzM4.Text="";
            textBoxPatternPronounFromMuzM5.Text="";
            textBoxPatternPronounFromMuzM6.Text="";
            textBoxPatternPronounFromMuzM7.Text="";


            textBoxPatternPronounFromZenJ1.Text="";
            textBoxPatternPronounFromZenJ2.Text="";
            textBoxPatternPronounFromZenJ3.Text="";
            textBoxPatternPronounFromZenJ4.Text="";
            textBoxPatternPronounFromZenJ5.Text="";
            textBoxPatternPronounFromZenJ6.Text="";
            textBoxPatternPronounFromZenJ7.Text="";

            textBoxPatternPronounFromZenM1.Text="";
            textBoxPatternPronounFromZenM2.Text="";
            textBoxPatternPronounFromZenM3.Text="";
            textBoxPatternPronounFromZenM4.Text="";
            textBoxPatternPronounFromZenM5.Text="";
            textBoxPatternPronounFromZenM6.Text="";
            textBoxPatternPronounFromZenM7.Text="";

            textBoxPatternPronounFromMunJ1.Text="";
            textBoxPatternPronounFromMunJ2.Text="";
            textBoxPatternPronounFromMunJ3.Text="";
            textBoxPatternPronounFromMunJ4.Text="";
            textBoxPatternPronounFromMunJ5.Text="";
            textBoxPatternPronounFromMunJ6.Text="";
            textBoxPatternPronounFromMunJ7.Text="";

            textBoxPatternPronounFromMunM1.Text="";
            textBoxPatternPronounFromMunM2.Text="";
            textBoxPatternPronounFromMunM3.Text="";
            textBoxPatternPronounFromMunM4.Text="";
            textBoxPatternPronounFromMunM5.Text="";
            textBoxPatternPronounFromMunM6.Text="";
            textBoxPatternPronounFromMunM7.Text="";

            textBoxPatternPronounFromStrJ1.Text="";
            textBoxPatternPronounFromStrJ2.Text="";
            textBoxPatternPronounFromStrJ3.Text="";
            textBoxPatternPronounFromStrJ4.Text="";
            textBoxPatternPronounFromStrJ5.Text="";
            textBoxPatternPronounFromStrJ6.Text="";
            textBoxPatternPronounFromStrJ7.Text="";

            textBoxPatternPronounFromStrM1.Text="";
            textBoxPatternPronounFromStrM2.Text="";
            textBoxPatternPronounFromStrM3.Text="";
            textBoxPatternPronounFromStrM4.Text="";
            textBoxPatternPronounFromStrM5.Text="";
            textBoxPatternPronounFromStrM6.Text="";
            textBoxPatternPronounFromStrM7.Text="";

            labelPatternPronounFromName.Visible=false;
            textBoxPatternPronounFromName.Visible=false;
            labelPatternPronounType.Visible=false;
            comboBoxPatternPronounFromType.Visible=false;

            labelPatternPronounFromMuzFall.Visible=false;
            labelPatternPronounFromMuzSingle.Visible=false;
            labelPatternPronounFromMuzMultiple.Visible=false;
            tableLayoutPanelPatternPronounFromMuz.Visible=false;
            //labelPatternPronounInfo.Visible=false;
           // ChangeTypePatternPronoun(CurrentPatternPronoun?.Type);
        }

        void PatternPronounFromClear() {
            listBoxPatternPronounFrom.Items.Clear();
            PatternPronounFromSetNone();
            itemsPatternPronounsFromFiltered?.Clear();
            itemsPatternPronounFrom?.Clear();
            CurrentPatternPronounFrom=null;
        }
        #endregion

        #region PronounPatternTo
        void PatternPronounToListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternPronounToSaveCurrent();

            int index=listBoxPatternPronounTo.SelectedIndex;
            if (itemsPatternPronounTo.Count==0) {
                PatternPronounToSetNone();
                return;
            }
            if (index>=itemsPatternPronounTo.Count)
                index=itemsPatternPronounTo.Count-1;
            if (index<0)
                index=0;

            CurrentPatternPronounTo=itemsPatternPronounTo[index];
            PatternPronounToSetCurrent();
            PatternPronounToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternPronounToButtonAdd_Click(object sender, EventArgs e) {
            PatternPronounToAddNewItem();
        }

        void PatternPronounToButtonRemove_Click(object sender, EventArgs e) {
            PatternPronounToRemoveItem(CurrentPatternPronounTo);
            PatternPronounToTextBoxFilter_TextChanged(null, new EventArgs());
        }

        void PatternPronounToTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternPronounToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternPronoun selectedId=null;
            if (listBoxPatternPronounTo.SelectedIndex!=-1) {
                selectedId=itemsPatternPronounsToFiltered[listBoxPatternPronounTo.SelectedIndex];
            }

            PatternPronounToRefreshFilteredList();

            listBoxPatternPronounTo.Items.Clear();
            for (int i=0; i<itemsPatternPronounsToFiltered.Count; i++) {
                ItemPatternPronoun item = itemsPatternPronounsToFiltered[i];

                string textToAdd=item.GetText();
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternPronounTo.Items.Add(textToAdd);
            }

            //PronounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternPronounsToFiltered.Count; i++){
                    if (itemsPatternPronounsToFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxPatternPronounTo.SelectedIndex=-1;
                    CurrentPatternPronounTo=null;
                } else listBoxPatternPronounTo.SelectedIndex=outIndex;
            } else {
                listBoxPatternPronounTo.SelectedIndex=-1;
                CurrentPatternPronounTo=null;
            }
            PatternPronounToSetCurrent();
        }

        void PatternPronounToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternPronounTo.Remove(CurrentPatternPronounTo);
        }

        void PatternPronounToSetListBox() {
            //string filter=textBoxPatternPronounFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxPatternPronounTo.SelectedIndex;
            listBoxPatternPronounTo.Items.Clear();
            for (int i=0; i<itemsPatternPronounsToFiltered.Count; i++) {
                ItemPatternPronoun item = itemsPatternPronounsToFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternPronounTo.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternPronounTo.Items.Count)index=listBoxPatternPronounTo.Items.Count-1;
            listBoxPatternPronounTo.SelectedIndex=index;
        }

        void PatternPronounToRefreshFilteredList() {
            if (itemsPatternPronounsToFiltered==null) itemsPatternPronounsToFiltered=new List<ItemPatternPronoun>();
            itemsPatternPronounsToFiltered.Clear();
            string filter=textBoxPatternPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternPronounTo.Count; i++) {
                    ItemPatternPronoun item = itemsPatternPronounTo[i];

                    if (item.Filter(filter)) {
                        itemsPatternPronounsToFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternPronounTo.Count; i++) {
                    ItemPatternPronoun item = itemsPatternPronounTo[i];
                    itemsPatternPronounsToFiltered.Add(item);
                }
            }
        }

        void PatternPronounToRefresh(){
            PatternPronounToRefreshFilteredList();
            PatternPronounToSetListBox();
            PatternPronounToListBoxSetCurrent();
        }

        void PatternPronounFromRefresh(){
            PatternPronounFromRefreshFilteredList();
            PatternPronounFromSetListBox();
            PatternPronounFromListBoxSetCurrent();
        }

        void PatternPronounToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternPronounToSaveCurrent();

            var newItem=new ItemPatternPronoun();
           // newItem.ID=itemsPronouns.Count;
            itemsPatternPronounTo.Add(newItem);
            CurrentPatternPronounTo=newItem;
            PatternPronounToRefreshFilteredList();
            PatternPronounToSetListBox();
            PatternPronounToListBoxSetCurrent();
            PatternPronounToSetCurrent();

            doingJob=false;
        }

        void PatternPronounToRemoveItem(ItemPatternPronoun item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternPronounTo.Remove(item);
            PatternPronounToRefreshFilteredList();
            PatternPronounToSetListBox();
            PatternPronounToSetCurrent();
        }

        void PatternPronounToSetCurrent(){
            if (itemsPatternPronounsToFiltered.Count==0) {
                PatternPronounToSetNone();
                return;
            }

            int index=listBoxPatternPronounTo.SelectedIndex;
            if (index>=itemsPatternPronounsToFiltered.Count) index=itemsPatternPronounsToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternPronounTo=itemsPatternPronounsToFiltered[index];

            textBoxPatternPronounToName.Text=CurrentPatternPronounTo.Name;
            comboBoxPatternPronounToType.SelectedIndex=(int)CurrentPatternPronounTo.Type;

             switch (CurrentPatternPronounTo.Type) {
                case PronounType.Unknown:
                    break;

                case PronounType.NoDeklination:
                    textBoxPatternPronounToMuzJ1.Text=CurrentPatternPronounTo.Shapes[0];

                    tableLayoutPanelPatternPronounToMun.Visible=false;
                    tableLayoutPanelPatternPronounToZen.Visible=false;
                    tableLayoutPanelPatternPronounToStr.Visible=false;
                    tableLayoutPanelPatternPronounToMuz.Visible=true;
                    break;

                case PronounType.DeklinationOnlySingle:
                    textBoxPatternPronounToMuzJ1.Text=CurrentPatternPronounTo.Shapes[0];
                    textBoxPatternPronounToMuzJ2.Text=CurrentPatternPronounTo.Shapes[1];
                    textBoxPatternPronounToMuzJ3.Text=CurrentPatternPronounTo.Shapes[2];
                    textBoxPatternPronounToMuzJ4.Text=CurrentPatternPronounTo.Shapes[3];
                    textBoxPatternPronounToMuzJ5.Text=CurrentPatternPronounTo.Shapes[4];
                    textBoxPatternPronounToMuzJ6.Text=CurrentPatternPronounTo.Shapes[5];
                    textBoxPatternPronounToMuzJ7.Text=CurrentPatternPronounTo.Shapes[6];


                    tableLayoutPanelPatternPronounToMun.Visible=false;
                    tableLayoutPanelPatternPronounToZen.Visible=false;
                    tableLayoutPanelPatternPronounToStr.Visible=false;
                    tableLayoutPanelPatternPronounToMuz.Visible=true;
                    break;

                case PronounType.Deklination:
                    textBoxPatternPronounToMuzJ1.Text=CurrentPatternPronounTo.Shapes[0];
                    textBoxPatternPronounToMuzJ2.Text=CurrentPatternPronounTo.Shapes[1];
                    textBoxPatternPronounToMuzJ3.Text=CurrentPatternPronounTo.Shapes[2];
                    textBoxPatternPronounToMuzJ4.Text=CurrentPatternPronounTo.Shapes[3];
                    textBoxPatternPronounToMuzJ5.Text=CurrentPatternPronounTo.Shapes[4];
                    textBoxPatternPronounToMuzJ6.Text=CurrentPatternPronounTo.Shapes[5];
                    textBoxPatternPronounToMuzJ7.Text=CurrentPatternPronounTo.Shapes[6];

                    textBoxPatternPronounToMuzM1.Text=CurrentPatternPronounTo.Shapes[7];
                    textBoxPatternPronounToMuzM2.Text=CurrentPatternPronounTo.Shapes[8];
                    textBoxPatternPronounToMuzM3.Text=CurrentPatternPronounTo.Shapes[9];
                    textBoxPatternPronounToMuzM4.Text=CurrentPatternPronounTo.Shapes[10];
                    textBoxPatternPronounToMuzM5.Text=CurrentPatternPronounTo.Shapes[11];
                    textBoxPatternPronounToMuzM6.Text=CurrentPatternPronounTo.Shapes[12];
                    textBoxPatternPronounToMuzM7.Text=CurrentPatternPronounTo.Shapes[13];


                    tableLayoutPanelPatternPronounToMun.Visible=false;
                    tableLayoutPanelPatternPronounToZen.Visible=false;
                    tableLayoutPanelPatternPronounToStr.Visible=false;
                    tableLayoutPanelPatternPronounToMuz.Visible=true;
                    break;

                case PronounType.DeklinationWithGender:
                    textBoxPatternPronounToMuzJ1.Text=CurrentPatternPronounTo.Shapes[0] ;
                    textBoxPatternPronounToMuzJ2.Text=CurrentPatternPronounTo.Shapes[1] ;
                    textBoxPatternPronounToMuzJ3.Text=CurrentPatternPronounTo.Shapes[2] ;
                    textBoxPatternPronounToMuzJ4.Text=CurrentPatternPronounTo.Shapes[3] ;
                    textBoxPatternPronounToMuzJ5.Text=CurrentPatternPronounTo.Shapes[4] ;
                    textBoxPatternPronounToMuzJ6.Text=CurrentPatternPronounTo.Shapes[5] ;
                    textBoxPatternPronounToMuzJ7.Text=CurrentPatternPronounTo.Shapes[6] ;
                    textBoxPatternPronounToMuzM1.Text=CurrentPatternPronounTo.Shapes[7] ;
                    textBoxPatternPronounToMuzM2.Text=CurrentPatternPronounTo.Shapes[8] ;
                    textBoxPatternPronounToMuzM3.Text=CurrentPatternPronounTo.Shapes[9] ;
                    textBoxPatternPronounToMuzM4.Text=CurrentPatternPronounTo.Shapes[10];
                    textBoxPatternPronounToMuzM5.Text=CurrentPatternPronounTo.Shapes[11];
                    textBoxPatternPronounToMuzM6.Text=CurrentPatternPronounTo.Shapes[12];
                    textBoxPatternPronounToMuzM7.Text=CurrentPatternPronounTo.Shapes[13];

                    textBoxPatternPronounToMunJ1.Text=CurrentPatternPronounTo.Shapes[14];
                    textBoxPatternPronounToMunJ2.Text=CurrentPatternPronounTo.Shapes[15];
                    textBoxPatternPronounToMunJ3.Text=CurrentPatternPronounTo.Shapes[16];
                    textBoxPatternPronounToMunJ4.Text=CurrentPatternPronounTo.Shapes[17];
                    textBoxPatternPronounToMunJ5.Text=CurrentPatternPronounTo.Shapes[18];
                    textBoxPatternPronounToMunJ6.Text=CurrentPatternPronounTo.Shapes[19];
                    textBoxPatternPronounToMunJ7.Text=CurrentPatternPronounTo.Shapes[20];
                    textBoxPatternPronounToMunM1.Text=CurrentPatternPronounTo.Shapes[21];
                    textBoxPatternPronounToMunM2.Text=CurrentPatternPronounTo.Shapes[22];
                    textBoxPatternPronounToMunM3.Text=CurrentPatternPronounTo.Shapes[23];
                    textBoxPatternPronounToMunM4.Text=CurrentPatternPronounTo.Shapes[24];
                    textBoxPatternPronounToMunM5.Text=CurrentPatternPronounTo.Shapes[25];
                    textBoxPatternPronounToMunM6.Text=CurrentPatternPronounTo.Shapes[26];
                    textBoxPatternPronounToMunM7.Text=CurrentPatternPronounTo.Shapes[27];

                    textBoxPatternPronounToZenJ1.Text=CurrentPatternPronounTo.Shapes[28];
                    textBoxPatternPronounToZenJ2.Text=CurrentPatternPronounTo.Shapes[29];
                    textBoxPatternPronounToZenJ3.Text=CurrentPatternPronounTo.Shapes[30];
                    textBoxPatternPronounToZenJ4.Text=CurrentPatternPronounTo.Shapes[31];
                    textBoxPatternPronounToZenJ5.Text=CurrentPatternPronounTo.Shapes[32];
                    textBoxPatternPronounToZenJ6.Text=CurrentPatternPronounTo.Shapes[33];
                    textBoxPatternPronounToZenJ7.Text=CurrentPatternPronounTo.Shapes[34];
                    textBoxPatternPronounToZenM1.Text=CurrentPatternPronounTo.Shapes[35];
                    textBoxPatternPronounToZenM2.Text=CurrentPatternPronounTo.Shapes[36];
                    textBoxPatternPronounToZenM3.Text=CurrentPatternPronounTo.Shapes[37];
                    textBoxPatternPronounToZenM4.Text=CurrentPatternPronounTo.Shapes[38];
                    textBoxPatternPronounToZenM5.Text=CurrentPatternPronounTo.Shapes[39];
                    textBoxPatternPronounToZenM6.Text=CurrentPatternPronounTo.Shapes[40];
                    textBoxPatternPronounToZenM7.Text=CurrentPatternPronounTo.Shapes[41];

                    textBoxPatternPronounToStrJ1.Text=CurrentPatternPronounTo.Shapes[42];
                    textBoxPatternPronounToStrJ2.Text=CurrentPatternPronounTo.Shapes[43];
                    textBoxPatternPronounToStrJ3.Text=CurrentPatternPronounTo.Shapes[44];
                    textBoxPatternPronounToStrJ4.Text=CurrentPatternPronounTo.Shapes[45];
                    textBoxPatternPronounToStrJ5.Text=CurrentPatternPronounTo.Shapes[46];
                    textBoxPatternPronounToStrJ6.Text=CurrentPatternPronounTo.Shapes[47];
                    textBoxPatternPronounToStrJ7.Text=CurrentPatternPronounTo.Shapes[48];
                    textBoxPatternPronounToStrM1.Text=CurrentPatternPronounTo.Shapes[49];
                    textBoxPatternPronounToStrM2.Text=CurrentPatternPronounTo.Shapes[50];
                    textBoxPatternPronounToStrM3.Text=CurrentPatternPronounTo.Shapes[51];
                    textBoxPatternPronounToStrM4.Text=CurrentPatternPronounTo.Shapes[52];
                    textBoxPatternPronounToStrM5.Text=CurrentPatternPronounTo.Shapes[53];
                    textBoxPatternPronounToStrM6.Text=CurrentPatternPronounTo.Shapes[54];
                    textBoxPatternPronounToStrM7.Text=CurrentPatternPronounTo.Shapes[55];


                    tableLayoutPanelPatternPronounToMun.Visible=true;
                    tableLayoutPanelPatternPronounToZen.Visible=true;
                    tableLayoutPanelPatternPronounToStr.Visible=true;
                    tableLayoutPanelPatternPronounToMuz.Visible=true;
                    break;
            }

            labelPatternPronounToName.Visible=true;
            textBoxPatternPronounToName.Visible=true;
            labelPatternPronounToType.Visible=true;
            comboBoxPatternPronounToType.Visible=true;

            labelPatternPronounToMuzFall.Visible=true;
            labelPatternPronounToMuzSingle.Visible=true;
            labelPatternPronounToMuzMultiple.Visible=true;
            tableLayoutPanelPatternPronounToMuz.Visible=true;

           //abelPatternPronounInfo.Visible=true;
            ChangeTypePatternPronounTo(CurrentPatternPronounTo?.Type);
        }

        void PatternPronounToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternPronounsToFiltered.Count; indexCur++) {
                if (itemsPatternPronounTo[indexCur]==CurrentPatternPronounTo) {
                    int indexList=listBoxPatternPronounTo.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternPronounTo.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternPronounToSaveCurrent() {
            if (CurrentPatternPronounTo==null) return;
            Edited=true;
            comboBoxPatternPronounToType.SelectedIndex=(int)CurrentPatternPronounTo.Type;
            CurrentPatternPronounTo.Name=textBoxPatternPronounToName.Text;
            switch (CurrentPatternPronounTo.Type) {
                case PronounType.Unknown:
                    break;

                case PronounType.NoDeklination:
                    if (CurrentPatternPronounTo.Shapes.Length==0)CurrentPatternPronounTo.Shapes=new string[]{""};
                    CurrentPatternPronounTo.Shapes[0]=textBoxPatternPronounToMuzJ1.Text;
                    break;

                case PronounType.DeklinationOnlySingle:
                    CurrentPatternPronounTo.Shapes[0]=textBoxPatternPronounToMuzJ1.Text;
                    CurrentPatternPronounTo.Shapes[1]=textBoxPatternPronounToMuzJ2.Text;
                    CurrentPatternPronounTo.Shapes[2]=textBoxPatternPronounToMuzJ3.Text;
                    CurrentPatternPronounTo.Shapes[3]=textBoxPatternPronounToMuzJ4.Text;
                    CurrentPatternPronounTo.Shapes[4]=textBoxPatternPronounToMuzJ5.Text;
                    CurrentPatternPronounTo.Shapes[5]=textBoxPatternPronounToMuzJ6.Text;
                    CurrentPatternPronounTo.Shapes[6]=textBoxPatternPronounToMuzJ7.Text;
                    break;

                case PronounType.Deklination:
                    CurrentPatternPronounTo.Shapes[0]=textBoxPatternPronounToMuzJ1.Text;
                    CurrentPatternPronounTo.Shapes[1]=textBoxPatternPronounToMuzJ2.Text;
                    CurrentPatternPronounTo.Shapes[2]=textBoxPatternPronounToMuzJ3.Text;
                    CurrentPatternPronounTo.Shapes[3]=textBoxPatternPronounToMuzJ4.Text;
                    CurrentPatternPronounTo.Shapes[4]=textBoxPatternPronounToMuzJ5.Text;
                    CurrentPatternPronounTo.Shapes[5]=textBoxPatternPronounToMuzJ6.Text;
                    CurrentPatternPronounTo.Shapes[6]=textBoxPatternPronounToMuzJ7.Text;

                    CurrentPatternPronounTo.Shapes[7]=textBoxPatternPronounToMuzM1.Text;
                    CurrentPatternPronounTo.Shapes[8]=textBoxPatternPronounToMuzM2.Text;
                    CurrentPatternPronounTo.Shapes[9]=textBoxPatternPronounToMuzM3.Text;
                    CurrentPatternPronounTo.Shapes[10]=textBoxPatternPronounToMuzM4.Text;
                    CurrentPatternPronounTo.Shapes[11]=textBoxPatternPronounToMuzM5.Text;
                    CurrentPatternPronounTo.Shapes[12]=textBoxPatternPronounToMuzM6.Text;
                    CurrentPatternPronounTo.Shapes[13]=textBoxPatternPronounToMuzM7.Text;
                    break;

                case PronounType.DeklinationWithGender:
                    CurrentPatternPronounTo.Shapes[0]=textBoxPatternPronounToMuzJ1.Text;
                    CurrentPatternPronounTo.Shapes[1]=textBoxPatternPronounToMuzJ2.Text;
                    CurrentPatternPronounTo.Shapes[2]=textBoxPatternPronounToMuzJ3.Text;
                    CurrentPatternPronounTo.Shapes[3]=textBoxPatternPronounToMuzJ4.Text;
                    CurrentPatternPronounTo.Shapes[4]=textBoxPatternPronounToMuzJ5.Text;
                    CurrentPatternPronounTo.Shapes[5]=textBoxPatternPronounToMuzJ6.Text;
                    CurrentPatternPronounTo.Shapes[6]=textBoxPatternPronounToMuzJ7.Text;
                    CurrentPatternPronounTo.Shapes[7]=textBoxPatternPronounToMuzM1.Text;
                    CurrentPatternPronounTo.Shapes[8]=textBoxPatternPronounToMuzM2.Text;
                    CurrentPatternPronounTo.Shapes[9]=textBoxPatternPronounToMuzM3.Text;
                    CurrentPatternPronounTo.Shapes[10]=textBoxPatternPronounToMuzM4.Text;
                    CurrentPatternPronounTo.Shapes[11]=textBoxPatternPronounToMuzM5.Text;
                    CurrentPatternPronounTo.Shapes[12]=textBoxPatternPronounToMuzM6.Text;
                    CurrentPatternPronounTo.Shapes[13]=textBoxPatternPronounToMuzM7.Text;

                    CurrentPatternPronounTo.Shapes[14]=textBoxPatternPronounToMunJ1.Text;
                    CurrentPatternPronounTo.Shapes[15]=textBoxPatternPronounToMunJ2.Text;
                    CurrentPatternPronounTo.Shapes[16]=textBoxPatternPronounToMunJ3.Text;
                    CurrentPatternPronounTo.Shapes[17]=textBoxPatternPronounToMunJ4.Text;
                    CurrentPatternPronounTo.Shapes[18]=textBoxPatternPronounToMunJ5.Text;
                    CurrentPatternPronounTo.Shapes[19]=textBoxPatternPronounToMunJ6.Text;
                    CurrentPatternPronounTo.Shapes[20]=textBoxPatternPronounToMunJ7.Text;
                    CurrentPatternPronounTo.Shapes[21]=textBoxPatternPronounToMunM1.Text;
                    CurrentPatternPronounTo.Shapes[22]=textBoxPatternPronounToMunM2.Text;
                    CurrentPatternPronounTo.Shapes[23]=textBoxPatternPronounToMunM3.Text;
                    CurrentPatternPronounTo.Shapes[24]=textBoxPatternPronounToMunM4.Text;
                    CurrentPatternPronounTo.Shapes[25]=textBoxPatternPronounToMunM5.Text;
                    CurrentPatternPronounTo.Shapes[26]=textBoxPatternPronounToMunM6.Text;
                    CurrentPatternPronounTo.Shapes[27]=textBoxPatternPronounToMunM7.Text;

                    CurrentPatternPronounTo.Shapes[28]=textBoxPatternPronounToZenJ1.Text;
                    CurrentPatternPronounTo.Shapes[29]=textBoxPatternPronounToZenJ2.Text;
                    CurrentPatternPronounTo.Shapes[30]=textBoxPatternPronounToZenJ3.Text;
                    CurrentPatternPronounTo.Shapes[31]=textBoxPatternPronounToZenJ4.Text;
                    CurrentPatternPronounTo.Shapes[32]=textBoxPatternPronounToZenJ5.Text;
                    CurrentPatternPronounTo.Shapes[33]=textBoxPatternPronounToZenJ6.Text;
                    CurrentPatternPronounTo.Shapes[34]=textBoxPatternPronounToZenJ7.Text;
                    CurrentPatternPronounTo.Shapes[35]=textBoxPatternPronounToZenM1.Text;
                    CurrentPatternPronounTo.Shapes[36]=textBoxPatternPronounToZenM2.Text;
                    CurrentPatternPronounTo.Shapes[37]=textBoxPatternPronounToZenM3.Text;
                    CurrentPatternPronounTo.Shapes[38]=textBoxPatternPronounToZenM4.Text;
                    CurrentPatternPronounTo.Shapes[39]=textBoxPatternPronounToZenM5.Text;
                    CurrentPatternPronounTo.Shapes[40]=textBoxPatternPronounToZenM6.Text;
                    CurrentPatternPronounTo.Shapes[41]=textBoxPatternPronounToZenM7.Text;

                    CurrentPatternPronounTo.Shapes[42]=textBoxPatternPronounToStrJ1.Text;
                    CurrentPatternPronounTo.Shapes[43]=textBoxPatternPronounToStrJ2.Text;
                    CurrentPatternPronounTo.Shapes[44]=textBoxPatternPronounToStrJ3.Text;
                    CurrentPatternPronounTo.Shapes[45]=textBoxPatternPronounToStrJ4.Text;
                    CurrentPatternPronounTo.Shapes[46]=textBoxPatternPronounToStrJ5.Text;
                    CurrentPatternPronounTo.Shapes[47]=textBoxPatternPronounToStrJ6.Text;
                    CurrentPatternPronounTo.Shapes[48]=textBoxPatternPronounToStrJ7.Text;
                    CurrentPatternPronounTo.Shapes[49]=textBoxPatternPronounToStrM1.Text;
                    CurrentPatternPronounTo.Shapes[50]=textBoxPatternPronounToStrM2.Text;
                    CurrentPatternPronounTo.Shapes[51]=textBoxPatternPronounToStrM3.Text;
                    CurrentPatternPronounTo.Shapes[52]=textBoxPatternPronounToStrM4.Text;
                    CurrentPatternPronounTo.Shapes[53]=textBoxPatternPronounToStrM5.Text;
                    CurrentPatternPronounTo.Shapes[54]=textBoxPatternPronounToStrM6.Text;
                    CurrentPatternPronounTo.Shapes[55]=textBoxPatternPronounToStrM7.Text;
                    break;
            }
        }

        void PatternPronounToSetNone(){
            textBoxPatternPronounToName.Text="";

            textBoxPatternPronounToMuzJ1.Text="";
            textBoxPatternPronounToMuzJ2.Text="";
            textBoxPatternPronounToMuzJ3.Text="";
            textBoxPatternPronounToMuzJ4.Text="";
            textBoxPatternPronounToMuzJ5.Text="";
            textBoxPatternPronounToMuzJ6.Text="";
            textBoxPatternPronounToMuzJ7.Text="";

            textBoxPatternPronounToMuzM1.Text="";
            textBoxPatternPronounToMuzM2.Text="";
            textBoxPatternPronounToMuzM3.Text="";
            textBoxPatternPronounToMuzM4.Text="";
            textBoxPatternPronounToMuzM5.Text="";
            textBoxPatternPronounToMuzM6.Text="";
            textBoxPatternPronounToMuzM7.Text="";


            textBoxPatternPronounToZenJ1.Text="";
            textBoxPatternPronounToZenJ2.Text="";
            textBoxPatternPronounToZenJ3.Text="";
            textBoxPatternPronounToZenJ4.Text="";
            textBoxPatternPronounToZenJ5.Text="";
            textBoxPatternPronounToZenJ6.Text="";
            textBoxPatternPronounToZenJ7.Text="";

            textBoxPatternPronounToZenM1.Text="";
            textBoxPatternPronounToZenM2.Text="";
            textBoxPatternPronounToZenM3.Text="";
            textBoxPatternPronounToZenM4.Text="";
            textBoxPatternPronounToZenM5.Text="";
            textBoxPatternPronounToZenM6.Text="";
            textBoxPatternPronounToZenM7.Text="";

            textBoxPatternPronounToMunJ1.Text="";
            textBoxPatternPronounToMunJ2.Text="";
            textBoxPatternPronounToMunJ3.Text="";
            textBoxPatternPronounToMunJ4.Text="";
            textBoxPatternPronounToMunJ5.Text="";
            textBoxPatternPronounToMunJ6.Text="";
            textBoxPatternPronounToMunJ7.Text="";

            textBoxPatternPronounToMunM1.Text="";
            textBoxPatternPronounToMunM2.Text="";
            textBoxPatternPronounToMunM3.Text="";
            textBoxPatternPronounToMunM4.Text="";
            textBoxPatternPronounToMunM5.Text="";
            textBoxPatternPronounToMunM2.Text="";
            textBoxPatternPronounToMunM7.Text="";

            textBoxPatternPronounToStrJ1.Text="";
            textBoxPatternPronounToStrJ2.Text="";
            textBoxPatternPronounToStrJ3.Text="";
            textBoxPatternPronounToStrJ4.Text="";
            textBoxPatternPronounToStrJ5.Text="";
            textBoxPatternPronounToStrJ6.Text="";
            textBoxPatternPronounToStrJ7.Text="";

            textBoxPatternPronounToStrM1.Text="";
            textBoxPatternPronounToStrM2.Text="";
            textBoxPatternPronounToStrM3.Text="";
            textBoxPatternPronounToStrM4.Text="";
            textBoxPatternPronounToStrM5.Text="";
            textBoxPatternPronounToStrM6.Text="";
            textBoxPatternPronounToStrM7.Text="";

            labelPatternPronounToName.Visible=false;
            textBoxPatternPronounToName.Visible=false;
            labelPatternPronounType.Visible=false;
            comboBoxPatternPronounToType.Visible=false;

            labelPatternPronounToMuzFall.Visible=false;
            labelPatternPronounToMuzSingle.Visible=false;
            labelPatternPronounToMuzMultiple.Visible=false;
            tableLayoutPanelPatternPronounToMuz.Visible=false;
            //labelPatternPronounInfo.Visible=false;
           // ChangeTypePatternPronoun(CurrentPatternPronoun?.Type);
        }

        void PatternPronounToClear() {
            listBoxPatternPronounTo.Items.Clear();
            PatternPronounToSetNone();
            itemsPatternPronounsToFiltered?.Clear();
            itemsPatternPronounTo?.Clear();
            CurrentPatternPronounTo=null;
        }
        #endregion

        #region Pronoun
        void ListBoxPronoun_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PronounSaveCurrent();

            int index=listBoxPronoun.SelectedIndex;
            if (itemsPronouns.Count==0) {
                PronounSetNone();
                return;
            }
            if (index>=itemsPronouns.Count)
                index=itemsPronouns.Count-1;
            if (index<0)
                index=0;

            CurrentPronoun=itemsPronouns[index];
            SetCurrentPronoun();
            PronounSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonPronounAdd_Click(object sender, EventArgs e) {
            AddNewItemPronoun();
        }

        void ButtonPronounRemove_Click(object sender, EventArgs e) {
            RemoveItemPronoun(CurrentPronoun);
            TextBoxPronounFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxPronounFilter_TextChanged(object sender, EventArgs e) {
            PronounSaveCurrent();

            // Získej aktuální prvek
            ItemPronoun selectedId=null;
            if (listBoxPronoun.SelectedIndex!=-1) {
                selectedId=itemsPronounsFiltered[listBoxPronoun.SelectedIndex];
            }

            PronounRefreshFilteredList();

            listBoxPronoun.Items.Clear();
            for (int i=0; i<itemsPronounsFiltered.Count; i++) {
                ItemPronoun item = itemsPronounsFiltered[i];

                string textToAdd=item.GetText(itemsPatternPronounFrom.Cast<ItemTranslatingPattern>().ToList(), itemsPatternPronounTo.Cast<ItemTranslatingPattern>().ToList());
                //if (string.IsNullOrEmpty(textToAdd)) {
                //    if (string.IsNullOrEmpty(item.PatternFrom)) {
                //        textToAdd="<Neznámé>";
                //    } else textToAdd="{"+item.PatternFrom+"}";
                //}

                listBoxPronoun.Items.Add(textToAdd);
            }

            //PronounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPronounsFiltered.Count; i++){
                    if (itemsPronounsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxPronoun.SelectedIndex=-1;
                    CurrentPronoun=null;
                } else listBoxPronoun.SelectedIndex=outIndex;
            } else {
                listBoxPronoun.SelectedIndex=-1;
                CurrentPronoun=null;
            }
            SetCurrentPronoun();
        }

        void RemoveCurrentPronoun(object sender, EventArgs e) {
            itemsPronouns.Remove(CurrentPronoun);
        }

        void PronounSetListBox() {
            //string filter=textBoxPronounFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxPronoun.SelectedIndex;
            listBoxPronoun.Items.Clear();
            for (int i=0; i<itemsPronounsFiltered.Count; i++) {
                ItemPronoun item = itemsPronounsFiltered[i];

                string textToAdd=item.GetText(itemsPatternPronounFrom.Cast<ItemTranslatingPattern>().ToList(),itemsPatternPronounTo.Cast<ItemTranslatingPattern>().ToList());
                //if (string.IsNullOrEmpty(textToAdd)) {
                //    if (string.IsNullOrEmpty(item.PatternFrom)) {
                //        textToAdd="<Neznámé>";
                //    } else textToAdd="{"+item.PatternFrom+"}";
                //}
                listBoxPronoun.Items.Add(textToAdd);
            }

            if (index>=listBoxPronoun.Items.Count)index=listBoxPronoun.Items.Count-1;
            listBoxPronoun.SelectedIndex=index;
        }

        void PronounRefreshFilteredList() {
            if (itemsPronounsFiltered==null) itemsPronounsFiltered=new List<ItemPronoun>();
            itemsPronounsFiltered.Clear();
            string filter=textBoxPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPronouns.Count; i++) {
                    ItemPronoun item = itemsPronouns[i];

                    if (item.Filter(filter)) {
                        itemsPronounsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPronouns.Count; i++) {
                    ItemPronoun item = itemsPronouns[i];
                    itemsPronounsFiltered.Add(item);
                }
            }
        }

        void AddNewItemPronoun() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PronounSaveCurrent();

            var newItem=new ItemPronoun();
            newItem.To=new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern()};
           // newItem.ID=itemsPronouns.Count;
            itemsPronouns.Add(newItem);
            CurrentPronoun=newItem;
            PronounRefreshFilteredList();
            PronounSetListBox();
            ListBoxSetCurrentPronoun();
            SetCurrentPronoun();

            doingJob=false;
        }

        void RemoveItemPronoun(ItemPronoun item) {
            Edited=true;
            ChangeCaptionText();
            itemsPronouns.Remove(item);
            PronounRefreshFilteredList();
            PronounSetListBox();
            SetCurrentPronoun();
        }

        void SetCurrentPronoun(){
            if (itemsPronounsFiltered.Count==0) {
                PronounSetNone();
                return;
            }

            int index=listBoxPronoun.SelectedIndex;
            if (index>=itemsPronounsFiltered.Count) index=itemsPronounsFiltered.Count-1;
            if (index<0) index=0;
            CurrentPronoun=itemsPronounsFiltered[index];

            textBoxPronounFrom.Visible=true;
          //  textBoxPronounTo.Visible=true;
            labelPronounFrom.Visible=true;
           // labelPronounTo.Visible=true;

            textBoxPronounFrom.Text=CurrentPronoun.From;
            //textBoxPronounTo.Text=CurrentPronoun.To;

            comboBoxPronounInputPatternFrom.Text=CurrentPronoun.PatternFrom;

            comboBoxPronounInputPatternFrom.Items.Clear();
          //  comboBoxPronounInputPatternTo.Items.Clear();
            foreach (ItemPatternPronoun x in itemsPatternPronounFrom) {
                comboBoxPronounInputPatternFrom.Items.Add(x.Name);
            }
         //   foreach (ItemPatternPronoun x in itemsPatternPronounTo) {
                //comboBoxPronounInputPatternTo.Items.Add(x.Name);
          //  }
            //comboBoxPronounInputPatternTo.Text=CurrentPronoun.PatternTo;
            //buttonAddToPronoun.Visible=true;
            simpleUIPronouns.Visible=true;
            simpleUIPronouns.SetData(CurrentPronoun.To.ToArray());
            List<string> options=new List<string>();
            foreach (ItemPatternPronoun x in itemsPatternPronounTo) options.Add(x.Name);
            simpleUIPronouns.SetComboboxes(options.ToArray());

            comboBoxPronounInputPatternFrom.Visible=true;
          //  comboBoxPronounInputPatternTo.Visible=true;

            labelPronounInputPatternFrom.Visible=true;
         //   labelPronounInputPatternTo.Visible=true;

            labelPronounShowFrom.Visible=true;
         //   labelPronounShowTo.Visible=true;
        }

        void ListBoxSetCurrentPronoun() {
            for (int indexCur=0; indexCur<itemsPronounsFiltered.Count; indexCur++) {
                if (itemsPronouns[indexCur]==CurrentPronoun) {
                    int indexList=listBoxPronoun.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPronoun.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PronounSaveCurrent() {
            if (CurrentPronoun==null) return;
            Edited=true;

            CurrentPronoun.From=textBoxPronounFrom.Text;
           // CurrentPronoun.To=textBoxPronounTo.Text;

            CurrentPronoun.PatternFrom=comboBoxPronounInputPatternFrom.Text;
         //   CurrentPronoun.PatternTo=comboBoxPronounInputPatternTo.Text;

            CurrentPronoun.To=simpleUIPronouns.GetData().ToList();
        }

        void PronounSetNone(){
            textBoxPronounFrom.Text="";
        //    textBoxPronounTo.Text="";

            comboBoxPronounInputPatternFrom.Text="";
            //comboBoxPronounInputPatternTo.Text="";
            //buttonAddToPronoun.Visible=false;
            textBoxPronounFrom.Visible=false;
           // textBoxPronounTo.Visible=false;
            labelPronounFrom.Visible=false;
          //  labelPronounTo.Visible=false;
          //  comboBoxPronounInputPatternTo.Visible=false;
            comboBoxPronounInputPatternFrom.Visible=false;
            labelPronounShowFrom.Visible=false;
           // labelPronounShowTo.Visible=false;
            labelPronounInputPatternFrom.Visible=false;
            //labelPronounInputPatternTo.Visible=false;
            simpleUIPronouns.Visible=false;
            simpleUIPronouns.Clear();
        }

        void ClearPronoun(){
            listBoxPronoun.Items.Clear();
            PronounSetNone();
            itemsPronounsFiltered?.Clear();
            itemsPronouns?.Clear();
            CurrentPronoun=null;
        }
        #endregion

        #region NumberPattern From
        void PatternNumberFromListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternNumberFromSaveCurrent();

            int index=PatternNumberFromlistBox.SelectedIndex;
            if (itemsPatternNumberFrom.Count==0) {
                PatternNumberFromSetNone();
                return;
            }
            if (index>=itemsPatternNumberFrom.Count)
                index=itemsPatternNumberFrom.Count-1;
            if (index<0)
                index=0;

            CurrentPatternNumberFrom=itemsPatternNumberFrom[index];
            PatternNumberFromSetCurrent();
            PatternNumberFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternNumberFromButtonAdd_Click(object sender, EventArgs e) {
            PatternNumberFromAddNewItem();
        }

        void PatternNumberFromButtonRemove_Click(object sender, EventArgs e) {
            PatternNumberFromRemoveItem(CurrentPatternNumberFrom);
            PatternNumberFromTextBoxFilter_TextChanged(null, new EventArgs());
        }

        void PatternNumberFromTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternNumberFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternNumber selectedId=null;
            if (PatternNumberFromlistBox.SelectedIndex!=-1) {
                selectedId=itemsPatternNumbersFromFiltered[PatternNumberFromlistBox.SelectedIndex];
            }

            PatternNumberFromRefreshFilteredList();

            PatternNumberFromlistBox.Items.Clear();
            for (int i=0; i<itemsPatternNumbersFromFiltered.Count; i++) {
                ItemPatternNumber item = itemsPatternNumbersFromFiltered[i];

                string textToAdd=item.GetText();
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternNumberFromlistBox.Items.Add(textToAdd);
            }

            //SetListBoxNumber();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternNumbersFromFiltered.Count; i++){
                    if (itemsPatternNumbersFromFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    PatternNumberFromlistBox.SelectedIndex=-1;
                    CurrentPatternNumberFrom=null;
                } else PatternNumberFromlistBox.SelectedIndex=outIndex;
            } else {
                PatternNumberFromlistBox.SelectedIndex=-1;
                CurrentPatternNumberFrom=null;
            }
            PatternNumberFromSetCurrent();
        }

        void PatternNumberFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternNumberFrom.Remove(CurrentPatternNumberFrom);
        }

        void PatternNumberFromSetListBox() {
            //string filter=textBoxPatternNumberFromFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=PatternNumberFromlistBox.SelectedIndex;
            PatternNumberFromlistBox.Items.Clear();
            for (int i=0; i<itemsPatternNumbersFromFiltered.Count; i++) {
                ItemPatternNumber item = itemsPatternNumbersFromFiltered[i];

                string textToAdd=item.GetText();
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternNumberFromlistBox.Items.Add(textToAdd);
            }

            if (index>=PatternNumberFromlistBox.Items.Count)index=PatternNumberFromlistBox.Items.Count-1;
            PatternNumberFromlistBox.SelectedIndex=index;
        }

        void PatternNumberFromRefreshFilteredList() {
            if (itemsPatternNumbersFromFiltered==null) itemsPatternNumbersFromFiltered=new List<ItemPatternNumber>();
            itemsPatternNumbersFromFiltered.Clear();
            string filter=textBoxPatternNumberFromFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternNumberFrom.Count; i++) {
                    ItemPatternNumber item = itemsPatternNumberFrom[i];

                    if (item.Filter(filter)) {
                        itemsPatternNumbersFromFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternNumberFrom.Count; i++) {
                    ItemPatternNumber item = itemsPatternNumberFrom[i];
                    itemsPatternNumbersFromFiltered.Add(item);
                }
            }
        }

        void PatternNumberFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternNumberFromSaveCurrent();

            var newItem=new ItemPatternNumber();
           // newItem.ID=itemsNumbers.Count;
            itemsPatternNumberFrom.Add(newItem);
            CurrentPatternNumberFrom=newItem;
            PatternNumberFromRefreshFilteredList();
            PatternNumberFromSetListBox();
            PatternNumberFromListBoxSetCurrent();
            PatternNumberFromSetCurrent();

            doingJob=false;
        }

        void PatternNumberFromRemoveItem(ItemPatternNumber item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternNumberFrom.Remove(item);
            PatternNumberFromRefreshFilteredList();
            PatternNumberFromSetListBox();
            PatternNumberFromSetCurrent();
        }

        void PatternNumberFromSetCurrent(){
            if (itemsPatternNumbersFromFiltered.Count==0) {
                PatternNumberFromSetNone();
                return;
            }

            int index=PatternNumberFromlistBox.SelectedIndex;
            if (index>=itemsPatternNumbersFromFiltered.Count) index=itemsPatternNumbersFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternNumberFrom=itemsPatternNumbersFromFiltered[index];
            textBoxPatternNumberFromName.Text=CurrentPatternNumberFrom.Name;
            comboBoxPatternNumberFromType.SelectedIndex=(int)CurrentPatternNumberFrom.ShowType;

            if (CurrentPatternNumberFrom.ShowType!=NumberType.Unknown) {
                if (CurrentPatternNumberFrom.ShowType==NumberType.NoDeklination || CurrentPatternNumberFrom.ShowType==NumberType.Deklination || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberFromMuzS1.Text=CurrentPatternNumberFrom.Shapes[0];
                    textBoxPatternNumberFromMuzS1.Visible=true;

                    textBoxPatternNumberFromName.Visible=true;
                    labelPatternNumberFromMuzFall.Visible=true;
                    labelPatternNumberFromMuzMultiple.Visible=true;
                    labelPatternNumberFromMuzSingle.Visible=true;
                } else {
                    textBoxPatternNumberFromMuzS1.Visible=false;
                    textBoxPatternNumberFromName.Visible=false;
                    labelPatternNumberFromMuzFall.Visible=false;
                    labelPatternNumberFromMuzMultiple.Visible=false;
                    labelPatternNumberFromMuzSingle.Visible=false;
                }
                if (CurrentPatternNumberFrom.ShowType==NumberType.Deklination || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberFromMuzS2.Text=CurrentPatternNumberFrom.Shapes[1];
                    textBoxPatternNumberFromMuzS3.Text=CurrentPatternNumberFrom.Shapes[2];
                    textBoxPatternNumberFromMuzS4.Text=CurrentPatternNumberFrom.Shapes[3];
                    textBoxPatternNumberFromMuzS5.Text=CurrentPatternNumberFrom.Shapes[4];
                    textBoxPatternNumberFromMuzS6.Text=CurrentPatternNumberFrom.Shapes[5];
                    textBoxPatternNumberFromMuzS7.Text=CurrentPatternNumberFrom.Shapes[6];
                    textBoxPatternNumberFromMuzS2.Visible=true;
                    textBoxPatternNumberFromMuzS3.Visible=true;
                    textBoxPatternNumberFromMuzS4.Visible=true;
                    textBoxPatternNumberFromMuzS5.Visible=true;
                    textBoxPatternNumberFromMuzS6.Visible=true;
                    textBoxPatternNumberFromMuzS7.Visible=true;
                } else {
                    textBoxPatternNumberFromMuzS2.Visible=false;
                    textBoxPatternNumberFromMuzS3.Visible=false;
                    textBoxPatternNumberFromMuzS4.Visible=false;
                    textBoxPatternNumberFromMuzS5.Visible=false;
                    textBoxPatternNumberFromMuzS6.Visible=false;
                    textBoxPatternNumberFromMuzS7.Visible=false;
                }
                if (CurrentPatternNumberFrom.ShowType==NumberType.Deklination || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberFromMuzM1.Text=CurrentPatternNumberFrom.Shapes[7];
                    textBoxPatternNumberFromMuzM2.Text=CurrentPatternNumberFrom.Shapes[8];
                    textBoxPatternNumberFromMuzM3.Text=CurrentPatternNumberFrom.Shapes[9];
                    textBoxPatternNumberFromMuzM4.Text=CurrentPatternNumberFrom.Shapes[10];
                    textBoxPatternNumberFromMuzM5.Text=CurrentPatternNumberFrom.Shapes[11];
                    textBoxPatternNumberFromMuzM6.Text=CurrentPatternNumberFrom.Shapes[12];
                    textBoxPatternNumberFromMuzM7.Text=CurrentPatternNumberFrom.Shapes[13];
                    textBoxPatternNumberFromMuzM1.Visible=true;
                    textBoxPatternNumberFromMuzM2.Visible=true;
                    textBoxPatternNumberFromMuzM3.Visible=true;
                    textBoxPatternNumberFromMuzM4.Visible=true;
                    textBoxPatternNumberFromMuzM5.Visible=true;
                    textBoxPatternNumberFromMuzM6.Visible=true;
                    textBoxPatternNumberFromMuzM7.Visible=true;
                    labelPatternNumberFromMuz.Visible=false;
                }else{
                    textBoxPatternNumberFromMuzM1.Visible=false;
                    textBoxPatternNumberFromMuzM2.Visible=false;
                    textBoxPatternNumberFromMuzM3.Visible=false;
                    textBoxPatternNumberFromMuzM4.Visible=false;
                    textBoxPatternNumberFromMuzM5.Visible=false;
                    textBoxPatternNumberFromMuzM6.Visible=false;
                    textBoxPatternNumberFromMuzM7.Visible=false;
                    labelPatternNumberFromMuz.Visible=true;
                }
                if (CurrentPatternNumberFrom.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberFromMunS1.Text=CurrentPatternNumberFrom.Shapes[14+0];
                    textBoxPatternNumberFromMunS2.Text=CurrentPatternNumberFrom.Shapes[14+1];
                    textBoxPatternNumberFromMunS3.Text=CurrentPatternNumberFrom.Shapes[14+2];
                    textBoxPatternNumberFromMunS4.Text=CurrentPatternNumberFrom.Shapes[14+3];
                    textBoxPatternNumberFromMunS5.Text=CurrentPatternNumberFrom.Shapes[14+4];
                    textBoxPatternNumberFromMunS6.Text=CurrentPatternNumberFrom.Shapes[14+5];
                    textBoxPatternNumberFromMunS7.Text=CurrentPatternNumberFrom.Shapes[14+6];
                    textBoxPatternNumberFromMunM1.Text=CurrentPatternNumberFrom.Shapes[14+7];
                    textBoxPatternNumberFromMunM2.Text=CurrentPatternNumberFrom.Shapes[14+8];
                    textBoxPatternNumberFromMunM3.Text=CurrentPatternNumberFrom.Shapes[14+9];
                    textBoxPatternNumberFromMunM4.Text=CurrentPatternNumberFrom.Shapes[14+10];
                    textBoxPatternNumberFromMunM5.Text=CurrentPatternNumberFrom.Shapes[14+11];
                    textBoxPatternNumberFromMunM6.Text=CurrentPatternNumberFrom.Shapes[14+12];
                    textBoxPatternNumberFromMunM7.Text=CurrentPatternNumberFrom.Shapes[14+13];

                    textBoxPatternNumberFromZenS1.Text=CurrentPatternNumberFrom.Shapes[28+0];
                    textBoxPatternNumberFromZenS2.Text=CurrentPatternNumberFrom.Shapes[28+1];
                    textBoxPatternNumberFromZenS3.Text=CurrentPatternNumberFrom.Shapes[28+2];
                    textBoxPatternNumberFromZenS4.Text=CurrentPatternNumberFrom.Shapes[28+3];
                    textBoxPatternNumberFromZenS5.Text=CurrentPatternNumberFrom.Shapes[28+4];
                    textBoxPatternNumberFromZenS6.Text=CurrentPatternNumberFrom.Shapes[28+5];
                    textBoxPatternNumberFromZenS7.Text=CurrentPatternNumberFrom.Shapes[28+6];
                    textBoxPatternNumberFromZenM1.Text=CurrentPatternNumberFrom.Shapes[28+7];
                    textBoxPatternNumberFromZenM2.Text=CurrentPatternNumberFrom.Shapes[28+8];
                    textBoxPatternNumberFromZenM3.Text=CurrentPatternNumberFrom.Shapes[28+9];
                    textBoxPatternNumberFromZenM4.Text=CurrentPatternNumberFrom.Shapes[28+10];
                    textBoxPatternNumberfromZenM5.Text=CurrentPatternNumberFrom.Shapes[28+11];
                    textBoxPatternNumberFromZenM6.Text=CurrentPatternNumberFrom.Shapes[28+12];
                    textBoxPatternNumberFromZenM7.Text=CurrentPatternNumberFrom.Shapes[28+13];

                    textBoxPatternNumberFromStrS1.Text=CurrentPatternNumberFrom.Shapes[42+0];
                    textBoxPatternNumberFromStrS2.Text=CurrentPatternNumberFrom.Shapes[42+1];
                    textBoxPatternNumberFromStrS3.Text=CurrentPatternNumberFrom.Shapes[42+2];
                    textBoxPatternNumberFromStrS4.Text=CurrentPatternNumberFrom.Shapes[42+3];
                    textBoxPatternNumberFromStrS5.Text=CurrentPatternNumberFrom.Shapes[42+4];
                    textBoxPatternNumberFromStrS6.Text=CurrentPatternNumberFrom.Shapes[42+5];
                    textBoxPatternNumberFromStrS7.Text=CurrentPatternNumberFrom.Shapes[42+6];
                    textBoxPatternNumberFromStrM1.Text=CurrentPatternNumberFrom.Shapes[42+7];
                    textBoxPatternNumberFromStrM2.Text=CurrentPatternNumberFrom.Shapes[42+8];
                    textBoxPatternNumberFromStrM3.Text=CurrentPatternNumberFrom.Shapes[42+9];
                    textBoxPatternNumberFromStrM4.Text=CurrentPatternNumberFrom.Shapes[42+10];
                    textBoxPatternNumberFromStrM5.Text=CurrentPatternNumberFrom.Shapes[42+11];
                    textBoxPatternNumberFromStrM6.Text=CurrentPatternNumberFrom.Shapes[42+12];
                    textBoxPatternNumberFromStrM7.Text=CurrentPatternNumberFrom.Shapes[42+13];

                    textBoxPatternNumberFromMunS1.Visible=true;
                    textBoxPatternNumberFromMunS2.Visible=true;
                    textBoxPatternNumberFromMunS3.Visible=true;
                    textBoxPatternNumberFromMunS4.Visible=true;
                    textBoxPatternNumberFromMunS5.Visible=true;
                    textBoxPatternNumberFromMunS6.Visible=true;
                    textBoxPatternNumberFromMunS7.Visible=true;
                    textBoxPatternNumberFromMunM1.Visible=true;
                    textBoxPatternNumberFromMunM2.Visible=true;
                    textBoxPatternNumberFromMunM3.Visible=true;
                    textBoxPatternNumberFromMunM4.Visible=true;
                    textBoxPatternNumberFromMunM5.Visible=true;
                    textBoxPatternNumberFromMunM6.Visible=true;
                    textBoxPatternNumberFromMunM7.Visible=true;

                    textBoxPatternNumberFromZenS1.Visible=true;
                    textBoxPatternNumberFromZenS2.Visible=true;
                    textBoxPatternNumberFromZenS3.Visible=true;
                    textBoxPatternNumberFromZenS4.Visible=true;
                    textBoxPatternNumberFromZenS5.Visible=true;
                    textBoxPatternNumberFromZenS6.Visible=true;
                    textBoxPatternNumberFromZenS7.Visible=true;
                    textBoxPatternNumberFromZenM1.Visible=true;
                    textBoxPatternNumberFromZenM2.Visible=true;
                    textBoxPatternNumberFromZenM3.Visible=true;
                    textBoxPatternNumberFromZenM4.Visible=true;
                    textBoxPatternNumberfromZenM5.Visible=true;
                    textBoxPatternNumberFromZenM6.Visible=true;
                    textBoxPatternNumberFromZenM7.Visible=true;

                    textBoxPatternNumberFromStrS1.Visible=true;
                    textBoxPatternNumberFromStrS2.Visible=true;
                    textBoxPatternNumberFromStrS3.Visible=true;
                    textBoxPatternNumberFromStrS4.Visible=true;
                    textBoxPatternNumberFromStrS5.Visible=true;
                    textBoxPatternNumberFromStrS6.Visible=true;
                    textBoxPatternNumberFromStrS7.Visible=true;
                    textBoxPatternNumberFromStrM1.Visible=true;
                    textBoxPatternNumberFromStrM2.Visible=true;
                    textBoxPatternNumberFromStrM3.Visible=true;
                    textBoxPatternNumberFromStrM4.Visible=true;
                    textBoxPatternNumberFromStrM5.Visible=true;
                    textBoxPatternNumberFromStrM6.Visible=true;
                    textBoxPatternNumberFromStrM7.Visible=true;

                    labelPatternNumberFromMun.Visible=true;
                    labelPatternNumberFromMunFall.Visible=true;
                    labelPatternNumberFromMunSingle.Visible=true;
                    labelPatternNumberFromMunMultiple.Visible=true;

                    labelPatternNumberFromZen.Visible=true;
                    labelPatternNumberFromZenFall.Visible=true;
                    labelPatternNumberFromZenSingle.Visible=true;
                    labelPatternNumberFromZenMultiple.Visible=true;

                    labelPatternNumberFromStr.Visible=true;
                    labelPatternNumberFromStrFall.Visible=true;
                    labelPatternNumberFromStrSingle.Visible=true;
                    labelPatternNumberFromStrMultiple.Visible=true;

                    tableLayoutPanelPatternNumberFromStr.Visible=true;
                    tableLayoutPanelPatternNumberFromZen.Visible=true;
                    tableLayoutPanelPatternNumberFromMun.Visible=true;
                }else{
                    textBoxPatternNumberFromMunS1.Visible=false;
                    textBoxPatternNumberFromMunS2.Visible=false;
                    textBoxPatternNumberFromMunS3.Visible=false;
                    textBoxPatternNumberFromMunS4.Visible=false;
                    textBoxPatternNumberFromMunS5.Visible=false;
                    textBoxPatternNumberFromMunS6.Visible=false;
                    textBoxPatternNumberFromMunS7.Visible=false;
                    textBoxPatternNumberFromMunM1.Visible=false;
                    textBoxPatternNumberFromMunM2.Visible=false;
                    textBoxPatternNumberFromMunM3.Visible=false;
                    textBoxPatternNumberFromMunM4.Visible=false;
                    textBoxPatternNumberFromMunM5.Visible=false;
                    textBoxPatternNumberFromMunM6.Visible=false;
                    textBoxPatternNumberFromMunM7.Visible=false;

                    textBoxPatternNumberFromZenS1.Visible=false;
                    textBoxPatternNumberFromZenS2.Visible=false;
                    textBoxPatternNumberFromZenS3.Visible=false;
                    textBoxPatternNumberFromZenS4.Visible=false;
                    textBoxPatternNumberFromZenS5.Visible=false;
                    textBoxPatternNumberFromZenS6.Visible=false;
                    textBoxPatternNumberFromZenS7.Visible=false;
                    textBoxPatternNumberFromZenM1.Visible=false;
                    textBoxPatternNumberFromZenM2.Visible=false;
                    textBoxPatternNumberFromZenM3.Visible=false;
                    textBoxPatternNumberFromZenM4.Visible=false;
                    textBoxPatternNumberfromZenM5.Visible=false;
                    textBoxPatternNumberFromZenM6.Visible=false;
                    textBoxPatternNumberFromZenM7.Visible=false;

                    textBoxPatternNumberFromStrS1.Visible=false;
                    textBoxPatternNumberFromStrS2.Visible=false;
                    textBoxPatternNumberFromStrS3.Visible=false;
                    textBoxPatternNumberFromStrS4.Visible=false;
                    textBoxPatternNumberFromStrS5.Visible=false;
                    textBoxPatternNumberFromStrS6.Visible=false;
                    textBoxPatternNumberFromStrS7.Visible=false;
                    textBoxPatternNumberFromStrM1.Visible=false;
                    textBoxPatternNumberFromStrM2.Visible=false;
                    textBoxPatternNumberFromStrM3.Visible=false;
                    textBoxPatternNumberFromStrM4.Visible=false;
                    textBoxPatternNumberFromStrM5.Visible=false;
                    textBoxPatternNumberFromStrM6.Visible=false;
                    textBoxPatternNumberFromStrM7.Visible=false;

                    labelPatternNumberFromMun.Visible=false;
                    labelPatternNumberFromMunFall.Visible=false;
                    labelPatternNumberFromMunSingle.Visible=false;
                    labelPatternNumberFromMunMultiple.Visible=false;

                    labelPatternNumberFromZen.Visible=false;
                    labelPatternNumberFromZenFall.Visible=false;
                    labelPatternNumberFromZenSingle.Visible=false;
                    labelPatternNumberFromZenMultiple.Visible=false;

                    labelPatternNumberFromStr.Visible=false;
                    labelPatternNumberFromStrFall.Visible=false;
                    labelPatternNumberFromStrSingle.Visible=false;
                    labelPatternNumberFromStrMultiple.Visible=false;


                    tableLayoutPanelPatternNumberFromStr.Visible=false;
                    tableLayoutPanelPatternNumberFromZen.Visible=false;
                    tableLayoutPanelPatternNumberFromMun.Visible=false;
                }
            }

            labelPatternNumberFromName.Visible=true;
            tableLayoutPanelPatternNumberFromMuz.Visible=true;
            labelPatternNumberFromType.Visible=true;
            comboBoxPatternNumberFromType.Visible=true;
        }

        void PatternNumberFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternNumbersFromFiltered.Count; indexCur++) {
                if (itemsPatternNumberFrom[indexCur]==CurrentPatternNumberFrom) {
                    int indexList=PatternNumberFromlistBox.SelectedIndex;
                    if (indexList==indexCur) return;
                    PatternNumberFromlistBox.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternNumberFromSaveCurrent() {
            if (CurrentPatternNumberFrom==null) return;
            Edited=true;
            CurrentPatternNumberFrom.ShowType=(NumberType)comboBoxPatternNumberFromType.SelectedIndex;
            CurrentPatternNumberFrom.Name=textBoxPatternNumberFromName.Text;

            if (CurrentPatternNumberFrom.ShowType!=NumberType.Unknown) {
                if (CurrentPatternNumberFrom.ShowType==NumberType.NoDeklination || CurrentPatternNumberFrom.ShowType==NumberType.Deklination || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternNumberFrom.Shapes[0]=textBoxPatternNumberFromMuzS1.Text;
                }
                if (CurrentPatternNumberFrom.ShowType==NumberType.Deklination || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternNumberFrom.Shapes[1]=textBoxPatternNumberFromMuzS2.Text;
                    CurrentPatternNumberFrom.Shapes[2]=textBoxPatternNumberFromMuzS3.Text;
                    CurrentPatternNumberFrom.Shapes[3]=textBoxPatternNumberFromMuzS4.Text;
                    CurrentPatternNumberFrom.Shapes[4]=textBoxPatternNumberFromMuzS5.Text;
                    CurrentPatternNumberFrom.Shapes[5]=textBoxPatternNumberFromMuzS6.Text;
                    CurrentPatternNumberFrom.Shapes[6]=textBoxPatternNumberFromMuzS7.Text;
                }
                if (CurrentPatternNumberFrom.ShowType==NumberType.Deklination || CurrentPatternNumberFrom.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternNumberFrom.Shapes[7]=textBoxPatternNumberFromMuzM1.Text;
                    CurrentPatternNumberFrom.Shapes[8]=textBoxPatternNumberFromMuzM2.Text;
                    CurrentPatternNumberFrom.Shapes[9]=textBoxPatternNumberFromMuzM3.Text;
                    CurrentPatternNumberFrom.Shapes[10]=textBoxPatternNumberFromMuzM4.Text;
                    CurrentPatternNumberFrom.Shapes[11]=textBoxPatternNumberFromMuzM5.Text;
                    CurrentPatternNumberFrom.Shapes[12]=textBoxPatternNumberFromMuzM6.Text;
                    CurrentPatternNumberFrom.Shapes[13]=textBoxPatternNumberFromMuzM7.Text;
                }
                if (CurrentPatternNumberFrom.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternNumberFrom.Shapes[14+0] =textBoxPatternNumberFromMunS1.Text;
                    CurrentPatternNumberFrom.Shapes[14+1] =textBoxPatternNumberFromMunS2.Text;
                    CurrentPatternNumberFrom.Shapes[14+2] =textBoxPatternNumberFromMunS3.Text;
                    CurrentPatternNumberFrom.Shapes[14+3] =textBoxPatternNumberFromMunS4.Text;
                    CurrentPatternNumberFrom.Shapes[14+4] =textBoxPatternNumberFromMunS5.Text;
                    CurrentPatternNumberFrom.Shapes[14+5] =textBoxPatternNumberFromMunS6.Text;
                    CurrentPatternNumberFrom.Shapes[14+6] =textBoxPatternNumberFromMunS7.Text;
                    CurrentPatternNumberFrom.Shapes[14+7] =textBoxPatternNumberFromMunM1.Text;
                    CurrentPatternNumberFrom.Shapes[14+8] =textBoxPatternNumberFromMunM2.Text;
                    CurrentPatternNumberFrom.Shapes[14+9] =textBoxPatternNumberFromMunM3.Text;
                    CurrentPatternNumberFrom.Shapes[14+10]=textBoxPatternNumberFromMunM4.Text;
                    CurrentPatternNumberFrom.Shapes[14+11]=textBoxPatternNumberFromMunM5.Text;
                    CurrentPatternNumberFrom.Shapes[14+12]=textBoxPatternNumberFromMunM6.Text;
                    CurrentPatternNumberFrom.Shapes[14+13]=textBoxPatternNumberFromMunM7.Text;

                    CurrentPatternNumberFrom.Shapes[28+0] =textBoxPatternNumberFromZenS1.Text;
                    CurrentPatternNumberFrom.Shapes[28+1] =textBoxPatternNumberFromZenS2.Text;
                    CurrentPatternNumberFrom.Shapes[28+2] =textBoxPatternNumberFromZenS3.Text;
                    CurrentPatternNumberFrom.Shapes[28+3] =textBoxPatternNumberFromZenS4.Text;
                    CurrentPatternNumberFrom.Shapes[28+4] =textBoxPatternNumberFromZenS5.Text;
                    CurrentPatternNumberFrom.Shapes[28+5] =textBoxPatternNumberFromZenS6.Text;
                    CurrentPatternNumberFrom.Shapes[28+6] =textBoxPatternNumberFromZenS7.Text;
                    CurrentPatternNumberFrom.Shapes[28+7] =textBoxPatternNumberFromZenM1.Text;
                    CurrentPatternNumberFrom.Shapes[28+8] =textBoxPatternNumberFromZenM2.Text;
                    CurrentPatternNumberFrom.Shapes[28+9] =textBoxPatternNumberFromZenM3.Text;
                    CurrentPatternNumberFrom.Shapes[28+10]=textBoxPatternNumberFromZenM4.Text;
                    CurrentPatternNumberFrom.Shapes[28+11]=textBoxPatternNumberfromZenM5.Text;
                    CurrentPatternNumberFrom.Shapes[28+12]=textBoxPatternNumberFromZenM6.Text;
                    CurrentPatternNumberFrom.Shapes[28+13]=textBoxPatternNumberFromZenM7.Text;

                    CurrentPatternNumberFrom.Shapes[42+0] =textBoxPatternNumberFromStrS1.Text;
                    CurrentPatternNumberFrom.Shapes[42+1] =textBoxPatternNumberFromStrS2.Text;
                    CurrentPatternNumberFrom.Shapes[42+2] =textBoxPatternNumberFromStrS3.Text;
                    CurrentPatternNumberFrom.Shapes[42+3] =textBoxPatternNumberFromStrS4.Text;
                    CurrentPatternNumberFrom.Shapes[42+4] =textBoxPatternNumberFromStrS5.Text;
                    CurrentPatternNumberFrom.Shapes[42+5] =textBoxPatternNumberFromStrS6.Text;
                    CurrentPatternNumberFrom.Shapes[42+6] =textBoxPatternNumberFromStrS7.Text;
                    CurrentPatternNumberFrom.Shapes[42+7] =textBoxPatternNumberFromStrM1.Text;
                    CurrentPatternNumberFrom.Shapes[42+8] =textBoxPatternNumberFromStrM2.Text;
                    CurrentPatternNumberFrom.Shapes[42+9] =textBoxPatternNumberFromStrM3.Text;
                    CurrentPatternNumberFrom.Shapes[42+10]=textBoxPatternNumberFromStrM4.Text;
                    CurrentPatternNumberFrom.Shapes[42+11]=textBoxPatternNumberFromStrM5.Text;
                    CurrentPatternNumberFrom.Shapes[42+12]=textBoxPatternNumberFromStrM6.Text;
                    CurrentPatternNumberFrom.Shapes[42+13]=textBoxPatternNumberFromStrM7.Text;
                }
            }
        }

        void PatternNumberFromSetNone(){
            textBoxPatternNumberFromName.Text="";

            textBoxPatternNumberFromMuzS1.Text="";
            textBoxPatternNumberFromMuzS2.Text="";
            textBoxPatternNumberFromMuzS3.Text="";
            textBoxPatternNumberFromMuzS4.Text="";
            textBoxPatternNumberFromMuzS5.Text="";
            textBoxPatternNumberFromMuzS6.Text="";
            textBoxPatternNumberFromMuzS7.Text="";
            textBoxPatternNumberFromMuzM1.Text="";
            textBoxPatternNumberFromMuzM2.Text="";
            textBoxPatternNumberFromMuzM3.Text="";
            textBoxPatternNumberFromMuzM4.Text="";
            textBoxPatternNumberFromMuzM5.Text="";
            textBoxPatternNumberFromMuzM6.Text="";
            textBoxPatternNumberFromMuzM7.Text="";

            textBoxPatternNumberFromMunS1.Text="";
            textBoxPatternNumberFromMunS2.Text="";
            textBoxPatternNumberFromMunS3.Text="";
            textBoxPatternNumberFromMunS4.Text="";
            textBoxPatternNumberFromMunS5.Text="";
            textBoxPatternNumberFromMunS6.Text="";
            textBoxPatternNumberFromMunS7.Text="";
            textBoxPatternNumberFromMunM1.Text="";
            textBoxPatternNumberFromMunM2.Text="";
            textBoxPatternNumberFromMunM3.Text="";
            textBoxPatternNumberFromMunM4.Text="";
            textBoxPatternNumberFromMunM5.Text="";
            textBoxPatternNumberFromMunM6.Text="";
            textBoxPatternNumberFromMunM7.Text="";

            textBoxPatternNumberFromZenS1.Text="";
            textBoxPatternNumberFromZenS2.Text="";
            textBoxPatternNumberFromZenS3.Text="";
            textBoxPatternNumberFromZenS4.Text="";
            textBoxPatternNumberFromZenS5.Text="";
            textBoxPatternNumberFromZenS6.Text="";
            textBoxPatternNumberFromZenS7.Text="";
            textBoxPatternNumberFromZenM1.Text="";
            textBoxPatternNumberFromZenM2.Text="";
            textBoxPatternNumberFromZenM3.Text="";
            textBoxPatternNumberFromZenM4.Text="";
            textBoxPatternNumberfromZenM5.Text="";
            textBoxPatternNumberFromZenM6.Text="";
            textBoxPatternNumberFromZenM7.Text="";

            textBoxPatternNumberFromStrS1.Text="";
            textBoxPatternNumberFromStrS2.Text="";
            textBoxPatternNumberFromStrS3.Text="";
            textBoxPatternNumberFromStrS4.Text="";
            textBoxPatternNumberFromStrS5.Text="";
            textBoxPatternNumberFromStrS6.Text="";
            textBoxPatternNumberFromStrS7.Text="";
            textBoxPatternNumberFromStrM1.Text="";
            textBoxPatternNumberFromStrM2.Text="";
            textBoxPatternNumberFromStrM3.Text="";
            textBoxPatternNumberFromStrM4.Text="";
            textBoxPatternNumberFromStrM5.Text="";
            textBoxPatternNumberFromStrM6.Text="";
            textBoxPatternNumberFromStrM7.Text="";

            labelPatternNumberFromName.Visible=false;
            tableLayoutPanelPatternNumberFromMuz.Visible=false;
            labelPatternNumberFromMuzFall.Visible=false;
            labelPatternNumberFromMuzMultiple.Visible=false;
            labelPatternNumberFromMuzSingle.Visible=false;
            labelPatternNumberFromType.Visible=false;
            comboBoxPatternNumberFromType.Visible=false;

            textBoxPatternNumberFromMuzS1.Visible=false;
            textBoxPatternNumberFromMuzS2.Visible=false;
            textBoxPatternNumberFromMuzS3.Visible=false;
            textBoxPatternNumberFromMuzS4.Visible=false;
            textBoxPatternNumberFromMuzS5.Visible=false;
            textBoxPatternNumberFromMuzS6.Visible=false;
            textBoxPatternNumberFromMuzS7.Visible=false;
            textBoxPatternNumberFromMuzM1.Visible=false;
            textBoxPatternNumberFromMuzM2.Visible=false;
            textBoxPatternNumberFromMuzM3.Visible=false;
            textBoxPatternNumberFromMuzM4.Visible=false;
            textBoxPatternNumberFromMuzM5.Visible=false;
            textBoxPatternNumberFromMuzM6.Visible=false;
            textBoxPatternNumberFromMuzM7.Visible=false;


            textBoxPatternNumberFromMunS1.Visible=false;
            textBoxPatternNumberFromMunS2.Visible=false;
            textBoxPatternNumberFromMunS3.Visible=false;
            textBoxPatternNumberFromMunS4.Visible=false;
            textBoxPatternNumberFromMunS5.Visible=false;
            textBoxPatternNumberFromMunS6.Visible=false;
            textBoxPatternNumberFromMunS7.Visible=false;
            textBoxPatternNumberFromMunM1.Visible=false;
            textBoxPatternNumberFromMunM2.Visible=false;
            textBoxPatternNumberFromMunM3.Visible=false;
            textBoxPatternNumberFromMunM4.Visible=false;
            textBoxPatternNumberFromMunM5.Visible=false;
            textBoxPatternNumberFromMunM6.Visible=false;
            textBoxPatternNumberFromMunM7.Visible=false;


            textBoxPatternNumberFromZenS1.Visible=false;
            textBoxPatternNumberFromZenS2.Visible=false;
            textBoxPatternNumberFromZenS3.Visible=false;
            textBoxPatternNumberFromZenS4.Visible=false;
            textBoxPatternNumberFromZenS5.Visible=false;
            textBoxPatternNumberFromZenS6.Visible=false;
            textBoxPatternNumberFromZenS7.Visible=false;
            textBoxPatternNumberFromZenM1.Visible=false;
            textBoxPatternNumberFromZenM2.Visible=false;
            textBoxPatternNumberFromZenM3.Visible=false;
            textBoxPatternNumberFromZenM4.Visible=false;
            textBoxPatternNumberfromZenM5.Visible=false;
            textBoxPatternNumberFromZenM6.Visible=false;
            textBoxPatternNumberFromZenM7.Visible=false;


            textBoxPatternNumberFromStrS1.Visible=false;
            textBoxPatternNumberFromStrS2.Visible=false;
            textBoxPatternNumberFromStrS3.Visible=false;
            textBoxPatternNumberFromStrS4.Visible=false;
            textBoxPatternNumberFromStrS5.Visible=false;
            textBoxPatternNumberFromStrS6.Visible=false;
            textBoxPatternNumberFromStrS7.Visible=false;
            textBoxPatternNumberFromStrM1.Visible=false;
            textBoxPatternNumberFromStrM2.Visible=false;
            textBoxPatternNumberFromStrM3.Visible=false;
            textBoxPatternNumberFromStrM4.Visible=false;
            textBoxPatternNumberFromStrM5.Visible=false;
            textBoxPatternNumberFromStrM6.Visible=false;
            textBoxPatternNumberFromStrM7.Visible=false;

            textBoxPatternNumberFromName.Visible=false;
        }

        void PatternNumberFromClear() {
            PatternNumberFromlistBox.Items.Clear();
            PatternNumberFromSetNone();
            itemsPatternNumbersFromFiltered?.Clear();
            itemsPatternNumberFrom?.Clear();
            CurrentPatternNumberFrom=null;
        }
        #endregion

        #region NumberPattern To
        void PatternNumberToListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternNumberToSaveCurrent();

            int index=PatternNumberTolistBox.SelectedIndex;
            if (itemsPatternNumberTo.Count==0) {
                PatternNumberToSetNone();
                return;
            }
            if (index>=itemsPatternNumberTo.Count)
                index=itemsPatternNumberTo.Count-1;
            if (index<0)
                index=0;

            CurrentPatternNumberTo=itemsPatternNumberTo[index];
            PatternNumberToSetCurrent();
            PatternNumberToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternNumberToButtonAdd_Click(object sender, EventArgs e) {
            PatternNumberToAddNewItem();
        }

        void PatternNumberToButtonRemove_Click(object sender, EventArgs e) {
            PatternNumberToRemoveItem(CurrentPatternNumberTo);
            PatternNumberToTextBoxFilter_TextChanged(null, new EventArgs());
        }

        void PatternNumberToTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternNumberToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternNumber selectedId=null;
            if (PatternNumberTolistBox.SelectedIndex!=-1) {
                selectedId=itemsPatternNumbersToFiltered[PatternNumberTolistBox.SelectedIndex];
            }

            PatternNumberToRefreshFilteredList();

            PatternNumberTolistBox.Items.Clear();
            for (int i=0; i<itemsPatternNumbersToFiltered.Count; i++) {
                ItemPatternNumber item = itemsPatternNumbersToFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternNumberTolistBox.Items.Add(textToAdd);
            }

            //SetListBoxNumber();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternNumbersToFiltered.Count; i++){
                    if (itemsPatternNumbersToFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    PatternNumberTolistBox.SelectedIndex=-1;
                    CurrentPatternNumberTo=null;
                } else PatternNumberTolistBox.SelectedIndex=outIndex;
            } else {
                PatternNumberTolistBox.SelectedIndex=-1;
                CurrentPatternNumberTo=null;
            }
            PatternNumberToSetCurrent();
        }

        void PatternNumberToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternNumberTo.Remove(CurrentPatternNumberTo);
        }

        void PatternNumberToSetListBox() {
            //string filter=textBoxPatternNumberToFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=PatternNumberTolistBox.SelectedIndex;
            PatternNumberTolistBox.Items.Clear();
            for (int i=0; i<itemsPatternNumbersToFiltered.Count; i++) {
                ItemPatternNumber item = itemsPatternNumbersToFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternNumberTolistBox.Items.Add(textToAdd);
            }

            if (index>=PatternNumberTolistBox.Items.Count)index=PatternNumberTolistBox.Items.Count-1;
            PatternNumberTolistBox.SelectedIndex=index;
        }

        void PatternNumberToRefreshFilteredList() {
            if (itemsPatternNumbersToFiltered==null) itemsPatternNumbersToFiltered=new List<ItemPatternNumber>();
            itemsPatternNumbersToFiltered.Clear();
            string filter=textBoxPatternNumberToFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternNumberTo.Count; i++) {
                    ItemPatternNumber item = itemsPatternNumberTo[i];

                    if (item.Filter(filter)) {
                        itemsPatternNumbersToFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternNumberTo.Count; i++) {
                    ItemPatternNumber item = itemsPatternNumberTo[i];
                    itemsPatternNumbersToFiltered.Add(item);
                }
            }
        }

        void PatternNumberToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternNumberToSaveCurrent();

            var newItem=new ItemPatternNumber();
           // newItem.ID=itemsNumbers.Count;
            itemsPatternNumberTo.Add(newItem);
            CurrentPatternNumberTo=newItem;
            PatternNumberToRefreshFilteredList();
            PatternNumberToSetListBox();
            PatternNumberToListBoxSetCurrent();
            PatternNumberToSetCurrent();

            doingJob=false;
        }

        void PatternNumberToRemoveItem(ItemPatternNumber item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternNumberTo.Remove(item);
            PatternNumberToRefreshFilteredList();
            PatternNumberToSetListBox();
            PatternNumberToSetCurrent();
        }

        void PatternNumberToRefresh() {
            PatternNumberToRefreshFilteredList();
            PatternNumberToSetListBox();
            PatternNumberToSetCurrent();
        }

        void PatternNumberFromRefresh() {
            PatternNumberFromRefreshFilteredList();
            PatternNumberFromSetListBox();
            PatternNumberFromSetCurrent();
        }

        void PatternNumberToSetCurrent(){
            if (itemsPatternNumbersToFiltered.Count==0) {
                PatternNumberToSetNone();
                return;
            }

            int index=PatternNumberTolistBox.SelectedIndex;
            if (index>=itemsPatternNumbersToFiltered.Count) index=itemsPatternNumbersToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternNumberTo=itemsPatternNumbersToFiltered[index];
            textBoxPatternNumberToName.Text=CurrentPatternNumberTo.Name;
            comboBoxPatternNumberToType.SelectedIndex=(int)CurrentPatternNumberTo.ShowType;

            if (CurrentPatternNumberTo.ShowType!=NumberType.Unknown) {
                if (CurrentPatternNumberTo.ShowType==NumberType.NoDeklination || CurrentPatternNumberTo.ShowType==NumberType.Deklination || CurrentPatternNumberTo.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternNumberTo.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberToMuzJ1.Text=CurrentPatternNumberTo.Shapes[0];
                    textBoxPatternNumberToMuzJ1.Visible=true;

                    textBoxPatternNumberToName.Visible=true;
                    labelPatternNumberToMuzFall.Visible=true;
                    labelPatternNumberToMuzMultiple.Visible=true;
                    labelPatternNumberToMuzSingle.Visible=true;
                } else {
                    textBoxPatternNumberToMuzJ1.Visible=false;
                    textBoxPatternNumberToName.Visible=false;
                    labelPatternNumberToMuzFall.Visible=false;
                    labelPatternNumberToMuzMultiple.Visible=false;
                    labelPatternNumberToMuzSingle.Visible=false;
                }
                if (CurrentPatternNumberTo.ShowType==NumberType.Deklination || CurrentPatternNumberTo.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternNumberTo.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberToMuzJ2.Text=CurrentPatternNumberTo.Shapes[1];
                    textBoxPatternNumberToMuzJ3.Text=CurrentPatternNumberTo.Shapes[2];
                    textBoxPatternNumberToMuzJ4.Text=CurrentPatternNumberTo.Shapes[3];
                    textBoxPatternNumberToMuzJ5.Text=CurrentPatternNumberTo.Shapes[4];
                    textBoxPatternNumberToMuzJ6.Text=CurrentPatternNumberTo.Shapes[5];
                    textBoxPatternNumberToMuzJ7.Text=CurrentPatternNumberTo.Shapes[6];
                    textBoxPatternNumberToMuzJ2.Visible=true;
                    textBoxPatternNumberToMuzJ3.Visible=true;
                    textBoxPatternNumberToMuzJ4.Visible=true;
                    textBoxPatternNumberToMuzJ5.Visible=true;
                    textBoxPatternNumberToMuzJ6.Visible=true;
                    textBoxPatternNumberToMuzJ7.Visible=true;
                } else {
                    textBoxPatternNumberToMuzJ2.Visible=false;
                    textBoxPatternNumberToMuzJ3.Visible=false;
                    textBoxPatternNumberToMuzJ4.Visible=false;
                    textBoxPatternNumberToMuzJ5.Visible=false;
                    textBoxPatternNumberToMuzJ6.Visible=false;
                    textBoxPatternNumberToMuzJ7.Visible=false;
                }
                if (CurrentPatternNumberTo.ShowType==NumberType.Deklination || CurrentPatternNumberTo.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberToMuzM1.Text=CurrentPatternNumberTo.Shapes[7];
                    textBoxPatternNumberToMuzM2.Text=CurrentPatternNumberTo.Shapes[8];
                    textBoxPatternNumberToMuzM3.Text=CurrentPatternNumberTo.Shapes[9];
                    textBoxPatternNumberToMuzM4.Text=CurrentPatternNumberTo.Shapes[10];
                    textBoxPatternNumberToMuzM5.Text=CurrentPatternNumberTo.Shapes[11];
                    textBoxPatternNumberToMuzM6.Text=CurrentPatternNumberTo.Shapes[12];
                    textBoxPatternNumberToMuzM7.Text=CurrentPatternNumberTo.Shapes[13];
                    textBoxPatternNumberToMuzM1.Visible=true;
                    textBoxPatternNumberToMuzM2.Visible=true;
                    textBoxPatternNumberToMuzM3.Visible=true;
                    textBoxPatternNumberToMuzM4.Visible=true;
                    textBoxPatternNumberToMuzM5.Visible=true;
                    textBoxPatternNumberToMuzM6.Visible=true;
                    textBoxPatternNumberToMuzM7.Visible=true;
                    labelPatternNumberToMuz.Visible=false;
                }else{
                    textBoxPatternNumberToMuzM1.Visible=false;
                    textBoxPatternNumberToMuzM2.Visible=false;
                    textBoxPatternNumberToMuzM3.Visible=false;
                    textBoxPatternNumberToMuzM4.Visible=false;
                    textBoxPatternNumberToMuzM5.Visible=false;
                    textBoxPatternNumberToMuzM6.Visible=false;
                    textBoxPatternNumberToMuzM7.Visible=false;
                    labelPatternNumberToMuz.Visible=true;
                }
                if (CurrentPatternNumberTo.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberToMunJ1.Text=CurrentPatternNumberTo.Shapes[14+0];
                    textBoxPatternNumberToMunJ2.Text=CurrentPatternNumberTo.Shapes[14+1];
                    textBoxPatternNumberToMunJ3.Text=CurrentPatternNumberTo.Shapes[14+2];
                    textBoxPatternNumberToMunJ4.Text=CurrentPatternNumberTo.Shapes[14+3];
                    textBoxPatternNumberToMunJ5.Text=CurrentPatternNumberTo.Shapes[14+4];
                    textBoxPatternNumberToMunJ6.Text=CurrentPatternNumberTo.Shapes[14+5];
                    textBoxPatternNumberToMunJ7.Text=CurrentPatternNumberTo.Shapes[14+6];
                    textBoxPatternNumberToMunM1.Text=CurrentPatternNumberTo.Shapes[14+7];
                    textBoxPatternNumberToMunM2.Text=CurrentPatternNumberTo.Shapes[14+8];
                    textBoxPatternNumberToMunM3.Text=CurrentPatternNumberTo.Shapes[14+9];
                    textBoxPatternNumberToMunM4.Text=CurrentPatternNumberTo.Shapes[14+10];
                    textBoxPatternNumberToMunM5.Text=CurrentPatternNumberTo.Shapes[14+11];
                    textBoxPatternNumberToMunM6.Text=CurrentPatternNumberTo.Shapes[14+12];
                    textBoxPatternNumberToMunM7.Text=CurrentPatternNumberTo.Shapes[14+13];

                    textBoxPatternNumberToZenJ1.Text=CurrentPatternNumberTo.Shapes[28+0];
                    textBoxPatternNumberToZenJ2.Text=CurrentPatternNumberTo.Shapes[28+1];
                    textBoxPatternNumberToZenJ3.Text=CurrentPatternNumberTo.Shapes[28+2];
                    textBoxPatternNumberToZenJ4.Text=CurrentPatternNumberTo.Shapes[28+3];
                    textBoxPatternNumberToZenJ5.Text=CurrentPatternNumberTo.Shapes[28+4];
                    textBoxPatternNumberToZenJ6.Text=CurrentPatternNumberTo.Shapes[28+5];
                    textBoxPatternNumberToZenJ7.Text=CurrentPatternNumberTo.Shapes[28+6];
                    textBoxPatternNumberToZenM1.Text=CurrentPatternNumberTo.Shapes[28+7];
                    textBoxPatternNumberToZenM2.Text=CurrentPatternNumberTo.Shapes[28+8];
                    textBoxPatternNumberToZenM3.Text=CurrentPatternNumberTo.Shapes[28+9];
                    textBoxPatternNumberToZenM4.Text=CurrentPatternNumberTo.Shapes[28+10];
                    textBoxPatternNumberToZenM5.Text=CurrentPatternNumberTo.Shapes[28+11];
                    textBoxPatternNumberToZenM6.Text=CurrentPatternNumberTo.Shapes[28+12];
                    textBoxPatternNumberToZenM7.Text=CurrentPatternNumberTo.Shapes[28+13];

                    textBoxPatternNumberToStrJ1.Text=CurrentPatternNumberTo.Shapes[42+0];
                    textBoxPatternNumberToStrJ2.Text=CurrentPatternNumberTo.Shapes[42+1];
                    textBoxPatternNumberToStrJ3.Text=CurrentPatternNumberTo.Shapes[42+2];
                    textBoxPatternNumberToStrJ4.Text=CurrentPatternNumberTo.Shapes[42+3];
                    textBoxPatternNumberToStrJ5.Text=CurrentPatternNumberTo.Shapes[42+4];
                    textBoxPatternNumberToStrJ6.Text=CurrentPatternNumberTo.Shapes[42+5];
                    textBoxPatternNumberToStrJ7.Text=CurrentPatternNumberTo.Shapes[42+6];
                    textBoxPatternNumberToStrM1.Text=CurrentPatternNumberTo.Shapes[42+7];
                    textBoxPatternNumberToStrM2.Text=CurrentPatternNumberTo.Shapes[42+8];
                    textBoxPatternNumberToStrM3.Text=CurrentPatternNumberTo.Shapes[42+9];
                    textBoxPatternNumberToStrM4.Text=CurrentPatternNumberTo.Shapes[42+10];
                    textBoxPatternNumberToStrM5.Text=CurrentPatternNumberTo.Shapes[42+11];
                    textBoxPatternNumberToStrM6.Text=CurrentPatternNumberTo.Shapes[42+12];
                    textBoxPatternNumberToStrM7.Text=CurrentPatternNumberTo.Shapes[42+13];

                    textBoxPatternNumberToMunJ1.Visible=true;
                    textBoxPatternNumberToMunJ2.Visible=true;
                    textBoxPatternNumberToMunJ3.Visible=true;
                    textBoxPatternNumberToMunJ4.Visible=true;
                    textBoxPatternNumberToMunJ5.Visible=true;
                    textBoxPatternNumberToMunJ6.Visible=true;
                    textBoxPatternNumberToMunJ7.Visible=true;
                    textBoxPatternNumberToMunM1.Visible=true;
                    textBoxPatternNumberToMunM2.Visible=true;
                    textBoxPatternNumberToMunM3.Visible=true;
                    textBoxPatternNumberToMunM4.Visible=true;
                    textBoxPatternNumberToMunM5.Visible=true;
                    textBoxPatternNumberToMunM6.Visible=true;
                    textBoxPatternNumberToMunM7.Visible=true;

                    textBoxPatternNumberToZenJ1.Visible=true;
                    textBoxPatternNumberToZenJ2.Visible=true;
                    textBoxPatternNumberToZenJ3.Visible=true;
                    textBoxPatternNumberToZenJ4.Visible=true;
                    textBoxPatternNumberToZenJ5.Visible=true;
                    textBoxPatternNumberToZenJ6.Visible=true;
                    textBoxPatternNumberToZenJ7.Visible=true;
                    textBoxPatternNumberToZenM1.Visible=true;
                    textBoxPatternNumberToZenM2.Visible=true;
                    textBoxPatternNumberToZenM3.Visible=true;
                    textBoxPatternNumberToZenM4.Visible=true;
                    textBoxPatternNumberToZenM5.Visible=true;
                    textBoxPatternNumberToZenM6.Visible=true;
                    textBoxPatternNumberToZenM7.Visible=true;

                    textBoxPatternNumberToStrJ1.Visible=true;
                    textBoxPatternNumberToStrJ2.Visible=true;
                    textBoxPatternNumberToStrJ3.Visible=true;
                    textBoxPatternNumberToStrJ4.Visible=true;
                    textBoxPatternNumberToStrJ5.Visible=true;
                    textBoxPatternNumberToStrJ6.Visible=true;
                    textBoxPatternNumberToStrJ7.Visible=true;
                    textBoxPatternNumberToStrM1.Visible=true;
                    textBoxPatternNumberToStrM2.Visible=true;
                    textBoxPatternNumberToStrM3.Visible=true;
                    textBoxPatternNumberToStrM4.Visible=true;
                    textBoxPatternNumberToStrM5.Visible=true;
                    textBoxPatternNumberToStrM6.Visible=true;
                    textBoxPatternNumberToStrM7.Visible=true;

                    labelPatternNumberToMun.Visible=true;
                    labelPatternNumberToMunFall.Visible=true;
                    labelPatternNumberToMunSingle.Visible=true;
                    labelPatternNumberToMunMultiple.Visible=true;

                    labelPatternNumberToZen.Visible=true;
                    labelPatternNumberToZenFall.Visible=true;
                    labelPatternNumberToZenSingle.Visible=true;
                    labelPatternNumberToZenMultiple.Visible=true;

                    labelPatternNumberToStr.Visible=true;
                    labelPatternNumberToStrFall.Visible=true;
                    labelPatternNumberToStrSingle.Visible=true;
                    labelPatternNumberToStrMultiple.Visible=true;

                    tableLayoutPanelPatternNumberToStr.Visible=true;
                    tableLayoutPanelPatternNumberToZen.Visible=true;
                    tableLayoutPanelPatternNumberToMun.Visible=true;
                }else{
                    textBoxPatternNumberToMunJ1.Visible=false;
                    textBoxPatternNumberToMunJ2.Visible=false;
                    textBoxPatternNumberToMunJ3.Visible=false;
                    textBoxPatternNumberToMunJ4.Visible=false;
                    textBoxPatternNumberToMunJ5.Visible=false;
                    textBoxPatternNumberToMunJ6.Visible=false;
                    textBoxPatternNumberToMunJ7.Visible=false;
                    textBoxPatternNumberToMunM1.Visible=false;
                    textBoxPatternNumberToMunM2.Visible=false;
                    textBoxPatternNumberToMunM3.Visible=false;
                    textBoxPatternNumberToMunM4.Visible=false;
                    textBoxPatternNumberToMunM5.Visible=false;
                    textBoxPatternNumberToMunM6.Visible=false;
                    textBoxPatternNumberToMunM7.Visible=false;

                    textBoxPatternNumberToZenJ1.Visible=false;
                    textBoxPatternNumberToZenJ2.Visible=false;
                    textBoxPatternNumberToZenJ3.Visible=false;
                    textBoxPatternNumberToZenJ4.Visible=false;
                    textBoxPatternNumberToZenJ5.Visible=false;
                    textBoxPatternNumberToZenJ6.Visible=false;
                    textBoxPatternNumberToZenJ7.Visible=false;
                    textBoxPatternNumberToZenM1.Visible=false;
                    textBoxPatternNumberToZenM2.Visible=false;
                    textBoxPatternNumberToZenM3.Visible=false;
                    textBoxPatternNumberToZenM4.Visible=false;
                    textBoxPatternNumberToZenM5.Visible=false;
                    textBoxPatternNumberToZenM6.Visible=false;
                    textBoxPatternNumberToZenM7.Visible=false;

                    textBoxPatternNumberToStrJ1.Visible=false;
                    textBoxPatternNumberToStrJ2.Visible=false;
                    textBoxPatternNumberToStrJ3.Visible=false;
                    textBoxPatternNumberToStrJ4.Visible=false;
                    textBoxPatternNumberToStrJ5.Visible=false;
                    textBoxPatternNumberToStrJ6.Visible=false;
                    textBoxPatternNumberToStrJ7.Visible=false;
                    textBoxPatternNumberToStrM1.Visible=false;
                    textBoxPatternNumberToStrM2.Visible=false;
                    textBoxPatternNumberToStrM3.Visible=false;
                    textBoxPatternNumberToStrM4.Visible=false;
                    textBoxPatternNumberToStrM5.Visible=false;
                    textBoxPatternNumberToStrM6.Visible=false;
                    textBoxPatternNumberToStrM7.Visible=false;

                    labelPatternNumberToMun.Visible=false;
                    labelPatternNumberToMunFall.Visible=false;
                    labelPatternNumberToMunSingle.Visible=false;
                    labelPatternNumberToMunMultiple.Visible=false;

                    labelPatternNumberToZen.Visible=false;
                    labelPatternNumberToZenFall.Visible=false;
                    labelPatternNumberToZenSingle.Visible=false;
                    labelPatternNumberToZenMultiple.Visible=false;

                    labelPatternNumberToStr.Visible=false;
                    labelPatternNumberToStrFall.Visible=false;
                    labelPatternNumberToStrSingle.Visible=false;
                    labelPatternNumberToStrMultiple.Visible=false;


                    tableLayoutPanelPatternNumberToStr.Visible=false;
                    tableLayoutPanelPatternNumberToZen.Visible=false;
                    tableLayoutPanelPatternNumberToMun.Visible=false;
                }
            }

            labelPatternNumberToName.Visible=true;
            tableLayoutPanelPatternNumberToMuz.Visible=true;
            labelPatternNumberToType.Visible=true;
            comboBoxPatternNumberToType.Visible=true;
        }

        void PatternNumberToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternNumbersToFiltered.Count; indexCur++) {
                if (itemsPatternNumberTo[indexCur]==CurrentPatternNumberTo) {
                    int indexList=PatternNumberTolistBox.SelectedIndex;
                    if (indexList==indexCur) return;
                    PatternNumberTolistBox.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternNumberToSaveCurrent() {
            if (CurrentPatternNumberTo==null) return;
            Edited=true;
            CurrentPatternNumberTo.ShowType=(NumberType)comboBoxPatternNumberToType.SelectedIndex;
            CurrentPatternNumberTo.Name=textBoxPatternNumberToName.Text;

            if (CurrentPatternNumberTo.ShowType!=NumberType.Unknown) {
                if (CurrentPatternNumberTo.ShowType==NumberType.NoDeklination || CurrentPatternNumberTo.ShowType==NumberType.Deklination || CurrentPatternNumberTo.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternNumberTo.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternNumberTo.Shapes[0]=textBoxPatternNumberToMuzJ1.Text;
                }
                if (CurrentPatternNumberTo.ShowType==NumberType.Deklination || CurrentPatternNumberTo.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternNumberTo.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternNumberTo.Shapes[1]=textBoxPatternNumberToMuzJ2.Text;
                    CurrentPatternNumberTo.Shapes[2]=textBoxPatternNumberToMuzJ3.Text;
                    CurrentPatternNumberTo.Shapes[3]=textBoxPatternNumberToMuzJ4.Text;
                    CurrentPatternNumberTo.Shapes[4]=textBoxPatternNumberToMuzJ5.Text;
                    CurrentPatternNumberTo.Shapes[5]=textBoxPatternNumberToMuzJ6.Text;
                    CurrentPatternNumberTo.Shapes[6]=textBoxPatternNumberToMuzJ7.Text;
                }
                if (CurrentPatternNumberTo.ShowType==NumberType.Deklination || CurrentPatternNumberTo.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternNumberTo.Shapes[7]=textBoxPatternNumberToMuzM1.Text;
                    CurrentPatternNumberTo.Shapes[8]=textBoxPatternNumberToMuzM2.Text;
                    CurrentPatternNumberTo.Shapes[9]=textBoxPatternNumberToMuzM3.Text;
                    CurrentPatternNumberTo.Shapes[10]=textBoxPatternNumberToMuzM4.Text;
                    CurrentPatternNumberTo.Shapes[11]=textBoxPatternNumberToMuzM5.Text;
                    CurrentPatternNumberTo.Shapes[12]=textBoxPatternNumberToMuzM6.Text;
                    CurrentPatternNumberTo.Shapes[13]=textBoxPatternNumberToMuzM7.Text;
                }
                if (CurrentPatternNumberTo.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternNumberTo.Shapes[14+0] =textBoxPatternNumberToMunJ1.Text;
                    CurrentPatternNumberTo.Shapes[14+1] =textBoxPatternNumberToMunJ2.Text;
                    CurrentPatternNumberTo.Shapes[14+2] =textBoxPatternNumberToMunJ3.Text;
                    CurrentPatternNumberTo.Shapes[14+3] =textBoxPatternNumberToMunJ4.Text;
                    CurrentPatternNumberTo.Shapes[14+4] =textBoxPatternNumberToMunJ5.Text;
                    CurrentPatternNumberTo.Shapes[14+5] =textBoxPatternNumberToMunJ6.Text;
                    CurrentPatternNumberTo.Shapes[14+6] =textBoxPatternNumberToMunJ7.Text;
                    CurrentPatternNumberTo.Shapes[14+7] =textBoxPatternNumberToMunM1.Text;
                    CurrentPatternNumberTo.Shapes[14+8] =textBoxPatternNumberToMunM2.Text;
                    CurrentPatternNumberTo.Shapes[14+9] =textBoxPatternNumberToMunM3.Text;
                    CurrentPatternNumberTo.Shapes[14+10]=textBoxPatternNumberToMunM4.Text;
                    CurrentPatternNumberTo.Shapes[14+11]=textBoxPatternNumberToMunM5.Text;
                    CurrentPatternNumberTo.Shapes[14+12]=textBoxPatternNumberToMunM6.Text;
                    CurrentPatternNumberTo.Shapes[14+13]=textBoxPatternNumberToMunM7.Text;

                    CurrentPatternNumberTo.Shapes[28+0] =textBoxPatternNumberToZenJ1.Text;
                    CurrentPatternNumberTo.Shapes[28+1] =textBoxPatternNumberToZenJ2.Text;
                    CurrentPatternNumberTo.Shapes[28+2] =textBoxPatternNumberToZenJ3.Text;
                    CurrentPatternNumberTo.Shapes[28+3] =textBoxPatternNumberToZenJ4.Text;
                    CurrentPatternNumberTo.Shapes[28+4] =textBoxPatternNumberToZenJ5.Text;
                    CurrentPatternNumberTo.Shapes[28+5] =textBoxPatternNumberToZenJ6.Text;
                    CurrentPatternNumberTo.Shapes[28+6] =textBoxPatternNumberToZenJ7.Text;
                    CurrentPatternNumberTo.Shapes[28+7] =textBoxPatternNumberToZenM1.Text;
                    CurrentPatternNumberTo.Shapes[28+8] =textBoxPatternNumberToZenM2.Text;
                    CurrentPatternNumberTo.Shapes[28+9] =textBoxPatternNumberToZenM3.Text;
                    CurrentPatternNumberTo.Shapes[28+10]=textBoxPatternNumberToZenM4.Text;
                    CurrentPatternNumberTo.Shapes[28+11]=textBoxPatternNumberToZenM5.Text;
                    CurrentPatternNumberTo.Shapes[28+12]=textBoxPatternNumberToZenM6.Text;
                    CurrentPatternNumberTo.Shapes[28+13]=textBoxPatternNumberToZenM7.Text;

                    CurrentPatternNumberTo.Shapes[42+0] =textBoxPatternNumberToStrJ1.Text;
                    CurrentPatternNumberTo.Shapes[42+1] =textBoxPatternNumberToStrJ2.Text;
                    CurrentPatternNumberTo.Shapes[42+2] =textBoxPatternNumberToStrJ3.Text;
                    CurrentPatternNumberTo.Shapes[42+3] =textBoxPatternNumberToStrJ4.Text;
                    CurrentPatternNumberTo.Shapes[42+4] =textBoxPatternNumberToStrJ5.Text;
                    CurrentPatternNumberTo.Shapes[42+5] =textBoxPatternNumberToStrJ6.Text;
                    CurrentPatternNumberTo.Shapes[42+6] =textBoxPatternNumberToStrJ7.Text;
                    CurrentPatternNumberTo.Shapes[42+7] =textBoxPatternNumberToStrM1.Text;
                    CurrentPatternNumberTo.Shapes[42+8] =textBoxPatternNumberToStrM2.Text;
                    CurrentPatternNumberTo.Shapes[42+9] =textBoxPatternNumberToStrM3.Text;
                    CurrentPatternNumberTo.Shapes[42+10]=textBoxPatternNumberToStrM4.Text;
                    CurrentPatternNumberTo.Shapes[42+11]=textBoxPatternNumberToStrM5.Text;
                    CurrentPatternNumberTo.Shapes[42+12]=textBoxPatternNumberToStrM6.Text;
                    CurrentPatternNumberTo.Shapes[42+13]=textBoxPatternNumberToStrM7.Text;
                }
            }
        }

        void PatternNumberToSetNone(){
            textBoxPatternNumberToName.Text="";

            textBoxPatternNumberToMuzJ1.Text="";
            textBoxPatternNumberToMuzJ2.Text="";
            textBoxPatternNumberToMuzJ3.Text="";
            textBoxPatternNumberToMuzJ4.Text="";
            textBoxPatternNumberToMuzJ5.Text="";
            textBoxPatternNumberToMuzJ6.Text="";
            textBoxPatternNumberToMuzJ7.Text="";
            textBoxPatternNumberToMuzM1.Text="";
            textBoxPatternNumberToMuzM2.Text="";
            textBoxPatternNumberToMuzM3.Text="";
            textBoxPatternNumberToMuzM4.Text="";
            textBoxPatternNumberToMuzM5.Text="";
            textBoxPatternNumberToMuzM6.Text="";
            textBoxPatternNumberToMuzM7.Text="";

            textBoxPatternNumberToMunJ1.Text="";
            textBoxPatternNumberToMunJ2.Text="";
            textBoxPatternNumberToMunJ3.Text="";
            textBoxPatternNumberToMunJ4.Text="";
            textBoxPatternNumberToMunJ5.Text="";
            textBoxPatternNumberToMunJ6.Text="";
            textBoxPatternNumberToMunJ7.Text="";
            textBoxPatternNumberToMunM1.Text="";
            textBoxPatternNumberToMunM2.Text="";
            textBoxPatternNumberToMunM3.Text="";
            textBoxPatternNumberToMunM4.Text="";
            textBoxPatternNumberToMunM5.Text="";
            textBoxPatternNumberToMunM6.Text="";
            textBoxPatternNumberToMunM7.Text="";

            textBoxPatternNumberToZenJ1.Text="";
            textBoxPatternNumberToZenJ2.Text="";
            textBoxPatternNumberToZenJ3.Text="";
            textBoxPatternNumberToZenJ4.Text="";
            textBoxPatternNumberToZenJ5.Text="";
            textBoxPatternNumberToZenJ6.Text="";
            textBoxPatternNumberToZenJ7.Text="";
            textBoxPatternNumberToZenM1.Text="";
            textBoxPatternNumberToZenM2.Text="";
            textBoxPatternNumberToZenM3.Text="";
            textBoxPatternNumberToZenM4.Text="";
            textBoxPatternNumberToZenM5.Text="";
            textBoxPatternNumberToZenM6.Text="";
            textBoxPatternNumberToZenM7.Text="";

            textBoxPatternNumberToStrJ1.Text="";
            textBoxPatternNumberToStrJ2.Text="";
            textBoxPatternNumberToStrJ3.Text="";
            textBoxPatternNumberToStrJ4.Text="";
            textBoxPatternNumberToStrJ5.Text="";
            textBoxPatternNumberToStrJ6.Text="";
            textBoxPatternNumberToStrJ7.Text="";
            textBoxPatternNumberToStrM1.Text="";
            textBoxPatternNumberToStrM2.Text="";
            textBoxPatternNumberToStrM3.Text="";
            textBoxPatternNumberToStrM4.Text="";
            textBoxPatternNumberToStrM5.Text="";
            textBoxPatternNumberToStrM6.Text="";
            textBoxPatternNumberToStrM7.Text="";

            labelPatternNumberToName.Visible=false;
            tableLayoutPanelPatternNumberToMuz.Visible=false;
            labelPatternNumberToMuzFall.Visible=false;
            labelPatternNumberToMuzMultiple.Visible=false;
            labelPatternNumberToMuzSingle.Visible=false;
            labelPatternNumberToType.Visible=false;
            comboBoxPatternNumberToType.Visible=false;

            textBoxPatternNumberToMuzJ1.Visible=false;
            textBoxPatternNumberToMuzJ2.Visible=false;
            textBoxPatternNumberToMuzJ3.Visible=false;
            textBoxPatternNumberToMuzJ4.Visible=false;
            textBoxPatternNumberToMuzJ5.Visible=false;
            textBoxPatternNumberToMuzJ6.Visible=false;
            textBoxPatternNumberToMuzJ7.Visible=false;
            textBoxPatternNumberToMuzM1.Visible=false;
            textBoxPatternNumberToMuzM2.Visible=false;
            textBoxPatternNumberToMuzM3.Visible=false;
            textBoxPatternNumberToMuzM4.Visible=false;
            textBoxPatternNumberToMuzM5.Visible=false;
            textBoxPatternNumberToMuzM6.Visible=false;
            textBoxPatternNumberToMuzM7.Visible=false;


            textBoxPatternNumberToMunJ1.Visible=false;
            textBoxPatternNumberToMunJ2.Visible=false;
            textBoxPatternNumberToMunJ3.Visible=false;
            textBoxPatternNumberToMunJ4.Visible=false;
            textBoxPatternNumberToMunJ5.Visible=false;
            textBoxPatternNumberToMunJ6.Visible=false;
            textBoxPatternNumberToMunJ7.Visible=false;
            textBoxPatternNumberToMunM1.Visible=false;
            textBoxPatternNumberToMunM2.Visible=false;
            textBoxPatternNumberToMunM3.Visible=false;
            textBoxPatternNumberToMunM4.Visible=false;
            textBoxPatternNumberToMunM5.Visible=false;
            textBoxPatternNumberToMunM6.Visible=false;
            textBoxPatternNumberToMunM7.Visible=false;


            textBoxPatternNumberToZenJ1.Visible=false;
            textBoxPatternNumberToZenJ2.Visible=false;
            textBoxPatternNumberToZenJ3.Visible=false;
            textBoxPatternNumberToZenJ4.Visible=false;
            textBoxPatternNumberToZenJ5.Visible=false;
            textBoxPatternNumberToZenJ6.Visible=false;
            textBoxPatternNumberToZenJ7.Visible=false;
            textBoxPatternNumberToZenM1.Visible=false;
            textBoxPatternNumberToZenM2.Visible=false;
            textBoxPatternNumberToZenM3.Visible=false;
            textBoxPatternNumberToZenM4.Visible=false;
            textBoxPatternNumberToZenM5.Visible=false;
            textBoxPatternNumberToZenM6.Visible=false;
            textBoxPatternNumberToZenM7.Visible=false;


            textBoxPatternNumberToStrJ1.Visible=false;
            textBoxPatternNumberToStrJ2.Visible=false;
            textBoxPatternNumberToStrJ3.Visible=false;
            textBoxPatternNumberToStrJ4.Visible=false;
            textBoxPatternNumberToStrJ5.Visible=false;
            textBoxPatternNumberToStrJ6.Visible=false;
            textBoxPatternNumberToStrJ7.Visible=false;
            textBoxPatternNumberToStrM1.Visible=false;
            textBoxPatternNumberToStrM2.Visible=false;
            textBoxPatternNumberToStrM3.Visible=false;
            textBoxPatternNumberToStrM4.Visible=false;
            textBoxPatternNumberToStrM5.Visible=false;
            textBoxPatternNumberToStrM6.Visible=false;
            textBoxPatternNumberToStrM7.Visible=false;

            textBoxPatternNumberToName.Visible=false;
        }

        void PatternNumberToClear() {
            PatternNumberTolistBox.Items.Clear();
            PatternNumberToSetNone();
            itemsPatternNumbersToFiltered?.Clear();
            itemsPatternNumberTo?.Clear();
            CurrentPatternNumberTo=null;
        }
        #endregion

        #region Number
        void ListBoxNumber_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentNumber();

            int index=listBoxNumbers.SelectedIndex;
            if (itemsNumbers.Count==0) {
                NumberSetNone();
                return;
            }
            if (index>=itemsNumbers.Count)
                index=itemsNumbers.Count-1;
            if (index<0)
                index=0;

            CurrentNumber=itemsNumbers[index];
            SetCurrentNumber();
            SetListBoxNumber();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonNumberAdd_Click(object sender, EventArgs e) {
            AddNewItemNumber();
        }

        void ButtonNumberRemove_Click(object sender, EventArgs e) {
            RemoveItemNumber(CurrentNumber);
            TextBoxNumberFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxNumberFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentNumber();

            // Získej aktuální prvek
            ItemNumber selectedId=null;
            if (listBoxNumbers.SelectedIndex!=-1) {
                selectedId=itemsNumbersFiltered[listBoxNumbers.SelectedIndex];
            }

            NumberRefreshFilteredList();

            listBoxNumbers.Items.Clear();
            for (int i=0; i<itemsNumbersFiltered.Count; i++) {
                ItemNumber item = itemsNumbersFiltered[i];

                string textToAdd=item.GetText(itemsPatternNumberFrom.Cast<ItemTranslatingPattern>().ToList(), itemsPatternNumberTo.Cast<ItemTranslatingPattern>().ToList());
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxNumbers.Items.Add(textToAdd);
            }

            //SetListBoxNumber();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsNumbersFiltered.Count; i++){
                    if (itemsNumbersFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxNumbers.SelectedIndex=-1;
                    CurrentNumber=null;
                } else listBoxNumbers.SelectedIndex=outIndex;
            } else {
                listBoxNumbers.SelectedIndex=-1;
                CurrentNumber=null;
            }
            SetCurrentNumber();
        }

        void RemoveCurrentNumber(object sender, EventArgs e) {
            itemsNumbers.Remove(CurrentNumber);
        }

        void SetListBoxNumber() {
            //string filter=textBoxNumberFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxNumbers.SelectedIndex;
            listBoxNumbers.Items.Clear();
            for (int i=0; i<itemsNumbersFiltered.Count; i++) {
                ItemNumber item = itemsNumbersFiltered[i];

                string textToAdd=item.GetText(itemsPatternNumberFrom.Cast<ItemTranslatingPattern>().ToList(), itemsPatternNumberTo.Cast<ItemTranslatingPattern>().ToList());
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxNumbers.Items.Add(textToAdd);
            }

            if (index>=listBoxNumbers.Items.Count)index=listBoxNumbers.Items.Count-1;
            listBoxNumbers.SelectedIndex=index;
        }

        void NumberRefreshFilteredList() {
            if (itemsNumbersFiltered==null) itemsNumbersFiltered=new List<ItemNumber>();
            itemsNumbersFiltered.Clear();
            string filter=textBoxNumberFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsNumbers.Count; i++) {
                    ItemNumber item = itemsNumbers[i];

                    if (item.Filter(filter)) {
                        itemsNumbersFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsNumbers.Count; i++) {
                    ItemNumber item = itemsNumbers[i];
                    itemsNumbersFiltered.Add(item);
                }
            }
        }

        void NumberRefresh(){
            NumberRefreshFilteredList();
            SetListBoxNumber();
            SetCurrentNumber();
        }

        void AddNewItemNumber() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentNumber();

            var newItem=new ItemNumber();
            newItem.To=new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern()};
           // newItem.ID=itemsNumbers.Count;
            itemsNumbers.Add(newItem);
            CurrentNumber=newItem;
            NumberRefreshFilteredList();
            SetListBoxNumber();
            ListBoxSetCurrentNumber();
            SetCurrentNumber();

            doingJob=false;
        }

        void RemoveItemNumber(ItemNumber item) {
            Edited=true;
            ChangeCaptionText();
            itemsNumbers.Remove(item);
            NumberRefreshFilteredList();
            SetListBoxNumber();
            SetCurrentNumber();
        }

        void SetCurrentNumber(){
            if (itemsNumbersFiltered.Count==0) {
                NumberSetNone();
                return;
            }

            int index=listBoxNumbers.SelectedIndex;
            if (index>=itemsNumbersFiltered.Count) index=itemsNumbersFiltered.Count-1;
            if (index<0) index=0;
            CurrentNumber=itemsNumbersFiltered[index];

            textBoxNumberFrom.Visible=true;
          //  textBoxNumberTo.Visible=true;
            labelNumberFrom.Visible=true;
          //  labelNumberTo.Visible=true;

            textBoxNumberFrom.Text=CurrentNumber.From;
          //  textBoxNumberTo.Text=CurrentNumber.To;

            comboBoxNumberInputPatternFrom.Text=CurrentNumber.PatternFrom;

            comboBoxNumberInputPatternFrom.Items.Clear();
         //   comboBoxNumberInputPatternTo.Items.Clear();
            foreach (ItemPatternNumber x in itemsPatternNumberFrom) {
                comboBoxNumberInputPatternFrom.Items.Add(x.Name);
            }
            //buttonAddToNumber.Visible=true;
            //foreach (ItemPatternNumber x in itemsPatternNumberTo) {
            //    comboBoxNumberInputPatternTo.Items.Add(x.Name);
            //}

        //    comboBoxNumberInputPatternTo.Text=CurrentNumber.PatternTo;

            comboBoxNumberInputPatternFrom.Visible=true;
        //    comboBoxNumberInputPatternTo.Visible=true;

            labelNumberInputPatternFrom.Visible=true;
         //   labelNumberInputPatternTo.Visible=true;

            labelNumberShowFrom.Visible=true;
           // labelNumberShowTo.Visible=true;
           simpleUINumbers.Visible=true;
            simpleUINumbers.Visible=true;
            simpleUINumbers.SetData(CurrentNumber.To.ToArray());
            List<string> options=new List<string>();
            foreach (ItemPatternNumber x in itemsPatternNumberTo) options.Add(x.Name);
            simpleUINumbers.SetComboboxes(options.ToArray());
        }

        void ListBoxSetCurrentNumber() {
            for (int indexCur=0; indexCur<itemsNumbersFiltered.Count; indexCur++) {
                if (itemsNumbers[indexCur]==CurrentNumber) {
                    int indexList=listBoxNumbers.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxNumbers.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentNumber() {
            if (CurrentNumber==null) return;
            Edited = true;
            CurrentNumber.From=textBoxNumberFrom.Text;
            //CurrentNumber.To=textBoxNumberTo.Text;

            CurrentNumber.PatternFrom=comboBoxNumberInputPatternFrom.Text;
           //CurrentNumber.PatternTo=comboBoxNumberInputPatternTo.Text;


            CurrentNumber.To=simpleUINumbers.GetData().ToList();
        }

        void NumberSetNone(){
            textBoxNumberFrom.Visible=false;
            textBoxNumberFrom.Text="";
           // textBoxNumberTo.Text="";
            comboBoxNumberInputPatternFrom.Text="";
           // comboBoxNumberInputPatternTo.Text="";

          //  textBoxNumberTo.Visible=false;
            labelNumberFrom.Visible=false;
          //  labelNumberTo.Visible=false;
            //comboBoxNumberInputPatternTo.Visible=false;
            comboBoxNumberInputPatternFrom.Visible=false;
            labelNumberShowFrom.Visible=false;
          //  labelNumberShowTo.Visible=false;
            labelNumberInputPatternFrom.Visible=false;
          //  labelNumberInputPatternTo.Visible=false;
          //buttonAddToNumber.Visible=false;
         //   textBoxNumberComment.Text="";
         //   textBoxNumberComment.Visible=false;
         //   labelNumberComment.Visible=false;
            simpleUINumbers.Visible=false;
            simpleUINumbers.Clear();
        }

        void ClearNumber(){
            listBoxNumbers.Items.Clear();
            NumberSetNone();
            itemsNumbersFiltered?.Clear();
            itemsNumbers?.Clear();
            CurrentNumber=null;
        }
        #endregion

        #region Verb
        void ListBoxVerb_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentVerb();

            int index=listBoxVerbs.SelectedIndex;
            if (itemsVerbs.Count==0) {
                VerbSetNone();
                return;
            }
            if (index>=itemsVerbs.Count)
                index=itemsVerbs.Count-1;
            if (index<0)
                index=0;

            CurrentVerb=itemsVerbs[index];
            SetCurrentVerb();
            VerbSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonVerbAdd_Click(object sender, EventArgs e) {
            AddNewItemVerb();
        }

        void ButtonVerbRemove_Click(object sender, EventArgs e) {
            RemoveItemVerb(CurrentVerb);
            TextBoxVerbFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxVerbFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentVerb();
            DrawingControl.SuspendDrawing(listBoxVerbs);
            // Získej aktuální prvek
            ItemVerb selectedId=null;
            if (listBoxVerbs.SelectedIndex!=-1) {
                selectedId=itemsVerbsFiltered[listBoxVerbs.SelectedIndex];
            }

            VerbRefreshFilteredList();

            listBoxVerbs.Items.Clear();
            for (int i=0; i<itemsVerbsFiltered.Count; i++) {
                ItemVerb item = itemsVerbsFiltered[i];

                //string textToAdd=item.GetText();

                string textToAdd=item.GetText(itemsPatternVerbFrom, itemsPatternVerbTo);
                //if (string.IsNullOrEmpty(item.PatternFrom)) {
                //        textToAdd="{"+item.PatternFrom+"}";
                //    } else textToAdd="<Neznámé>";

                listBoxVerbs.Items.Add(textToAdd);
            }

            //SetListBoxVerb();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsVerbsFiltered.Count; i++){
                    if (itemsVerbsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxVerbs.SelectedIndex=-1;
                    CurrentVerb=null;
                } else listBoxVerbs.SelectedIndex=outIndex;
            } else {
                listBoxVerbs.SelectedIndex=-1;
                CurrentVerb=null;
            }
            SetCurrentVerb();
            DrawingControl.ResumeDrawing(listBoxVerbs);
        }

        void RemoveCurrentVerb(object sender, EventArgs e) {
            itemsVerbs.Remove(CurrentVerb);
        }

        void VerbSetListBox() {
            int index=listBoxVerbs.SelectedIndex;
            listBoxVerbs.Items.Clear();
            for (int i=0; i<itemsVerbsFiltered.Count; i++) {
                ItemVerb item = itemsVerbsFiltered[i];


                string textToAdd=item.GetText(itemsPatternVerbFrom, itemsPatternVerbTo);
                //string textToAdd=item.GetText();
                //if (string.IsNullOrEmpty(textToAdd)) {
                //    if (string.IsNullOrEmpty(item.PatternFrom)) {
                //        textToAdd="{"+item.PatternFrom+"}";
                //    } else textToAdd="<Neznámé>";
                //}

                listBoxVerbs.Items.Add(textToAdd);
            }

            if (index>=listBoxVerbs.Items.Count)index=listBoxVerbs.Items.Count-1;
            listBoxVerbs.SelectedIndex=index;
        }

        void VerbRefreshFilteredList() {
            if (itemsVerbsFiltered==null) itemsVerbsFiltered=new List<ItemVerb>();
            itemsVerbsFiltered.Clear();
            string filter=textBoxVerbsFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsVerbs.Count; i++) {
                    ItemVerb item = itemsVerbs[i];

                    if (item.Filter(filter)) {
                        itemsVerbsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsVerbs.Count; i++) {
                    ItemVerb item = itemsVerbs[i];
                    itemsVerbsFiltered.Add(item);
                }
            }
        }

        void AddNewItemVerb() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentVerb();

            var newItem=new ItemVerb();
           // newItem.ID=itemsVerbs.Count;
           newItem.To=new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{ Body="",Pattern="",Comment=""} };
            itemsVerbs.Add(newItem);
            CurrentVerb=newItem;
            VerbRefreshFilteredList();
            VerbSetListBox();
            VerbListBoxSetCurrent();
            SetCurrentVerb();
            ///buttonVerbAddTo.Visible=true;
            doingJob=false;
        }

        void RemoveItemVerb(ItemVerb item) {
            Edited=true;
            ChangeCaptionText();
            itemsVerbs.Remove(item);
            VerbRefreshFilteredList();
            VerbSetListBox();
            SetCurrentVerb();
        }

        void SetCurrentVerb(){
            if (itemsVerbsFiltered.Count==0) {
                VerbSetNone();
                return;
            }

            int index=listBoxVerbs.SelectedIndex;
            if (index>=itemsVerbsFiltered.Count) index=itemsVerbsFiltered.Count-1;
            if (index<0) index=0;
            CurrentVerb=itemsVerbsFiltered[index];

            textBoxVerbFrom.Visible=true;
            //textBoxVerbTo.Visible=true;
            labelVerbFrom.Visible=true;
           // labelVerbTo.Visible=true;

            textBoxVerbFrom.Text=CurrentVerb.From;

            comboBoxVerbInputPatternFrom.Text=CurrentVerb.PatternFrom;

            comboBoxVerbInputPatternFrom.Items.Clear();
            //comboBoxVerbInputPatternTo.Items.Clear();
            foreach (ItemPatternVerb x in itemsPatternVerbFrom) {
                comboBoxVerbInputPatternFrom.Items.Add(x.Name);
            }
          //  foreach (ItemPatternVerb x in itemsPatternVerbTo) {
           //     comboBoxVerbInputPatternTo.Items.Add(x.Name);
           // }

           // comboBoxVerbInputPatternTo.Text=CurrentVerb.PatternTo;
           // textBoxVerbTo.Text=CurrentVerb.To;

            simpleUIVerbs.SetData(CurrentVerb.To.ToArray());
            List<string> options=new List<string>();
            foreach (ItemPatternVerb x in itemsPatternVerbTo) {
                options.Add(x.Name);
            }
            simpleUIVerbs.SetComboboxes(options.ToArray());

            comboBoxVerbInputPatternFrom.Visible=true;
           // comboBoxVerbInputPatternTo.Visible=true;

            labelVerbInputPatternFrom.Visible=true;
           // labelVerbInputPatternTo.Visible=true;
           //buttonVerbAddTo.Visible=true;
            labelVerbShowFrom.Visible=true;
            simpleUIVerbs.Visible=true;
           // labelVerbShowTo.Visible=true;
        }

        void VerbListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsVerbsFiltered.Count; indexCur++) {
                if (itemsVerbs[indexCur]==CurrentVerb) {
                    int indexList=listBoxVerbs.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxVerbs.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentVerb() {
            if (CurrentVerb==null) return;

            CurrentVerb.From=textBoxVerbFrom.Text;
            CurrentVerb.To=simpleUIVerbs.GetData().ToList();//textBoxVerbTo.Text;

            CurrentVerb.PatternFrom=comboBoxVerbInputPatternFrom.Text;
          //  CurrentVerb.PatternTo=comboBoxVerbInputPatternTo.Text;
        }

        void VerbSetNone(){
            textBoxVerbFrom.Text="";
           // textBoxVerbTo.Text="";

            comboBoxVerbInputPatternFrom.Text="";
            //comboBoxVerbInputPatternTo.Text="";
            //buttonVerbAddTo.Visible=false;
            textBoxVerbFrom.Visible=false;
        //    textBoxVerbTo.Visible=false;
            labelVerbFrom.Visible=false;
          //  labelVerbTo.Visible=false;
          //  comboBoxVerbInputPatternTo.Visible=false;
            comboBoxVerbInputPatternFrom.Visible=false;
            labelVerbShowFrom.Visible=false;
          //  labelVerbShowTo.Visible=false;
            labelVerbInputPatternFrom.Visible=false;
           // labelVerbInputPatternTo.Visible=false;

           // textBoxVerbComment.Text="";
           // textBoxVerbComment.Visible=false;
           // labelVerbComment.Visible=false;

            simpleUIVerbs.Visible=false;
        }

        void ClearVerb() {
            listBoxVerbs.Items.Clear();
            VerbSetNone();
            simpleUIVerbs.Clear();
            itemsVerbsFiltered?.Clear();
            itemsVerbs?.Clear();
            CurrentVerb=null;
        }
        #endregion

        #region VerbPattern From
        //void PatternVerbFromcomboBoxShowType_SelectedIndexChanged(object sender, EventArgs e) {
        //    if (doingJob) return;
        //    if (CurrentPatternVerbFrom!=null) {
        //        PatternVerbFromSaveCurrent();
        //        PatternVerbFromCheckBoxesApply();
        //        //if (CurrentPatternVerbFrom.TypeShow!=(VerbTypeShow)comboBoxPatternVerbFromShowType.SelectedIndex) {
        //        //    CurrentPatternVerbFrom.TypeShow=(VerbTypeShow)comboBoxPatternVerbFromShowType.SelectedIndex;
        //            PatternVerbFromSetCurrent();
        //        //}
        //    }
        //}

        void PatternVerbFromCheckBoxesSet() {
            if (CurrentPatternVerbFrom==null) {
                 checkBoxPatternVerbFromContinous.Checked        = false;
                 checkBoxPatternVerbFromPastActive.Checked       = false;
                 checkBoxPatternVerbFromPastPassive.Checked      = false;
                 checkBoxPatternVerbFromTransgressiveCont.Checked= false;
                 checkBoxPatternVerbFromTransgressivePast.Checked= false;
                 checkBoxPatternVerbFromAuxiliary.Checked        = false;
                 checkBoxPatternVerbFromFuture.Checked           = false;
                 checkBoxPatternVerbFromImperative.Checked       = false;
                return;
            }

            checkBoxPatternVerbFromContinous.Checked        = CurrentPatternVerbFrom.SContinous;
            checkBoxPatternVerbFromPastActive.Checked       = CurrentPatternVerbFrom.SPastActive;
            checkBoxPatternVerbFromPastPassive.Checked      = CurrentPatternVerbFrom.SPastPassive;
            checkBoxPatternVerbFromTransgressiveCont.Checked= CurrentPatternVerbFrom.STransgressiveCont;
            checkBoxPatternVerbFromTransgressivePast.Checked= CurrentPatternVerbFrom.STransgressivePast;
            checkBoxPatternVerbFromAuxiliary.Checked        = CurrentPatternVerbFrom.SAuxiliary;
            checkBoxPatternVerbFromFuture.Checked           = CurrentPatternVerbFrom.SFuture;
            checkBoxPatternVerbFromImperative.Checked       = CurrentPatternVerbFrom.SImperative;

            tableLayoutPanelPatternVerbFromContinous.Visible    = CurrentPatternVerbFrom.SContinous;
            tableLayoutPanelPatternVerbFromPastActive.Visible   = CurrentPatternVerbFrom.SPastActive;
            tableLayoutPanelPatternVerbFromPastPassive.Visible  = CurrentPatternVerbFrom.SPastPassive;
            textBoxPatternVerbFromTr1.Visible                   = CurrentPatternVerbFrom.STransgressiveCont;
            textBoxPatternVerbFromTr2.Visible                   = CurrentPatternVerbFrom.STransgressiveCont;
            textBoxPatternVerbFromTr3.Visible                   = CurrentPatternVerbFrom.STransgressiveCont;
            textBoxPatternVerbFromTr4.Visible                   = CurrentPatternVerbFrom.STransgressivePast;
            textBoxPatternVerbFromTr5.Visible                   = CurrentPatternVerbFrom.STransgressivePast;
            textBoxPatternVerbFromTr6.Visible                   = CurrentPatternVerbFrom.STransgressivePast;
            tableLayoutPanelPatternVerbFromAuxiliary.Visible    = CurrentPatternVerbFrom.SAuxiliary;
            tableLayoutPanelPatternVerbFromFuture.Visible       = CurrentPatternVerbFrom.SFuture;
            tableLayoutPanelPatternVerbFromImperative.Visible   = CurrentPatternVerbFrom.SImperative;
           tableLayoutPanelPatternVerbFromTransgressive.Visible=true;
       //     tableLayoutPanelPatternVerbFromTransgressive.Visible=(!CurrentPatternVerbFrom.STransgressiveCont || !CurrentPatternVerbFrom.STransgressivePast);
        }

        void PatternVerbFromCheckBoxesSave() {
            CurrentPatternVerbFrom.SContinous           = checkBoxPatternVerbFromContinous.Checked;
            CurrentPatternVerbFrom.SPastActive          = checkBoxPatternVerbFromPastActive.Checked;
            CurrentPatternVerbFrom.SPastPassive         = checkBoxPatternVerbFromPastPassive.Checked;
            CurrentPatternVerbFrom.STransgressiveCont   = checkBoxPatternVerbFromTransgressiveCont.Checked;
            CurrentPatternVerbFrom.STransgressivePast   = checkBoxPatternVerbFromTransgressivePast.Checked;
            CurrentPatternVerbFrom.SAuxiliary           = checkBoxPatternVerbFromAuxiliary.Checked;
            CurrentPatternVerbFrom.SFuture              = checkBoxPatternVerbFromFuture.Checked;
            CurrentPatternVerbFrom.SImperative          = checkBoxPatternVerbFromImperative.Checked;
        }

        void PatternVerbFromListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternVerbFromSaveCurrent();

            int index=PatternVerbFromlistBox.SelectedIndex;
            if (itemsPatternVerbFrom.Count==0) {
                PatternVerbFromSetNone();
                return;
            }
            if (index>=itemsPatternVerbFrom.Count) index=itemsPatternVerbFrom.Count-1;
            if (index<0)  index=0;

            CurrentPatternVerbFrom=itemsPatternVerbFrom[index];
            PatternVerbFromSetCurrent();
            PatternVerbFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternVerbFromButtonAdd_Click(object sender, EventArgs e) {
            PatternVerbFromAddNewItem();
        }

        void PatternVerbFromButtonRemove_Click(object sender, EventArgs e) {
            PatternVerbFromRemoveItem(CurrentPatternVerbFrom);
            PatternVerbFromTextBoxFilter_TextChanged(null, new EventArgs());
        }

        void PatternVerbFromTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternVerbFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternVerb selectedId=null;
            if (PatternVerbFromlistBox.SelectedIndex!=-1) {
                selectedId=itemsPatternVerbsFromFiltered[PatternVerbFromlistBox.SelectedIndex];
            }

            PatternVerbFromRefreshFilteredList();

            PatternVerbFromlistBox.Items.Clear();
            for (int i=0; i<itemsPatternVerbsFromFiltered.Count; i++) {
                ItemPatternVerb item = itemsPatternVerbsFromFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternVerbFromlistBox.Items.Add(textToAdd);
            }

            //SetListBoxVerb();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternVerbsFromFiltered.Count; i++){
                    if (itemsPatternVerbsFromFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    PatternVerbFromlistBox.SelectedIndex=-1;
                    CurrentPatternVerbFrom=null;
                } else PatternVerbFromlistBox.SelectedIndex=outIndex;
            } else {
                PatternVerbFromlistBox.SelectedIndex=-1;
                CurrentPatternVerbFrom=null;
            }
            PatternVerbFromSetCurrent();
        }

        void PatternVerbFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternVerbFrom.Remove(CurrentPatternVerbFrom);
        }

        void PatternVerbFromSetListBox() {
            int index=PatternVerbFromlistBox.SelectedIndex;
            PatternVerbFromlistBox.Items.Clear();
            for (int i=0; i<itemsPatternVerbsFromFiltered.Count; i++) {
                ItemPatternVerb item = itemsPatternVerbsFromFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternVerbFromlistBox.Items.Add(textToAdd);
            }

            if (index>=PatternVerbFromlistBox.Items.Count)index=PatternVerbFromlistBox.Items.Count-1;
            PatternVerbFromlistBox.SelectedIndex=index;
        }

        void PatternVerbFromRefreshFilteredList() {
            if (itemsPatternVerbsFromFiltered==null) itemsPatternVerbsFromFiltered=new List<ItemPatternVerb>();
            itemsPatternVerbsFromFiltered.Clear();
            string filter=textBoxPatternVerbFromFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPatternVerbFrom.Count; i++) {
                    ItemPatternVerb item = itemsPatternVerbFrom[i];

                    if (item.Filter(filter)) {
                        itemsPatternVerbsFromFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternVerbFrom.Count; i++) {
                    ItemPatternVerb item = itemsPatternVerbFrom[i];
                    itemsPatternVerbsFromFiltered.Add(item);
                }
            }
        }

        void PatternVerbFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternVerbFromSaveCurrent();

            var newItem=new ItemPatternVerb();
           // newItem.ID=itemsVerbs.Count;
            itemsPatternVerbFrom.Add(newItem);
            CurrentPatternVerbFrom=newItem;
            PatternVerbFromRefreshFilteredList();
            PatternVerbFromSetListBox();
            PatternVerbFromListBoxSetCurrent();
            PatternVerbFromSetCurrent();

            doingJob=false;
        }

        void PatternVerbFromRemoveItem(ItemPatternVerb item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternVerbFrom.Remove(item);
            PatternVerbFromRefreshFilteredList();
            PatternVerbFromSetListBox();
            PatternVerbFromSetCurrent();
        }

        void PatternVerbFromSetCurrent() {
            if (itemsPatternVerbsFromFiltered.Count==0) {
                PatternVerbFromSetNone();
                return;
            }

            int index=PatternVerbFromlistBox.SelectedIndex;
            if (index>=itemsPatternVerbsFromFiltered.Count) index=itemsPatternVerbsFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternVerbFrom=itemsPatternVerbsFromFiltered[index];

            textBoxPatternVerbFromName.Text=CurrentPatternVerbFrom.Name;
            comboBoxPatternVerbFromType.SelectedIndex=(int)CurrentPatternVerbFrom.Type;
            PatternVerbFromCheckBoxesSet();
          //  if (comboBoxPatternVerbFromShowType.SelectedIndex!=(int)CurrentPatternVerbFrom.TypeShow)comboBoxPatternVerbFromShowType.SelectedIndex=(int)CurrentPatternVerbFrom.TypeShow;
            textBoxPatternVerbFromInfinitive.Text=CurrentPatternVerbFrom.Infinitive;
            textBoxPatternVerbFromInfinitive.Visible=true;

            //if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.FutureActive || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.FuturePassive){
            //    tableLayoutPanelPatternVerbFromContinous.Visible=false;
            // //   labelPatternVerbFromContinous.Visible=false;
            //    labelPatternVerbFromContinousMultiple.Visible=false;
            //    labelPatternVerbFromContinousSingle.Visible=false;
            //  //  labelPatternVerbFromContinousPeople.Visible=false;
            //} else {
                textBoxPatternVerbFromPrJ1.Text=CurrentPatternVerbFrom.Continous[0];
                textBoxPatternVerbFromPrJ2.Text=CurrentPatternVerbFrom.Continous[1];
                textBoxPatternVerbFromPrJ3.Text=CurrentPatternVerbFrom.Continous[2];
                textBoxPatternVerbFromPrM1.Text=CurrentPatternVerbFrom.Continous[3];
                textBoxPatternVerbFromPrM2.Text=CurrentPatternVerbFrom.Continous[4];
                textBoxPatternVerbFromPrM3.Text=CurrentPatternVerbFrom.Continous[5];

            //    tableLayoutPanelPatternVerbFromContinous.Visible=true;
            // //   labelPatternVerbFromContinous.Visible=true;
            //    labelPatternVerbFromContinousMultiple.Visible=true;
            //    labelPatternVerbFromContinousSingle.Visible=true;
            // //   labelPatternVerbFromContinousPeople.Visible=true;
            //}

            //if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.All || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Unknown || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.FutureActive || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.FuturePassive) {
                textBoxPatternVerbFromBuJ1.Text=CurrentPatternVerbFrom.Future[0];
                textBoxPatternVerbFromBuJ2.Text=CurrentPatternVerbFrom.Future[1];
                textBoxPatternVerbFromBuJ3.Text=CurrentPatternVerbFrom.Future[2];
                textBoxPatternVerbFromBuM1.Text=CurrentPatternVerbFrom.Future[3];
                textBoxPatternVerbFromBuM2.Text=CurrentPatternVerbFrom.Future[4];
                textBoxPatternVerbFromBuM3.Text=CurrentPatternVerbFrom.Future[5];

             //   tableLayoutPanelPatternVerbFromFuture.Visible=true;
             //   labelPatternVerbFromFuture.Visible=true;
                labelPatternVerbFromFutureMultiple.Visible=true;
             //   labelPatternVerbFromFuturePeople.Visible=true;
                labelPatternVerbFromFutureSingle.Visible=true;
            //} else {
            //    tableLayoutPanelPatternVerbFromFuture.Visible=false;
            // //   labelPatternVerbFromFuture.Visible=false;
            //    labelPatternVerbFromFutureMultiple.Visible=false;
            ////    labelPatternVerbFromFuturePeople.Visible=false;
            //    labelPatternVerbFromFutureSingle.Visible=false;
            //}

            //if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.All || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Unknown){
                textBoxPatternVerbFromAux1.Text=CurrentPatternVerbFrom.Auxiliary[0];
                textBoxPatternVerbFromAux2.Text=CurrentPatternVerbFrom.Auxiliary[1];
                textBoxPatternVerbFromAux3.Text=CurrentPatternVerbFrom.Auxiliary[2];
                textBoxPatternVerbFromAux4.Text=CurrentPatternVerbFrom.Auxiliary[3];
                textBoxPatternVerbFromAux5.Text=CurrentPatternVerbFrom.Auxiliary[4];
                textBoxPatternVerbFromAux6.Text=CurrentPatternVerbFrom.Auxiliary[5];

                textBoxPatternVerbFromAux1.Visible=true;
                textBoxPatternVerbFromAux2.Visible=true;
                textBoxPatternVerbFromAux3.Visible=true;
                textBoxPatternVerbFromAux4.Visible=true;
                textBoxPatternVerbFromAux5.Visible=true;
                textBoxPatternVerbFromAux6.Visible=true;
            //    tableLayoutPanelPatternVerbFromAuxiliary.Visible=true;
              //  labelPatternVerbFromAuxText.Visible=true;
                labelPatternVerbFromSingle.Visible=true;
                labelPatternVerbFromMultiple.Visible=true;
            //}else{


            //    textBoxPatternVerbFromAux1.Visible=false;
            //    textBoxPatternVerbFromAux2.Visible=false;
            //    textBoxPatternVerbFromAux3.Visible=false;
            //    textBoxPatternVerbFromAux4.Visible=false;
            //    textBoxPatternVerbFromAux5.Visible=false;
            //    textBoxPatternVerbFromAux6.Visible=false;
            //    tableLayoutPanelPatternVerbFromAuxiliary.Visible=false;
            //  //  labelPatternVerbFromAuxText.Visible=false;
            //    labelPatternVerbFromSingle.Visible=false;
            //    labelPatternVerbFromMultiple.Visible=false;
            //}

            textBoxPatternVerbFromRoJ2.Text=CurrentPatternVerbFrom.Imperative[0];
            textBoxPatternVerbFromRoM1.Text=CurrentPatternVerbFrom.Imperative[1];
            textBoxPatternVerbFromRoM2.Text=CurrentPatternVerbFrom.Imperative[2];
         //   tableLayoutPanelPatternVerbFromImperative.Visible=true;
         //   labelPatternVerbFromImperative.Visible=true;
            labelPatternVerbFromImperativeMultiple.Visible=true;
         //   labelPatternVerbFromImperativePeople.Visible=true;
            labelPatternVerbFromImperativeSingle.Visible=true;

            //if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Trpne || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.TrpneCinne || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.All || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Unknown) {
                textBoxPatternVerbFromMtMzJ.Text=CurrentPatternVerbFrom.PastPassive[0];
                textBoxPatternVerbFromMtMnJ.Text=CurrentPatternVerbFrom.PastPassive[1];
                textBoxPatternVerbFromMtZeJ.Text=CurrentPatternVerbFrom.PastPassive[2];
                textBoxPatternVerbFromMtStJ.Text=CurrentPatternVerbFrom.PastPassive[3];
                textBoxPatternVerbFromMtMzM.Text=CurrentPatternVerbFrom.PastPassive[4];
                textBoxPatternVerbFromMtMnM.Text=CurrentPatternVerbFrom.PastPassive[5];
                textBoxPatternVerbFromMtZeM.Text=CurrentPatternVerbFrom.PastPassive[6];
                textBoxPatternVerbFromMtStM.Text=CurrentPatternVerbFrom.PastPassive[7];

              //  tableLayoutPanelPatternVerbFromPastPassive.Visible=true;
           //     labelPatternVerbFromPastInactive.Visible=true;
                labelPatternVerbFromPastInactiveMultiple.Visible=true;
            //   labelPatternVerbFromPastInactivePeople.Visible=true;
                labelPatternVerbFromPastInactiveSingle.Visible=true;
            //} else {
            //    tableLayoutPanelPatternVerbFromPastPassive.Visible=false;
            // //   labelPatternVerbFromPastInactive.Visible=false;
            //    labelPatternVerbFromPastInactiveMultiple.Visible=false;
            //  //  labelPatternVerbFromPastInactivePeople.Visible=false;
            //    labelPatternVerbFromPastInactiveSingle.Visible=false;
            //}

            //if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Cinne || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.TrpneCinne || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.All || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Unknown) {
                textBoxPatternVerbFromMcMzJ.Text=CurrentPatternVerbFrom.PastActive[0];
                textBoxPatternVerbFromMcMnJ.Text=CurrentPatternVerbFrom.PastActive[1];
                textBoxPatternVerbFromMcZeJ.Text=CurrentPatternVerbFrom.PastActive[2];
                textBoxPatternVerbFromMcStJ.Text=CurrentPatternVerbFrom.PastActive[3];
                textBoxPatternVerbFromMcMzM.Text=CurrentPatternVerbFrom.PastActive[4];
                textBoxPatternVerbFromMcMnM.Text=CurrentPatternVerbFrom.PastActive[5];
                textBoxPatternVerbFromMcZeM.Text=CurrentPatternVerbFrom.PastActive[6];
                textBoxPatternVerbFromMcStM.Text=CurrentPatternVerbFrom.PastActive[7];

            //    tableLayoutPanelPatternVerbFromPastActive.Visible=true;
             //   labelPatternVerbFromPastActive.Visible=true;
                labelPatternVerbFromPastActiveMultiple.Visible=true;
            //    labelPatternVerbFromPastActivePeople.Visible=true;
                labelPatternVerbFromPastActiveSingle.Visible=true;
            //}else{
            //     tableLayoutPanelPatternVerbFromPastActive.Visible=false;
            ////    labelPatternVerbFromPastActive.Visible=false;
            //    labelPatternVerbFromPastActiveMultiple.Visible=false;
            // //   labelPatternVerbFromPastActivePeople.Visible=false;
            //    labelPatternVerbFromPastActiveSingle.Visible=false;
            //}

            textBoxPatternVerbFromTr1.Text=CurrentPatternVerbFrom.TransgressiveCont[0];
            textBoxPatternVerbFromTr2.Text=CurrentPatternVerbFrom.TransgressiveCont[1];
            textBoxPatternVerbFromTr3.Text=CurrentPatternVerbFrom.TransgressiveCont[2];
            textBoxPatternVerbFromTr4.Text=CurrentPatternVerbFrom.TransgressivePast[0];
            textBoxPatternVerbFromTr5.Text=CurrentPatternVerbFrom.TransgressivePast[1];
            textBoxPatternVerbFromTr6.Text=CurrentPatternVerbFrom.TransgressivePast[2];

            tableLayoutPanelPatternVerbFromTransgressive.Visible=true;
            labelPatternVerbFromTransgressive.Visible=true;
          //  PatternVerbFromTransgressiveContinous.Visible=true;
          //  PatternVerbFromTransgressivePast.Visible=true;

            textBoxPatternVerbFromTr1.Text=CurrentPatternVerbFrom.TransgressiveCont[0];
            textBoxPatternVerbFromTr2.Text=CurrentPatternVerbFrom.TransgressiveCont[1];
            textBoxPatternVerbFromTr3.Text=CurrentPatternVerbFrom.TransgressiveCont[2];
            textBoxPatternVerbFromTr4.Text=CurrentPatternVerbFrom.TransgressivePast[0];
            textBoxPatternVerbFromTr5.Text=CurrentPatternVerbFrom.TransgressivePast[1];
            textBoxPatternVerbFromTr6.Text=CurrentPatternVerbFrom.TransgressivePast[2];


            textBoxPatternVerbFromName.Visible=true;
            labelPatternVerbFromName.Visible=true;
            labelPatternVerbFromType.Visible=true;

            comboBoxPatternVerbFromType.Visible=true;

            textBoxPatternVerbFromInfinitive.Visible=true;
            labelPatternVerbFromInfinitive.Visible=true;
        }

        void PatternVerbFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternVerbsFromFiltered.Count; indexCur++) {
                if (itemsPatternVerbFrom[indexCur]==CurrentPatternVerbFrom) {
                    int indexList=PatternVerbFromlistBox.SelectedIndex;
                    if (indexList==indexCur) return;
                    PatternVerbFromlistBox.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternVerbFromSaveCurrent() {
            if (CurrentPatternVerbFrom==null) return;
            PatternVerbFromCheckBoxesSave();

            CurrentPatternVerbFrom.Name=textBoxPatternVerbFromName.Text;
            CurrentPatternVerbFrom.Type=(VerbType)comboBoxPatternVerbFromType.SelectedIndex;
           // CurrentPatternVerbFrom.TypeShow=(VerbTypeShow)comboBoxPatternVerbFromShowType.SelectedIndex;
            CurrentPatternVerbFrom.Infinitive=textBoxPatternVerbFromInfinitive.Text;

            CurrentPatternVerbFrom.Continous[0]=textBoxPatternVerbFromPrJ1.Text;
            CurrentPatternVerbFrom.Continous[1]=textBoxPatternVerbFromPrJ2.Text;
            CurrentPatternVerbFrom.Continous[2]=textBoxPatternVerbFromPrJ3.Text;
            CurrentPatternVerbFrom.Continous[3]=textBoxPatternVerbFromPrM1.Text;
            CurrentPatternVerbFrom.Continous[4]=textBoxPatternVerbFromPrM2.Text;
            CurrentPatternVerbFrom.Continous[5]=textBoxPatternVerbFromPrM3.Text;

            CurrentPatternVerbFrom.Future[0]=textBoxPatternVerbFromBuJ1.Text;
            CurrentPatternVerbFrom.Future[1]=textBoxPatternVerbFromBuJ2.Text;
            CurrentPatternVerbFrom.Future[2]=textBoxPatternVerbFromBuJ3.Text;
            CurrentPatternVerbFrom.Future[3]=textBoxPatternVerbFromBuM1.Text;
            CurrentPatternVerbFrom.Future[4]=textBoxPatternVerbFromBuM2.Text;
            CurrentPatternVerbFrom.Future[5]=textBoxPatternVerbFromBuM3.Text;

            CurrentPatternVerbFrom.Imperative[0]=textBoxPatternVerbFromRoJ2.Text;
            CurrentPatternVerbFrom.Imperative[1]=textBoxPatternVerbFromRoM1.Text;
            CurrentPatternVerbFrom.Imperative[2]=textBoxPatternVerbFromRoM2.Text;

            CurrentPatternVerbFrom.PastPassive[0]=textBoxPatternVerbFromMtMzJ.Text;
            CurrentPatternVerbFrom.PastPassive[1]=textBoxPatternVerbFromMtMnJ.Text;
            CurrentPatternVerbFrom.PastPassive[2]=textBoxPatternVerbFromMtZeJ.Text;
            CurrentPatternVerbFrom.PastPassive[3]=textBoxPatternVerbFromMtStJ.Text;
            CurrentPatternVerbFrom.PastPassive[4]=textBoxPatternVerbFromMtMzM.Text;
            CurrentPatternVerbFrom.PastPassive[5]=textBoxPatternVerbFromMtMnM.Text;
            CurrentPatternVerbFrom.PastPassive[6]=textBoxPatternVerbFromMtZeM.Text;
            CurrentPatternVerbFrom.PastPassive[7]=textBoxPatternVerbFromMtStM.Text;

            CurrentPatternVerbFrom.PastActive[0]=textBoxPatternVerbFromMcMzJ.Text;
            CurrentPatternVerbFrom.PastActive[1]=textBoxPatternVerbFromMcMnJ.Text;
            CurrentPatternVerbFrom.PastActive[2]=textBoxPatternVerbFromMcZeJ.Text;
            CurrentPatternVerbFrom.PastActive[3]=textBoxPatternVerbFromMcStJ.Text;
            CurrentPatternVerbFrom.PastActive[4]=textBoxPatternVerbFromMcMzM.Text;
            CurrentPatternVerbFrom.PastActive[5]=textBoxPatternVerbFromMcMnM.Text;
            CurrentPatternVerbFrom.PastActive[6]=textBoxPatternVerbFromMcZeM.Text;
            CurrentPatternVerbFrom.PastActive[7]=textBoxPatternVerbFromMcStM.Text;

            CurrentPatternVerbFrom.TransgressiveCont[0]=textBoxPatternVerbFromTr1.Text;
            CurrentPatternVerbFrom.TransgressiveCont[1]=textBoxPatternVerbFromTr2.Text;
            CurrentPatternVerbFrom.TransgressiveCont[2]=textBoxPatternVerbFromTr3.Text;
            CurrentPatternVerbFrom.TransgressivePast[0]=textBoxPatternVerbFromTr4.Text;
            CurrentPatternVerbFrom.TransgressivePast[1]=textBoxPatternVerbFromTr5.Text;
            CurrentPatternVerbFrom.TransgressivePast[2]=textBoxPatternVerbFromTr6.Text;

            CurrentPatternVerbFrom.Auxiliary[0]=textBoxPatternVerbFromAux1.Text;
            CurrentPatternVerbFrom.Auxiliary[1]=textBoxPatternVerbFromAux2.Text;
            CurrentPatternVerbFrom.Auxiliary[2]=textBoxPatternVerbFromAux3.Text;
            CurrentPatternVerbFrom.Auxiliary[3]=textBoxPatternVerbFromAux4.Text;
            CurrentPatternVerbFrom.Auxiliary[4]=textBoxPatternVerbFromAux5.Text;
            CurrentPatternVerbFrom.Auxiliary[5]=textBoxPatternVerbFromAux6.Text;
        }

        void PatternVerbFromSetNone(){
            textBoxPatternVerbFromName.Text="";

            textBoxPatternVerbFromPrJ1.Text="";
            textBoxPatternVerbFromPrJ2.Text="";
            textBoxPatternVerbFromPrJ3.Text="";
            textBoxPatternVerbFromPrM1.Text="";
            textBoxPatternVerbFromPrM2.Text="";
            textBoxPatternVerbFromPrM3.Text="";

            textBoxPatternVerbFromBuJ1.Text="";
            textBoxPatternVerbFromBuJ2.Text="";
            textBoxPatternVerbFromBuJ3.Text="";
            textBoxPatternVerbFromBuM1.Text="";
            textBoxPatternVerbFromBuM2.Text="";
            textBoxPatternVerbFromBuM3.Text="";

            textBoxPatternVerbFromRoJ2.Text="";
            textBoxPatternVerbFromRoM1.Text="";
            textBoxPatternVerbFromRoM2.Text="";

            textBoxPatternVerbFromMtMzJ.Text="";
            textBoxPatternVerbFromMtMnJ.Text="";
            textBoxPatternVerbFromMtZeJ.Text="";
            textBoxPatternVerbFromMtStJ.Text="";
            textBoxPatternVerbFromMtMzM.Text="";
            textBoxPatternVerbFromMtMnM.Text="";
            textBoxPatternVerbFromMtZeM.Text="";
            textBoxPatternVerbFromMtStM.Text="";

            textBoxPatternVerbFromMcMzJ.Text="";
            textBoxPatternVerbFromMcMnJ.Text="";
            textBoxPatternVerbFromMcZeJ.Text="";
            textBoxPatternVerbFromMcStJ.Text="";
            textBoxPatternVerbFromMcMzM.Text="";
            textBoxPatternVerbFromMcMnM.Text="";
            textBoxPatternVerbFromMcZeM.Text="";
            textBoxPatternVerbFromMcStM.Text="";


            textBoxPatternVerbFromTr1.Text="";
            textBoxPatternVerbFromTr2.Text="";
            textBoxPatternVerbFromTr3.Text="";
            textBoxPatternVerbFromTr4.Text="";
            textBoxPatternVerbFromTr5.Text="";
            textBoxPatternVerbFromTr6.Text="";

            PatternVerbFromCheckBoxesSet();

            //labelPatternVerbFromName.Visible=false;
            //labelPatternVerbFromType.Visible=false;
            //tableLayoutPanelPatternVerbFromContinous.Visible=false;
            //labelPatternVerbFromContinous.Visible=false;
            //labelPatternVerbFromContinousMultiple.Visible=false;
            //labelPatternVerbFromContinousSingle.Visible=false;
            //PatternVerbFromTransgressiveContinous.Visible=false;
            //tableLayoutPanelPatternVerbFromFuture.Visible=false;
            //labelPatternVerbFromFuture.Visible=false;
            //labelPatternVerbFromFutureMultiple.Visible=false;
            //labelPatternVerbFromFuturePeople.Visible=false;
            //labelPatternVerbFromFutureSingle.Visible=false;
            //comboBoxPatternVerbFromType.Visible=false;
            //PatternVerbFromTransgressivePast.Visible=false;
            //tableLayoutPanelPatternVerbFromImperative.Visible=false;
            //labelPatternVerbFromImperative.Visible=false;
            //labelPatternVerbFromImperativeMultiple.Visible=false;
            //labelPatternVerbFromImperativePeople.Visible=false;
            //labelPatternVerbFromImperativeSingle.Visible=false;
            //labelPatternVerbFromContinousPeople.Visible=false;
            //tableLayoutPanelPatternVerbFromPastPassive.Visible=false;
            //labelPatternVerbFromPastInactive.Visible=false;
            //labelPatternVerbFromPastInactiveMultiple.Visible=false;
            //labelPatternVerbFromPastInactivePeople.Visible=false;
            //labelPatternVerbFromPastInactiveSingle.Visible=false;
            //textBoxPatternVerbFromName.Visible=false;
            //tableLayoutPanelPatternVerbFromPastActive.Visible=false;
            //labelPatternVerbFromPastActive.Visible=false;
            //labelPatternVerbFromPastActiveMultiple.Visible=false;
            //labelPatternVerbFromPastActivePeople.Visible=false;
            //labelPatternVerbFromPastActiveSingle.Visible=false;

            //tableLayoutPanelPatternVerbFromTransgressive.Visible=false;
            //labelPatternVerbFromTransgressive.Visible=false;
        }

        void PatternVerbFromClear() {
            PatternVerbFromlistBox.Items.Clear();
            PatternVerbFromSetNone();
            itemsPatternVerbsFromFiltered?.Clear();
            itemsPatternVerbFrom?.Clear();
            CurrentPatternVerbFrom=null;
        }
        #endregion

        #region VerbPattern To
        void PatternVerbToCheckBoxesSet() {
            if (CurrentPatternVerbTo==null) {
                checkBoxPatternVerbToContinous.Visible        = false;
                checkBoxPatternVerbToPastActive.Visible       = false;
                checkBoxPatternVerbToPastPassive.Visible      = false;
                checkBoxPatternVerbToTransgressiveCont.Visible= false;
                checkBoxPatternVerbToTransgressivePast.Visible= false;
                checkBoxPatternVerbToAuxiliary.Visible        = false;
                checkBoxPatternVerbToFuture.Visible           = false;
                checkBoxPatternVerbToImperative.Visible       = false;
                return;
            }
            checkBoxPatternVerbToContinous.Visible        = true;
            checkBoxPatternVerbToPastActive.Visible       = true;
            checkBoxPatternVerbToPastPassive.Visible      = true;
            checkBoxPatternVerbToTransgressiveCont.Visible= true;
            checkBoxPatternVerbToTransgressivePast.Visible= true;
            checkBoxPatternVerbToAuxiliary.Visible        = true;
            checkBoxPatternVerbToFuture.Visible           = true;
            checkBoxPatternVerbToImperative.Visible       = true;
            checkBoxPatternVerbToContinous.Checked        = CurrentPatternVerbTo.SContinous;
            checkBoxPatternVerbToPastActive.Checked       = CurrentPatternVerbTo.SPastActive;
            checkBoxPatternVerbToPastPassive.Checked      = CurrentPatternVerbTo.SPastPassive;
            checkBoxPatternVerbToTransgressiveCont.Checked= CurrentPatternVerbTo.STransgressiveCont;
            checkBoxPatternVerbToTransgressivePast.Checked= CurrentPatternVerbTo.STransgressivePast;
            checkBoxPatternVerbToAuxiliary.Checked        = CurrentPatternVerbTo.SAuxiliary;
            checkBoxPatternVerbToFuture.Checked           = CurrentPatternVerbTo.SFuture;
            checkBoxPatternVerbToImperative.Checked       = CurrentPatternVerbTo.SImperative;

            tableLayoutPanelPatternVerbToContinous.Visible    = CurrentPatternVerbTo.SContinous;
            tableLayoutPanelPatternVerbToPastActive.Visible   = CurrentPatternVerbTo.SPastActive;
            tableLayoutPanelPatternVerbToPastPassive.Visible  = CurrentPatternVerbTo.SPastPassive;
            textBoxPatternVerbToTr1.Visible                   = CurrentPatternVerbTo.STransgressiveCont;
            textBoxPatternVerbToTr2.Visible                   = CurrentPatternVerbTo.STransgressiveCont;
            textBoxPatternVerbToTr3.Visible                   = CurrentPatternVerbTo.STransgressiveCont;
            textBoxPatternVerbToTr4.Visible                   = CurrentPatternVerbTo.STransgressivePast;
            textBoxPatternVerbToTr5.Visible                   = CurrentPatternVerbTo.STransgressivePast;
            textBoxPatternVerbToTr6.Visible                   = CurrentPatternVerbTo.STransgressivePast;
            tableLayoutPanelPatternVerbToAuxiliary.Visible    = CurrentPatternVerbTo.SAuxiliary;
            tableLayoutPanelPatternVerbToFuture.Visible       = CurrentPatternVerbTo.SFuture;
            tableLayoutPanelPatternVerbToImperative.Visible   = CurrentPatternVerbTo.SImperative;
            tableLayoutPanelPatternVerbToTransgressive.Visible=true;
       //    tableLayoutPanelPatternVerbToTransgressive.Visible=(!CurrentPatternVerbTo.STransgressiveCont || !CurrentPatternVerbTo.STransgressivePast);
        }

        void PatternVerbToCheckBoxesSave() {
            CurrentPatternVerbTo.SContinous           = checkBoxPatternVerbToContinous.Checked;
            CurrentPatternVerbTo.SPastActive          = checkBoxPatternVerbToPastActive.Checked;
            CurrentPatternVerbTo.SPastPassive         = checkBoxPatternVerbToPastPassive.Checked;
            CurrentPatternVerbTo.STransgressiveCont   = checkBoxPatternVerbToTransgressiveCont.Checked;
            CurrentPatternVerbTo.STransgressivePast   = checkBoxPatternVerbToTransgressivePast.Checked;
            CurrentPatternVerbTo.SAuxiliary           = checkBoxPatternVerbToAuxiliary.Checked;
            CurrentPatternVerbTo.SFuture              = checkBoxPatternVerbToFuture.Checked;
            CurrentPatternVerbTo.SImperative          = checkBoxPatternVerbToImperative.Checked;
        }

        void PatternVerbTocomboBoxShowType_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            if (CurrentPatternVerbTo!=null) {
                PatternVerbToSaveCurrent();
                PatternVerbToCheckBoxesSave();
                PatternVerbToSetCurrent();
                //if (CurrentPatternVerbTo.TypeShow!=(VerbTypeShow)comboBoxPatternVerbToShowType.SelectedIndex) {
                //    CurrentPatternVerbTo.TypeShow=(VerbTypeShow)comboBoxPatternVerbToShowType.SelectedIndex;

                //}
            }
        }

        void PatternVerbToListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternVerbToSaveCurrent();

            int index=PatternVerbToListBox.SelectedIndex;
            if (itemsPatternVerbTo.Count==0) {
                PatternVerbToSetNone();
                return;
            }
            if (index>=itemsPatternVerbTo.Count) index=itemsPatternVerbTo.Count-1;
            if (index<0)  index=0;

            CurrentPatternVerbTo=itemsPatternVerbTo[index];
            PatternVerbToSetCurrent();
            PatternVerbToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternVerbToButtonAdd_Click(object sender, EventArgs e) {
            PatternVerbToAddNewItem();
        }

        void PatternVerbToButtonRemove_Click(object sender, EventArgs e) {
            PatternVerbToRemoveItem(CurrentPatternVerbTo);
            PatternVerbToTextBoxFilter_TextChanged(null, new EventArgs());
        }

        void PatternVerbToTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternVerbToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternVerb selectedId=null;
            if (PatternVerbToListBox.SelectedIndex!=-1) {
                selectedId=itemsPatternVerbsToFiltered[PatternVerbToListBox.SelectedIndex];
            }

            PatternVerbToRefreshFilteredList();

            PatternVerbToListBox.Items.Clear();
            for (int i=0; i<itemsPatternVerbsToFiltered.Count; i++) {
                ItemPatternVerb item = itemsPatternVerbsToFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternVerbToListBox.Items.Add(textToAdd);
            }

            //SetListBoxVerb();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPatternVerbsToFiltered.Count; i++){
                    if (itemsPatternVerbsToFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    PatternVerbToListBox.SelectedIndex=-1;
                    CurrentPatternVerbTo=null;
                } else PatternVerbToListBox.SelectedIndex=outIndex;
            } else {
                PatternVerbToListBox.SelectedIndex=-1;
                CurrentPatternVerbTo=null;
            }
            PatternVerbToSetCurrent();
        }

        void PatternVerbToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternVerbTo.Remove(CurrentPatternVerbTo);
        }

        void PatternVerbToSetListBox() {
            //string filter=textBoxPatternVerbToFilter.Text;
          //  bool useFilter = filter!="" && filter!="*";

            int index=PatternVerbToListBox.SelectedIndex;
            PatternVerbToListBox.Items.Clear();
            for (int i=0; i<itemsPatternVerbsToFiltered.Count; i++) {
                ItemPatternVerb item = itemsPatternVerbsToFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternVerbToListBox.Items.Add(textToAdd);
            }

            if (index>=PatternVerbToListBox.Items.Count)index=PatternVerbToListBox.Items.Count-1;
            PatternVerbToListBox.SelectedIndex=index;
        }

        void PatternVerbToRefreshFilteredList() {
            if (itemsPatternVerbsToFiltered==null) itemsPatternVerbsToFiltered=new List<ItemPatternVerb>();
            itemsPatternVerbsToFiltered.Clear();
            string filter=textBoxPatternVerbToFilter.Text;
            //bool useFilter = ;

            if (filter!="" && filter!="*") {
                for (int i=0; i<itemsPatternVerbTo.Count; i++) {
                    ItemPatternVerb item = itemsPatternVerbTo[i];

                    if (item.Filter(filter)) {
                        itemsPatternVerbsToFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPatternVerbTo.Count; i++) {
                    ItemPatternVerb item = itemsPatternVerbTo[i];
                    itemsPatternVerbsToFiltered.Add(item);
                }
            }
        }

        void PatternVerbToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternVerbToSaveCurrent();

            var newItem=new ItemPatternVerb();
           // newItem.ID=itemsVerbs.Count;
            itemsPatternVerbTo.Add(newItem);
            CurrentPatternVerbTo=newItem;
            PatternVerbToRefreshFilteredList();
            PatternVerbToSetListBox();
            PatternVerbToListBoxSetCurrent();
            PatternVerbToSetCurrent();

            doingJob=false;
        }

        void PatternVerbToRemoveItem(ItemPatternVerb item) {
            Edited=true;
            ChangeCaptionText();
            itemsPatternVerbTo.Remove(item);
            PatternVerbToRefreshFilteredList();
            PatternVerbToSetListBox();
            PatternVerbToSetCurrent();
        }

        void PatternVerbToSetCurrent() {
            if (itemsPatternVerbsToFiltered.Count==0) {
                PatternVerbToSetNone();
                return;
            }

            int index=PatternVerbToListBox.SelectedIndex;
            if (index>=itemsPatternVerbsToFiltered.Count) index=itemsPatternVerbsToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternVerbTo=itemsPatternVerbsToFiltered[index];

            textBoxPatternVerbToName.Text=CurrentPatternVerbTo.Name;
            comboBoxPatternVerbToType.SelectedIndex=(int)CurrentPatternVerbTo.Type;
            PatternVerbToCheckBoxesSet();
           // if (comboBoxPatternVerbToShowType.SelectedIndex!=(int)CurrentPatternVerbTo.TypeShow)comboBoxPatternVerbToShowType.SelectedIndex=(int)CurrentPatternVerbTo.TypeShow;
            textBoxPatternVerbToInfinitive.Text=CurrentPatternVerbTo.Infinitive;
            textBoxPatternVerbToInfinitive.Visible=true;

            //if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.FutureActive || CurrentPatternVerbTo.TypeShow==VerbTypeShow.FuturePassive){
            //    tableLayoutPanelPatternVerbToContinous.Visible=false;
            //    labelPatternVerbToContinous.Visible=false;
            //    labelPatternVerbToContinousMultiple.Visible=false;
            //    labelPatternVerbToContinousSingle.Visible=false;
            //    labelPatternVerbToContinousPeople.Visible=false;
            //} else {
                textBoxPatternVerbToPrJ1.Text=CurrentPatternVerbTo.Continous[0];
                textBoxPatternVerbToPrJ2.Text=CurrentPatternVerbTo.Continous[1];
                textBoxPatternVerbToPrJ3.Text=CurrentPatternVerbTo.Continous[2];
                textBoxPatternVerbToPrM1.Text=CurrentPatternVerbTo.Continous[3];
                textBoxPatternVerbToPrM2.Text=CurrentPatternVerbTo.Continous[4];
                textBoxPatternVerbToPrM3.Text=CurrentPatternVerbTo.Continous[5];

              //  tableLayoutPanelPatternVerbToContinous.Visible=true;
                checkBoxPatternVerbToContinous.Visible=true;
                labelPatternVerbToContinousMultiple.Visible=true;
                labelPatternVerbToContinousSingle.Visible=true;
               // labelPatternVerbToContinousPeople.Visible=true;
           // }

           // if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.All || CurrentPatternVerbTo.TypeShow==VerbTypeShow.Unknown || CurrentPatternVerbTo.TypeShow==VerbTypeShow.FutureActive || CurrentPatternVerbTo.TypeShow==VerbTypeShow.FuturePassive) {
                textBoxPatternVerbToBuJ1.Text=CurrentPatternVerbTo.Future[0];
                textBoxPatternVerbToBuJ2.Text=CurrentPatternVerbTo.Future[1];
                textBoxPatternVerbToBuJ3.Text=CurrentPatternVerbTo.Future[2];
                textBoxPatternVerbToBuM1.Text=CurrentPatternVerbTo.Future[3];
                textBoxPatternVerbToBuM2.Text=CurrentPatternVerbTo.Future[4];
                textBoxPatternVerbToBuM3.Text=CurrentPatternVerbTo.Future[5];

               // tableLayoutPanelPatternVerbToFuture.Visible=true;
                checkBoxPatternVerbToFuture.Visible=true;
                labelPatternVerbToFutureMultiple.Visible=true;
               // labelPatternVerbToFuturePeople.Visible=true;
                labelPatternVerbToFutureSingle.Visible=true;
            //} else {
            //    tableLayoutPanelPatternVerbToFuture.Visible=false;
            //    labelPatternVerbToFuture.Visible=false;
            //    labelPatternVerbToFutureMultiple.Visible=false;
            //    labelPatternVerbToFuturePeople.Visible=false;
            //    labelPatternVerbToFutureSingle.Visible=false;
            //}

            //if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.All || CurrentPatternVerbTo.TypeShow==VerbTypeShow.Unknown){
                textBoxPatternVerbToAux1.Text=CurrentPatternVerbTo.Auxiliary[0];
                textBoxPatternVerbToAux2.Text=CurrentPatternVerbTo.Auxiliary[1];
                textBoxPatternVerbToAux3.Text=CurrentPatternVerbTo.Auxiliary[2];
                textBoxPatternVerbToAux4.Text=CurrentPatternVerbTo.Auxiliary[3];
                textBoxPatternVerbToAux5.Text=CurrentPatternVerbTo.Auxiliary[4];
                textBoxPatternVerbToAux6.Text=CurrentPatternVerbTo.Auxiliary[5];

                textBoxPatternVerbToAux1.Visible=true;
                textBoxPatternVerbToAux2.Visible=true;
                textBoxPatternVerbToAux3.Visible=true;
                textBoxPatternVerbToAux4.Visible=true;
                textBoxPatternVerbToAux5.Visible=true;
                textBoxPatternVerbToAux6.Visible=true;
              //  tableLayoutPanelPatternVerbToAuxiliary.Visible=true;
                checkBoxPatternVerbToAuxiliary.Visible=true;
                labelPatternVerbToSingle.Visible=true;
                labelPatternVerbToMultiple.Visible=true;
            //}else{
            //    textBoxPatternVerbToAux1.Visible=false;
            //    textBoxPatternVerbToAux2.Visible=false;
            //    textBoxPatternVerbToAux3.Visible=false;
            //    textBoxPatternVerbToAux4.Visible=false;
            //    textBoxPatternVerbToAux5.Visible=false;
            //    textBoxPatternVerbToAux6.Visible=false;
            //    tableLayoutPanelPatternVerbToAux.Visible=false;
            //    labelPatternVerbToAuxText.Visible=false;
            //    labelPatternVerbToSingle.Visible=false;
            //    labelPatternVerbToMultiple.Visible=false;
            //}

            textBoxPatternVerbToRoJ2.Text=CurrentPatternVerbTo.Imperative[0];
            textBoxPatternVerbToRoM1.Text=CurrentPatternVerbTo.Imperative[1];
            textBoxPatternVerbToRoM2.Text=CurrentPatternVerbTo.Imperative[2];
         //   tableLayoutPanelPatternVerbToImperative.Visible=true;
            checkBoxPatternVerbToImperative.Visible=true;
            labelPatternVerbToImperativeMultiple.Visible=true;
          //  labelPatternVerbToImperativePeople.Visible=true;
            labelPatternVerbToImperativeSingle.Visible=true;

          //  if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.Trpne || CurrentPatternVerbTo.TypeShow==VerbTypeShow.TrpneCinne || CurrentPatternVerbTo.TypeShow==VerbTypeShow.All || CurrentPatternVerbTo.TypeShow==VerbTypeShow.Unknown) {
                textBoxPatternVerbToMtMzJ.Text=CurrentPatternVerbTo.PastPassive[0];
                textBoxPatternVerbToMtMnJ.Text=CurrentPatternVerbTo.PastPassive[1];
                textBoxPatternVerbToMtZeJ.Text=CurrentPatternVerbTo.PastPassive[2];
                textBoxPatternVerbToMtStJ.Text=CurrentPatternVerbTo.PastPassive[3];
                textBoxPatternVerbToMtMzM.Text=CurrentPatternVerbTo.PastPassive[4];
                textBoxPatternVerbToMtMnM.Text=CurrentPatternVerbTo.PastPassive[5];
                textBoxPatternVerbToMtZeM.Text=CurrentPatternVerbTo.PastPassive[6];
                textBoxPatternVerbToMtStM.Text=CurrentPatternVerbTo.PastPassive[7];

               // tableLayoutPanelPatternVerbToPastPassive.Visible=true;
                //labelPatternVerbToPastInactive.Visible=true;
                labelPatternVerbToPastInactiveMultiple.Visible=true;
                labelPatternVerbToPastInactivePeople.Visible=true;
                labelPatternVerbToPastInactiveSingle.Visible=true;
            //} else {
            //    tableLayoutPanelPatternVerbToPastInactive.Visible=false;
            //    labelPatternVerbToPastInactive.Visible=false;
            //    labelPatternVerbToPastInactiveMultiple.Visible=false;
            //    labelPatternVerbToPastInactivePeople.Visible=false;
            //    labelPatternVerbToPastInactiveSingle.Visible=false;
            //}

            //if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.Cinne || CurrentPatternVerbTo.TypeShow==VerbTypeShow.TrpneCinne || CurrentPatternVerbTo.TypeShow==VerbTypeShow.All || CurrentPatternVerbTo.TypeShow==VerbTypeShow.Unknown) {
                textBoxPatternVerbToMcMzJ.Text=CurrentPatternVerbTo.PastActive[0];
                textBoxPatternVerbToMcMnJ.Text=CurrentPatternVerbTo.PastActive[1];
                textBoxPatternVerbToMcZeJ.Text=CurrentPatternVerbTo.PastActive[2];
                textBoxPatternVerbToMcStJ.Text=CurrentPatternVerbTo.PastActive[3];
                textBoxPatternVerbToMcMzM.Text=CurrentPatternVerbTo.PastActive[4];
                textBoxPatternVerbToMcMnM.Text=CurrentPatternVerbTo.PastActive[5];
                textBoxPatternVerbToMcZeM.Text=CurrentPatternVerbTo.PastActive[6];
                textBoxPatternVerbToMcStM.Text=CurrentPatternVerbTo.PastActive[7];

             //   tableLayoutPanelPatternVerbToPastActive.Visible=true;
                //labelPatternVerbToPastActive.Visible=true;
                labelPatternVerbToPastActiveMultiple.Visible=true;
                labelPatternVerbToPastActivePeople.Visible=true;
                labelPatternVerbToPastActiveSingle.Visible=true;
            //}else{
            //     tableLayoutPanelPatternVerbToPastActive.Visible=false;
            //    labelPatternVerbToPastActive.Visible=false;
            //    labelPatternVerbToPastActiveMultiple.Visible=false;
            //    labelPatternVerbToPastActivePeople.Visible=false;
            //    labelPatternVerbToPastActiveSingle.Visible=false;
            //}

            textBoxPatternVerbToTr1.Text=CurrentPatternVerbTo.TransgressiveCont[0];
            textBoxPatternVerbToTr2.Text=CurrentPatternVerbTo.TransgressiveCont[1];
            textBoxPatternVerbToTr3.Text=CurrentPatternVerbTo.TransgressiveCont[2];
            textBoxPatternVerbToTr4.Text=CurrentPatternVerbTo.TransgressivePast[0];
            textBoxPatternVerbToTr5.Text=CurrentPatternVerbTo.TransgressivePast[1];
            textBoxPatternVerbToTr6.Text=CurrentPatternVerbTo.TransgressivePast[2];

         //   tableLayoutPanelPatternVerbToTransgressive.Visible=true;
            labelPatternVerbToTransgressive.Visible=true;
          //  checkBoxPatternVerbToTransgressiveCont.Visible=true;
           // checkBoxPatternVerbToTransgressivePast.Visible=true;

            textBoxPatternVerbToTr1.Text=CurrentPatternVerbTo.TransgressiveCont[0];
            textBoxPatternVerbToTr2.Text=CurrentPatternVerbTo.TransgressiveCont[1];
            textBoxPatternVerbToTr3.Text=CurrentPatternVerbTo.TransgressiveCont[2];
            textBoxPatternVerbToTr4.Text=CurrentPatternVerbTo.TransgressivePast[0];
            textBoxPatternVerbToTr5.Text=CurrentPatternVerbTo.TransgressivePast[1];
            textBoxPatternVerbToTr6.Text=CurrentPatternVerbTo.TransgressivePast[2];


            textBoxPatternVerbToName.Visible=true;
            labelPatternVerbToName.Visible=true;
            labelPatternVerbToType.Visible=true;

            comboBoxPatternVerbToType.Visible=true;





            textBoxPatternVerbToInfinitive.Visible=true;
            labelPatternVerbToInfinitive.Visible=true;
        }

        void PatternVerbToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternVerbsToFiltered.Count; indexCur++) {
                if (itemsPatternVerbTo[indexCur]==CurrentPatternVerbTo) {
                    int indexList=PatternVerbToListBox.SelectedIndex;
                    if (indexList==indexCur) return;
                    PatternVerbToListBox.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void PatternVerbToSaveCurrent() {
            if (CurrentPatternVerbTo==null) return;

            CurrentPatternVerbTo.Name=textBoxPatternVerbToName.Text;
            CurrentPatternVerbTo.Type=(VerbType)comboBoxPatternVerbToType.SelectedIndex;
            PatternVerbToCheckBoxesSave();
           // CurrentPatternVerbTo.TypeShow=(VerbTypeShow)comboBoxPatternVerbToShowType.SelectedIndex;
            CurrentPatternVerbTo.Infinitive=textBoxPatternVerbToInfinitive.Text;

            CurrentPatternVerbTo.Continous[0]=textBoxPatternVerbToPrJ1.Text;
            CurrentPatternVerbTo.Continous[1]=textBoxPatternVerbToPrJ2.Text;
            CurrentPatternVerbTo.Continous[2]=textBoxPatternVerbToPrJ3.Text;
            CurrentPatternVerbTo.Continous[3]=textBoxPatternVerbToPrM1.Text;
            CurrentPatternVerbTo.Continous[4]=textBoxPatternVerbToPrM2.Text;
            CurrentPatternVerbTo.Continous[5]=textBoxPatternVerbToPrM3.Text;

            CurrentPatternVerbTo.Future[0]=textBoxPatternVerbToBuJ1.Text;
            CurrentPatternVerbTo.Future[1]=textBoxPatternVerbToBuJ2.Text;
            CurrentPatternVerbTo.Future[2]=textBoxPatternVerbToBuJ3.Text;
            CurrentPatternVerbTo.Future[3]=textBoxPatternVerbToBuM1.Text;
            CurrentPatternVerbTo.Future[4]=textBoxPatternVerbToBuM2.Text;
            CurrentPatternVerbTo.Future[5]=textBoxPatternVerbToBuM3.Text;

            CurrentPatternVerbTo.Imperative[0]=textBoxPatternVerbToRoJ2.Text;
            CurrentPatternVerbTo.Imperative[1]=textBoxPatternVerbToRoM1.Text;
            CurrentPatternVerbTo.Imperative[2]=textBoxPatternVerbToRoM2.Text;

            CurrentPatternVerbTo.PastPassive[0]=textBoxPatternVerbToMtMzJ.Text;
            CurrentPatternVerbTo.PastPassive[1]=textBoxPatternVerbToMtMnJ.Text;
            CurrentPatternVerbTo.PastPassive[2]=textBoxPatternVerbToMtZeJ.Text;
            CurrentPatternVerbTo.PastPassive[3]=textBoxPatternVerbToMtStJ.Text;
            CurrentPatternVerbTo.PastPassive[4]=textBoxPatternVerbToMtMzM.Text;
            CurrentPatternVerbTo.PastPassive[5]=textBoxPatternVerbToMtMnM.Text;
            CurrentPatternVerbTo.PastPassive[6]=textBoxPatternVerbToMtZeM.Text;
            CurrentPatternVerbTo.PastPassive[7]=textBoxPatternVerbToMtStM.Text;

            CurrentPatternVerbTo.PastActive[0]=textBoxPatternVerbToMcMzJ.Text;
            CurrentPatternVerbTo.PastActive[1]=textBoxPatternVerbToMcMnJ.Text;
            CurrentPatternVerbTo.PastActive[2]=textBoxPatternVerbToMcZeJ.Text;
            CurrentPatternVerbTo.PastActive[3]=textBoxPatternVerbToMcStJ.Text;
            CurrentPatternVerbTo.PastActive[4]=textBoxPatternVerbToMcMzM.Text;
            CurrentPatternVerbTo.PastActive[5]=textBoxPatternVerbToMcMnM.Text;
            CurrentPatternVerbTo.PastActive[6]=textBoxPatternVerbToMcZeM.Text;
            CurrentPatternVerbTo.PastActive[7]=textBoxPatternVerbToMcStM.Text;

            CurrentPatternVerbTo.TransgressiveCont[0]=textBoxPatternVerbToTr1.Text;
            CurrentPatternVerbTo.TransgressiveCont[1]=textBoxPatternVerbToTr2.Text;
            CurrentPatternVerbTo.TransgressiveCont[2]=textBoxPatternVerbToTr3.Text;
            CurrentPatternVerbTo.TransgressivePast[0]=textBoxPatternVerbToTr4.Text;
            CurrentPatternVerbTo.TransgressivePast[1]=textBoxPatternVerbToTr5.Text;
            CurrentPatternVerbTo.TransgressivePast[2]=textBoxPatternVerbToTr6.Text;

            CurrentPatternVerbTo.Auxiliary[0]=textBoxPatternVerbToAux1.Text;
            CurrentPatternVerbTo.Auxiliary[1]=textBoxPatternVerbToAux2.Text;
            CurrentPatternVerbTo.Auxiliary[2]=textBoxPatternVerbToAux3.Text;
            CurrentPatternVerbTo.Auxiliary[3]=textBoxPatternVerbToAux4.Text;
            CurrentPatternVerbTo.Auxiliary[4]=textBoxPatternVerbToAux5.Text;
            CurrentPatternVerbTo.Auxiliary[5]=textBoxPatternVerbToAux6.Text;
        }

        void PatternVerbToSetNone(){
            textBoxPatternVerbToName.Text="";

            textBoxPatternVerbToPrJ1.Text="";
            textBoxPatternVerbToPrJ2.Text="";
            textBoxPatternVerbToPrJ3.Text="";
            textBoxPatternVerbToPrM1.Text="";
            textBoxPatternVerbToPrM2.Text="";
            textBoxPatternVerbToPrM3.Text="";

            textBoxPatternVerbToBuJ1.Text="";
            textBoxPatternVerbToBuJ2.Text="";
            textBoxPatternVerbToBuJ3.Text="";
            textBoxPatternVerbToBuM1.Text="";
            textBoxPatternVerbToBuM2.Text="";
            textBoxPatternVerbToBuM3.Text="";

            textBoxPatternVerbToRoJ2.Text="";
            textBoxPatternVerbToRoM1.Text="";
            textBoxPatternVerbToRoM2.Text="";

            textBoxPatternVerbToMtMzJ.Text="";
            textBoxPatternVerbToMtMnJ.Text="";
            textBoxPatternVerbToMtZeJ.Text="";
            textBoxPatternVerbToMtStJ.Text="";
            textBoxPatternVerbToMtMzM.Text="";
            textBoxPatternVerbToMtMnM.Text="";
            textBoxPatternVerbToMtZeM.Text="";
            textBoxPatternVerbToMtStM.Text="";

            textBoxPatternVerbToMcMzJ.Text="";
            textBoxPatternVerbToMcMnJ.Text="";
            textBoxPatternVerbToMcZeJ.Text="";
            textBoxPatternVerbToMcStJ.Text="";
            textBoxPatternVerbToMcMzM.Text="";
            textBoxPatternVerbToMcMnM.Text="";
            textBoxPatternVerbToMcZeM.Text="";
            textBoxPatternVerbToMcStM.Text="";


            textBoxPatternVerbToTr1.Text="";
            textBoxPatternVerbToTr2.Text="";
            textBoxPatternVerbToTr3.Text="";
            textBoxPatternVerbToTr4.Text="";
            textBoxPatternVerbToTr5.Text="";
            textBoxPatternVerbToTr6.Text="";

            labelPatternVerbToName.Visible=false;
            labelPatternVerbToType.Visible=false;
            tableLayoutPanelPatternVerbToContinous.Visible=false;
            checkBoxPatternVerbToContinous.Visible=false;
            labelPatternVerbToContinousMultiple.Visible=false;
            labelPatternVerbToContinousSingle.Visible=false;
            checkBoxPatternVerbToTransgressiveCont.Visible=false;
            tableLayoutPanelPatternVerbToFuture.Visible=false;
            checkBoxPatternVerbToFuture.Visible=false;
            labelPatternVerbToFutureMultiple.Visible=false;
         //   checkBoxPatternVerbToFuturePeople.Visible=false;
            labelPatternVerbToFutureSingle.Visible=false;
            comboBoxPatternVerbToType.Visible=false;
            checkBoxPatternVerbToTransgressivePast.Visible=false;
            tableLayoutPanelPatternVerbToImperative.Visible=false;
            checkBoxPatternVerbToImperative.Visible=false;
            labelPatternVerbToImperativeMultiple.Visible=false;
         //   labelPatternVerbToImperativePeople.Visible=false;
            labelPatternVerbToImperativeSingle.Visible=false;
          //  labelPatternVerbToContinousPeople.Visible=false;
            tableLayoutPanelPatternVerbToPastPassive.Visible=false;
            //labelPatternVerbToPastInactive.Visible=false;
            labelPatternVerbToPastInactiveMultiple.Visible=false;
            labelPatternVerbToPastInactivePeople.Visible=false;
            labelPatternVerbToPastInactiveSingle.Visible=false;
            textBoxPatternVerbToName.Visible=false;
            tableLayoutPanelPatternVerbToPastActive.Visible=false;
           // labelPatternVerbToPastActive.Visible=false;
            labelPatternVerbToPastActiveMultiple.Visible=false;
            labelPatternVerbToPastActivePeople.Visible=false;
            labelPatternVerbToPastActiveSingle.Visible=false;

            tableLayoutPanelPatternVerbToTransgressive.Visible=false;
            labelPatternVerbToTransgressive.Visible=false;
        }

        void PatternVerbToClear() {
            PatternVerbToListBox.Items.Clear();
            PatternVerbToSetNone();
            itemsPatternVerbsToFiltered?.Clear();
            itemsPatternVerbTo?.Clear();
            CurrentPatternVerbTo=null;
        }
        #endregion

        #region Adverb
        void ListBoxAdverb_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentAdverb();

            int index=listBoxAdverb.SelectedIndex;
            if (itemsAdverbs.Count==0) {
                AdverbSetNone();
                return;
            }
            if (index>=itemsAdverbs.Count)    index=itemsAdverbs.Count-1;
            if (index<0) index=0;

            CurrentAdverb=itemsAdverbs[index];
            SetCurrentAdverb();
            SetListBoxAdverb();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonAdverbAdd_Click(object sender, EventArgs e) {
            AddNewItemAdverb();
        }

        void ButtonAdverbRemove_Click(object sender, EventArgs e) {
            RemoveItemAdverb(CurrentAdverb);
            TextBoxAdverbFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxAdverbFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentAdverb();

            // Získej aktuální prvek
            ItemAdverb selectedId=null;
            if (listBoxAdverb.SelectedIndex!=-1) {
                selectedId=itemsAdverbsFiltered[listBoxAdverb.SelectedIndex];
            }

            AdverbRefreshFilteredList();

            listBoxAdverb.Items.Clear();
            for (int i=0; i<itemsAdverbsFiltered.Count; i++) {
                ItemAdverb item = itemsAdverbsFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";
                listBoxAdverb.Items.Add(textToAdd);
            }

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsAdverbsFiltered.Count; i++){
                    if (itemsAdverbsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxAdverb.SelectedIndex=-1;
                    CurrentAdverb=null;
                } else listBoxAdverb.SelectedIndex=outIndex;
            } else {
                listBoxAdverb.SelectedIndex=-1;
                CurrentAdverb=null;
            }
            SetCurrentAdverb();
        }

        void RemoveCurrentAdverb(object sender, EventArgs e) {
            itemsAdverbs.Remove(CurrentAdverb);
        }

        void SetListBoxAdverb() {
           //string filter=textBoxAdverbFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxAdverb.SelectedIndex;
            listBoxAdverb.Items.Clear();
            for (int i=0; i<itemsAdverbsFiltered.Count; i++) {
                ItemAdverb item = itemsAdverbsFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxAdverb.Items.Add(textToAdd);
            }

            if (index>=listBoxAdverb.Items.Count)index=listBoxAdverb.Items.Count-1;
            listBoxAdverb.SelectedIndex=index;
        }

        void AdverbRefreshFilteredList() {
            if (itemsAdverbsFiltered==null) itemsAdverbsFiltered=new List<ItemAdverb>();
            itemsAdverbsFiltered.Clear();
            string filter=textBoxAdverbFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsAdverbs.Count; i++) {
                    ItemAdverb item = itemsAdverbs[i];

                    if (item.Filter(filter)) {
                        itemsAdverbsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsAdverbs.Count; i++) {
                    ItemAdverb item = itemsAdverbs[i];
                    itemsAdverbsFiltered.Add(item);
                }
            }
        }

        void AddNewItemAdverb() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentAdverb();

            var newItem=new ItemAdverb();
            newItem.To=new List<TranslatingToData>{ new TranslatingToData()};
            // newItem.ID=itemsAdverbs.Count;
            itemsAdverbs.Add(newItem);
            CurrentAdverb=newItem;
            AdverbRefreshFilteredList();
            SetListBoxAdverb();
            ListBoxSetCurrentAdverb();
            SetCurrentAdverb();

            doingJob=false;
        }

        void RemoveItemAdverb(ItemAdverb item) {
            Edited=true;
            ChangeCaptionText();
            itemsAdverbs.Remove(item);
            AdverbRefreshFilteredList();
            SetListBoxAdverb();
            SetCurrentAdverb();
        }

        void SetCurrentAdverb(){
            if (itemsAdverbsFiltered.Count==0) {
                AdverbSetNone();
                return;
            }

            int index=listBoxAdverb.SelectedIndex;
            if (index>=itemsAdverbsFiltered.Count) index=itemsAdverbsFiltered.Count-1;
            if (index<0) index=0;
            CurrentAdverb=itemsAdverbsFiltered[index];

            textBoxAdverbFrom.Visible=true;
            //textBoxAdverbTo.Visible=true;
            labelAdverbFrom.Visible=true;
           // labelAdverbTo.Visible=true;
            //buttonAddToAdverb.Visible=true;
            textBoxAdverbFrom.Text= CurrentAdverb.From;
           // textBoxAdverbTo.Text= CurrentAdverb.To;


            simpleUIAdverb.Visible=true;
            simpleUIAdverb.SetData(CurrentAdverb.To.ToArray());
        }

        void ListBoxSetCurrentAdverb() {
            for (int indexCur=0; indexCur<itemsAdverbsFiltered.Count; indexCur++) {
                if (itemsAdverbs[indexCur]==CurrentAdverb) {
                    int indexList=listBoxAdverb.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxAdverb.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentAdverb() {
            if (CurrentAdverb==null) return;

            CurrentAdverb.From=textBoxAdverbFrom.Text;
           // CurrentAdverb.To=textBoxAdverbTo.Text;


            CurrentAdverb.To=simpleUIAdverb.GetData().ToList();
        }

        void AdverbSetNone(){
            textBoxAdverbFrom.Text="";
           // textBoxAdverbTo.Text="";
            textBoxAdverbFrom.Visible=false;
           // textBoxAdverbTo.Visible=false;
            labelAdverbFrom.Visible=false;
           // labelAdverbTo.Visible=false;

            //textBoxAdverbComment.Text="";
            //textBoxAdverbComment.Visible=false;
            //labelAdverbComment.Visible=false;
            //buttonAddToAdverb.Visible=false;
            simpleUIAdverb.Visible=false;
            simpleUIAdverb.Clear();
        }

        void ClearAdverb(){
            listBoxAdverb.Items.Clear();
            AdverbSetNone();
            itemsAdverbsFiltered?.Clear();
            itemsAdverbs?.Clear();
            CurrentAdverb=null;
        }
        #endregion

        #region Preposition
        void ListBoxPrepositions_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentPreposition();

            int index=listBoxPreposition.SelectedIndex;
            if (itemsPrepositions.Count==0) {
                PrepositionSetNone();
                return;
            }
            if (index>=itemsPrepositions.Count)
                index=itemsPrepositions.Count-1;
            if (index<0)
                index=0;

            CurrentPreposition=itemsPrepositions[index];
            SetCurrentPreposition();
            SetListBoxPreposition();
            doingJob=false;
        }

        void ButtonPrepositionAdd_Click(object sender, EventArgs e) {
            AddNewItemPreposition();
        }

        void ButtonPrepositionRemove_Click(object sender, EventArgs e) {
            RemoveItemPreposition(CurrentPreposition);
            TextBoxPrepositionFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxPrepositionFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentPreposition();

            // Získej aktuální prvek
            ItemPreposition selectedId=null;
            if (listBoxPreposition.SelectedIndex!=-1) {
                selectedId=itemsPrepositionsFiltered[listBoxPreposition.SelectedIndex];
            }

            PrepositionRefreshFilteredList();

            listBoxPreposition.Items.Clear();
            for (int i=0; i<itemsPrepositionsFiltered.Count; i++) {
                ItemPreposition item = itemsPrepositionsFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPreposition.Items.Add(textToAdd);
            }

            //PronounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsPrepositionsFiltered.Count; i++){
                    if (itemsPrepositionsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxPreposition.SelectedIndex=-1;
                    CurrentPreposition=null;
                } else listBoxPreposition.SelectedIndex=outIndex;
            } else {
                listBoxPreposition.SelectedIndex=-1;
                CurrentPreposition=null;
            }
            SetCurrentPreposition();
        }

        void RemoveCurrentPreposition(object sender, EventArgs e) {
            itemsPrepositions.Remove(CurrentPreposition);
        }

        void SetListBoxPreposition() {
       //     string filter=textBoxPrepositionFilter.Text;
          //  bool useFilter = filter!="" && filter!="*";

            int index=listBoxPreposition.SelectedIndex;
            listBoxPreposition.Items.Clear();
            for (int i=0; i<itemsPrepositionsFiltered.Count; i++) {
                ItemPreposition item = itemsPrepositionsFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPreposition.Items.Add(textToAdd);
            }

            if (index>=listBoxPreposition.Items.Count)index=listBoxPreposition.Items.Count-1;
            listBoxPreposition.SelectedIndex=index;
        }

        void PrepositionRefreshFilteredList() {
            if (itemsPrepositionsFiltered==null) itemsPrepositionsFiltered=new List<ItemPreposition>();
            itemsPrepositionsFiltered.Clear();
            string filter=textBoxPrepositionFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsPrepositions.Count; i++) {
                    ItemPreposition item = itemsPrepositions[i];

                    if (item.Filter(filter)) {
                        itemsPrepositionsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsPrepositions.Count; i++) {
                    ItemPreposition item = itemsPrepositions[i];
                    itemsPrepositionsFiltered.Add(item);
                }
            }
        }

        void AddNewItemPreposition() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentPreposition();

            var newItem=new ItemPreposition();
            newItem.To=new List<TranslatingToData>{ new TranslatingToData()};
           // newItem.ID=itemsPronouns.Count;
            itemsPrepositions.Add(newItem);
            CurrentPreposition=newItem;
            PrepositionRefreshFilteredList();
            SetListBoxPreposition();
            ListBoxSetCurrentPreposition();
            SetCurrentPreposition();

            doingJob=false;
        }

        void RemoveItemPreposition(ItemPreposition item) {
            Edited=true;
            ChangeCaptionText();
            itemsPrepositions.Remove(item);
            PrepositionRefreshFilteredList();
            SetListBoxPreposition();
            SetCurrentPreposition();
        }

        void SetCurrentPreposition(){
            if (itemsPrepositionsFiltered.Count==0) {
                PrepositionSetNone();
                return;
            }

            int index=listBoxPreposition.SelectedIndex;
            if (index>=itemsPrepositionsFiltered.Count) index=itemsPrepositionsFiltered.Count-1;
            if (index<0) index=0;
            CurrentPreposition=itemsPrepositionsFiltered[index];

            textBoxPrepositionFrom.Text=CurrentPreposition.From;
          //  textBoxPrepositionTo.Text=CurrentPreposition.To;
            textBoxPrepositionFall.Text=CurrentPreposition.Fall;

            textBoxPrepositionFrom.Visible=true;
           // textBoxPrepositionTo.Visible=true;
            textBoxPrepositionFall.Visible=true;

            labelPrepositionFrom.Visible=true;
          //  labelPrepositionTo.Visible=true;
            labelPrepositionFall.Visible=true;

            simpleUIPreposition.Visible=true;
            simpleUIPreposition.SetData(CurrentPreposition.To.ToArray());

          //  ChangeTypePreposition(CurrentPreposition?.Type);
        }

        void ListBoxSetCurrentPreposition() {
            for (int indexCur=0; indexCur<itemsPrepositionsFiltered.Count; indexCur++) {
                if (itemsPrepositions[indexCur]==CurrentPreposition) {
                    int indexList=listBoxPreposition.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPreposition.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentPreposition() {
            if (CurrentPreposition==null) return;

            CurrentPreposition.From =textBoxPrepositionFrom.Text;
          //  CurrentPreposition.To   =textBoxPrepositionTo.Text;
            CurrentPreposition.Fall =textBoxPrepositionFall.Text;

            CurrentPreposition.To=simpleUIPreposition.GetData().ToList();
        }

        void PrepositionSetNone(){
            textBoxPrepositionFrom.Text="";
          //  textBoxPrepositionTo.Text="";
            textBoxPrepositionFall.Text="";

            textBoxPrepositionFrom.Visible=false;
          //  textBoxPrepositionTo.Visible=false;
            textBoxPrepositionFall.Visible=false;

            labelPrepositionFrom.Visible=false;
          //  labelPrepositionTo.Visible=false;
            labelPrepositionFall.Visible=false;


            //textBoxAdverbComment.Text="";
            //textBoxAdverbComment.Visible=false;
            //labelAdverbComment.Visible=false;

            simpleUIPreposition.Visible=false;
            simpleUIPreposition.Clear();
        }

        //void ClearPreposition() {
        //    listBoxPreposition.Items.Clear();
        //    PrepositionSetNone();
        //    itemsPrepositionsFiltered?.Clear();
        //    itemsPrepositions?.Clear();
        //    CurrentPreposition=null;
        //}
        #endregion

        #region Conjunction
        void ListBoxConjunction_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentConjunction();

            int index=listBoxConjunction.SelectedIndex;
            if (itemsConjunctions.Count==0) {
                ConjunctionSetNone();
                return;
            }
            if (index>=itemsConjunctions.Count)    index=itemsConjunctions.Count-1;
            if (index<0) index=0;

            CurrentConjunction=itemsConjunctions[index];
            SetCurrentConjunction();
            SetListBoxConjunction();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonConjunctionAdd_Click(object sender, EventArgs e) {
            AddNewItemConjunction();
        }

        void ButtonConjunctionRemove_Click(object sender, EventArgs e) {
            RemoveItemConjunction(CurrentConjunction);
            TextBoxConjunctionFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxConjunctionFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentConjunction();

            // Získej aktuální prvek
            ItemConjunction selectedId=null;
            if (listBoxConjunction.SelectedIndex!=-1) {
                selectedId=itemsConjunctionsFiltered[listBoxConjunction.SelectedIndex];
            }

            ConjunctionRefreshFilteredList();

            listBoxConjunction.Items.Clear();
            for (int i=0; i<itemsConjunctionsFiltered.Count; i++) {
                ItemConjunction item = itemsConjunctionsFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxConjunction.Items.Add(textToAdd);
            }

            //SetListBoxConjunction();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsConjunctionsFiltered.Count; i++){
                    if (itemsConjunctionsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxConjunction.SelectedIndex=-1;
                    CurrentConjunction=null;
                } else listBoxConjunction.SelectedIndex=outIndex;
            } else {
                listBoxConjunction.SelectedIndex=-1;
                CurrentConjunction=null;
            }
            SetCurrentConjunction();
        }

        void RemoveCurrentConjunction(object sender, EventArgs e) {
            itemsConjunctions.Remove(CurrentConjunction);
        }

        void SetListBoxConjunction() {
            //string filter=textBoxConjunctionFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxConjunction.SelectedIndex;
            listBoxConjunction.Items.Clear();
            for (int i=0; i<itemsConjunctionsFiltered.Count; i++) {
                ItemConjunction item = itemsConjunctionsFiltered[i];

                string textToAdd=item.GetText();
                //if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxConjunction.Items.Add(textToAdd);
            }

            if (index>=listBoxConjunction.Items.Count)index=listBoxConjunction.Items.Count-1;
            listBoxConjunction.SelectedIndex=index;
        }

        void ConjunctionRefreshFilteredList() {
            if (itemsConjunctionsFiltered==null) itemsConjunctionsFiltered=new List<ItemConjunction>();
            itemsConjunctionsFiltered.Clear();
            string filter=textBoxConjunctionFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsConjunctions.Count; i++) {
                    ItemConjunction item = itemsConjunctions[i];

                    if (item.Filter(filter)) {
                        itemsConjunctionsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsConjunctions.Count; i++) {
                    ItemConjunction item = itemsConjunctions[i];
                    itemsConjunctionsFiltered.Add(item);
                }
            }
        }

        void AddNewItemConjunction() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentConjunction();

            var newItem=new ItemConjunction();
            newItem.To=new List<TranslatingToData>{ new TranslatingToData()};
            // newItem.ID=itemsConjunctions.Count;
            itemsConjunctions.Add(newItem);
            CurrentConjunction=newItem;
            ConjunctionRefreshFilteredList();
            SetListBoxConjunction();
            ListBoxSetCurrentConjunction();
            SetCurrentConjunction();

            doingJob=false;
        }

        void RemoveItemConjunction(ItemConjunction item) {
            Edited=true;
            ChangeCaptionText();
            itemsConjunctions.Remove(item);
            ConjunctionRefreshFilteredList();
            SetListBoxConjunction();
            SetCurrentConjunction();
        }

        void SetCurrentConjunction(){
            if (itemsConjunctionsFiltered.Count==0) {
                ConjunctionSetNone();
                return;
            }

            int index=listBoxConjunction.SelectedIndex;
            if (index>=itemsConjunctionsFiltered.Count) index=itemsConjunctionsFiltered.Count-1;
            if (index<0) index=0;
            CurrentConjunction=itemsConjunctionsFiltered[index];

            textBoxConjunctionFrom.Visible=true;
            //textBoxConjunctionTo.Visible=true;
            labelConjunctionFrom.Visible=true;
           // labelConjunctionTo.Visible=true;

           textBoxConjunctionFrom.Text= CurrentConjunction.From;
          // textBoxConjunctionTo.Text= CurrentConjunction.To;
          //buttonAddToConjuction.Visible=true;
            simpleUIConjuction.Visible=true;
            simpleUIConjuction.SetData(CurrentConjunction.To.ToArray());

        }

        void ListBoxSetCurrentConjunction() {
            for (int indexCur=0; indexCur<itemsConjunctionsFiltered.Count; indexCur++) {
                if (itemsConjunctions[indexCur]==CurrentConjunction) {
                    int indexList=listBoxConjunction.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxConjunction.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        //int GetListBoxSelectedIndexConjunction() {
        //    if (listBoxConjunctions.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterConjunction.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) {
        //        var item=itemsConjunctionsFiltered[listBoxConjunctions.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsConjunctions.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else {
        //        return listBoxConjunctions.SelectedIndex;
        //    }

        //    return -1;
        //}

        void SaveCurrentConjunction() {
            if (CurrentConjunction==null) return;

            CurrentConjunction.From=textBoxConjunctionFrom.Text;
          //  CurrentConjunction.To=textBoxConjunctionTo.Text;


            CurrentConjunction.To=simpleUIConjuction.GetData().ToList();
        }

        void ConjunctionSetNone(){
            textBoxConjunctionFrom.Text="";
            //textBoxConjunctionTo.Text="";
            textBoxConjunctionFrom.Visible=false;
            //textBoxConjunctionTo.Visible=false;
            labelConjunctionFrom.Visible=false;
            //labelConjunctionTo.Visible=false;

           // textBoxConjuctionComment.Text="";
           // textBoxConjuctionComment.Visible=false;
          //  labelConjuctionComment.Visible=false;
            //buttonAddToConjuction.Visible=false;

            simpleUIConjuction.Visible=false;
            simpleUIConjuction.Clear();
        }

        void ClearConjunction(){
            listBoxConjunction.Items.Clear();
            ConjunctionSetNone();
            itemsConjunctionsFiltered?.Clear();
            itemsConjunctions?.Clear();
            CurrentConjunction=null;
        }
        #endregion

        #region Particle
        void listBoxParticle_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentParticle();

            int index=listBoxParticle.SelectedIndex;
            if (itemsParticles.Count==0) {
                ParticleSetNone();
                return;
            }
            if (index>=itemsParticles.Count)    index=itemsParticles.Count-1;
            if (index<0) index=0;

            CurrentParticle=itemsParticles[index];
            SetCurrentParticle();
            SetListBoxParticle();
          //  SetCurrent();
            doingJob=false;
        }

        void buttonParticleAdd_Click(object sender, EventArgs e) {
            AddNewItemParticle();
        }

        void buttonParticleRemove_Click(object sender, EventArgs e) {
            RemoveItemParticle(CurrentParticle);
            TextBoxParticleFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxParticleFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentParticle();

            // Získej aktuální prvek
            ItemParticle selectedId=null;
            if (listBoxParticle.SelectedIndex!=-1) {
                selectedId=itemsParticlesFiltered[listBoxParticle.SelectedIndex];
            }

            ParticleRefreshFilteredList();

            listBoxParticle.Items.Clear();
            for (int i=0; i<itemsParticlesFiltered.Count; i++) {
                ItemParticle item = itemsParticlesFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxParticle.Items.Add(textToAdd);
            }

            //SetListBoxParticle();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsParticlesFiltered.Count; i++){
                    if (itemsParticlesFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxParticle.SelectedIndex=-1;
                    CurrentParticle=null;
                } else listBoxParticle.SelectedIndex=outIndex;
            } else {
                listBoxParticle.SelectedIndex=-1;
                CurrentParticle=null;
            }
            SetCurrentParticle();
        }

        void RemoveCurrentParticle(object sender, EventArgs e) {
            itemsParticles.Remove(CurrentParticle);
        }

        void SetListBoxParticle() {
            //string filter=textBoxParticleFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxParticle.SelectedIndex;
            listBoxParticle.Items.Clear();
            for (int i=0; i<itemsParticlesFiltered.Count; i++) {
                ItemParticle item = itemsParticlesFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxParticle.Items.Add(textToAdd);
            }

            if (index>=listBoxParticle.Items.Count)index=listBoxParticle.Items.Count-1;
            listBoxParticle.SelectedIndex=index;
        }

        void ParticleRefreshFilteredList() {
            if (itemsParticlesFiltered==null) itemsParticlesFiltered=new List<ItemParticle>();
            itemsParticlesFiltered.Clear();
            string filter=textBoxParticleFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsParticles.Count; i++) {
                    ItemParticle item = itemsParticles[i];

                    if (item.Filter(filter)) {
                        itemsParticlesFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsParticles.Count; i++) {
                    ItemParticle item = itemsParticles[i];
                    itemsParticlesFiltered.Add(item);
                }
            }
        }

        void AddNewItemParticle() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentParticle();

            var newItem=new ItemParticle();
            newItem.To=new List<TranslatingToData>{ new TranslatingToData()};
            // newItem.ID=itemsParticles.Count;
            itemsParticles.Add(newItem);
            CurrentParticle=newItem;
            ParticleRefreshFilteredList();
            SetListBoxParticle();
            ListBoxSetCurrentParticle();
            SetCurrentParticle();

            doingJob=false;
        }

        void RemoveItemParticle(ItemParticle item) {
            Edited=true;
            ChangeCaptionText();
            itemsParticles.Remove(item);
            ParticleRefreshFilteredList();
            SetListBoxParticle();
            SetCurrentParticle();
        }

        void SetCurrentParticle(){
            if (itemsParticlesFiltered.Count==0) {
                ParticleSetNone();
                return;
            }

            int index=listBoxParticle.SelectedIndex;
            if (index>=itemsParticlesFiltered.Count) index=itemsParticlesFiltered.Count-1;
            if (index<0) index=0;
            CurrentParticle=itemsParticlesFiltered[index];

            textBoxParticleFrom.Visible=true;
            //textBoxParticleTo.Visible=true;
            labelParticleFrom.Visible=true;
           // labelParticleTo.Visible=true;

           textBoxParticleFrom.Text= CurrentParticle.From;
           //textBoxParticleTo.Text= CurrentParticle.To;
            //buttonAddToParticle.Visible=true;
            simpleUIParticle.Visible=true;
            simpleUIParticle.SetData(CurrentParticle.To.ToArray());
        }

        void ListBoxSetCurrentParticle() {
            for (int indexCur=0; indexCur<itemsParticlesFiltered.Count; indexCur++) {
                if (itemsParticles[indexCur]==CurrentParticle) {
                    int indexList=listBoxParticle.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxParticle.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        //int GetListBoxSelectedIndexParticle() {
        //    if (listBoxParticles.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterParticle.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) {
        //        var item=itemsParticlesFiltered[listBoxParticles.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsParticles.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else {
        //        return listBoxParticles.SelectedIndex;
        //    }

        //    return -1;
        //}

        void SaveCurrentParticle() {
            if (CurrentParticle==null) return;

            CurrentParticle.From=textBoxParticleFrom.Text;
            //CurrentParticle.To=textBoxParticleTo.Text;


            CurrentParticle.To=simpleUIParticle.GetData().ToList();
        }

        void ParticleSetNone(){
            textBoxParticleFrom.Text="";
           // textBoxParticleTo.Text="";
            textBoxParticleFrom.Visible=false;
           // textBoxParticleTo.Visible=false;
            labelParticleFrom.Visible=false;
            //labelParticleTo.Visible=false;

            //buttonAddToParticle.Visible=false;
            simpleUIParticle.Visible=false;
            simpleUIParticle.Clear();
        }

        void ClearParticle(){
            listBoxParticle.Items.Clear();
            ParticleSetNone();
            itemsParticlesFiltered?.Clear();
            itemsParticles?.Clear();
            CurrentParticle=null;
        }
        #endregion

        #region Interjection
        void listBoxInterjection_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentInterjection();

            int index=listBoxInterjection.SelectedIndex;
            if (itemsInterjections.Count==0) {
                InterjectionSetNone();
                return;
            }
            if (index>=itemsInterjections.Count)    index=itemsInterjections.Count-1;
            if (index<0) index=0;

            CurrentInterjection=itemsInterjections[index];
            SetCurrentInterjection();
            SetListBoxInterjection();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonInterjectionAdd_Click(object sender, EventArgs e) {
            AddNewItemInterjection();
        }

        void ButtonInterjectionRemove_Click(object sender, EventArgs e) {
            RemoveItemInterjection(CurrentInterjection);
            TextBoxInterjectionFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxInterjectionFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentInterjection();

            // Získej aktuální prvek
            ItemInterjection selectedId=null;
            if (listBoxInterjection.SelectedIndex!=-1) {
                selectedId=itemsInterjectionsFiltered[listBoxInterjection.SelectedIndex];
            }

            InterjectionRefreshFilteredList();

            listBoxInterjection.Items.Clear();
            for (int i=0; i<itemsInterjectionsFiltered.Count; i++) {
                ItemInterjection item = itemsInterjectionsFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxInterjection.Items.Add(textToAdd);
            }

            //SetListBoxInterjection();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsInterjectionsFiltered.Count; i++){
                    if (itemsInterjectionsFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxInterjection.SelectedIndex=-1;
                    CurrentInterjection=null;
                } else listBoxInterjection.SelectedIndex=outIndex;
            } else {
                listBoxInterjection.SelectedIndex=-1;
                CurrentInterjection=null;
            }
            SetCurrentInterjection();
        }

        void RemoveCurrentInterjection(object sender, EventArgs e) {
            itemsInterjections.Remove(CurrentInterjection);
        }

        void SetListBoxInterjection() {
            //string filter=textBoxInterjectionFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxInterjection.SelectedIndex;
            listBoxInterjection.Items.Clear();
            for (int i=0; i<itemsInterjectionsFiltered.Count; i++) {
                ItemInterjection item = itemsInterjectionsFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxInterjection.Items.Add(textToAdd);
            }

            if (index>=listBoxInterjection.Items.Count)index=listBoxInterjection.Items.Count-1;
            listBoxInterjection.SelectedIndex=index;
        }

        void InterjectionRefreshFilteredList() {
            if (itemsInterjectionsFiltered==null) itemsInterjectionsFiltered=new List<ItemInterjection>();
            itemsInterjectionsFiltered.Clear();
            string filter=textBoxInterjectionFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsInterjections.Count; i++) {
                    ItemInterjection item = itemsInterjections[i];

                    if (item.Filter(filter)) {
                        itemsInterjectionsFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsInterjections.Count; i++) {
                    ItemInterjection item = itemsInterjections[i];
                    itemsInterjectionsFiltered.Add(item);
                }
            }
        }

        void AddNewItemInterjection() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentInterjection();

            var newItem=new ItemInterjection();
            newItem.To=new List<TranslatingToData>{ new TranslatingToData()};
            // newItem.ID=itemsInterjections.Count;
            itemsInterjections.Add(newItem);
            CurrentInterjection=newItem;
            InterjectionRefreshFilteredList();
            SetListBoxInterjection();
            ListBoxSetCurrentInterjection();
            SetCurrentInterjection();

            doingJob=false;
        }

        void RemoveItemInterjection(ItemInterjection item) {
            Edited=true;
            ChangeCaptionText();
            itemsInterjections.Remove(item);
            InterjectionRefreshFilteredList();
            SetListBoxInterjection();
            SetCurrentInterjection();
        }

        void SetCurrentInterjection(){
            if (itemsInterjectionsFiltered.Count==0) {
                InterjectionSetNone();
                return;
            }

            int index=listBoxInterjection.SelectedIndex;
            if (index>=itemsInterjectionsFiltered.Count) index=itemsInterjectionsFiltered.Count-1;
            if (index<0) index=0;
            CurrentInterjection=itemsInterjectionsFiltered[index];

            textBoxInterjectionFrom.Visible=true;
            //textBoxInterjectionTo.Visible=true;
            labelInterjectionFrom.Visible=true;
           // labelInterjectionTo.Visible=true;

            textBoxInterjectionFrom.Text= CurrentInterjection.From;
         //  textBoxInterjectionTo.Text= CurrentInterjection.To;

            //buttonAddToInterjection.Visible=true;

            simpleUIInterjection.Visible=true;
            simpleUIInterjection.SetData(CurrentInterjection.To.ToArray());

        }

        void ListBoxSetCurrentInterjection() {
            for (int indexCur=0; indexCur<itemsInterjectionsFiltered.Count; indexCur++) {
                if (itemsInterjections[indexCur]==CurrentInterjection) {
                    int indexList=listBoxInterjection.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxInterjection.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        //int GetListBoxSelectedIndexInterjection() {
        //    if (listBoxInterjections.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterInterjection.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) {
        //        var item=itemsInterjectionsFiltered[listBoxInterjections.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsInterjections.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else {
        //        return listBoxInterjections.SelectedIndex;
        //    }

        //    return -1;
        //}

        void SaveCurrentInterjection() {
            if (CurrentInterjection==null) return;

            CurrentInterjection.From=textBoxInterjectionFrom.Text;
            //CurrentInterjection.To=textBoxInterjectionTo.Text;

            CurrentInterjection.To=simpleUIInterjection.GetData().ToList();
        }

        void InterjectionSetNone(){
            textBoxInterjectionFrom.Text="";
           // textBoxInterjectionTo.Text="";
            textBoxInterjectionFrom.Visible=false;
           // textBoxInterjectionTo.Visible=false;
            labelInterjectionFrom.Visible=false;
         //  labelInterjectionTo.Visible=false;

            //buttonAddToInterjection.Visible=false;

            simpleUIInterjection.Visible=false;
            simpleUIInterjection.Clear();
        }

        void ClearInterjection(){
            listBoxInterjection.Items.Clear();
            InterjectionSetNone();
            itemsInterjectionsFiltered?.Clear();
            itemsInterjections?.Clear();
            CurrentInterjection=null;
        }
        #endregion

        #region ReplaceS
        void ListBoxReplaceSs_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            ReplaceSSaveCurrent();

            int index=listBoxReplaceS.SelectedIndex;
            if (itemsReplaceS.Count==0) {
                ReplaceSSetNone();
                return;
            }
            if (index>=itemsReplaceS.Count)
                index=itemsReplaceS.Count-1;
            if (index<0)
                index=0;

            CurrentReplaceS=itemsReplaceS[index];
            ReplaceSSetCurrent();
            ReplaceSSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonReplaceSAdd_Click(object sender, EventArgs e) {
            AddNewItemReplaceS();
        }

        void ButtonReplaceSRemove_Click(object sender, EventArgs e) {
            RemoveItemReplaceS(CurrentReplaceS);
            TextBoxReplaceSFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxReplaceSFilter_TextChanged(object sender, EventArgs e) {
            ReplaceSSaveCurrent();

            // Získej aktuální prvek
            ItemReplaceS selectedId=null;
            if (listBoxReplaceS.SelectedIndex!=-1) {
                selectedId=itemsReplaceSFiltered[listBoxReplaceS.SelectedIndex];
            }

            ReplaceSRefreshFilteredList();

            listBoxReplaceS.Items.Clear();
            for (int i=0; i<itemsReplaceSFiltered.Count; i++) {
                ItemReplaceS item = itemsReplaceSFiltered[i];

                string textToAdd=item.GetText();
              //  if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceS.Items.Add(textToAdd);
            }

            //PronounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsReplaceSFiltered.Count; i++){
                    if (itemsReplaceSFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxReplaceS.SelectedIndex=-1;
                    CurrentReplaceS=null;
                } else listBoxReplaceS.SelectedIndex=outIndex;
            } else {
                listBoxReplaceS.SelectedIndex=-1;
                CurrentReplaceS=null;
            }
            ReplaceSSetCurrent();
        }

        void RemoveCurrentReplaceS(object sender, EventArgs e) {
            itemsReplaceS.Remove(CurrentReplaceS);
        }

        void ReplaceSSetListBox() {
            //string filter=textBoxReplaceSFilter.Text;
           // bool useFilter = filter!="" && filter!="*";

            int index=listBoxReplaceS.SelectedIndex;
            listBoxReplaceS.Items.Clear();
            for (int i=0; i<itemsReplaceSFiltered.Count; i++) {
                ItemReplaceS item = itemsReplaceSFiltered[i];

                string textToAdd=item.GetText();
            //    if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceS.Items.Add(textToAdd);
            }

            if (index>=listBoxReplaceS.Items.Count)index=listBoxReplaceS.Items.Count-1;
            listBoxReplaceS.SelectedIndex=index;
        }

        void ReplaceSRefreshFilteredList() {
            if (itemsReplaceSFiltered==null) itemsReplaceSFiltered=new List<ItemReplaceS>();
            itemsReplaceSFiltered.Clear();
            string filter=textBoxReplaceSFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsReplaceS.Count; i++) {
                    ItemReplaceS item = itemsReplaceS[i];

                    if (item.Filter(filter)) {
                        itemsReplaceSFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsReplaceS.Count; i++) {
                    ItemReplaceS item = itemsReplaceS[i];
                    itemsReplaceSFiltered.Add(item);
                }
            }
        }

        void AddNewItemReplaceS() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            ReplaceSSaveCurrent();

            var newItem=new ItemReplaceS();
           // newItem.ID=itemsPronouns.Count;
            itemsReplaceS.Add(newItem);
            CurrentReplaceS=newItem;
            ReplaceSRefreshFilteredList();
            ReplaceSSetListBox();
            ReplaceSListBoxSetCurrent();
            ReplaceSSetCurrent();

            doingJob=false;
        }

        void RemoveItemReplaceS(ItemReplaceS item) {
            Edited=true;
            ChangeCaptionText();
            itemsReplaceS.Remove(item);
            ReplaceSRefreshFilteredList();
            ReplaceSSetListBox();
            ReplaceSSetCurrent();
        }

        void ReplaceSSetCurrent(){
            if (itemsReplaceSFiltered.Count==0) {
                ReplaceSSetNone();
                return;
            }

            int index=listBoxReplaceS.SelectedIndex;
            if (index>=itemsReplaceSFiltered.Count) index=itemsReplaceSFiltered.Count-1;
            if (index<0) index=0;
            CurrentReplaceS=itemsReplaceSFiltered[index];

            textBoxReplaceSFrom.Text=CurrentReplaceS.From;
            textBoxReplaceSTo.Text=CurrentReplaceS.To;
          //  textBoxReplaceSFall.Text=CurrentReplaceS.Fall;

            textBoxReplaceSFrom.Visible=true;
            textBoxReplaceSTo.Visible=true;
            //textBoxReplaceSFall.Visible=true;

            labelReplaceSFrom.Visible=true;
            labelReplaceSTo.Visible=true;
            //labelReplaceSFall.Visible=true;

          //  ChangeTypeReplaceS(CurrentReplaceS?.Type);
        }

        void ReplaceSListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsReplaceSFiltered.Count; indexCur++) {
                if (itemsReplaceS[indexCur]==CurrentReplaceS) {
                    int indexList=listBoxReplaceS.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxReplaceS.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void ReplaceSSaveCurrent() {
            if (CurrentReplaceS==null) return;

            CurrentReplaceS.From =textBoxReplaceSFrom.Text;
            CurrentReplaceS.To   =textBoxReplaceSTo.Text;
          //  CurrentReplaceS.Fall =textBoxReplaceSFall.Text;

        }

        void ReplaceSSetNone(){
            textBoxReplaceSFrom.Text="";
            textBoxReplaceSTo.Text="";
          //  textBoxReplaceSFall.Text="";

            textBoxReplaceSFrom.Visible=false;
            textBoxReplaceSTo.Visible=false;
          //  textBoxReplaceSFall.Visible=false;

            labelReplaceSFrom.Visible=false;
            labelReplaceSTo.Visible=false;
          //  labelReplaceSFall.Visible=false;
        }

        //void ClearReplaceS() {
        //    listBoxReplaceS.Items.Clear();
        //    ReplaceSSetNone();
        //    itemsReplaceSFiltered?.Clear();
        //    itemsReplaceS?.Clear();
        //    CurrentReplaceS=null;
        //}
        #endregion

        #region ReplaceG
        void ListBoxReplaceGs_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            ReplaceGSaveCurrent();

            int index=listBoxReplaceG.SelectedIndex;
            if (itemsReplaceG.Count==0) {
                SetNoneReplaceG();
                return;
            }
            if (index>=itemsReplaceG.Count)
                index=itemsReplaceG.Count-1;
            if (index<0)
                index=0;

            CurrentReplaceG=itemsReplaceG[index];
            SetCurrentReplaceG();
            ReplaceGSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonReplaceGAdd_Click(object sender, EventArgs e) {
            AddNewItemReplaceG();
        }

        void ButtonReplaceGRemove_Click(object sender, EventArgs e) {
            RemoveItemReplaceG(CurrentReplaceG);
            TextBoxReplaceGFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxReplaceGFilter_TextChanged(object sender, EventArgs e) {
            ReplaceGSaveCurrent();

            // Získej aktuální prvek
            ItemReplaceG selectedId=null;
            if (listBoxReplaceG.SelectedIndex!=-1) {
                selectedId=itemsReplaceGFiltered[listBoxReplaceG.SelectedIndex];
            }

            ReplaceGRefreshFilteredList();

            listBoxReplaceG.Items.Clear();
            for (int i=0; i<itemsReplaceGFiltered.Count; i++) {
                ItemReplaceG item = itemsReplaceGFiltered[i];

                string textToAdd=item.GetText();
           //     if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceG.Items.Add(textToAdd);
            }

            //PronounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsReplaceGFiltered.Count; i++){
                    if (itemsReplaceGFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxReplaceG.SelectedIndex=-1;
                    CurrentReplaceG=null;
                } else listBoxReplaceG.SelectedIndex=outIndex;
            } else {
                listBoxReplaceG.SelectedIndex=-1;
                CurrentReplaceG=null;
            }
            SetCurrentReplaceG();
        }

        void RemoveCurrentReplaceG(object sender, EventArgs e) {
            itemsReplaceG.Remove(CurrentReplaceG);
        }

        void ReplaceGSetListBox() {
            //string filter=textBoxReplaceGFilter.Text;
            //bool useFilter = filter!="" && filter!="*";

            int index=listBoxReplaceG.SelectedIndex;
            listBoxReplaceG.Items.Clear();
            for (int i=0; i<itemsReplaceGFiltered.Count; i++) {
                ItemReplaceG item = itemsReplaceGFiltered[i];

                string textToAdd=item.GetText();
               // if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceG.Items.Add(textToAdd);
            }

            if (index>=listBoxReplaceG.Items.Count)index=listBoxReplaceG.Items.Count-1;
            listBoxReplaceG.SelectedIndex=index;
        }

        void ReplaceGRefreshFilteredList() {
            if (itemsReplaceGFiltered==null) itemsReplaceGFiltered=new List<ItemReplaceG>();
            itemsReplaceGFiltered.Clear();
            string filter=textBoxReplaceGFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsReplaceG.Count; i++) {
                    ItemReplaceG item = itemsReplaceG[i];

                    if (item.Filter(filter)) {
                        itemsReplaceGFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsReplaceG.Count; i++) {
                    ItemReplaceG item = itemsReplaceG[i];
                    itemsReplaceGFiltered.Add(item);
                }
            }
        }

        void AddNewItemReplaceG() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            ReplaceGSaveCurrent();

            var newItem=new ItemReplaceG();
           // newItem.ID=itemsPronouns.Count;
            itemsReplaceG.Add(newItem);
            CurrentReplaceG=newItem;
            ReplaceGRefreshFilteredList();
            ReplaceGSetListBox();
            ListBoxSetCurrentReplaceG();
            SetCurrentReplaceG();

            doingJob=false;
        }

        void RemoveItemReplaceG(ItemReplaceG item) {
            Edited=true;
            ChangeCaptionText();
            itemsReplaceG.Remove(item);
            ReplaceGRefreshFilteredList();
            ReplaceGSetListBox();
            SetCurrentReplaceG();
        }

        void SetCurrentReplaceG(){
            if (itemsReplaceGFiltered.Count==0) {
                SetNoneReplaceG();
                return;
            }

            int index=listBoxReplaceG.SelectedIndex;
            if (index>=itemsReplaceGFiltered.Count) index=itemsReplaceGFiltered.Count-1;
            if (index<0) index=0;
            CurrentReplaceG=itemsReplaceGFiltered[index];

            textBoxReplaceGFrom.Text=CurrentReplaceG.From;
            textBoxReplaceGTo.Text=CurrentReplaceG.To;
           // textBoxReplaceGFall.Text=CurrentReplaceG.Fall;

            textBoxReplaceGFrom.Visible=true;
            textBoxReplaceGTo.Visible=true;
            //textBoxReplaceGFall.Visible=true;

            labelReplaceGFrom.Visible=true;
            labelReplaceGTo.Visible=true;
           // labelReplaceGFall.Visible=true;

          //  ChangeTypeReplaceG(CurrentReplaceG?.Type);
        }

        void ListBoxSetCurrentReplaceG() {
            for (int indexCur=0; indexCur<itemsReplaceGFiltered.Count; indexCur++) {
                if (itemsReplaceG[indexCur]==CurrentReplaceG) {
                    int indexList=listBoxReplaceG.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxReplaceG.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void ReplaceGSaveCurrent() {
            if (CurrentReplaceG==null) return;

            CurrentReplaceG.From =textBoxReplaceGFrom.Text;
            CurrentReplaceG.To   =textBoxReplaceGTo.Text;
          //  CurrentReplaceG.Fall =textBoxReplaceGFall.Text;

        }

        void SetNoneReplaceG(){
            textBoxReplaceGFrom.Text="";
            textBoxReplaceGTo.Text="";
         //   textBoxReplaceGFall.Text="";

            textBoxReplaceGFrom.Visible=false;
            textBoxReplaceGTo.Visible=false;
          //  textBoxReplaceGFall.Visible=false;

            labelReplaceGFrom.Visible=false;
            labelReplaceGTo.Visible=false;
           // labelReplaceGFall.Visible=false;
        }

        //void ClearReplaceG() {
        //    listBoxReplaceG.Items.Clear();
        //    SetNoneReplaceG();
        //    itemsReplaceGFiltered?.Clear();
        //    itemsReplaceG?.Clear();
        //    CurrentReplaceG=null;
        //}
        #endregion

        #region ReplaceE
        void ListBoxReplaceEs_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentReplaceE();

            int index=listBoxReplaceE.SelectedIndex;
            if (itemsReplaceE.Count==0) {
                SetNoneReplaceE();
                return;
            }
            if (index>=itemsReplaceE.Count)
                index=itemsReplaceE.Count-1;
            if (index<0)
                index=0;

            CurrentReplaceE=itemsReplaceE[index];
            SetCurrentReplaceE();
            ReplaceESetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonReplaceEAdd_Click(object sender, EventArgs e) {
            AddNewItemReplaceE();
        }

        void ButtonReplaceERemove_Click(object sender, EventArgs e) {
            RemoveItemReplaceE(CurrentReplaceE);
            TextBoxReplaceEFilter_TextChanged(null, new EventArgs());
        }

        void TextBoxReplaceEFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentReplaceE();

            // Získej aktuální prvek
            ItemReplaceE selectedId=null;
            if (listBoxReplaceE.SelectedIndex!=-1) {
                selectedId=itemsReplaceEFiltered[listBoxReplaceE.SelectedIndex];
            }

            ReplaceERefreshFilteredList();

            listBoxReplaceE.Items.Clear();
            for (int i=0; i<itemsReplaceEFiltered.Count; i++) {
                ItemReplaceE item = itemsReplaceEFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceE.Items.Add(textToAdd);
            }

            //PronounSetListBox();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){
                int outIndex=-1;
                for (int i=0; i<itemsReplaceEFiltered.Count; i++){
                    if (itemsReplaceEFiltered[i]==selectedId){
                        outIndex=i;
                        break;
                    }
                }

                if (outIndex==-1){
                    listBoxReplaceE.SelectedIndex=-1;
                    CurrentReplaceE=null;
                } else listBoxReplaceE.SelectedIndex=outIndex;
            } else {
                listBoxReplaceE.SelectedIndex=-1;
                CurrentReplaceE=null;
            }
            SetCurrentReplaceE();
        }

        void RemoveCurrentReplaceE(object sender, EventArgs e) {
            itemsReplaceE.Remove(CurrentReplaceE);
        }

        void renameWithBindsToolStripMenuItem1_Click(object sender, EventArgs e) {
              if (CurrentPatternNounFrom != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternNoun '" + CurrentPatternNounFrom.Name + "' s odkazy na...",
                    Input = CurrentPatternNounFrom.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternNounFrom.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternNounFromSaveCurrent();
                            SaveCurrentNoun();

                            foreach (ItemNoun noun in itemsNouns) {
                                if (noun.PatternFrom == CurrentPatternNounFrom.Name) {
                                    noun.PatternFrom = edit.ReturnString;
                                }
                            }
                            CurrentPatternNounFrom.Name = edit.ReturnString;
                            PatternNounFromSetCurrent();
                            SetCurrentNoun();
                        }
                    }
                }
            }
        }

        void renameWithBindsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentPatternNounTo != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternNoun '" + CurrentPatternNounTo.Name + "' s odkazy na...",
                    Input = CurrentPatternNounTo.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternNounTo.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternNounToSaveCurrent();
                            SaveCurrentNoun();

                            foreach (ItemNoun noun in itemsNouns) {
                                for (int i=0; i<noun.To.Count; i++){
                                    TranslatingToDataWithPattern d = noun.To[i];
                                    if (d.Pattern== CurrentPatternNounTo.Name) {
                                        noun.To[i] = new TranslatingToDataWithPattern{Body=d.Body, Pattern=edit.ReturnString};
                                    }
                                }
                            }

                            CurrentPatternNounTo.Name = edit.ReturnString;
                            PatternNounToSetCurrent();
                            SetCurrentNoun();
                        }
                    }
                }
            }
        }

        void RenameWithBindsToolStripMenuItem2_Click(object sender, EventArgs e) {
            if (CurrentPatternAdjectiveFrom != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternAdjective '" + CurrentPatternAdjectiveFrom.Name + "' s odkazy na...",
                    Input = CurrentPatternAdjectiveFrom.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternAdjectiveFrom.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternAdjectiveFromSaveCurrent();
                            SaveCurrentAdjective();

                            foreach (ItemAdjective Adjective in itemsAdjectives) {
                                if (Adjective.PatternFrom == CurrentPatternAdjectiveFrom.Name) {
                                    Adjective.PatternFrom = edit.ReturnString;
                                }
                            }
                            CurrentPatternAdjectiveFrom.Name = edit.ReturnString;
                            PatternAdjectiveFromSetCurrent();
                            PatternAdjectiveFromSetListBox();
                            SetCurrentAdjective();
                        }
                    }
                }
            }
        }

        void ToolStripMenuItem53_Click(object sender, EventArgs e) {
              if (CurrentPatternAdjectiveTo != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternAdjective '" + CurrentPatternAdjectiveTo.Name + "' s odkazy na...",
                    Input = CurrentPatternAdjectiveTo.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternAdjectiveTo.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternAdjectiveToSaveCurrent();
                            SaveCurrentAdjective();

                            foreach (ItemAdjective Adjective in itemsAdjectives) {
                                //if (Adjective.PatternTo == CurrentPatternAdjectiveTo.Name) {
                                //    Adjective.PatternTo = edit.ReturnString;
                                //}

                                for (int i=0; i<Adjective.To.Count; i++){
                                    TranslatingToDataWithPattern d = Adjective.To[i];
                                    if (d.Pattern == CurrentPatternAdjectiveTo.Name) {
                                        //if (d.Item1.Contains(d.Item2)) return true;
                                        Adjective.To[i] = new TranslatingToDataWithPattern{Body=d.Body, Pattern=edit.ReturnString };
                                    }
                                }
                            }
                            CurrentPatternAdjectiveTo.Name = edit.ReturnString;
                            PatternAdjectiveToSetCurrent();
                            PatternAdjectiveToSetListBox();

                            SetCurrentAdjective();
                        }
                    }
                }
            }
        }

        void RenameWithBindsToolStripMenuItem4_Click(object sender, EventArgs e) {
            if (CurrentPatternVerbFrom != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternVerb '" + CurrentPatternVerbFrom.Name + "' s odkazy na...",
                    Input = CurrentPatternVerbFrom.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternVerbFrom.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternVerbFromSaveCurrent();
                            SaveCurrentVerb();

                            foreach (ItemVerb Verb in itemsVerbs) {
                                if (Verb.PatternFrom == CurrentPatternVerbFrom.Name) {
                                    Verb.PatternFrom = edit.ReturnString;
                                }
                            }
                            CurrentPatternVerbFrom.Name = edit.ReturnString;
                            PatternVerbFromSetCurrent();
                            PatternVerbFromSetListBox();
                            SetCurrentVerb();
                        }
                    }
                }
            }
        }

        void RenameWithBindsToolStripMenuItem3_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounFrom != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternPronoun '" + CurrentPatternPronounFrom.Name + "' s odkazy na...",
                    Input = CurrentPatternPronounFrom.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternPronounFrom.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternPronounFromSaveCurrent();
                            PronounSaveCurrent();

                            foreach (ItemPronoun Pronoun in itemsPronouns) {
                                if (Pronoun.PatternFrom == CurrentPatternPronounFrom.Name) {
                                    Pronoun.PatternFrom = edit.ReturnString;
                                }
                            }
                            CurrentPatternPronounFrom.Name = edit.ReturnString;
                            PatternPronounFromSetCurrent();
                            SetCurrentPronoun();

                            PatternPronounFromSetListBox();
                        }
                    }
                }
            }
        }

        void ToolStripMenuItem75_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounTo != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternPronoun '" + CurrentPatternPronounTo.Name + "' s odkazy na...",
                    Input = CurrentPatternPronounTo.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternPronounTo.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternPronounToSaveCurrent();
                            PronounSaveCurrent();

                            foreach (ItemPronoun Pronoun in itemsPronouns) {
                                //if (Pronoun.PatternTo == CurrentPatternPronounTo.Name) {
                                //    Pronoun.PatternTo = edit.ReturnString;
                                //}

                                for (int i=0; i<Pronoun.To.Count; i++){
                                    TranslatingToDataWithPattern d = Pronoun.To[i];
                                    if (d.Pattern == CurrentPatternPronounTo.Name) {
                                        //if (d.Item1.Contains(d.Item2)) return true;
                                        Pronoun.To[i].Pattern = edit.ReturnString;
                                    }
                                }
                            }
                            CurrentPatternPronounTo.Name = edit.ReturnString;
                            PatternPronounToSetCurrent();
                            SetCurrentPronoun();

                            PatternPronounToSetListBox();
                        }
                    }
                }
            }
        }

        void ToolStripMenuItem69_Click(object sender, EventArgs e) {
            if (CurrentPatternNumberFrom != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternNumber '" + CurrentPatternNumberFrom.Name + "' s odkazy na...",
                    Input = CurrentPatternNumberFrom.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternNumberFrom.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternNumberFromSaveCurrent();
                            SaveCurrentNumber();

                            foreach (ItemNumber Number in itemsNumbers) {
                                if (Number.PatternFrom == CurrentPatternNumberFrom.Name) {
                                    Number.PatternFrom = edit.ReturnString;
                                }
                            }
                            CurrentPatternNumberFrom.Name = edit.ReturnString;
                            PatternNumberFromSetCurrent();
                            SetCurrentNumber();
                        }
                    }
                }
            }
        }

        void ToolStripMenuItem86_Click(object sender, EventArgs e) {
            if (CurrentPatternNumberTo != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternNumber '" + CurrentPatternNumberTo.Name + "' s odkazy na...",
                    Input = CurrentPatternNumberTo.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternNumberTo.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternNumberToSaveCurrent();
                            SaveCurrentNumber();

                            foreach (ItemNumber Number in itemsNumbers) {
                                //if (Number.PatternTo == CurrentPatternNumberTo.Name) {
                                //    Number.PatternTo = edit.ReturnString;
                                //}

                                for (int i=0; i<Number.To.Count; i++){
                                    TranslatingToDataWithPattern d = Number.To[i];
                                    if (d.Pattern == CurrentPatternNumberTo.Name) {
                                        //if (d.Item1.Contains(d.Item2)) return true;
                                        Number.To[i] = new TranslatingToDataWithPattern{Body=d.Body, Pattern=edit.ReturnString };
                                    }
                                }
                            }
                            CurrentPatternNumberTo.Name = edit.ReturnString;
                            PatternNumberToSetCurrent();
                            SetCurrentNumber();
                        }
                    }
                }
            }
        }

        void toolStripMenuItem68_Click(object sender, EventArgs e) {
            // From pattternNumberFrom to patternNumberTo
            if (CurrentPatternNumberFrom == null)
                return;
            ItemPatternNumber PatternNumberFrom = CurrentPatternNumberFrom;
            itemsPatternNumberTo.Add(PatternNumberFrom);
            itemsPatternNumberFrom.Remove(PatternNumberFrom);

            PatternNumberToRefreshFilteredList();
            PatternNumberFromRefreshFilteredList();

            PatternNumberFromSetListBox();
            PatternNumberFromSetCurrent();

            PatternNumberToSetListBox();
            PatternNumberToSetCurrent();
        }

        void toolStripMenuItem88_Click(object sender, EventArgs e) {
            // From pattternNumberFrom to patternNumberTo
            if (CurrentPatternNumberTo == null)  return;
            ItemPatternNumber PatternNumberTo = CurrentPatternNumberTo;
            itemsPatternNumberFrom.Add(PatternNumberTo);
            itemsPatternNumberTo.Remove(PatternNumberTo);

            PatternNumberToRefreshFilteredList();
            PatternNumberFromRefreshFilteredList();

            PatternNumberFromSetListBox();
            PatternNumberFromSetCurrent();

            PatternNumberToSetListBox();
            PatternNumberToSetCurrent();
        }

        void addFromWikidirectonaryToolStripMenuItem1_Click(object sender, EventArgs e) {
            string name = GetString("", "Název verb");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;

            try{
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    List<Table> tables=new List<Table>();
                    Computation.FindTableInHTML(html, "konjugace verbum", ref tables);

                    ItemPatternVerb pattern = new ItemPatternVerb {
                        Name = name,
                       // TypeShow = VerbTypeShow.Unknown,
                        Infinitive = name
                    };
                   // bool future=false;
                    if (Computation.FindTableInListByName(tables, "Oznamovací způsob", out Table ozn)) {
                        if (ozn.Rows[2].Cells[0].Text=="Přítomný čas") {
                            pattern.SContinous=true;
                            pattern.Continous=new string[6];
                            for (int i=0; i<6; i++) {
                                pattern.Continous[i]=ozn.Rows[2].Cells[1+i].Text;
                            }
                        }
                        if (ozn.Rows.Count==4){
                            if (ozn.Rows[3].Cells[0].Text=="Budoucí čas") {
                                pattern.SFuture=true;
                                pattern.Future=new string[6];
                                //future=true;
                                for (int i=0; i<6; i++) {
                                    pattern.Future[i]=ozn.Rows[3].Cells[1+i].Text;
                                }
                               // pattern.TypeShow=VerbTypeShow.All;
                            }
                        }
                    }else{
                        MessageBox.Show("Špatné jméno");
                        return;
                    }

                    if (Computation.FindTableInListByName(tables, "Rozkazovací způsob", out Table roz)) {
                        pattern.Imperative=new string[3];
                        pattern.SImperative=true;
                        for (int i=0; i<3; i++) {
                            pattern.Imperative[i]=roz.Rows[2].Cells[1+i].Text;
                        }
                    }

                if (Computation.FindTableInListByName(tables, "Příčestí", out Table tr)) {
                    if (tr.Rows[2].Cells[0].Text=="Činné") {
                        pattern.SPastActive=true;
                        //if (future)pattern.TypeShow=VerbTypeShow.All;
                        //else pattern.TypeShow=VerbTypeShow.Cinne;
                        pattern.PastActive=new string[8];
                        if (tr.Rows[2].Cells.Count==7){
                            pattern.PastActive[0]=tr.Rows[2].Cells[1].Text;
                            pattern.PastActive[1]=tr.Rows[2].Cells[1].Text;
                            pattern.PastActive[2]=tr.Rows[2].Cells[2].Text;
                            pattern.PastActive[3]=tr.Rows[2].Cells[3].Text;
                            pattern.PastActive[4]=tr.Rows[2].Cells[4].Text;
                            pattern.PastActive[5]=tr.Rows[2].Cells[5].Text;
                            pattern.PastActive[6]=tr.Rows[2].Cells[5].Text;
                            pattern.PastActive[7]=tr.Rows[2].Cells[6].Text;
                        }else{
                            for (int i=0; i<8; i++) {
                                pattern.PastActive[i]=tr.Rows[2].Cells[1+i].Text;
                            }
                        }
                    }
                    if (tr.Rows[2].Cells[0].Text=="Trpné") {
                        pattern.SPastPassive=true;
                        pattern.PastPassive=new string[8];
                        if (tr.Rows[2].Cells.Count==7) {
                            pattern.PastPassive[0]=tr.Rows[2].Cells[1].Text;
                            pattern.PastPassive[1]=tr.Rows[2].Cells[1].Text;
                            pattern.PastPassive[2]=tr.Rows[2].Cells[2].Text;
                            pattern.PastPassive[3]=tr.Rows[2].Cells[3].Text;
                            pattern.PastPassive[4]=tr.Rows[2].Cells[4].Text;
                            pattern.PastPassive[5]=tr.Rows[2].Cells[5].Text;
                            pattern.PastPassive[6]=tr.Rows[2].Cells[5].Text;
                            pattern.PastPassive[7]=tr.Rows[2].Cells[6].Text;
                        }else{
                            for (int i=0; i<8; i++) {
                                pattern.PastPassive[i]=tr.Rows[2].Cells[1+i].Text;
                            }
                        }
                        //if (future)pattern.TypeShow=VerbTypeShow.FuturePassive;
                        //else pattern.TypeShow=VerbTypeShow.Trpne;
                    }
                    if (tr.Rows.Count>=4) {
                        //if (future)pattern.TypeShow=VerbTypeShow.All;
                        //else pattern.TypeShow=VerbTypeShow.TrpneCinne;
                        if (tr.Rows[3].Cells[0].Text=="Činné") {
                            pattern.SPastActive=true;
                            pattern.PastActive=new string[8];
                            if (tr.Rows[2].Cells.Count==7) {
                                pattern.PastActive[0]=tr.Rows[3].Cells[1].Text;
                                pattern.PastActive[1]=tr.Rows[3].Cells[1].Text;
                                pattern.PastActive[2]=tr.Rows[3].Cells[2].Text;
                                pattern.PastActive[3]=tr.Rows[3].Cells[3].Text;
                                pattern.PastActive[4]=tr.Rows[3].Cells[4].Text;
                                pattern.PastActive[5]=tr.Rows[3].Cells[5].Text;
                                pattern.PastActive[6]=tr.Rows[3].Cells[5].Text;
                                pattern.PastActive[7]=tr.Rows[3].Cells[6].Text;
                            } else {
                                for (int i=0; i<8; i++) {
                                    pattern.PastActive[i]=tr.Rows[3].Cells[1+i].Text;
                                }
                            }
                        }
                        if (tr.Rows[3].Cells[0].Text=="Trpné") {
                            pattern.SPastPassive=true;
                            pattern.PastPassive=new string[8];
                            if (tr.Rows[2].Cells.Count==7) {
                                pattern.PastPassive[0]=tr.Rows[3].Cells[1].Text;
                                pattern.PastPassive[1]=tr.Rows[3].Cells[1].Text;
                                pattern.PastPassive[2]=tr.Rows[3].Cells[2].Text;
                                pattern.PastPassive[3]=tr.Rows[3].Cells[3].Text;
                                pattern.PastPassive[4]=tr.Rows[3].Cells[4].Text;
                                pattern.PastPassive[5]=tr.Rows[3].Cells[5].Text;
                                pattern.PastPassive[6]=tr.Rows[3].Cells[5].Text;
                                pattern.PastPassive[7]=tr.Rows[3].Cells[6].Text;
                            } else {
                                for (int i=0; i<8; i++) {
                                    pattern.PastPassive[i]=tr.Rows[3].Cells[1+i].Text;
                                }
                            }
                        }
                    }
                }

                if (Computation.FindTableInListByName(tables, "Přechodníky", out Table pre)) {
                    if (pre.Rows.Count>=4) {
                        if (pre.Rows[2].Cells[0].Text=="Přítomný") {
                            pattern.TransgressiveCont=new string[3];
                            pattern.STransgressiveCont=true;
                            for (int i=0; i<3; i++) pattern.TransgressiveCont[i]=pre.Rows[2].Cells[1+i].Text;
                        } else if (pre.Rows[2].Cells[0].Text=="Minulý") {
                            pattern.TransgressivePast=new string[3];
                            pattern.STransgressivePast=true;
                            for (int i=0; i<3; i++) pattern.TransgressivePast[i]=pre.Rows[2].Cells[1+i].Text;
                        }

                        if (pre.Rows[3].Cells[0].Text=="Přítomný") {
                            pattern.TransgressiveCont=new string[3];
                            pattern.STransgressiveCont=true;
                            for (int i=0; i<3; i++) pattern.TransgressiveCont[i]=pre.Rows[3].Cells[1+i].Text;
                         } else if (pre.Rows[3].Cells[0].Text=="Minulý") {
                            pattern.TransgressivePast=new string[3];
                            pattern.STransgressivePast=true;
                            for (int i=0; i<3; i++) pattern.TransgressivePast[i]=pre.Rows[3].Cells[1+i].Text;
                        }
                    }
                }

                pattern.Optimize();

                itemsPatternVerbFrom.Add(pattern);
                PatternVerbFromRefreshFilteredList();
                PatternVerbToCheckBoxesSet();
                PatternVerbFromSetListBox();
                PatternVerbFromListBoxSetCurrent();
            };
            }catch{ MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);
        }

        void optimalizovatToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (CurrentPatternVerbFrom!=null) {
                PatternVerbFromSaveCurrent();
                CurrentPatternVerbFrom.Optimize();
                PatternVerbFromSetCurrent();
                PatternVerbFromRefreshFilteredList();
                PatternVerbFromSetListBox();
            }
        }

        void toolStripMenuItem99_Click(object sender, EventArgs e) {
            // From pattternVerbFrom to patternVerbTo
            if (CurrentPatternVerbTo == null)  return;
            ItemPatternVerb PatternVerbTo = CurrentPatternVerbTo;
            itemsPatternVerbFrom.Add(PatternVerbTo);
            itemsPatternVerbTo.Remove(PatternVerbTo);

            PatternVerbToRefreshFilteredList();
            PatternVerbFromRefreshFilteredList();

            PatternVerbFromSetListBox();
            PatternVerbFromSetCurrent();

            PatternVerbToSetListBox();
            PatternVerbToSetCurrent();
        }

        void toolStripMenuItem100_Click(object sender, EventArgs e) {
            PatternVerbToButtonRemove_Click(null,null);
        }

        void moveToolStripMenuItem1_Click(object sender, EventArgs e) {
             // From pattternNounFrom to patternNounTo
            if (CurrentPatternNounTo == null)  return;
            ItemPatternNoun PatternNounTo = CurrentPatternNounTo;
            itemsPatternNounFrom.Add(PatternNounTo);
            itemsPatternNounTo.Remove(PatternNounTo);

            PatternNounToRefreshFilteredList();
            PatternNounFromRefreshFilteredList();

            PatternNounFromSetListBox();
            PatternNounFromSetCurrent();


            PatternNounToSetListBox();
            PatternNounToSetCurrent();
        }

        void toolStripMenuItem55_Click(object sender, EventArgs e) {
             // From pattternAdjectiveFrom to patternAdjectiveTo
            if (CurrentPatternAdjectiveTo == null)  return;
            ItemPatternAdjective PatternAdjectiveTo = CurrentPatternAdjectiveTo;
            itemsPatternAdjectiveFrom.Add(PatternAdjectiveTo);
            itemsPatternAdjectiveTo.Remove(PatternAdjectiveTo);

            PatternAdjectiveToRefreshFilteredList();
            PatternAdjectiveFromRefreshFilteredList();

            PatternAdjectiveFromSetListBox();
            PatternAdjectiveFromSetCurrent();

            PatternAdjectiveToSetListBox();
            PatternAdjectiveToSetCurrent();
        }

        void toolStripMenuItem77_Click(object sender, EventArgs e) {
            // From pattternPronounFrom to patternPronounTo
            if (CurrentPatternPronounTo == null)  return;
            ItemPatternPronoun PatternPronounTo = CurrentPatternPronounTo;
            itemsPatternPronounFrom.Add(PatternPronounTo);
            itemsPatternPronounTo.Remove(PatternPronounTo);

            PatternPronounToRefreshFilteredList();
            PatternPronounFromRefreshFilteredList();

            PatternPronounFromSetListBox();
            PatternPronounFromSetCurrent();

            PatternPronounToSetListBox();
            PatternPronounToSetCurrent();
        }

        void toolStripMenuItem97_Click(object sender, EventArgs e) {
            if (CurrentPatternVerbTo != null) {
                FormString edit = new FormString {
                    LabelText = "Přejmenovat PatternVerb '" + CurrentPatternVerbTo.Name + "' s odkazy na...",
                    Input = CurrentPatternVerbTo.Name
                };
                edit.RefreshInp();
                DialogResult dr = edit.ShowDialog();
                if (dr == DialogResult.OK) {
                    if (CurrentPatternVerbTo.Name != edit.ReturnString) {
                        if (!string.IsNullOrEmpty(edit.ReturnString)) {
                            PatternVerbToSaveCurrent();
                            SaveCurrentVerb();

                            foreach (ItemVerb verb in itemsVerbs) {
                                //if (Verb.PatternTo == CurrentPatternVerbTo.Name) {
                                //    Verb.PatternTo = edit.ReturnString;
                                //}
                                for (int i=0; i<verb.To.Count; i++){
                                    TranslatingToDataWithPattern d = verb.To[i];
                                    if (d.Pattern == CurrentPatternVerbTo.Name) {
                                        //if (d.Item1.Contains(d.Item2)) return true;
                                        verb.To[i] = new TranslatingToDataWithPattern{Body=d.Body, Pattern=edit.ReturnString };
                                    }
                                }
                            }
                            CurrentPatternVerbTo.Name = edit.ReturnString;
                            PatternVerbToSetCurrent();
                            PatternVerbToSetListBox();
                            SetCurrentVerb();
                        }
                    }
                }
            }
        }

        void OptimizeToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentPatternVerbTo!=null) CurrentPatternVerbTo.Optimize();
        }

        void ComboBoxPatternPronounToType_SelectedIndexChanged(object sender, EventArgs e) {
            if (CurrentPatternPronounTo != null) {
                CurrentPatternPronounTo.Type = (PronounType)comboBoxPatternPronounToType.SelectedIndex;
                ChangeTypePatternPronounTo(CurrentPatternPronounTo?.Type);
            }
        }

        void ToolStripMenuItem76_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounTo != null) {
                itemsPatternPronounTo.Add(CurrentPatternPronounTo.Duplicate());
            }
        }

        void DuplicaeToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentPatternNounTo != null) {
                ItemPatternNoun item =CurrentPatternNounTo.Duplicate();
                itemsPatternNounTo.Add(item);
                PatternNounToRefreshFilteredList();
                PatternNounToSetListBox();
                PatternNounToSetCurrent();
            }
        }

        void optimizeToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounTo!=null) {
                PatternPronounToSaveCurrent();
                CurrentPatternPronounTo.Optimize();
                PatternPronounToSetCurrent();
                PatternPronounToSaveCurrent();
                PatternPronounToRefreshFilteredList();
                PatternPronounToSetListBox();
                PatternPronounToSetCurrent();
            }
        }

        void OptimizeToolStripMenuItem2_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounFrom!=null) {
                PatternPronounFromSaveCurrent();
                CurrentPatternPronounFrom.Optimize();
                PatternPronounFromSetCurrent();
                PatternPronounFromRefreshFilteredList();
                PatternPronounFromSetListBox();
            }
        }

        void DuplicateToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (CurrentPatternNounFrom!=null) {
                PatternNounFromSaveCurrent();
                ItemPatternNoun item = CurrentPatternNounFrom.Duplicate();

                itemsPatternNounFrom.Add(item);
                PatternNounFromRefreshFilteredList();
                PatternNounFromSetListBox();
                PatternNounFromSetCurrent();
            }
        }

        void ToolStripMenuItem79_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounTo==null) return;
              switch (CurrentPatternPronounTo.Type) {
                case PronounType.Unknown:
                    break;

                case PronounType.NoDeklination:
                    CurrentPatternPronounTo.Shapes[0]="";
                    break;

                case PronounType.DeklinationOnlySingle:
                    CurrentPatternPronounTo.Shapes[0]="";
                    CurrentPatternPronounTo.Shapes[1]="";
                    CurrentPatternPronounTo.Shapes[2]="";
                    CurrentPatternPronounTo.Shapes[3]="";
                    CurrentPatternPronounTo.Shapes[4]="";
                    CurrentPatternPronounTo.Shapes[5]="";
                    CurrentPatternPronounTo.Shapes[6]="";
                    break;

                case PronounType.Deklination:
                    CurrentPatternPronounTo.Shapes[0]="";
                    CurrentPatternPronounTo.Shapes[1]="";
                    CurrentPatternPronounTo.Shapes[2]="";
                    CurrentPatternPronounTo.Shapes[3]="";
                    CurrentPatternPronounTo.Shapes[4]="";
                    CurrentPatternPronounTo.Shapes[5]="";
                    CurrentPatternPronounTo.Shapes[6]="";

                    CurrentPatternPronounTo.Shapes[ 7]="";
                    CurrentPatternPronounTo.Shapes[ 8]="";
                    CurrentPatternPronounTo.Shapes[ 9]="";
                    CurrentPatternPronounTo.Shapes[10]="";
                    CurrentPatternPronounTo.Shapes[11]="";
                    CurrentPatternPronounTo.Shapes[12]="";
                    CurrentPatternPronounTo.Shapes[13]="";
                    break;

                case PronounType.DeklinationWithGender:
                    CurrentPatternPronounTo.Shapes[ 0]="";
                    CurrentPatternPronounTo.Shapes[ 1]="";
                    CurrentPatternPronounTo.Shapes[ 2]="";
                    CurrentPatternPronounTo.Shapes[ 3]="";
                    CurrentPatternPronounTo.Shapes[ 4]="";
                    CurrentPatternPronounTo.Shapes[ 5]="";
                    CurrentPatternPronounTo.Shapes[ 6]="";
                    CurrentPatternPronounTo.Shapes[ 7]="";
                    CurrentPatternPronounTo.Shapes[ 8]="";
                    CurrentPatternPronounTo.Shapes[ 9]="";
                    CurrentPatternPronounTo.Shapes[10]="";
                    CurrentPatternPronounTo.Shapes[11]="";
                    CurrentPatternPronounTo.Shapes[12]="";
                    CurrentPatternPronounTo.Shapes[13]="";

                    CurrentPatternPronounTo.Shapes[14]="";
                    CurrentPatternPronounTo.Shapes[15]="";
                    CurrentPatternPronounTo.Shapes[16]="";
                    CurrentPatternPronounTo.Shapes[17]="";
                    CurrentPatternPronounTo.Shapes[18]="";
                    CurrentPatternPronounTo.Shapes[19]="";
                    CurrentPatternPronounTo.Shapes[20]="";
                    CurrentPatternPronounTo.Shapes[21]="";
                    CurrentPatternPronounTo.Shapes[22]="";
                    CurrentPatternPronounTo.Shapes[23]="";
                    CurrentPatternPronounTo.Shapes[24]="";
                    CurrentPatternPronounTo.Shapes[25]="";
                    CurrentPatternPronounTo.Shapes[26]="";
                    CurrentPatternPronounTo.Shapes[27]="";

                    CurrentPatternPronounTo.Shapes[28]="";
                    CurrentPatternPronounTo.Shapes[29]="";
                    CurrentPatternPronounTo.Shapes[30]="";
                    CurrentPatternPronounTo.Shapes[31]="";
                    CurrentPatternPronounTo.Shapes[32]="";
                    CurrentPatternPronounTo.Shapes[33]="";
                    CurrentPatternPronounTo.Shapes[34]="";
                    CurrentPatternPronounTo.Shapes[35]="";
                    CurrentPatternPronounTo.Shapes[36]="";
                    CurrentPatternPronounTo.Shapes[37]="";
                    CurrentPatternPronounTo.Shapes[38]="";
                    CurrentPatternPronounTo.Shapes[39]="";
                    CurrentPatternPronounTo.Shapes[40]="";
                    CurrentPatternPronounTo.Shapes[41]="";

                    CurrentPatternPronounTo.Shapes[42]="";
                    CurrentPatternPronounTo.Shapes[43]="";
                    CurrentPatternPronounTo.Shapes[44]="";
                    CurrentPatternPronounTo.Shapes[45]="";
                    CurrentPatternPronounTo.Shapes[46]="";
                    CurrentPatternPronounTo.Shapes[47]="";
                    CurrentPatternPronounTo.Shapes[48]="";
                    CurrentPatternPronounTo.Shapes[49]="";
                    CurrentPatternPronounTo.Shapes[50]="";
                    CurrentPatternPronounTo.Shapes[51]="";
                    CurrentPatternPronounTo.Shapes[52]="";
                    CurrentPatternPronounTo.Shapes[53]="";
                    CurrentPatternPronounTo.Shapes[54]="";
                    CurrentPatternPronounTo.Shapes[55]="";
                    break;
            }
            PatternPronounToSetCurrent();
        }

        void toolStripMenuItem63_Click(object sender, EventArgs e) {
            string name = GetString("", "Název numery");
            if (name == null) return;

            if (name=="dvě" || name=="dva") {
                AddNumber(ItemPatternNumber.Dve());
                return;
            }
            DownloadDataCompletedEventHandler handler=null;

            try {
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    ItemPatternNumber pattern = ParseItemWikidirectonary.ItemPatternNumber(html, out string error, name);
                    if (!string.IsNullOrEmpty(error)) MessageBox.Show(error);
                    if (pattern!=null) {
                        AddNumber(pattern);
                    }
                };
            }catch{ MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);

            void AddNumber(ItemPatternNumber pattern) {
                itemsPatternNumberFrom.Add(pattern);
                PatternNumberFromRefreshFilteredList();
                PatternNumberFromSetListBox();
                PatternNumberFromListBoxSetCurrent();
            }
            //DownloadDataCompletedEventHandler handler=null;

            //try{
            //    handler += (sender2, e2) => {
            //        if (e2.Error!=null) {
            //            MessageBox.Show("Error "+e2.Error.Message);
            //            return;
            //        }
            //        var data = e2.Result;
            //        string html = Encoding.UTF8.GetString(data);

            //        List<Table> tables=new List<Table>();
            //        Computation.FindTableInHTML(html, "deklinace numerale", ref tables);

            //        if (tables.Count==1) {
            //            Table table=tables[0];

            //            if (table.Rows.Count==9 && table.Rows[2].Cells.Count==9) {
            //                ItemPatternNumber pattern = new ItemPatternNumber {
            //                    Name = name,
            //                    ShowType = NumberType.DeklinationWithGender,
            //                    Shapes = new string[7 * 8]
            //                };


            //                for (int i=0; i<7; i++) pattern.Shapes[i+0*7]=table.Rows[2+i].Cells[1+0].Text;
            //                for (int i=0; i<7; i++) pattern.Shapes[i+2*7]=table.Rows[2+i].Cells[1+1].Text;
            //                for (int i=0; i<7; i++) pattern.Shapes[i+4*7]=table.Rows[2+i].Cells[1+2].Text;
            //                for (int i=0; i<7; i++) pattern.Shapes[i+6*7]=table.Rows[2+i].Cells[1+3].Text;

            //                for (int i=0; i<7; i++) pattern.Shapes[i+1*7]=table.Rows[2+i].Cells[1+4].Text;
            //                for (int i=0; i<7; i++) pattern.Shapes[i+3*7]=table.Rows[2+i].Cells[1+5].Text;
            //                for (int i=0; i<7; i++) pattern.Shapes[i+5*7]=table.Rows[2+i].Cells[1+6].Text;
            //                for (int i=0; i<7; i++) pattern.Shapes[i+7*7]=table.Rows[2+i].Cells[1+7].Text;



            //                pattern.Optimize();

            //                itemsPatternNumberFrom.Add(pattern);
            //                PatternNumberFromRefreshFilteredList();
            //                PatternNumberFromSetListBox();
            //                PatternNumberFromListBoxSetCurrent();
            //            } else if (table.Rows.Count==8 && table.Rows[2].Cells.Count==2) {
            //                ItemPatternNumber pattern = new ItemPatternNumber {
            //                    Name = name,
            //                    ShowType = NumberType.DeklinationOnlySingle,
            //                    Shapes = new string[7 * 8]
            //                };

            //                for (int i=0; i<7; i++) {
            //                    pattern.Shapes[i]=table.Rows[1+i].Cells[1].Text;
            //                }

            //                pattern.Optimize();

            //                itemsPatternNumberFrom.Add(pattern);
            //                PatternNumberFromRefreshFilteredList();
            //                PatternNumberFromSetListBox();
            //                PatternNumberFromListBoxSetCurrent();
            //            }
            //        }
            //    };
            //}catch{ MessageBox.Show("Error");}
            //Computation.DownloadString(ref handler, name);
        }

        void toolStripMenuItem66_Click(object sender, EventArgs e) {
            if (CurrentPatternNumberFrom!=null) {
                PatternNumberFromSaveCurrent();
                ItemPatternNumber item =CurrentPatternNumberFrom.Duplicate();
                itemsPatternNumberFrom.Add(item);
                PatternNumberFromRefreshFilteredList();
                PatternNumberFromSetListBox();
                PatternNumberFromListBoxSetCurrent();
            }
        }

        void ToolStripMenuItem147_Click(object sender, EventArgs e) {
             OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Textové soubory|*.*txt|Všecky sóbore|*.*"
            };
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    doingJob = true;
                    Edited = true;
                    ChangeCaptionText();
                    SaveCurrentReplaceE();
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines) {
                        string[] parts = line.Split('|');
                        if (parts[0] == "GE" && parts.Length == 3) {
                            foreach (string s in parts[1].Split('#')) {
                                ItemReplaceE w = new ItemReplaceE {
                                    From = s,
                                    To = parts[2].Replace("#", ",")
                                };
                                itemsReplaceE.Add(w);
                            }
                        }
                    }

                    ReplaceERefreshFilteredList();
                    ReplaceESetListBox();
                    ListBoxSetCurrentReplaceE();
                    SetCurrentReplaceE();

                    doingJob = false;
                }
            }
        }

        void toolStripMenuItem107_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Textové soubory|*.*txt|Všecky sóbore|*.*"
            };
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    doingJob = true;
                    Edited = true;
                    ChangeCaptionText();
                    PhraseSaveCurrent();
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines) {
                        string[] parts = line.Split('|');
                        if (parts[0] == "P" && parts.Length == 3) {
                            foreach (string s in parts[1].Split('#')) {
                                ItemPhrase w = new ItemPhrase {
                                    From = s,
                                    To = Methods.LoadListTranslatingToDataWithPattern(parts[2],'#')//parts[2].Replace("#", ",")
                                };
                                itemsPhrases.Add(w);
                            }
                        }
                    }

                    PhraseRefreshFilteredList();
                    PhraseSetListBox();
                    PhraseListBoxSetCurrent();
                    PhraseSetCurrent();

                    doingJob = false;
                }
            }
        }

        void toolStripMenuItem140_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Textové soubory|*.*txt|Všecky sóbore|*.*"
            };
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    doingJob = true;
                    Edited = true;
                    ChangeCaptionText();
                    ReplaceGSaveCurrent();
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines) {
                        string[] parts = line.Split('|');
                        if (parts[0] == "G" && parts.Length == 3) {
                            foreach (string s in parts[1].Split('#')) {
                                ItemReplaceG w = new ItemReplaceG {
                                    From = s,
                                    To = parts[2].Replace("#", ",")
                                };
                                itemsReplaceG.Add(w);
                            }
                        }
                    }

                    ReplaceGRefreshFilteredList();
                    ReplaceGSetListBox();
                    ListBoxSetCurrentReplaceG();
                    SetCurrentReplaceG();

                    doingJob = false;
                }
            }
        }

        void toolStripMenuItem133_Click(object sender, EventArgs e) {
             OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Textové soubory|*.*txt|Všecky sóbore|*.*"
            };
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    doingJob = true;
                    Edited = true;
                    ChangeCaptionText();
                    ReplaceSSaveCurrent();
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines) {
                        string[] parts = line.Split('|');
                        if (parts[0] == "GS" && parts.Length == 3) {
                            foreach (string s in parts[1].Split('#')) {
                                ItemReplaceS w = new ItemReplaceS {
                                    From = s,
                                    To = parts[2].Replace("#", ",")
                                };
                                itemsReplaceS.Add(w);
                            }
                        }
                    }

                    ReplaceSRefreshFilteredList();
                    ReplaceSSetListBox();
                    ReplaceSListBoxSetCurrent();
                    ReplaceSSetCurrent();

                    doingJob = false;
                }
            }
        }

        void toolStripMenuItem114_Click(object sender, EventArgs e) {
             OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Textové soubory|*.*txt|Všecky sóbore|*.*"
            };
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    doingJob = true;
                    Edited = true;
                    ChangeCaptionText();
                    SaveCurrentSentence();
                    string[] lines = File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines) {
                        string[] parts = line.Split('|');
                        if (parts[0] == "S" && parts.Length == 3) {
                            foreach (string s in parts[1].Split('#')) {
                                ItemSentence w = new ItemSentence {
                                    From = s,
                                    To = Methods.LoadListTranslatingToDataWithPattern(parts[2],'#')//.Replace("#", ",")
                                };
                                itemsSentences.Add(w);
                            }
                        }
                    }

                    SentenceRefreshFilteredList();
                    SetListBoxSentence();
                    ListBoxSetCurrentSentence();
                    SetCurrentSentence();

                    doingJob = false;
                }
            }
        }

        void PatternNounFromRefresh() {
            PatternNounFromRefreshFilteredList();
            PatternNounFromSetListBox();
            PatternNounFromListBoxSetCurrent();
        }

        void PatternNounToRefresh() {
            PatternNounToRefreshFilteredList();
            PatternNounToSetListBox();
            PatternNounToListBoxSetCurrent();
        }

        void NounRefresh() {
            NounRefreshFilteredList();
            NounSetListBox();
            NounListBoxSetCurrent();
        }

        void ClearToolStripMenuItem4_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounFrom==null) return;
            switch (CurrentPatternPronounFrom.Type) {
                case PronounType.Unknown:
                    break;

                case PronounType.NoDeklination:
                    CurrentPatternPronounFrom.Shapes[0]="";
                    break;

                case PronounType.DeklinationOnlySingle:
                    CurrentPatternPronounFrom.Shapes[0]="";
                    CurrentPatternPronounFrom.Shapes[1]="";
                    CurrentPatternPronounFrom.Shapes[2]="";
                    CurrentPatternPronounFrom.Shapes[3]="";
                    CurrentPatternPronounFrom.Shapes[4]="";
                    CurrentPatternPronounFrom.Shapes[5]="";
                    CurrentPatternPronounFrom.Shapes[6]="";
                    break;

                case PronounType.Deklination:
                    CurrentPatternPronounFrom.Shapes[0]="";
                    CurrentPatternPronounFrom.Shapes[1]="";
                    CurrentPatternPronounFrom.Shapes[2]="";
                    CurrentPatternPronounFrom.Shapes[3]="";
                    CurrentPatternPronounFrom.Shapes[4]="";
                    CurrentPatternPronounFrom.Shapes[5]="";
                    CurrentPatternPronounFrom.Shapes[6]="";

                    CurrentPatternPronounFrom.Shapes[ 7]="";
                    CurrentPatternPronounFrom.Shapes[ 8]="";
                    CurrentPatternPronounFrom.Shapes[ 9]="";
                    CurrentPatternPronounFrom.Shapes[10]="";
                    CurrentPatternPronounFrom.Shapes[11]="";
                    CurrentPatternPronounFrom.Shapes[12]="";
                    CurrentPatternPronounFrom.Shapes[13]="";
                    break;

                case PronounType.DeklinationWithGender:
                    CurrentPatternPronounFrom.Shapes[ 0]="";
                    CurrentPatternPronounFrom.Shapes[ 1]="";
                    CurrentPatternPronounFrom.Shapes[ 2]="";
                    CurrentPatternPronounFrom.Shapes[ 3]="";
                    CurrentPatternPronounFrom.Shapes[ 4]="";
                    CurrentPatternPronounFrom.Shapes[ 5]="";
                    CurrentPatternPronounFrom.Shapes[ 6]="";
                    CurrentPatternPronounFrom.Shapes[ 7]="";
                    CurrentPatternPronounFrom.Shapes[ 8]="";
                    CurrentPatternPronounFrom.Shapes[ 9]="";
                    CurrentPatternPronounFrom.Shapes[10]="";
                    CurrentPatternPronounFrom.Shapes[11]="";
                    CurrentPatternPronounFrom.Shapes[12]="";
                    CurrentPatternPronounFrom.Shapes[13]="";

                    CurrentPatternPronounFrom.Shapes[14]="";
                    CurrentPatternPronounFrom.Shapes[15]="";
                    CurrentPatternPronounFrom.Shapes[16]="";
                    CurrentPatternPronounFrom.Shapes[17]="";
                    CurrentPatternPronounFrom.Shapes[18]="";
                    CurrentPatternPronounFrom.Shapes[19]="";
                    CurrentPatternPronounFrom.Shapes[20]="";
                    CurrentPatternPronounFrom.Shapes[21]="";
                    CurrentPatternPronounFrom.Shapes[22]="";
                    CurrentPatternPronounFrom.Shapes[23]="";
                    CurrentPatternPronounFrom.Shapes[24]="";
                    CurrentPatternPronounFrom.Shapes[25]="";
                    CurrentPatternPronounFrom.Shapes[26]="";
                    CurrentPatternPronounFrom.Shapes[27]="";

                    CurrentPatternPronounFrom.Shapes[28]="";
                    CurrentPatternPronounFrom.Shapes[29]="";
                    CurrentPatternPronounFrom.Shapes[30]="";
                    CurrentPatternPronounFrom.Shapes[31]="";
                    CurrentPatternPronounFrom.Shapes[32]="";
                    CurrentPatternPronounFrom.Shapes[33]="";
                    CurrentPatternPronounFrom.Shapes[34]="";
                    CurrentPatternPronounFrom.Shapes[35]="";
                    CurrentPatternPronounFrom.Shapes[36]="";
                    CurrentPatternPronounFrom.Shapes[37]="";
                    CurrentPatternPronounFrom.Shapes[38]="";
                    CurrentPatternPronounFrom.Shapes[39]="";
                    CurrentPatternPronounFrom.Shapes[40]="";
                    CurrentPatternPronounFrom.Shapes[41]="";

                    CurrentPatternPronounFrom.Shapes[42]="";
                    CurrentPatternPronounFrom.Shapes[43]="";
                    CurrentPatternPronounFrom.Shapes[44]="";
                    CurrentPatternPronounFrom.Shapes[45]="";
                    CurrentPatternPronounFrom.Shapes[46]="";
                    CurrentPatternPronounFrom.Shapes[47]="";
                    CurrentPatternPronounFrom.Shapes[48]="";
                    CurrentPatternPronounFrom.Shapes[49]="";
                    CurrentPatternPronounFrom.Shapes[50]="";
                    CurrentPatternPronounFrom.Shapes[51]="";
                    CurrentPatternPronounFrom.Shapes[52]="";
                    CurrentPatternPronounFrom.Shapes[53]="";
                    CurrentPatternPronounFrom.Shapes[54]="";
                    CurrentPatternPronounFrom.Shapes[55]="";
                    break;
            }
            PatternPronounFromSetCurrent();
        }

        void aBCToolStripMenuItem2_Click(object sender, EventArgs e) {
            SimpleWordSaveCurrent();
            itemsSimpleWords = itemsSimpleWords.OrderBy(a => a.From).ToList();
            SimpleWordRefreshFilteredList();
            SimpleWordSetListBox();
        }

        void toolStripMenuItem111_Click(object sender, EventArgs e) {
            PhraseSaveCurrent();
            itemsPhrases = itemsPhrases.OrderBy(a => a.From).ToList();
            PhraseRefreshFilteredList();
            PhraseSetListBox();
        }

        void toolStripMenuItem137_Click(object sender, EventArgs e) {
            ReplaceSSaveCurrent();
            itemsReplaceS = itemsReplaceS.OrderBy(a => a.From).ToList();
            ReplaceSRefreshFilteredList();
            ReplaceSSetListBox();
        }

        void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternPronounTo();
        }

        void splitContainerPatternNumberTo_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternNumberTo();
        }

        void SplitContainer4_SplitterMoved(object sender, SplitterEventArgs e) {
            EditPosButtonsPatternVerbTo();
        }

        void AddFromClipboardToolStripMenuItem_Click(object sender, EventArgs e) {
            PatternNumberFromAddNewItem();
            Button2_Click(null,null);
        }

        void duplikateToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentPatternAdjectiveFrom!=null){
                PatternAdjectiveFromSaveCurrent();

                ItemPatternAdjective item =CurrentPatternAdjectiveFrom.Duplicate();

                itemsPatternAdjectiveFrom.Add(item);
                PatternAdjectiveFromRefreshFilteredList();
                PatternAdjectiveFromSetListBox();
                PatternAdjectiveFromSetCurrent();
            }
        }

        void DuplicateToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentPatternVerbFrom!=null){
                PatternVerbFromSaveCurrent();

                ItemPatternVerb item =CurrentPatternVerbFrom.Duplicate();
                itemsPatternVerbFrom.Add(item);

                PatternVerbFromRefreshFilteredList();
                PatternVerbFromSetListBox();
                PatternVerbFromSetCurrent();
            }
        }

        void OptimizeToolStripMenuItem4_Click(object sender, EventArgs e) {
            if (CurrentPatternAdjectiveTo!=null){
                CurrentPatternAdjectiveTo.Optimize();
                PatternAdjectiveToSetCurrent();
                PatternAdjectiveToRefreshFilteredList();
                PatternAdjectiveToSetListBox();
            }
        }

        void OptimizeToolStripMenuItem3_Click(object sender, EventArgs e) {
            if (CurrentPatternAdjectiveFrom!=null){
                CurrentPatternAdjectiveFrom.Optimize();
                PatternAdjectiveFromSetCurrent();
                PatternAdjectiveFromRefreshFilteredList();
                PatternAdjectiveFromSetListBox();
            }
        }

        void CheckBoxPatternVerbToContinous_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbToContinous.Visible=checkBoxPatternVerbToContinous.Checked;
        }

        void CheckBoxPatternVerbToFuture_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbToFuture.Visible=checkBoxPatternVerbToFuture.Checked;
        }

        void CheckBoxPatternVerbToImperative_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbToImperative.Visible=checkBoxPatternVerbToImperative.Checked;
        }

        void CheckBoxPatternVerbToPastPassive_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbToPastPassive.Visible=checkBoxPatternVerbToPastPassive.Checked;
        }

        void CheckBoxPatternVerbToPastActive_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbToPastActive.Visible=checkBoxPatternVerbToPastActive.Checked;
        }

        void CheckBoxPatternVerbToAuxiliary_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbToAuxiliary.Visible=checkBoxPatternVerbToAuxiliary.Checked;
        }

        void CheckBoxPatternVerbToTransgressiveCont_CheckedChanged(object sender, EventArgs e) {
            textBoxPatternVerbToTr1.Visible=checkBoxPatternVerbToTransgressiveCont.Checked;
            textBoxPatternVerbToTr2.Visible=checkBoxPatternVerbToTransgressiveCont.Checked;
            textBoxPatternVerbToTr3.Visible=checkBoxPatternVerbToTransgressiveCont.Checked;
        }

        void CheckBoxPatternVerbToTransgressivePast_CheckedChanged(object sender, EventArgs e) {
            textBoxPatternVerbToTr4.Visible=checkBoxPatternVerbToTransgressivePast.Checked;
            textBoxPatternVerbToTr5.Visible=checkBoxPatternVerbToTransgressivePast.Checked;
            textBoxPatternVerbToTr6.Visible=checkBoxPatternVerbToTransgressivePast.Checked;
        }

        void checkBoxPatternVerbFromContinous_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbFromContinous.Visible=checkBoxPatternVerbFromContinous.Checked;
        }

        void CheckBoxPatternVerbFromFuture_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbFromFuture.Visible=checkBoxPatternVerbFromFuture.Checked;
        }

        void CheckBoxPatternVerbFromImperative_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbFromImperative.Visible=checkBoxPatternVerbFromImperative.Checked;
        }

        void CheckBoxPatternVerbFromPastPassive_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbFromPastPassive.Visible=checkBoxPatternVerbFromPastPassive.Checked;
        }

        void CheckBoxPatternVerbFromPastActive_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbFromPastActive.Visible=checkBoxPatternVerbFromPastActive.Checked;
        }

        void CheckBoxPatternVerbFromAuxiliary_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbToAuxiliary.Visible=checkBoxPatternVerbFromAuxiliary.Checked;
        }

        void checkBoxPatternVerbFromTransgressiveCont_CheckedChanged(object sender, EventArgs e) {
            textBoxPatternVerbFromTr1.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
            textBoxPatternVerbFromTr2.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
            textBoxPatternVerbFromTr3.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
        }

        void checkBoxPatternVerbFromTransgressivePast_CheckedChanged(object sender, EventArgs e) {
            textBoxPatternVerbFromTr4.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
            textBoxPatternVerbFromTr5.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
            textBoxPatternVerbFromTr6.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
        }

        void CheckBoxPatternVerbFromContinous_CheckedChanged(object sender, EventArgs e) {
            tableLayoutPanelPatternVerbFromContinous.Visible=checkBoxPatternVerbFromContinous.Checked;
        }

        void CheckBoxPatternVerbFromTransgressiveCont_CheckedChanged(object sender, EventArgs e) {
            textBoxPatternVerbFromTr1.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
            textBoxPatternVerbFromTr2.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
            textBoxPatternVerbFromTr3.Visible=checkBoxPatternVerbFromTransgressiveCont.Checked;
        }

        void CheckBoxPatternVerbFromTransgressivePast_CheckedChanged(object sender, EventArgs e) {
            textBoxPatternVerbFromTr4.Visible=checkBoxPatternVerbFromTransgressivePast.Checked;
            textBoxPatternVerbFromTr5.Visible=checkBoxPatternVerbFromTransgressivePast.Checked;
            textBoxPatternVerbFromTr6.Visible=checkBoxPatternVerbFromTransgressivePast.Checked;
        }

        void DuplicateToolStripMenuItem2_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounFrom!=null) {
                PatternPronounFromSaveCurrent();
                ItemPatternPronoun item =CurrentPatternPronounFrom.Duplicate();
                itemsPatternPronounFrom.Add(item);
                PatternPronounFromRefreshFilteredList();
                PatternPronounFromSetListBox();
                PatternPronounFromListBoxSetCurrent();
            }
        }

        void aBCToolStripMenuItem3_Click(object sender, EventArgs e) {
            doingJob = true;
            PatternVerbFromSaveCurrent();
            itemsPatternVerbFrom = itemsPatternVerbFrom.OrderBy(a => a.Name).ToList();
            PatternVerbFromRefreshFilteredList();

            PatternVerbFromSetListBox();
            doingJob = false;
        }

        void toolStripMenuItem103_Click(object sender, EventArgs e) {
            doingJob = true;
            PatternVerbToSaveCurrent();
            itemsPatternVerbTo = itemsPatternVerbTo.OrderBy(a => a.Name).ToList();
            PatternVerbToRefreshFilteredList();

            PatternVerbToSetListBox();
            doingJob = false;
        }

        void listBoxPatternNounFrom_KeyDown(object sender, KeyEventArgs e) {
            if (e.Alt){
                if (e.KeyCode==Keys.M) {
                    // From pattternNounFrom to patternnounTo
                    if (CurrentPatternNounFrom == null)   return;
                    ItemPatternNoun PatternNounFrom = CurrentPatternNounFrom;
                    itemsPatternNounTo.Add(PatternNounFrom);
                    itemsPatternNounFrom.Remove(PatternNounFrom);

                    PatternNounToRefreshFilteredList();
                    PatternNounFromRefreshFilteredList();

                    PatternNounFromSetListBox();
                    PatternNounFromSetCurrent();

                    PatternNounToSetListBox();
                    PatternNounToSetCurrent();
                }
            }
        }

        void CopyToolStripMenuItem_Click(object sender, EventArgs e) {
            Clipboard.SetText("TW"+CurrentVersion+"|PRONOUN|"+CurrentPatternPronounFrom.Save());
        }

        void CopyToolStripMenuItem1_Click(object sender, EventArgs e) {
            Clipboard.SetText("TW"+CurrentVersion+"|PRONOUN|"+CurrentPatternPronounTo.Save());
        }

        void pasteToolStripMenuItem1_Click(object sender, EventArgs e) {
            string txt=Clipboard.GetText();
            if (txt.StartsWith("TW"+CurrentVersion+"|PRONOUN|")){
                ItemPatternPronoun item=ItemPatternPronoun.Load(txt.Substring(("TW"+CurrentVersion+"|PRONOUN|").Length));
                if (item!=null){
                    itemsPatternPronounTo.Add(item);
                    PatternPronounToSetListBox();
                }
            }
        }

        void pasteToolStripMenuItem2_Click(object sender, EventArgs e) {
            string txt=Clipboard.GetText();
            if (txt.StartsWith("TW"+CurrentVersion+"|PRONOUN|")){
                ItemPatternPronoun item=ItemPatternPronoun.Load(txt.Substring(("TW"+CurrentVersion+"|PRONOUN|").Length));
                if (item!=null){
                    itemsPatternPronounFrom.Add(item);
                    PatternPronounFromRefreshFilteredList();
                    PatternPronounFromSetListBox();
                }
            }
        }

        void copyToolStripMenuItem3_Click(object sender, EventArgs e) {
           Clipboard.SetText("TW"+CurrentVersion+"|NUMBER|"+CurrentPatternNumberTo.Save());
        }

        void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            string txt=Clipboard.GetText();
            if (txt.StartsWith("TW"+CurrentVersion+"|NUMBER|")){
                ItemPatternNumber item=ItemPatternNumber.Load(txt.Substring(("TW"+CurrentVersion+"|NUMBER|").Length));
                if (item!=null){
                    itemsPatternNumberTo.Add(item);
                    PatternNumberToSetListBox();
                }
            }
        }

        void CopyToolStripMenuItem2_Click(object sender, EventArgs e) {
            Clipboard.SetText("TW"+CurrentVersion+"|NUMBER|"+CurrentPatternNumberFrom.Save());
        }

        void PasteToolStripMenuItem3_Click(object sender, EventArgs e) {
            string txt=Clipboard.GetText();
            if (txt.StartsWith("TW"+CurrentVersion+"|NUMBER|")){
                ItemPatternNumber item=ItemPatternNumber.Load(txt.Substring(("TW"+CurrentVersion+"|NUMBER|").Length));
                if (item!=null) {
                    itemsPatternNumberFrom.Add(item);
                    PatternNumberFromSetListBox();
                }
            }
        }

        void DetekovatZeSeznamuToolStripMenuItem_Click(object sender, EventArgs e) {
            FormGetData f = new FormGetData();
            f.ShowDialog();
            if (f.dr==DialogResult.OK) {
                List<TranslationsList> list=f.translationsList;
                foreach (TranslationsList t in list) {

                    DownloadDataCompletedEventHandler handler=null;

                    try{
                        handler += (sender2, e2) => {
                            if (!e2.Cancelled){
                            var data = e2.Result;
                            string html = Encoding.UTF8.GetString(data);
                            NodeHTML node = Computation.findNode(html,"toc");
                            bool pods=node.FindText("podstatné jméno");
                            bool prid=node.FindText("přídavné jméno");
                            bool zajm=node.FindText("zájmeno");
                            bool cisl=node.FindText("číslovka");
                            bool slov=node.FindText("sloveso");
                            bool pris=node.FindText("příslovce");
                            bool pred=node.FindText("předložka");
                            bool spoj=node.FindText("spojka");
                            bool cast=node.FindText("částice");
                            bool cito=node.FindText("citoslovce");

                            if (pods && !prid && !zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito) {
                                itemsNouns.Add(new ItemNoun{ From=t.From[0], To=new TranslatingToDataWithPattern[]{new TranslatingToDataWithPattern{Body=t.To[0],  Pattern=""} }.ToList()});
                            } else if (!pods && prid && !zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsAdjectives.Add(new ItemAdjective{ From=t.From[0], To=new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body=t.To[0] } } });
                            } else if (!pods && !prid && zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsPronouns.Add(new ItemPronoun{ From=t.From[0],To=new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body=t.To[0]} } });
                            }else if (!pods && !prid && zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsNumbers.Add(new ItemNumber{ From=t.From[0], To=new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body=t.To[0]} } });
                            }else if (!pods && !prid && zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsVerbs.Add(new ItemVerb{ From=t.From[0], To=new TranslatingToDataWithPattern[]{new TranslatingToDataWithPattern{Body=t.To[0],  Pattern=""} }.ToList()});
                            }else if (!pods && !prid && zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsAdverbs.Add(new ItemAdverb{ From=t.From[0], To=new List<TranslatingToData>{new TranslatingToData{Text=t.To[0] } } });
                            }else if (!pods && !prid && zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsPrepositions.Add(new ItemPreposition{ From=t.From[0],To=new List<TranslatingToData>{new TranslatingToData{Text=t.To[0]} } });
                            }else if (!pods && !prid && zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsConjunctions.Add(new ItemConjunction{ From=t.From[0],To=new List<TranslatingToData>{new TranslatingToData{Text=t.To[0]} } });
                            }else if (!pods && !prid && zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsParticles.Add(new ItemParticle{ From=t.From[0],To=new List<TranslatingToData>{new TranslatingToData{Text=t.To[0]} } });
                            }else if (!pods && !prid && zajm && !cisl && !slov && !pris && !pred && !spoj && !cast && !cito){
                                itemsInterjections.Add(new ItemInterjection{ From=t.From[0], To=new List<TranslatingToData>{new TranslatingToData{Text=t.To[0]} } });
                            }else itemsSimpleWords.Add(new ItemSimpleWord{ From=t.From[0], To=new List<TranslatingToData>{new TranslatingToData{Text=t.To[0]} } });

                            NounRefreshFilteredList();
                            NounSetListBox();

                            AdjectiveRefreshFilteredList();
                            AdjectiveSetListBox();

                            PronounRefreshFilteredList();
                            PronounSetListBox();

                            NumberRefreshFilteredList();
                            SetListBoxNumber();

                            VerbRefreshFilteredList();
                            VerbSetListBox();

                            AdverbRefreshFilteredList();
                            SetListBoxAdverb();

                            PrepositionRefreshFilteredList();
                            SetListBoxPreposition();

                            ConjunctionRefreshFilteredList();
                            SetListBoxConjunction();

                            ParticleRefreshFilteredList();
                            SetListBoxParticle();

                            InterjectionRefreshFilteredList();
                            SetListBoxInterjection();

                            SimpleWordRefreshFilteredList();
                            SimpleWordSetListBox();
                            }
                        };
                    }catch{ }

                    Computation.DownloadString(ref handler,t.From[0]);
                }

            }
        }

        void CopyToolStripMenuItem4_Click(object sender, EventArgs e) {
             if (CurrentPatternAdjectiveFrom!=null) Clipboard.SetText("TW"+CurrentVersion+"|ADJECTIVE|"+CurrentPatternAdjectiveFrom.Save());
        }

        void pasteToolStripMenuItem4_Click(object sender, EventArgs e) {
            string txt=Clipboard.GetText();
            if (txt.StartsWith("TW"+CurrentVersion+"|ADJECTIVE|")){
                ItemPatternAdjective item=ItemPatternAdjective.Load(txt.Substring(("TW"+CurrentVersion+"|ADJECTIVE|").Length));
                if (item!=null){
                    itemsPatternAdjectiveFrom.Add(item);
                    PatternAdjectiveFromSetListBox();
                }
            }
        }

        void copyToolStripMenuItem5_Click(object sender, EventArgs e) {
             if (CurrentPatternAdjectiveTo!=null) Clipboard.SetText("TW"+CurrentVersion+"|ADJECTIVE|"+CurrentPatternAdjectiveTo.Save());
        }

        void pasteToolStripMenuItem5_Click(object sender, EventArgs e) {
            string txt=Clipboard.GetText();
            if (txt.StartsWith("TW"+CurrentVersion+"|ADJECTIVE|")){
                ItemPatternAdjective item=ItemPatternAdjective.Load(txt.Substring(("TW"+CurrentVersion+"|ADJECTIVE|").Length));
                if (item!=null){
                    itemsPatternAdjectiveTo.Add(item);
                    PatternAdjectiveToSetListBox();
                }
            }
        }

        void addToolStripMenuItem1_Click_1(object sender, EventArgs e) {
            if (CurrentPatternNounTo!=null){
                PatternNounToSaveCurrent();
                CurrentPatternNounTo.AddQuestionMark();
                PatternNounToSetCurrent();
            }
        }

        void TabControl10_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode==Keys.N && e.Control) {
                switch (tabControl10.SelectedIndex) {
                   case 0:
                        AddNewItemAdverb();
                        break;

                   case 1:
                        AddNewItemPreposition();
                        break;

                   case 2:
                        AddNewItemConjunction();
                        break;

                   case 3:
                        AddNewItemParticle();
                        break;

                   case 4:
                        AddNewItemInterjection();
                        break;

                   default:
                        throw new Exception();
                }
            }
        }

        void tabControl3_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode==Keys.N && e.Control) {
                switch (tabControl3.SelectedIndex) {
                   case 0:
                        AddNewItemSentencePart();
                        break;

                   case 1:
                        AddNewItemSentence();
                        break;

                   case 2:
                        AddNewItemSentencePatternPart();
                        break;

                   case 3:
                        AddNewItemSentencePattern();
                        break;

                   default:
                        throw new Exception();
                }
            }
        }

        void tabControl4_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode==Keys.N && e.Control) {
                switch (tabControl4.SelectedIndex) {
                   case 0:
                        SimpleWordAddNewItem();
                        break;

                   case 1:
                        PhraseAddNewItem();
                        break;

                   case 2:
                        AddNewItemReplaceS();
                        break;

                   case 3:
                        AddNewItemReplaceE();
                        break;

                   case 4:
                        AddNewItemReplaceG();
                        break;

                   default:
                        throw new Exception();
                }
            }
        }

        void tabControl5_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode==Keys.N && e.Control) {
                switch (tabControl5.SelectedIndex) {
                   case 0:
                        AddNewItemNoun();
                        break;

                   case 1:
                        PatternNounFromAddNewItem();
                        break;

                   case 2:
                        PatternNounToAddNewItem();
                        break;

                   default:
                        throw new Exception();
                }
            }
        }

        void TabControl6_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode==Keys.N && e.Control) {
                switch (tabControl6.SelectedIndex) {
                   case 0:
                        AddNewItemAdjective();
                        break;

                   case 1:
                        PatternAdjectiveFromAddNewItem();
                        break;

                   case 2:
                        PatternAdjectiveToAddNewItem();
                        break;

                   default:
                        throw new Exception();
                }
            }
        }

        void tabControl7_KeyDown(object sender, KeyEventArgs e) {
             if (e.KeyCode==Keys.N && e.Control) {
                switch (tabControl7.SelectedIndex) {
                   case 0:
                        AddNewItemPronoun();
                        break;

                   case 1:
                        PatternPronounFromAddNewItem();
                        break;

                   case 2:
                        PatternPronounToAddNewItem();
                        break;

                   default:
                        throw new Exception();
                }
            }
        }

        void tabControl8_KeyDown(object sender, KeyEventArgs e) {
             if (e.KeyCode==Keys.N && e.Control) {
                switch (tabControl8.SelectedIndex) {
                   case 0:
                        AddNewItemNumber();
                        break;

                   case 1:
                        PatternNumberFromAddNewItem();
                        break;

                   case 2:
                        PatternNumberToAddNewItem();
                        break;

                   default:
                        throw new Exception();
                }
            }
        }

        void tabControl9_KeyDown(object sender, KeyEventArgs e) {
             if (e.KeyCode==Keys.N && e.Control) {
                switch (tabControl9.SelectedIndex) {
                   case 0:
                        AddNewItemVerb();
                        break;

                   case 1:
                        PatternVerbFromAddNewItem();
                        break;

                   case 2:
                        PatternVerbToAddNewItem();
                        break;

                   default:
                        throw new Exception();
                }
            }
        }

        void addMarkToolStripMenuItem_Click(object sender, EventArgs e) {
            if (CurrentPatternAdjectiveTo!=null){
                PatternAdjectiveToSaveCurrent();
                CurrentPatternAdjectiveTo.AddQuestionMark();
                PatternAdjectiveToSetCurrent();
            }
        }

        void addMarkToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounTo!=null){
                PatternPronounToSaveCurrent();
                CurrentPatternPronounTo.AddQuestionMark();
                PatternPronounToSetCurrent();
            }
        }

        void addMarkToolStripMenuItem2_Click(object sender, EventArgs e) {
            if (CurrentPatternNumberTo!=null){
                PatternNumberToSaveCurrent();
                CurrentPatternNumberTo.AddQuestionMark();
                PatternNumberToSetCurrent();
            }
        }

        void addMarkToolStripMenuItem3_Click(object sender, EventArgs e) {
            if (CurrentPatternVerbTo!=null) {
                PatternVerbToSaveCurrent();
                CurrentPatternVerbTo.AddQuestionMark();
                PatternVerbToSetCurrent();
            }
        }

        void tenToolStripMenuItem_Click(object sender, EventArgs e) {
            PatternPronounToSaveCurrent();
            itemsPatternPronounTo.Add(ItemPatternPronoun.tEN);
            PatternPronounToRefresh();
        }

        void tENToolStripMenuItem1_Click(object sender, EventArgs e) {
            PatternPronounFromSaveCurrent();
            itemsPatternPronounFrom.Add(ItemPatternPronoun.tEN);
            PatternPronounFromRefresh();
        }

        void toolStripMenuItem23_Click(object sender, EventArgs e) {
            PatternVerbFromSaveCurrent();
            itemsPatternVerbFrom.Add(ItemPatternVerb.BÝT);
            PatternVerbFromRefresh();
        }

        void toolStripMenuItem118_Click(object sender, EventArgs e) {
            SaveCurrentSentence();
            itemsSentences = itemsSentences.OrderBy(a => a.From).ToList();
            SentenceRefreshFilteredList();
            SetListBoxSentence();
        }

        void toolStripMenuItem130_Click(object sender, EventArgs e) {
            SaveCurrentSentencePart();
            itemsSentenceParts = itemsSentenceParts.OrderBy(a => a.From).ToList();
            SentencePartRefreshFilteredList();
            SetListBoxSentencePart();
        }

        void toolStripMenuItem125_Click(object sender, EventArgs e) {
            SaveCurrentSentencePattern();
            itemsSentencePatterns = itemsSentencePatterns.OrderBy(a => a.From).ToList();
            SentencePatternRefreshFilteredList();
            SetListBoxSentencePattern();
        }

        void toolStripMenuItem158_Click(object sender, EventArgs e) {
            SaveCurrentSentencePart();
            itemsSentenceParts = itemsSentenceParts.OrderBy(a => a.From).ToList();
            SentencePartRefreshFilteredList();
            SetListBoxSentencePart();
        }

        void addFromToToolStripMenuItem_Click(object sender, EventArgs e) {
             string name = GetString("", "Název noun");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;

            try{
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    List<Table> tables=new List<Table>();
                    Computation.FindTableInHTML(html, "deklinace substantivum", ref tables);

                    ItemPatternNoun pattern = new ItemPatternNoun {
                        Name = name,
                        Shapes=new string[14]
                    };
                    bool found=false;
                    if (tables.Count>0){
                        Table table=tables[0];
                        if (table.Rows.Count==8) {
                            if (table.Rows[0].Cells[0].Text.ToLower()=="pád \\ číslo") {
                                found=true;
                                for (int i=0; i<7; i++) {
                                    pattern.Shapes[i]=table.Rows[i+1].Cells[1].Text;
                                }
                                for (int i=0; i<7; i++) {
                                    pattern.Shapes[i+7]=table.Rows[i+1].Cells[2].Text;
                                }
                            } else MessageBox.Show("Error, something else,non substantivnum");
                        } else MessageBox.Show("Error, something else,height");
                    } else MessageBox.Show("Error, nic");
                if (!found) return;
                    if (html.Contains("rod střední"))pattern.Gender=GenderNoun.Neuter;
                    else if (html.Contains("rod ženský"))pattern.Gender=GenderNoun.Feminine;
                    else if (html.Contains("rod mužský neživotný"))pattern.Gender=GenderNoun.MasculineInanimate;
                    else if (html.Contains("rod mužský životný"))pattern.Gender=GenderNoun.MasculineAnimal;
                    pattern.Optimize();

                    itemsPatternNounFrom.Add(pattern);
                    ItemPatternNoun patternTo =pattern.Clone();
                    patternTo.AddQuestionMark();
                    itemsPatternNounTo.Add(patternTo);

                    PatternNounFromRefreshFilteredList();
                    PatternNounFromSetListBox();
                    PatternNounFromListBoxSetCurrent();

                    PatternNounToRefreshFilteredList();
                    PatternNounToSetListBox();
                    PatternNounToListBoxSetCurrent();
                };
            } catch { MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);
        }

        void býtToolStripMenuItem_Click(object sender, EventArgs e) {
            PatternVerbFromSaveCurrent();
            itemsPatternVerbFrom.Add(ItemPatternVerb.BÝT);
            PatternVerbFromRefresh();
        }

        private void buttonAddToAdjective_Click(object sender, EventArgs e) {
            simpleUIAdjective.Add("","","");
        }

        private void buttonAddToPronoun_Click(object sender, EventArgs e) {
            simpleUIPronouns.Add("","","");
        }

        private void buttonAddToAdverb_Click(object sender, EventArgs e) {
            simpleUIAdverb.Add("","");
        }

        private void buttonAddToPreposition_Click(object sender, EventArgs e) {
            simpleUIPreposition.Add("","");
        }

        private void buttonAddToConjuction_Click(object sender, EventArgs e) {
            simpleUIConjuction.Add("","");
        }

        private void buttonAddToInterjection_Click(object sender, EventArgs e) {
            simpleUIInterjection.Add("","");
        }

        private void buttonAddToParticle_Click(object sender, EventArgs e) {
            simpleUIParticle.Add("","");
        }

        private void buttonAddToSimpleWord_Click(object sender, EventArgs e) {
            simpleUISimpleWord.Add("", "");
        }

        private void buttonAddToNumber_Click(object sender, EventArgs e) {
            simpleUINumbers.Add("","","");
        }

        private void buttonAddToPhrase_Click(object sender, EventArgs e) {
            simpleUIPhrase.Add("","");
        }

        private void buttonAddToSentencePart_Click(object sender, EventArgs e) {
            simpleUISentencePart.Add("","");
        }

        private void buttonSentence_Click(object sender, EventArgs e) {
            simpleUISentence.Add("","");
        }

        private void buttonPhrasePatternAdd_Click_1(object sender, EventArgs e) {
            PhrasePatternAddNewItem();
        }

        private void duplikovatToolStripMenuItem_Click_1(object sender, EventArgs e) {
            if (CurrentVerb!=null) {
                itemsVerbs.Add(CurrentVerb.Clone());
                VerbRefresh();
            }
        }

        private void aBCToolStripMenuItem1_Click(object sender, EventArgs e) {
            doingJob = true;
            SaveCurrentAdjective();
            itemsAdjectives = itemsAdjectives.OrderBy(a => a.From).ToList();
            AdjectiveRefreshFilteredList();
            AdjectiveSetListBox();
            doingJob = false;
        }

        private void toolStripMenuItem144_Click(object sender, EventArgs e) {
            doingJob = true;
            ReplaceGSaveCurrent();
            itemsReplaceG = itemsReplaceG.OrderBy(a => a.From).ToList();
            ReplaceGRefreshFilteredList();
            ReplaceGSetListBox();
            doingJob = false;
        }

        private void toolStripMenuItem151_Click(object sender, EventArgs e) {
            doingJob = true;
            SaveCurrentReplaceE();
            itemsReplaceE = itemsReplaceE.OrderBy(a => a.From).ToList();
            ReplaceERefreshFilteredList();
            ReplaceESetListBox();
            doingJob = false;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e) {            
            doingJob = true;
            PronounSaveCurrent();
            itemsPronouns = itemsPronouns.OrderBy(a => a.From).ToList();
            PronounRefreshFilteredList();
            PronounSetListBox();
            doingJob = false;
        }

        private void aBCEndingToolStripMenuItem_Click(object sender, EventArgs e) {
            doingJob = true;
            SaveCurrentReplaceE();
            itemsReplaceE = itemsReplaceE.OrderBy(a => ReverseString(a.From)).ToList();
            ReplaceERefreshFilteredList();
            ReplaceESetListBox();
            doingJob = false;

            string ReverseString(string str) {                 
                char[] charArray = str.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
        }

        private void toolStripMenuItem81_Click(object sender, EventArgs e) {        
            doingJob = true;
            PatternPronounToSaveCurrent();
            itemsPatternPronounTo = itemsPatternPronounTo.OrderBy(a => a.Name).ToList();
            PatternPronounToRefreshFilteredList();
            PatternPronounToSetListBox();
            doingJob = false;
        }

        void addFromToToolStripMenuItem1_Click(object sender, EventArgs e) {
            string name = GetString("", "Název pronomenu");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;

            try{
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    List<Table> tables=new List<Table>();
                    Computation.FindTableInHTML(html, "deklinace pronomen", ref tables);

                    if (tables.Count>=1) {
                        Table table=tables[0];
                        if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
                            ItemPatternPronoun pattern = new ItemPatternPronoun {
                                Name = name,
                                Type = PronounType.DeklinationWithGender,
                                Shapes = new string[8*7]
                            };

                            for (int r=0; r<7; r++) pattern.Shapes[r]=table.Rows[2+r].Cells[1+0].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*2]=table.Rows[2+r].Cells[1+1].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*4]=table.Rows[2+r].Cells[1+2].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*6]=table.Rows[2+r].Cells[1+3].Text;

                            for (int r=0; r<7; r++) pattern.Shapes[r+7*1]=table.Rows[2+r].Cells[1+4].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*3]=table.Rows[2+r].Cells[1+5].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*5]=table.Rows[2+r].Cells[1+6].Text;
                            for (int r=0; r<7; r++) pattern.Shapes[r+7*7]=table.Rows[2+r].Cells[1+7].Text;

                            pattern.Optimize();
                            itemsPatternPronounFrom.Add(pattern);
                            PatternPronounFromRefresh();

                            ItemPatternPronoun pattern2=pattern.Clone();
                            pattern2.AddQuestionMark();
                            itemsPatternPronounTo.Add(pattern2);

                            PatternPronounToRefresh();
                        } else if (table.Rows.Count==8 && table.Rows[3].Cells.Count==2) {
                            ItemPatternPronoun pattern = new ItemPatternPronoun {
                                Name = name,
                                Type = PronounType.DeklinationOnlySingle,
                                Shapes = new string[7]
                            };
                            for (int c=0; c<7; c++) {
                                pattern.Shapes[c]=table.Rows[1+c].Cells[1].Text;
                            }

                            pattern.Optimize();
                            itemsPatternPronounFrom.Add(pattern);

                            PatternPronounFromRefresh();

                            ItemPatternPronoun pattern2=pattern.Clone();
                            pattern2.AddQuestionMark();
                            itemsPatternPronounTo.Add(pattern2);

                            PatternPronounToRefresh();
                        }
                    }
                };
            }catch{ MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);
        }

        void addToolStripMenuItem7_Click(object sender, EventArgs e) {

        }

        private void addStartingStringToolStripMenuItem2_Click(object sender, EventArgs e) {
            if (CurrentPatternNounTo!=null) {
                FormString form=new FormString();
                form.LabelText="Přidat na začátek pronoma string";
                form.ShowDialog();
                string str=form.ReturnString;
                if (!string.IsNullOrEmpty(str)) {
                    PatternNounToSaveCurrent();
                    CurrentPatternNounTo.AddStartingString(str);
                    PatternNounToSetCurrent();
                }
            }
        }

        private void addStartingStringToolStripMenuItem3_Click(object sender, EventArgs e) {
            if (CurrentPatternAdjectiveTo!=null) {
                PatternAdjectiveToSaveCurrent();
                FormString form=new FormString();
                form.LabelText="Přidat na začátek pronoma string";
                form.ShowDialog();
                string str=form.ReturnString;
                if (!string.IsNullOrEmpty(str)) CurrentPatternAdjectiveTo.AddStartingString(str);
                PatternAdjectiveToSetCurrent();
            }
        }

        private void toolStripMenuItem54_Click(object sender, EventArgs e) {
            if (CurrentPatternAdjectiveTo!=null){
                PatternAdjectiveToSaveCurrent();

                ItemPatternAdjective item =CurrentPatternAdjectiveTo.Duplicate();

                itemsPatternAdjectiveTo.Add(item);
                PatternAdjectiveToRefreshFilteredList();
                PatternAdjectiveToSetListBox();
                PatternAdjectiveToSetCurrent();
            }
        }

        private void AddLinkedFromtoToolStripMenuItem3_Click(object sender, EventArgs e) {
            string name = GetString("", "Název substantiva");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;
            #if !DEBUG
            try{
            #endif
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    ItemPatternNoun pattern = ParseItemWikidirectonary.ItemPatternNoun(html, out string error, name);
                    if (!string.IsNullOrEmpty(error)) MessageBox.Show(error);
                    if (pattern!=null) {
                        ItemPatternNoun patternTo=pattern.Clone();

                        pattern.Optimize();
                        itemsPatternNounFrom.Add(pattern);
                        PatternNounFromRefresh();

                        patternTo.ConvertToPhonetics();
                        patternTo.Optimize();
                        patternTo.AddQuestionMark();

                        itemsPatternNounTo.Add(patternTo);

                        PatternNounToRefresh();

                        ItemNoun num = new ItemNoun {
                            PatternFrom = pattern.Name,
                            From = pattern.GetPrefix(),

                            To = new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body=patternTo.GetPrefix(), Pattern=patternTo.Name } },
                        };
                        itemsNouns.Add(num);

                        NounRefresh();
                    }
                };
            #if !DEBUG
            } catch { MessageBox.Show("Error");}
            #endif
            Computation.DownloadString(ref handler, name);
        }

        private void AddStartingStringToolStripMenuItem4_Click(object sender, EventArgs e) {
            if (CurrentPatternVerbTo!=null) {
                PatternVerbToSaveCurrent();
                FormString form=new FormString();
                form.LabelText="Přidat na začátek verba string";
                form.ShowDialog();
                string str=form.ReturnString;
                if (!string.IsNullOrEmpty(str)) CurrentPatternVerbTo.AddStartingString(str);
                PatternVerbToSetCurrent();
            }
        }

        private void toolStripMenuItemNumberAdd_Click(object sender, EventArgs e) {

        }

        private void addStartingStringToolStripMenuItem5_Click(object sender, EventArgs e) {
             if (CurrentPatternNumberTo!=null) {
                PatternNumberToSaveCurrent();
                FormString form=new FormString();
                form.LabelText="Přidat na začátek pronoma string";
                form.ShowDialog();
                string str=form.ReturnString;
                if (!string.IsNullOrEmpty(str)) CurrentPatternNumberTo.AddStartingString(str);
                PatternNumberToSetCurrent();
            }
        }

        private void tabControl1_Resize(object sender, EventArgs e) {
            textBoxComment.Height=tabPage41.Height-textBoxComment.Location.Y;
        }

        private void toolStripMenuItem23_Click_1(object sender, EventArgs e) {
            doingJob = true;
            SaveCurrentAdverb();
            itemsAdverbs = itemsAdverbs.OrderBy(a => a.From).ToList();
            AdverbRefreshFilteredList();
            SetListBoxAdverb();
            doingJob = false;
        }

        void addFromToToolStripMenuItem2_Click(object sender, EventArgs e) {
            string name = GetString("", "Název verb");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;
            #if !DEBUG
            try{
            #endif
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    List<Table> tables=new List<Table>();
                    Computation.FindTableInHTML(html, "konjugace verbum", ref tables);

                    ItemPatternVerb pattern = new ItemPatternVerb {
                        Name = name,
                        Infinitive = name
                    };
                    if (Computation.FindTableInListByName(tables, "Oznamovací způsob", out Table ozn)) {
                        Row rowPrit=Computation.GetRowByFirstCellText(ozn, "přítomný čas");
                        Row rowBud=Computation.GetRowByFirstCellText(ozn, "budoucí čas");
                        if (/*ozn.Rows[2].Cells[0].Text=="Přítomný čas"*/rowPrit!=null) {
                            pattern.SContinous=true;
                            pattern.Continous=new string[6];
                            for (int i=0; i<6; i++) {
                                pattern.Continous[i]=/*ozn.Rows[2]*/rowPrit.Cells[1+i].Text;
                            }
                        }
                        if (/*ozn.Rows.Count==4*/rowBud!=null) {
                          //  if (ozn.Rows[2].Cells[0].Text=="Budoucí čas") {
                                pattern.SFuture=true;
                                pattern.Future=new string[6];
                                for (int i=0; i<6; i++) {
                                    pattern.Future[i]=/*ozn.Rows[3]*/rowBud.Cells[1+i].Text;
                                }
                           // }
                        }
                    }else{
                        MessageBox.Show("Špatné jméno");
                        return;
                    }

                    if (Computation.FindTableInListByName(tables, "Rozkazovací způsob", out Table roz)) {
                        pattern.Imperative=new string[3];
                        pattern.SImperative=true;
                        for (int i=0; i<3; i++) {
                            pattern.Imperative[i]=roz.Rows[2].Cells[1+i].Text;
                        }
                    }

                if (Computation.FindTableInListByName(tables, "Příčestí", out Table tr)) {
                    Row rowC=Computation.GetRowByFirstCellText(tr, "činné");
                    if (/*tr.Rows[2].Cells[0].Text=="Činné"*/rowC!=null) {
                        pattern.SPastActive=true;
                        pattern.PastActive=new string[8];
                        if (/*tr.Rows[2]*/rowC.Cells.Count==7){
                            pattern.PastActive[0]=/*tr.Rows[2]*/rowC.Cells[1].Text;
                            pattern.PastActive[1]=/*tr.Rows[2]*/rowC.Cells[1].Text;
                            pattern.PastActive[2]=/*tr.Rows[2]*/rowC.Cells[2].Text;
                            pattern.PastActive[3]=/*tr.Rows[2]*/rowC.Cells[3].Text;
                            pattern.PastActive[4]=/*tr.Rows[2]*/rowC.Cells[4].Text;
                            pattern.PastActive[5]=/*tr.Rows[2]*/rowC.Cells[5].Text;
                            pattern.PastActive[6]=/*tr.Rows[2]*/rowC.Cells[5].Text;
                            pattern.PastActive[7]=/*tr.Rows[2]*/rowC.Cells[6].Text;
                        }else{
                            for (int i=0; i<8; i++) {
                                pattern.PastActive[i]=/*tr.Rows[2]*/rowC.Cells[1+i].Text;
                            }
                        }
                    }
                    Row rowT=Computation.GetRowByFirstCellText(tr, "trpné");
                    if (/*tr.Rows[2].Cells[0].Text=="Trpné"*/rowT!=null) {
                        pattern.SPastPassive=true;
                        pattern.PastPassive=new string[8];
                        if (/*tr.Rows[2]*/rowT.Cells.Count==7) {
                            pattern.PastPassive[0]=/*tr.Rows[2]*/rowT.Cells[1].Text;
                            pattern.PastPassive[1]=/*tr.Rows[2]*/rowT.Cells[1].Text;
                            pattern.PastPassive[2]=/*tr.Rows[2]*/rowT.Cells[2].Text;
                            pattern.PastPassive[3]=/*tr.Rows[2]*/rowT.Cells[3].Text;
                            pattern.PastPassive[4]=/*tr.Rows[2]*/rowT.Cells[4].Text;
                            pattern.PastPassive[5]=/*tr.Rows[2]*/rowT.Cells[5].Text;
                            pattern.PastPassive[6]=/*tr.Rows[2]*/rowT.Cells[5].Text;
                            pattern.PastPassive[7]=/*tr.Rows[2]*/rowT.Cells[6].Text;
                        }else{
                            for (int i=0; i<8; i++) {
                                pattern.PastPassive[i]=/*tr.Rows[2]*/rowT.Cells[1+i].Text;
                            }
                        }
                    }
                    //if (tr.Rows.Count>=4) {
                    //    if (tr.Rows[3].Cells[0].Text=="Činné") {
                    //        pattern.SPastActive=true;
                    //        pattern.PastActive=new string[8];
                    //        if (tr.Rows[2].Cells.Count==7) {
                    //            pattern.PastActive[0]=tr.Rows[3].Cells[1].Text;
                    //            pattern.PastActive[1]=tr.Rows[3].Cells[1].Text;
                    //            pattern.PastActive[2]=tr.Rows[3].Cells[2].Text;
                    //            pattern.PastActive[3]=tr.Rows[3].Cells[3].Text;
                    //            pattern.PastActive[4]=tr.Rows[3].Cells[4].Text;
                    //            pattern.PastActive[5]=tr.Rows[3].Cells[5].Text;
                    //            pattern.PastActive[6]=tr.Rows[3].Cells[5].Text;
                    //            pattern.PastActive[7]=tr.Rows[3].Cells[6].Text;
                    //        } else {
                    //            for (int i=0; i<8; i++) {
                    //                pattern.PastActive[i]=tr.Rows[3].Cells[1+i].Text;
                    //            }
                    //        }
                    //    }
                    //    if (tr.Rows[3].Cells[0].Text=="Trpné") {
                    //        pattern.SPastPassive=true;
                    //        pattern.PastPassive=new string[8];
                    //        if (tr.Rows[2].Cells.Count==7) {
                    //            pattern.PastPassive[0]=tr.Rows[3].Cells[1].Text;
                    //            pattern.PastPassive[1]=tr.Rows[3].Cells[1].Text;
                    //            pattern.PastPassive[2]=tr.Rows[3].Cells[2].Text;
                    //            pattern.PastPassive[3]=tr.Rows[3].Cells[3].Text;
                    //            pattern.PastPassive[4]=tr.Rows[3].Cells[4].Text;
                    //            pattern.PastPassive[5]=tr.Rows[3].Cells[5].Text;
                    //            pattern.PastPassive[6]=tr.Rows[3].Cells[5].Text;
                    //            pattern.PastPassive[7]=tr.Rows[3].Cells[6].Text;
                    //        } else {
                    //            for (int i=0; i<8; i++) {
                    //                pattern.PastPassive[i]=tr.Rows[3].Cells[1+i].Text;
                    //            }
                    //        }
                    //    }
                   // }
                }

                if (Computation.FindTableInListByName(tables, "Přechodníky", out Table pre)) {
                    if (pre.Rows.Count>=4) {
                        if (pre.Rows[2].Cells[0].Text=="Přítomný") {
                            pattern.TransgressiveCont=new string[3];
                            pattern.STransgressiveCont=true;
                            for (int i=0; i<3; i++) pattern.TransgressiveCont[i]=pre.Rows[2].Cells[1+i].Text;
                        } else if (pre.Rows[2].Cells[0].Text=="Minulý") {
                            pattern.TransgressivePast=new string[3];
                            pattern.STransgressivePast=true;
                            for (int i=0; i<3; i++) pattern.TransgressivePast[i]=pre.Rows[2].Cells[1+i].Text;
                        }

                        if (pre.Rows[3].Cells[0].Text=="Přítomný") {
                            pattern.TransgressiveCont=new string[3];
                            pattern.STransgressiveCont=true;
                            for (int i=0; i<3; i++) pattern.TransgressiveCont[i]=pre.Rows[3].Cells[1+i].Text;
                         } else if (pre.Rows[3].Cells[0].Text=="Minulý") {
                            pattern.TransgressivePast=new string[3];
                            pattern.STransgressivePast=true;
                            for (int i=0; i<3; i++) pattern.TransgressivePast[i]=pre.Rows[3].Cells[1+i].Text;
                        }
                    }
                }

                pattern.Optimize();

                itemsPatternVerbFrom.Add(pattern);
                PatternVerbFromRefresh();

                ItemPatternVerb pattern2 =pattern.Clone();
                pattern2.AddQuestionMark();

                itemsPatternVerbTo.Add(pattern2);
                PatternVerbToRefresh();

            };
            #if !DEBUG
            }catch{ MessageBox.Show("Error");}
            #endif
            Computation.DownloadString(ref handler, name);
        }

        void PatternVerbToRefresh(){
            PatternVerbToRefreshFilteredList();
            PatternVerbToCheckBoxesSet();
            PatternVerbToSetListBox();
            PatternVerbToListBoxSetCurrent();
        }

        private void buttonVerbAddTo_Click(object sender, EventArgs e) {
            if (CurrentVerb!=null) {
                simpleUIVerbs.Add("","","");
            }
        }

        void VerbRefresh(){
            VerbRefreshFilteredList();
            VerbSetListBox();
            VerbListBoxSetCurrent();
        }

        void PatternVerbFromRefresh() {
            PatternVerbFromRefreshFilteredList();
            PatternVerbFromCheckBoxesSet();
            PatternVerbFromSetListBox();
            PatternVerbFromListBoxSetCurrent();
        }

        void addFromtoToolStripMenuItem3_Click(object sender, EventArgs e) {
            string name = GetString("", "Název numery");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;

            try {
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    ItemPatternNumber pattern = ParseItemWikidirectonary.ItemPatternNumber(html, out string error, name);
                    if (!string.IsNullOrEmpty(error)) MessageBox.Show(error);
                    if (pattern!=null) {
                        AddNumber(pattern);
                    }
                };
            }catch{ MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);


            void AddNumber(ItemPatternNumber pattern) {
                itemsPatternNumberFrom.Add(pattern);
                PatternNumberFromRefresh();

                ItemPatternNumber pattern2 = pattern.Clone();
                pattern2.AddQuestionMark();
                itemsPatternNumberTo.Add(pattern2);
                PatternNumberToRefresh();
            }

            //                itemsPatternNumberFrom.Add(pattern);
            //                PatternNumberFromRefreshFilteredList();
            //                PatternNumberFromSetListBox();
            //                PatternNumberFromListBoxSetCurrent();
            //try{
            //    handler += (sender2, e2) => {
            //        if (e2.Error!=null) {
            //            MessageBox.Show("Error "+e2.Error.Message);
            //            return;
            //        }
            //        var data = e2.Result;
            //        string html = Encoding.UTF8.GetString(data);

            //        List<Table> tables=new List<Table>();
            //        Computation.FindTableInHTML(html, "deklinace numerale", ref tables);

            //        if (tables.Count>=1) {
            //            Table table=tables[0];
            //            if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
            //                ItemPatternNumber pattern = new ItemPatternNumber {
            //                    Name = name,
            //                    ShowType = NumberType.DeklinationWithGender,
            //                    Shapes = new string[8*7]
            //                };

            //                for (int r=0; r<7; r++) pattern.Shapes[r]=table.Rows[2+r].Cells[1+0].Text;
            //                for (int r=0; r<7; r++) pattern.Shapes[r+7*2]=table.Rows[2+r].Cells[1+1].Text;
            //                for (int r=0; r<7; r++) pattern.Shapes[r+7*4]=table.Rows[2+r].Cells[1+2].Text;
            //                for (int r=0; r<7; r++) pattern.Shapes[r+7*6]=table.Rows[2+r].Cells[1+3].Text;

            //                for (int r=0; r<7; r++) pattern.Shapes[r+7*1]=table.Rows[2+r].Cells[1+4].Text;
            //                for (int r=0; r<7; r++) pattern.Shapes[r+7*3]=table.Rows[2+r].Cells[1+5].Text;
            //                for (int r=0; r<7; r++) pattern.Shapes[r+7*5]=table.Rows[2+r].Cells[1+6].Text;
            //                for (int r=0; r<7; r++) pattern.Shapes[r+7*7]=table.Rows[2+r].Cells[1+7].Text;

            //                pattern.Optimize();
            //                itemsPatternNumberFrom.Add(pattern);
            //                PatternNumberFromRefresh();

            //                ItemPatternNumber pattern2=pattern.Clone();
            //                pattern2.AddQuestionMark();
            //                itemsPatternNumberTo.Add(pattern2);

            //                PatternNumberToRefresh();
            //            } else if (table.Rows.Count==8 && table.Rows[3].Cells.Count==2) {
            //                ItemPatternNumber pattern = new ItemPatternNumber {
            //                    Name = name,
            //                    ShowType = NumberType.DeklinationOnlySingle,
            //                    Shapes = new string[7]
            //                };
            //                for (int c=0; c<7; c++) {
            //                    pattern.Shapes[c]=table.Rows[1+c].Cells[1].Text;
            //                }

            //                pattern.Optimize();
            //                itemsPatternNumberFrom.Add(pattern);

            //                PatternNumberFromRefresh();

            //                ItemPatternNumber pattern2=pattern.Clone();
            //                pattern2.AddQuestionMark();
            //                itemsPatternNumberTo.Add(pattern2);

            //                PatternNumberToRefresh();
            //            }
            //        }
            //    };
            //}catch{ MessageBox.Show("Error");}
            //Computation.DownloadString(ref handler, name);
        }

        void addLinkedFromtoToolStripMenuItem_Click(object sender, EventArgs e) {
            string name = GetString("", "Název numery");
            if (name == null) return;
            if (name=="dvě" || name=="dva") {
                AddNumber(ItemPatternNumber.Dve());
                return;
            }
            DownloadDataCompletedEventHandler handler=null;

            try {
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    ItemPatternNumber pattern = ParseItemWikidirectonary.ItemPatternNumber(html, out string error, name);
                    if (!string.IsNullOrEmpty(error)) MessageBox.Show(error);
                    if (pattern!=null) {
                        AddNumber(pattern);
                    }
                };
            } catch { MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);


            void AddNumber(ItemPatternNumber pattern) {
                ItemPatternNumber pattern2=pattern.Clone();

                pattern.Optimize();
                itemsPatternNumberFrom.Add(pattern);
                PatternNumberFromRefresh();

                pattern2.ConvertToPhonetics();
                pattern2.Optimize();
                pattern2.AddQuestionMark();
                itemsPatternNumberTo.Add(pattern2);

                PatternNumberToRefresh();

                ItemNumber num = new ItemNumber {
                    From = pattern.GetPrefix(),
                    PatternFrom = pattern.Name,
                    To =new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{Body=pattern2.GetPrefix(), Pattern = pattern2.Name} }
                };
                itemsNumbers.Add(num);

                NumberRefresh();
            }
        }

        void tENToolStripMenuItem2_Click(object sender, EventArgs e) {
            PronounSaveCurrent();
            ItemPronoun pronoun = ItemPronoun.tEN;

            if (!itemsPatternPronounFrom.ExistsWithName("tEN")) {//exist
                itemsPatternPronounFrom.Add(ItemPatternPronoun.tEN);
                PatternPronounFromRefresh();

                ItemPatternPronoun v= ItemPatternPronoun.tEN;
                v.AddQuestionMark();
                itemsPatternPronounTo.Add(v);
                PatternPronounToRefresh();
            }

            itemsPronouns.Add(pronoun);
            PronounRefresh();
        }

        void PronounRefresh(){
            PronounRefreshFilteredList();
            PronounSetListBox();
            SetCurrentPronoun();
        }

        void ListBoxPronoun_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) contextMenuStripPronoun.Show(MousePosition);
        }

        void addLinkedFromtoToolStripMenuItem1_Click(object sender, EventArgs e) {
            string name = GetString("", "Název pronomenu");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;

            try{
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    List<Table> tables=new List<Table>();
                    ItemPatternPronoun pattern = ParseItemWikidirectonary.ItemPatternPronoun(html, out string error, name);
                    if (pattern!=null){
                    //Computation.FindTableInHTML(html, "deklinace pronomen", ref tables);

                    //if (tables.Count>=1) {
                    //    Table table=tables[0];
                    //    if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
                    //        ItemPatternPronoun pattern = new ItemPatternPronoun {
                    //            Name = name,
                    //            Type = PronounType.DeklinationWithGender,
                    //            Shapes = new string[8*7]
                    //        };

                    //        for (int r=0; r<7; r++) pattern.Shapes[r]=table.Rows[2+r].Cells[1+0].Text;
                    //        for (int r=0; r<7; r++) pattern.Shapes[r+7*2]=table.Rows[2+r].Cells[1+1].Text;
                    //        for (int r=0; r<7; r++) pattern.Shapes[r+7*4]=table.Rows[2+r].Cells[1+2].Text;
                    //        for (int r=0; r<7; r++) pattern.Shapes[r+7*6]=table.Rows[2+r].Cells[1+3].Text;

                    //        for (int r=0; r<7; r++) pattern.Shapes[r+7*1]=table.Rows[2+r].Cells[1+4].Text;
                    //        for (int r=0; r<7; r++) pattern.Shapes[r+7*3]=table.Rows[2+r].Cells[1+5].Text;
                    //        for (int r=0; r<7; r++) pattern.Shapes[r+7*5]=table.Rows[2+r].Cells[1+6].Text;
                    //        for (int r=0; r<7; r++) pattern.Shapes[r+7*7]=table.Rows[2+r].Cells[1+7].Text;

                    //        pattern.Optimize();
                    //        itemsPatternPronounFrom.Add(pattern);
                    //        PatternPronounFromRefresh();

                    //        ItemPatternPronoun pattern2=pattern.Clone();
                    //        pattern2.AddQuestionMark();
                    //        itemsPatternPronounTo.Add(pattern2);
                    //        PatternPronounToRefresh();

                    //        ItemPronoun pronoun = new ItemPronoun {
                    //            From=pattern.GetPrefix(),
                    //            PatternFrom = pattern.Name,
                    //            To=new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{ Body=pattern2.GetPrefix(),  Pattern = pattern2.Name} }
                    //        };

                    //        Edited=true;
                    //        itemsPronouns.Add(pronoun);
                    //        PronounRefresh();
                    //    } else if (table.Rows.Count==8 && table.Rows[3].Cells.Count==2) {
                    //        ItemPatternPronoun pattern = new ItemPatternPronoun {
                    //            Name = name,
                    //            Type = PronounType.DeklinationOnlySingle,
                    //            Shapes = new string[7]
                    //        };
                    //        for (int c=0; c<7; c++) {
                    //            pattern.Shapes[c]=table.Rows[1+c].Cells[1].Text;
                    //        }
                            ItemPatternPronoun pattern2=pattern.Clone();

                            pattern.Optimize();
                            itemsPatternPronounFrom.Add(pattern);
                            PatternPronounFromRefresh();

                            pattern2.ConvertToPhonetics();
                            pattern2.Optimize();
                            pattern2.AddQuestionMark();

                            itemsPatternPronounTo.Add(pattern2);
                            PatternPronounToRefresh();

                            ItemPronoun pronoun = new ItemPronoun {
                                PatternFrom = pattern.Name,
                                From=pattern.GetPrefix(),
                                To=new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{Body=pattern2.GetPrefix(), Pattern = pattern2.Name} }
                            };
                            itemsPronouns.Add(pronoun);
                            Edited=true;
                            PronounRefresh();

                        }
                    //}
                };
            }catch{ MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);
        }

        void addFromtoToolStripMenuItem4_Click(object sender, EventArgs e) {
            string name = GetString("", "Název verb");
            if (name == null) return;

            DownloadDataCompletedEventHandler handler=null;

            try{
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    ItemPatternVerb pattern=ParseItemWikidirectonary.ItemPatternVerb(html, out string error, name);

                //    List<Table> tables=new List<Table>();
                //    Computation.FindTableInHTML(html, "konjugace verbum", ref tables);

                //    ItemPatternVerb pattern = new ItemPatternVerb {
                //        Name = name,
                //        Infinitive = name
                //    };
                //    if (Computation.FindTableInListByName(tables, "Oznamovací způsob", out Table ozn)) {
                //        Row rowPrit=Computation.GetRowByFirstCellText(ozn, "přítomný čas");
                //        if (rowPrit!=null/*ozn.Rows[2].Cells[0].Text=="Přítomný čas"*/) {
                //            pattern.SContinous=true;
                //            pattern.Continous=new string[6];
                //            for (int i=0; i<6; i++) {
                //                pattern.Continous[i]=rowPrit/*ozn.Rows[2]*/.Cells[1+i].Text;
                //            }
                //        }
                //        Row rowBud=Computation.GetRowByFirstCellText(ozn, "budoucí čas");
                //        if (rowBud!=null/*ozn.Rows.Count==4*/){
                //          //  if (ozn.Rows[3].Cells[0].Text=="Budoucí čas") {
                //                pattern.SFuture=true;
                //                pattern.Future=new string[6];
                //                for (int i=0; i<6; i++) {
                //                    pattern.Future[i]=/*ozn.Rows[3]*/rowBud.Cells[1+i].Text;
                //                }
                //           // }
                //        }
                //    }else{
                //        MessageBox.Show("Špatné jméno");
                //        return;
                //    }

                //    if (Computation.FindTableInListByName(tables, "Rozkazovací způsob", out Table roz)) {
                //        pattern.Imperative=new string[3];
                //        pattern.SImperative=true;
                //        for (int i=0; i<3; i++) {
                //            pattern.Imperative[i]=roz.Rows[2].Cells[1+i].Text;
                //        }
                //    }

                //if (Computation.FindTableInListByName(tables, "Příčestí", out Table tr)) {
                //    Row rowCin=Computation.GetRowByFirstCellText(tr, "činné");
                //    Row rowTrp=Computation.GetRowByFirstCellText(tr, "trpné");
                //    if (/*tr.Rows[2].Cells[0].Text=="Činné"*/rowCin!=null) {
                //        pattern.SPastActive=true;
                //        pattern.PastActive=new string[8];
                //        if (/*tr.Rows[2]*/rowCin.Cells.Count==7){
                //            pattern.PastActive[0]=/*tr.Rows[2]*/rowCin.Cells[1].Text;
                //            pattern.PastActive[1]=/*tr.Rows[2]*/rowCin.Cells[1].Text;
                //            pattern.PastActive[2]=/*tr.Rows[2]*/rowCin.Cells[2].Text;
                //            pattern.PastActive[3]=/*tr.Rows[2]*/rowCin.Cells[3].Text;
                //            pattern.PastActive[4]=/*tr.Rows[2]*/rowCin.Cells[4].Text;
                //            pattern.PastActive[5]=/*tr.Rows[2]*/rowCin.Cells[5].Text;
                //            pattern.PastActive[6]=/*tr.Rows[2]*/rowCin.Cells[5].Text;
                //            pattern.PastActive[7]=/*tr.Rows[2]*/rowCin.Cells[6].Text;
                //        }else{
                //            for (int i=0; i<8; i++) {
                //                pattern.PastActive[i]=/*tr.Rows[2]*/rowCin.Cells[1+i].Text;
                //            }
                //        }
                //    }
                //    if (/*tr.Rows[2].Cells[0].Text=="Trpné"*/rowTrp!=null) {
                //        pattern.SPastPassive=true;
                //        pattern.PastPassive=new string[8];
                //        if (/*tr.Rows[2]*/rowTrp.Cells.Count==7) {
                //            pattern.PastPassive[0]=/*tr.Rows[2]*/rowTrp.Cells[1].Text;
                //            pattern.PastPassive[1]=/*tr.Rows[2]*/rowTrp.Cells[1].Text;
                //            pattern.PastPassive[2]=/*tr.Rows[2]*/rowTrp.Cells[2].Text;
                //            pattern.PastPassive[3]=/*tr.Rows[2]*/rowTrp.Cells[3].Text;
                //            pattern.PastPassive[4]=/*tr.Rows[2]*/rowTrp.Cells[4].Text;
                //            pattern.PastPassive[5]=/*tr.Rows[2]*/rowTrp.Cells[5].Text;
                //            pattern.PastPassive[6]=/*tr.Rows[2]*/rowTrp.Cells[5].Text;
                //            pattern.PastPassive[7]=/*tr.Rows[2]*/rowTrp.Cells[6].Text;
                //        }else{
                //            for (int i=0; i<8; i++) {
                //                pattern.PastPassive[i]=/*tr.Rows[2]*/rowTrp.Cells[1+i].Text;
                //            }
                //        }
                //    }
                //    //if (tr.Rows.Count>=4) {
                //    //    if (tr.Rows[3].Cells[0].Text=="Činné") {
                //    //        pattern.SPastActive=true;
                //    //        pattern.PastActive=new string[8];
                //    //        if (tr.Rows[2].Cells.Count==7) {
                //    //            pattern.PastActive[0]=tr.Rows[3].Cells[1].Text;
                //    //            pattern.PastActive[1]=tr.Rows[3].Cells[1].Text;
                //    //            pattern.PastActive[2]=tr.Rows[3].Cells[2].Text;
                //    //            pattern.PastActive[3]=tr.Rows[3].Cells[3].Text;
                //    //            pattern.PastActive[4]=tr.Rows[3].Cells[4].Text;
                //    //            pattern.PastActive[5]=tr.Rows[3].Cells[5].Text;
                //    //            pattern.PastActive[6]=tr.Rows[3].Cells[5].Text;
                //    //            pattern.PastActive[7]=tr.Rows[3].Cells[6].Text;
                //    //        } else {
                //    //            for (int i=0; i<8; i++) {
                //    //                pattern.PastActive[i]=tr.Rows[3].Cells[1+i].Text;
                //    //            }
                //    //        }
                //    //    }
                //    //    if (tr.Rows[3].Cells[0].Text=="Trpné") {
                //    //        pattern.SPastPassive=true;
                //    //        pattern.PastPassive=new string[8];
                //    //        if (tr.Rows[2].Cells.Count==7) {
                //    //            pattern.PastPassive[0]=tr.Rows[3].Cells[1].Text;
                //    //            pattern.PastPassive[1]=tr.Rows[3].Cells[1].Text;
                //    //            pattern.PastPassive[2]=tr.Rows[3].Cells[2].Text;
                //    //            pattern.PastPassive[3]=tr.Rows[3].Cells[3].Text;
                //    //            pattern.PastPassive[4]=tr.Rows[3].Cells[4].Text;
                //    //            pattern.PastPassive[5]=tr.Rows[3].Cells[5].Text;
                //    //            pattern.PastPassive[6]=tr.Rows[3].Cells[5].Text;
                //    //            pattern.PastPassive[7]=tr.Rows[3].Cells[6].Text;
                //    //        } else {
                //    //            for (int i=0; i<8; i++) {
                //    //                pattern.PastPassive[i]=tr.Rows[3].Cells[1+i].Text;
                //    //            }
                //    //        }
                //    //    }
                //    //}
                //}

                //if (Computation.FindTableInListByName(tables, "Přechodníky", out Table pre)) {
                //    if (pre.Rows.Count>=4) {
                //        if (pre.Rows[2].Cells[0].Text=="Přítomný") {
                //            pattern.TransgressiveCont=new string[3];
                //            pattern.STransgressiveCont=true;
                //            for (int i=0; i<3; i++) pattern.TransgressiveCont[i]=pre.Rows[2].Cells[1+i].Text;
                //        } else if (pre.Rows[2].Cells[0].Text=="Minulý") {
                //            pattern.TransgressivePast=new string[3];
                //            pattern.STransgressivePast=true;
                //            for (int i=0; i<3; i++) pattern.TransgressivePast[i]=pre.Rows[2].Cells[1+i].Text;
                //        }

                //        if (pre.Rows[3].Cells[0].Text=="Přítomný") {
                //            pattern.TransgressiveCont=new string[3];
                //            pattern.STransgressiveCont=true;
                //            for (int i=0; i<3; i++) pattern.TransgressiveCont[i]=pre.Rows[3].Cells[1+i].Text;
                //         } else if (pre.Rows[3].Cells[0].Text=="Minulý") {
                //            pattern.TransgressivePast=new string[3];
                //            pattern.STransgressivePast=true;
                //            for (int i=0; i<3; i++) pattern.TransgressivePast[i]=pre.Rows[3].Cells[1+i].Text;
                //        }
                //    }
                //}

                ItemPatternVerb pattern2 =pattern.Clone();

                pattern.Optimize();
                itemsPatternVerbFrom.Add(pattern);
                PatternVerbFromRefresh();

                pattern2.ConvertToPhonetics();
                pattern2.Optimize();
                pattern2.AddQuestionMark();

                itemsPatternVerbTo.Add(pattern2);
                PatternVerbToRefresh();

                itemsVerbs.Add(new ItemVerb{
                    From=pattern.GetPrefix(),
                    PatternFrom=pattern.Name,
                    To=new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{ Body=pattern2.GetPrefix(), Pattern=pattern2.Name } },
                });

                VerbRefresh();
            };
            }catch{ MessageBox.Show("Error");}
            Computation.DownloadString(ref handler, name);
        }

        void button1_Click(object sender, EventArgs e) {
            int totals=0;
            totals+=itemsSimpleWords.Count;
            totals+=itemsPhrases.Count;
            totals+=itemsSentences.Count;
            totals+=itemsSentencePatterns.Count;
            totals+=itemsSentencePatternParts.Count;

            totals+=itemsNouns.Count;
            totals+=itemsPatternNounFrom.Count;
            totals+=itemsPatternNounTo.Count;

            totals+=itemsAdjectives.Count;
            totals+=itemsPatternAdjectiveFrom.Count;
            totals+=itemsPatternAdjectiveTo.Count;

            totals+=itemsPronouns.Count;
            totals+=itemsPatternPronounFrom.Count;
            totals+=itemsPatternPronounTo.Count;

            totals+=itemsNumbers.Count;
            totals+=itemsPatternNumberFrom.Count;
            totals+=itemsPatternNumberTo.Count;

            totals+=itemsVerbs.Count;
            totals+=itemsPatternVerbFrom.Count;
            totals+=itemsPatternVerbTo.Count;

            totals+=itemsAdverbs.Count;
            totals+=itemsPrepositions.Count;
            totals+=itemsConjunctions.Count;
            totals+=itemsParticles.Count;
            totals+=itemsInterjections.Count;
            totals+=itemsReplaceS.Count;
            totals+=itemsReplaceG.Count;
            totals+=itemsReplaceE.Count;
            totals+=itemsSentenceParts.Count;

            MessageBox.Show("Celkový počet záznamů: "+totals);
        }

        void tENHLEToolStripMenuItem_Click(object sender, EventArgs e) {
            PronounSaveCurrent();
            ItemPronoun pronoun =ItemPronoun.tENHLE;

            if (!itemsPatternPronounFrom.ExistsWithName("tENHLE")) {//exist
                itemsPatternPronounFrom.Add(ItemPatternPronoun.tENHLE);
                PatternPronounFromRefresh();

                ItemPatternPronoun v= ItemPatternPronoun.tENHLE;
                v.AddQuestionMark();
                itemsPatternPronounTo.Add(v);
                PatternPronounToRefresh();
            }

            itemsPronouns.Add(pronoun);
            PronounRefresh();
        }

        void toolStripMenuItem98_Click(object sender, EventArgs e) {
             if (CurrentPatternVerbTo!=null){
                PatternVerbToSaveCurrent();

                ItemPatternVerb item =CurrentPatternVerbTo.Duplicate();
                itemsPatternVerbTo.Add(item);

                PatternVerbToRefreshFilteredList();
                PatternVerbToSetListBox();
                PatternVerbToSetCurrent();
            }
        }

        void addLinkedFromtoToolStripMenuItem2_Click(object sender, EventArgs e) {
             string name = GetString("", "Název adjektiva");
            if (name == null) return;

            if (name=="rád") { 
                ItemAdjective adj=new ItemAdjective();
                adj.From="rád";
                adj.PatternFrom="rád";
                adj.To=new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body="rá", Pattern="ráD" } };
                itemsAdjectives.Add(adj);

                ItemPatternAdjective patternFrom=ItemPatternAdjective.rad();
                patternFrom.Optimize();
                itemsPatternAdjectiveFrom.Add(patternFrom);

                ItemPatternAdjective patternTo=ItemPatternAdjective.rad_ph();
                patternTo.ConvertToPhonetics();
                patternTo.Optimize();
                patternTo.AddQuestionMark();
                itemsPatternAdjectiveTo.Add(patternTo);
                
                AdjectiveRefresh();
                PatternAdjectiveFromRefresh();
                PatternAdjectiveToRefresh();
                return;
            }

            DownloadDataCompletedEventHandler handler=null;
            #if !DEBUG
            try{
            #endif
                handler += (sender2, e2) => {
                    if (e2.Error!=null) {
                        MessageBox.Show("Error "+e2.Error.Message);
                        return;
                    }
                    var data = e2.Result;
                    string html = Encoding.UTF8.GetString(data);

                    ItemPatternAdjective pattern=ParseItemWikidirectonary.ItemPatternAdjective(html, out string error, name);
                    if (pattern!=null){
                   // List<Table> tables=new List<Table>();
                   // Computation.FindTableInHTML(html, "deklinace adjektivum", ref tables);


                   //// bool future=false;
                   // if (tables.Count>=1) {
                   //     Table table=tables[0];
                   //     if (table.Rows.Count==9 && table.Rows[3].Cells.Count==9) {
                   //         ItemPatternAdjective pattern = new ItemPatternAdjective {
                   //             Name = name,
                   //            // TypeShow = VerbTypeShow.Unknown,
                   //             Feminine=new string[18],
                   //             Middle=new string[18],
                   //             MasculineAnimate=new string[18],
                   //             MasculineInanimate=new string[18],
                   //         };

                   //         for (int r=0; r<7; r++) pattern.MasculineAnimate[r]=table.Rows[2+r].Cells[1+0].Text;
                   //         for (int r=0; r<7; r++) pattern.MasculineInanimate[r/*+7*2*/]=table.Rows[2+r].Cells[1+1].Text;
                   //         for (int r=0; r<7; r++) pattern.Feminine[r/*+7*4*/]=table.Rows[2+r].Cells[1+2].Text;
                   //         for (int r=0; r<7; r++) pattern.Middle[r/*+7*6*/]=table.Rows[2+r].Cells[1+3].Text;

                   //         for (int r=0; r<7; r++) pattern.MasculineAnimate[r+7/**1*/+2]=table.Rows[2+r].Cells[1+4].Text;
                   //         for (int r=0; r<7; r++) pattern.MasculineInanimate[r+7/**3*/+2]=table.Rows[2+r].Cells[1+5].Text;
                   //         for (int r=0; r<7; r++) pattern.Feminine[r+7/*+7*5*/+2]=table.Rows[2+r].Cells[1+6].Text;
                   //         for (int r=0; r<7; r++) pattern.Middle[r+7/*+7*7*/+2]=table.Rows[2+r].Cells[1+7].Text;

                            ItemPatternAdjective patternTo=pattern.Clone();

                            pattern.Optimize();
                            itemsPatternAdjectiveFrom.Add(pattern);
                            PatternAdjectiveFromRefresh();

                            patternTo.ConvertToPhonetics();
                            patternTo.Optimize();
                            patternTo.AddQuestionMark();

                            itemsPatternAdjectiveTo.Add(patternTo);

                            PatternAdjectiveToRefresh();

                            ItemAdjective num = new ItemAdjective {
                                PatternFrom = pattern.Name,
                                From = pattern.GetPrefix(),

                                To = new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{Body=patternTo.GetPrefix(), Pattern = patternTo.Name}},
                            };
                            itemsAdjectives.Add(num);

                            AdjectiveRefresh();
                     //   }
                    }
                };
            #if !DEBUG
            } catch { MessageBox.Show("Error");}
            #endif
            Computation.DownloadString(ref handler, name);
        }

        void PatternAdjectiveFromRefresh() {
            PatternAdjectiveFromRefreshFilteredList();
            PatternAdjectiveFromSetListBox();
            PatternAdjectiveFromListBoxSetCurrent();
        }

        void PatternAdjectiveToRefresh() {
            PatternAdjectiveToRefreshFilteredList();
            PatternAdjectiveToSetListBox();
            PatternAdjectiveToListBoxSetCurrent();
        }

        void AdjectiveRefresh() {
            AdjectiveRefreshFilteredList();
            AdjectiveSetListBox();
            AdjectiveListBoxSetCurrent();
        }

        void vytvořitToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd=new OpenFileDialog();
            ofd.Multiselect=true;
            ofd.Filter="TranslatorWritter|*.trw|All files|*.*";
            DialogResult dr =ofd.ShowDialog();
            if (dr==DialogResult.OK) {
                SaveFileDialog sfd = new SaveFileDialog {
                    Filter = "TranslatorWritter Archive|*.trw_a|All files|*.*"
                };
                DialogResult dr2 =sfd.ShowDialog();
                if (dr2==DialogResult.OK) {
                    if (!File.Exists(sfd.FileName)){
                        Packer packer=new Packer();

                        Thread thread = new Thread(() => packer.CreatePackageAsync(ofd.FileNames, sfd.FileName));
                        FormWait fw=new FormWait("Počkejte", "Sestavování balíčku");
                        fw.FormClosed += Fw_FormClosed;
                        fw.Show();

                       // packer.Done += Packer_Done;
                        packer.ProgressChange += Packer_ProgressChange;

                        thread.Start();

                        void Packer_ProgressChange(object sender2, Packer.SampleEventArgs e2) {
                            fw.Invoke((MethodInvoker)delegate {
                                 fw.Percentage=(int)(e2.Percentage*100);
                                if (e2.Percentage==1) fw.Close();
                            });


                        }

                        void Fw_FormClosed(object sender2, FormClosedEventArgs e2) {
                           if (thread.IsAlive) thread.Abort();
                        }
                    }
                }
            }
        }

        void obaličkovatToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd=new OpenFileDialog();
            ofd.Filter="TranslatorWritter Archive|*.trw_a|All files|*.*";
            DialogResult dr = ofd.ShowDialog();
            if (dr==DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    FolderBrowserDialog fd=new FolderBrowserDialog();
                    DialogResult dr2 =fd.ShowDialog();
                    if (dr2==DialogResult.OK) {
                        Packer.ExtractMergedFiles(ofd.FileName,fd.SelectedPath);
                    }
                }
            }
        }

        void buttonInfoText_Click(object sender, EventArgs e) {
            MessageBox.Show("Styly\r\n#... záhlaví kapitoly\r\n-...Odrážka\r\n\r\nUvést - doporučeno\r\n#Zdroje dat\r\nVýslovnost (symboly)");
        }

        void BÝTToolStripMenuItem1_Click(object sender, EventArgs e) {
            SaveCurrentVerb();
            ItemVerb verb =new ItemVerb{
                PatternFrom="BÝT",
                To=new List<TranslatingToDataWithPattern>{new TranslatingToDataWithPattern{Body="", Pattern="BÝT" }},
            };

            if (!itemsPatternVerbFrom.ExistsWithName("BÝT")) {//exist
                itemsPatternVerbFrom.Add(ItemPatternVerb.BÝT);
                PatternVerbFromRefreshFilteredList();
                PatternVerbFromSetListBox();
                PatternVerbFromSetCurrent();

                ItemPatternVerb v= ItemPatternVerb.BÝT;
                v.AddQuestionMark();
                itemsPatternVerbTo.Add(v);
                PatternVerbToRefreshFilteredList();
                PatternVerbToSetListBox();
                PatternVerbToSetCurrent();
            }

            itemsVerbs.Add(verb);
            VerbRefreshFilteredList();
            VerbSetListBox();
            SetCurrentVerb();
        }

        void ReplaceESetListBox() {
            int index=listBoxReplaceE.SelectedIndex;
            listBoxReplaceE.Items.Clear();
            for (int i=0; i<itemsReplaceEFiltered.Count; i++) {
                ItemReplaceE item = itemsReplaceEFiltered[i];

                string textToAdd=item.GetText();
             //   if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceE.Items.Add(textToAdd);
            }

            if (index>=listBoxReplaceE.Items.Count)index=listBoxReplaceE.Items.Count-1;
            listBoxReplaceE.SelectedIndex=index;
        }

        void button3_Click(object sender, EventArgs e) {
            if (CurrentNoun!=null) {
                simpleUINouns.Add("","","");
            }
        }

        private void addStartingStringToolStripMenuItem1_Click(object sender, EventArgs e) {
            if (CurrentPatternPronounTo!=null) {
                PatternPronounToSaveCurrent();
                FormString form=new FormString();
                form.LabelText="Přidat na začátek pronoma string";
                form.ShowDialog();
                string str=form.ReturnString;
                if (!string.IsNullOrEmpty(str)) CurrentPatternPronounTo.AddStartingString(str);
                PatternPronounToSetCurrent();
            }
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e) {
            doingJob = true;
            SaveCurrentVerb();
            itemsVerbs = itemsVerbs.OrderBy(a => a.From).ToList();
            VerbRefreshFilteredList();
            VerbSetListBox();
            VerbListBoxSetCurrent();
            doingJob = false;
        }

        private void tENTOToolStripMenuItem_Click(object sender, EventArgs e) {
            PronounSaveCurrent();
            ItemPronoun pronoun =new ItemPronoun{
                From="t",
                PatternFrom="tENTO",
                To=new List<TranslatingToDataWithPattern>{ new TranslatingToDataWithPattern{ Body="t",Pattern="tENTO"} }
            };

            if (!itemsPatternPronounFrom.ExistsWithName("tENTO")) {//exist
                itemsPatternPronounFrom.Add(ItemPatternPronoun.tENTO);
                PatternPronounFromRefresh();

                ItemPatternPronoun v= ItemPatternPronoun.tENTO;
                v.AddQuestionMark();
                itemsPatternPronounTo.Add(v);
                PatternPronounToRefresh();
            }

            itemsPronouns.Add(pronoun);
            PronounRefresh();
        }

        private void vložitExistujícíToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog ofd=new OpenFileDialog();
            ofd.CheckFileExists=true;
            ofd.Filter="Soubor Trans Writter|*.trw|Textové soubory|*.txt|Všechny soubory|*.*";
            DialogResult dr=ofd.ShowDialog();
            if (dr==DialogResult.OK) {
                if (File.Exists(ofd.FileName)) {
                    string[] lines=File.ReadAllLines(ofd.FileName);

                    var _LoadedSaveVersion=FormMain.LoadedSaveVersion;
                    var _LoadedVersionNumber=FormMain.LoadedVersionNumber;

                    LoadedSaveVersion=lines[0];
                    if (LoadedSaveVersion.Length>4){
                        string num=LoadedSaveVersion.Substring(4);
                        if (num=="1.0") LoadedVersionNumber=1;
                        else if (num=="0.1") LoadedVersionNumber=0;
                        else{
                            if (float.TryParse(num, out LoadedVersionNumber)) { } else LoadedVersionNumber=-1;
                        }
                    }

                    // Head
                    int i = 0;
                    for (; i < lines.Length; i++) {
                        string line = lines[i];

                        if (line == "-") break;
                        string subtype = line.Substring(0, 1);
                        switch (subtype) {
                            // Comment info
                            case "i":
                                textBoxInfo.Text += Environment.NewLine+line.Substring(1).Replace("\\n", Environment.NewLine);
                                break;

                            case "a":
                                if (string.IsNullOrEmpty(textBoxAuthor.Text)) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "f":
                                if (string.IsNullOrEmpty(textBoxLangFrom.Text)) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "e":
                                if (string.IsNullOrEmpty(textBoxSelect.Text=line.Substring(1))) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "c":
                                textBoxComment.Text+=Environment.NewLine+line.Substring(1).Replace("\\n", Environment.NewLine);
                                break;

                            case "z":
                                if (string.IsNullOrEmpty(textBoxZachytne.Text)) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "l":
                                if (string.IsNullOrEmpty(textBoxLang.Text)) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "g":
                                if (string.IsNullOrEmpty(textBoxGPS.Text)) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "x":
                                if (string.IsNullOrEmpty(textBoxtypeLang.Text)) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "o":
                                if (string.IsNullOrEmpty(textBoxOblast.Text)) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "r":
                                if (string.IsNullOrEmpty(textBoxLocOriginal.Text)) textBoxAuthor.Text=line.Substring(1);
                                break;

                            case "#":
                                break;
                        }
                    }

                    // SentencePattern
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsSentencePatterns.Add(ItemSentencePattern.Load(line));
                    }

                    // SentencePatternPart
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsSentencePatternParts.Add(ItemSentencePatternPart.Load(line));
                    }

                    // Sentences
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsSentences.Add(ItemSentence.Load(line));
                    }

                    // SentencePart
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsSentenceParts.Add(ItemSentencePart.Load(line));
                    }

                    // Phrase
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsPhrases.Add(ItemPhrase.Load(line));
                    }

                    // SimpleWords
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsSimpleWords.Add(ItemSimpleWord.Load(line));
                    }

                    // ReplaceS
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsReplaceS.Add(ItemReplaceS.Load(line));
                    }

                    // ReplaceG
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsReplaceG.Add(ItemReplaceG.Load(line));
                    }

                    // ReplaceE
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        itemsReplaceE.Add(ItemReplaceE.Load(line));
                    }

                    // PatternNounFrom
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsPatternNounFrom.Add(ItemPatternNoun.Load(line));
                    }

                    // PatternNounTo
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")   break;
                        if (line == "")  continue;
                        itemsPatternNounTo.Add(ItemPatternNoun.Load(line));
                    }

                    // Noun
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsNouns.Add(ItemNoun.Load(line));
                    }

                    // PatternAdjectives
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsPatternAdjectiveFrom.Add(ItemPatternAdjective.Load(line));
                    }

                    // PatternAdjectivesTo
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsPatternAdjectiveTo.Add(ItemPatternAdjective.Load(line));
                    }

                    // Adjectives
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsAdjectives.Add(ItemAdjective.Load(line));
                    }

                    // PatternPronounsFrom
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsPatternPronounFrom.Add(ItemPatternPronoun.Load(line));
                    }

                    // PatternPronounsTo
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsPatternPronounTo.Add(ItemPatternPronoun.Load(line));
                    }

                    // Pronouns
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsPronouns.Add(ItemPronoun.Load(line));
                    }

                    // PatternNumbersFrom
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        var item=ItemPatternNumber.Load(line);
                        if (item!=null) itemsPatternNumberFrom.Add(item);
                    }

                    // PatternNumbersTo
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        var item=ItemPatternNumber.Load(line);
                        if (item!=null) itemsPatternNumberTo.Add(item);
                    }

                    // Numbers
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        itemsNumbers.Add(ItemNumber.Load(line));
                    }

                    // PatternVerbsFrom
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsPatternVerbFrom.Add(ItemPatternVerb.Load(line));
                    }

                    // PatternVerbsTo
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsPatternVerbTo.Add(ItemPatternVerb.Load(line));
                    }

                    // Verb
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        itemsVerbs.Add(ItemVerb.Load(line));
                    }

                    // Adverb
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")   break;
                        if (line == "")  continue;
                        itemsAdverbs.Add(ItemAdverb.Load(line));
                    }

                    // Preposition
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsPrepositions.Add(ItemPreposition.Load(line));
                    }

                    // Conjunction
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsConjunctions.Add(ItemConjunction.Load(line));
                    }

                    // Particle
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-")  break;
                        if (line == "")  continue;
                        itemsParticles.Add(ItemParticle.Load(line));
                    }

                    // Interjection
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsInterjections.Add(ItemInterjection.Load(line));
                    }

                    // PhrasePattern
                    for (i++; i < lines.Length; i++) {
                        string line = lines[i];
                        if (line == "-") break;
                        if (line == "")  continue;
                        itemsPhrasePattern.Add(ItemPhrasePattern.Load(line));
                    }

                    
                    LoadedSaveVersion = _LoadedSaveVersion;
                    LoadedVersionNumber = _LoadedVersionNumber;

                    TextBoxSentencePatternFilter_TextChanged(null, null);
                    TextBoxSentenceFilter_TextChanged(null, null);
                    TextBoxSimpleWordFilter_TextChanged(null, null);
                    TextBoxNounFilter_TextChanged(null, null);
                    TextBoxPatternNounFromFilter_TextChanged(null, null);
                    TextBoxPatternNounToFilter_TextChanged(null, null);
                    PatternAdjectiveFromTextBoxFilter_TextChanged(null, null);
                    TextBoxAdjectiveFilter_TextChanged(null, null);
                    TextBoxNumberFilter_TextChanged(null, null);
                    PatternNumberFromTextBoxFilter_TextChanged(null, null);
                    PatternPronounFromTextBoxFilter_TextChanged(null, null);
                    TextBoxPronounFilter_TextChanged(null, null);
                    PatternVerbFromTextBoxFilter_TextChanged(null, null);
                    TextBoxVerbFilter_TextChanged(null, null);
                    TextBoxPrepositionFilter_TextChanged(null, null);
                    TextBoxConjunctionFilter_TextChanged(null, null);
                    TextBoxInterjectionFilter_TextChanged(null, null);
                    TextBoxParticleFilter_TextChanged(null, null);
                    TextBoxAdverbFilter_TextChanged(null, null);
                    TextBoxSentencePatternPartFilter_TextChanged(null, null);
                    TextBoxPhraseFilter_TextChanged(null, null);
                    TextBoxReplaceSFilter_TextChanged(null, null);
                    TextBoxReplaceEFilter_TextChanged(null, null);
                    TextBoxReplaceGFilter_TextChanged(null, null);

                    PatternNumberToTextBoxFilter_TextChanged(null, null);
                    PatternVerbToTextBoxFilter_TextChanged(null, null);
                    PatternAdjectiveToTextBoxFilter_TextChanged(null, null);
                    PatternPronounToTextBoxFilter_TextChanged(null, null);
                }
            }
        }

        void ReplaceERefreshFilteredList() {
            if (itemsReplaceEFiltered==null) itemsReplaceEFiltered=new List<ItemReplaceE>();
            itemsReplaceEFiltered.Clear();
            string filter=textBoxReplaceEFilter.Text;
            bool useFilter = filter!="" && filter!="*";

            if (useFilter) {
                for (int i=0; i<itemsReplaceE.Count; i++) {
                    ItemReplaceE item = itemsReplaceE[i];

                    if (item.Filter(filter)) {
                        itemsReplaceEFiltered.Add(item);
                    }
                }
            } else {
                for (int i=0; i<itemsReplaceE.Count; i++) {
                    ItemReplaceE item = itemsReplaceE[i];
                    itemsReplaceEFiltered.Add(item);
                }
            }
        }

        void AddNewItemReplaceE() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentReplaceE();

            var newItem=new ItemReplaceE();
           // newItem.ID=itemsPronouns.Count;
            itemsReplaceE.Add(newItem);
            CurrentReplaceE=newItem;
            ReplaceERefreshFilteredList();
            ReplaceESetListBox();
            ListBoxSetCurrentReplaceE();
            SetCurrentReplaceE();

            doingJob=false;
        }

        void RemoveItemReplaceE(ItemReplaceE item) {
            Edited=true;
            ChangeCaptionText();
            itemsReplaceE.Remove(item);
            ReplaceERefreshFilteredList();
            ReplaceESetListBox();
            SetCurrentReplaceE();
        }

        void SetCurrentReplaceE(){
            if (itemsReplaceEFiltered.Count==0) {
                SetNoneReplaceE();
                return;
            }

            int index=listBoxReplaceE.SelectedIndex;
            if (index>=itemsReplaceEFiltered.Count) index=itemsReplaceEFiltered.Count-1;
            if (index<0) index=0;
            CurrentReplaceE=itemsReplaceEFiltered[index];

            textBoxReplaceEFrom.Text=CurrentReplaceE.From;
            textBoxReplaceETo.Text=CurrentReplaceE.To;
        //    textBoxReplaceEFall.Text=CurrentReplaceE.Fall;

            textBoxReplaceEFrom.Visible=true;
            textBoxReplaceETo.Visible=true;
          //  textBoxReplaceEFall.Visible=true;

            labelReplaceEFrom.Visible=true;
            labelReplaceETo.Visible=true;
          //  labelReplaceEFall.Visible=true;

          //  ChangeTypeReplaceE(CurrentReplaceE?.Type);
        }

        void ListBoxSetCurrentReplaceE() {
            for (int indexCur=0; indexCur<itemsReplaceEFiltered.Count; indexCur++) {
                if (itemsReplaceE[indexCur]==CurrentReplaceE) {
                    int indexList=listBoxReplaceE.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxReplaceE.SelectedIndex=indexCur;
                    break;
                }
            }
        }

        void SaveCurrentReplaceE() {
            if (CurrentReplaceE==null) return;
            CurrentReplaceE.From =textBoxReplaceEFrom.Text;
            CurrentReplaceE.To   =textBoxReplaceETo.Text;
        }

        void SetNoneReplaceE(){
            textBoxReplaceEFrom.Text="";
            textBoxReplaceETo.Text="";

            textBoxReplaceEFrom.Visible=false;
            textBoxReplaceETo.Visible=false;

            labelReplaceEFrom.Visible=false;
            labelReplaceETo.Visible=false;
        }
        #endregion

        //void NounAddTools() {
        //    simpleUINouns = new SimpleToWithPattern {
        //        Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
        //        Location = new Point(0, 110/*180*/),
        //        Size = new Size(splitContainerNoun.Panel2.Width, splitContainerNoun.Panel2.Height - 180),
        //    };
        //    simpleUINouns.Add("","","");
        //    splitContainerNoun.Panel2.Controls.Add(simpleUINouns);
        //}

        //void VerbAddTools() {
        //    simpleUIVerbs = new SimpleToWithPattern {
        //        Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
        //        Location = new Point(0, 110),
        //        Size = new Size(splitContainerVerb.Panel2.Width, splitContainerVerb.Panel2.Height - 180),
        //    };
        //    simpleUIVerbs.Add("","","");
        //    splitContainerVerb.Panel2.Controls.Add(simpleUIVerbs);
        //}

        static SimpleToWithPattern AddToToolsWithPattern(SplitContainer splitContainer) {
            SimpleToWithPattern controlUI = new SimpleToWithPattern {
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(0, 110),
                Size = new Size(splitContainer.Panel2.Width, splitContainer.Panel2.Height - 180),
            };
            controlUI.Add("","","");
            splitContainer.Panel2.Controls.Add(controlUI);
            return controlUI;
        }

        static SimpleTo AddToTools(SplitContainer splitContainer) {
            SimpleTo controlUI = new SimpleTo {
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(0, 110),
                Size = new Size(splitContainer.Panel2.Width, splitContainer.Panel2.Height - 180),
            };
            controlUI.Add("","");
            splitContainer.Panel2.Controls.Add(controlUI);
            return controlUI;
        }
    }
}