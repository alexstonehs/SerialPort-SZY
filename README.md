A simple serialport connection helper code. Base on .net Framework 4.5.2 and above

How to use:
ZyCom com = new ZyCom( new ComConnInfo {Com = "COM2",Baud = 9600, DataBit = 8, Parity = Parity.None, StopBit = StopBits.One}););
com.Open();
com.DataReceived += ComOnDataReceived;

private void ComOnDataReceived(byte[] data)
{
  Console.WriteLine($"[{DateTime.Now}]Data received：{Encoding.ASCII.GetString(data)}");
}

void SendData(byte[] data)
{
  com.SendData(data);
}
