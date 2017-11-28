using System;
using System.Text;
using System.Collections;
using System.Xml;
using System.Configuration;
using System.IO;

using System.Collections.Generic;
using gvs_lib_csharp.gvs.connection;
using gvs_lib_csharp.gvs.typ.edge;
using gvs_lib_csharp.gvs.typ.graph;
using gvs_lib_csharp.gvs.typ.vertex;
using gvs_lib_csharp._3.gvs.graph;

namespace gvs_lib_csharp.gvs.graph
{
	/// <summary>
	/// This class represents the graph. Null values are translated on standard or 
	/// empty strings. The class works over references. It does not play a role, 
	/// if values are doubly added or removed. The connectioninformation have to be 
	/// set in teh App.config file.
	/// The key GVSPortFile or the Keys GVSHost,GVSPort have to be set.
	/// If no configuration is aviable, localhost with port 3000 will be set
	/// 																																					 *  set over Properties. 
	/// </summary>
	public class GVSGraph{
		private String host=null;
		private int port=0;
		
		private XmlDocument document=null;
		private XMLConnection xmlConnection=null;
		private const String GVSPORTFILE="GVSPortFile";
		private const String GVSHOST="GVSHost";
		private const String GVSPORT="GVSPort";

		private long gvsGraphId=0;
		private String gvsGraphName="";
		private GVSGraphTyp gvsGraphTyp=null;
		private int maxLabelLength=0;
	
		//Allgemein
		private const String ROOT="GVS";
		private const String ATTRIBUTEID="Id";
		private const String LABEL="Label";
		private const String FILLCOLOR="Fillcolor";
		private const String ICON="Icon";
		private const String LINECOLOR="Linecolor";
		private const String LINESTYLE="Linestyle";
		private const String LINETHICKNESS="Linethickness";
		private const String STANDARD="standard";

		//Graph
		private const String GRAPH="Graph";
		private const String BACKGROUND="Background";
		private const String MAXLABELLENGTH="MaxLabelLength"; 
		private const String VERTIZES="Vertizes";
		private const String RELATIVVERTEX="RelativVertex";
		private const String DEFAULTVERTEX="DefaultVertex";
		private const String XPOS="XPos";
		private const String YPOS="YPos";
		private const String EDGES="Edges";
		private const String EDGE="Edge";
		private const String ISDIRECTED="IsDirected";
		private const String FROMVERTEX="FromVertex";
		private const String TOVERTEX="ToVertex";
		private const String ARROWPOS="DrawArrowOnPosition";

		//Datas
		private static GVSGraphTyp defaultGraphTyp=new GVSGraphTyp(GVSGraphTyp.Background.standard);
		private ArrayList gvsGraphVertizes;
		private ArrayList gvsGraphEdges;
		
		/// <summary>
		///	 Creates a Graph with default background
		/// </summary>
		/// <param name="pGVSGraphName"></param>
		public GVSGraph(String pGVSGraphName):this(pGVSGraphName,null){
		}

		/// <summary>
		///	 Creates the Graph-Object. Id will be set to System.currentTimeMillis()
		///  If no properties are set, the default port 3000 and localhost will be applied. 
		/// </summary>
		/// <param name="pGVSGraphName"></param>
		/// <param name="pGVSGraphTyp"></param>
		public GVSGraph(String pGVSGraphName, GVSGraphTyp pGVSGraphTyp){
			
			//Create the System.currentTimeMillis()(JAVA)
			TimeSpan t = DateTime.Now.Subtract(new DateTime(1970,01,01,01,0,0,0));
			long time = (long)(t.TotalMilliseconds);
			this.gvsGraphId=time;

			this.gvsGraphName=pGVSGraphName;
			if(this.gvsGraphName==null) {
				this.gvsGraphName="";
			}
			this.gvsGraphTyp=pGVSGraphTyp;
			if(this.gvsGraphTyp==null) {
				this.gvsGraphTyp=defaultGraphTyp;
			}
			gvsGraphVertizes = new ArrayList();
			gvsGraphEdges= new ArrayList();

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
					Console.WriteLine("Error Portfile");
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
				this.host="127.0.0.1";
				this.port=3000;
			}
			xmlConnection= new XMLConnection(host,port);
			xmlConnection.connectToServer();

		}

