using System;
using System.Xml;
using System.Configuration;
using System.Text;
using System.Collections;
using gvs_lib_csharp.gvs.connection;
using gvs_lib_csharp.gvs.typ.node;

namespace gvs_lib_csharp.gvs.tree
{
	/// <summary>
	/// This class takes up those nodes to a Collection and transfers it to 
	/// the server. It is to be made certain that the tree does not contain cycles. 
	/// It does not play a role,if values are doubly added or removed. 
	/// The connectioninformation have to be set in teh App.config file.
	/// The key GVSPortFile or the Keys GVSHost,GVSPort have to be set.
	/// If no configuration is aviable, localhost with port 3000 will be set.
	/// 
	/// Actually only BinaryTrees are supported, because the Layoutalgorithm are missing.
	/// 
	/// </summary>
	public class GVSTreeWithCollection {
		//For Send	
		private XmlDocument document=null;
		private XMLConnection xmlConnection=null;
	
		private string host=null;
		private int port=0;
	
		//Datas
		private long gvsTreeId=0;
		private string gvsTreeName="";
		private ArrayList gvsTreeNodes;
		private int maxLabelLength=0;
	
		//	Config
		private const string GVSPORTFILE="GVSPortFile";
		private const string GVSHOST="GVSHost";
		private const string GVSPORT="GVSPort";
	
		// General
		private const string ROOT="GVS";
		private const string ATTRIBUTEID="Id";
		private const string LABEL="Label";
		private const string FILLCOLOR="Fillcolor";
		private const string LINECOLOR="Linecolor";
		private const string LINESTYLE="Linestyle";
		private const string LINETHICKNESS="Linethickness";
		private const string STANDARD="standard";
	
		//	Tree
		private const string TREE="Tree";
		private const string NODES="Nodes";
		private const string DEFAULTNODE="DefaultNode";
		private const string BINARYNODE="BinaryNode";
		private const string TREEROOTID = "TreeRootId";
		private const string CHILDID="Childid";
		private const string RIGTHCHILD="Rigthchild";
		private const string LEFTCHILD="Leftchild";

		/// <summary>
		///  Init the tree and the connection
		/// </summary>
		/// <param name="pGVSTreeName"></param>
		public GVSTreeWithCollection(string pGVSTreeName){

			TimeSpan t = DateTime.Now.Subtract(new DateTime(1970,01,01,01,0,0,0));
			long time = (long)(t.TotalMilliseconds);
			
			this.gvsTreeId=time;
			this.gvsTreeName=pGVSTreeName;
			this.gvsTreeNodes= new ArrayList();

			if(this.gvsTreeName==null) {
				this.gvsTreeName="";
			}

			string gvsPortFile = ConfigurationSettings.AppSettings[GVSPORTFILE];
			string gvsHost =    ConfigurationSettings.AppSettings[GVSHOST];
			string gvsPort =    ConfigurationSettings.AppSettings[GVSPORT];
			if(gvsPortFile!=null){
				try{
					Console.WriteLine("Load socketinformation from " + gvsPortFile);
					XmlTextReader reader = new XmlTextReader(gvsPortFile);
					while(reader.Read()){
						if(reader.IsStartElement("Port")){
							reader.Read();
							port=int.Parse(reader.Value);
						}
						if(reader.IsStartElement("Host")){
							reader.Read();
							host=reader.Value.ToString();
						}
					}
				}
				catch(Exception ex){
					Console.WriteLine("Fehler Portfile");
					Console.WriteLine(ex.Message);
				}
			}
			else if(gvsHost!=null && gvsPort!=null){
				try{
					Console.WriteLine("Load Socketinformation from AppConfig (Host,Port)");
					this.host=gvsHost;
					this.port=int.Parse(gvsPort);
				}
				catch(Exception ex){
					Console.WriteLine("port or host failed");
					Console.WriteLine(ex.Message);
				}
			}

			else{
				Console.WriteLine("Keine Connection angaben. Lokaler Server. Port auf 3000");
				this.host="127.0.0.1";
				this.port=3000;
			}
			xmlConnection= new XMLConnection(host,port);
			xmlConnection.connectToServer();
		}

