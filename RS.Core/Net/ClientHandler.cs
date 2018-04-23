using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace RS.Core.Net
{
	/// <summary>
	/// Class1 �ͻ�����ͨ��/����ͨ��.
	/// </summary>
	public class ClientHandler
	{
		/// ��summary�� 
		/// �������ݻ�������С100K 
		/// ��/summary�� 
		private const int DefaultBufferSize =100*1024; 		
		private bool _isClose=true;
		/// <summary>
		/// Ĭ�Ͻ������ݳ�ʱΪ3���֣�������3������δ������һ�����ݰ�ʱ������ֹ���ӡ�
		/// </summary>
		private int _OutTime=180000;
		private int _sendoutTime=180000;
		/// <summary>
		/// �洢��ͻ�ͨ�ŵ�ͨ���ж�
		/// </summary>
        private Dictionary<int, ClientHandler> _sessionTable = new Dictionary<int, ClientHandler>(53);
		/// <summary>
		/// ϵͳ�����ͨ����Ψһ��ʶ
		/// </summary>
		private int _HandlerID=0;		
		/// <summary>
		/// Э����������������������յ����ݰ���
		/// </summary>
		private MessageAgreement agree;
		/// <summary>
		/// Socketͨ���������ͻ�����������˽���ͨ��
		/// </summary>
		private Socket ClientSocket;
		/// <summary>
		/// ��ȡ���ػ�Զ��������Ϣ
		/// </summary>
		private IPEndPoint _LocalEndPoint;  
		private IPEndPoint _RemoteEndPoint;  
		/// <summary>
		/// ö��ͨ������:����������ͨ��,�ͻ��˻Ựͨ��,����˷������ͻ��˽��лỰ��ͨ��ͨ��
		/// </summary>
		private enum _HandlerType{LHandler=1,CHandler,SHandler};
		private int HandlerType;
		/// <summary>
		/// ����������ݻ�����
		/// </summary>
		private byte[] _recvDataBuffer;
		/// <summary>
		/// ��������ɵ����ͻ�������
		/// </summary>
		public const int DefaultMaxClient=100; 	
		/// ��summary�� 
		/// �����ӵ����ͻ����� 
		/// ��/summary�� 
		private int _clientCount;
		/// <summary>
		/// ��ǰ����ͨ����״̬
		/// </summary>
		private bool _isRun=false;

		/// <summary>
		/// ���캯��������ͨ��
		/// </summary>
		/// <param name="port">�������������Ķ˿ں�</param>
		/// <param name="maxClient">�������ɵĿͻ���</param>
		public ClientHandler(int port,int maxClient)
		{
			HandlerType=((int)_HandlerType.LHandler);
			_clientCount=maxClient;
            _sessionTable = new Dictionary<int, ClientHandler>(maxClient);
			_recvDataBuffer = new byte[DefaultBufferSize]; 
			string name =Dns.GetHostName();
			IPAddress ipAddress =Dns.GetHostEntry(name).AddressList[0];
			_LocalEndPoint=new IPEndPoint(ipAddress,port);
			ClientSocket= new Socket(_LocalEndPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
			_RemoteEndPoint=null;
			_HandlerID=(int)ClientSocket.Handle;	
			_isClose=false;
		}
		// ��summary�� 
		/// ���캯��(Ĭ��ʹ��Default���뷽ʽ��DefaultMaxClient(100)���ͻ��˵�����) 
		/// ��/summary�� 
		/// ��param name="port"���������˼����Ķ˿ںš�/param�� 
		public ClientHandler(int port):this( port, DefaultMaxClient) 
		{
		}
		/// <summary>
		/// ���캯�����������˴���һ����ͻ�����ͨ�ŵ�ͨ��
		/// </summary>
		/// <param name="CSocket">��ͻ��˽���ͨ��ϢSocket</param>
		public ClientHandler(Socket CSocket)
		{
            agree = new MessageAgreement(this)
            {
                OutTime = this._OutTime
            };
            HandlerType =(int)_HandlerType.SHandler;
			this.ClientSocket=CSocket;
			_HandlerID=(int)ClientSocket.Handle;
			_recvDataBuffer = new byte[DefaultBufferSize]; 
			ClientSocket.BeginReceive(_recvDataBuffer,0 ,_recvDataBuffer.Length, SocketFlags.None, 
				new AsyncCallback(ReceiveData),ClientSocket);
			ClientSocket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendTimeout,_sendoutTime); 
			_RemoteEndPoint=(IPEndPoint)ClientSocket.RemoteEndPoint;
			_LocalEndPoint=(IPEndPoint)ClientSocket.LocalEndPoint;
			agree.Error+=new MessageAgreement.ErrorEvent(Agree_Error);
			agree.ReceiveData+=new MessageAgreement.ReceiveDataEvent(Agree_ReceiveData);
			agree.ReceiveDataOutTime+=new MessageAgreement.ReceiveDataOutTimeEvent(Agree_ReceiveDataOutTime);
			_isClose=false;
		}

		/// <summary>
		/// �ͻ���ͨ�����캯��
		/// </summary>
		/// <param name="address">Զ�̷������˵�������/IP��ַ/����</param>
		/// <param name="port">Զ�̷������������˿ں�</param>
		public ClientHandler(string address,int port)
		{
            agree = new MessageAgreement(this)
            {
                OutTime = this._OutTime
            };
            HandlerType =(int)_HandlerType.CHandler;
			_recvDataBuffer = new byte[DefaultBufferSize];
            IPAddress ipAddress = Dns.GetHostEntry(address).AddressList[0];
			_RemoteEndPoint=new IPEndPoint(ipAddress,port);
			ClientSocket = new  Socket(_RemoteEndPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp);
			ClientSocket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.SendTimeout,_sendoutTime); 
			_HandlerID=(int)ClientSocket.Handle;
			_isClose=true;
			agree.Error+=new MessageAgreement.ErrorEvent(Agree_Error);
			agree.ReceiveData+=new MessageAgreement.ReceiveDataEvent(Agree_ReceiveData);
			agree.ReceiveDataOutTime+=new MessageAgreement.ReceiveDataOutTimeEvent(Agree_ReceiveDataOutTime);
		}
		
		/// ��summary�� 
		/// �ͻ��˻Ự����,�������еĿͻ���,������Ըü��ϵ����ݽ����޸� 
		/// ��/summary�� 
        public Dictionary<int, ClientHandler> ConnectionPool
		{ 
			get{return _sessionTable;} 
		}

		/// <summary>
		/// ȡ�õ�ǰͨ�����
		/// </summary>
		public int HandlerID
		{
			get{return _HandlerID;}
		}

		/// <summary>
		/// ��ȡ��ǰͨ������״̬
		/// </summary>
		public bool Connected
		{
			get{return ClientSocket.Connected;}
		}
		/// <summary>
		/// �������ӳ�ʱ��ʱ��,�԰ٺ���Ϊ��λ
		/// </summary>
		public int TimeOut
		{
			get
			{
				return _OutTime;
			}
			set
			{
				_OutTime=value;
				agree.OutTime=_OutTime;
			}
		}
		/// ��summary�� 
		/// �������������ɿͻ��˵�������� 
		/// ��/summary�� 
		public int MaxClientCount
		{ 
			get{return _clientCount;} 
		} 

		/// ��summary�� 
		/// ��ǰ�Ŀͻ��������� 
		/// ��/summary�� 
		public int PresentClientCount 
		{ 
			get{return _sessionTable.Count;} 
		} 

		/// ��summary�� 
		/// ����������״̬ 
		/// ��/summary�� 
		public bool IsRun 
		{ 
			get{return _isRun;} 
		} 
		
		/// <summary>
		/// ��ȡ��ǰͨ���ı���������ַ��Ϣ
		/// </summary>
		public IPEndPoint LocalEndPoint
		{
			get{return (IPEndPoint)ClientSocket.LocalEndPoint;}
		}

		/// <summary>
		/// ��ȡ��ǰͨ��ͨ�ŵ�Զ��������ַ��Ϣ
		/// </summary>
		public IPEndPoint RemoteEndPoint
		{
			get{return (IPEndPoint)ClientSocket.RemoteEndPoint;}
		}
					
		/// <summary>
		/// �������˿�ʼ����������Զ�̿ͻ����������򼤻�ConnectionRequest�¼�,����������һ���쳣
		/// </summary>
		public void Start()
		{
			if (HandlerType!=(int)_HandlerType.LHandler )//�����������ͨ��ʱ,�����.
			{
				OnError(this,"Start����ֻ�����ڷ�����������ͨ��!");
				return;
			}
			if(_isRun) 
			{ 
				OnError(this,"����ͨ���Ѿ�����������.");
				return;
			} 
			try
			{
				ClientSocket.Bind(_LocalEndPoint);
				ClientSocket.Listen(_clientCount); 
				_isRun=true;
				ClientSocket.BeginAccept(new AsyncCallback(ClientConnRequest),ClientSocket);
			} 
			catch (Exception e) 
			{				
				OnError(this,e.Message);
			}        
		}

		/// ��summary�� 
		/// �ͻ�����������ʱ������ 
		/// ��param name="iar"�����������������ӵ�Socket����/param�� 
		protected virtual void ClientConnRequest(IAsyncResult iar) 
		{ 
			//���������ֹͣ�˷���,�Ͳ����ٽ����µĿͻ��� 
			if(!_isRun) 
			{ 
				return; 
			}
            try
            {
                //���Զ��������������Ϣ���Եȴ����롣
                Socket oldserver = (Socket)iar.AsyncState;
                ClientHandler client = new ClientHandler(oldserver.EndAccept(iar));
                //�ڷ������˲���һ���ͻ����������¼�
                if (ConnectionRequest != null)
                {
                    try
                    {
                        ConnectionRequest(this, client);
                    }
                    catch 
                    {
                    }      
                }
                //�������������ͻ�����������
                ClientSocket.BeginAccept(new AsyncCallback(ClientConnRequest), ClientSocket);
            }
            catch (Exception e)
            {
                OnError(this, e.Message);
            }
		} 
		
		/// <summary>
		/// �������˽��տͻ�������,������һ����ÿͻ�����ͨ�ŵ�ͨ��,����ͨ�����뵽�Ự�ͻ�������.
		/// </summary>
		/// <param name="CShandler">�������˷����Զ�������Ự��ͨ��</param>
		/// <returns>���ط���ĸ�ͨ��ID��</returns>
		public int Accept(ClientHandler CShandler)
		{
			if (_isClose)
			{
				OnError(this,"ͨ���ѹر�");
				return 0;
			}
			ClientHandler Ch=CShandler; 			
			if(_sessionTable.Count>= _clientCount) 
			{  
                CShandler.Close();
                //����������,����֪ͨ			
                ServerFull?.Invoke(this);
                return 0;
			} 
			else 
			{ 							
				try
				{
                    //����Զ�����Ӽ��뵽�ж���
					int k=Ch.HandlerID;
					this._sessionTable[k]=Ch;
					return Ch.HandlerID;
				}
				catch (Exception e)
				{
					OnError(this,e.Message );
					return 0;
				}				
			}        
		}

		protected virtual void ReceiveData(IAsyncResult iar) 
		{ 
			Socket client = (Socket)iar.AsyncState; 
			try 
			{ 
				if (_isClose)
				{
				   return;
				}
				int recv = client.EndReceive(iar); 
				if( recv > 0 )   //���յ�����
				{ 
					agree.NoWait();
					if (ReceiveOriginalData!=null)
					{
                        try
                        {
                            ReceiveOriginalData(_recvDataBuffer, recv);
                        }
                        catch { }
					}
				} 
				//���������������ͻ��˵����� 
				if (!client.Connected)
				{
					this.Close();
					return;
				}
				client.BeginReceive( _recvDataBuffer, 0, _recvDataBuffer.Length , SocketFlags.None, 
				new AsyncCallback( ReceiveData ), client); 				
			} 
			catch(ObjectDisposedException ex)
			{
				OnError(this,ex.Message);
				_isClose=true;
			}
			catch(InvalidOperationException ex)
			{
				OnError(this,ex.Message);
			}
			catch(SocketException)
			{
				if (HandlerClose!=null)
				{
					int hd=this._HandlerID;
					HandlerClose(this,hd);
				}
				this.Close();
			}
			catch(Exception ex) 
			{ 
				OnError(this,ex.Message);
			} 
		} 
		/// ��summary�� 
		/// �ͻ�ͨ����������
		/// ��/summary�� 
		/// ��param name="datagram"��Ҫ���͵����ݡ�/param�� 
		public void Send(byte[] DataToSend)
		{       
			if (_isClose)
			{
				OnError(this,"ͨ���ѹر�");
				return;
			}
			if (HandlerType==(int)_HandlerType.LHandler )//�����������ͨ��ʱ,�����.
			{
				OnError(this,"������������ͨ������ʹ��Send��������ͨ��!");
				return;
			}
			try
			{
				if (ClientSocket.Connected)
				{                   
					byte[] sendBytes =agree.SendDataConverse(DataToSend);
					IAsyncResult iar;
					iar=ClientSocket.BeginSend(sendBytes,0,sendBytes.Length,SocketFlags.None ,null,null);
					bool b=iar.AsyncWaitHandle.WaitOne();
					if(b)
					{
						SendDataEnd(iar);
					}
				}
				else
				{
					OnError(this,"Զ������δ����,��������ʧ��!");
				}
			}
			catch (Exception e)
			{
				OnError(this,e.Message);
			}		
		}

		/// ��summary�� 
		/// ����������ɴ����� 
		/// ��/summary�� 
		/// ��param name="iar"��Ŀ��ͻ���Socket��/param�� 
		protected virtual void SendDataEnd(IAsyncResult iar) 
		{ 
			try
			{
				//	Socket client = (Socket)iar.AsyncState; 
				if (_isClose)
				{
				   return;
				}
				int sent = ClientSocket.EndSend(iar); 
				//�������������¼�
				if (sent>21)
				{
					sent=sent-21;
				}
				if (this.SendComplete!=null)
				{
					SendComplete(this,sent);
				}				
			}
			catch(Exception e)
			{
				OnError(this,e.Message);
			}
		} 


		/// <summary>
		/// ��������ֹͣ����
		/// </summary>
		public virtual void Stop() 
		{ 
			if (HandlerType!=(int)_HandlerType.LHandler )//�����������ͨ��ʱ,�����.
			{
				OnError(this,"Stop����ֻ�����ڷ�����������ͨ��!");
				return;
			}
			if( !_isRun ) 
			{ 
				OnError(this,"����ͨ��ֹͣ��������."); 
				return;
			} 

			/*���������䣬һ��Ҫ�ڹر����пͻ�����ǰ���� 
			������EndConn����ִ��� */
			_isRun = false; 

			//�ر���������,����ͻ��˻���Ϊ��ǿ�ƹر����� 
			if( ClientSocket.Connected ) 
			{ 
				ClientSocket.Shutdown( SocketShutdown.Both ); 				
			} 

			CloseAllClient(); 
			//������Դ 
			ClientSocket.Close(); 
			_sessionTable = null;
		} 

		/// <summary>
		/// �ͻ�����������Զ�̷���������
		/// </summary>
		public void SocketConnect()
		{
			if (HandlerType!=(int)_HandlerType.CHandler )//�����������ͨ��ʱ,�����.
			{
				OnError(this,"connect����ֻ�����ڿͻ���ͨ��ͨ��!");
				return;
			}
			try
			{
				if (ClientSocket.Connected)
				{
					OnError(this,"�����ӵ�Զ���������������");
					return;
				}
				ClientSocket.BeginConnect(_RemoteEndPoint,new AsyncCallback(ConnectEnd),ClientSocket);
			}
			catch(Exception e)
			{
				OnError(this,e.Message);
			}
		}

		/// <summary>
		/// �����ӳɹ������һ������ʱ�������ϲ�һ�����ӳɹ��¼�������¼���
		/// </summary>
		/// <param name="iar"></param>
		protected virtual void ConnectEnd(IAsyncResult iar) 
		{
			try
			{
				Socket client=(Socket)iar.AsyncState;
				client.EndConnect(iar);
                Connect?.Invoke(this);
                _isClose =false;
				client.BeginReceive( _recvDataBuffer, 0, _recvDataBuffer.Length , SocketFlags.None, 
					new AsyncCallback( ReceiveData ),client); 
				
			}
			catch (Exception e)
			{
				if (Error!=null)
				{
					OnError(this,e.Message);
				}
			}
		}
		/// <summary>
		/// �رշ������˻�ͻ������Ӳ��ͷ����й�������Դ��
		/// </summary>
		public void Close() 
		{
			try
			{
				_isClose=true;
				_sessionTable=null;
				this._HandlerID=0;
				this._isRun=false;
				this._sessionTable=null;
                try
                {
                    if (ClientSocket.Connected)
                        ClientSocket.Shutdown(SocketShutdown.Both);
                }
                catch { }
				this.ClientSocket.Close();
				agree.Close();
				agree=null;				
			}
			catch
			{}		
		}

		/// <summary>
		/// �ر�ĳ���ͻ���ͨ��
		/// </summary>
		/// <param name="ClientHandlerID">ͨ��ͨ����ID��</param>
		public void CloseClient(int ClientHandlerID)
		{
			if (HandlerType!=((int)_HandlerType.LHandler) )//�����������ͨ��ʱ,�����.
			{
				OnError(this,"CloseClient����ֻ�����ڷ�����������ͨ��!");
				return;
			}
            
			ClientHandler k=FindHandler(ClientHandlerID);
			if (k!=null)
			{
				k.Close();
                try
                {
                    this._sessionTable.Remove(ClientHandlerID);
                }
                catch (Exception e)
                {
                    OnError(this, e.Message);
                }
			}
						
		}
		/// <summary>
		/// ����ָ��ID�ŵ�ͨ��ͨ��
		/// </summary>
		/// <param name="handlerid">ID��</param>
		/// <returns>���ҵ�,�򷵻ظ�ͨ��,���򷵻ؿ�</returns>
		public ClientHandler FindHandler(int handlerid)
		{
            if (ConnectionPool.ContainsKey(handlerid))
                return ConnectionPool[handlerid];
            else
                return null;
		}	
		
		/// <summary>
		/// ��������������пͻ��ж�
		/// </summary>
		public virtual void CloseAllClient() 
		{ 		
			foreach(ClientHandler client in _sessionTable.Values) 
			{ 
				client.Close();
			} 
			_sessionTable.Clear();
		}

		//�������¼�
		/// <summary>
		/// ���ӳ�ʱ�¼�
		/// </summary>
		public delegate void GetDataOutTimeEvent(object sender,System.EventArgs e);
		public event GetDataOutTimeEvent GetDataOutTime;
		/// <summary>
		/// �ͻ������ӳɹ��¼�
		/// </summary>
		public delegate void ConnectEvent(object sender);
		public event ConnectEvent Connect;
		
		/// <summary>
		/// ���յ������¼�
		/// </summary>
		public delegate void DataArrivalEvent(object sender, ReceiveEventArgs e);   
		public event DataArrivalEvent DataArrival;
		/// <summary>
		/// ���ۺ�ʱ��ֻҪ��̨�����г��ִ������磬����ʧ�ܣ������ں�̨�շ�����ʧ�ܣ��¼��ͻ���֡�
		/// </summary>
		public delegate void ErrorEvent(object sender,string description);     //����
		public event ErrorEvent Error;
		protected virtual void OnError(object sender,string ErrorString)
		{
            Error?.Invoke(sender, ErrorString);
        }

		internal delegate void ReceiveOriginalDataEvent(byte[] _recvDataBuffer,int recv);   //�����������
		internal event ReceiveOriginalDataEvent ReceiveOriginalData;		
		
		/// <summary>
		/// ���������ݲ������¼�
		/// </summary>
		public delegate void SendCompleteEvent(object sender,int DataNumber);   //�����������
		public event SendCompleteEvent SendComplete;
		/// <summary>
		/// ��Զ�̿ͻ���������ʱ���������¼�
		/// </summary>
		public delegate void ConnectionRequestHandler(object sender,ClientHandler Client);
		public event ConnectionRequestHandler ConnectionRequest;
		/// ��summary�� 
		/// �������пͻ����ӳ��Ѿ����¼� 
		/// ��/summary�� 
		public delegate void ServerFullEvent(object sender);
		public event ServerFullEvent ServerFull; 
		/// <summary>
		/// ͨ���ر��¼�
		/// </summary>
		public delegate void HandlerCloseEvent(object sender,int handlerid);
		public event HandlerCloseEvent HandlerClose;
		private void Agree_Error(string description)
		{
			OnError(this,description);
		}

		private void Agree_ReceiveData(byte[] _recvDataBuffer)
		{
			if (DataArrival!=null)
			{
				ReceiveEventArgs ee=new ReceiveEventArgs(true,_recvDataBuffer,((IPEndPoint)ClientSocket.RemoteEndPoint));
				DataArrival(this,ee);
			}
		}

		private void Agree_ReceiveDataOutTime()
		{
			if (this.GetDataOutTime !=null)
			{
				System.EventArgs e=new EventArgs();
				GetDataOutTime(this,e);
			}
			this.Close();
		}
	}

	public class ReceiveEventArgs : EventArgs 
	{
		private bool isComplete;
		private byte[] receivedata;
		private IPEndPoint ipPoint;
		public  ReceiveEventArgs(bool isComplete,byte[] receivedata,IPEndPoint ipPoint)
		{
			this.isComplete=isComplete;
			this.receivedata=receivedata;
			this.ipPoint=ipPoint;
		}

		public bool IsComplate
		{
			get{return isComplete;}
		}

		public byte[] ReceiveData
		{
			get{return receivedata;}
		}
		public IPEndPoint IpPoint
		{
			get{return ipPoint;}
		}
	}
}
