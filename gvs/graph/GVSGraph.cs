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

		private string host=null;
		private int port=0;
		
		private XmlDocument document=null;
		private XMLConnection xmlConnection=null;
		private const string GVSPORTFILE ="GVSPortFile";
		private const string GVSHOST ="GVSHost";
		private const string GVSPORT ="GVSPort";

		private long gvsGraphId=0;
		private string gvsGraphName ="";
		private GVSGraphTyp gvsGraphTyp=null;
		private int maxLabelLength=0;
	
		//Allgemein
		private const string ROOT ="GVS";
		private const string ATTRIBUTEID ="Id";
		private const string LABEL ="Label";
		private const string FILLCOLOR ="Fillcolor";
		private const string ICON ="Icon";
		private const string LINECOLOR ="Linecolor";
		private const string LINESTYLE ="Linestyle";
		private const string LINETHICKNESS ="Linethickness";
		private const string STANDARD ="standard";

		//Graph
		private const string GRAPH ="Graph";
		private const string BACKGROUND ="Background";
		private const string MAXLABELLENGTH="MaxLabelLength"; 
		private const string VERTIZES="Vertizes";
		private const string RELATIVVERTEX="RelativVertex";
		private const string DEFAULTVERTEX="DefaultVertex";
		private const string XPOS="XPos";
		private const string YPOS="YPos";
		private const string EDGES="Edges";
		private const string EDGE="Edge";
		private const string ISDIRECTED="IsDirected";
		private const string FROMVERTEX="FromVertex";
		private const string TOVERTEX="ToVertex";
		private const string ARROWPOS="DrawArrowOnPosition";

		//Datas
		private static GVSGraphTyp defaultGraphTyp=new GVSGraphTyp(GVSGraphTyp.Background.standard);
		private HashSet<GVSDefaultVertex> gvsGraphVertizes;
		private HashSet<GVSGraphEdge> gvsGraphEdges;
		
		/// <summary>
		///	 Creates a Graph with default background
		/// </summary>
		/// <param name="pGVSGraphName"></param>
		public GVSGraph(string pGVSGraphName):this(pGVSGraphName,null){
		}

		/// <summary>
		///	 Creates the Graph-Object. Id will be set to System.currentTimeMillis()
		///  If no properties are set, the default port 3000 and localhost will be applied. 
		/// </summary>
		/// <param name="pGVSGraphName"></param>
		/// <param name="pGVSGraphTyp"></param>
		public GVSGraph(string pGVSGraphName, GVSGraphTyp pGVSGraphTyp){
			
			//Create the System.currentTimeMillis()(JAVA)
			var t = DateTime.Now.Subtract(new DateTime(1970,01,01,01,0,0,0));
			var time = (long)(t.TotalMilliseconds);
			this.gvsGraphId=time;

		    this.gvsGraphName = pGVSGraphName ?? "";
			this.gvsGraphTyp=pGVSGraphTyp ?? defaultGraphTyp;
			
			gvsGraphVertizes = new HashSet<GVSDefaultVertex>();
			gvsGraphEdges= new HashSet<GVSGraphEdge>();

			var gvsPortFile = ConfigurationSettings.AppSettings[GVSPORTFILE];
			var gvsHost =    ConfigurationSettings.AppSettings[GVSHOST];
			var gvsPort =    ConfigurationSettings.AppSettings[GVSPORT];

            if (gvsPortFile!=null){
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
		public void Add(GVSDefaultVertex pGVSVertex){
			this.gvsGraphVertizes.Add(pGVSVertex);
		}
	
		/// <summary>
		///	  Add a RealtivVertex
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void Add(GVSRelativeVertex pGVSVertex){
            this.gvsGraphVertizes.Add(pGVSVertex);
		}
	
		/// <summary>
		///	 Add a UndirectedEdge
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void Add(GVSUndirectedEdge pGVSEdge){
			this.gvsGraphEdges.Add(pGVSEdge);
		}
	
		/// <summary>
		///	 Add a DirectedEdge
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void Add(GVSDirectedEdge pGVSEdge){
			this.gvsGraphEdges.Add(pGVSEdge);
		}

		/// <summary>
		///	 Add a Collection of GVSComponents
		/// </summary>
		/// <param name="pGVSComponent"></param>
		public void Add(ICollection<Object> pGVSComponent){
			
			foreach(var tmp in pGVSComponent){
				var interfaces=tmp.GetType().GetInterfaces();
				foreach(var theInterface in interfaces){
					if(theInterface==typeof(GVSDirectedEdge)){
						this.Add((GVSDirectedEdge)tmp);							 
					}
					else if(theInterface==typeof(GVSUndirectedEdge)){
						this.Add((GVSUndirectedEdge)tmp);		
					}
					else if(theInterface==typeof(GVSRelativeVertex)){
						this.Add((GVSRelativeVertex)tmp);	
					}
					else if(theInterface==typeof(GVSDefaultVertex)){
						this.Add((GVSDefaultVertex)tmp);
					}
				}
			}
		}

		/// <summary>
		///	 Add a Array of DirectedEdges
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void Add(GVSDirectedEdge[] pGVSEdge){
            foreach (var gvsEdge in pGVSEdge)
            {
                this.gvsGraphEdges.Add(gvsEdge);
            }
        }
	
		/// <summary>
		///	 Add a Array of UndirectedEdges
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void Add(GVSUndirectedEdge[] pGVSEdge){
            foreach (var gvsEdge in pGVSEdge)
            {
                this.gvsGraphEdges.Add(gvsEdge);
            }
        }
	
		/// <summary>
		///	 Add a Array of DefaultVertizes
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void Add(GVSDefaultVertex[] pGVSVertex) {
            foreach (var gvsVertex in pGVSVertex)
            {
                this.gvsGraphVertizes.Add(gvsVertex);
            }
        }
	
		/// <summary>
		///	  Add a Array of RelativVertizes
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void Add(GVSRelativeVertex[] pGVSVertex) {
            foreach (var gvsVertex in pGVSVertex)
            {
                this.gvsGraphVertizes.Add(gvsVertex);
            }
		}
	
		/// <summary>
		///	 Remove a DefaultVertex. Connected edges will be removed to 
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void Remove(GVSDefaultVertex pGVSVertex){
			var toRemove = new ArrayList();
			foreach(var edge in gvsGraphEdges){
				var interfaces=edge.GetType().GetInterfaces();
				foreach(var theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSDirectedEdge).FullName){
						var de = (GVSDirectedEdge) edge;
						if(pGVSVertex==de.GetGvsEndVertex()){
							toRemove.Add(de);
						}
						if(pGVSVertex==de.GetGvsStartVertex()){
							toRemove.Add(de);
						}
					}
					else if(theInterface.FullName==typeof(GVSUndirectedEdge).FullName){
						var ue = (GVSUndirectedEdge)edge;
						var vertizes=ue.GetGvsVertizes();
                        foreach (var gvsVertex in vertizes)
                        {
                            if (gvsVertex == pGVSVertex)
                            {
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
		public void Remove(GVSRelativeVertex pGVSVertex){
			var toRemove = new ArrayList();
			foreach(var edge in gvsGraphEdges){
				var interfaces=edge.GetType().GetInterfaces();
				foreach(var theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSDirectedEdge).FullName){
						var de = (GVSDirectedEdge)edge;
						if(pGVSVertex==de.GetGvsEndVertex()){
							toRemove.Add(de);
						}
						if(pGVSVertex==de.GetGvsStartVertex()){
							toRemove.Add(de);
						}
					}
					else if(theInterface.FullName==typeof(GVSUndirectedEdge).FullName){
						var ue = (GVSUndirectedEdge)edge;
						var vertizes=ue.GetGvsVertizes();
					    foreach (var gvsVertex in vertizes)
					    {
                            if (gvsVertex == pGVSVertex)
                            {
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
		public void Remove(GVSDirectedEdge pGVSEdge){
			this.gvsGraphEdges.Remove(pGVSEdge);
		}
	
		/// <summary>
		///	 Remove a UndirectedEdge
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void Remove(GVSUndirectedEdge pGVSEdge){
			this.gvsGraphEdges.Remove(pGVSEdge);
		}

		/// <summary>
		///	  Remove a Collection of GVSComponents. 
		/// </summary>
		/// <param name="pGVSComponent"></param>
		public void Remove(ICollection pGVSComponent){
			foreach(var tmp in pGVSComponent){
				var interfaces=tmp.GetType().GetInterfaces();
				foreach(var theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSDirectedEdge).FullName){
						this.Remove((GVSDirectedEdge)tmp);							 
					}
					else if(theInterface.FullName==typeof(GVSUndirectedEdge).FullName){
						this.Remove((GVSUndirectedEdge)tmp);		
					}
					else if(theInterface.FullName==typeof(GVSRelativeVertex).FullName){
						this.Remove((GVSRelativeVertex)tmp);	
					}
					else if(theInterface.FullName==typeof(GVSDefaultVertex).FullName){
						this.Remove((GVSDefaultVertex)tmp);
					}
				}
			}
		}

		/// <summary>
		///	 Remove a Array of DirectedEdges
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void Remove(GVSDirectedEdge[] pGVSEdge){
            foreach (var gvsEdge in pGVSEdge)
            {
                Remove(gvsEdge);
            }
        }
	
		/// <summary>
		///	 Remove a Array of UndirectedEdges
		/// </summary>
		/// <param name="pGVSEdge"></param>
		public void Remove(GVSUndirectedEdge[] pGVSEdge){
            foreach (var gvsEdge in pGVSEdge)
            {
                Remove(gvsEdge);
            }
        }
	
		/// <summary>
		///	 Remove a Array of DefaultVertizes
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void Remove(GVSDefaultVertex[] pGVSVertex){
		    foreach (var gvsVertex in pGVSVertex)
		    {
		        Remove(gvsVertex);
		    }
		}

		/// <summary>
		///	 Remove a Array of RealtiveVertizes
		/// </summary>
		/// <param name="pGVSVertex"></param>
		public void Remove(GVSRelativeVertex[] pGVSVertex){
            foreach (var gvsVertex in pGVSVertex)
            {
                Remove(gvsVertex);
            }
        }

		/// <summary>
		///	 Set the maxLabelLength.
		/// </summary>
		/// <param name="pMaxLength"></param>
		public void SetMaxLabelLength(int pMaxLength){
			this.maxLabelLength=pMaxLength;
		}

		/// <summary>
		///	 Build the Xml and send it to the GVSServer
		/// </summary>
		public void Display(){
			document = new XmlDocument();
			var dec = document.CreateXmlDeclaration("1.0",null,null);
			dec.Encoding=Encoding.UTF8.ToString();
			document.AppendChild(dec);
			var root = document.CreateElement(ROOT);
			document.AppendChild(root);
			
			var graph = document.CreateElement(GRAPH);
			root.AppendChild(graph);
			graph.SetAttribute(ATTRIBUTEID,this.gvsGraphId.ToString());
			
			var graphLabel= document.CreateElement(LABEL);
			graph.AppendChild(graphLabel);
			graphLabel.AppendChild(document.CreateTextNode( this.gvsGraphName));
			
			var graphBackground= document.CreateElement(BACKGROUND);
			graph.AppendChild(graphBackground);
			graphBackground.AppendChild(document.CreateTextNode(this.gvsGraphTyp.GetBackground().ToString()));
			
			var maxLabelLength = document.CreateElement(MAXLABELLENGTH);
			graph.AppendChild(maxLabelLength);
			maxLabelLength.AppendChild(document.CreateTextNode(this.maxLabelLength.ToString()));

			var vertizes = document.CreateElement(VERTIZES);
			root.AppendChild(vertizes);

			foreach(var vertex in gvsGraphVertizes){
				var interfaces=vertex.GetType().GetInterfaces();
				foreach(var theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSRelativeVertex).FullName){
						BuildRelativVertex(vertizes,(GVSRelativeVertex)vertex);
						break;	
					}
					else if(theInterface.FullName==typeof(GVSDefaultVertex).FullName){
						BuildDefaultVertex(vertizes,(GVSDefaultVertex)vertex); 
					}
				}
			}

			var edges = document.CreateElement(EDGES);
			root.AppendChild(edges);

			foreach(var edge in gvsGraphEdges){
				var interfaces=edge.GetType().GetInterfaces();
				foreach(var theInterface in interfaces){
					if(theInterface.FullName==typeof(GVSDirectedEdge).FullName){
						BuildDirectedEdge(edges,(GVSDirectedEdge)edge);
						break;	
					}
					else if(theInterface.FullName==typeof(GVSUndirectedEdge).FullName){
						BuildUndirectedEdge(edges,(GVSUndirectedEdge)edge);
						break;
					}
				}
			}

			xmlConnection.sendFile(document);
		}

		//*************************************************XML_BUILDERS****************************
		private void BuildDefaultVertex(XmlElement pParent, GVSDefaultVertex pVertex){
			var defaultVertex = document.CreateElement(DEFAULTVERTEX);
			pParent.AppendChild(defaultVertex);
			defaultVertex.SetAttribute(ATTRIBUTEID,pVertex.GetHashCode().ToString());
			var vertexTypNull=pVertex.GetGvsVertexTyp();
			if(vertexTypNull!=null){
				if(pVertex.GetGvsVertexTyp().GetType().FullName==typeof(GVSEllipseVertexTyp).FullName){
					var vertexTyp=
						((GVSEllipseVertexTyp)(pVertex.GetGvsVertexTyp()));
					var label = document.CreateElement(LABEL);
					defaultVertex.AppendChild(label);
					var vertexLabel=pVertex.GetGvsVertexLabel() ?? "";

                    label.AppendChild(document.CreateTextNode(vertexLabel));

					var lineColor = document.CreateElement(LINECOLOR);
					defaultVertex.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(vertexTyp.GetLineColor().ToString()));

					var lineStyle = document.CreateElement(LINESTYLE);
					defaultVertex.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(vertexTyp.GetLineStyle().ToString()));


					var lineThick = document.CreateElement(LINETHICKNESS);
					defaultVertex.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(vertexTyp.getLineThickness().ToString()));


					var fillColor = document.CreateElement(FILLCOLOR);
					defaultVertex.AppendChild(fillColor);
					fillColor.AppendChild(document.CreateTextNode(vertexTyp.GetFillColor().ToString()));
				}
				else if(pVertex.GetGvsVertexTyp().GetType().FullName==typeof(GVSIconVertexTyp).FullName){
					var vertexTyp= ((GVSIconVertexTyp)(pVertex.GetGvsVertexTyp()));
					var label = document.CreateElement(LABEL);
					defaultVertex.AppendChild(label);
					var vertexLabel = pVertex.GetGvsVertexLabel() ?? "";
					
					label.AppendChild(document.CreateTextNode(vertexLabel));
				
					var lineColor = document.CreateElement(LINECOLOR);
					defaultVertex.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(vertexTyp.GetLineColor().ToString()));

					var lineStyle = document.CreateElement(LINESTYLE);
					defaultVertex.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(vertexTyp.GetLineStyle().ToString()));

					var lineThick = document.CreateElement(LINETHICKNESS);
					defaultVertex.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(vertexTyp.GetLineThickness().ToString()));

					var icon = document.CreateElement(ICON);
					defaultVertex.AppendChild(icon);
					icon.AppendChild(document.CreateTextNode(vertexTyp.GetIcon().ToString()));
				}
				else{
					Console.WriteLine("VertexTyp isn't a ellipse or icon");
				}
			}
			else{
				var label = document.CreateElement(LABEL);
				defaultVertex.AppendChild(label);
				var vertexLabel=pVertex.GetGvsVertexLabel() ?? "";
				
				label.AppendChild(document.CreateTextNode(vertexLabel));
				
				var lineColor = document.CreateElement(LINECOLOR);
				defaultVertex.AppendChild(lineColor);
				lineColor.AppendChild(document.CreateTextNode(STANDARD));

				var lineStyle = document.CreateElement(LINESTYLE);
				defaultVertex.AppendChild(lineStyle);
				lineStyle.AppendChild(document.CreateTextNode(STANDARD));

				var lineThick = document.CreateElement(LINETHICKNESS);
				defaultVertex.AppendChild(lineThick);
				lineThick.AppendChild(document.CreateTextNode(STANDARD));

				var fillColor = document.CreateElement(FILLCOLOR);
				defaultVertex.AppendChild(fillColor);
				fillColor.AppendChild(document.CreateTextNode(STANDARD));
			}
		}
		
		private void BuildRelativVertex(XmlElement pParent, GVSRelativeVertex pVertex){
			var relativeVertex = document.CreateElement(RELATIVVERTEX);
			pParent.AppendChild(relativeVertex);
			relativeVertex.SetAttribute(ATTRIBUTEID,pVertex.GetHashCode().ToString());
			var vertexTypNull=pVertex.GetGvsVertexTyp();
			if(vertexTypNull!=null){
				if(pVertex.GetGvsVertexTyp().GetType().FullName==typeof(GVSEllipseVertexTyp).FullName){
					var vertexTyp= ((GVSEllipseVertexTyp)(pVertex.GetGvsVertexTyp()));
					var label = document.CreateElement(LABEL);
					relativeVertex.AppendChild(label);
					var vertexLabel=pVertex.GetGvsVertexLabel() ?? "";

					label.AppendChild(document.CreateTextNode(vertexLabel));

					var lineColor = document.CreateElement(LINECOLOR);
					relativeVertex.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(vertexTyp.GetLineColor().ToString()));

					var lineStyle = document.CreateElement(LINESTYLE);
					relativeVertex.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(vertexTyp.GetLineStyle().ToString()));

					var lineThick = document.CreateElement(LINETHICKNESS);
					relativeVertex.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(vertexTyp.getLineThickness().ToString()));

					var fillColor = document.CreateElement(FILLCOLOR);
					relativeVertex.AppendChild(fillColor);
					fillColor.AppendChild(document.CreateTextNode(vertexTyp.GetFillColor().ToString()));

					var xPos = document.CreateElement(XPOS);
					relativeVertex.AppendChild(xPos);
					xPos.AppendChild(document.CreateTextNode(pVertex.GetX().ToString()));

					var yPos = document.CreateElement(YPOS);
					relativeVertex.AppendChild(yPos);
					yPos.AppendChild(document.CreateTextNode(pVertex.GetY().ToString()));
				}
				else if(pVertex.GetGvsVertexTyp().GetType().FullName==typeof(GVSIconVertexTyp).FullName){
					var vertexTyp= ((GVSIconVertexTyp)(pVertex.GetGvsVertexTyp()));
					var label = document.CreateElement(LABEL);
					relativeVertex.AppendChild(label);
					var vertexLabel=pVertex.GetGvsVertexLabel() ?? "";

					label.AppendChild(document.CreateTextNode(vertexLabel));

					var lineColor = document.CreateElement(LINECOLOR);
					relativeVertex.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(vertexTyp.GetLineColor().ToString()));

					var lineStyle = document.CreateElement(LINESTYLE);
					relativeVertex.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(vertexTyp.GetLineStyle().ToString()));

					var lineThick = document.CreateElement(LINETHICKNESS);
					relativeVertex.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(vertexTyp.GetLineThickness().ToString()));

					var icon = document.CreateElement(ICON);
					relativeVertex.AppendChild(icon);
					icon.AppendChild(document.CreateTextNode(vertexTyp.GetIcon().ToString()));

					var xPos = document.CreateElement(XPOS);
					relativeVertex.AppendChild(xPos);
					xPos.AppendChild(document.CreateTextNode(pVertex.GetX().ToString()));

					var yPos = document.CreateElement(YPOS);
					relativeVertex.AppendChild(yPos);
					yPos.AppendChild(document.CreateTextNode(pVertex.GetY().ToString()));
				}
				else{
					Console.WriteLine("VertexTyp isn't a ellipse or icon");
				}
			}
			else{
				var label = document.CreateElement(LABEL);
				relativeVertex.AppendChild(label);
				var vertexLabel=pVertex.GetGvsVertexLabel() ?? "";
				
				label.AppendChild(document.CreateTextNode(vertexLabel));

				var lineColor = document.CreateElement(LINECOLOR);
				relativeVertex.AppendChild(lineColor);
				lineColor.AppendChild(document.CreateTextNode(STANDARD));

				var lineStyle = document.CreateElement(LINESTYLE);
				relativeVertex.AppendChild(lineStyle);
				lineStyle.AppendChild(document.CreateTextNode(STANDARD));


				var lineThick = document.CreateElement(LINETHICKNESS);
				relativeVertex.AppendChild(lineThick);
				lineThick.AppendChild(document.CreateTextNode(STANDARD));


				var fillColor = document.CreateElement(FILLCOLOR);
				relativeVertex.AppendChild(fillColor);
				fillColor.AppendChild(document.CreateTextNode(STANDARD));

				var xPos = document.CreateElement(XPOS);
				relativeVertex.AppendChild(xPos);
				xPos.AppendChild(document.CreateTextNode(pVertex.GetX().ToString()));

				var yPos = document.CreateElement(YPOS);
				relativeVertex.AppendChild(yPos);
				yPos.AppendChild(document.CreateTextNode(pVertex.GetY().ToString()));
			}
		}

		private void BuildDirectedEdge(XmlElement pParent, GVSDirectedEdge pEdge){
			var vertex1 = pEdge.GetGvsStartVertex();
			var vertex2 = pEdge.GetGvsEndVertex();
			var vertex1Exist = false;
			var vertex2Exist = false;
		
			foreach(var vertex in gvsGraphVertizes){
				if(vertex==vertex1){
					vertex1Exist=true;
				}
				if(vertex==vertex2){
					vertex2Exist=true;
				}
			}
		
			if(vertex1Exist==true && vertex2Exist==true &&vertex1!=null && vertex2!=null){
				var edgeTyp = pEdge.GetGvsEdgeTyp();
				var directedEdge = document.CreateElement(EDGE);
				pParent.AppendChild(directedEdge);
				directedEdge.SetAttribute(ATTRIBUTEID,pEdge.GetHashCode().ToString());
				directedEdge.SetAttribute(ISDIRECTED,"true");
				if(edgeTyp!=null){
					var label = document.CreateElement(LABEL);
					directedEdge.AppendChild(label);
					var edgeLabel=pEdge.GetGvsEdgeLabel() ?? "";
					
					label.AppendChild(document.CreateTextNode(edgeLabel));

					var lineColor = document.CreateElement(LINECOLOR);
					directedEdge.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(edgeTyp.GetLineColor().ToString()));

					var lineStyle = document.CreateElement(LINESTYLE);
					directedEdge.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(edgeTyp.GetLineStyle().ToString()));


					var lineThick = document.CreateElement(LINETHICKNESS);
					directedEdge.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(edgeTyp.GetLineThickness().ToString()));


					var fromVertex = document.CreateElement(FROMVERTEX);
					directedEdge.AppendChild(fromVertex);
					fromVertex.AppendChild(document.CreateTextNode(pEdge.GetGvsStartVertex().GetHashCode().ToString()));
					
					var toVertex = document.CreateElement(TOVERTEX);
					directedEdge.AppendChild(toVertex);
					toVertex.AppendChild(document.CreateTextNode(pEdge.GetGvsEndVertex().GetHashCode().ToString()));
				}

				else{
					var label = document.CreateElement(LABEL);
					directedEdge.AppendChild(label);
					var edgeLabel=pEdge.GetGvsEdgeLabel() ?? "";
					
					label.AppendChild(document.CreateTextNode(edgeLabel));

					var lineColor = document.CreateElement(LINECOLOR);
					directedEdge.AppendChild(lineColor);
					lineColor.AppendChild(document.CreateTextNode(STANDARD));

					var lineStyle = document.CreateElement(LINESTYLE);
					directedEdge.AppendChild(lineStyle);
					lineStyle.AppendChild(document.CreateTextNode(STANDARD));

					var lineThick = document.CreateElement(LINETHICKNESS);
					directedEdge.AppendChild(lineThick);
					lineThick.AppendChild(document.CreateTextNode(STANDARD));
					
					var fromVertex = document.CreateElement(FROMVERTEX);
					directedEdge.AppendChild(fromVertex);
					fromVertex.AppendChild(document.CreateTextNode(pEdge.GetGvsStartVertex().GetHashCode().ToString()));
					
					var toVertex = document.CreateElement(TOVERTEX);
					directedEdge.AppendChild(toVertex);
					toVertex.AppendChild(document.CreateTextNode(pEdge.GetGvsEndVertex().GetHashCode().ToString()));				}
			}
			else{
				Console.WriteLine("Vertex missing or null");
			}
		}

		private void BuildUndirectedEdge(XmlElement pParent, GVSUndirectedEdge pEdge){
			if(pEdge.GetGvsVertizes()!=null){
				var vertex1 = pEdge.GetGvsVertizes()[0];
				var vertex2 = pEdge.GetGvsVertizes()[1];
				var vertex1Exist=false;
				var vertex2Exist=false;
			
				foreach(var vertex in gvsGraphVertizes){
					if(vertex==vertex1){
						vertex1Exist=true;
					}
					if(vertex==vertex2){
						vertex2Exist=true;
					}
				}

				if(vertex1Exist==true && vertex2Exist==true &&vertex1!=null &&vertex2!=null){
					var edgeTyp = pEdge.GetGvsEdgeTyp();
					var undirectedEdge = document.CreateElement(EDGE);
					pParent.AppendChild(undirectedEdge);
					undirectedEdge.SetAttribute(ATTRIBUTEID,pEdge.GetHashCode().ToString());
					var arrowPos=pEdge.HasArrow();
					undirectedEdge.SetAttribute(ISDIRECTED,"false");
					undirectedEdge.SetAttribute(ARROWPOS,arrowPos.ToString());
					if(edgeTyp!=null){
						var label = document.CreateElement(LABEL);
						undirectedEdge.AppendChild(label);
						var edgeLabel=pEdge.GetGvsEdgeLabel() ?? "";

						label.AppendChild(document.CreateTextNode(edgeLabel));

						var lineColor = document.CreateElement(LINECOLOR);
						undirectedEdge.AppendChild(lineColor);
						lineColor.AppendChild(document.CreateTextNode(STANDARD));

						var lineStyle = document.CreateElement(LINESTYLE);
						undirectedEdge.AppendChild(lineStyle);
						lineStyle.AppendChild(document.CreateTextNode(STANDARD));


						var lineThick = document.CreateElement(LINETHICKNESS);
						undirectedEdge.AppendChild(lineThick);
						lineThick.AppendChild(document.CreateTextNode(STANDARD));
						
						
						var fromVertex = document.CreateElement(FROMVERTEX);
						undirectedEdge.AppendChild(fromVertex);
						fromVertex.AppendChild(document.CreateTextNode(pEdge.GetGvsVertizes()[0].GetHashCode().ToString()));
						
						var toVertex = document.CreateElement(TOVERTEX);
						undirectedEdge.AppendChild(toVertex);
						toVertex.AppendChild(document.CreateTextNode(pEdge.GetGvsVertizes()[1].GetHashCode().ToString()));
					}
					else{
						var label = document.CreateElement(LABEL);
						undirectedEdge.AppendChild(label);
						var edgeLabel=pEdge.GetGvsEdgeLabel() ?? "";

						label.AppendChild(document.CreateTextNode(edgeLabel));

						var lineColor = document.CreateElement(LINECOLOR);
						undirectedEdge.AppendChild(lineColor);
						lineColor.AppendChild(document.CreateTextNode(STANDARD));

						var lineStyle = document.CreateElement(LINESTYLE);
						undirectedEdge.AppendChild(lineStyle);
						lineStyle.AppendChild(document.CreateTextNode(STANDARD));

						var lineThick = document.CreateElement(LINETHICKNESS);
						undirectedEdge.AppendChild(lineThick);
						lineThick.AppendChild(document.CreateTextNode(STANDARD));
						
						var fromVertex = document.CreateElement(FROMVERTEX);
						undirectedEdge.AppendChild(fromVertex);
						fromVertex.AppendChild(document.CreateTextNode(pEdge.GetGvsVertizes()[0].GetHashCode().ToString()));
						
						var toVertex = document.CreateElement(TOVERTEX);
						undirectedEdge.AppendChild(toVertex);
						toVertex.AppendChild(document.CreateTextNode(pEdge.GetGvsVertizes()[1].GetHashCode().ToString()));
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

		public void Disconnect(){
			xmlConnection.disconnectFromServer();
		}

		~GVSGraph() {
			this.Disconnect();
		}

	}
}
