using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;

namespace gvs_lib_csharp.gvs.connection
{
	/// <summary>
	/// Connection-Class for the GVS. If the Server is busy,
	/// the client will exit.
	/// </summary>
	public class XMLConnection{
		private int serverPort=0;
		private String serverAdress="";
		private TcpClient socket;
		StreamWriter outWriter;
		StreamReader inReader;

		public XMLConnection(String pServerAdress, int pServerPort) {
			lock(this){
				this.serverAdress=pServerAdress;
				this.serverPort=pServerPort;
			}
		}
		
		/// <summary>
		/// Connect to the GVS-Server and reserve the Service
		/// If the Server is busy, the client wilb be terminated
		/// </summary>
		/// <returns>answer from the Server</returns>
		public String connectToServer(){
			lock(this){
				String answer="";
				try {
					Console.WriteLine("Connect to server: " +
						serverAdress + "port: "+ serverPort);

					socket=new TcpClient(serverAdress,serverPort);
					Console.WriteLine("Connection ready");
					NetworkStream stream = socket.GetStream();
	            
					outWriter = new StreamWriter(stream);
					inReader = new StreamReader(stream);
					
					outWriter.WriteLine("reserveGVS");
					outWriter.Flush();

					answer=inReader.ReadLine();
					if(answer=="OK") {
						Console.WriteLine(answer);
						Console.WriteLine("Service reserved");
					}
					else {
						Console.WriteLine("Server busy");
						Console.WriteLine("Close Connection");
						inReader.Close();
						outWriter.Flush();
						outWriter.Close();
						socket.Close();
					}
				}
				catch (Exception err){
					Console.WriteLine(err.Message);
				}
				return answer;
			}
		}

		/// <summary>
		/// Sends the Xml-Document to the GVS-Server									* 
		/// </summary>
		/// <param name="doc">document to send</param>
		public void sendFile(XmlDocument doc){
			lock(this){
				try{

					XmlTextWriter writer = new XmlTextWriter(outWriter);
					doc.Save(outWriter);
					outWriter.Flush();
					Console.WriteLine("Send data");
					outWriter.WriteLine(";");
					outWriter.Flush();
				}
				catch(Exception exx){
				}
			}
		}

		/// <summary>
		/// Disconnect from Server. Must be called to transfer the datas properly
		/// </summary>
		public void disconnectFromServer(){
			lock(this){
				
				try {
					outWriter.WriteLine("releaseGVS");
					outWriter.Flush();
					Console.WriteLine("Disconnect from server");
					inReader.Close();
					outWriter.Flush();
					outWriter.Close();
					socket.Close();
				} 
				catch (IOException err){
					Console.WriteLine(err.ToString());
				}
				catch(Exception ex){
					//Console.WriteLine(ex.Message);
				}
			}
		}
	}
}