		/// <summary>
		///	 Add a Binarynode
		/// </summary>
		/// <param name="pGVSNode"></param>
		public void add(GVSBinaryTreeNode pGVSNode){
			if(gvsTreeNodes.Contains(pGVSNode)){
			}
			else{
				this.gvsTreeNodes.Add(pGVSNode);
			}
		}

		/// <summary>
		///	 Add a Defaultnode
		/// </summary>
		/// <param name="pGVSNode"></param>
		public void add(GVSDefaultTreeNode pGVSNode){
			if(gvsTreeNodes.Contains(pGVSNode)){
			}
			else{
				this.gvsTreeNodes.Add(pGVSNode);
			}
		}
		
		/// <summary>
		///	 Add a BinaryNode-Array
		/// </summary>
		/// <param name="pGVSNode"></param>
		public void add(GVSBinaryTreeNode[] pGVSNode){
			for(int count=0;count<pGVSNode.Length;count++) {
				if(this.gvsTreeNodes.Contains(pGVSNode[count])){
				}
				else{
					this.gvsTreeNodes.Add(pGVSNode[count]);
				}
			}
		}

		/// <summary>
		///	 Add a DefaultNode-Array
		/// </summary>
		/// <param name="pGVSNode"></param>
		public void add(GVSDefaultTreeNode[] pGVSNode){
			for(int count=0;count<pGVSNode.Length;count++) {
				if(this.gvsTreeNodes.Contains(pGVSNode[count])){
				}
				else{
					this.gvsTreeNodes.Add(pGVSNode[count]);
				}
			}	
		}

		/// <summary>
		///	 Add a Collection of TreeNodes
		/// </summary>
		/// <param name="pGVSComponent"></param>
		public void add(ICollection pGVSComponent){	
			foreach(Object tmp in pGVSComponent){
				Type[] interfaces=tmp.GetType().GetInterfaces();
				foreach(Type theInterface in interfaces){
					if(theInterface==typeof(GVSDefaultTreeNode)){
						this.add((GVSDefaultTreeNode)tmp);							 
					}
					else if(theInterface==typeof(GVSBinaryTreeNode)){
						this.add((GVSBinaryTreeNode)tmp);		
					}
				}
			}
		}
		
		/// <summary>
		///	 Remove a DefaultNode
		/// </summary>
		/// <param name="pGVSNode"></param>
		public void remove(GVSDefaultTreeNode pGVSNode){
			this.gvsTreeNodes.Remove(pGVSNode);
		}
	
		/// <summary>
		///	 Remove a BinaryNode
		/// </summary>
		/// <param name="pGVSNode"></param>
		public void remove(GVSBinaryTreeNode pGVSNode){
			this.gvsTreeNodes.Remove(pGVSNode);
		}
		
		/// <summary>
		///	 Remove a DefaultNode-Array
		/// </summary>
		/// <param name="pGVSNode"></param>
		public void remove(GVSDefaultTreeNode[] pGVSNode){
			for(int count=0;count<pGVSNode.Length;count++) {
				this.remove(pGVSNode[count]);	
			}
		}

		/// <summary>
		///	 Remove a BinaryNode-Array
		/// </summary>
		/// <param name="pGVSNode"></param>
		public void remove(GVSBinaryTreeNode[] pGVSNode){
			for(int count=0;count<pGVSNode.Length;count++) {
				this.remove(pGVSNode[count]);
			}
		}

