using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WindowsFormsApp1
{
    class Crypt
    {
        private static uint[] SystemFileCrypt(uint r4, byte[] bytes)
        {
            uint r5 = 0x19660d;
            uint r0 = 0;
            uint local_var = r4;

            uint[] Decoded = new uint[373];

            byte[] num_array = new byte[4];
            for (int i = 0; i < 373; i++)
            {
                r0 = local_var;
                r4 = r0 * r5;
                r4 = (uint)(r4 & 0x00FFFFFFFF);
                r4 = r4 + 0x3c6f0000;
                r4 = r4 - 0xca1;
                r4 = (uint)(r4 & 0x00FFFFFFFF);
                local_var = r4;
                if (bytes.Length > 1493)
                {
                    Array.Copy(bytes, 4 * i + 0x40, num_array, 0x0, 0x4);
                } else
                {
                    Array.Copy(bytes, 4 * i, num_array, 0x0, 0x4);
                }
                Array.Reverse(num_array);
                r0 = BitConverter.ToUInt32(num_array, 0);
                r0 = r0 ^ r4;
                //Console.WriteLine(r0.ToString("x") + " " + r0);
                Decoded[i] = r0;
            }
            //Console.WriteLine("-");
            return Decoded;

        }
        
        public static String[] decodeRawBytes(byte[] bytes, ref MemCard MemoryCard)
        {
            byte[] Part1 = new byte[0x2000];
            byte[] Part2 = new byte[0x2000];

            Array.Copy(bytes, 0x0, Part1, 0x0, 0x2000);
            Array.Copy(bytes, 0x2000, Part2, 0x0, 0x2000);

            byte[] HeaderLine = new byte[0x10];
            Array.Copy(bytes, 0x0, HeaderLine, 0x0, 0x10);
            byte[] EndLine1 = new byte[0x10];
            Array.Copy(Part1, 0x1ff0, EndLine1, 0x0, 0x10);
            byte[] EndLine2 = new byte[0x10];
            Array.Copy(Part2, 0x1ff0, EndLine2, 0x0, 0x10);

            //Process Part 1
            byte[] key_array = new byte[] { 0, 0, Part1[0x1ff0], Part1[0x1ff1] };
            Array.Reverse(key_array);
            //Console.WriteLine(key_array);
            uint r4 = BitConverter.ToUInt32(key_array, 0);
            uint[] Part1Decoded = SystemFileCrypt(r4, Part1);
            
            //Process Part 2
            key_array = new byte[] { 0, 0, Part2[0x1ff0], Part2[0x1ff1] };
            Array.Reverse(key_array);
            //Console.WriteLine(key_array);
            r4 = BitConverter.ToUInt32(key_array, 0);
            //Console.WriteLine(r4.ToString("x"));
            uint[] Part2Decoded = SystemFileCrypt(r4, Part2);

            MemoryCard.HeaderLine = HeaderLine;
            if (Part2Decoded[372] > Part1Decoded[372])
            {
                MemoryCard.PreservedBlock = Part1;
                MemoryCard.NewBlock = Part2;
                MemoryCard.PreservedEnd = EndLine1;
                MemoryCard.NewEnd = EndLine2;
                MemoryCard.firstispres = true;
                MemoryCard.NewIndex = Part2Decoded[372];
                return ProcessInts(Part2Decoded);
            } else
            {
                MemoryCard.PreservedBlock = Part2;
                MemoryCard.NewBlock = Part1;
                MemoryCard.PreservedEnd = EndLine2;
                MemoryCard.NewEnd = EndLine1;
                MemoryCard.firstispres = false;
                MemoryCard.NewIndex = Part1Decoded[372];
                return ProcessInts(Part1Decoded);
            }

            
        }

        private static String[] ProcessInts(uint[] IntArray)
        {
            //Basic settings
            String[] ProcessedValues = new string[IntArray.Length - 1];
            ProcessedValues[0] = Convert.ToString( IntArray[0], 2).PadLeft(8, '0');
            //Console.WriteLine(ProcessedValues[0]);
            ProcessedValues[1] = Convert.ToString(IntArray[1], 2).PadLeft(8, '0');
            //Console.WriteLine(ProcessedValues[1]);
            uint EndSettings = (uint)( (IntArray[2] & 0xFFFFFF00) );
            ProcessedValues[2] = EndSettings.ToString("x").PadLeft(8, '0');
            //Console.WriteLine(ProcessedValues[2]);
            char FirstChar = Convert.ToChar( (uint)((IntArray[2] & 0x000000FF)) );
            char SecondChar = Convert.ToChar ( (uint)((IntArray[3] & 0xFF000000) >> 24) );
            char ThirdChar = Convert.ToChar ( (uint)((IntArray[3] & 0x00FF0000) >> 16) );
            ProcessedValues[3] = FirstChar + "" + SecondChar + "" + ThirdChar;
            //Console.WriteLine("tag");
            //Console.WriteLine(ProcessedValues[3]);

            //gp times
            for (int i = 4; i < 84; i = i + 4)
            {
                ProcessedValues[0 + i] = ProcessCharVehicle( IntArray[0 + i].ToString("x").PadLeft(8, '0') );

                String trophy = ( (IntArray[i + 1] & 0xFF000000) >> 24).ToString("x");
                trophy = trophy.PadLeft(2, '0');

                String points;
                if (IntArray[i + 1] > 0)
                {
                   points = ( (IntArray[i + 1] >> 16) & 0x00FF).ToString("x");
                   points = points.PadLeft(2, '0');
                } else
                {
                    points = "00";
                }               


                String render = (IntArray[i + 1] & 0x0000ffff).ToString("x");
                render = render.PadLeft(4, '0');
                ProcessedValues[1 + i] = trophy +  points +  render;
                ProcessedValues[2 + i] = ProcessTag( IntArray[2 + i].ToString("x") );
                if ( (IntArray[i + 1] & 0x0000ffff) != 0)
                {
                    ProcessedValues[3 + i] = IntArray[3 + i].ToString().PadLeft(8, '0');
                } else
                {
                    ProcessedValues[3 + i] = "";
                }
                
            }

            //3lap times
            for (int i = 84; i < 324; i = i + 3)
            {
                ProcessedValues[0 + i] = ProcessCharVehicle( IntArray[0 + i].ToString("x").PadLeft(8, '0'));
                ProcessedValues[1 + i] = ProcessTag(IntArray[1 + i].ToString("x") );
                if (IntArray[2 + i] != 5999999)
                {
                    ProcessedValues[2 + i] = IntArray[2 + i].ToString().PadLeft(8, '0');
                }
                else
                {
                    ProcessedValues[2 + i] = "";
                }
                
            }

            //flap times
            for (int i = 324; i < 372; i = i + 3)
            {
                ProcessedValues[0 + i] = ProcessCharVehicle(IntArray[0 + i].ToString("x").PadLeft(8, '0'));
                ProcessedValues[1 + i] = ProcessTag(IntArray[1 + i].ToString("x"));
                if (IntArray[2 + i] != 5999999)
                {
                    ProcessedValues[2 + i] = IntArray[2 + i].ToString().PadLeft(8, '0');
                }
                else
                {
                    ProcessedValues[2 + i] = "";
                }
            }
            return ProcessedValues;
        }

        private static String ProcessCharVehicle(String HexValues)
        {
            //Console.WriteLine(HexValues);
            int FirstChar = Convert.ToInt32("0x" + HexValues[0] + HexValues[1], 16);
            int SecondChar = Convert.ToInt32("0x" + HexValues[2] + HexValues[3], 16);
            int Vehicle = Convert.ToInt32("0x" + HexValues[4] + HexValues[5], 16);
            if (Vehicle > 20)
            {
                Vehicle = 0;
            }
            return FirstChar.ToString().PadLeft(2, '0') + SecondChar.ToString().PadLeft(2, '0') + Vehicle.ToString().PadLeft(2, '0');
        }

        private static String ProcessTag(String HexValues)
        {
            if (HexValues.Length > 1)
            {
                //Console.WriteLine(HexValues);
                
                //Console.WriteLine(HexValues[0] + HexValues[1] ) ;
                char FirstChar  = Convert.ToChar( Convert.ToUInt32( ""+ HexValues[0] + HexValues[1], 16));
                char SecondChar = Convert.ToChar(Convert.ToUInt32("" + HexValues[2] + HexValues[3], 16));
                char ThirdChar = Convert.ToChar(Convert.ToUInt32("" + HexValues[4] + HexValues[5], 16));
                //Console.WriteLine(FirstChar + "" + SecondChar + "" + ThirdChar);
                return FirstChar + "" + SecondChar + "" + ThirdChar;
            } else
            {
                return "   ";
            }
            
        }

        //saving protocol
        public static Byte[] EncodeString(String[] HexValues, ref MemCard MemoryCard)
        {
            
            byte[] ByteValues = new byte[0x2000];

            
            //header text
            byte[] HeaderText = new byte[0x40];
            Array.Copy(MemoryCard.NewBlock, HeaderText, 0x40);
            /*
            Array.Copy(Encoding.ASCII.GetBytes("Mario Kart: Double Dash!! "), HeaderText, 0x1a);
            DateTime Today = DateTime.Now;
            byte[] DateText = Encoding.ASCII.GetBytes("Game Data " + Today.Month.ToString().PadLeft(2, '0') + "/"
                + Today.Day.ToString().PadLeft(2, '0') + "/" + Today.Year.ToString().Substring(2, 2) );
            Array.Copy(DateText, 0x0, HeaderText, 0x20, 0x12);
            */
            Array.Copy(HeaderText, ByteValues, 0x40);

            //Add index into stuff to be encoded
            String[] HexValuesP = new string[373];
            Array.Copy(HexValues, HexValuesP, HexValues.Length);
            HexValuesP[372] = MemoryCard.NewIndex.ToString("x").PadLeft(8, '0');

            //convert string array to byte array to be encoded
            byte[] Values = new byte[373 * 4];
            for ( int i = 0; i < HexValuesP.Length; i += 1)
            {

                Values[4 *i] = (byte)Convert.ToInt32( HexValuesP[i].Substring(0, 2) , 16);
                Values[4 * i + 1] = (byte)Convert.ToInt32(HexValuesP[i].Substring(2, 2), 16);
                Values[4 * i + 2] = (byte)Convert.ToInt32(HexValuesP[i].Substring(4, 2), 16);
                Values[4 * i + 3] = (byte)Convert.ToInt32(HexValuesP[i].Substring(6, 2), 16);
                

            }

            byte[] EndLine = new byte[0x10];
            Array.Copy(MemoryCard.NewBlock, 0x1ff0, EndLine, 0x0, 0x10);
            byte[] key_array = new byte[] { 0, 0, EndLine[0x00], EndLine[0x01] };
            Array.Reverse(key_array);
            uint r4 = BitConverter.ToUInt32(key_array, 0);
            //Console.WriteLine( r4.ToString("x") );

            uint[] Encoded = SystemFileCrypt(r4, Values);
            /*
            for(int i = 0; i < Encoded.Length; i++)
            {
                Console.WriteLine(Encoded[i].ToString("x"));
            }
            */
            Array.Copy(EndLine, 0x0, ByteValues, 0x1ff0, 0x10);
            
            for (int i = 0; i < Encoded.Length; i ++)
            {
                String HexValue = Encoded[i].ToString("x").PadLeft(8, '0');
                ByteValues[4 * i + 0x40] = (byte)Convert.ToInt32(HexValue.Substring(0, 2), 16);
                ByteValues[4 * i + 1 + 0x40] = (byte)Convert.ToInt32(HexValue.Substring(2, 2), 16);
                ByteValues[4 * i + 2 + 0x40] = (byte)Convert.ToInt32(HexValue.Substring(4, 2), 16);
                ByteValues[4 * i + 3 + 0x40] = (byte)Convert.ToInt32(HexValue.Substring(6, 2), 16);

            }

            byte[] Filler = new byte[0x19dc];
            Array.Copy(MemoryCard.NewBlock, 0x40 + 373 * 4, Filler, 0x0, 0x19dc);
            Array.Copy(Filler, 0x0, ByteValues, 0x40 + 373 * 4, 0x19dc);
            //Array.Copy(MemoryCard.PreservedBlock, 0x40 + 373 * 4, ByteValues, 0x40 + 373 * 4, 0x19dc);
            //Array.Copy(GetFiller(), 0x0, ByteValues, 373 * 4 + 0x40, 0x19dc);

            byte[] CRCArray = new byte[0x2000];
            Array.Copy(HeaderText, CRCArray, 0x40);
            Array.Copy(Values, 0x0, CRCArray, 0x40, Values.Length);
            Array.Copy(Filler, 0x0, CRCArray, 0x40 + 373 * 4, 0x19dc);
            Array.Copy(EndLine, 0x0, CRCArray, 0x40 + 373 * 4 + 0x19dc, 0xc);

            byte[] getCRC = GetCRC(CRCArray);
            Array.Copy(getCRC, 0x0, ByteValues, 0x1ffc, 0x4);
            return ByteValues;
            
        }

        private static byte[] GetFiller()
        {
            uint local_38 = 0x18f;
            uint r0 = 0x19dc;
            uint r4 = 0x19660d;
            uint r5 = 0;

            byte[] Filler = new byte[0x19dc];

            for( int i = 0; i < 0x19dc; i++)
            {
                uint r3 = local_38;
                r0 = r5 + 0x2634;
                r5 += 1;
                r3 = r3 * r4;

                r3 = r3 & 0x0FFFFFFFF;

                r3 = r3 + 0x3c6f0000;

                r3 = r3 - 0xca1;

                r3 = r3 & 0x0FFFFFFFF;

                local_38 = r3;
                r3 = (r3 >> 24) & 0x000000FF;

                Filler[i] = (byte)r3;

            }
            return Filler;
        }

        private static byte[] GetCRC(byte[] Block)
        {
            byte[] CRC = new byte[4];
            uint[] Keys = new uint[0x100];
            uint r10 = 0xffffffff;

            uint r8 = 0x0;
            uint r3 = 0x0;
            uint r0 = 0x8;
            
            

            do
            {
                uint r9 = r8;
                for (int i = 0; i < 8; i++ ) {
                    uint r7 = r9 & 0x00000001;
                    r9 = (r9 >> 0x1) & 0x7FFFFFFF;
                    if (r7 != 0) r9 ^= 0xedb88320;
                    //Console.WriteLine(r9.ToString("x"));
                }
                r8 += 1;
                Keys[r3] = r9;
                //Console.WriteLine(r9.ToString("x"));
                //Console.WriteLine("-");
                r3 += 1;
            } while (r8 < 0x100);
            
            for(int i = 0; i < 0x1ffc; i ++)
            {
                r0 = Block[i];
                //Console.WriteLine(r0.ToString("x"));
                uint r5 = ((r10 << 0x18) | (r10 >> 0x8)) & 0x00FFFFFF;
                r0 = r10 ^ r0;
                //Console.WriteLine(r0.ToString("x"));
                r0 = ((r0 << 0x2) | (r0 >> 0x1e)) & 0x000003FC;
                //Console.WriteLine(r0.ToString("x"));
                r0 = Keys[r0  / 4];
                //Console.WriteLine(r0.ToString("x"));
                r10 = r5 ^ r0;
                //Console.WriteLine(r10.ToString("x"));
                //Console.WriteLine("-");

            }
            r0 = 0xffffffff;
            r3 = r10 ^ r0;


            CRC = BitConverter.GetBytes(r3);
            Array.Reverse(CRC);
            Console.WriteLine(r3.ToString("x"));
            return CRC;
        }
    }
}
