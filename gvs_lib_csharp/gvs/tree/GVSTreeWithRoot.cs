using System;
using System.Xml;
using System.Text;
using System.Collections;

using GVS_Client_Socket_v1._3.gvs.typ.node;
using GVS_Client_Socket_v1._3.gvs.connection;
using static System.Configuration.ConfigurationSettings;

namespace GVS_Client_Socket_v1._3.gvs.tree
{
	/// <summary>
	/// This class takes up only a rootnode. For transfer, the class build the tree
	/// recursivly and add the nodes to a collection. 
	/// It is to be made certain that the tree does not contain cycles. 
	/// The connectioninformation have to be set in teh App.config file.
	/// The key GVSPortFile or the Keys GVSHost,GVSPort have to be set.
	/// If no configuration is aviable, localhost with port 3000 will be set.
	/// 
	/// Actually only BinaryTrees are supported, because the Layoutalgorithm are missing.
	/// 
	/// </summary>
	public class GVSTreeWithRoot {
		
		private XmlDocument document=null;
		private XMLConnection xmlConnection=null;
	
		private String host=null;
		private int port=0;
	
		private long gvsTreeId=0;
		private String gvsTreeName="";
		private GVSTreeNode gvsTreeRoot=null;
		private int maxLabelLength=0;
	
		//	Config
		private const String GVSPORTFILE="GVSPortFile";
		private const String GVSHOST="GVSHost";
		private const String GVSPORT="GVSPort";
	
		//General
		private const String ROOT="GVS";
		private const String ATTRIBUTEID="Id";
		private const String LABEL="Label";
		private const String FILLCOLOR="Fillcolor";
		private const String LINECOLOR="Linecolor";
		private const String LINESTYLE="Linestyle";
		private const String LINETHICKNESS="Linethickness";
		private const String STANDARD="standard";
	
		//	Tree
		private const String TREE="Tree";
		private const String NODES="Nodes";
		private const String DEFAULTNODE="DefaultNode";
		private const String BINARYNODE="BinaryNode";
		private const String TREEROOTID = "TreeRootId";
		private const String CHILDID="Childid";
		private const String RIGTHCHILD="Rigthchild";
		private const String LEFTCHILD="Leftchild";

		//Datas
		private ArrayList gvsTreeNodes;

		/// <summary>
		///  Init the tree and the connection
		/// </summary>
		/// <param name="pGVSTreeName"></param>
		public GVSTreeWithRoot(String pGVSTreeName){
			TimeSpan t = DateTime.Now.Subtract(new DateTime(1970,01,01,01,0,0,0));
			long time = (long)(t.TotalMilliseconds);
			this.gvsTreeId=time;
			this.gvsTreeNodes= new ArrayList();
			this.gvsTreeName=pGVSTreeName;

			if(this.gvsTreeName==null) {
				this.gvsTreeName="";
			}

			string gvsPortFile = AppSettings[GVSPORTFILE];
			string gvsHost =    AppSettings[GVSHOST];
			string gvsPort =    AppSettings[GVSPORT];
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
		///  Set the rootnode for the tree
		/// </summary>
		/// <param name="pGVSRootTreeNode"></param>
		public void setRoot(GVSTreeNode pGVSRootTreeNode) {
			this.gvsTreeRoot=pGVSRootTreeNode;
		}

		/// <summary>
		///  Set the maxLabelLength
		/// </summary>
		/// <param name="pMaxLabelLength"></param>
		public void setMaxLabelLength(int pMaxLabelLength){
			this.maxLabelLength=pMaxLabelLength;
		}


		/// <summary>
		/// Build the tree and check for cycles. 
		/// If the tree is ok, it will be send to the server
		/// </summary>
		public void display(){
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

			if(this.gvsTreeRoot!=null){
				XmlElement treeRoot= document.CreateElement(TREEROOTID); 
				tree.AppendChild(treeRoot);
				treeRoot.AppendChild(document.CreateTextNode(this.gvsTreeRoot.GetHashCode().ToString()));
			 
				XmlElement nodes = document.CreateElement(NODES);
				root.AppendChild(nodes);
				buildNode(nodes,this.gvsTreeRoot);
 
			}
			else{
				Console.WriteLine("Kein Root deklariert");
			}

			if(!checkForCycles()){

				document.Save(Console.Out);
				xmlConnection.sendFile(document);
			}
			else{
				Console.WriteLine("Baum enthält Zyklen. Kein senden erlaubt");
			}
		}

		/// <summary>
		///  Disconnect from server
		/// </summary>
		public void disconnect(){
			xmlConnection.disconnectFromServer();
		}

		/// <summary>
		/// Call disconnect()
		/// </summary>
		~GVSTreeWithRoot() {
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
					Console.WriteLine("CICLE in the tree!!!!");
					break;
				}
			}
			return hasCycle;
		
		}

