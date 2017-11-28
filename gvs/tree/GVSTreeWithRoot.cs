using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using gvs_lib_csharp.gvs.connection;
using gvs_lib_csharp.gvs.styles;
using static System.Configuration.ConfigurationSettings;

namespace gvs_lib_csharp.gvs.tree
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
	
		private string host=null;
		private int port=0;
	
		private long gvsTreeId=0;
		private string gvsTreeName="";
		private GVSTreeNode gvsTreeRoot=null;
	
		//	Config
		private const string GVSPORTFILE="GVSPortFile";
		private const string GVSHOST="GVSHost";
		private const string GVSPORT="GVSPort";
	
		//General
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

		//Datas
		private HashSet<GVSTreeNode> gvsTreeNodes;

		/// <summary>
		///  Init the tree and the connection
		/// </summary>
		/// <param name="pGVSTreeName"></param>
		public GVSTreeWithRoot(string pGVSTreeName){
			var t = DateTime.Now.Subtract(new DateTime(1970,01,01,01,0,0,0));
			var time = (long)(t.TotalMilliseconds);
			this.gvsTreeId=time;
			this.gvsTreeNodes= new HashSet<GVSTreeNode>();
			this.gvsTreeName=pGVSTreeName ?? "";

			var gvsPortFile = AppSettings[GVSPORTFILE];
			var gvsHost =    AppSettings[GVSHOST];
			var gvsPort =    AppSettings[GVSPORT];
			if(gvsPortFile!=null){
				try{
					Console.WriteLine("Load socketinformation from " + gvsPortFile);
					var reader = new XmlTextReader(gvsPortFile);
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
		public void SetRoot(GVSTreeNode pGVSRootTreeNode) {
			this.gvsTreeRoot=pGVSRootTreeNode;
		}

		/// <summary>
		/// Build the tree and check for cycles. 
		/// If the tree is ok, it will be send to the server
		/// </summary>
		public void Display(){
			document = new XmlDocument();
			var dec = document.CreateXmlDeclaration("1.0",null,null);
			dec.Encoding=Encoding.UTF8.ToString();
			document.AppendChild(dec);
			var root = document.CreateElement(ROOT);
			document.AppendChild(root);

			 
			var tree = document.CreateElement(TREE);
			root.AppendChild(tree);
			tree.SetAttribute(ATTRIBUTEID,this.gvsTreeId.ToString());

			var treeLabel = document.CreateElement(LABEL);
			tree.AppendChild(treeLabel);
			treeLabel.AppendChild(document.CreateTextNode(this.gvsTreeName));

			if(this.gvsTreeRoot!=null){
				var treeRoot= document.CreateElement(TREEROOTID); 
				tree.AppendChild(treeRoot);
				treeRoot.AppendChild(document.CreateTextNode(this.gvsTreeRoot.GetHashCode().ToString()));
			 
				var nodes = document.CreateElement(NODES);
				root.AppendChild(nodes);
				BuildNode(nodes,this.gvsTreeRoot);
 
			}
			else{
				Console.WriteLine("Kein Root deklariert");
			}

			if(!CheckForCycles()){

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
		public void Disconnect(){
			xmlConnection.disconnectFromServer();
		}

		/// <summary>
		/// Call disconnect()
		/// </summary>
		~GVSTreeWithRoot() {
			this.Disconnect();
		}


		private bool CheckForCycles() {
			var hasCycle=false;
			var toCheck=new HashSet<GVSTreeNode>(gvsTreeNodes);
			foreach(var actualNode in toCheck){
				var counter=0;
				foreach(var nodeToCheck in gvsTreeNodes){
					if(((GVSBinaryTreeNode)nodeToCheck).GetGvsLeftChild()==actualNode||
						((GVSBinaryTreeNode)nodeToCheck).GetGvsRigthChild()==actualNode){
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
		private void BuildNode(XmlElement pParent, GVSTreeNode pNode){
			gvsTreeNodes.Add(pNode);
			var interfaces=pNode.GetType().GetInterfaces();
			foreach(var theInterface in interfaces){
				if(theInterface.FullName==typeof(GVSBinaryTreeNode).FullName){
					BuildBinaryNode(pParent,(GVSBinaryTreeNode)pNode);
					var tmpNode=((GVSBinaryTreeNode)pNode).GetGvsLeftChild();
					if(tmpNode!=null){
						BuildNode(pParent,tmpNode);	
					}
					tmpNode=((GVSBinaryTreeNode)pNode).GetGvsRigthChild();
					if(tmpNode!=null){
						BuildNode(pParent,tmpNode);	
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
							XmlElement child = document.CreateElement(CHILDID);
							defaultNode.AppendChild(child);
							child.AppendChild(document.CreateTextNode(childs[size].GetHashCode().ToString()));
					}
				}
			}			*/
	
		private void BuildBinaryNode(XmlElement pParent, GVSBinaryTreeNode pNode){
			var binaryNode = document.CreateElement(DEFAULTNODE); 
			pParent.AppendChild(binaryNode);
			binaryNode.SetAttribute(ATTRIBUTEID,pNode.GetHashCode().ToString());
			var nodeStyle =pNode.GetStyle() ?? new GVSStyle();

			var label = document.CreateElement(LABEL);
			binaryNode.AppendChild(label);
			var theLabel=pNode.GetGvsNodeLabel() ?? "";
            label.AppendChild(document.CreateTextNode(theLabel));

            var lineColor = document.CreateElement(LINECOLOR); 
			binaryNode.AppendChild(lineColor);

			var lineStyle = document.CreateElement(LINESTYLE);
			binaryNode.AppendChild(lineStyle);
			
			var lineThick = document.CreateElement(LINETHICKNESS);
			binaryNode.AppendChild(lineThick);
			
			var fillColor = document.CreateElement(FILLCOLOR);
			binaryNode.AppendChild(fillColor);
			
				
		    lineColor.AppendChild(document.CreateTextNode(nodeStyle.GetLineColor().ToString()));
		    lineStyle.AppendChild(document.CreateTextNode(nodeStyle.GetLineStyle().ToString()));	
		    lineThick.AppendChild(document.CreateTextNode(nodeStyle.GetLineThickness().ToString()));	
		    fillColor.AppendChild(document.CreateTextNode(nodeStyle.GetFillColor().ToString()));
			
			
			var leftNode=pNode.GetGvsLeftChild();
			var rigthNode=pNode.GetGvsRigthChild();
			if(leftNode!=null){
				var leftChild = document.CreateElement(LEFTCHILD);
				binaryNode.AppendChild(leftChild);
				leftChild.AppendChild(document.CreateTextNode(leftNode.GetHashCode().ToString()));
			}
			if(rigthNode!=null){
				var rigthChild = document.CreateElement(RIGTHCHILD);
				binaryNode.AppendChild(rigthChild);
				rigthChild.AppendChild(document.CreateTextNode(rigthNode.GetHashCode().ToString()));
			}
		}
	}
}
