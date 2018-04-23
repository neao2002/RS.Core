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
	/// Class1 客户连接通道/侦听通道.
	/// </summary>
	public class ClientHandler
	{
		/// 〈summary〉 
		/// 接收数据缓冲区大小100K 
		/// 〈/summary〉 
		private const int DefaultBufferSize =100*1024; 		
		private bool _isClose=true;
		/// <summary>
		/// 默认接收数据超时为3分种，即当在3分种内未接收完一个数据包时，则中止连接。
		/// </summary>
		private int _OutTime=180000;
		private int _sendoutTime=180000;
		/// <summary>
		/// 存储与客户通信的通道列队
		/// </summary>
        private Dictionary<int, ClientHandler> _sessionTable = new Dictionary<int, ClientHandler>(53);
		/// <summary>
		/// 系统分配给通道的唯一标识
		/// </summary>
		private int _HandlerID=0;		
		/// <summary>
		/// 协议解析器，用来解析所接收的数据包。
		/// </summary>
		private MessageAgreement agree;
		/// <summary>
		/// Socket通道，用来客户端与服务器端进行通信
		/// </summary>
		private Socket ClientSocket;
		/// <summary>
		/// 获取本地或远程主机信息
		/// </summary>
		private IPEndPoint _LocalEndPoint;  
		private IPEndPoint _RemoteEndPoint;  
		/// <summary>
		/// 枚举通道类型:服务器侦听通道,客户端会话通道,服务端分配的与客户端进行会话的通信通道
		/// </summary>
		private enum _HandlerType{LHandler=1,CHandler,SHandler};
		private int HandlerType;
		/// <summary>
		/// 定义接收数据缓冲区
		/// </summary>
		private byte[] _recvDataBuffer;
		/// <summary>
		/// 定义可容纳的最大客户连接数
		/// </summary>
		public const int DefaultMaxClient=100; 	
		/// 〈summary〉 
		/// 可连接的最大客户端数 
		/// 〈/summary〉 
		private int _clientCount;
		/// <summary>
		/// 当前侦听通道的状态
		/// </summary>
		private bool _isRun=false;

		/// <summary>
		/// 构造函数：侦听通道
		/// </summary>
		/// <param name="port">服务器端侦听的端口号</param>
		/// <param name="maxClient">最大可容纳的客户数</param>
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
		// 〈summary〉 
		/// 构造函数(默认使用Default编码方式和DefaultMaxClient(100)个客户端的容量) 
		/// 〈/summary〉 
		/// 〈param name="port"〉服务器端监听的端口号〈/param〉 
		public ClientHandler(int port):this( port, DefaultMaxClient) 
		{
		}
		/// <summary>
		/// 构造函数：服务器端创建一个与客户进行通信的通道
		/// </summary>
		/// <param name="CSocket">与客户端进行通信息Socket</param>
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
		/// 客户端通道构造函数
		/// </summary>
		/// <param name="address">远程服务器端的主机名/IP地址/域名</param>
		/// <param name="port">远程服务器端主机端口号</param>
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
		
		/// 〈summary〉 
		/// 客户端会话集合,保存所有的客户端,不允许对该集合的内容进行修改 
		/// 〈/summary〉 
        public Dictionary<int, ClientHandler> ConnectionPool
		{ 
			get{return _sessionTable;} 
		}

		/// <summary>
		/// 取得当前通道句柄
		/// </summary>
		public int HandlerID
		{
			get{return _HandlerID;}
		}

		/// <summary>
		/// 获取当前通道连接状态
		/// </summary>
		public bool Connected
		{
			get{return ClientSocket.Connected;}
		}
		/// <summary>
		/// 设置连接超时的时间,以百毫秒为单位
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
		/// 〈summary〉 
		/// 服务器可以容纳客户端的最大能力 
		/// 〈/summary〉 
		public int MaxClientCount
		{ 
			get{return _clientCount;} 
		} 

		/// 〈summary〉 
		/// 当前的客户端连接数 
		/// 〈/summary〉 
		public int PresentClientCount 
		{ 
			get{return _sessionTable.Count;} 
		} 

		/// 〈summary〉 
		/// 服务器运行状态 
		/// 〈/summary〉 
		public bool IsRun 
		{ 
			get{return _isRun;} 
		} 
		
		/// <summary>
		/// 获取当前通道的本地主机地址信息
		/// </summary>
		public IPEndPoint LocalEndPoint
		{
			get{return (IPEndPoint)ClientSocket.LocalEndPoint;}
		}

		/// <summary>
		/// 获取当前通道通信的远程主机地址信息
		/// </summary>
		public IPEndPoint RemoteEndPoint
		{
			get{return (IPEndPoint)ClientSocket.RemoteEndPoint;}
		}
					
		/// <summary>
		/// 服务器端开始侦听，如有远程客户连接请求，则激活ConnectionRequest事件,如出错，则产生一个异常
		/// </summary>
		public void Start()
		{
			if (HandlerType!=(int)_HandlerType.LHandler )//如果不是侦听通道时,则出错.
			{
				OnError(this,"Start方法只能用于服务器端侦听通道!");
				return;
			}
			if(_isRun) 
			{ 
				OnError(this,"侦听通道已经在运行侦听.");
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

		/// 〈summary〉 
		/// 客户端连接请求时处理函数 
		/// 〈param name="iar"〉欲建立服务器连接的Socket对象〈/param〉 
		protected virtual void ClientConnRequest(IAsyncResult iar) 
		{ 
			//如果服务器停止了服务,就不能再接收新的客户端 
			if(!_isRun) 
			{ 
				return; 
			}
            try
            {
                //获得远程连接主机的信息，以等待接入。
                Socket oldserver = (Socket)iar.AsyncState;
                ClientHandler client = new ClientHandler(oldserver.EndAccept(iar));
                //在服务器端产生一个客户连接请求事件
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
                //继续接受其它客户端连接请求
                ClientSocket.BeginAccept(new AsyncCallback(ClientConnRequest), ClientSocket);
            }
            catch (Exception e)
            {
                OnError(this, e.Message);
            }
		} 
		
		/// <summary>
		/// 服务器端接收客户的连接,并构建一个与该客户进行通信的通道,将该通道加入到会话客户队列中.
		/// </summary>
		/// <param name="CShandler">服务器端分配的远程主机会话的通道</param>
		/// <returns>返回分配的该通道ID号</returns>
		public int Accept(ClientHandler CShandler)
		{
			if (_isClose)
			{
				OnError(this,"通道已关闭");
				return 0;
			}
			ClientHandler Ch=CShandler; 			
			if(_sessionTable.Count>= _clientCount) 
			{  
                CShandler.Close();
                //服务器已满,发出通知			
                ServerFull?.Invoke(this);
                return 0;
			} 
			else 
			{ 							
				try
				{
                    //将该远程连接加入到列队中
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
				if( recv > 0 )   //接收到数据
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
				//继续接收来自来客户端的数据 
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
		/// 〈summary〉 
		/// 客户通道发送数据
		/// 〈/summary〉 
		/// 〈param name="datagram"〉要发送的数据〈/param〉 
		public void Send(byte[] DataToSend)
		{       
			if (_isClose)
			{
				OnError(this,"通道已关闭");
				return;
			}
			if (HandlerType==(int)_HandlerType.LHandler )//如果不是侦听通道时,则出错.
			{
				OnError(this,"服务器端侦听通道不能使用Send方法进行通信!");
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
					OnError(this,"远程主机未连接,发送数据失败!");
				}
			}
			catch (Exception e)
			{
				OnError(this,e.Message);
			}		
		}

		/// 〈summary〉 
		/// 发送数据完成处理函数 
		/// 〈/summary〉 
		/// 〈param name="iar"〉目标客户端Socket〈/param〉 
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
				//激活发送数据完成事件
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
		/// 服务器端停止服务
		/// </summary>
		public virtual void Stop() 
		{ 
			if (HandlerType!=(int)_HandlerType.LHandler )//如果不是侦听通道时,则出错.
			{
				OnError(this,"Stop方法只能用于服务器端侦听通道!");
				return;
			}
			if( !_isRun ) 
			{ 
				OnError(this,"侦听通道停止运行侦听."); 
				return;
			} 

			/*这个条件语句，一定要在关闭所有客户端以前调用 
			否则在EndConn会出现错误 */
			_isRun = false; 

			//关闭数据连接,负责客户端会认为是强制关闭连接 
			if( ClientSocket.Connected ) 
			{ 
				ClientSocket.Shutdown( SocketShutdown.Both ); 				
			} 

			CloseAllClient(); 
			//清理资源 
			ClientSocket.Close(); 
			_sessionTable = null;
		} 

		/// <summary>
		/// 客户端请求连接远程服务器主机
		/// </summary>
		public void SocketConnect()
		{
			if (HandlerType!=(int)_HandlerType.CHandler )//如果不是侦听通道时,则出错.
			{
				OnError(this,"connect方法只能用于客户端通信通道!");
				return;
			}
			try
			{
				if (ClientSocket.Connected)
				{
					OnError(this,"已连接到远程序服务器主机！");
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
		/// 当连接成功或出现一个导常时，激活上层一个连接成功事件或错误事件。
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
		/// 关闭服务器端或客户端连接并释放所有关联的资源。
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
		/// 关闭某个客户的通信
		/// </summary>
		/// <param name="ClientHandlerID">通信通道的ID号</param>
		public void CloseClient(int ClientHandlerID)
		{
			if (HandlerType!=((int)_HandlerType.LHandler) )//如果不是侦听通道时,则出错.
			{
				OnError(this,"CloseClient方法只能用于服务器端侦听通道!");
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
		/// 查找指定ID号的通信通道
		/// </summary>
		/// <param name="handlerid">ID号</param>
		/// <returns>如找到,则返回该通道,否则返回空</returns>
		public ClientHandler FindHandler(int handlerid)
		{
            if (ConnectionPool.ContainsKey(handlerid))
                return ConnectionPool[handlerid];
            else
                return null;
		}	
		
		/// <summary>
		/// 清除服务器端所有客户列队
		/// </summary>
		public virtual void CloseAllClient() 
		{ 		
			foreach(ClientHandler client in _sessionTable.Values) 
			{ 
				client.Close();
			} 
			_sessionTable.Clear();
		}

		//产生的事件
		/// <summary>
		/// 连接超时事件
		/// </summary>
		public delegate void GetDataOutTimeEvent(object sender,System.EventArgs e);
		public event GetDataOutTimeEvent GetDataOutTime;
		/// <summary>
		/// 客户端连接成功事件
		/// </summary>
		public delegate void ConnectEvent(object sender);
		public event ConnectEvent Connect;
		
		/// <summary>
		/// 接收到数据事件
		/// </summary>
		public delegate void DataArrivalEvent(object sender, ReceiveEventArgs e);   
		public event DataArrivalEvent DataArrival;
		/// <summary>
		/// 无论何时，只要后台处理中出现错误（例如，连接失败，或者在后台收发数据失败）事件就会出现。
		/// </summary>
		public delegate void ErrorEvent(object sender,string description);     //出错
		public event ErrorEvent Error;
		protected virtual void OnError(object sender,string ErrorString)
		{
            Error?.Invoke(sender, ErrorString);
        }

		internal delegate void ReceiveOriginalDataEvent(byte[] _recvDataBuffer,int recv);   //发送数据完毕
		internal event ReceiveOriginalDataEvent ReceiveOriginalData;		
		
		/// <summary>
		/// 发送完数据产生的事件
		/// </summary>
		public delegate void SendCompleteEvent(object sender,int DataNumber);   //发送数据完毕
		public event SendCompleteEvent SendComplete;
		/// <summary>
		/// 有远程客户请求连接时，产生该事件
		/// </summary>
		public delegate void ConnectionRequestHandler(object sender,ClientHandler Client);
		public event ConnectionRequestHandler ConnectionRequest;
		/// 〈summary〉 
		/// 服务器中客户连接池已经满事件 
		/// 〈/summary〉 
		public delegate void ServerFullEvent(object sender);
		public event ServerFullEvent ServerFull; 
		/// <summary>
		/// 通道关闭事件
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
