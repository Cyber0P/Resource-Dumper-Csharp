using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace ResourceDumper_CS
{
    class Program
    {

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]String lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr FindResource(IntPtr hModule, String lpName, uint lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        private static String Path;
        private static String DTN;
        private static String ResName;
        static void Main(String[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("The command is => -Path -DateTypeNumber -Resource Name");
            Console.WriteLine("===============================");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("DateTypeNumbers:");
            Console.WriteLine("CURSOR (1)\nBITMAP (2)\nICON (3)\nMENU (4)\nDIALOG (5)\nSTRING (6)\nFONTDIR (7)\nFONT (8)\nACCELERATOR (9)\nRCDATA (10)\nMESSAGETABLE (11)");
            Console.WriteLine("===============================");

            again: String input = Console.ReadLine();

            if (Checkinput(input))
            {
                Dump();
            }
            else
            {
                Console.WriteLine("Check the input again!");
                goto again;
            }
            Console.Read();
        }

        private static Boolean Checkinput(String input)
        {  
            
            try
            {
               String a = input;
               String[] arr = a.Split('-');
          
                if (File.Exists(arr[1]) && (Convert.ToInt32(arr[2]) >= 1 && Convert.ToInt32(arr[2]) <= 11))
                {
                    Path = arr[1].Replace(" ", "");
                    DTN = arr[2];
                    ResName = arr[3];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

      private static void Dump() 
      {

     IntPtr hModule = LoadLibrary(Path);
          
	if (hModule == IntPtr.Zero) 
    {
		ErrorShow();
	}
	else 
      {
		IntPtr hRes = FindResource(hModule, ResName, Convert.ToUInt32(DTN));
		if (hRes == IntPtr.Zero) 
        {
			ErrorShow();
		}
		else
           {
			IntPtr lRes = LoadResource(hModule, hRes);
			if (lRes == IntPtr.Zero) 
            {
				ErrorShow();
			}
			else 
               {
				uint ResSize = SizeofResource(hModule, hRes);
				if (ResSize != 0) {
					Byte[] ReourceRAW = new Byte[ResSize];

					Marshal.Copy(lRes, ReourceRAW, 0, (Int32) ResSize);

					File.WriteAllBytes(Path + ".bin", ReourceRAW);
					FreeLibrary(hModule);

					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.WriteLine("Done");
					Console.WriteLine("Saved in: " + Path + ".bin");
				}
			   }
	    	}
     	}
    }

        private static void ErrorShow()
        {
            Int32 lasterror = Marshal.GetLastWin32Error();
            Win32Exception innerEx = new Win32Exception(lasterror);
            Console.WriteLine(innerEx);
        }
    }
}
