using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MrhumasMusicOverlay
{
    class HexColor
    {
        private string _Name;
        private string _Hex;

        public string Name { get { return _Name; } set { _Name = value; } }
        public string Hex { get { return _Hex; } set { _Hex = value; } }

        public static List<HexColor> Colors = new List<HexColor>();

        public HexColor(string name, string hex)
        {
            Name = name;
            Hex = hex;
        }

        public Color ConvertToColor()
        {
            string hexValue = Hex;
            Match match = Regex.Match(hexValue, "(#?)([A-Fa-f0-9]{6})");
            //Good: #343434 abcdef
            //Bad: 34343r abcde#f

            //If the Hex code is valid
            if(match.Groups[2].Success)
            {
                hexValue = match.Groups[2].Value;
                //Convert the hex values to bytes
                byte r = ConvertHexToByte(hexValue.Substring(0, 2));
                byte g = ConvertHexToByte(hexValue.Substring(2, 2));
                byte b = ConvertHexToByte(hexValue.Substring(4, 2));

                //Create and return the Color object
                return new Color()
                {
                    R = r,
                    G = g,
                    B = b,
                    A = 255
                };
            }
            else
            {
                //This error should never show up unless someone messes with the Colors file.
                System.Windows.Forms.MessageBox.Show($"The color {Name} doesn't have a valid hex code ({Hex}). This color will be ignored.");
                return new Color();
            }
        }

        public byte ConvertHexToByte(string hex)
        {
            hex = hex.ToLower();
            double total = 0;
            int iteration = 0;
            for (int i = hex.Length - 1; i >= 0; i--)
            {
                switch(hex[i])
                {
                    case '0':
                        total += 0 * Math.Pow(16, iteration);
                        break;
                    case '1':
                        total += 1 * Math.Pow(16, iteration);
                        break;
                    case '2':
                        total += 2 * Math.Pow(16, iteration);
                        break;
                    case '3':
                        total += 3 * Math.Pow(16, iteration);
                        break;
                    case '4':
                        total += 4 * Math.Pow(16, iteration);
                        break;
                    case '5':
                        total += 5 * Math.Pow(16, iteration);
                        break;
                    case '6':
                        total += 6 * Math.Pow(16, iteration);
                        break;
                    case '7':
                        total += 7 * Math.Pow(16, iteration);
                        break;
                    case '8':
                        total += 8 * Math.Pow(16, iteration);
                        break;
                    case '9':
                        total += 9 * Math.Pow(16, iteration);
                        break;
                    case 'a':
                        total += 10 * Math.Pow(16, iteration);
                        break;
                    case 'b':
                        total += 11 * Math.Pow(16, iteration);
                        break;
                    case 'c':
                        total += 12 * Math.Pow(16, iteration);
                        break;
                    case 'd':
                        total += 13 * Math.Pow(16, iteration);
                        break;
                    case 'e':
                        total += 14 * Math.Pow(16, iteration);
                        break;
                    case 'f':
                        total += 15 * Math.Pow(16, iteration);
                        break;
                }

                iteration++;
            }

            return (byte)total;
        }

        static readonly string colorsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Mrhuma's Music Overlay\\";
        static readonly string colorsFile = "Colors.json";

        //Returns if the file exists or not
        public static bool CheckForColorsFile()
        {
            return File.Exists(colorsFolder + colorsFile);
        }

        //Create an empty colors file
        public static void CreateColorsFile()
        {
            Directory.CreateDirectory(colorsFolder);
            using (FileStream fs = File.Create(colorsFolder + colorsFile))
            {
                fs.Dispose();
            }
        }

        //Write to the colors file
        public static void WriteToFile()
        {
            string json = JsonConvert.SerializeObject(Colors, Formatting.Indented);
            File.WriteAllText(colorsFolder + colorsFile, json);
        }

        //Read from the colors file
        public static void ReadFromFile()
        {
            string json = File.ReadAllText(colorsFolder + colorsFile);
            Colors =  JsonConvert.DeserializeObject<List<HexColor>>(json);
        }
    }
}