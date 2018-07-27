using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Security.Cryptography;
namespace RS.Net
{
	/// <summary>
	/// MyTransferAgreement 的摘要说明。
	/// </summary>
	class MessageAgreement
	{   
		/// <summary>
		/// 用来临时存储接收到数据数组的集合
		/// </summary>
		int _OutTime=180000;
		/// <summary>
		/// 用来计算超时的计时器
		/// </summary>
		private ManualResetEvent myW=new ManualResetEvent(false);
		private ArrayList byteCollection=new ArrayList();
		private ClientHandler ch;
		public MessageAgreement(ClientHandler sender)
		{
			ch=sender;
			ch.ReceiveOriginalData+=new ClientHandler.ReceiveOriginalDataEvent(Ch_ReceiveOriginalData);
		}

		public int OutTime
		{
			get
			{
				return _OutTime;
			}
			set
			{
				_OutTime=value;
			}
		}
		//   字节长度           1               4          变长   16 
		//传送的数据格式为　（包头（！）     数据包长度　　内容 校验码 ）
		internal byte[] SendDataConverse(byte[] sendData)
		{
			if (sendData.Length<4)
			{
			  int k=sendData.Length;
			}

			byte[] DataToSend=sendData;
			int j=DataToSend.Length;
			byte[] sendBytes = new byte[j+21];
			//数据包长度
			byte[] length=LengthByte(j);
			sendBytes[0]=this.Head();
			Array.Copy(length,0,sendBytes,1,length.Length);
			Array.Copy(DataToSend,0,sendBytes,5,DataToSend.Length);
			
			//产生校验码
			byte[] ckeckCode=MD5Method.MD5hash(sendBytes);
			int m=0;
		　  for(int i=sendBytes.Length-16;i<sendBytes.Length;i++)
		   {
			   sendBytes[i]=ckeckCode[m];
			   m++;
		   }
			return sendBytes;
		}
		private byte Head()
		{
			byte[] bs=Encoding.Default.GetBytes("!");
			return bs[0];
		}
		
		private byte[] LengthByte(int length)
		{
			byte[] bytes=BitConverter.GetBytes(length);
			return bytes;
		}


		//返回true表示接收到包头
        
		internal bool CheckHead(byte[] receiveData)
		{
			if (receiveData.Length==0)
			{
				return false;
			}
			else
			{
				bool Eql=false;
				byte[] bs=Encoding.Default.GetBytes("!");
				byte head=bs[0];
				if(receiveData[0]==head)
				{
					Eql=true;
				}
				return Eql;
			}
		}
			
		//返回true表示数据接收完毕
		internal bool CheckComplete(byte[] receiveData)
		{
			try
			{
				bool Eql=false;
				int lengh=BitConverter.ToInt32(receiveData,1);
				if(lengh==receiveData.Length-21)
				{
					Eql=true;
				}
				return Eql;
			}
			catch
			{
			  return false;
			}
		}
		//返回true则表示接收数据正确
		internal bool CheckReceiveData(byte[] receiveData)
		{
			bool Eql=false;
			byte[] checkCode=MD5Method.MD5hash(receiveData);
			int m=0;
			for(int i=receiveData.Length-16;i<receiveData.Length;i++)
			{
				if(checkCode[m]!=receiveData[i])
				{
					Eql=true;
				}
				m++;
			}
			if(Eql)return false;
			else return true;
		}

		internal byte[] PacketConvertToData(byte[] receiveData)
		{
			byte[] bytes=new byte[receiveData.Length-21];
			Array.Copy(receiveData,5,bytes,0,bytes.Length);
			return bytes;
		}


