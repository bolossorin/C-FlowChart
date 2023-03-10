using Crainiate.Diagramming;
using Crainiate.Diagramming.Flowcharting;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowchartConverter.Nodes
{
    abstract class DecisionNode :BaseNode
    {
        protected const int horizontalSpace = 100;
        protected const int MOVE_UP = 1;
        protected const int MOVE_DOWN = 2;
        protected const int MOVE_RIGHT = 3;
        protected int moveDirection = MOVE_DOWN;
        private HolderNode trueNode;
        private HolderNode backNode;
        protected ConnectorNode trueConnector;
        // ConnectorNode backConnector;
        readonly private string holderTag;
        readonly private string backConnectorTag;
        readonly private string trueConnectorTag;
        
        public bool shifted = false;
        public override PointF NodeLocation
        {
            get
            {
                return nodeLocation;
            }

            set
            {
                if (value.X != NodeLocation.X || value.Y != NodeLocation.Y)
                {
                    if (value.Y > NodeLocation.Y)
                    {
                        moveDirection = MOVE_DOWN;
                    }
                    else if (value.X > NodeLocation.X) {
                        moveDirection = MOVE_RIGHT;
                    }
                    else
                    {
                        moveDirection = MOVE_UP;
                    }
                    base.NodeLocation = value;
                    
                    moveConnections();
                }
            }
        }

        public HolderNode TrueNode
        {
            get
            {
                return trueNode;
            }

            set
            {
                trueNode = value;
            }
        }

        public HolderNode BackNode
        {
            get
            {
                return backNode;
            }

            set
            {
                backNode = value;
            }
        }

        public ConnectorNode TrueConnector
        {
            get
            {
                return trueConnector;
            }

            set
            {
                trueConnector = value;
            }
        }

        public DecisionNode()
        {

            Shape.StencilItem = Stencil[FlowchartStencilType.Preparation];
            Shape.BackColor = System.Drawing.ColorTranslator.FromHtml("#e06000");
            Shape.GradientColor = Color.Black;
           
                            
            holderTag = ShapeTag + " holder";
            backConnectorTag = ShapeTag + " backConnector";
            trueConnectorTag = ShapeTag + " trueConnector";
            

            makeConnections();

        }

        abstract protected void makeConnections();
       
        abstract protected void moveConnections();

       
        public override void addRemoveFlag(bool v)
        {
            BackNode.ToBeRemoved = true;
            TrueNode.ToBeRemoved = true;
            BaseNode nextNode = TrueNode;
            while (nextNode.OutConnector.EndNode != BackNode)
            {
                nextNode.OutConnector.EndNode.addRemoveFlag(true);
                nextNode = nextNode.OutConnector.EndNode;
            }
            nextNode = null;
            //base.addRemoveFlag(v);
            this.ToBeRemoved = true;
        }
        override public void addToModel()
        {
            base.addToModel();
            TrueNode.addToModel();
            BackNode.addToModel();
            //Controller.Model.Shapes.Add(holderTag,holderNode.Shape);
            //  Model.Lines.Add(backConnectorTag, backConnector.Connector);
        }
        protected void attachToTrue(BaseNode newNode, bool addToEnd)
        {
            if (addToEnd) //this means that the clicked link is between holder and loop
            {
                //add this node to last node in true link
                BaseNode lastNode = TrueNode;
                while (!(lastNode.OutConnector.EndNode is HolderNode))
                {
                    lastNode = lastNode.OutConnector.EndNode;

                }
                lastNode.attachNode(newNode);
            }
            else
                TrueNode.attachNode(newNode);


        }
      
        public override void attachNode(BaseNode newNode, ConnectorNode clickedConnector)
        {
           
            clickedConnector.StartNode.attachNode(newNode);
            if (OutConnector.EndNode.NodeLocation.Y < BackNode.NodeLocation.Y+shiftY)
                shiftMainTrack(); //this causes a problem when 
                              //backNode shifts dirctely after being attached to another while node

        }

        public abstract void shiftMainTrack(int moreShift = 0);
        override public void setText(String label) {
            float oldWidth = Shape.Width;
            base.setText(label);
            //if the shape is bigger then move nodes to right by the difference
            if (!Controller.LoadingProject)
            {
                TrueNode.shiftRight((int)(Shape.Width - oldWidth));
                BackNode.shiftRight((int)(Shape.Width - oldWidth));
            }
        }

    }

}
