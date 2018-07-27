using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Security.Cryptography;
namespace RS.Net
{
	/// <summary>
	/// MyTransferAgreement ��ժҪ˵����
	/// </summary>
	class MessageAgreement
	{   
		/// <summary>
		/// ������ʱ�洢���յ���������ļ���
		/// </summary>
		int _OutTime=180000;
		/// <summary>
		/// �������㳬ʱ�ļ�ʱ��
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
		//   �ֽڳ���           1               4          �䳤   16 
		//���͵����ݸ�ʽΪ������ͷ������     ���ݰ����ȡ������� У���� ��
		internal byte[] SendDataConverse(byte[] sendData)
		{
			if (sendData.Length<4)
			{
			  int k=sendData.Length;
			}

			byte[] DataToSend=sendData;
			int j=DataToSend.Length;
			byte[] sendBytes = new byte[j+21];
			//���ݰ�����
			byte[] length=LengthByte(j);
			sendBytes[0]=this.Head();
			Array.Copy(length,0,sendBytes,1,length.Length);
			Array.Copy(DataToSend,0,sendBytes,5,DataToSend.Length);
			
			//����У����
			byte[] ckeckCode=MD5Method.MD5hash(sendBytes);
			int m=0;
		��  for(int i=sendBytes.Length-16;i<sendBytes.Length;i++)
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


		//����true��ʾ���յ���ͷ
        
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
			
		//����true��ʾ���ݽ������
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
		//����true���ʾ����������ȷ
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
		/// �������յ������ݰ����д����������յ����ݰ��Ϸ����򼤻��ѽ����¼���
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
				if(!Havehead)//�����½��յ������ݷ��ļ�ͷʱ,���ʾ���յ��Ƿ�����
				{
					byteCollection.Clear();
					return;
				}
				else
				{
					//�����յ����������͵��������ݰ�ʱ
					if(IsComplete)
					{
						byteCollection.Clear(); 
						GetDataReceived(bytestemp);
					}
					else
					{ 
						//�����������͵����ݰ�ʱ,�������ֿ���:һ���ǽ��յ����������͵�һ����,һ���ǽ��յ��������η��͵�һ����.
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
								//�����յ������ݰ������������͵����ݰ�ʱ
								byte[] reve=new byte[lengh];
								Array.Copy(tempk,0,reve,0,reve.Length);
								byteCollection.RemoveRange(0,reve.Length); 
								GetDataReceived(reve);
							}
						}while(istrue);
						//��ʼ��ʱ
						Wait();
					}
				}
			}
			catch(Exception e)
			{
                //���Ｄ��һ�����¼�
                Error?.Invoke(e.Message);
            }
		}

		internal void Close()
		{
			byteCollection.Clear();
			byteCollection=null;
		}

		/// <summary>
		/// ���յ�һ���������ݰ���ͨ�����·����Ը����ݰ����м�飬��Ϸ���֪ͨ�ϲ���գ���Ƿ����򼤻�һ�����¼�
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
		/// �������������ݰ�ʱ�������¼�
		/// </summary>
		internal delegate void ReceiveDataEvent(byte[] _recvDataBuffer);
		internal event ReceiveDataEvent ReceiveData;

		/// <summary>
		/// �������ݳ�ʱʱ�������¼�
		/// </summary>
		internal delegate void ReceiveDataOutTimeEvent();
		internal event ReceiveDataOutTimeEvent ReceiveDataOutTime;

		internal delegate void ErrorEvent(string description);     //����
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
