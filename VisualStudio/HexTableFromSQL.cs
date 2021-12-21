using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

class Configuration
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Server { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string StructureFolder { get; set; }
    public string OutputFolder { get; set; }
}

namespace ConsoleApp1
{
    class Program
    {
        static string StructurePath = string.Empty;
        static string[] Offset = new string[500];

        static void Main(string[] args)
        {
            // read config file
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

            var myConfig = deserializer.Deserialize<Configuration>(File.ReadAllText( AppDomain.CurrentDomain.BaseDirectory + @"\config.yaml"));

            // not necessary
            string temp_name = Environment.MachineName;
            Console.WriteLine("HELLO, MR." + temp_name.Remove(0, 4));

            // provide connection details
            SqlConnection myConnection = new SqlConnection(
            "user id=" + myConfig.UserName + ";" +
            "password=" + myConfig.Password + ";" +
            "server=" + myConfig.Server + "," + myConfig.Port + ";" +
            "Trusted_Connection=yes;" +
            "database=" + myConfig.Database + "; ");

            // Begin Main part
            // Turn on the connection
            Program.SwitchSQLConnection(myConnection);

            // get list of files to compile
            string[] fileNames = Program.FindUpdatedFiles(myConnection);

            // output files to assemble for debug purposes
            Console.WriteLine("fileNames.Length " + fileNames.Length);

            // update extra tables and set all tables as compile = 0
            Program.BeforeCompilation(myConnection);

            for (int f = 0; f < fileNames.Length; f++)
            {
                // get which structure we need and load it
                StructurePath = myConfig.StructureFolder + "Structure_" + fileNames[f] + ".txt";
                Console.WriteLine("StructurePath " + StructurePath);

                //Get names of all tables from structure file(1st line)
                string[] TableNames = Program.GetStructureData(0);

                Program.Offset[0] = "0";
                using (System.IO.FileStream stream = File.Open(myConfig.OutputFolder + fileNames[f] + ".dat", FileMode.OpenOrCreate))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        long offset = -4;
                        //go through all the tables, retrieving data
                        for (int i = 0; i < TableNames.Length; i += 1)
                        {
                            //writer.Write(Program.Convert(Program.GetTableQTY(myConnection, TableNames[i]), "u32"));
                            offset = writer.BaseStream.Length;
                            string[] Columns = Program.GetStructureData(1 + i * 2);
                            string[] ColType = Program.GetStructureData(2 + i * 2);

                            // this line is where we actually retrieve the data, convert it and append to file
                            Program.RetrieveSQLData(myConnection, TableNames[i], Columns, ColType, stream, writer);

                            offset = writer.BaseStream.Length;
                            Program.Offset[i + 1] = offset.ToString();
                        }
                    }
                }
            }
            Program.AfterCompilation(myConnection);
            // Turn off the connection
            Program.SwitchSQLConnection(myConnection);
            // so that the window will not close
            // Console.ReadLine();
        }

        // this function works as a on/off switch for the sql connection
        static void SwitchSQLConnection(SqlConnection myConnection) {

            if (myConnection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else if (myConnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    myConnection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                Console.WriteLine("Failed to open or close connection");
            }

            Console.WriteLine("SQL connection state: " + myConnection.State);
        }

        // read structure file, get info we neeed to know how to read sql data
        static string[] GetStructureData(int TableNum)
        {
            string[] output;
            string[] Structure = File.ReadAllLines(StructurePath);
            output = Structure[TableNum].Split(',');
            return output;
        }

        // actually access sql, append text to file
        static void RetrieveSQLData(SqlConnection myConnection, string TableName, string[] Columns, string[] ColType, System.IO.FileStream stream, BinaryWriter writer)
        {
            if (myConnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    SqlDataReader myReader = null;
                    SqlCommand myCommand = new SqlCommand("select * from " + TableName + " order by " + Columns[0], myConnection);

                    myReader = myCommand.ExecuteReader();
                    while( myReader.Read() )
                    {
                        for (int i = 0; i < Columns.Length; i++) 
                        {
                            object ColValue = string.Empty;

                            if (Columns[i] == "Unknown")
                            {
                                ColValue = "0";
                            }
                            else
                            {
                                // optimization needed
                                ColValue = myReader[Columns[i]];
                            }
                            writer.Write(Program.Convert(ColValue, ColType[i]));
                        }
                    }
                    myReader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else {
                Console.WriteLine("RetrieveSQLData: Failed" + '\n' + "Connection State: " + myConnection.State);
            }
        }

        // most important function that must convert data differently depending on the DataType (example: u32, float, cstr, etc. )
        static byte[] Convert(object input, string DataType)
        {
            byte[] output = {0};

            if (DataType == "i8")
            {
                byte[] temp = BitConverter.GetBytes(Int16.Parse(input.ToString()));
                output = new byte[] { temp[0] };
            }
            else if (DataType == "i16")
            {
                byte[] temp = BitConverter.GetBytes(Int16.Parse(input.ToString()));
                output = temp;
            }
            else if (DataType == "i32")
            {
                byte[] temp = BitConverter.GetBytes(Int32.Parse(input.ToString()));
                output = temp;
            }
            else if (DataType == "i64")
            {
                byte[] temp = BitConverter.GetBytes(Int64.Parse(input.ToString()));
                output = temp;
            }
            else if (DataType == "u32")
            {
                byte[] temp = BitConverter.GetBytes(UInt32.Parse(input.ToString()));
                output = temp;
            }
            else if (DataType == "u64")
            {
                byte[] temp = BitConverter.GetBytes(UInt64.Parse(input.ToString()));
                output = temp;
            }
            else if (DataType == "float")
            {
                byte[] temp = BitConverter.GetBytes(float.Parse(input.ToString()));
                output = temp;
            }
            else if (DataType == "double")
            {
                byte[] temp = BitConverter.GetBytes(double.Parse(input.ToString()));
                output = temp;
            }
            else if (DataType == "cstr")
            {
                byte[] temp = Encoding.Default.GetBytes(input.ToString());
                int L = temp.Length;
                byte[] L_byte = Convert(L.ToString(), "u32");

                byte[] result = new byte[L_byte.Length + temp.Length];
                Array.Copy(L_byte, result, L_byte.Length);
                Array.Copy(temp, 0, result, L_byte.Length, temp.Length); 

                output = result; 
            }
            else if (DataType == "cstr2560")
            {
                byte[] temp = Encoding.Default.GetBytes(input.ToString());

                byte[] result = new byte[2560];
                Array.Copy(temp, result, temp.Length);

                output = result;
            }
            else if (DataType == "cstr64")
            {
                byte[] temp = Encoding.Default.GetBytes(input.ToString());

                byte[] result = new byte[64];
                Array.Copy(temp, result, temp.Length);

                output = result;
            }
            else if (DataType == "cstr39")
            {
                byte[] temp = Encoding.Default.GetBytes(input.ToString());

                byte[] result = new byte[39];
                Array.Copy(temp, result, temp.Length);

                output = result;
            }
            else if (DataType == "cstr36")
            {
                byte[] temp = Encoding.Default.GetBytes(input.ToString());

                byte[] result = new byte[36];
                Array.Copy(temp, result, temp.Length);

                output = result;
            }
            else if (DataType == "cstr33")
            {
                byte[] temp = Encoding.Default.GetBytes(input.ToString());

                byte[] result = new byte[33];
                Array.Copy(temp, result, temp.Length);

                output = result;
            }
            else if (DataType == "cstr32")
            {
                byte[] temp = Encoding.Default.GetBytes(input.ToString());

                byte[] result = new byte[32];
                Array.Copy(temp, result, temp.Length);

                output = result;
            }
            else if (DataType == "cstr8")
            {
                byte[] temp = Encoding.Default.GetBytes(input.ToString());

                byte[] result = new byte[8];
                Array.Copy(temp, result, result.Length);

                output = result;
            }
            else if (DataType == "byte")
            {
                output = ByteSwap((byte[])input);
            }
            else if (DataType == "byte8")
            {
                output = ByteSwap((byte[])input);
            }
            else
            {
                Console.WriteLine("Convert: DataType not recognized: " + DataType);
                Console.ReadLine();
            }

            return output;
        }

        // switch numbers in the HEX
        // 1234 becomes 3412
        static byte[] ByteSwap(byte[] stringToSwap)
        {
            byte[] original = stringToSwap;
            byte[] newA = new byte[original.Length];

            int maxIndex = newA.Length - 1;

            for (int b = 0; b < newA.Length; b++)
            {
                newA[b] = original[maxIndex - b];
            }

            return newA;
        }

        // Получаем список файлов для сборки
        static string[] FindUpdatedFiles(SqlConnection myConnection)
        {
            List<string> result = new List<string>();
            if (myConnection.State == System.Data.ConnectionState.Open)
            {
                try
                {
                    SqlDataReader myReader = null;
                    SqlCommand myCommand = new SqlCommand( "select TableName from Info where Compile = 1", myConnection );

                    myReader = myCommand.ExecuteReader();
                    while( myReader.Read() )
                    {
                        object ColValue = string.Empty;
                        ColValue = myReader[0];
                        result.Add(ColValue.ToString());
                    }
                    myReader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                Console.WriteLine("RetrieveSQLData: Failed" + '\n' + "Connection State: " + myConnection.State);
            }
            return result.ToArray();
        }

        // Применяем скрипт до сборки
        static void BeforeCompilation(SqlConnection myConnection)
        {
            try
            {
                string Command = File.ReadAllText( AppDomain.CurrentDomain.BaseDirectory + @"\BeforeCompilation.txt" );

                SqlCommand myCommand = new SqlCommand(Command, myConnection);
                myCommand.ExecuteNonQuery();
                Console.WriteLine("BeforeCompilation: Finished");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // Применяем скрипт после сборки
        static void AfterCompilation(SqlConnection myConnection)
        {
            try
            {
                string Command = File.ReadAllText( AppDomain.CurrentDomain.BaseDirectory + @"\AfterCompilation.txt" );

                SqlCommand myCommand = new SqlCommand(Command, myConnection);
                myCommand.ExecuteNonQuery();
                Console.WriteLine("AfterCompilation: Finished");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}