		/// <summary>
		///	 Remove a Collection of TreeNodes
		/// </summary>
		/// <param name="pGVSComponent"></param>
		public void remove(ICollection pGVSComponent){
			foreach(Object tmp in pGVSComponent){
				Type[] interfaces=tmp.GetType().GetInterfaces();
				foreach(Type theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSBinaryTreeNode).FullName){
						this.remove((GVSBinaryTreeNode)tmp);							 
					}
					else if(theInterface.FullName==typeof(GVSDefaultTreeNode).FullName){
						this.remove((GVSDefaultTreeNode)tmp);		
					}
				}
			}
		}

		/// <summary>
		///  Set the maxLabelLength
		/// </summary>
		/// <param name="pMaxLabelLength"></param>
		public void setMaxLabelLength(int pMaxLabelLength){
			this.maxLabelLength=pMaxLabelLength;
		}

		/// <summary>
		///  Build the Xml and send it.
		///  It examined whether the tree cycles contains. 
		///  If the Client terminated, since this is not permitted
		/// </summary>
		public void display(){
			if(checkForCycles()){
				document = new XmlDocument();
				XmlDeclaration dec = document.CreateXmlDeclaration("1.0",null,null);
				dec.Encoding=Encoding.UTF8.ToString();
				document.AppendChild(dec);
				XmlElement root = document.CreateElement(ROOT);
				document.AppendChild(root);

				 
				XmlElement tree = document.CreateElement(TREE);
				root.AppendChild(tree);
				tree.SetAttribute(ATTRIBUTEID,this.gvsTreeId.ToString());

				XmlElement treeLabel = document.CreateElement(LABEL);
				tree.AppendChild(treeLabel);
				treeLabel.AppendChild(document.CreateTextNode(this.gvsTreeName));

				XmlElement nodes = document.CreateElement(NODES);
				root.AppendChild(nodes);

				
				foreach(GVSTreeNode node in gvsTreeNodes){

					Type[] interfaces=node.GetType().GetInterfaces();
					foreach(Type theInterface in interfaces){
						if(theInterface.FullName==typeof(GVSBinaryTreeNode).FullName){
							GVSBinaryTreeNode theNode=(GVSBinaryTreeNode)node;
							if(theNode!=null){
								buildBinaryNode(nodes,theNode);
							}
							else{
								//Trace
							}
							break;
						}
						/*else if(theInterface.FullName==typeof(GVSDefaultTreeNode).FullName){
							GVSDefaultTreeNode theNode =(GVSDefaultTreeNode)node;
							if(theNode!=null){
								buildDefaultNode(nodes,theNode);
							}
							else{
							}
							break;
						}		   */
					}
				}
				document.Save(Console.Out);
				xmlConnection.sendFile(document);
			}
		}

		/// <summary>
		///	 Disconnect from server
		/// </summary>
		public void disconnect(){
			xmlConnection.disconnectFromServer();
		}

		/// <summary>
		/// Call disconnect()
		/// </summary>
		~GVSTreeWithCollection() {
			this.disconnect();
		}

		private bool checkForCycles() {
			bool hasCycle=false;
			ArrayList toCheck=new ArrayList(gvsTreeNodes);
			foreach(GVSBinaryTreeNode actualNode in toCheck){
				int counter=0;
				foreach(GVSBinaryTreeNode nodeToCheck in gvsTreeNodes){
					if(nodeToCheck.getGVSLeftChild()==actualNode||
						nodeToCheck.getGVSRigthChild()==actualNode){
						counter++;
					}
				}
				if(counter>=2){
					hasCycle=true;
					Console.WriteLine("CYCLE in the tree!!!!");
					break;
				}
			}
			return hasCycle;
		
		}

		//*****************************************XML-BUILDERS***********************************************

		/*	private void buildDefaultNode(XmlElement pParent, GVSDefaultTreeNode pNode){
			
				XmlElement defaultNode = document.CreateElement(DEFAULTNODE); 
				pParent.AppendChild(defaultNode);
				defaultNode.SetAttribute(ATTRIBUTEID,pNode.GetHashCode().ToString());
				GVSNodeTyp nodeTyp =pNode.getNodeTyp();
			
				XmlElement label = document.CreateElement(LABEL);
				defaultNode.AppendChild(label);
				string theLabel=pNode.getNodeLabel();
			
				XmlElement lineColor = document.CreateElement(LINECOLOR); 
				defaultNode.AppendChild(lineColor);

				XmlElement lineStyle = document.CreateElement(LINESTYLE);
				defaultNode.AppendChild(lineStyle);
			
				XmlElement lineThick = document.CreateElement(LINETHICKNESS);
				defaultNode.AppendChild(lineThick);
			
				XmlElement fillColor = document.CreateElement(FILLCOLOR);
				defaultNode.AppendChild(fillColor);
			
				if(theLabel==null){
					theLabel="";
				}
				label.AppendChild(document.CreateTextNode(theLabel));
			
				if(nodeTyp!=null){
			
					lineColor.AppendChild(document.CreateTextNode(nodeTyp.getLineColor().ToString()));
					lineStyle.AppendChild(document.CreateTextNode(nodeTyp.getLineStyle().ToString()));	
					lineThick.AppendChild(document.CreateTextNode(nodeTyp.getLineThickness().ToString()));	
					fillColor.AppendChild(document.CreateTextNode(nodeTyp.getFillColor().ToString()));
				}
				else{
					lineColor.AppendChild(document.CreateTextNode(STANDARD));
					lineStyle.AppendChild(document.CreateTextNode(STANDARD));	
					lineThick.AppendChild(document.CreateTextNode(STANDARD));	
					fillColor.AppendChild(document.CreateTextNode(STANDARD));
				}
				GVSDefaultTreeNode[] childs=pNode.getChildNodes();
				if(childs!=null){
					for(int size=0;size<childs.Length;size++){
						if(gvsTreeNodes.Contains(childs[size])){
							XmlElement child = document.CreateElement(CHILDID);
							defaultNode.AppendChild(child);
							child.AppendChild(document.CreateTextNode(childs[size].GetHashCode().ToString()));
						}
						else{
							Console.WriteLine("Child " + childs[size].getNodeLabel() +" existiert nicht in der Collection");
						}		
					}
				}	
			}		*/
	
		private void buildBinaryNode(XmlElement pParent, GVSBinaryTreeNode pNode){
			
			XmlElement binaryNode = document.CreateElement(DEFAULTNODE); 
			pParent.AppendChild(binaryNode);
			binaryNode.SetAttribute(ATTRIBUTEID,pNode.GetHashCode().ToString());
			GVSNodeTyp nodeTyp =pNode.getGVSNodeTyp();
			
			XmlElement label = document.CreateElement(LABEL);
			binaryNode.AppendChild(label);
			string theLabel=pNode.getGVSNodeLabel();
			
			XmlElement lineColor = document.CreateElement(LINECOLOR); 
			binaryNode.AppendChild(lineColor);

			XmlElement lineStyle = document.CreateElement(LINESTYLE);
			binaryNode.AppendChild(lineStyle);
			
			XmlElement lineThick = document.CreateElement(LINETHICKNESS);
			binaryNode.AppendChild(lineThick);
			
			XmlElement fillColor = document.CreateElement(FILLCOLOR);
			binaryNode.AppendChild(fillColor);
			
			if(theLabel==null) {
				theLabel="";
			}
			label.AppendChild(document.CreateTextNode(theLabel));
			
		
			if(nodeTyp!=null){
				
				lineColor.AppendChild(document.CreateTextNode(nodeTyp.getLineColor().ToString()));
				lineStyle.AppendChild(document.CreateTextNode(nodeTyp.getLineStyle().ToString()));	
				lineThick.AppendChild(document.CreateTextNode(nodeTyp.getLineThickness().ToString()));	
				fillColor.AppendChild(document.CreateTextNode(nodeTyp.getFillColor().ToString()));
			}
			else{
				lineColor.AppendChild(document.CreateTextNode(STANDARD));
				lineStyle.AppendChild(document.CreateTextNode(STANDARD));	
				lineThick.AppendChild(document.CreateTextNode(STANDARD));	
				fillColor.AppendChild(document.CreateTextNode(STANDARD));
			}
			
			GVSBinaryTreeNode leftNode=pNode.getGVSLeftChild();
			GVSBinaryTreeNode rigthNode=pNode.getGVSRigthChild();
			if(leftNode!=null){
				if(this.gvsTreeNodes.Contains(leftNode)){
					XmlElement leftChild = document.CreateElement(LEFTCHILD);
					binaryNode.AppendChild(leftChild);
					leftChild.AppendChild(document.CreateTextNode(leftNode.GetHashCode().ToString()));
				}
				else{
					Console.WriteLine("Leftchild " + leftNode.getGVSNodeLabel()+" existiert nicht in der Collection");
				}
			}
			if(rigthNode!=null){
				if(this.gvsTreeNodes.Contains(rigthNode)){
					XmlElement rigthChild = document.CreateElement(RIGTHCHILD);
					binaryNode.AppendChild(rigthChild);
					rigthChild.AppendChild(document.CreateTextNode(rigthNode.GetHashCode().ToString()));
				}
				else{
					Console.WriteLine("Rigthchild " + rigthNode.getGVSNodeLabel()+" existiert nicht in der Collection");
				}
			}
		}
	}
}