		/// <summary>
		///	 Add a DefaultVertex
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void add(GVSDefaultVertex pGVSVertex){
			if(gvsGraphVertizes.Contains(pGVSVertex)){
			}
			else{
				this.gvsGraphVertizes.Add(pGVSVertex);
			}
		}
	
		/// <summary>
		///	  Add a RealtivVertex
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void add(GVSRelativeVertex pGVSVertex){
			if(this.gvsGraphVertizes.Contains(pGVSVertex)){
			}
			else{
				this.gvsGraphVertizes.Add(pGVSVertex);
			}
		}
	
		/// <summary>
		///	 Add a UndirectedEdge
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void add(GVSUndirectedEdge pGVSEdge){
			if(this.gvsGraphEdges.Contains(pGVSEdge)){
			}
			else{
				this.gvsGraphEdges.Add(pGVSEdge);
			}
		}
	
		/// <summary>
		///	 Add a DirectedEdge
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void add(GVSDirectedEdge pGVSEdge){
			if(this.gvsGraphEdges.Contains(pGVSEdge)){
			}
			else{
				this.gvsGraphEdges.Add(pGVSEdge);
			}
		}

		/// <summary>
		///	 Add a Collection of GVSComponents
		/// </summary>
		/// <param name="pGVSComponent"></param>
		public void add(ICollection<Object> pGVSComponent){
			
			foreach(Object tmp in pGVSComponent){
				Type[] interfaces=tmp.GetType().GetInterfaces();
				foreach(Type theInterface in interfaces){
					if(theInterface==typeof(GVSDirectedEdge)){
						this.add((GVSDirectedEdge)tmp);							 
					}
					else if(theInterface==typeof(GVSUndirectedEdge)){
						this.add((GVSUndirectedEdge)tmp);		
					}
					else if(theInterface==typeof(GVSRelativeVertex)){
						this.add((GVSRelativeVertex)tmp);	
					}
					else if(theInterface==typeof(GVSDefaultVertex)){
						this.add((GVSDefaultVertex)tmp);
					}
				}
			}
		}

		/// <summary>
		///	 Add a Array of DirectedEdges
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void add(GVSDirectedEdge[] pGVSEdge){
			for(int count=0;count<pGVSEdge.Length;count++){
				if(this.gvsGraphEdges.Contains(pGVSEdge[count])){
				}
				else{
					this.gvsGraphEdges.Add(pGVSEdge[count]);
				}
			}
		}
	
		/// <summary>
		///	 Add a Array of UndirectedEdges
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void add(GVSUndirectedEdge[] pGVSEdge){
			for(int count=0;count<pGVSEdge.Length;count++){
				if(this.gvsGraphEdges.Contains(pGVSEdge[count])){
				}
				else{
					this.gvsGraphEdges.Add(pGVSEdge[count]);
				}
			}
		}
	
		/// <summary>
		///	 Add a Array of DefaultVertizes
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void add(GVSDefaultVertex[] pGVSVertex) {
			for(int count=0;count<pGVSVertex.Length;count++){
				if(gvsGraphVertizes.Contains(pGVSVertex[count])){
				}
				else{
					this.gvsGraphVertizes.Add(pGVSVertex[count]);
				}
			}
		}
	
		/// <summary>
		///	  Add a Array of RelativVertizes
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void add(GVSRelativeVertex[] pGVSVertex) {
			for(int count=0;count<pGVSVertex.Length;count++){
				if(gvsGraphVertizes.Contains(pGVSVertex[count])){
				}
				else{
					this.gvsGraphVertizes.Add(pGVSVertex[count]);
				}
			}
		}
	
