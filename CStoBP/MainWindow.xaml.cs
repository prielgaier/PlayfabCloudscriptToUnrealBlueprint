﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GraphNodeCustomEvent customEventNode = new GraphNodeCustomEvent(GetFunctionNameFromCloudscriptFunction(TB_CloudscriptFunction.Text), GetArgumentsFromCloudscriptFunction(TB_CloudscriptFunction.Text));
            GraphNodePrintString printStringNode = new GraphNodePrintString(customEventNode);
            GraphNodeConstructJsonObject graphNodeConstructJsonObject = new GraphNodeConstructJsonObject();
            GraphNodeSetJsonVariable graphNodeSetJsonVariable = new GraphNodeSetJsonVariable(printStringNode, graphNodeConstructJsonObject, GetFunctionNameFromCloudscriptFunction(TB_CloudscriptFunction.Text));

            Clipboard.SetText(customEventNode.GetBeginObjectClassString() + "\n" + printStringNode.GetBeginObjectClassString() + "\n" + graphNodeConstructJsonObject.GetBeginObjectClassString() + "\n" + graphNodeSetJsonVariable.GetBeginObjectClassString());
            //show copied to clipboard
            BTN_Convert.Content = "Copied to clipboard!";
            //delay 2 seconds
            Task.Delay(2000).ContinueWith(_ =>
            {
                //after 2 seconds, reset button text
                BTN_Convert.Dispatcher.Invoke(() => BTN_Convert.Content = "Convert");
            });
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
        class GraphNodeCustomEvent : GraphNode
        {
            public GraphNodeCustomEvent(string customFunctionName, string[] customFunctionArgs)
            {
                Class = "/Script/BlueprintGraph.K2Node_CustomEvent";
                Name = "K2Node_CustomEvent_54";
                NodePosX = "11568";
                NodePosY = "11168";
                CustomFunctionName = "CS_" + customFunctionName;
                AddPin(new GraphPinDelegateHandle(this));
                AddPin(new GraphPinThen(this));
                foreach(string argumentName in customFunctionArgs)
                {
                    AddPin(new GraphPinStringOut(this, argumentName));
                    AddUserDefinedPin(new GraphPinStringUserDefined(this, argumentName));
                }
            }

        }
        class GraphNodePrintString : GraphNodeKismetFunction
        {
            public GraphNodePrintString(GraphNode nodeBefore)
            {
                Class = "/Script/BlueprintGraph.K2Node_CallFunction";
                FunctionReference = "(MemberParent=/Script/CoreUObject.Class'\"/Script/Engine.KismetSystemLibrary\"',MemberName=\"PrintString\")";
                Name = "K2Node_CallFunction_21";
                NodePosX = "11888";
                NodePosY = "11168";
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
                NodePosX = "12288";
                NodePosY = "11552";
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
        class GraphNodeSetJsonVariable : GraphNodeKismetVariable
        {
            public GraphNodeSetJsonVariable(GraphNode previousNode, GraphNode constructNode, string csFunctionName)
            {
                Class = "/Script/BlueprintGraph.K2Node_VariableSet";
                Name = "K2Node_VariableSet_19";
                VariableReference = "(MemberName=\"CS_" + csFunctionName + "Params" + "\",MemberGuid=" + GenerateRandomHexNumber() +",bSelfContext=True)";
                NodePosX = "12288";
                NodePosY = "11168";
                AddPin(new GraphPinExec(this, previousNode.Pins[1]));
                AddPin(new GraphPinThen(this));
                AddPin(new GraphPinJsonIn(this, constructNode.Pins[2], "CS_" + csFunctionName + "Params", "", ""));
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
            public GraphNodeSetStringFieldInJson(GraphNode jsonObjectNode, GraphPin customEventStringPinParameter, string stringFieldName)
            {
                Class = "/Script/BlueprintGraph.K2Node_CallFunction";
                Name = "K2Node_CallFunction_193";
                FunctionReference = "(MemberParent=/Script/CoreUObject.Class'\"/Script/PlayFab.PlayFabJsonObject\"',MemberName=\"SetStringField\")";
                NodePosX = "12288";
                NodePosY = "11168";
                AddPin(new GraphPinExec(this, jsonObjectNode.Pins[1]));
                AddPin(new GraphPinThen(this));
                AddPin(new GraphPinJsonIn(this, jsonObjectNode.Pins[3], "self", "NSLOCTEXT(\"K2Node\", \"Target\", \"Target\")", "Target\\nPlay Fab Json Object Object Reference"));
                AddPin(new GraphPinStringFieldName(this, stringFieldName));
                AddPin(new GraphPinStringIn(this, stringFieldName, "String Value\\nString", customEventStringPinParameter));
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
            public void PinTypeEqualsPinCategory(string pinTypeEqualsPinCategory)
            {
                this._PinTypeEqualsPinCategory = pinTypeEqualsPinCategory;
                GraphPinText = GraphPinText + "PinType=(PinCategory=" + Wrap(pinTypeEqualsPinCategory) + "),";
            }
            public void DesiredPinDirection(EEdGraphPinDirection desiredPinDirection)
            {
                this._DesiredPinDirection = desiredPinDirection;
                GraphPinText = GraphPinText + "DesiredPinDirection=" + desiredPinDirection.ToString() + ")";
            }

            public void PinSubCategory(string pinSubCategory)
            {
                this._PinSubCategory = pinSubCategory;
                GraphPinText = GraphPinText + "PinType.PinSubCategory=" + Wrap(pinSubCategory) + ",";
            }

            public void PinSubCategoryObject(string pinSubCategoryObject)
            {
                this._PinSubCategoryObject = pinSubCategoryObject;
                GraphPinText = GraphPinText + "PinType.PinSubCategoryObject=" + pinSubCategoryObject + ",";
            }

            public void PinSubCategoryMemberReference(string pinSubCategoryMemberReference)
            {
                this._PinSubCategoryMemberReference = pinSubCategoryMemberReference;
                GraphPinText = GraphPinText + "PinType.PinSubCategoryMemberReference=" + pinSubCategoryMemberReference + ",";
            }
            public void MemberName(string memberName)
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
                GraphPinText = GraphPinText + Wrap("DefaultObject=" + defaultObject) + ",";
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
                LinkedTo("(" + connectToPin._ParentNodeRef.Name + " " + connectToPin._PinId + ")");
                _LinkedTo = "(" + connectToPin._ParentNodeRef.Name + " " + connectToPin._PinId + ")";

                connectToPin.UnCloseGraphPinText();
                connectToPin.LinkedTo("(" + _ParentNodeRef.Name + " " + _PinId + ")");
                connectToPin.CloseGraphPinText();
                connectToPin._LinkedTo = "(" + _ParentNodeRef.Name + " " + _PinId + ")";
            }


        }
        class GraphPinExec:GraphPin
        {
            public GraphPinExec(GraphNode parentNodeRef) : base(parentNodeRef)
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
                LinkToPin(linkToPin);
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
        class GraphPinDelegateHandle : GraphPin
        {
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
        }
        class GraphPinJsonIn : GraphPin
        {
            public GraphPinJsonIn(GraphNode parentNodeRef, GraphPin inputJson, string pinName, string pinFriendlyName, string pinToolTip) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(pinName);
                if(pinFriendlyName.Length > 0)
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
                bAdvancedView(false);
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
        }
        class GraphPinStringIn : GraphPin
        {
            public GraphPinStringIn(GraphNode parentNodeRef, string inputPinName, string pinToolTip, GraphPin customEventNodePin) : base(parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
                OpenGraphPinText();
                PinId(GenerateRandomHexNumber());
                PinName(inputPinName);
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
                LinkToPin(customEventNodePin);
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
            string[] words = input.Split(' ', ',', '(', ')', ';', '\n');
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







