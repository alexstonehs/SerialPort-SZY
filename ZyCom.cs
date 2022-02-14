using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;


namespace TYSerialPort
{
    /// <summary>
    /// TJTY串口
    /// </summary>
    public class ZyCom:IDisposable

    {
        /// <summary>
        /// 串口对象
        /// </summary>
        private readonly SerialPort _port;
        /// <summary>
        /// 是否正在接收数据
        /// </summary>
        //private bool _doingReceipting;

        public delegate void ReceiveData(byte[] data);
        /// <summary>
        /// 串口数据接收事件
        /// </summary>
        public event ReceiveData DataReceived;

        public delegate void ComCallBackData(string msg, CallbackType type);
        /// <summary>
        /// 回调消息接收事件
        /// </summary>
        public event ComCallBackData CallBackDataReceived;
        /// <summary>
        /// 接收数据缓存队列
        /// </summary>
        private Queue<byte> _readBuffer;

        //private bool _comOpen = false;

        public ZyCom(ComConnInfo ci)
        {
            //SerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits);
            _port = new SerialPort(ci.Com, ci.Baud, ci.Parity, ci.DataBit, ci.StopBit);
            _readBuffer = new Queue<byte>();
            //_port.DataReceived += PortOnDataReceived;
            if (ci.DtrEnable)
                _port.DtrEnable = true;
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open()
        {
            _port.Open();
            //_comOpen = true;
            TryReadBuffer();
        }
        /// <summary>
        /// 开始读取串口数据
        /// </summary>
        void TryReadBuffer()
        {
            void ComDataReceived()
            {
                DataReceived?.Invoke(_readBuffer.ToArray());
                _readBuffer.Clear();
                _readBuffer = new Queue<byte>();
            }
            Thread readThread = new Thread(() =>
            {
                while (_port.IsOpen)
                {
                    while (_port.BytesToRead > 0)
                    {
                        try
                        {
                            var singleData = _port.ReadByte();
                            if (singleData != -1)
                            {
                                _readBuffer.Enqueue((byte)singleData);
                            }
                            else
                            {
                                ComDataReceived();
                            }
                        }
                        catch (Exception e)
                        {
                            CallBackDataReceived?.Invoke($"{e.Message}|{e.StackTrace}",CallbackType.Error);
                        }
                    }
                    if(_readBuffer.Count > 0)
                    {
                        ComDataReceived();
                    }
                    Thread.Sleep(10);
                }
            }){IsBackground = true};
            readThread.Start();


        }

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //_doingReceipting = true;
                if (!_port.IsOpen)
                {
                    return;
                }
                Thread.Sleep(200);
                var buffer = new byte[_port.BytesToRead];
                _port.Read(buffer, 0, buffer.Length);
                DataReceived?.Invoke(buffer);
            }
            catch (Exception ex)
            {
                CallBackDataReceived?.Invoke($"DataReceived事件异常：{ex.Message}", CallbackType.Error);
            }
            finally
            {
                //_doingReceipting = false;
            }
        }
        ///// <summary>
        ///// 关闭串口
        ///// </summary>
        ///// <returns></returns>
        //public async Task Close()
        //{
        //    if (_port == null)
        //        return;
        //    if (_port.IsOpen)
        //    {
        //        await Task.Run(() =>
        //        {
        //            while (_doingReceipting)
        //            {
        //                Thread.Sleep(500);
        //            }
        //        });
        //        _port.Close();
        //        //_comOpen = false;
        //    }
        //}
        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            if (_port == null)
                return;
            if(_port.IsOpen)
                _port.Close();
        }
        /// <summary>
        /// 发送数据至串口
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data)
        {
            try
            {
                if (_port.IsOpen)
                {
                    _port.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                CallBackDataReceived?.Invoke($"SendData方法异常：{ex.Message}", CallbackType.Error);
            }
        }
        public void Dispose()
        {
            if (_port != null)
            {
                Close();
                _port.Dispose();
            }
            
        }
    }
}