		/// <summary>
		///	 Remove a DefaultVertex. Connected edges will be removed to 
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void remove(GVSDefaultVertex pGVSVertex){
			ArrayList toRemove = new ArrayList();
			foreach(Object edge in gvsGraphEdges){
				Type[] interfaces=edge.GetType().GetInterfaces();
				foreach(Type theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSDirectedEdge).FullName){
						GVSDirectedEdge de = (GVSDirectedEdge)edge;
						if(pGVSVertex==de.getGVSEndVertex()){
							toRemove.Add(de);
						}
						if(pGVSVertex==de.getGVSStartVertex()){
							toRemove.Add(de);
						}
					}
					else if(theInterface.FullName==typeof(GVSUndirectedEdge).FullName){
						GVSUndirectedEdge ue = (GVSUndirectedEdge)edge;
						var vertizes=ue.getGVSVertizes();
						for(int counter=0;counter<vertizes.Length;counter++){
							if(vertizes[counter]==pGVSVertex){
								toRemove.Add(ue);
							}
						}
					}
				}
			}
			foreach(GVSGraphEdge edgeToRemove in toRemove){
				this.gvsGraphEdges.Remove(edgeToRemove);
			}
			this.gvsGraphVertizes.Remove(pGVSVertex);
		}

		/// <summary>
		/// Remove a RealtivVertex. Connected edges will be removed to 
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void remove(GVSRelativeVertex pGVSVertex){
			ArrayList toRemove = new ArrayList();
			foreach(Object edge in gvsGraphEdges){
				Type[] interfaces=edge.GetType().GetInterfaces();
				foreach(Type theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSDirectedEdge).FullName){
						GVSDirectedEdge de = (GVSDirectedEdge)edge;
						if(pGVSVertex==de.getGVSEndVertex()){
							toRemove.Add(de);
						}
						if(pGVSVertex==de.getGVSStartVertex()){
							toRemove.Add(de);
						}
					}
					else if(theInterface.FullName==typeof(GVSUndirectedEdge).FullName){
						GVSUndirectedEdge ue = (GVSUndirectedEdge)edge;
						var vertizes=ue.getGVSVertizes();
						for(int counter=0;counter<vertizes.Length;counter++){
							if(vertizes[counter]==pGVSVertex){
								toRemove.Add(ue);
							}
						}
					}
				}
			}
			foreach(GVSGraphEdge edgeToRemove in toRemove){
				this.gvsGraphEdges.Remove(edgeToRemove);
			}
			this.gvsGraphVertizes.Remove(pGVSVertex);
		}

		/// <summary>
		///	 Remove a DirectedEdge
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void remove(GVSDirectedEdge pGVSEdge){
			this.gvsGraphEdges.Remove(pGVSEdge);
		}
	
		/// <summary>
		///	 Remove a UndirectedEdge
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void remove(GVSUndirectedEdge pGVSEdge){
			this.gvsGraphEdges.Remove(pGVSEdge);
		}

		/// <summary>
		///	  Remove a Collection of GVSComponents. 
		/// </summary>
		/// <param name="pGVSComponent"></param>
		public void remove(ICollection pGVSComponent){
			foreach(Object tmp in pGVSComponent){
				Type[] interfaces=tmp.GetType().GetInterfaces();
				foreach(Type theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSDirectedEdge).FullName){
						this.remove((GVSDirectedEdge)tmp);							 
					}
					else if(theInterface.FullName==typeof(GVSUndirectedEdge).FullName){
						this.remove((GVSUndirectedEdge)tmp);		
					}
					else if(theInterface.FullName==typeof(GVSRelativeVertex).FullName){
						this.remove((GVSRelativeVertex)tmp);	
					}
					else if(theInterface.FullName==typeof(GVSDefaultVertex).FullName){
						this.remove((GVSDefaultVertex)tmp);
					}
				}
			}
		}

		/// <summary>
		///	 Remove a Array of DirectedEdges
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void remove(GVSDirectedEdge[] pGVSEdge){
			for(int count=0;count<pGVSEdge.Length;count++){
				this.gvsGraphEdges.Remove(pGVSEdge[count]);
			}
		}
	
		/// <summary>
		///	 Remove a Array of UndirectedEdges
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void remove(GVSUndirectedEdge[] pGVSEdge){
			for(int count=0;count<pGVSEdge.Length;count++){
				this.gvsGraphEdges.Remove(pGVSEdge[count]);
			}
		}
	
		/// <summary>
		///	 Remove a Array of DefaultVertizes
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void remove(GVSDefaultVertex[] pGVSVertex){
			for(int count=0;count<pGVSVertex.Length;count++) {
				this.remove(pGVSVertex[count]);	
			}
		}

		/// <summary>
		///	 Remove a Array of RealtiveVertizes
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void remove(GVSRelativeVertex[] pGVSVertex){
			for(int count=0;count<pGVSVertex.Length;count++) {
				this.remove(pGVSVertex[count]);	
			}
		}

		/// <summary>
		///	 Set the maxLabelLength.
		/// </summary>
		/// <param name="pMaxLength"></param>
		public void setMaxLabelLength(int pMaxLength){
			this.maxLabelLength=pMaxLength;
		}

		/// <summary>
		///	 Build the Xml and send it to the GVSServer
		/// </summary>
		public void display(){
			document = new XmlDocument();
			XmlDeclaration dec = document.CreateXmlDeclaration("1.0",null,null);
			dec.Encoding=Encoding.UTF8.ToString();
			document.AppendChild(dec);
			XmlElement root = document.CreateElement(ROOT);
			document.AppendChild(root);
			
			XmlElement graph = document.CreateElement(GRAPH);
			root.AppendChild(graph);
			graph.SetAttribute(ATTRIBUTEID,this.gvsGraphId.ToString());
			
			XmlElement graphLabel= document.CreateElement(LABEL);
			graph.AppendChild(graphLabel);
			graphLabel.AppendChild(document.CreateTextNode( this.gvsGraphName));
			
			XmlElement graphBackground= document.CreateElement(BACKGROUND);
			graph.AppendChild(graphBackground);
			graphBackground.AppendChild(document.CreateTextNode(this.gvsGraphTyp.getBackground().ToString()));
			
			XmlElement maxLabelLength = document.CreateElement(MAXLABELLENGTH);
			graph.AppendChild(maxLabelLength);
			maxLabelLength.AppendChild(document.CreateTextNode(this.maxLabelLength.ToString()));

			XmlElement vertizes = document.CreateElement(VERTIZES);
			root.AppendChild(vertizes);

			foreach(Object vertex in gvsGraphVertizes){
				Type[] interfaces=vertex.GetType().GetInterfaces();
				foreach(Type theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSRelativeVertex).FullName){
						buildRelativVertex(vertizes,(GVSRelativeVertex)vertex);
						break;	
					}
					else if(theInterface.FullName==typeof(GVSDefaultVertex).FullName){
						buildDefaultVertex(vertizes,(GVSDefaultVertex)vertex); 
					}
				}
			}

			XmlElement edges = document.CreateElement(EDGES);
			root.AppendChild(edges);

			foreach(Object edge in gvsGraphEdges){
				Type[] interfaces=edge.GetType().GetInterfaces();
				foreach(Type theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSDirectedEdge).FullName){
						buildDirectedEdge(edges,(GVSDirectedEdge)edge);
						break;	
					}
					else if(theInterface.FullName==typeof(GVSUndirectedEdge).FullName){
						buildUndirectedEdge(edges,(GVSUndirectedEdge)edge);
						break;
					}
				}
			}

			xmlConnection.sendFile(document);
		}

		//*************************************************XML_BUILDERS****************************
		private void buildDefaultVertex(XmlElement pParent, GVSDefaultVertex pVertex){
			XmlElement defaultVertex = document.CreateElement(DEFAULTVERTEX);
			pParent.AppendChild(defaultVertex);
			defaultVertex.SetAttribute(ATTRIBUTEID,pVertex.GetHashCode().ToString());
			var vertexTypNull=pVertex.getGVSVertexTyp();
			if(vertexTypNull!=null){
				if(pVertex.getGVSVertexTyp().GetType().FullName==typeof(GVSEllipseVertexTyp).FullName){
					GVSEllipseVertexTyp vertexTyp=
						((GVSEllipseVertexTyp)(pVertex.getGVSVertexTyp()));
					XmlElement label = document.CreateElement(LABEL);
					defaultVertex.AppendChild(label);
					String vertexLabel=pVertex.getGVSVertexLabel();
					if(vertexLabel==null){
						vertexLabel="";
					}
					label.AppendChild(document.CreateTextNode(vertexLabel));

					XmlElement lineColor = document.CreateElement(LINECOLOR);
					defaultVertex.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(vertexTyp.getLineColor().ToString()));

					XmlElement lineStyle = document.CreateElement(LINESTYLE);
					defaultVertex.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(vertexTyp.getLineStyle().ToString()));


					XmlElement lineThick = document.CreateElement(LINETHICKNESS);
					defaultVertex.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(vertexTyp.getLineThickness().ToString()));


					XmlElement fillColor = document.CreateElement(FILLCOLOR);
					defaultVertex.AppendChild(fillColor);
					fillColor.AppendChild(document.CreateTextNode(vertexTyp.getFillColor().ToString()));
				}
				else if(pVertex.getGVSVertexTyp().GetType().FullName==typeof(GVSIconVertexTyp).FullName){
					GVSIconVertexTyp vertexTyp=
						((GVSIconVertexTyp)(pVertex.getGVSVertexTyp()));
					XmlElement label = document.CreateElement(LABEL);
					defaultVertex.AppendChild(label);
					String vertexLabel=pVertex.getGVSVertexLabel();
					if(vertexLabel==null){
						vertexLabel="";
					}
					label.AppendChild(document.CreateTextNode(vertexLabel));
				
					XmlElement lineColor = document.CreateElement(LINECOLOR);
					defaultVertex.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(vertexTyp.getLineColor().ToString()));

					XmlElement lineStyle = document.CreateElement(LINESTYLE);
					defaultVertex.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(vertexTyp.getLineStyle().ToString()));


					XmlElement lineThick = document.CreateElement(LINETHICKNESS);
					defaultVertex.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(vertexTyp.getLineThickness().ToString()));


					XmlElement icon = document.CreateElement(ICON);
					defaultVertex.AppendChild(icon);
					icon.AppendChild(document.CreateTextNode(vertexTyp.getIcon().ToString()));
				}
				else{
					Console.WriteLine("VertexTyp isn't a ellipse or icon");
				}
			}
			else{
				XmlElement label = document.CreateElement(LABEL);
				defaultVertex.AppendChild(label);
				String vertexLabel=pVertex.getGVSVertexLabel();
				if(vertexLabel==null){
					vertexLabel="";
				}
				label.AppendChild(document.CreateTextNode(vertexLabel));
				
				XmlElement lineColor = document.CreateElement(LINECOLOR);
				defaultVertex.AppendChild(lineColor);
				lineColor.AppendChild(document.CreateTextNode(STANDARD));

				XmlElement lineStyle = document.CreateElement(LINESTYLE);
				defaultVertex.AppendChild(lineStyle);
				lineStyle.AppendChild(document.CreateTextNode(STANDARD));

				XmlElement lineThick = document.CreateElement(LINETHICKNESS);
				defaultVertex.AppendChild(lineThick);
				lineThick.AppendChild(document.CreateTextNode(STANDARD));

				XmlElement fillColor = document.CreateElement(FILLCOLOR);
				defaultVertex.AppendChild(fillColor);
				fillColor.AppendChild(document.CreateTextNode(STANDARD));
			}
		}
		
		private void buildRelativVertex(XmlElement pParent, GVSRelativeVertex pVertex){
			XmlElement relativeVertex = document.CreateElement(RELATIVVERTEX);
			pParent.AppendChild(relativeVertex);
			relativeVertex.SetAttribute(ATTRIBUTEID,pVertex.GetHashCode().ToString());
			var vertexTypNull=pVertex.getGVSVertexTyp();
			if(vertexTypNull!=null){
				if(pVertex.getGVSVertexTyp().GetType().FullName==typeof(GVSEllipseVertexTyp).FullName){
					GVSEllipseVertexTyp vertexTyp=
						((GVSEllipseVertexTyp)(pVertex.getGVSVertexTyp()));
					XmlElement label = document.CreateElement(LABEL);
					relativeVertex.AppendChild(label);
					String vertexLabel=pVertex.getGVSVertexLabel();
					if(vertexLabel==null){
						vertexLabel="";
					}
					label.AppendChild(document.CreateTextNode(vertexLabel));

					XmlElement lineColor = document.CreateElement(LINECOLOR);
					relativeVertex.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(vertexTyp.getLineColor().ToString()));

					XmlElement lineStyle = document.CreateElement(LINESTYLE);
					relativeVertex.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(vertexTyp.getLineStyle().ToString()));


					XmlElement lineThick = document.CreateElement(LINETHICKNESS);
					relativeVertex.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(vertexTyp.getLineThickness().ToString()));


					XmlElement fillColor = document.CreateElement(FILLCOLOR);
					relativeVertex.AppendChild(fillColor);
					fillColor.AppendChild(document.CreateTextNode(vertexTyp.getFillColor().ToString()));

					XmlElement xPos = document.CreateElement(XPOS);
					relativeVertex.AppendChild(xPos);
					xPos.AppendChild(document.CreateTextNode(pVertex.getX().ToString()));

					XmlElement yPos = document.CreateElement(YPOS);
					relativeVertex.AppendChild(yPos);
					yPos.AppendChild(document.CreateTextNode(pVertex.getY().ToString()));
				}
				else if(pVertex.getGVSVertexTyp().GetType().FullName==typeof(GVSIconVertexTyp).FullName){
					GVSIconVertexTyp vertexTyp=
						((GVSIconVertexTyp)(pVertex.getGVSVertexTyp()));
					XmlElement label = document.CreateElement(LABEL);
					relativeVertex.AppendChild(label);
					String vertexLabel=pVertex.getGVSVertexLabel();
					if(vertexLabel==null){
						vertexLabel="";
					}
					label.AppendChild(document.CreateTextNode(vertexLabel));

					XmlElement lineColor = document.CreateElement(LINECOLOR);
					relativeVertex.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(vertexTyp.getLineColor().ToString()));

					XmlElement lineStyle = document.CreateElement(LINESTYLE);
					relativeVertex.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(vertexTyp.getLineStyle().ToString()));


					XmlElement lineThick = document.CreateElement(LINETHICKNESS);
					relativeVertex.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(vertexTyp.getLineThickness().ToString()));


					XmlElement icon = document.CreateElement(ICON);
					relativeVertex.AppendChild(icon);
					icon.AppendChild(document.CreateTextNode(vertexTyp.getIcon().ToString()));

					XmlElement xPos = document.CreateElement(XPOS);
					relativeVertex.AppendChild(xPos);
					xPos.AppendChild(document.CreateTextNode(pVertex.getX().ToString()));

					XmlElement yPos = document.CreateElement(YPOS);
					relativeVertex.AppendChild(yPos);
					yPos.AppendChild(document.CreateTextNode(pVertex.getY().ToString()));
				}
				else{
					Console.WriteLine("VertexTyp isn't a ellipse or icon");
					
				}
			}
			else{
				XmlElement label = document.CreateElement(LABEL);
				relativeVertex.AppendChild(label);
				String vertexLabel=pVertex.getGVSVertexLabel();
				if(vertexLabel==null){
					vertexLabel="";
				}
				label.AppendChild(document.CreateTextNode(vertexLabel));

				XmlElement lineColor = document.CreateElement(LINECOLOR);
				relativeVertex.AppendChild(lineColor);
				lineColor.AppendChild(document.CreateTextNode(STANDARD));

				XmlElement lineStyle = document.CreateElement(LINESTYLE);
				relativeVertex.AppendChild(lineStyle);
				lineStyle.AppendChild(document.CreateTextNode(STANDARD));


				XmlElement lineThick = document.CreateElement(LINETHICKNESS);
				relativeVertex.AppendChild(lineThick);
				lineThick.AppendChild(document.CreateTextNode(STANDARD));


				XmlElement fillColor = document.CreateElement(FILLCOLOR);
				relativeVertex.AppendChild(fillColor);
				fillColor.AppendChild(document.CreateTextNode(STANDARD));

				XmlElement xPos = document.CreateElement(XPOS);
				relativeVertex.AppendChild(xPos);
				xPos.AppendChild(document.CreateTextNode(pVertex.getX().ToString()));

				XmlElement yPos = document.CreateElement(YPOS);
				relativeVertex.AppendChild(yPos);
				yPos.AppendChild(document.CreateTextNode(pVertex.getY().ToString()));
			}
		}

		private void buildDirectedEdge(XmlElement pParent, GVSDirectedEdge pEdge){
			GVSDefaultVertex vertex1 = pEdge.getGVSStartVertex();
			GVSDefaultVertex vertex2 = pEdge.getGVSEndVertex();
			bool vertex1Exist=false;
			bool vertex2Exist=false;
		
			foreach(GVSDefaultVertex vertex in gvsGraphVertizes){
				if(vertex==vertex1){
					vertex1Exist=true;
				}
				if(vertex==vertex2){
					vertex2Exist=true;
				}
			}
		
			if(vertex1Exist==true && vertex2Exist==true &&vertex1!=null && vertex2!=null){
				var edgeTyp = pEdge.getGVSEdgeTyp();
				XmlElement directedEdge = document.CreateElement(EDGE);
				pParent.AppendChild(directedEdge);
				directedEdge.SetAttribute(ATTRIBUTEID,pEdge.GetHashCode().ToString());
				directedEdge.SetAttribute(ISDIRECTED,"true");
				if(edgeTyp!=null){
					XmlElement label = document.CreateElement(LABEL);
					directedEdge.AppendChild(label);
					String edgeLabel=pEdge.getGVSEdgeLabel();
					if(edgeLabel==null){
						edgeLabel="";
					}
					label.AppendChild(document.CreateTextNode(edgeLabel));

					XmlElement lineColor = document.CreateElement(LINECOLOR);
					directedEdge.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(edgeTyp.getLineColor().ToString()));

					XmlElement lineStyle = document.CreateElement(LINESTYLE);
					directedEdge.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(edgeTyp.getLineStyle().ToString()));


					XmlElement lineThick = document.CreateElement(LINETHICKNESS);
					directedEdge.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(edgeTyp.getLineThickness().ToString()));


					XmlElement fromVertex = document.CreateElement(FROMVERTEX);
					directedEdge.AppendChild(fromVertex);
					fromVertex.AppendChild(document.CreateTextNode(pEdge.getGVSStartVertex().GetHashCode().ToString()));
					
					XmlElement toVertex = document.CreateElement(TOVERTEX);
					directedEdge.AppendChild(toVertex);
					toVertex.AppendChild(document.CreateTextNode(pEdge.getGVSEndVertex().GetHashCode().ToString()));
				}

				else{
					XmlElement label = document.CreateElement(LABEL);
					directedEdge.AppendChild(label);
					String edgeLabel=pEdge.getGVSEdgeLabel();
					if(edgeLabel==null){
						edgeLabel="";
					}
					label.AppendChild(document.CreateTextNode(edgeLabel));

					XmlElement lineColor = document.CreateElement(LINECOLOR);
					directedEdge.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(STANDARD));

					XmlElement lineStyle = document.CreateElement(LINESTYLE);
					directedEdge.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(STANDARD));


					XmlElement lineThick = document.CreateElement(LINETHICKNESS);
					directedEdge.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(STANDARD));
					
					
					XmlElement fromVertex = document.CreateElement(FROMVERTEX);
					directedEdge.AppendChild(fromVertex);
					fromVertex.AppendChild(document.CreateTextNode(pEdge.getGVSStartVertex().GetHashCode().ToString()));
					
					XmlElement toVertex = document.CreateElement(TOVERTEX);
					directedEdge.AppendChild(toVertex);
					toVertex.AppendChild(document.CreateTextNode(pEdge.getGVSEndVertex().GetHashCode().ToString()));				}
			}
			else{
				Console.WriteLine("Vertex missing or null");
			}
		}

		private void buildUndirectedEdge(XmlElement pParent, GVSUndirectedEdge pEdge){
			if(pEdge.getGVSVertizes()!=null){
				var vertex1 = pEdge.getGVSVertizes()[0];
				var vertex2 = pEdge.getGVSVertizes()[1];
				bool vertex1Exist=false;
				bool vertex2Exist=false;
			
				foreach(GVSDefaultVertex vertex in gvsGraphVertizes){
					if(vertex==vertex1){
						vertex1Exist=true;
					}
					if(vertex==vertex2){
						vertex2Exist=true;
					}
				}

				if(vertex1Exist==true && vertex2Exist==true &&vertex1!=null &&vertex2!=null){
					GVSEdgeTyp edgeTyp = pEdge.getGVSEdgeTyp();
					XmlElement undirectedEdge = document.CreateElement(EDGE);
					pParent.AppendChild(undirectedEdge);
					undirectedEdge.SetAttribute(ATTRIBUTEID,pEdge.GetHashCode().ToString());
					int arrowPos=pEdge.hasArrow();
					undirectedEdge.SetAttribute(ISDIRECTED,"false");
					undirectedEdge.SetAttribute(ARROWPOS,arrowPos.ToString());
					if(edgeTyp!=null){
						XmlElement label = document.CreateElement(LABEL);
						undirectedEdge.AppendChild(label);
						String edgeLabel=pEdge.getGVSEdgeLabel();
						if(edgeLabel==null){
							edgeLabel="";
						}
						label.AppendChild(document.CreateTextNode(edgeLabel));

						XmlElement lineColor = document.CreateElement(LINECOLOR);
						undirectedEdge.AppendChild(lineColor);
						lineColor.AppendChild(document.CreateTextNode(STANDARD));

						XmlElement lineStyle = document.CreateElement(LINESTYLE);
						undirectedEdge.AppendChild(lineStyle);
						lineStyle.AppendChild(document.CreateTextNode(STANDARD));


						XmlElement lineThick = document.CreateElement(LINETHICKNESS);
						undirectedEdge.AppendChild(lineThick);
						lineThick.AppendChild(document.CreateTextNode(STANDARD));
						
						
						XmlElement fromVertex = document.CreateElement(FROMVERTEX);
						undirectedEdge.AppendChild(fromVertex);
						fromVertex.AppendChild(document.CreateTextNode(pEdge.getGVSVertizes()[0].GetHashCode().ToString()));
						
						XmlElement toVertex = document.CreateElement(TOVERTEX);
						undirectedEdge.AppendChild(toVertex);
						toVertex.AppendChild(document.CreateTextNode(pEdge.getGVSVertizes()[1].GetHashCode().ToString()));
					}
					else{
						XmlElement label = document.CreateElement(LABEL);
						undirectedEdge.AppendChild(label);
						String edgeLabel=pEdge.getGVSEdgeLabel();
						if(edgeLabel==null){
							edgeLabel="";
						}
						label.AppendChild(document.CreateTextNode(edgeLabel));

						XmlElement lineColor = document.CreateElement(LINECOLOR);
						undirectedEdge.AppendChild(lineColor);
						lineColor.AppendChild(document.CreateTextNode(STANDARD));

						XmlElement lineStyle = document.CreateElement(LINESTYLE);
						undirectedEdge.AppendChild(lineStyle);
						lineStyle.AppendChild(document.CreateTextNode(STANDARD));

						XmlElement lineThick = document.CreateElement(LINETHICKNESS);
						undirectedEdge.AppendChild(lineThick);
						lineThick.AppendChild(document.CreateTextNode(STANDARD));
						
						XmlElement fromVertex = document.CreateElement(FROMVERTEX);
						undirectedEdge.AppendChild(fromVertex);
						fromVertex.AppendChild(document.CreateTextNode(pEdge.getGVSVertizes()[0].GetHashCode().ToString()));
						
						XmlElement toVertex = document.CreateElement(TOVERTEX);
						undirectedEdge.AppendChild(toVertex);
						toVertex.AppendChild(document.CreateTextNode(pEdge.getGVSVertizes()[1].GetHashCode().ToString()));
					}
				}
				else{
					Console.WriteLine("Vertex missing or null");
				}
			}
			else{
				Console.WriteLine("No Array or null");
			}
		}

		public void disconnect(){
			xmlConnection.disconnectFromServer();
		}

		~GVSGraph() {
			this.disconnect();
		}

	}
}
