// Bitmap --> HalconImage
public HObject ConvertBMP(Bitmap bitmap)
{
    HObject halconImage = new HObject();
    BitmapData bmpdata = new BitmapData();
  
    bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
    HOperatorSet.GenImage1(out halconImage, "byte", bitmap.Width, bitmap.Height, bmpdata.Scan0);
    bitmap.UnlockBits(bmpdata);
    bitmap.Dispose();
          
    return halconImage;
}

// string --> HTuple   
// HTuple --> string 强制类型转换
// convert.toInt32()
HTuple myThresh = Int32.Parse(textBox3.Text);


// 对话框保存图片
private void ToolStripMenuItem_Click(object sender, EventArgs e)
{
    SaveFileDialog savefile = new SaveFileDialog();
    savefile.Filter = "BMP文件|*.bmp|所有文件|*.*";
    if(savefile.ShowDialog() == DialogResult.OK)
    {
        string str = savefile.FileName;
        //HOperatorSet.WriteImage(hImage, "bmp", 0, str);
        MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}


//打开对话框
private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
{
    OpenFileDialog dlg = new OpenFileDialog();
    
    if (dlg.ShowDialog() == DialogResult.OK)
    {
        dlg.Filter = "JPG文件|*.jpg|BMP文件|*.bmp|PNG文件|*.png";        
        img = new Image<Bgr, Byte>(dlg.FileName);
    }
    pictureBox1.Image = img.ToBitmap();

    dlg.Dispose();
    
}

//关闭窗口
private void main_FormClosing(object sender, FormClosingEventArgs e)
{
          
     DialogResult close = MessageBox.Show("是否关闭窗口？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
     if (close == DialogResult.Yes)
     {
          e.Cancel = false;
          
          System.Environment.Exit(0);       
     }
     else
     {
          e.Cancel = true;
     }                
}

// 键值
private void main_KeyDown(object sender, KeyEventArgs e)
{
    if(e.KeyCode == Keys.Escape)
    {
         this.Close();
    }
}



// 定时器触发事件
private void button5_Click(object sender, EventArgs e)
{
    System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
    t.Tick += new EventHandler(button2_Click);//触发事件
    t.Interval = 3000;//3000ms
    t.Enabled = true;
    t.Start();
}



// 串口通信简单实现
public Form1()
{
    InitializeComponent();
    TextBox.CheckForIllegalCrossThreadCalls = false;
}

private void Form1_Load(object sender, EventArgs e)
{
    serialPort1.Open();
}

private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
{
    byte[] recv = new byte[serialPort1.BytesToRead];
    serialPort1.Read(recv, 0, recv.Length); //ASCII码接收
    serialPort1.DiscardInBuffer();

    for (int i = 0; i < recv.Length; i++)
    {
        textBox1.Text += recv[i].ToString() + " ";
    }

}

private void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    serialPort1.Close();
}

private void button1_Click(object sender, EventArgs e)
{
    byte[] send = new byte[] { 56,57 }; //ASCII码发送
    serialPort1.Write(send, 0, send.Length);
    
}


// server 程序
private void button1_Click(object sender, EventArgs e)
{
    serverThread = new Thread(new ThreadStart(Listen));
    serverThread.Start();    
}

private void Listen()
{
    Int32 port = 60000;
    IPAddress ip = IPAddress.Parse("192.168.199.209");

    server = new TcpListener(ip, port);
    server.Start();

    Byte[] bytes = new Byte[256];
    String data = null;


    while (true)
    {
        client = server.AcceptTcpClient();
        networkStream = client.GetStream();

        int i;

        while ((i = networkStream.Read(bytes, 0, bytes.Length)) != 0)
        {

            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            textBox1.Text += data;
            data = data.ToUpper();
            //textBox1.Text += data;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

            networkStream.Write(msg, 0, msg.Length);
        }


        //client.Close();

    }
}


// client 程序
private void button1_Click(object sender, EventArgs e)
{

    IPAddress ip = IPAddress.Parse(textBox1.Text);
    Int32 port = Int32.Parse(textBox2.Text);
    
    string hostname = Dns.GetHostEntry(ip).HostName;

    tc = new TcpClient(hostname, port);
    stream = tc.GetStream();
  
    
}

private void button2_Click(object sender, EventArgs e)
{
    
    Byte[] data = System.Text.Encoding.ASCII.GetBytes(textBox3.Text);
    stream.Write(data, 0, data.Length);

    Byte[] rcvdata = new Byte[256];
    string res = string.Empty;
    Int32 bytes = stream.Read(rcvdata, 0, rcvdata.Length);
    res = System.Text.Encoding.ASCII.GetString(rcvdata);
    label4.Text = res;

}

//空白