		//***************************************XML-BUILDERS***********************************************
		private void buildNode(XmlElement pParent, GVSTreeNode pNode){
			gvsTreeNodes.Add(pNode);
			Type[] interfaces=pNode.GetType().GetInterfaces();
			foreach(Type theInterface in interfaces){
				if(theInterface.FullName==typeof(GVSBinaryTreeNode).FullName){
					buildBinaryNode(pParent,(GVSBinaryTreeNode)pNode);
					GVSBinaryTreeNode tmpNode=((GVSBinaryTreeNode)pNode).getGVSLeftChild();
					if(tmpNode!=null){
						buildNode(pParent,tmpNode);	
					}
					tmpNode=((GVSBinaryTreeNode)pNode).getGVSRigthChild();
					if(tmpNode!=null){
						buildNode(pParent,tmpNode);	
					}
					break;
				}
				else if(theInterface.FullName==typeof(GVSDefaultTreeNode).FullName){
					//buildDefaultNode(pParent,(GVSDefaultTreeNode)pNode);
					//GVSDefaultTreeNode[] childs=((GVSDefaultTreeNode)(pNode)).getChildNodes();
					//if(childs!=null){
					//for(int size=0;size<childs.Length;size++){
					//buildNode(pParent,childs[size]);
					//}
					//}
				}
			}	
		}
	
		/*	private void buildDefaultNode(XmlElement pParent, GVSDefaultTreeNode pNode){
				XmlElement defaultNode = document.CreateElement(DEFAULTNODE); 
				pParent.AppendChild(defaultNode);
				defaultNode.SetAttribute(ATTRIBUTEID,pNode.GetHashCode().ToString());
				GVSNodeTyp nodeTyp =pNode.getNodeTyp();
			
				XmlElement label = document.CreateElement(LABEL);
				defaultNode.AppendChild(label);
				String theLabel=pNode.getNodeLabel();
			
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
							XmlElement child = document.CreateElement(CHILDID);
							defaultNode.AppendChild(child);
							child.AppendChild(document.CreateTextNode(childs[size].GetHashCode().ToString()));
					}
				}
			}			*/
	
		private void buildBinaryNode(XmlElement pParent, GVSBinaryTreeNode pNode){
			XmlElement binaryNode = document.CreateElement(DEFAULTNODE); 
			pParent.AppendChild(binaryNode);
			binaryNode.SetAttribute(ATTRIBUTEID,pNode.GetHashCode().ToString());
			GVSNodeTyp nodeTyp =pNode.getGVSNodeTyp();
			
			XmlElement label = document.CreateElement(LABEL);
			binaryNode.AppendChild(label);
			String theLabel=pNode.getGVSNodeLabel();
			
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
				XmlElement leftChild = document.CreateElement(LEFTCHILD);
				binaryNode.AppendChild(leftChild);
				leftChild.AppendChild(document.CreateTextNode(leftNode.GetHashCode().ToString()));
			}
			if(rigthNode!=null){
				XmlElement rigthChild = document.CreateElement(RIGTHCHILD);
				binaryNode.AppendChild(rigthChild);
				rigthChild.AppendChild(document.CreateTextNode(rigthNode.GetHashCode().ToString()));
			}
		}
	}
}