		internal void NoWait()
		{
		   myW.Set();
		}
		private void Wait()
		{
			bool js=myW.WaitOne(this._OutTime,false);
			if (!js)
			{
                ReceiveDataOutTime?.Invoke();
			}
		}
		/// <summary>
		/// 对所接收到的数据包进行处理，如所接收的数据包合法，则激活已接收事件。
		/// </summary>
		internal void GetFullDatahundle(byte[] bytes,int BytesRead)
		{	
			try
			{                
				byte[] btemp=new byte[BytesRead];
				Array.Copy(bytes,0,btemp,0,btemp.Length);	 
				byteCollection.AddRange((ICollection)btemp);
				if (byteCollection.Count<21)
				{
					Wait();
					return;
				}
				byte[] bytestemp=new byte[byteCollection.Count];
				byteCollection.CopyTo(bytestemp,0);
				bool Havehead=CheckHead(bytestemp);
				bool IsComplete=CheckComplete(bytestemp);
				if(!Havehead)//当重新接收到的数据非文件头时,则表示接收到非法数据
				{
					byteCollection.Clear();
					return;
				}
				else
				{
					//当接收到的是所发送的完整数据包时
					if(IsComplete)
					{
						byteCollection.Clear(); 
						GetDataReceived(bytestemp);
					}
					else
					{ 
						//当不是所发送的数据包时,则有两种可能:一种是接收到的是所发送的一部分,一种是接收到的是两次发送的一部分.
						bool istrue=false;
						do
						{
							istrue=false;
							byte[] tempk=new byte[byteCollection.Count];
							if (tempk.Length<21)
							{
								break;
							}
							else
							{
								byteCollection.CopyTo(tempk,0);
								int lengh=BitConverter.ToInt32(tempk,1)+21;
								if (lengh<tempk.Length)
								{
									istrue=true;
								}
								else
								{
									if (lengh==tempk.Length)
									{
										byteCollection.Clear(); 
										GetDataReceived(tempk);
										break;
									}
									else
									{
										break;
									}
								}
								//当接收到的数据包是两次所发送的数据包时
								byte[] reve=new byte[lengh];
								Array.Copy(tempk,0,reve,0,reve.Length);
								byteCollection.RemoveRange(0,reve.Length); 
								GetDataReceived(reve);
							}
						}while(istrue);
						//开始计时
						Wait();
					}
				}
			}
			catch(Exception e)
			{
                //这里激活一错误事件
                Error?.Invoke(e.Message);
            }
		}

		internal void Close()
		{
			byteCollection.Clear();
			byteCollection=null;
		}

		/// <summary>
		/// 接收到一个完整数据包后，通过以下方法对该数据包进行检查，如合法，通知上层接收，如非法，则激活一错误事件
		/// </summary>
		/// <param name="bytestemp"></param>
		private void GetDataReceived(byte[] bytestemp) 
		{
			bool IsRight=CheckReceiveData(bytestemp);
			if(IsRight)
			{
				byte[] data =PacketConvertToData(bytestemp);
                ReceiveData?.Invoke(data);
            }
			else
			{
                ReceiveData?.Invoke(Encoding.Default.GetBytes("Receive Data Exception!!"));
            }
		} 
		private void OverTimer_GetDataOutTime(object sender, EventArgs e)
		{
            ReceiveDataOutTime?.Invoke();
        }

		/// <summary>
		/// 解析完完整数据包时产生的事件
		/// </summary>
		internal delegate void ReceiveDataEvent(byte[] _recvDataBuffer);
		internal event ReceiveDataEvent ReceiveData;

		/// <summary>
		/// 接收数据超时时产生的事件
		/// </summary>
		internal delegate void ReceiveDataOutTimeEvent();
		internal event ReceiveDataOutTimeEvent ReceiveDataOutTime;

		internal delegate void ErrorEvent(string description);     //出错
		internal event ErrorEvent Error;

		private void Ch_ReceiveOriginalData(byte[] _recvDataBuffer, int recv)
		{
			GetFullDatahundle(_recvDataBuffer,recv);
		}
	}
	class MD5Method
	{
		public static byte[] MD5hash(byte[] MessageBytes)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] HashValue = md5.ComputeHash(MessageBytes,5,MessageBytes.Length-21);
			return HashValue;
		}	
	}
}
