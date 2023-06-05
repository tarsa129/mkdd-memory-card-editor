using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        public MemCard MemoryCard;
        public byte[] ImageBlock;
        public String FileName;
        public String[] StoredValues;
        public CourseRecords[] TimeTrials;
        public CupRecord[] GrandPrixs;
        public Boolean Initialized;

        public int TTComboBoxIndex;
        public int GPComboBoxIndex;
        public MainForm()
        {
            MemoryCard = new MemCard();
            ImageBlock = new byte[0x2040];
            TimeTrials = new CourseRecords[16];
            GrandPrixs = new CupRecord[5];
            TTComboBoxIndex = 0;
            GPComboBoxIndex = 0;
            Initialized = false;
            InitializeComponent();
            
        }

        //Methods that deal with opening a .gci
        private void opengciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog OpenGci = new OpenFileDialog();
            OpenGci.Title = "Open MKDD .gci File";
            OpenGci.Filter = " MKDD .gci file (*.gci)|*.gci";
            if (OpenGci.ShowDialog() == DialogResult.OK)
            {
                this.cmbTTCourse.SelectedIndex = 0;
                this.GPComboBox.SelectedIndex = 0;

                String filePath = OpenGci.FileName;
                byte[] bytes = File.ReadAllBytes(filePath);

                savegciAsNTSCJToolStripMenuItem.Enabled = false;
                savegciAsPALToolStripMenuItem.Enabled = false;
                savegciAsNTSCUToolStripMenuItem.Enabled = false;
                savegciToolStripMenuItem.Enabled = false;
                main_tab.Enabled = false;
                

                Array.Copy(bytes, ImageBlock, ImageBlock.Length);
                //Console.WriteLine( (bytes.Length).ToString("x"));
                Array.Copy(bytes, 0x2040, bytes, 0x0, 0x4000);
                this.StoredValues = Crypt.decodeRawBytes(bytes, ref MemoryCard);
                FillFormFields(this.StoredValues);
                this.FileName = filePath + ".gci";



                savegciAsNTSCJToolStripMenuItem.Enabled = true;
                savegciAsPALToolStripMenuItem.Enabled = true;
                savegciAsNTSCUToolStripMenuItem.Enabled = true;
                savegciToolStripMenuItem.Enabled = true;
                main_tab.Enabled = true;

                


            }
        }
        private void FillFormFields(String[] Values)
        {
            String[] IDs = new string[] { "21", "22", "23", "24", "25", "26", "28", "29", "2a", "2b", "2c", "2d", "2f", "31", "32", "33" };
            String[] GPIDs = new string[] { "0", "1", "2", "3", "4" };
            //settings part 1
            Values[0] = Values[0].PadLeft(4, '0');
            FillPart1Settings(Values[0]);

            //unlockables
            Values[1] = Values[1].PadLeft(32, '0');
            FillUnlockables(Values[1]);

            Values[2] = Values[2].PadLeft(6, '0');
            FillPart2Settings(Values[2]);
            txtTag.Text = Values[3];

            String[] GPTimes = new String[80];
            Array.Copy(Values, 0x4, GPTimes, 0x0, 80);
            int Index = 0;
            for (int i = 0; i < 5; i++)
            {
                GrandPrixs[i] = new CupRecord(GPIDs[i]);
                GrandPrixs[i].MakeGPRecords(GPTimes, Index);
                Index += 16;
            }
            FillGPTimes(GrandPrixs[0].ToStringArray());

            String[] TT3lapTimes = new String[240];
            Array.Copy(Values, 84, TT3lapTimes, 0x0, 240);
            Index = 0;
            for (int i = 0; i< 16; i ++)
            {
                TimeTrials[i] = new CourseRecords(IDs[i]);
                TimeTrials[i].Make3lapRecords(TT3lapTimes, Index);
                Index += 15;
            }
            Fill3lapTimes(TimeTrials[0].ToStringArray3Lap());

            String[] TTFlapTimes = new String[48];
            Array.Copy(Values, 324, TTFlapTimes, 0x0, 48);
            

            Index = 0;
            for (int i = 0; i < 16; i++)
            {
                TimeTrials[i].MakeflapRecord(TTFlapTimes, Index);
                Index += 3;
            }
            FillFlapTimes(TimeTrials[0].ToStringArrayFLap());

            //Console.WriteLine(TimeTrials[0].ToStringArrayFLap()[0]);
            //Console.WriteLine(TimeTrials[1].ToStringArrayFLap()[0]);
            //Console.WriteLine(TimeTrials[2].ToStringArrayFLap()[0]);
            
            FillPbsTimes(TT3lapTimes, TTFlapTimes);

            this.cmbTTCourse.SelectedIndex = TTComboBoxIndex;
            this.GPComboBox.SelectedIndex = GPComboBoxIndex;
            Initialized = true;
        }
        private void FillPart1Settings(String Values)
        {
            Values = Values.PadLeft(32, '0');
            if (Values[28] == '0')
            {
                radGOn.Checked = true;
            }
            else
            {
                radGOff.Checked = true; 
            }
            if (Values[29] == '0')
            {
                radROn.Checked = true;
            }
            else
            {
                radROff.Checked = true;
            }
            if (Values[30] == '1')
            {
                radSurround.Checked = true;
            }
            else if (Values[31] == '1')
            {
                radMono.Checked = true;
            }
            else
            {
                radStereo.Checked = true;
            }
        }
        private void FillUnlockables(String Values)
        {
            //Console.WriteLine(Values);
            chkPK.Checked = Values[0x3] == '1';
            chkBP.Checked = Values[0x4] == '1';
            chkPP.Checked = Values[0x5] == '1';
            chkWR.Checked = Values[0x6] == '1';
            chkTB.Checked = Values[0x7] == '1';
            chkBC.Checked = Values[0x8] == '1';
            chkGF.Checked = Values[0x9] == '1';
            chkuwuK.Checked = Values[0xa] == '1';
            chkTK.Checked = Values[0xb] == '1';
            chkBB.Checked = Values[0xc] == '1';
            chkBT.Checked = Values[0xd] == '1';
            chkPW.Checked = Values[0xe] == '1';
            chkRB.Checked = Values[0xf] == '1';
            chkTaK.Checked = Values[0x17] == '1';
            chkLM.Checked = Values[0x18] == '1';
            chkuwu.Checked = Values[0x1a] == '1';
            chkAct.Checked = Values[0x1b] == '1';
            chkSpec.Checked = Values[0x1c] == '1';
            chkMM.Checked = Values[0x1d] == '1';
            chkSMS.Checked = Values[0x1f] == '1';

            if (chkSMS.Checked && chkuwu.Checked)
            {
                chkChars.Checked = true;
            }
            if (chkAct.Checked == true && chkSpec.Checked == true && chkMM.Checked == true && chkLM.Checked == true && chkTaK.Checked == true)
            {
                chkStages.Checked = true;
            }
            if (chkPK.Checked == true &&  chkBP.Checked == true && chkPP.Checked == true && chkWR.Checked == true && chkTB.Checked == true
                && chkBC.Checked == true && chkGF.Checked == true && chkuwuK.Checked == true && chkTK.Checked == true
                && chkBB.Checked == true && chkBT.Checked == true && chkPW.Checked == true && chkRB.Checked == true)
            {
                chkKart.Checked = true;
            }
            if (chkChars.Checked == true && chkStages.Checked == true && chkKart.Checked == true)
            {
                chkEverything.Checked = true;
            }
        }
        private void FillPart2Settings(String Values)
        {
            //Console.WriteLine(Values);
            int Volume = Convert.ToInt32("" + Values[0] + Values[1], 16);
            if (Volume >= 128)
            {
                Volume = Volume - 256;
            }
            tckVolume.Value = Volume;
            lblVolume.Text = "" + Volume;

            int Item = int.Parse("" + Values[3], System.Globalization.NumberStyles.HexNumber);
            //Console.WriteLine(Item);
            if (Item == 0)
            {
                radRec.Checked = true;
            } else if (Item == 2)
            {
                radBasic.Checked = true;
            } else if (Item == 1)
            {
                radNone.Checked = true;
            } else if (Item == 33)
            {
                radFrantic.Checked = true;
            }

            //Console.WriteLine(Values[5]);
            int Laps = int.Parse("" + Values[5], System.Globalization.NumberStyles.HexNumber);
            //Console.WriteLine(Laps);
            if (Laps > cmbLaps.Items.Count)
            {
                Laps = 0;
            }
            cmbLaps.SelectedIndex = Laps;
        }
        private void FillGPTimes(String[] Values)
        {
            // Console.WriteLine(Values[1]);
            String[] Abbrev = new string[] { "Mush", "Flow", "Star", "Spec", "Act" };
            String[] Speeds = new string[] { "5", "10", "15", "M" };

            int ValIndex = 0;
            //for (int i = 0; i < 5; i ++)
            
            for (int i = 0; i < 1; i ++)
            {
                for (int j = 0; j < 4; j ++)
                {
                    //Console.WriteLine(Values[ValIndex]);
                    ComboBox CMBBox = this.Controls.Find("cmb" + Abbrev[i] + Speeds[j] + "1p", true).FirstOrDefault() as ComboBox;
                    CMBBox.SelectedIndex = Convert.ToInt32("0x" + Values[ValIndex][0] + "" + Values[ValIndex][1], 16);

                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + Speeds[j] + "2p", true).FirstOrDefault() as ComboBox;
                    CMBBox.SelectedIndex = Convert.ToInt32("0x" + Values[ValIndex][2] + "" + Values[ValIndex][3], 16);

                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + Speeds[j] + "V", true).FirstOrDefault() as ComboBox;
                    CMBBox.SelectedIndex = Convert.ToInt32(Values[ValIndex][4] + "" + Values[ValIndex][5], 16);

                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + Speeds[j] + "T", true).FirstOrDefault() as ComboBox;
                    CMBBox.SelectedIndex = Convert.ToInt32(Values[ValIndex + 1][0] + "" + Values[ValIndex + 1][1], 16);

                    TextBox Box = this.Controls.Find("txt" + Abbrev[i] + Speeds[j] + "p", true).FirstOrDefault() as TextBox;
                    Box.Text = Convert.ToInt32(Values[ValIndex + 1][2] + "" + Values[ValIndex + 1][3], 16).ToString();


                    Box = this.Controls.Find("txt" + Abbrev[i] + Speeds[j] + "T", true).FirstOrDefault() as TextBox;
                    if (Values[ValIndex + 2] != "0")
                    {
                        Char FirstChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 2][0] + "" + Values[ValIndex + 2][1], 16));
                        Char SecondChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 2][2] + "" + Values[ValIndex + 2][3], 16));
                        Char ThirdChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 2][4] + "" + Values[ValIndex + 2][5], 16));

                        Box.Text = "" + FirstChar + SecondChar + ThirdChar;
                    } else
                    {
                        Box.Text = "";
                    }
                    CheckBox chkBox = this.Controls.Find("chk" + Abbrev[i] + Speeds[j] + "C", true).FirstOrDefault() as CheckBox;
                    chkBox.Checked = (Values[ValIndex + 1][5] == '3');


                    Box = this.Controls.Find("txt" + Abbrev[i] + Speeds[j], true).FirstOrDefault() as TextBox;
                    Box.Text = msToTime(Values[ValIndex + 3]);
                    ValIndex += 4;
                }
                
            }
            
        }
        private void Fill3lapTimes(String[] Values){

            String[] Abbrev = new string[] { "BP", "PB", "DC", "LC", "MaC", "YC", "MB", "MuC", "WS", "WC", "DDJ", "DKM", "BC", "RR", "DDD", "SL" };
            int ValIndex = 0;
            //for (int i = 0; i < 16; i ++)
            for (int i = 3; i < 4; i ++)
            {
                //TextBox Box = this.Controls.Find("txt" + Abbrev[i] + "3lap", true).FirstOrDefault() as TextBox;
                //Box.Text = msToTime(Values[ValIndex + 2]);
                for(int j = 1; j < 6; j++)
                {
                    ComboBox CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "3lap" + j + "1p", true).FirstOrDefault() as ComboBox;
                    CMBBox.SelectedIndex = Convert.ToInt32("0x" + Values[ValIndex][0] + "" + Values[ValIndex][1], 16);
                    //CMBBox.SelectedIndex = Convert.ToInt32(Values[ValIndex][0] + "" + Values[ValIndex][1]);


                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "3lap" + j + "2p", true).FirstOrDefault() as ComboBox;
                    CMBBox.SelectedIndex = Convert.ToInt32("0x" + Values[ValIndex][2] + "" + Values[ValIndex][3], 16);

                    
                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "3lap" + j + "V", true).FirstOrDefault() as ComboBox;
                    CMBBox.SelectedIndex = Convert.ToInt32(Values[ValIndex][4] + "" + Values[ValIndex][5], 16);

                    TextBox Box = this.Controls.Find("txt" + Abbrev[i] + "3lap" + j + "T", true).FirstOrDefault() as TextBox;
                    if (Values[ValIndex + 1] != "0")
                    {
                        Char FirstChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 1][0] + "" + Values[ValIndex + 1][1], 16));
                        Char SecondChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 1][2] + "" + Values[ValIndex + 1][3], 16));
                        Char ThirdChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 1][4] + "" + Values[ValIndex + 1][5], 16));

                        Box.Text = "" + FirstChar + SecondChar + ThirdChar;
                    }
                    else
                    {
                        Box.Text = "";
                    }
                    Box = this.Controls.Find("txt" + Abbrev[i] + "3lap" + j, true).FirstOrDefault() as TextBox;
                    Box.Text = msToTime(Values[ValIndex + 2]);

                    ValIndex += 3;
                }
            }
           
        }
        private void FillFlapTimes(String[] Values)
        {
            Console.WriteLine(Values[0]);
            String[] Abbrev = new string[] { "BP", "PB", "DC", "LC", "MaC", "YC", "MB", "MuC", "WS", "WC", "DDJ", "DKM", "BC", "RR", "DDD", "SL" };
            int ValIndex = 0;
            //for (int i = 0; i < 16; i++)
            for (int i = 3; i < 4; i++)
            {
                //TextBox Box = this.Controls.Find("txt" + Abbrev[i] + "flap", true).FirstOrDefault() as TextBox;
                //Box.Text = msToTime(Values[ValIndex + 2]);

                ComboBox CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "flap" + "1p", true).FirstOrDefault() as ComboBox;
                CMBBox.SelectedIndex = Convert.ToInt32("0x" + Values[ValIndex][0] + "" + Values[ValIndex][1], 16);

                CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "flap" + "2p", true).FirstOrDefault() as ComboBox;
                CMBBox.SelectedIndex = Convert.ToInt32("0x" + Values[ValIndex][2] + "" + Values[ValIndex][3], 16);

                CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "flap" + "V", true).FirstOrDefault() as ComboBox;
                CMBBox.SelectedIndex = Convert.ToInt32("0x" + Values[ValIndex][4] + "" + Values[ValIndex][5], 16);

                TextBox Box = this.Controls.Find("txt" + Abbrev[i] + "flap" + "T", true).FirstOrDefault() as TextBox;
                if (Values[ValIndex + 1] != "0")
                {
                    Char FirstChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 1][0] + "" + Values[ValIndex + 1][1], 16));
                    Char SecondChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 1][2] + "" + Values[ValIndex + 1][3], 16));
                    Char ThirdChar = Convert.ToChar(Convert.ToUInt32(Values[ValIndex + 1][4] + "" + Values[ValIndex + 1][5], 16));
                    Box.Text = "" + FirstChar + SecondChar + ThirdChar;
                }
                else
                {
                    Box.Text = "";
                }

                Box = this.Controls.Find("txt" + Abbrev[i] + "flap1", true).FirstOrDefault() as TextBox;
                Box.Text = msToTime( Values[ValIndex + 2]);

                ValIndex += 3;
                
            }
 
        }
        private void FillPbsTimes(String[] Values, String[] FValues)
        {
            //Console.WriteLine(FValues.Length);

            String[] Abbrev = new string[] { "BP", "PB", "DC", "LC", "MaC", "YC", "MB", "MuC", "WS", "WC", "DDJ", "DKM", "BC", "RR", "DDD", "SL" };
            int ValIndex = 0;
            int FValIndex = 0;
            for (int i = 0; i < 16; i++)
            {
                TextBox Box = this.Controls.Find("txt" + Abbrev[i] + "3lap", true).FirstOrDefault() as TextBox;
                Box.Text = msToTime(formatTime( Values[ValIndex + 2]));

                ValIndex += 15;

                
                Box = this.Controls.Find("txt" + Abbrev[i] + "flap", true).FirstOrDefault() as TextBox;
                Box.Text = msToTime(formatTime(FValues[FValIndex + 2]));

                FValIndex += 3;
            }
                
            
        }
        
        private static String msToTime(String milliseconds)
        {
            //Console.WriteLine(milliseconds);
            if (milliseconds.Length > 0)
            {
                //Console.WriteLine(milliseconds);
                int time = Convert.ToInt32("0x" + milliseconds, 16);
                int ms = time % 1000;
                time = Convert.ToInt32(Math.Floor((double)time / 1000));
                int sec = time % 60;
                int min = Convert.ToInt32(Math.Floor((double)time / 60));
                return "" + min + ":" + sec.ToString().PadLeft(2, '0') + ":" + ms.ToString().PadLeft(3,'0');
            } else
            {
                return "";
            }
            
        }
        private static String formatTime(String milliseconds)
        {

            if (milliseconds.Length == 0)
            {
                return "5B8D7F";
            }
            else
            {
                return Convert.ToInt32(milliseconds).ToString("x");

            }

        }
    
        //Methods that deal with user inputs
        //Edit all the relevant checkboxes
        private void UnlockChanged(object sender, EventArgs e)
        {
            CheckBox[] Karts = new CheckBox[] { chkPK, chkBP, chkPP, chkWR, chkTB, chkBC, chkGF, chkuwuK, chkTK, chkBB, chkBT, chkPW, chkRB };
            CheckBox[] Stages = new CheckBox[] { chkAct, chkSpec, chkMM, chkLM, chkTaK };
            CheckBox[] Chars = new CheckBox[] { chkSMS, chkuwu };
            
            if (!sender.Equals(chkChars) && !sender.Equals(chkStages) && !sender.Equals(chkKart) && !sender.Equals(chkEverything) )
            {
                
                chkChars.Checked = chkSMS.Checked && chkuwu.Checked;
                
                
                chkStages.Checked = chkAct.Checked == true && chkSpec.Checked == true && chkMM.Checked == true && chkLM.Checked == true && chkTaK.Checked == true;
                
                
                chkKart.Checked = chkPK.Checked == true && chkBP.Checked == true && chkPP.Checked == true && chkWR.Checked == true && chkTB.Checked == true
                    && chkBC.Checked == true && chkGF.Checked == true && chkuwuK.Checked == true && chkTK.Checked == true
                    && chkBB.Checked == true && chkBT.Checked == true && chkPW.Checked == true && chkRB.Checked == true;
                
                
                    chkEverything.Checked = chkChars.Checked == true && chkStages.Checked == true && chkKart.Checked == true;
                
            } 
            else
            {
                if ( (sender.Equals(chkKart) && chkKart.Checked == true) || (sender.Equals(chkEverything) && chkEverything.Checked == true ) )
                {
                    foreach( CheckBox Kart in Karts)
                    {
                        Kart.Checked = true;
                    }
                }
                if (sender.Equals(chkStages) && chkStages.Checked == true || (sender.Equals(chkEverything) && chkEverything.Checked == true))
                {
                    foreach (CheckBox Stage in Stages)
                    {
                        Stage.Checked = true;
                    }
                }
                if(sender.Equals(chkChars) && chkChars.Checked == true || (sender.Equals(chkEverything) && chkEverything.Checked == true))
                {
                    foreach (CheckBox Chara in Chars)
                    {
                        Chara.Checked = true;
                    }
                }
                if (sender.Equals(chkEverything) && chkEverything.Checked == true)
                {
                    chkKart.Checked = true;
                    chkStages.Checked = true;
                    chkChars.Checked = true;
                }
            }
            
        }  
        private void PBChanged(object sender, KeyEventArgs e){
            
            String Name = ((TextBox)sender).Name;


            String[] Abbrev = new string[] { "BP", "PB", "DC", "LC", "MaC", "YC", "MB", "MuC", "WS", "WC", "DDJ", "DKM", "BC", "RR", "DDD", "SL" };
            //Console.WriteLine(Name);
            if (Name.Contains ("1") ){
                // you are editing the course itself, meaning that you are guaranteed a match via index
                // we only want to write upon a valid string, which can be captured with a try except
                
                Name = Name.Replace("1", "");
                if (Name.Contains("flap"))
                {
                    try {
                        
                        TextBox Box = this.Controls.Find("txt" + Abbrev[TTComboBoxIndex] + "flap", true).FirstOrDefault() as TextBox;
                        Box.Text = ((TextBox)sender).Text;
                        TimeTrials[TTComboBoxIndex].FromStringArrayflap(GetflapTimes());
                    } catch
                    {

                    }
                    
                }
                else
                {
                    try
                    {
                        TextBox Box = this.Controls.Find("txt" + Abbrev[TTComboBoxIndex] + "3lap", true).FirstOrDefault() as TextBox;
                        Box.Text = ((TextBox)sender).Text;
                        TimeTrials[TTComboBoxIndex].FromStringArray3lap(Get3lapTimes());
                    } catch
                    {

                    }
                    
                }

                

            }
            else
            {
                //you are editing the pb thing, meaning that you are NOT guaranteed a match
                //check for match
                if ( Name.Contains( Abbrev[TTComboBoxIndex]) )
                {
                    //if the match is found, try to update box and array
                    

                    try
                    {
                        if (Name.Contains("3lap"))
                        {
                            TextBox Box = this.Controls.Find("txtLC3lap1", true).FirstOrDefault() as TextBox;
                            Box.Text = ((TextBox)sender).Text;
                            TimeTrials[TTComboBoxIndex].FromStringArrayflap(GetflapTimes());
                        }
                        else
                        {
                            TextBox Box = this.Controls.Find("txtLCflap1", true).FirstOrDefault() as TextBox;
                            Box.Text = ((TextBox)sender).Text;
                            TimeTrials[TTComboBoxIndex].FromStringArray3lap(Get3lapTimes());
                        }


                    }
                    catch
                    {

                    }
                } else
                {
                    //create a new function for storing back just the time

                    //find the j
                    
                    for (int i = 0; i < 16; i ++ )
                    {
                        if (Name.Contains(Abbrev[i]))
                        {
                            if (Name.Contains("3lap"))
                            {

                                TimeTrials[i].set3lapTime(TimetoMS(((TextBox)sender).Text));
                            }
                            else
                            {
                                TimeTrials[i].setFlapTime(TimetoMS(((TextBox)sender).Text));
                            }
                        }
                    }

                    
                }

            }
            //TimeTrials[TTComboBoxIndex].FromStringArray3lap(Get3lapTimes());
            //TimeTrials[TTComboBoxIndex].FromStringArrayflap(GetflapTimes()); 
        }
        //Ensure that tags are limited to 3 characters
        private void TagChanged(object sender, KeyEventArgs e)
        {
            if (((TextBox)sender).Text.Length > 3)
            {
                String Text = ((TextBox)sender).Text;
                ((TextBox)sender).Text = Text.Substring(0, 3);
            }
        }
        //Edit volume label
        private void tckVolume_Scroll(object sender, EventArgs e)
        {
            lblVolume.Text = "" + tckVolume.Value;
        }

        //emthods that deal with saving .gci files
        private void savegciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeTrials[TTComboBoxIndex].FromStringArray3lap(Get3lapTimes());
            TimeTrials[TTComboBoxIndex].FromStringArrayflap(GetflapTimes());
            GrandPrixs[GPComboBoxIndex].FromStringArray(GetGPTimes());

            String[] Values = GetFormFields();

            byte[] NewBlock = Crypt.EncodeString(Values, ref MemoryCard);
            using (BinaryWriter  writer = new BinaryWriter(File.Open(this.FileName, FileMode.Create)))
            {
                writer.Write(ImageBlock);
                if (MemoryCard.firstispres)
                {
                    writer.Write(MemoryCard.PreservedBlock);
                    writer.Write(NewBlock);                 
                } else
                {
                    writer.Write(NewBlock);
                    writer.Write(MemoryCard.PreservedBlock);
                                
                }
            }
        }

        private String[] GetFormFields()
        {
            String[] Values = new string[372];
            //all values will return in hex
            Values[0] = GetPart1Settings();
            Values[1] = GetUnlockables();
            Values[2] = GetPart2Settings();
            Values[3] = GetTag();

            int Index = 4;
            foreach ( CupRecord cup in GrandPrixs ) {
                Array.Copy(cup.ToStringArray(), 0, Values, Index, 16);
                Index += 16;
            }

           
            foreach (CourseRecords track in TimeTrials)
            {
                Array.Copy(track.ToStringArray3Lap(), 0, Values, Index, 15);
                Index += 15;
            }

            foreach (CourseRecords track in TimeTrials)
            {
                Array.Copy(track.ToStringArrayFLap(), 0, Values, Index, 3);
                Index += 3;
            }


            return Values;
        }
        private String GetPart1Settings()
        {
            int Part1Settings = 0;
            if (radGOff.Checked)
            {
                Part1Settings = Part1Settings += 8;
            } 
            if (radROff.Checked)
            {
                Part1Settings = Part1Settings += 4;
            }
            if (radSurround.Checked)
            {
                Part1Settings = Part1Settings += 2;
            } else if (radMono.Checked)
            {
                Part1Settings = Part1Settings += 1;
            }

            return Part1Settings.ToString("x").PadLeft(8, '0') ;
        }   
        private String GetUnlockables()
        {
            char[] Unlockables = ( "".PadLeft(32, '0') ).ToCharArray();
            if (chkPK.Checked) Unlockables[3] = '1';
            if (chkBP.Checked) Unlockables[4] = '1';
            if (chkPP.Checked) Unlockables[5] = '1';
            if (chkWR.Checked) Unlockables[6] = '1';
            if (chkTB.Checked) Unlockables[7] = '1';
            if (chkBC.Checked) Unlockables[8] = '1';
            if (chkGF.Checked) Unlockables[9] = '1';
            if (chkuwuK.Checked) Unlockables[10] = '1';
            if (chkTK.Checked) Unlockables[11] = '1';
            if (chkBB.Checked) Unlockables[12] = '1';
            if (chkBT.Checked) Unlockables[13] = '1';
            if (chkPW.Checked) Unlockables[14] = '1';
            if (chkRB.Checked) Unlockables[15] = '1';
            
            if (chkTaK.Checked) Unlockables[23] = '1';
            if (chkLM.Checked) Unlockables[24] = '1';
            if (chkSMS.Checked) Unlockables[25] = '1';
            if (chkuwu.Checked) Unlockables[26] = '1';
            if (chkAct.Checked) Unlockables[27] = '1';
            if (chkSpec.Checked) Unlockables[28] = '1';
            if (chkMM.Checked) Unlockables[29] = '1';
            if (chkSMS.Checked) Unlockables[30] = '1';
            if (chkuwu.Checked) Unlockables[31] = '1';

            String Booleans = new string(Unlockables);
            String Value = "";
            for (int i = 0; i <32; i = i +  4)
            {
                uint val = Convert.ToUInt32(Booleans.Substring(i, 4), 2);
                Value += val.ToString("X");
                
            }
            /*
            if (chkEverything.Checked)
            {
               // Value = "FFFFFFFF";
            }
            */
            return Value;
        }
        private String GetPart2Settings()
        {
            Byte Volume = (byte)tckVolume.Value;
            //Console.WriteLine(Volume.ToString("x").PadLeft(2, '0'));
            Byte Item = 0;
            if (radNone.Checked) Item = 1;
            else if (radBasic.Checked) Item = 2;
            else if (radFrantic.Checked) Item = 3;

            Byte Laps = (byte)cmbLaps.SelectedIndex;

            String TagText = txtTag.Text.ToUpper().PadRight(3, ' ');
            Byte FirstChar = Convert.ToByte(  TagText[0] ) ;

            return Volume.ToString("x").PadLeft(2, '0') + Item.ToString("x").PadLeft(2, '0') + Laps.ToString("x").PadLeft(2, '0') + FirstChar.ToString("X").PadLeft(2, '0');
        }
        private String GetTag()
        {
            String TagText = txtTag.Text.ToUpper().PadRight(3, ' ');
            Byte SecondChar = Convert.ToByte(TagText[1]);
            Byte ThirdChar = Convert.ToByte(TagText[2]);
            return SecondChar.ToString("x").PadLeft(2, '0') + ThirdChar.ToString("x").PadLeft(2, '0') + "0000";
        }
        private String[] GetGPTimes()
        {
            String[] GPTimes = new String[16];
            String[] Abbrev = new string[] { "Mush", "Flow", "Star", "Spec", "Act" };
            String[] Speeds = new string[] { "5", "10", "15", "M" };

            int ValIndex = 0;
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    ComboBox CMBBox = this.Controls.Find("cmb" + Abbrev[i] + Speeds[j] + "1p", true).FirstOrDefault() as ComboBox;
                    String CharVehic = CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');
                    Boolean Completed = CMBBox.SelectedIndex != 0;

                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + Speeds[j] + "2p", true).FirstOrDefault() as ComboBox;
                    CharVehic += CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');

                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + Speeds[j] + "V", true).FirstOrDefault() as ComboBox;
                    CharVehic += CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');

                    CharVehic += j.ToString("x").PadLeft(2, '0');

                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + Speeds[j] + "T", true).FirstOrDefault() as ComboBox;
                    String TrophPoint = CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');

                    TextBox Box = this.Controls.Find("txt" + Abbrev[i] + Speeds[j] + "p", true).FirstOrDefault() as TextBox;

                    if (Box.Text.All(char.IsNumber))
                    {
                        int Point;
                        Point = Convert.ToInt32(Box.Text);
                        if (Point > 255)
                        {
                            Point = 255;
                        } else if (Point < 0)
                        {
                            Point = 0;
                        }

                        TrophPoint += Point.ToString("x").PadLeft(2, '0');
                    } else
                    {
                        int Point = 0;
                        TrophPoint += Point.ToString("x").PadLeft(2, '0');
                    }

                    if (Completed) {
                        CheckBox chkBox = this.Controls.Find("chk" + Abbrev[i] + Speeds[j] + "C", true).FirstOrDefault() as CheckBox;
                        if (chkBox.Checked)
                        {
                            TrophPoint += "03ff";
                        } else
                        {
                            TrophPoint += "01ff";
                        }
                        
                    } 
                    else TrophPoint += "0000";
                    

                    Box = this.Controls.Find("txt" + Abbrev[i] + Speeds[j] + "T", true).FirstOrDefault() as TextBox;

                    String TagText = Box.Text.ToUpper().PadRight(3, ' ');
                    Byte    FirstChar = Convert.ToByte(TagText[0]);
                    Byte    SecondChar = Convert.ToByte(TagText[1]);
                    Byte    ThirdChar = Convert.ToByte(TagText[2]);
                    
                    String Tag = FirstChar.ToString("x").PadLeft(2, '0') + SecondChar.ToString("x").PadLeft(2, '0') + ThirdChar.ToString("x").PadLeft(2, '0') + "00";

                    Box = this.Controls.Find("txt" + Abbrev[i] + Speeds[j], true).FirstOrDefault() as TextBox;
                    String Time = TimetoMS(Box.Text);

                    if (!Time.Equals("005b8d7f") && CharVehic.Substring(0, 4) == "0000")
                    {
                        CharVehic = "1210" + CharVehic.Substring(4, 4);
                    } else if  (Time.Equals("005b8d7f"))
                    {
                        CharVehic = "0000FF00";
                        Tag = "00000000";
                        TrophPoint = "00000000";
                    }

                    GPTimes[ValIndex] = CharVehic;
                    GPTimes[ValIndex + 1] = TrophPoint;
                    GPTimes[ValIndex + 2] = Tag;
                    GPTimes[ValIndex + 3] = Time;
                    ValIndex += 4;
                }

            }


            return GPTimes;
        }
        private String[] Get3lapTimes()
        {
            String[] TT3lapTimes = new String[15];
            String[] Abbrev = new string[] { "BP", "PB", "DC", "LC", "MaC", "YC", "MB", "MuC", "WS", "WC", "DDJ", "DKM", "BC", "RR", "DDD", "SL" };
            String[] IDs = new string[] { "21", "22", "23", "24", "25", "26", "28", "29", "2a", "2b", "2c", "2d", "2f", "31", "32", "33" };
            int ValIndex = 0;
            for (int i = 3; i < 4; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    ComboBox CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "3lap" + j + "1p", true).FirstOrDefault() as ComboBox;
                    String CharVehic = CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');

                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "3lap" + j + "2p", true).FirstOrDefault() as ComboBox;
                    CharVehic += CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');

                    CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "3lap" + j + "V", true).FirstOrDefault() as ComboBox;
                    CharVehic += CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');


                    CharVehic += IDs[i];

                    TextBox Box = this.Controls.Find("txt" + Abbrev[i] + "3lap" + j + "T", true).FirstOrDefault() as TextBox;
                    String TagText = Box.Text.ToUpper().PadRight(3, ' ');
                    Byte FirstChar = Convert.ToByte(TagText[0]);
                    Byte SecondChar = Convert.ToByte(TagText[1]);
                    Byte ThirdChar = Convert.ToByte(TagText[2]);

                    String Tag = FirstChar.ToString("x").PadLeft(2, '0') + SecondChar.ToString("x").PadLeft(2, '0') + ThirdChar.ToString("x").PadLeft(2, '0') + "00";

                    Box = this.Controls.Find("txt" + Abbrev[i] + "3lap" + j, true).FirstOrDefault() as TextBox;
                    String Time = TimetoMS(Box.Text);

                    if (!Time.Equals("005b8d7f") && ( CharVehic.Substring(0, 2) == "00" || CharVehic.Substring(2, 2) == "00") )
                    {
                        if (CharVehic.Substring(0, 2) == "00" || CharVehic.Substring(2, 2) == "00")
                        {
                            CharVehic = "1210" + CharVehic.Substring(4, 4);
                        } else if (CharVehic.Substring(0, 2) == "00")
                        {
                            CharVehic = "12" + CharVehic.Substring(2);
                        } else if (CharVehic.Substring(2, 2) == "00")
                        {
                            CharVehic = CharVehic.Substring(0, 2) + "12" + CharVehic.Substring(4, 4);
                        }

                        
                    }
                    else if (Time.Equals("005b8d7f"))
                    {
                        CharVehic = "0000FF00";
                        Tag = "00000000";
                        
                    }
                    //Console.WriteLine(Tag);
                    TT3lapTimes[ValIndex] = CharVehic;
                    TT3lapTimes[ValIndex + 1] = Tag;
                    TT3lapTimes[ValIndex + 2] = Time;
                    ValIndex += 3;
                }
            }
            return TT3lapTimes;
        }
        private String[] GetflapTimes()
        {
            String[] TTflapTimes = new String[3];
            String[] Abbrev = new string[] { "BP", "PB", "DC", "LC", "MaC", "YC", "MB", "MuC", "WS", "WC", "DDJ", "DKM", "BC", "RR", "DDD", "SL" };
            String[] IDs = new string[] { "21", "22", "23", "24", "25", "26", "28", "29", "2a", "2b", "2c", "2d", "2f", "31", "32", "33" };
            int ValIndex = 0;
            for (int i = 3; i < 4; i++)
            {
                
                ComboBox CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "flap" + "1p", true).FirstOrDefault() as ComboBox;
                String CharVehic = CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');

                CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "flap" + "2p", true).FirstOrDefault() as ComboBox;
                CharVehic += CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');

                CMBBox = this.Controls.Find("cmb" + Abbrev[i] + "flap" + "V", true).FirstOrDefault() as ComboBox;
                CharVehic += CMBBox.SelectedIndex.ToString("x").PadLeft(2, '0');

                CharVehic += IDs[i];

                TextBox Box = this.Controls.Find("txt" + Abbrev[i] + "flap" + "T", true).FirstOrDefault() as TextBox;
                    
                String TagText = Box.Text.ToUpper().PadRight(3, ' ');
                Byte FirstChar = Convert.ToByte(TagText[0]);
                Byte SecondChar = Convert.ToByte(TagText[1]);
                Byte ThirdChar = Convert.ToByte(TagText[2]);
                String Tag = FirstChar.ToString("x").PadLeft(2, '0') + SecondChar.ToString("x").PadLeft(2, '0') + ThirdChar.ToString("x").PadLeft(2, '0') + "00";

                Box = this.Controls.Find("txt" + Abbrev[i] + "flap1", true).FirstOrDefault() as TextBox;
                String Time = TimetoMS(Box.Text);
                
                if (!Time.Equals("005b8d7f") && CharVehic.Substring(0, 4) == "0000")
                {
                    CharVehic = "1210" + CharVehic.Substring(4, 4);
                }
                else if (Time.Equals("005b8d7f"))
                {
                    CharVehic = "0000FF00";
                    Tag = "00000000";

                }

                TTflapTimes[ValIndex] = CharVehic;
                TTflapTimes[ValIndex + 1] = Tag;
                TTflapTimes[ValIndex + 2] = Time;
                

                ValIndex += 3;
                
            }
            return TTflapTimes;
        }
    
        private String TimetoMS(String Val)
        {
            // Console.WriteLine(Val);
            if (Val.Length > 0)
            {
                if (Val.Length == 9)
                {
                    int min = Convert.ToInt32("" + Val[0] + Val[1]);
                    int sec = Convert.ToInt32("" + Val[3] + Val[4]);
                    int ms = Convert.ToInt32("" + Val[6] + Val[7] + Val[8]);
                    int time = min * 60000 + sec * 1000 + ms;
                    
                    return time.ToString("X").PadLeft(8, '0');
                } else if (Val.Length ==8)
                {
                    int min = Convert.ToInt32(""+ Val[0]);
                    int sec = Convert.ToInt32("" + Val[2] + Val[3]);
                    int ms = Convert.ToInt32("" + Val[5] + Val[6] + Val[7]);
                    int time = min * 60000 + sec * 1000 + ms;
                    return time.ToString("X").PadLeft(8, '0');
                } else if (Val.Length == 6)
                {
                    int min = 0;
                    int sec = Convert.ToInt32("" + Val[0] + Val[1]);
                    int ms = Convert.ToInt32("" + Val[3] + Val[4] + Val[5]);
                    int time = min * 60000 + sec * 1000 + ms;
                    return time.ToString("X").PadLeft(8, '0');
                } else if (Val.Length == 5)
                {
                    int min = 0;
                    int sec = Convert.ToInt32("" + Val[0]);
                    int ms = Convert.ToInt32("" + Val[2] + Val[3] + Val[4]);
                    int time = min * 60000 + sec * 1000 + ms;
                    return time.ToString("X").PadLeft(8, '0');
                }  else
                {
                    return "005b8d7f";
                }
                
            } else
            {
                return "005b8d7f";
            }
            
        }

        //regions
        private void savegciAsNTSCUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageBlock[3] = 0x45;
            savegciToolStripMenuItem_Click(sender, e);
        }

        private void savegciAsPALToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageBlock[3] = 0x50;
            savegciToolStripMenuItem_Click(sender, e);
        }

        private void savegciAsNTSCJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageBlock[3] = 0x4A;
            savegciToolStripMenuItem_Click(sender, e);
        }

        // Changing the combo box index

        private void cmbTTCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initialized) {
                TimeTrials[TTComboBoxIndex].FromStringArray3lap(Get3lapTimes());
                TimeTrials[TTComboBoxIndex].FromStringArrayflap(GetflapTimes());
                TTComboBoxIndex = cmbTTCourse.SelectedIndex;
                Fill3lapTimes(TimeTrials[TTComboBoxIndex].ToStringArray3Lap());
                FillFlapTimes(TimeTrials[TTComboBoxIndex].ToStringArrayFLap());
            }


            
        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Initialized)
            {
                GrandPrixs[GPComboBoxIndex].FromStringArray(GetGPTimes());
                GPComboBoxIndex = GPComboBox.SelectedIndex;
                FillGPTimes(GrandPrixs[GPComboBoxIndex].ToStringArray());
            }
            
        }
    }
}
