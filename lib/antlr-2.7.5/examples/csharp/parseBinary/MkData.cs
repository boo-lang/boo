using System;
using System.IO;

class MkData {
  public static void Main(string[] args) {
    byte[] data = {0x00,     // begin short
		   0x01, 0x12,
		   0x01,  // begin string
		   (byte)'a', (byte)' ', (byte)'t', (byte)'e',
		   (byte)'s', (byte)'t',
		   0x02   // end string
    };
    FileStream fs = new FileStream("data", FileMode.Create, FileAccess.Write, FileShare.Read, 32, false);
    fs.Write(data, 0, data.Length);
    fs.Flush();
    fs.Close();
  }
}

