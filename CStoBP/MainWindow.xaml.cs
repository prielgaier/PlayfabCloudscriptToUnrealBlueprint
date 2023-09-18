using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CStoBP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string prefix = "";
        public static bool printString = true;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] functionArgs = GetArgumentsFromCloudscriptFunction(TB_CloudscriptFunction.Text);
            string functionName = GetFunctionNameFromCloudscriptFunction(TB_CloudscriptFunction.Text);

            GraphNodeCustomEvent customEventNode = new GraphNodeCustomEvent(functionName, functionArgs);

            GraphNode linkToSetJsonParameters;
            GraphNodePrintString printStringNode = new GraphNodePrintString(customEventNode);

            if (printString)
            {

                linkToSetJsonParameters = printStringNode;
            }
            else
            {
                linkToSetJsonParameters = customEventNode;
            }
            if (functionArgs.Length > 0)
            {
                GraphNodeConstructJsonObject graphNodeConstructJsonObject = new GraphNodeConstructJsonObject();
            GraphNodeSetJsonVariable graphNodeSetJsonVariable = new GraphNodeSetJsonVariable(linkToSetJsonParameters, graphNodeConstructJsonObject, functionName);

            //a list of GraphNodeSetStringFieldInJson
            List<GraphNodeSetStringFieldInJson> graphNodesSetStringFieldInJsonList = new List<GraphNodeSetStringFieldInJson>();

            for (int i = 0; i < functionArgs.Length; i++)
            {
                if(i == 0)
                {
                    graphNodesSetStringFieldInJsonList.Add(new GraphNodeSetStringFieldInJson(graphNodeSetJsonVariable.Pins[1], customEventNode.Pins[2 + i], functionArgs[i], i));
                    graphNodesSetStringFieldInJsonList[0].Pins[2].LinkToPin(graphNodeSetJsonVariable.Pins[3]);
                }
                else
                {
                    graphNodesSetStringFieldInJsonList.Add(new GraphNodeSetStringFieldInJson(graphNodesSetStringFieldInJsonList[i - 1].Pins[1], customEventNode.Pins[2 + i], functionArgs[i], i));
                    graphNodesSetStringFieldInJsonList[i - 1].Pins[2].LinkToPin(graphNodeSetJsonVariable.Pins[3]);
                }
            }

            string setStringFieldsNodes = "";


                foreach (GraphNodeSetStringFieldInJson setStringFieldinJsonNode in graphNodesSetStringFieldInJsonList)
                {
                    setStringFieldsNodes = setStringFieldsNodes + setStringFieldinJsonNode.GetBeginObjectClassString() + "\n";
                }

                GraphNodeMakeRequestStruct graphNodeMakeRequestStruct = new GraphNodeMakeRequestStruct(graphNodeSetJsonVariable.Pins[3], functionName, functionArgs.Length);

                GraphNodeSetStringFieldInJson lastNode = graphNodesSetStringFieldInJsonList[graphNodesSetStringFieldInJsonList.Count - 1];
                GraphNodeExecuteCloudScript graphNodeExecuteCloudScript = new GraphNodeExecuteCloudScript(lastNode.Pins[1], graphNodeMakeRequestStruct.Pins[0]);

                List<GraphPin> pinsToLinkJsonSetOutput = new List<GraphPin>();
                for (int i = 0; i < functionArgs.Length; i++)
                {
                    pinsToLinkJsonSetOutput.Add(graphNodesSetStringFieldInJsonList[i].Pins[2]);
                }
                pinsToLinkJsonSetOutput.Add(graphNodeMakeRequestStruct.Pins[3]);
                graphNodeSetJsonVariable.Pins[3].LinkToPins(pinsToLinkJsonSetOutput);
                graphNodeSetJsonVariable.Pins[3].CloseGraphPinText();

                GraphNodeCustomSuccessEvent graphNodeCustomSuccessEvent = new GraphNodeCustomSuccessEvent(functionName, graphNodeExecuteCloudScript.Pins[7], graphNodeExecuteCloudScript);
                GraphNodeBreakResultStruct graphNodeBreakResultStruct = new GraphNodeBreakResultStruct(graphNodeCustomSuccessEvent);
                GraphNodeCustomFailedEvent graphNodeCustomFailedEvent = new GraphNodeCustomFailedEvent(functionName, graphNodeExecuteCloudScript.Pins[8], graphNodeExecuteCloudScript);
                GraphNodeHandleError graphNodeHandleError = new GraphNodeHandleError(graphNodeCustomFailedEvent);
                GraphNodeMacroHandleSuccessButError graphNodeMacroHandleSuccessButError = new GraphNodeMacroHandleSuccessButError(graphNodeCustomSuccessEvent.Pins[1], graphNodeBreakResultStruct.Pins[1], graphNodeBreakResultStruct);

                if (printString)
                {
                    Clipboard.SetText(customEventNode.GetBeginObjectClassString() + "\n" + printStringNode.GetBeginObjectClassString() + "\n" + graphNodeConstructJsonObject.GetBeginObjectClassString() + "\n" + graphNodeSetJsonVariable.GetBeginObjectClassString() + "\n" + setStringFieldsNodes + graphNodeMakeRequestStruct.GetBeginObjectClassString() + "\n" + graphNodeExecuteCloudScript.GetBeginObjectClassString() + "\n" + graphNodeCustomSuccessEvent.GetBeginObjectClassString() + "\n" + graphNodeCustomFailedEvent.GetBeginObjectClassString() + "\n" + graphNodeHandleError.GetBeginObjectClassString() + "\n" + graphNodeBreakResultStruct.GetBeginObjectClassString() + "\n" + graphNodeMacroHandleSuccessButError.GetBeginObjectClassString());
                }
                else
                {
                    Clipboard.SetText(customEventNode.GetBeginObjectClassString() + "\n" + graphNodeConstructJsonObject.GetBeginObjectClassString() + "\n" + graphNodeSetJsonVariable.GetBeginObjectClassString() + "\n" + setStringFieldsNodes + graphNodeMakeRequestStruct.GetBeginObjectClassString() + "\n" + graphNodeExecuteCloudScript.GetBeginObjectClassString() + "\n" + graphNodeCustomSuccessEvent.GetBeginObjectClassString() + "\n" + graphNodeCustomFailedEvent.GetBeginObjectClassString() + "\n" + graphNodeHandleError.GetBeginObjectClassString() + "\n" + graphNodeBreakResultStruct.GetBeginObjectClassString() + "\n" + graphNodeMacroHandleSuccessButError.GetBeginObjectClassString());
                }

                //show copied to clipboard
                BTN_Convert.Content = "Copied to clipboard!";
                //delay 2 seconds
                Task.Delay(2000).ContinueWith(_ =>
                {
                    //after 2 seconds, reset button text
                    BTN_Convert.Dispatcher.Invoke(() => BTN_Convert.Content = "Convert");
                });
            }
            else if (functionArgs.Length == 0)
            {

                GraphNodeMakeRequestStruct graphNodeMakeRequestStruct = new GraphNodeMakeRequestStruct(functionName, 0);

                GraphNodeMakeRequestStruct lastNode = graphNodeMakeRequestStruct;
                GraphNodeExecuteCloudScript graphNodeExecuteCloudScript = new GraphNodeExecuteCloudScript(lastNode.Pins[1], graphNodeMakeRequestStruct.Pins[0]);



                GraphNodeCustomSuccessEvent graphNodeCustomSuccessEvent = new GraphNodeCustomSuccessEvent(functionName, graphNodeExecuteCloudScript.Pins[7], graphNodeExecuteCloudScript);
                GraphNodeBreakResultStruct graphNodeBreakResultStruct = new GraphNodeBreakResultStruct(graphNodeCustomSuccessEvent);
                GraphNodeCustomFailedEvent graphNodeCustomFailedEvent = new GraphNodeCustomFailedEvent(functionName, graphNodeExecuteCloudScript.Pins[8], graphNodeExecuteCloudScript);
                GraphNodeHandleError graphNodeHandleError = new GraphNodeHandleError(graphNodeCustomFailedEvent);
                GraphNodeMacroHandleSuccessButError graphNodeMacroHandleSuccessButError = new GraphNodeMacroHandleSuccessButError(graphNodeCustomSuccessEvent.Pins[1], graphNodeBreakResultStruct.Pins[1], graphNodeBreakResultStruct);

                if (printString)
                {
                    Clipboard.SetText(customEventNode.GetBeginObjectClassString() + "\n" + printStringNode.GetBeginObjectClassString() + "\n" + graphNodeExecuteCloudScript.GetBeginObjectClassString() + "\n" + graphNodeCustomSuccessEvent.GetBeginObjectClassString() + "\n" + graphNodeCustomFailedEvent.GetBeginObjectClassString() + "\n" + graphNodeHandleError.GetBeginObjectClassString() + "\n" + graphNodeBreakResultStruct.GetBeginObjectClassString() + "\n" + graphNodeMacroHandleSuccessButError.GetBeginObjectClassString());
                }
                else
                {
                    Clipboard.SetText(customEventNode.GetBeginObjectClassString() + "\n" + graphNodeMakeRequestStruct.GetBeginObjectClassString() + "\n" + graphNodeExecuteCloudScript.GetBeginObjectClassString() + "\n" + graphNodeCustomSuccessEvent.GetBeginObjectClassString() + "\n" + graphNodeCustomFailedEvent.GetBeginObjectClassString() + "\n" + graphNodeHandleError.GetBeginObjectClassString() + "\n" + graphNodeBreakResultStruct.GetBeginObjectClassString() + "\n" + graphNodeMacroHandleSuccessButError.GetBeginObjectClassString());
                }

                //show copied to clipboard
                BTN_Convert.Content = "Copied to clipboard!";
                //delay 2 seconds
                Task.Delay(2000).ContinueWith(_ =>
                {
                    //after 2 seconds, reset button text
                    BTN_Convert.Dispatcher.Invoke(() => BTN_Convert.Content = "Convert");
                });
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TB_CloudscriptFunction != null && TB_CloudscriptFunction.Text.Length > 12 && TB_CloudscriptFunction.Text.Substring(0, 8) == "handlers")
            {
                TB_FunctionArguments.Text = "";
                string[] args = GetArgumentsFromCloudscriptFunction(TB_CloudscriptFunction.Text);
                string funcName = GetFunctionNameFromCloudscriptFunction(TB_CloudscriptFunction.Text);
                TB_FunctionName.Text = funcName;
                foreach (string arg in args)
                {
                    if(arg == args[0])
                    {
                        TB_FunctionArguments.Text = arg;
                    }
                    else
                    {
                        TB_FunctionArguments.Text = TB_FunctionArguments.Text + "\n" + arg;
                    }
                }
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            prefix = TB_Prefix.Text;
        }

        private void CB_PrintString_Checked(object sender, RoutedEventArgs e)
        {
            printString = true;
        }

        private void CB_PrintString_Unchecked(object sender, RoutedEventArgs e)
        {
            printString = false;
        }


        #region <Classes>

        class GraphNode
        {
            public string Class = "";
            public string Name = "";
            public string CustomFunctionName = "";
            public string NodePosX = "11568";
            public string NodePosY = "11152";
            public string ErrorType = "4";
            public string NodeGuid = GenerateRandomHexNumber();
            public GraphPin[] Pins = { };
            public GraphPin[] UserDefinedPins = { };
            public GraphNode()
            {
            }
            public GraphNode(GraphPin[] pins)
            {
                Pins = pins;
            }
            //add a pin to a graph node
            public void AddPin(GraphPin pin)
            {
                GraphPin[] pins = new GraphPin[Pins.Length + 1];
                for (int i = 0; i < Pins.Length; i++)
                {
                    pins[i] = Pins[i];
                }
                pins[Pins.Length] = pin;
                pins[Pins.Length]._ParentNodeRef = this;
                Pins = pins;
            }
            public void AddUserDefinedPin(GraphPin pin)
            {
                GraphPin[] pins = new GraphPin[UserDefinedPins.Length + 1];
                for (int i = 0; i < UserDefinedPins.Length; i++)
                {
                    pins[i] = UserDefinedPins[i];
                }
                pins[UserDefinedPins.Length] = pin;
                pins[UserDefinedPins.Length]._ParentNodeRef = this;
                UserDefinedPins = pins;
            }

            public virtual string GetBeginObjectClassString()
            {

                if (Pins.Length > 0)
                {
                    if (UserDefinedPins.Length > 0)
                    {
                        string myPins = PinsToString(Pins);
                        string myUserDefinedPins = PinsToString(UserDefinedPins);
                        return
                           "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                           "   " + "CustomFunctionName=" + Wrap(CustomFunctionName) + "\n" +
                           "   " + "NodePosX=" + NodePosX + "\n" +
                           "   " + "NodePosY=" + NodePosY + "\n" +
                           "   " + "ErrorType=" + ErrorType + "\n" +
                           "   " + "NodeGuid=" + NodeGuid + "\n" +
                           "   " + myPins + "\n" +
                           "   " + myUserDefinedPins + " \n" +
                           "End Object";
                    }
                    if (UserDefinedPins.Length <= 0)
                    {

                        string myPins = PinsToString(Pins);
                        return
                           "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                           "   " + "CustomFunctionName=" + Wrap(CustomFunctionName) + "\n" +
                           "   " + "NodePosX=" + NodePosX + "\n" +
                           "   " + "NodePosY=" + NodePosY + "\n" +
                           "   " + "ErrorType=" + ErrorType + "\n" +
                           "   " + "NodeGuid=" + NodeGuid + "\n" +
                           "   " + myPins + "\n" +
                           "End Object";
                    }
                }

                return
                    "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                    "   " + "CustomFunctionName=" + Wrap(CustomFunctionName) + "\n" +
                    "   " + "NodePosX=" + NodePosX + "\n" +
                    "   " + "NodePosY=" + NodePosY + "\n" +
                    "   " + "ErrorType=" + ErrorType + "\n" +
                    "   " + "NodeGuid=" + NodeGuid + "\n" +
                    "End Object";
            }

            public string PinsToString(GraphPin[] PinsToString)
            {
                string stringified = "";
                if (PinsToString != null)
                {
                    foreach (GraphPin pin in PinsToString)
                    {
                        if (stringified == "")
                        {
                            stringified = pin.GetCustomPropertiesString();
                        }
                        else
                        {
                            stringified = stringified + "\n" + "   " + pin.GetCustomPropertiesString();
                        }
                    }
                    //stringified = stringified + "\n";
                    return stringified;
                }
                return stringified;
            }
        }
        class GraphNodeKismetFunction : GraphNode
        {
            public string FunctionReference = "";
            public override string GetBeginObjectClassString()
            {
                if (Pins.Length > 0)
                {
                    string myPins = PinsToString(Pins);
                    return
                       "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                       "   " + "FunctionReference=" + FunctionReference + "\n" +
                       "   " + "NodePosX=" + NodePosX + "\n" +
                       "   " + "NodePosY=" + NodePosY + "\n" +
                       "   " + "ErrorType=" + ErrorType + "\n" +
                       "   " + "NodeGuid=" + NodeGuid + "\n" +
                       "   " + myPins + "\n" +
                       "End Object";
                }
                return
                    "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                    "   " + "FunctionReference=" + FunctionReference + "\n" +
                    "   " + "NodePosX=" + NodePosX + "\n" +
                    "   " + "NodePosY=" + NodePosY + "\n" +
                    "   " + "ErrorType=" + ErrorType + "\n" +
                    "   " + "NodeGuid=" + NodeGuid + "\n" +
                    "End Object";
            }
        }
        class GraphNodeKismetMacro : GraphNode
        {
            public string MacroReference = "";
            public override string GetBeginObjectClassString()
            {
                if (Pins.Length > 0)
                {
                    string myPins = PinsToString(Pins);
                    return
                       "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                       "   " + "MacroGraphReference=" + MacroReference + "\n" +
                       "   " + "NodePosX=" + NodePosX + "\n" +
                       "   " + "NodePosY=" + NodePosY + "\n" +
                       "   " + "NodeGuid=" + NodeGuid + "\n" +
                       "   " + myPins + "\n" +
                       "End Object";
                }
                return
                    "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                    "   " + "MacroGraphReference=" + MacroReference + "\n" +
                    "   " + "NodePosX=" + NodePosX + "\n" +
                    "   " + "NodePosY=" + NodePosY + "\n" +
                    "   " + "NodeGuid=" + NodeGuid + "\n" +
                    "End Object";
            }
        }
        class GraphNodeKismetVariable : GraphNode
        {
            public string VariableReference = "";
            public override string GetBeginObjectClassString()
            {
                if (Pins.Length > 0)
                {
                    string myPins = PinsToString(Pins);
                    return
                       "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                       "   " + "VariableReference=" + VariableReference + "\n" +
                       "   " + "NodePosX=" + NodePosX + "\n" +
                       "   " + "NodePosY=" + NodePosY + "\n" +
                       "   " + "ErrorType=" + ErrorType + "\n" +
                       "   " + "NodeGuid=" + NodeGuid + "\n" +
                       "   " + myPins + "\n" +
                       "End Object";
                }
                return
                    "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                    "   " + "VariableReference=" + VariableReference + "\n" +
                    "   " + "NodePosX=" + NodePosX + "\n" +
                    "   " + "NodePosY=" + NodePosY + "\n" +
                    "   " + "ErrorType=" + ErrorType + "\n" +
                    "   " + "NodeGuid=" + NodeGuid + "\n" +
                    "End Object";
            }
        }
        class GraphNodeMacroHandleSuccessButError : GraphNodeKismetMacro
        {
            public GraphNodeMacroHandleSuccessButError(GraphPin thenPinToLink, GraphPin errorPinToLink, GraphNodeBreakResultStruct graphNodeForCoordinates)
            {
                Class = "/Script/BlueprintGraph.K2Node_MacroInstance";
                Name = "K2Node_MacroInstance_1";
                NodePosX = (int.Parse(graphNodeForCoordinates.NodePosX) + 416).ToString();
                NodePosY = (int.Parse(graphNodeForCoordinates.NodePosY) - 64).ToString();
                MacroReference = "(MacroGraph=/Script/Engine.EdGraph'\"Handle Cloudscript Success Call But Fail Due to error\"',GraphBlueprint=/Script/Engine.Blueprint'\"/Game/_D/BPs/GameInstanceBP.GameInstanceBP\"',GraphGuid=7C48E03C47A0E6C9DF74BDB0509A9738)";
                AddPin(new GraphPinExec(this, thenPinToLink, "exec"));
                AddPin(new GraphPinJsonIn(this, errorPinToLink, "error", "", "", false));
                AddPin(new GraphPinThen(this, "Success"));
                AddPin(new GraphPinThen(this, "Fail"));
            }
        }
        class GraphNodeCustomEvent : GraphNode
        {
            public GraphNodeCustomEvent(string customFunctionName, string[] customFunctionArgs)
            {
                Class = "/Script/BlueprintGraph.K2Node_CustomEvent";
                Name = "K2Node_CustomEvent_54";
                NodePosX = "64";
                NodePosY = "272";
                if (prefix == "")
                {
                    CustomFunctionName = customFunctionName;
                }
                else
                {
                    CustomFunctionName = prefix + "_" + customFunctionName;
                }
                AddPin(new GraphPinDelegateHandle(this));
                AddPin(new GraphPinThen(this));
                foreach(string argumentName in customFunctionArgs)
                {
                    AddPin(new GraphPinStringOut(this, argumentName));
                    AddUserDefinedPin(new GraphPinStringUserDefined(this, argumentName));
                }
            }

        }
        class GraphNodeCustomSuccessEvent : GraphNode
        {
            public GraphNodeCustomSuccessEvent(string customFunctionName, GraphPin pinToLink, GraphNode nodeBefore)
            {
                Class = "/Script/BlueprintGraph.K2Node_CustomEvent";
                Name = "K2Node_CustomEvent_8";
                NodePosX = (int.Parse(nodeBefore.NodePosX)).ToString();
                NodePosY = (int.Parse(nodeBefore.NodePosY) - 160).ToString();
                if (prefix == "")
                {
                    CustomFunctionName = "Success_" + customFunctionName;
                }
                else
                {
                    CustomFunctionName = "Success_" + prefix + "_" + customFunctionName;
                }
                AddPin(new GraphPinDelegateHandle(this, pinToLink));
                AddPin(new GraphPinThen(this));
                AddPin(new GraphPinResultStructOut(this));
                AddPin(new GraphPinCustomDataOut(this));
                AddUserDefinedPin(new GraphPinUserDefinedSuccessStructOut(this));
                AddUserDefinedPin(new GraphPinUserDefinedCustomDataOut(this));
            }

        }
        class GraphNodeCustomFailedEvent : GraphNode
        {
            public GraphNodeCustomFailedEvent(string customFunctionName, GraphPin pinToLink, GraphNode nodeBefore)
            {
                Class = "/Script/BlueprintGraph.K2Node_CustomEvent";
                Name = "K2Node_CustomEvent_9";
                NodePosX = (int.Parse(nodeBefore.NodePosX)).ToString();
                NodePosY = (int.Parse(nodeBefore.NodePosY) + 224).ToString();
                if (prefix == "")
                {
                    CustomFunctionName = "Failed_" + customFunctionName;
                }
                else
                {
                    CustomFunctionName = "Failed_" + prefix + "_" + customFunctionName;
                }
                AddPin(new GraphPinDelegateHandle(this, pinToLink));
                AddPin(new GraphPinThen(this));
                AddPin(new GraphPinFailedStructOut(this));
                AddPin(new GraphPinCustomDataOut(this));
                AddUserDefinedPin(new GraphPinUserDefinedFailedStructOut(this));
                AddUserDefinedPin(new GraphPinUserDefinedCustomDataOut(this));
            }

        }
        class GraphNodePrintString : GraphNodeKismetFunction
        {
            public GraphNodePrintString(GraphNode nodeBefore)
            {
                Class = "/Script/BlueprintGraph.K2Node_CallFunction";
                FunctionReference = "(MemberParent=/Script/CoreUObject.Class'\"/Script/Engine.KismetSystemLibrary\"',MemberName=\"PrintString\")";
                Name = "K2Node_CallFunction_21";
                NodePosX = "64";
                NodePosY = "96";
                AddPin(new GraphPin(this));
                Pins[0].OpenGraphPinText();
                Pins[0].PinId(GenerateRandomHexNumber());
                Pins[0].PinName("execute");
                Pins[0].PinToolTip("\\nExec");
                Pins[0].PinCategory("exec");
                Pins[0].PinSubCategory("");
                Pins[0].PinSubCategoryObject("None");
                Pins[0].PinSubCategoryMemberReference("()");
                Pins[0].PinValueType("()");
                Pins[0].ContainerType(EPinContainerType.None);
                Pins[0].bIsReference(false);
                Pins[0].bIsConst(false);
                Pins[0].bIsWeakPointer(false);
                Pins[0].bIsUObjectWrapper(false);
                Pins[0].bSerializeAsSinglePrecisionFloat(false);
                Pins[0].LinkToPin(nodeBefore.Pins[1]);
                Pins[0].PersistentGuid("00000000000000000000000000000000");
                Pins[0].bHidden(false);
                Pins[0].bNotConnectable(false);
                Pins[0].bDefaultValueIsReadOnly(false);
                Pins[0].bDefaultValueIsIgnored(false);
                Pins[0].bAdvancedView(false);
                Pins[0].bOrphanedPin(false);
                Pins[0].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[1].OpenGraphPinText();
                Pins[1].PinId(GenerateRandomHexNumber());
                Pins[1].PinName("then");
                Pins[1].PinToolTip("\\nExec");
                Pins[1].Direction(EEdGraphPinDirection.EGPD_Output);
                Pins[1].PinCategory("exec");
                Pins[1].PinSubCategory("");
                Pins[1].PinSubCategoryObject("None");
                Pins[1].PinSubCategoryMemberReference("()");
                Pins[1].PinValueType("()");
                Pins[1].ContainerType(EPinContainerType.None);
                Pins[1].bIsReference(false);
                Pins[1].bIsConst(false);
                Pins[1].bIsWeakPointer(false);
                Pins[1].bIsUObjectWrapper(false);
                Pins[1].bSerializeAsSinglePrecisionFloat(false);
                Pins[1].PersistentGuid("00000000000000000000000000000000");
                Pins[1].bHidden(false);
                Pins[1].bNotConnectable(false);
                Pins[1].bDefaultValueIsReadOnly(false);
                Pins[1].bDefaultValueIsIgnored(false);
                Pins[1].bAdvancedView(false);
                Pins[1].bOrphanedPin(false);
                Pins[1].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[2].OpenGraphPinText();
                Pins[2].PinId(GenerateRandomHexNumber());
                Pins[2].PinName("self");
                Pins[2].PinFriendlyName("NSLOCTEXT(\"K2Node\", \"Target\", \"Target\")");
                Pins[2].PinToolTip("Target\\nKismet System Library Object Reference");
                Pins[2].PinCategory("object");
                Pins[2].PinSubCategory("");
                Pins[2].PinSubCategoryObject("/Script/CoreUObject.Class'" + Wrap("/Script/Engine.KismetSystemLibrary") + "'");
                Pins[2].PinSubCategoryMemberReference("()");
                Pins[2].PinValueType("()");
                Pins[2].ContainerType(EPinContainerType.None);
                Pins[2].bIsReference(false);
                Pins[2].bIsConst(false);
                Pins[2].bIsWeakPointer(false);
                Pins[2].bIsUObjectWrapper(false);
                Pins[2].bSerializeAsSinglePrecisionFloat(false);
                Pins[2].DefaultObject("/Script/Engine.Default__KismetSystemLibrary");
                Pins[2].PersistentGuid("00000000000000000000000000000000");
                Pins[2].bHidden(true);
                Pins[2].bNotConnectable(false);
                Pins[2].bDefaultValueIsReadOnly(false);
                Pins[2].bDefaultValueIsIgnored(false);
                Pins[2].bAdvancedView(false);
                Pins[2].bOrphanedPin(false);
                Pins[2].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[3].OpenGraphPinText();
                Pins[3].PinId(GenerateRandomHexNumber());
                Pins[3].PinName("WorldContextObject");
                Pins[3].PinToolTip("World Context Object\\nObject Reference");
                Pins[3].PinCategory("object");
                Pins[3].PinSubCategory("");
                Pins[3].PinSubCategoryObject("/Script/CoreUObject.Class'" + Wrap("/Script/CoreUObject.Object") + "'");
                Pins[3].PinSubCategoryMemberReference("()");
                Pins[3].PinValueType("()");
                Pins[3].ContainerType(EPinContainerType.None);
                Pins[3].bIsReference(false);
                Pins[3].bIsConst(true);
                Pins[3].bIsWeakPointer(false);
                Pins[3].bIsUObjectWrapper(false);
                Pins[3].bSerializeAsSinglePrecisionFloat(false);
                Pins[3].PersistentGuid("00000000000000000000000000000000");
                Pins[3].bHidden(true);
                Pins[3].bNotConnectable(false);
                Pins[3].bDefaultValueIsReadOnly(false);
                Pins[3].bDefaultValueIsIgnored(false);
                Pins[3].bAdvancedView(false);
                Pins[3].bOrphanedPin(false);
                Pins[3].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[4].OpenGraphPinText();
                Pins[4].PinId(GenerateRandomHexNumber());
                Pins[4].PinName("InString");
                Pins[4].PinToolTip("In String\\nString\\n\\nThe string to log out");
                Pins[4].PinCategory("string");
                Pins[4].PinSubCategory("");
                Pins[4].PinSubCategoryObject("None");
                Pins[4].PinSubCategoryMemberReference("()");
                Pins[4].PinValueType("()");
                Pins[4].ContainerType(EPinContainerType.None);
                Pins[4].bIsReference(false);
                Pins[4].bIsConst(true);
                Pins[4].bIsWeakPointer(false);
                Pins[4].bIsUObjectWrapper(false);
                Pins[4].bSerializeAsSinglePrecisionFloat(false);
                Pins[4].DefaultValue("Called " + nodeBefore.CustomFunctionName);
                Pins[4].AutogeneratedDefaultValue("Hello");
                Pins[4].PersistentGuid("00000000000000000000000000000000");
                Pins[4].bHidden(false);
                Pins[4].bNotConnectable(false);
                Pins[4].bDefaultValueIsReadOnly(false);
                Pins[4].bDefaultValueIsIgnored(false);
                Pins[4].bAdvancedView(false);
                Pins[4].bOrphanedPin(false);
                Pins[4].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[5].OpenGraphPinText();
                Pins[5].PinId(GenerateRandomHexNumber());
                Pins[5].PinName("bPrintToScreen");
                Pins[5].PinToolTip("Print to Screen\\nBoolean\\n\\nWhether or not to print the output to the screen");
                Pins[5].PinCategory("bool");
                Pins[5].PinSubCategory("");
                Pins[5].PinSubCategoryObject("None");
                Pins[5].PinSubCategoryMemberReference("()");
                Pins[5].PinValueType("()");
                Pins[5].ContainerType(EPinContainerType.None);
                Pins[5].bIsReference(false);
                Pins[5].bIsConst(false);
                Pins[5].bIsWeakPointer(false);
                Pins[5].bIsUObjectWrapper(false);
                Pins[5].bSerializeAsSinglePrecisionFloat(false);
                Pins[5].DefaultValue("true");
                Pins[5].AutogeneratedDefaultValue("true");
                Pins[5].PersistentGuid("00000000000000000000000000000000");
                Pins[5].bHidden(false);
                Pins[5].bNotConnectable(false);
                Pins[5].bDefaultValueIsReadOnly(false);
                Pins[5].bDefaultValueIsIgnored(false);
                Pins[5].bAdvancedView(true);
                Pins[5].bOrphanedPin(false);
                Pins[5].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[6].OpenGraphPinText();
                Pins[6].PinId(GenerateRandomHexNumber());
                Pins[6].PinName("bPrintToLog");
                Pins[6].PinToolTip("Print to Log\\nBoolean\\n\\nWhether or not to print the output to the log");
                Pins[6].PinCategory("bool");
                Pins[6].PinSubCategory("");
                Pins[6].PinSubCategoryObject("None");
                Pins[6].PinSubCategoryMemberReference("()");
                Pins[6].PinValueType("()");
                Pins[6].ContainerType(EPinContainerType.None);
                Pins[6].bIsReference(false);
                Pins[6].bIsConst(false);
                Pins[6].bIsWeakPointer(false);
                Pins[6].bIsUObjectWrapper(false);
                Pins[6].bSerializeAsSinglePrecisionFloat(false);
                Pins[6].DefaultValue("true");
                Pins[6].AutogeneratedDefaultValue("true");
                Pins[6].PersistentGuid("00000000000000000000000000000000");
                Pins[6].bHidden(false);
                Pins[6].bNotConnectable(false);
                Pins[6].bDefaultValueIsReadOnly(false);
                Pins[6].bDefaultValueIsIgnored(false);
                Pins[6].bAdvancedView(true);
                Pins[6].bOrphanedPin(false);
                Pins[6].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[7].OpenGraphPinText();
                Pins[7].PinId(GenerateRandomHexNumber());
                Pins[7].PinName("TextColor");
                Pins[7].PinToolTip("Text Color\\nLinear Color Structure\\n\\nThe color of the text to display");
                Pins[7].PinCategory("struct");
                Pins[7].PinSubCategory("");
                Pins[7].PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'" + Wrap("/Script/CoreUObject.LinearColor") + "'");
                Pins[7].PinSubCategoryMemberReference("()");
                Pins[7].PinValueType("()");
                Pins[7].ContainerType(EPinContainerType.None);
                Pins[7].bIsReference(false);
                Pins[7].bIsConst(false);
                Pins[7].bIsWeakPointer(false);
                Pins[7].bIsUObjectWrapper(false);
                Pins[7].bSerializeAsSinglePrecisionFloat(false);
                Pins[7].DefaultValue("(R=1.000000,G=0.349312,B=0.097049,A=1.000000)");
                Pins[7].AutogeneratedDefaultValue("(R=0.000000,G=0.660000,B=1.000000,A=1.000000)");
                Pins[7].PersistentGuid("00000000000000000000000000000000");
                Pins[7].bHidden(false);
                Pins[7].bNotConnectable(false);
                Pins[7].bDefaultValueIsReadOnly(false);
                Pins[7].bDefaultValueIsIgnored(false);
                Pins[7].bAdvancedView(true);
                Pins[7].bOrphanedPin(false);
                Pins[7].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[8].OpenGraphPinText();
                Pins[8].PinId(GenerateRandomHexNumber());
                Pins[8].PinName("Duration");
                Pins[8].PinToolTip("Duration\\nFloat (single-precision)\\n\\nThe display duration (if Print to Screen is True). Using negative number will result in loading the duration time from the config.");
                Pins[8].PinCategory("real");
                Pins[8].PinSubCategory("float");
                Pins[8].PinSubCategoryObject("None");
                Pins[8].PinSubCategoryMemberReference("()");
                Pins[8].PinValueType("()");
                Pins[8].ContainerType(EPinContainerType.None);
                Pins[8].bIsReference(false);
                Pins[8].bIsConst(false);
                Pins[8].bIsWeakPointer(false);
                Pins[8].bIsUObjectWrapper(false);
                Pins[8].bSerializeAsSinglePrecisionFloat(false);
                Pins[8].DefaultValue("5.000000");
                Pins[8].AutogeneratedDefaultValue("2.000000");
                Pins[8].PersistentGuid("00000000000000000000000000000000");
                Pins[8].bHidden(false);
                Pins[8].bNotConnectable(false);
                Pins[8].bDefaultValueIsReadOnly(false);
                Pins[8].bDefaultValueIsIgnored(false);
                Pins[8].bAdvancedView(true);
                Pins[8].bOrphanedPin(false);
                Pins[8].CloseGraphPinText();
                AddPin(new GraphPin(this));
                Pins[9].OpenGraphPinText();
                Pins[9].PinId(GenerateRandomHexNumber());
                Pins[9].PinName("Key");
                Pins[9].PinToolTip("Key\\nName\\n\\nIf a non-empty key is provided, the message will replace any existing on-screen messages with the same key.");
                Pins[9].PinCategory("name");
                Pins[9].PinSubCategory("");
                Pins[9].PinSubCategoryObject("None");
                Pins[9].PinSubCategoryMemberReference("()");
                Pins[9].PinValueType("()");
                Pins[9].ContainerType(EPinContainerType.None);
                Pins[9].bIsReference(false);
                Pins[9].bIsConst(true);
                Pins[9].bIsWeakPointer(false);
                Pins[9].bIsUObjectWrapper(false);
                Pins[9].bSerializeAsSinglePrecisionFloat(false);
                Pins[9].DefaultValue("None");
                Pins[9].AutogeneratedDefaultValue("None");
                Pins[9].PersistentGuid("00000000000000000000000000000000");
                Pins[9].bHidden(false);
                Pins[9].bNotConnectable(false);
                Pins[9].bDefaultValueIsReadOnly(false);
                Pins[9].bDefaultValueIsIgnored(false);
                Pins[9].bAdvancedView(true);
                Pins[9].bOrphanedPin(false);
                Pins[9].CloseGraphPinText();

            }
            public override string GetBeginObjectClassString()
            {
                string myPins = PinsToString(Pins);
                string myUserDefinedPins = PinsToString(UserDefinedPins);
                return
                   "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                   "   " + "FunctionReference=" + FunctionReference + "\n" +
                   "   " + "NodePosX=" + NodePosX + "\n" +
                   "   " + "NodePosY=" + NodePosY + "\n" +
                   "   " + "AdvancedPinDisplay=Hidden" + "\n" +
                   "   " + "EnabledState = DevelopmentOnly" + "\n" +
                   "   " + "NodeGuid=" + NodeGuid + "\n" +
                   "   " + myPins + "\n" +
                   "End Object";
            }

        }
        class GraphNodeConstructJsonObject : GraphNodeKismetFunction
        {
            public GraphNodeConstructJsonObject()
            {
                Class = "/Script/BlueprintGraph.K2Node_CallFunction";
                Name = "K2Node_CallFunction_183";
                FunctionReference = "(MemberParent=/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"',MemberName=\"ConstructJsonObject\")";
                NodePosX = "608";
                NodePosY = "32";
                AddPin(new GraphPinSelfConstructedJsonObject(this));
                AddPin(new GraphPinWorldContextObject(this));
                AddPin(new GraphPinConstructedJsonObjectOut(this));
            }
            public override string GetBeginObjectClassString()
            {
                string myPins = PinsToString(Pins);
                return
                   "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                   "   " + "bIsPureFunc=True" + "\n" +
                   "   " + "FunctionReference=" + FunctionReference + "\n" +
                   "   " + "NodePosX=" + NodePosX + "\n" +
                   "   " + "NodePosY=" + NodePosY + "\n" +
                   "   " + "NodeGuid=" + NodeGuid + "\n" +
                   "   " + myPins + "\n" +
                   "End Object";
            }
        }
        class GraphNodeHandleError : GraphNodeKismetFunction
        {
            public GraphNodeHandleError(GraphNode nodeBefore)
            {
                Class = "/Script/BlueprintGraph.K2Node_CallFunction";
                Name = "K2Node_CallFunction_0";
                FunctionReference = "(MemberParent=/Script/Engine.BlueprintGeneratedClass'\"/Darkion_Function_Library/FLib_Darkion.FLib_Darkion_C\"',MemberName=\"Popup Message for Error\",MemberGuid=7D27D2264C7598635335A48743261F07)";
                NodePosX = (int.Parse(nodeBefore.NodePosX) + 464).ToString();
                NodePosY = (int.Parse(nodeBefore.NodePosY) + 16).ToString();
                AddPin(new GraphPinExec(this, nodeBefore.Pins[1]));
                AddPin(new GraphPinThen(this));
                AddPin(new GraphPinSelfFLibDarkionRef(this));
                AddPin(new GraphPinFailedStructIn(this, nodeBefore.Pins[2]));
                AddPin(new GraphPinBoolIn(this, "IsCriticalError", "", "Is Critical Error?\\nBoolean"));
                AddPin(new GraphPin__WorldContextObject(this));
            }
            public override string GetBeginObjectClassString()
            {
                string myPins = PinsToString(Pins);
                return
                   "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                   "   " + "FunctionReference=" + FunctionReference + "\n" +
                   "   " + "NodePosX=" + NodePosX + "\n" +
                   "   " + "NodePosY=" + NodePosY + "\n" +
                   "   " + "NodeGuid=" + NodeGuid + "\n" +
                   "   " + myPins + "\n" +
                   "End Object";
            }
        }
        class GraphNodeSetJsonVariable : GraphNodeKismetVariable
        {
            public GraphNodeSetJsonVariable(GraphNode previousNode, GraphNode constructNode, string csFunctionName)
            {
                Class = "/Script/BlueprintGraph.K2Node_VariableSet";
                Name = "K2Node_VariableSet_19";
                if (prefix == "")
                {
                    VariableReference = "(MemberName=\"" + csFunctionName + "Params" + "\",MemberGuid=" + GenerateRandomHexNumber() + ",bSelfContext=True)";
                }
                else
                {
                    VariableReference = "(MemberName=\"" + prefix + "_" + csFunctionName + "Params" + "\",MemberGuid=" + GenerateRandomHexNumber() + ",bSelfContext=True)";
                }
                NodePosX = "528";
                NodePosY = "112";
                AddPin(new GraphPinExec(this, previousNode.Pins[1]));
                AddPin(new GraphPinThen(this));
                if (prefix == "")
                {
                    AddPin(new GraphPinJsonIn(this, constructNode.Pins[2], csFunctionName + "Params", "", "", false));
                }
                else
                {
                    AddPin(new GraphPinJsonIn(this, constructNode.Pins[2], prefix + "_" + csFunctionName + "Params", "", "", false));
                }
                AddPin(new GraphPinJsonOut(this));
                AddPin(new GraphPinSelfGameInstanceBPRef(this));
            }
            public override string GetBeginObjectClassString()
            {
                if (Pins.Length > 0)
                {
                    string myPins = PinsToString(Pins);
                    return
                       "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                       "   " + "VariableReference=" + VariableReference + "\n" +
                       "   " + "NodePosX=" + NodePosX + "\n" +
                       "   " + "NodePosY=" + NodePosY + "\n" +
                       "   " + "NodeGuid=" + NodeGuid + "\n" +
                       "   " + myPins + "\n" +
                       "End Object";
                }
                return
                    "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                    "   " + "VariableReference=" + VariableReference + "\n" +
                    "   " + "NodePosX=" + NodePosX + "\n" +
                    "   " + "NodePosY=" + NodePosY + "\n" +
                    "   " + "NodeGuid=" + NodeGuid + "\n" +
                    "End Object";
            }
        }
        class GraphNodeSetStringFieldInJson : GraphNodeKismetFunction
        {
            public GraphNodeSetStringFieldInJson(GraphPin linkToExec, GraphPin linkToJsonObjectInput, GraphPin linkToStringValueInput, string stringFieldName, int indexForSpaceBetweenNodes)
            {
                Class = "/Script/BlueprintGraph.K2Node_CallFunction";
                Name = "K2Node_CallFunction_19" + indexForSpaceBetweenNodes;
                FunctionReference = "(MemberParent=/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"',MemberName=\"SetStringField\")";
                NodePosX = "1008";
                NodePosY = (80 + 192 * indexForSpaceBetweenNodes).ToString();
                AddPin(new GraphPinExec(this, linkToExec));
                AddPin(new GraphPinThen(this));
                AddPin(new GraphPinJsonIn(this, "self", "NSLOCTEXT(\"K2Node\", \"Target\", \"Target\")", "Target\\nPlay Fab Json Object Object Reference", false));
                AddPin(new GraphPinStringFieldName(this, stringFieldName));
                AddPin(new GraphPinStringIn(this, "String Value\\nString", linkToStringValueInput));
            }
            public GraphNodeSetStringFieldInJson(GraphPin linkToExec, GraphPin linkToStringValueInput, string stringFieldName, int indexForSpaceBetweenNodes)
            {
                Class = "/Script/BlueprintGraph.K2Node_CallFunction";
                Name = "K2Node_CallFunction_19" + indexForSpaceBetweenNodes;
                FunctionReference = "(MemberParent=/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"',MemberName=\"SetStringField\")";
                NodePosX = "1008";
                NodePosY = (80 + 192 * indexForSpaceBetweenNodes).ToString();
                AddPin(new GraphPinExec(this, linkToExec));
                AddPin(new GraphPinThen(this));
                AddPin(new GraphPinJsonIn(this, "self", "NSLOCTEXT(\"K2Node\", \"Target\", \"Target\")", "Target\\nPlay Fab Json Object Object Reference", false));
                AddPin(new GraphPinStringFieldName(this, stringFieldName));
                AddPin(new GraphPinStringIn(this, "String Value\\nString", linkToStringValueInput));
            }
            public override string GetBeginObjectClassString()
            {
                string myPins = PinsToString(Pins);
                return
                   "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                   "   " + "FunctionReference=" + FunctionReference + "\n" +
                   "   " + "NodePosX=" + NodePosX + "\n" +
                   "   " + "NodePosY=" + NodePosY + "\n" +
                   "   " + "NodeGuid=" + NodeGuid + "\n" +
                   "   " + myPins + "\n" +
                   "End Object";
            }
        }
        class GraphNodeExecuteCloudScript : GraphNodeKismetFunction
        {
            string ProxyFactoryFunctionName;
            string ProxyFactoryClass;
            string ProxyClass;
            public GraphNodeExecuteCloudScript(GraphPin linkToExec, GraphPin linkToRequestStruct)
            {
                Class = "/Script/BlueprintGraph.K2Node_AsyncAction";
                Name = "K2Node_AsyncAction_18";
                ProxyFactoryFunctionName = "ExecuteCloudScript";
                ProxyFactoryClass = "/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabClientAPI\"'";
                ProxyClass = "/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabClientAPI\"'";
                NodePosX = (int.Parse(linkToExec._ParentNodeRef.NodePosX) + 550).ToString();
                NodePosY = (int.Parse(linkToExec._ParentNodeRef.NodePosY) + 16).ToString();
                AddPin(new GraphPinExec(this, linkToExec));
                AddPin(new GraphPinThen(this));
                AddPin(new GraphPinOnPlayFabResponse(this));
                AddPin(new GraphPinResponseStruct(this));
                AddPin(new GraphPinCustomDataOut(this));
                AddPin(new GraphPinWasSuccessfulOut(this));
                AddPin(new GraphPinRequestStructIn(this, linkToRequestStruct));
                AddPin(new GraphPinDelegateOnSuccess(this));
                AddPin(new GraphPinDelegateOnFailure(this));
                AddPin(new GraphPinCustomDataIn(this));
            }
            public override string GetBeginObjectClassString()
            {
                string myPins = PinsToString(Pins);
                return
                   "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                   "   " + "ProxyFactoryFunctionName=" + Wrap(ProxyFactoryFunctionName) + "\n" +
                   "   " + "ProxyFactoryClass=" + ProxyFactoryClass + "\n" +
                   "   " + "ProxyClass=" + ProxyClass + "\n" +
                   "   " + "NodePosX=" + NodePosX + "\n" +
                   "   " + "NodePosY=" + NodePosY + "\n" +
                   "   " + "NodeGuid=" + NodeGuid + "\n" +
                   "   " + myPins + "\n" +
                   "End Object";
            }
        }
        class GraphNodeMakeRequestStruct : GraphNode
        {
            bool bMadeAfterOverridePinRemoval;
            public GraphNodeMakeRequestStruct(GraphPin inputFuncParametersJsonObj, string functionName, int indexForSpaceBetweenNodes)
            {
                Class = "/Script/BlueprintGraph.K2Node_MakeStruct";
                Name = "K2Node_MakeStruct_20";
                bMadeAfterOverridePinRemoval = true;
                NodePosX = "928";
                NodePosY = (80 + 192 * indexForSpaceBetweenNodes).ToString();
                AddPin(new GraphPinRequestStructOut(this));
                AddPin(new GraphPinCustomTagsIn(this));
                AddPin(new GraphPinStringIn(this, functionName));
                AddPin(new GraphPinJsonIn(this, "FunctionParameter", Wrap("Function Parameter"), "Function Parameter\\nPlay Fab Json Object Object Reference\\n\\nFunction Parameter:\\r\\nObject that is passed in to the function as the first argument", true));
                AddPin(new GraphPinBoolIn(this, "GeneratePlayStreamEvent", Wrap("Generate Play Stream Event"), "Generate Play Stream Event\\nBoolean\\n\\nGenerate Play Stream Event:\\r\\nGenerate a \\'player_executed_cloudscript\\' PlayStream event containing the results of the function execution and other\\ncontextual information. This event will show up in the PlayStream debugger console for the player in Game Manager."));
                AddPin(new GraphPinByteIn(this, "RevisionSelection", Wrap("Revision Selection"), "Revision Selection\\nECloudScriptRevisionOption Enum\\n\\nRevision Selection:\\r\\nOption for which revision of the CloudScript to execute. \\'Latest\\' executes the most recently created revision,\\'Live\\'\\nexecutes the current live,published revision,and \\'Specific\\' executes the specified revision. The default value is\\n\\'Specific\\',if the SpeificRevision parameter is specified,otherwise it is \\'Live\\'.", "/Script/CoreUObject.Enum'\"/Script/PlayFab.ECloudScriptRevisionOption\"'", "pfenum_Live", "pfenum_Live"));
                AddPin(new GraphPinIntIn(this, "SpecificRevision", Wrap("Specific Revision"), "Specific Revision\\nInteger\\n\\nSpecific Revision:\\r\\nThe specivic revision to execute,when RevisionSelection is set to \\'Specific\\'", "0", "0"));
                AddPin(new GraphPinAuthContextIn(this, true));
            }
            public GraphNodeMakeRequestStruct(string functionName, int indexForSpaceBetweenNodes)
            {
                Class = "/Script/BlueprintGraph.K2Node_MakeStruct";
                Name = "K2Node_MakeStruct_20";
                bMadeAfterOverridePinRemoval = true;
                NodePosX = "928";
                NodePosY = (80 + 192 * indexForSpaceBetweenNodes).ToString();
                AddPin(new GraphPinRequestStructOut(this));
                AddPin(new GraphPinCustomTagsIn(this));
                AddPin(new GraphPinStringIn(this, functionName));
                AddPin(new GraphPinJsonIn(this, "FunctionParameter", Wrap("Function Parameter"), "Function Parameter\\nPlay Fab Json Object Object Reference\\n\\nFunction Parameter:\\r\\nObject that is passed in to the function as the first argument", true));
                AddPin(new GraphPinBoolIn(this, "GeneratePlayStreamEvent", Wrap("Generate Play Stream Event"), "Generate Play Stream Event\\nBoolean\\n\\nGenerate Play Stream Event:\\r\\nGenerate a \\'player_executed_cloudscript\\' PlayStream event containing the results of the function execution and other\\ncontextual information. This event will show up in the PlayStream debugger console for the player in Game Manager."));
                AddPin(new GraphPinByteIn(this, "RevisionSelection", Wrap("Revision Selection"), "Revision Selection\\nECloudScriptRevisionOption Enum\\n\\nRevision Selection:\\r\\nOption for which revision of the CloudScript to execute. \\'Latest\\' executes the most recently created revision,\\'Live\\'\\nexecutes the current live,published revision,and \\'Specific\\' executes the specified revision. The default value is\\n\\'Specific\\',if the SpeificRevision parameter is specified,otherwise it is \\'Live\\'.", "/Script/CoreUObject.Enum'\"/Script/PlayFab.ECloudScriptRevisionOption\"'", "pfenum_Live", "pfenum_Live"));
                AddPin(new GraphPinIntIn(this, "SpecificRevision", Wrap("Specific Revision"), "Specific Revision\\nInteger\\n\\nSpecific Revision:\\r\\nThe specivic revision to execute,when RevisionSelection is set to \\'Specific\\'", "0", "0"));
                AddPin(new GraphPinAuthContextIn(this, true));
            }
            public override string GetBeginObjectClassString()
            {
                string myPins = PinsToString(Pins);
                return
                   "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                   "   " + "bMadeAfterOverridePinRemoval=" + bMadeAfterOverridePinRemoval + "\n" +
                   "   " + "ShowPinForProperties(0)=(PropertyName=\"CustomTags\",PropertyFriendlyName=\"Custom Tags\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"),\"0\",\"Custom Tags\",\"1\",\"The optional custom tags associated with the request (e.g. build number,external trace identifiers,etc.).\",\"Delimiter\",\":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(1)=(PropertyName=\"FunctionName\",PropertyFriendlyName=\"Function Name\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"),\"0\",\"Function Name\",\"1\",\"The name of the CloudScript function to execute\",\"Delimiter\",\":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(2)=(PropertyName=\"FunctionParameter\",PropertyFriendlyName=\"Function Parameter\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"),\"0\",\"Function Parameter\",\"1\",\"Object that is passed in to the function as the first argument\",\"Delimiter\",\":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(3)=(PropertyName=\"GeneratePlayStreamEvent\",PropertyFriendlyName=\"Generate Play Stream Event\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"),\"0\",\"Generate Play Stream Event\",\"1\",\"Generate a \\'player_executed_cloudscript\\' PlayStream event containing the results of the function execution and other\\ncontextual information. This event will show up in the PlayStream debugger console for the player in Game Manager.\",\"Delimiter\",\":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(4)=(PropertyName=\"RevisionSelection\",PropertyFriendlyName=\"Revision Selection\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"),\"0\",\"Revision Selection\",\"1\",\"Option for which revision of the CloudScript to execute. \\'Latest\\' executes the most recently created revision,\\'Live\\'\\nexecutes the current live,published revision,and \\'Specific\\' executes the specified revision. The default value is\\n\\'Specific\\',if the SpeificRevision parameter is specified,otherwise it is \\'Live\\'.\",\"Delimiter\",\":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(5)=(PropertyName=\"SpecificRevision\",PropertyFriendlyName=\"Specific Revision\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"),\"0\",\"Specific Revision\",\"1\",\"The specivic revision to execute,when RevisionSelection is set to \\'Specific\\'\",\"Delimiter\",\":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(6)=(PropertyName=\"AuthenticationContext\",PropertyFriendlyName=\"Authentication Context\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"),\"0\",\"Authentication Context\",\"1\",\"An optional authentication context (can used in multi-user scenarios)\",\"Delimiter\",\":\\r\\n\"),CategoryName=\"PlayFab | Core\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "StructType=/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.ClientExecuteCloudScriptRequest\"'" + "\n" +
                   "   " + "NodePosX=" + NodePosX + "\n" +
                   "   " + "NodePosY=" + NodePosY + "\n" +
                   "   " + "AdvancedPinDisplay=Shown" + "\n" +
                   "   " + "NodeGuid=" + NodeGuid + "\n" +
                   "   " + myPins + "\n" +
                   "End Object";
            }
        }
        class GraphNodeBreakResultStruct : GraphNode
        {
            bool bMadeAfterOverridePinRemoval;
            public GraphNodeBreakResultStruct(GraphNode nodeBefore)
            {
                Class = "/Script/BlueprintGraph.K2Node_BreakStruct";
                Name = "K2Node_BreakStruct_11";
                bMadeAfterOverridePinRemoval = true;
                NodePosX = (int.Parse(nodeBefore.NodePosX) + 448).ToString();
                NodePosY = (int.Parse(nodeBefore.NodePosY) + 80).ToString();
                AddPin(new GraphPinResultStructIn(this, nodeBefore.Pins[2], "ClientExecuteCloudScriptResult"));
                AddPin(new GraphPinJsonOut(this, "Error", "Error", @"Error\nPlay Fab Json Object Object Reference\n\nError:\r\nInformation about the error, if any, that occurred during execution"));
                AddPin(new GraphPinJsonOut(this, "FunctionResult", "Function Result", @"Function Result\nPlay Fab Json Object Object Reference\n\nFunction Result:\r\nThe object returned from the CloudScript function, if any", true));
            }
            public override string GetBeginObjectClassString()
            {
                string myPins = PinsToString(Pins);
                return
                   "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                   "   " + "bMadeAfterOverridePinRemoval=" + bMadeAfterOverridePinRemoval + "\n" +
                   "   " + "ShowPinForProperties(0)=(PropertyName=\"APIRequestsIssued\",PropertyFriendlyName=\"APIRequests Issued\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"APIRequests Issued\", \"1\", \"Number of PlayFab API requests issued by the CloudScript function\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(1)=(PropertyName=\"Error\",PropertyFriendlyName=\"Error\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Error\", \"1\", \"Information about the error, if any, that occurred during execution\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(2)=(PropertyName=\"ExecutionTimeSeconds\",PropertyFriendlyName=\"Execution Time Seconds\",PropertyTooltip=\"Execution Time Seconds\",CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(3)=(PropertyName=\"FunctionName\",PropertyFriendlyName=\"Function Name\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Function Name\", \"1\", \"The name of the function that executed\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(4)=(PropertyName=\"FunctionResult\",PropertyFriendlyName=\"Function Result\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Function Result\", \"1\", \"The object returned from the CloudScript function, if any\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=True,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(5)=(PropertyName=\"FunctionResultTooLarge\",PropertyFriendlyName=\"Function Result Too Large\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Function Result Too Large\", \"1\", \"Flag indicating if the FunctionResult was too large and was subsequently dropped from this event. This only occurs if\\nthe total event size is larger than 350KB.\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(6)=(PropertyName=\"HttpRequestsIssued\",PropertyFriendlyName=\"Http Requests Issued\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Http Requests Issued\", \"1\", \"Number of external HTTP requests issued by the CloudScript function\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(7)=(PropertyName=\"Logs\",PropertyFriendlyName=\"Logs\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Logs\", \"1\", \"Entries logged during the function execution. These include both entries logged in the function code using log.info()\\nand log.error() and error entries for API and HTTP request failures.\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(8)=(PropertyName=\"LogsTooLarge\",PropertyFriendlyName=\"Logs Too Large\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Logs Too Large\", \"1\", \"Flag indicating if the logs were too large and were subsequently dropped from this event. This only occurs if the total\\nevent size is larger than 350KB after the FunctionResult was removed.\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(9)=(PropertyName=\"MemoryConsumedBytes\",PropertyFriendlyName=\"Memory Consumed Bytes\",PropertyTooltip=\"Memory Consumed Bytes\",CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(10)=(PropertyName=\"ProcessorTimeSeconds\",PropertyFriendlyName=\"Processor Time Seconds\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Processor Time Seconds\", \"1\", \"Processor time consumed while executing the function. This does not include time spent waiting on API calls or HTTP\\nrequests.\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(11)=(PropertyName=\"Revision\",PropertyFriendlyName=\"Revision\",PropertyTooltip=LOCGEN_FORMAT_NAMED(INVTEXT(\"{0}{Delimiter}{1}\"), \"0\", \"Revision\", \"1\", \"The revision of the CloudScript that executed\", \"Delimiter\", \":\\r\\n\"),CategoryName=\"PlayFab | Client | Server-Side Cloud Script Models\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "ShowPinForProperties(12)=(PropertyName=\"Request\",PropertyFriendlyName=\"Request\",PropertyTooltip=\"Request\",CategoryName=\"PlayFab | Core\",bShowPin=False,bCanToggleVisibility=True)" + "\n" +
                   "   " + "StructType=/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.ClientExecuteCloudScriptResult\"'" + "\n" +
                   "   " + "NodePosX=" + NodePosX + "\n" +
                   "   " + "NodePosY=" + NodePosY + "\n" +
                   "   " + "AdvancedPinDisplay=Shown" + "\n" +
                   "   " + "NodeGuid=" + NodeGuid + "\n" +
                   "   " + myPins + "\n" +
                   "End Object";
            }
        }
        class GraphPin
        {

            //example values in comments
            public GraphNode _ParentNodeRef;
            public GraphPin(GraphNode parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
            }

            public string _PinId = GenerateRandomHexNumber();
            public string _PinName = "";
            public string _PinFriendlyName = "";
            public string _PinToolTip = "";
            public EEdGraphPinDirection _Direction = EEdGraphPinDirection.EGPD_Output;
            public EEdGraphPinDirection _DesiredPinDirection = EEdGraphPinDirection.EGPD_Output;
            public string _PinCategory = "";
            public string _PinTypeEqualsPinCategory = "";
            public string _PinSubCategory = "";
            public string _PinSubCategoryObject = " ";
            public string _PinSubCategoryMemberReference = "()";
            public string _MemberName = "()";
            public string _MemberGuid = "()";
            public string _PinValueType = "()";
            public EPinContainerType _ContainerType = EPinContainerType.None;
            public bool _bIsReference = false;
            public bool _bIsConst = false;
            public bool _bIsWeakPointer = false;
            public bool _bIsUObjectWrapper = false;
            public bool _bSerializeAsSinglePrecisionFloat = false;
            public string _DefaultValue = "";
            public string _AutogeneratedDefaultValue = "";
            public string _LinkedTo = "()";
            public string _DefaultObject = "None";
            public string _PersistentGuid = "00000000000000000000000000000000";
            public bool _bHidden = false;
            public bool _bNotConnectable = false;
            public bool _bDefaultValueIsReadOnly = false;
            public bool _bDefaultValueIsIgnored = false;
            public bool _bAdvancedView = false;
            public bool _bOrphanedPin = false;

            public string GraphPinText = "";



            #region <Functions for string GraphPinText>

            public void OpenGraphPinText()
            {
                GraphPinText = "CustomProperties Pin (";
            }

            public void OpenUserDefinedGraphPinText()
            {
                GraphPinText = "CustomProperties UserDefinedPin (";
            }

            public void CloseGraphPinText()
            {
                GraphPinText = GraphPinText + ")";
            }

            public void UnCloseGraphPinText()
            {
                GraphPinText = GraphPinText.Remove(GraphPinText.Length - 1, 1);
            }

            public void PinId(string pinId)
            {
                this._PinId = pinId;
                GraphPinText = GraphPinText + "PinId=" + pinId + ",";
            }

            public void PinName(string pinName)
            {
                GraphPinText = GraphPinText + "PinName=" + Wrap(pinName) + ",";
            }

            public void PinFriendlyName(string pinFriendlyName)
            {
                this._PinFriendlyName = pinFriendlyName;
                GraphPinText = GraphPinText + "PinFriendlyName=" + pinFriendlyName + ",";
            }

            public void PinToolTip(string pinToolTip)
            {
                this._PinToolTip = pinToolTip;
                GraphPinText = GraphPinText + "PinToolTip=" + Wrap(pinToolTip) + ",";
            }

            public void Direction(EEdGraphPinDirection direction)
            {
                this._Direction = direction;
                GraphPinText = GraphPinText + "Direction=" + Wrap(direction.ToString()) + ",";
            }

            public void PinCategory(string pinCategory)
            {
                this._PinCategory = pinCategory;
                GraphPinText = GraphPinText + "PinType.PinCategory=" + Wrap(pinCategory) + ",";
            }
            public virtual void PinTypeEqualsPinCategory(string pinTypeEqualsPinCategory)
            {
                this._PinTypeEqualsPinCategory = pinTypeEqualsPinCategory;
                GraphPinText = GraphPinText + "PinType=(PinCategory=" + Wrap(pinTypeEqualsPinCategory) + ",";
            }
            public void DesiredPinDirection(EEdGraphPinDirection desiredPinDirection)
            {
                this._DesiredPinDirection = desiredPinDirection;
                GraphPinText = GraphPinText + "DesiredPinDirection=" + desiredPinDirection.ToString() + ")";
            }

            public virtual void PinSubCategory(string pinSubCategory)
            {
                this._PinSubCategory = pinSubCategory;
                GraphPinText = GraphPinText + "PinType.PinSubCategory=" + Wrap(pinSubCategory) + ",";
            }

            public virtual void PinSubCategoryObject(string pinSubCategoryObject)
            {
                this._PinSubCategoryObject = pinSubCategoryObject;
                GraphPinText = GraphPinText + "PinType.PinSubCategoryObject=" + pinSubCategoryObject + ",";
            }

            public void PinSubCategoryMemberReference(string pinSubCategoryMemberReference)
            {
                this._PinSubCategoryMemberReference = pinSubCategoryMemberReference;
                GraphPinText = GraphPinText + "PinType.PinSubCategoryMemberReference=" + pinSubCategoryMemberReference + ",";
            }
            public virtual void MemberName(string memberName)
            {
                this._MemberName = memberName;
                GraphPinText = GraphPinText + "MemberName=" + memberName + ",";
            }
            public void MemberGuid(string memberGuid)
            {
                this._MemberGuid = memberGuid;
                GraphPinText = GraphPinText + "MemberGuid=" + memberGuid + ",";
            }
            public void PinValueType(string pinValueType)
            {
                this._PinValueType = pinValueType;
                GraphPinText = GraphPinText + "PinType.PinValueType=" + pinValueType + ",";
            }

            public void ContainerType(EPinContainerType containerType)
            {
                this._ContainerType = containerType;
                GraphPinText = GraphPinText + "PinType.ContainerType=" + containerType.ToString() + ",";
            }

            public void bIsReference(bool bIsReference)
            {
                this._bIsReference = bIsReference;
                GraphPinText = GraphPinText + "PinType.bIsReference=" + bIsReference + ",";
            }

            public void bIsConst(bool bIsConst)
            {
                this._bIsConst = bIsConst;
                GraphPinText = GraphPinText + "PinType.bIsConst=" + bIsConst + ",";
            }

            public void bIsWeakPointer(bool bIsWeakPointer)
            {
                this._bIsWeakPointer = bIsWeakPointer;
                GraphPinText = GraphPinText + "PinType.bIsWeakPointer=" + bIsWeakPointer + ",";
            }

            public void bIsUObjectWrapper(bool bIsUObjectWrapper)
            {
                this._bIsUObjectWrapper = bIsUObjectWrapper;
                GraphPinText = GraphPinText + "PinType.bIsUObjectWrapper=" + bIsUObjectWrapper + ",";
            }

            public void bSerializeAsSinglePrecisionFloat(bool bSerializeAsSinglePrecisionFloat)
            {
                this._bSerializeAsSinglePrecisionFloat = bSerializeAsSinglePrecisionFloat;
                GraphPinText = GraphPinText + "PinType.bSerializeAsSinglePrecisionFloat=" + bSerializeAsSinglePrecisionFloat + ",";
            }

            public void DefaultValue(string DefaultValue)
            {
                this._DefaultValue = DefaultValue;
                GraphPinText = GraphPinText + "DefaultValue=" + Wrap(DefaultValue) + ",";
            }
            public void AutogeneratedDefaultValue(string AutogeneratedDefaultValue)
            {
                this._AutogeneratedDefaultValue = AutogeneratedDefaultValue;
                GraphPinText = GraphPinText + "AutogeneratedDefaultValue=" + Wrap(AutogeneratedDefaultValue) + ",";
            }

            public void LinkedTo(string linkedTo)
            {
                this._LinkedTo = linkedTo;
                GraphPinText = GraphPinText + "LinkedTo=" + linkedTo + ",";
            }
            public void DefaultObject(string defaultObject)
            {
                this._DefaultObject = defaultObject;
                GraphPinText = GraphPinText + "DefaultObject=" + defaultObject + ",";
            }
            public void PersistentGuid(string persistentGuid)
            {
                this._PersistentGuid = persistentGuid;
                GraphPinText = GraphPinText + "PersistentGuid=" + persistentGuid + ",";
            }

            public void bHidden(bool bHidden)
            {
                this._bHidden = bHidden;
                GraphPinText = GraphPinText + "bHidden=" + bHidden + ",";
            }

            public void bNotConnectable(bool bNotConnectable)
            {
                this._bNotConnectable = bNotConnectable;
                GraphPinText = GraphPinText + "bNotConnectable=" + bNotConnectable + ",";
            }

            public void bDefaultValueIsReadOnly(bool bDefaultValueIsReadOnly)
            {
                this._bDefaultValueIsReadOnly = bDefaultValueIsReadOnly;
                GraphPinText = GraphPinText + "bDefaultValueIsReadOnly=" + bDefaultValueIsReadOnly + ",";
            }

            public void bDefaultValueIsIgnored(bool bDefaultValueIsIgnored)
            {
                this._bDefaultValueIsIgnored = bDefaultValueIsIgnored;
                GraphPinText = GraphPinText + "bDefaultValueIsIgnored=" + bDefaultValueIsIgnored + ",";
            }

            public void bAdvancedView(bool bAdvancedView)
            {
                this._bAdvancedView = bAdvancedView;
                GraphPinText = GraphPinText + "bAdvancedView=" + bAdvancedView + ",";
            }

            public void bOrphanedPin(bool bOrphanedPin)
            {
                this._bOrphanedPin = bOrphanedPin;
                GraphPinText = GraphPinText + "bOrphanedPin=" + bOrphanedPin + ",";
            }

            #endregion

            public string GetCustomPropertiesString()
            {
                return GraphPinText;
            }


            public void LinkToPin(GraphPin connectToPin)
            {

                LinkedTo("(" + connectToPin._ParentNodeRef.Name + " " + connectToPin._PinId + ",)");
                _LinkedTo = "(" + connectToPin._ParentNodeRef.Name + " " + connectToPin._PinId + ",)";

                //connectToPin.UnCloseGraphPinText();
                //connectToPin.LinkedTo("(" + _ParentNodeRef.Name + " " + _PinId + ")");
                //connectToPin._LinkedTo = "(" + _ParentNodeRef.Name + " " + _PinId + ")";
                //connectToPin.CloseGraphPinText();

            }
            public void LinkToPins(List<GraphPin> connectToPins)
            {
                UnCloseGraphPinText();
                string allPinsStringified = "(";
                for(int i = 0; i < connectToPins.ToArray().Length; i++)
                {
                    allPinsStringified += connectToPins[i]._ParentNodeRef.Name + " " + connectToPins[i]._PinId + ",";
                    connectToPins[i].UnCloseGraphPinText();
                    connectToPins[i].LinkedTo("(" + _ParentNodeRef.Name + " " + _PinId + ")");
                    connectToPins[i]._LinkedTo = "(" + _ParentNodeRef.Name + " " + _PinId + ")";
                    connectToPins[i].CloseGraphPinText();
                }
                allPinsStringified += ")";
                LinkedTo(allPinsStringified);
                _LinkedTo = allPinsStringified;
            }


        }
        class GraphUserDefinedPin : GraphPin
        {
            public GraphUserDefinedPin(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
            }
            public override void PinSubCategoryObject(string pinSubCategoryObject)
            {
                this._PinSubCategoryObject = pinSubCategoryObject;
                GraphPinText = GraphPinText + "PinSubCategoryObject=" + pinSubCategoryObject + ",";
            }
        }
        class GraphPinExec : GraphPin
        {
            public GraphPinExec(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("execute");
                PinToolTip("\\nExec");
                PinCategory("exec");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinExec(GraphNode parentNodeRef, GraphPin linkToPin) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("execute");
                PinCategory("exec");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                LinkToPin(linkToPin);
                CloseGraphPinText();
            }
            public GraphPinExec(GraphNode parentNodeRef, GraphPin linkToPin, string pinName) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                PinCategory("exec");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                LinkToPin(linkToPin);
                CloseGraphPinText();
            }
        }
        class GraphPinDelegateHandle : GraphPin
        {
            public override void MemberName(string memberName)
            {
                this._MemberName = memberName;
                GraphPinText = GraphPinText + "MemberName=" + Wrap(memberName) + ",";
            }
            public GraphPinDelegateHandle(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("OutputDelegate");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("delegate");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("(MemberParent=/Script/Engine.BlueprintGeneratedClass'\"/Game/_D/BPs/GameInstanceBP.GameInstanceBP_C\"'");
                MemberName(parentNodeRef.CustomFunctionName);
                MemberGuid(GenerateRandomHexNumber());
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinDelegateHandle(GraphNode parentNodeRef, GraphPin pinToLink) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("OutputDelegate");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("delegate");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("(MemberParent=/Script/Engine.BlueprintGeneratedClass'\"/Game/_D/BPs/GameInstanceBP.GameInstanceBP_C\"'");
                MemberName(parentNodeRef.CustomFunctionName);
                MemberGuid(GenerateRandomHexNumber());
                CloseGraphPinText();//u need dis
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                LinkToPin(pinToLink);
                CloseGraphPinText();
            }
        }
        class GraphPinThen : GraphPin
        {
            public GraphPinThen(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("then");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("exec");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinThen(GraphNode parentNodeRef, string pinName) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("exec");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinOnPlayFabResponse : GraphPin
        {
            public GraphPinOnPlayFabResponse(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("OnPlayFabResponse");
                PinFriendlyName("On Play Fab Response");
                PinToolTip("On Play Fab Response");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("exec");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinResponseStruct : GraphPin
        {
            public GraphPinResponseStruct(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("response");
                PinToolTip("Response\\nPlay Fab Base Model Structure");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("struct");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.PlayFabBaseModel\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinCustomDataOut : GraphPin
        {
            public GraphPinCustomDataOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("customData");
                PinToolTip("Custom Data\\nObject Reference");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/CoreUObject.Object\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinCustomDataIn : GraphPin
        {
            public GraphPinCustomDataIn(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("customData");
                PinToolTip("Custom Data\\nObject Reference");
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/CoreUObject.Object\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinCustomTagsIn : GraphPin
        {
            public GraphPinCustomTagsIn(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("CustomTags");
                PinFriendlyName("Custom Tags");
                PinToolTip("Custom Tags\\nPlay Fab Json Object Object Reference\\n\\nCustom Tags:\\r\\nThe optional custom tags associated with the request (e.g. build number,external trace identifiers,etc.).");
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinWasSuccessfulOut : GraphPin
        {
            public GraphPinWasSuccessfulOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("successful");
                PinToolTip("Successful\\nBoolean");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("bool");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinRequestStructIn : GraphPin
        {
            public GraphPinRequestStructIn(GraphNode parentNodeRef, GraphPin inputRequestStruct) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("request");
                PinToolTip("Request\\nClient Execute Cloud Script Request Structure");
                PinCategory("struct");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.ClientExecuteCloudScriptRequest\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                LinkToPin(inputRequestStruct);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinRequestStructOut : GraphPin
        {
            public GraphPinRequestStructOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("ClientExecuteCloudScriptRequest");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("struct");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.ClientExecuteCloudScriptRequest\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinResultStructIn : GraphPin
        {
            public GraphPinResultStructIn(GraphNode parentNodeRef, GraphPin inputResultStruct, string pinName) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                PinCategory("struct");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.ClientExecuteCloudScriptResult\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(true);
                bIsConst(true);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                LinkToPin(inputResultStruct);
                CloseGraphPinText();
            }
        }
        class GraphPinResultStructOut : GraphPin
        {
            public GraphPinResultStructOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("result");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("struct");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.ClientExecuteCloudScriptResult\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinFailedStructIn : GraphPin
        {
            public GraphPinFailedStructIn(GraphNode parentNodeRef, GraphPin linkToPin) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("Error");
                PinToolTip("Error\\nPlayFab Error Structure");
                PinCategory("struct");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.PlayFabError\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                LinkToPin(linkToPin);
                CloseGraphPinText();
            }
        }
        class GraphPinFailedStructOut : GraphPin
        {
            public GraphPinFailedStructOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("error");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("struct");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.PlayFabError\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinUserDefinedSuccessStructOut : GraphUserDefinedPin
        {
            public GraphPinUserDefinedSuccessStructOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenUserDefinedGraphPinText();
                PinName("result");
                PinTypeEqualsPinCategory("struct");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.ClientExecuteCloudScriptResult\"')");
                DesiredPinDirection(EEdGraphPinDirection.EGPD_Output);
            }
        }
        class GraphPinUserDefinedFailedStructOut : GraphUserDefinedPin
        {
            public GraphPinUserDefinedFailedStructOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenUserDefinedGraphPinText();
                PinName("error");
                PinTypeEqualsPinCategory("struct");
                PinSubCategoryObject("/Script/CoreUObject.ScriptStruct'\"/Script/PlayFab.PlayFabError\"')");
                DesiredPinDirection(EEdGraphPinDirection.EGPD_Output);
            }
        }
        class GraphPinUserDefinedCustomDataOut : GraphUserDefinedPin
        {
            public GraphPinUserDefinedCustomDataOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenUserDefinedGraphPinText();
                PinName("customData");
                PinTypeEqualsPinCategory("object");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/CoreUObject.Object\"')");
                DesiredPinDirection(EEdGraphPinDirection.EGPD_Output);
            }
        }
        class GraphPinDelegateOnSuccess : GraphPin
        {
            public GraphPinDelegateOnSuccess(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("onSuccess");
                PinToolTip("On Success\\nDelegate");
                PinCategory("delegate");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("(MemberParent=/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabClientAPI\"',MemberName=\"DelegateOnSuccessExecuteCloudScript__DelegateSignature\")");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinDelegateOnFailure : GraphPin
        {
            public GraphPinDelegateOnFailure(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("onFailure");
                PinToolTip("On Failure\\nDelegate");
                PinCategory("delegate");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("(MemberParent=/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabClientAPI\"',MemberName=\"DelegateOnFailurePlayFabError__DelegateSignature\")");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinJsonOut : GraphPin
        {
            public GraphPinJsonOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("Output_Get");
                PinToolTip("Retrieves the value of the variable, can use instead of a separate Get node");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinJsonOut(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                if(pinFriendlyName != "")
                {
                    PinFriendlyName(Wrap(pinFriendlyName));
                }
                PinToolTip(pinToolTip);
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinJsonOut(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip, bool advancedView) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                if (pinFriendlyName != "")
                {
                    PinFriendlyName(pinFriendlyName);
                }
                PinToolTip(pinToolTip);
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(advancedView);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinJsonOut(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip, EPinContainerType containerType, bool advancedView) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                if (pinFriendlyName != "")
                {
                    PinFriendlyName(pinFriendlyName);
                }
                PinToolTip(pinToolTip);
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(containerType);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(advancedView);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinJsonIn : GraphPin
        {
            public GraphPinJsonIn(GraphNode parentNodeRef, GraphPin inputJson, string pinName, string pinFriendlyName, string pinToolTip, bool BoolAdvancedView) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                if (pinFriendlyName.Length > 0)
                {
                    PinFriendlyName(pinFriendlyName);
                }
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                LinkToPin(inputJson);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(BoolAdvancedView);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinJsonIn(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip, bool BoolAdvancedView) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                if (pinFriendlyName.Length > 0)
                {
                    PinFriendlyName(pinFriendlyName);
                }
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(BoolAdvancedView);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinAuthContextIn : GraphPin
        {
            public GraphPinAuthContextIn(GraphNode parentNodeRef, bool BoolAdvancedView) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("AuthenticationContext");
                PinFriendlyName("Authentication Context");
                PinToolTip("Authentication Context\\nPlay Fab Authentication Context Object Reference\\n\\nAuthentication Context:\\r\\nAn optional authentication context (can used in multi-user scenarios)");
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFabCommon.PlayFabAuthenticationContext\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(BoolAdvancedView);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinStringOut : GraphPin
        {
            public GraphPinStringOut(GraphNode parentNodeRef, string stringName) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(stringName);
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("string");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinStringOut(GraphNode parentNodeRef, string stringName, string stringFriendlyName, string stringToolTip, bool advancedView) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(stringName);
                if(stringFriendlyName != "")
                {
                    PinFriendlyName(stringFriendlyName);
                }
                if (stringToolTip != "")
                {
                    PinFriendlyName(stringToolTip);
                }
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("string");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(advancedView);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinStringIn : GraphPin
        {
            public GraphPinStringIn(GraphNode parentNodeRef, string pinToolTip, GraphPin parameterFromCustomEventNode) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("StringValue");
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                PinCategory("string");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                LinkToPin(parameterFromCustomEventNode);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinStringIn(GraphNode parentNodeRef, string functionName) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("FunctionName");
                PinFriendlyName(Wrap("Function Name"));
                PinToolTip("Function Name\\nString\\n\\nFunction Name:\\r\\nThe name of the CloudScript function to execute");
                PinCategory("string");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                DefaultValue(functionName);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinBoolIn : GraphPin
        {
            public GraphPinBoolIn(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                PinFriendlyName(pinFriendlyName);
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                PinCategory("bool");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                DefaultValue("true");
                AutogeneratedDefaultValue("False");
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(true);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinBoolOut : GraphPin
        {
            public GraphPinBoolOut(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip, bool advancedView) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                PinFriendlyName(pinFriendlyName);
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("bool");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(advancedView);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinByteIn : GraphPin
        {
            public GraphPinByteIn(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip, string pinSubCategoryObject, string defaultValue, string autogeneratedDefaultValue) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                PinFriendlyName(pinFriendlyName);
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                PinCategory("byte");
                PinSubCategory("");
                PinSubCategoryObject(pinSubCategoryObject);
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                DefaultValue(defaultValue);
                AutogeneratedDefaultValue(autogeneratedDefaultValue);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(true);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinIntIn : GraphPin
        {
            public GraphPinIntIn(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip, string defaultValue, string autogeneratedDefaultValue) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                PinFriendlyName(pinFriendlyName);
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                PinCategory("int");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                DefaultValue(defaultValue);
                AutogeneratedDefaultValue(autogeneratedDefaultValue);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(true);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinIntOut : GraphPin
        {
            public GraphPinIntOut(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                PinFriendlyName(pinFriendlyName);
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("int");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
            public GraphPinIntOut(GraphNode parentNodeRef, string pinName, string pinFriendlyName, string pinToolTip, bool advancedView) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                PinFriendlyName(pinFriendlyName);
                if (pinToolTip.Length > 0)
                {
                    PinToolTip(pinToolTip);
                }
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("int");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(advancedView);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinStringUserDefined : GraphPin
        {
            public GraphPinStringUserDefined(GraphNode parentNodeRef, string stringName) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenUserDefinedGraphPinText();
                PinName(stringName);
                PinTypeEqualsPinCategory("string");
                DesiredPinDirection(EEdGraphPinDirection.EGPD_Output);
            }
        }
        class GraphPinSelfGameInstanceBPRef : GraphPin
        {
            public GraphPinSelfGameInstanceBPRef(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("self");
                PinFriendlyName("NSLOCTEXT(\"K2Node\", \"Target\", \"Target\")");
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/Engine.BlueprintGeneratedClass'\"/Game/_D/BPs/GameInstanceBP.GameInstanceBP_C\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(true);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinSelfConstructedJsonObject : GraphPin
        {
            public GraphPinSelfConstructedJsonObject(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("self");
                PinFriendlyName("NSLOCTEXT(\"K2Node\",\"Target\",\"Target\")");
                PinToolTip("Target\\nPlay Fab Json Object Object Reference");
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                DefaultObject("/Script/PlayFab.Default__PlayFabJsonObject");
                PersistentGuid("00000000000000000000000000000000");
                bHidden(true);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinWorldContextObject : GraphPin
        {
            public GraphPinWorldContextObject(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("WorldContextObject");
                PinToolTip("World Context Object\\nObject Reference");
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/CoreUObject.Object\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(true);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPin__WorldContextObject : GraphPin
        {
            public GraphPin__WorldContextObject(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("__WorldContextObject");
                PinToolTip("World Context Object\\nObject Reference");
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/CoreUObject.Object\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(true);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinSelfFLibDarkionRef : GraphPin
        {
            public GraphPinSelfFLibDarkionRef(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("self");
                PinFriendlyName("NSLOCTEXT(\"K2Node\", \"Target\", \"Target\")");
                PinToolTip("Target\\nFLib Darkion Object Reference");
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/Engine.BlueprintGeneratedClass'\"/Darkion_Function_Library/FLib_Darkion.FLib_Darkion_C\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                DefaultObject("/Darkion_Function_Library/FLib_Darkion.Default__FLib_Darkion_C");
                PersistentGuid("00000000000000000000000000000000");
                bHidden(true);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinConstructedJsonObjectOut : GraphPin
        {
            public GraphPinConstructedJsonObjectOut(GraphNode parentNodeRef) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("ReturnValue");
                PinToolTip("Return Value\\nPlay Fab Json Object Object Reference\\n\\nCreate new Json object");
                Direction(EEdGraphPinDirection.EGPD_Output);
                PinCategory("object");
                PinSubCategory("");
                PinSubCategoryObject("/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"'");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        class GraphPinStringFieldName : GraphPin
        {
            public GraphPinStringFieldName(GraphNode parentNodeRef, string fieldStringName) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName("FieldName");
                PinToolTip("Field Name\\nString");
                PinCategory("string");
                PinSubCategory("");
                PinSubCategoryObject("None");
                PinSubCategoryMemberReference("()");
                PinValueType("()");
                ContainerType(EPinContainerType.None);
                bIsReference(false);
                bIsConst(false);
                bIsWeakPointer(false);
                bIsUObjectWrapper(false);
                bSerializeAsSinglePrecisionFloat(false);
                DefaultValue(fieldStringName);
                PersistentGuid("00000000000000000000000000000000");
                bHidden(false);
                bNotConnectable(false);
                bDefaultValueIsReadOnly(false);
                bDefaultValueIsIgnored(false);
                bAdvancedView(false);
                bOrphanedPin(false);
                CloseGraphPinText();
            }
        }
        #endregion

        #region <Enums>
        /** Enum used to define which way data flows into or out of this pin. */
        enum EEdGraphPinDirection
        {
            EGPD_Input,
            EGPD_Output
        };
        /** Enum used to define what container type a pin represents. */
        enum EPinContainerType
        {
            None,
            Array,
            Set,
            Map
        };
        #endregion

        #region <Cloudscript function functions>
        string[] GetArgumentsFromCloudscriptFunction(string input)
        {
            List<string> argsWords = new List<string>();
            string[] words = input.Split(' ', ',', '(', ')','[', ']', ';', '\r', '\n', '}', '{');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].StartsWith("args."))
                {
                    argsWords.Add(words[i].Substring(5));
                }
            }
            List<string> uniqueArgs = new List<string>();
            foreach (string word in argsWords)
            {
                bool isUnique = true;
                foreach (string arg in uniqueArgs) 
                {
                    if(arg == word)
                    {
                        isUnique = false;
                    }
                }
                if(isUnique)
                {
                    uniqueArgs.Add(word);
                }
            }
            return uniqueArgs.ToArray();
        }
        static string GetFunctionNameFromCloudscriptFunction(string input)
        {
            if(input.Substring(0, 9) == "handlers.")
            {
                string firstSubString = input.Substring(9, input.Length - 9);
                string secondSubstring = firstSubString.Split(' ')[0];
                return secondSubstring;
            }
            else
            {
                return "Function Name";
            }
        }
        static string[] GetReturnedOutputsFromCloudscriptFunction(string input)
        {
            List<string> outputNames = new List<string>();
            string outputsCombined = input.Substring(input.IndexOf("return"), input.Length - input.IndexOf("return")).Trim();
            string[] outputs = outputsCombined.Split(' ', ',', '{', '}', ':');

            List<string> uniqueOutputNames = outputNames.Distinct().ToList();

            return outputs;
        }
        #endregion

        #region <Helpers>

        public static string GenerateRandomHexNumber()
        {
            Random random = new Random();
            byte[] buffer = new byte[32 / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (32 % 2 == 0)
            {
                return result;
            }
        }
        static string Wrap(string str)
        {
            string result = "\"" + str + "\"";
            return result;
        }


        #endregion


    }

}







