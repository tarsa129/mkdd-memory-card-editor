using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    
    public class CourseRecords
    {
        private string CourseID;
        private CourseRecord[] Records3Lap;
        private CourseRecord Recordflap;
        public CourseRecords(string CourseID)
        {
            this.CourseID = CourseID;
        }

        public void Make3lapRecords(string[] Values, int Index)
        {
            Records3Lap = new CourseRecord[5];
            for (int i = 0; i < 5; i++)
            {
                Records3Lap[i] = new CourseRecord(Values, Index);
                Index += 3;
            }

        }

        public void MakeflapRecord(String[] Values, int Index)
        {
            Recordflap = new CourseRecord(Values, Index);

        }

        public String[] ToStringArray3Lap()
        {
            String[] Values = new String[15];
            int Index = 0;
            foreach (CourseRecord record in Records3Lap)
            {
                record.ToStringArray(Values, Index, CourseID);
                Index += 3;
            }
            
            return Values;
        }

        public void FromStringArray3lap(String[] Values)
        {
            int Index = 0;
            foreach (CourseRecord record in Records3Lap)
            {
                record.FromStringArray(Values, Index);
                Index += 3;
            }
        }

        public string[] ToStringArrayFLap()
        {
            String[] Values = new String[3];
            int Index = 0;
            this.Recordflap.ToStringArray(Values, Index, CourseID);

            return Values;
        }
        public void FromStringArrayflap(String[] Values)
        {
            int Index = 0;

            this.Recordflap.FromStringArray(Values, Index);

            
        }

        public void setFlapTime(String Time)
        {
            this.Recordflap.setTime(Time);
        }

        public void set3lapTime(String Time)
        {
            this.Records3Lap[0].setTime(Time);
        }
    }

    class CourseRecord
    {
        public int Milliseconds;
        public int OneP;
        public int TwoP;
        public int Vehicle;
        public String Tag;
        public CourseRecord(string[] Values, int Index) : this( Values, Index, false)
        {

            
        }

        public CourseRecord(string[] Values, int Index, bool Gp) {
            this.OneP = Convert.ToInt32(Values[Index][0] + "" + Values[Index][1]);
            this.TwoP = Convert.ToInt32(Values[Index][2] + "" + Values[Index][3]);
            this.Vehicle = Convert.ToInt32(Values[Index][4] + "" + Values[Index][5]);

            if (Gp) {
                Index += 1;
            }
            Byte FirstChar = Convert.ToByte(Values[Index + 1][0]);
            Byte SecondChar = Convert.ToByte(Values[Index + 1][1]);
            Byte ThirdChar = Convert.ToByte(Values[Index + 1][2]);

            this.Tag = FirstChar.ToString("x").PadLeft(2, '0') + SecondChar.ToString("x").PadLeft(2, '0') + ThirdChar.ToString("x").PadLeft(2, '0') + "00";
            
            if (Values[Index + 2].Length == 0)
            {
                this.Milliseconds = 5999999;
            }

            //this.Milliseconds = Convert.ToInt32(Values[Index + 2],16);
            this.Milliseconds = Convert.ToInt32(Values[Index + 2]);
        }


        public String[] ToStringArray(String[] Values, int Index, String CourseID)
        {
            return ToStringArray(Values, Index, CourseID, false);
        }

        public String[] ToStringArray(String[] Values, int Index, String CourseID, bool GP)
        {
            String CharVehic = OneP.ToString("x").PadLeft(2, '0');
            CharVehic += TwoP.ToString("x").PadLeft(2, '0');
            CharVehic += Vehicle.ToString("x").PadLeft(2, '0');
            CharVehic += CourseID.PadLeft(2, '0');

            //Console.WriteLine(Milliseconds);
            // int Millis = Convert.ToInt32(Values[Index + 2]);
            

            Values[Index] = CharVehic;

            if (GP)
            {
                Index += 1;
            }

            Values[Index + 1] = Tag;
            String TimeText = Milliseconds.ToString("x").PadLeft(8, '0');
            //String TimeText = Milliseconds.ToString().PadLeft(8, '0');
            Values[Index + 2] = TimeText;

            return Values;
        }

        public void FromStringArray(String[] Values, int Index) 
        {
            FromStringArray(Values, Index, false);
        }

        public void FromStringArray(String[] Values, int Index, bool GP)
        {
            this.OneP = Convert.ToInt32(Values[Index][0] + "" + Values[Index][1], 16);
            this.TwoP = Convert.ToInt32(Values[Index][2] + "" + Values[Index][3], 16);
            this.Vehicle = Convert.ToInt32(Values[Index][4] + "" + Values[Index][5], 16);

            if (GP)
            {
                Index += 1;
            }

            //Console.WriteLine(Values[Index + 1]);
            /*
            Byte FirstChar = Convert.ToByte(Values[Index + 1][0]);
            Byte SecondChar = Convert.ToByte(Values[Index + 1][1]);
            Byte ThirdChar = Convert.ToByte(Values[Index + 1][2]);
            */
            //this.Tag = FirstChar.ToString("x").PadLeft(2, '0') + SecondChar.ToString("x").PadLeft(2, '0') + ThirdChar.ToString("x").PadLeft(2, '0') + "00";
            this.Tag = Values[Index + 1];
            this.Milliseconds = Convert.ToInt32(Values[Index + 2], 16);
        }

        public void setTime(String Value)
        {
            this.Milliseconds = Convert.ToInt32(Value, 16);
        }
    }

    public class CupRecord
    {
        private String CupID;
        private GPRecord[] CCRecords;

        public CupRecord(String ID)
        {
            this.CupID = ID;
            CCRecords = new GPRecord[4];
        }

        public void MakeGPRecords(String[] Values, int Index)
        {
            for (int i = 0; i < 4; i++)
            {
                CCRecords[i] = new GPRecord(Values, Index);
                Index += 4;
            }
        }

        public void FromStringArray(String[] Values)
        {
            int Index = 0;
            for (int i = 0; i < 4; i++)
            {
                CCRecords[i].FromStringArray(Values, Index);
                Index += 4;
            }
        }

        public String[] ToStringArray()
        {
            String[] Values = new String[16];
            int Index = 0;
            foreach (GPRecord record in CCRecords)
            {
                record.ToStringArray(Values, Index, CupID);
                Index += 4;
            }

            return Values;
        }
       

    }

    class GPRecord :CourseRecord
    {

        public bool CoOp;
        public byte Points;
        public byte Troph;
        
        public GPRecord(string[] Values, int Index) : base(Values, Index, true)
        {
            this.Troph = Convert.ToByte(Values[Index + 1][0] + "" + Values[Index + 1][1], 16);
            this.Points = Convert.ToByte(Values[Index + 1][2] + "" + Values[Index + 1][3], 16);
            this.CoOp = Values[Index + 1][5] == '3';
        }

        public new String[] ToStringArray(String[] Values, int Index, String CourseID)
        {
            String[] ThreeInfo = base.ToStringArray(Values, Index, CourseID, true);
            String TrophPoint = Troph.ToString("x").PadLeft(2, '0');
            TrophPoint += Points.ToString("x").PadLeft(2, '0');

            

            if (ThreeInfo[3] == "5999999")
            {
                TrophPoint += "0000";
            }
            else
            {
                if (CoOp)
                {
                    TrophPoint += "03";
                }
                else
                {
                    TrophPoint += "01";
                }
                TrophPoint += "ff";
            }

            ThreeInfo[Index + 1] = TrophPoint;
            return ThreeInfo;
        }

        public new void FromStringArray(String[] Values, int Index)
        {
            this.Troph = Convert.ToByte(Values[Index + 1][0] + "" + Values[Index + 1][1], 16);
            this.Points = Convert.ToByte(Values[Index + 1][2] + "" + Values[Index + 1][3], 16);
            this.CoOp = Values[Index + 1][5] == '3';
            base.FromStringArray(Values, Index, true);
        }
    }
}
