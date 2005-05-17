import java.io.*;

class MkData {
  public static void main(String[] args) throws Exception {
    byte data[] = {0,     // begin short
		   0x01, 0x12,
		   0x01,  // begin string
		   (byte)'a', (byte)' ', (byte)'t', (byte)'e',
		   (byte)'s', (byte)'t',
		   0x02   // end string
    };
    FileOutputStream f = new FileOutputStream("data");
    f.write(data);
    f.close();
  }
}

