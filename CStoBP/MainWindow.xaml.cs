using System;
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
            GraphNode printStringNode = new GraphNodePrintString(customEventNode);
            Clipboard.SetText(customEventNode.GetBeginObjectClassString() + "\n" + printStringNode.GetBeginObjectClassString());
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TB_CloudscriptFunction != null && TB_CloudscriptFunction.Text.Length > 12 && TB_CloudscriptFunction.Text.Substring(0, 8) == "handlers")
            {
                TB_FunctionArguments.Text = "";
                foreach (string s in GetArgumentsFromCloudscriptFunction(TB_CloudscriptFunction.Text))
                {
                    if(s == GetArgumentsFromCloudscriptFunction(TB_CloudscriptFunction.Text)[0])
                    {
                        TB_FunctionArguments.Text = s;
                    }
                    else
                    {
                        TB_FunctionArguments.Text = TB_FunctionArguments.Text + "\n" + s;
                    }
                }
                TB_FunctionName.Text = GetFunctionNameFromCloudscriptFunction(TB_CloudscriptFunction.Text);
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
            public string NodeGuid = GenerateRandomHexNumber(32);
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
            public void AddPinExec()
            {
                AddPin(new GraphPin(this));
                Pins[Pins.Length].OpenGraphPinText();
                Pins[Pins.Length].PinId(GenerateRandomHexNumber(32));
                Pins[Pins.Length].PinName("execute");
                Pins[Pins.Length].PinToolTip("\\nExec");
                Pins[Pins.Length].PinCategory("exec");
                Pins[Pins.Length].PinSubCategory("");
                Pins[Pins.Length].PinSubCategoryObject("None");
                Pins[Pins.Length].PinSubCategoryMemberReference("()");
                Pins[Pins.Length].PinValueType("()");
                Pins[Pins.Length].ContainerType(EPinContainerType.None);
                Pins[Pins.Length].bIsReference(false);
                Pins[Pins.Length].bIsConst(false);
                Pins[Pins.Length].bIsWeakPointer(false);
                Pins[Pins.Length].bIsUObjectWrapper(false);
                Pins[Pins.Length].bSerializeAsSinglePrecisionFloat(false);
                Pins[Pins.Length].LinkedTo("()");
                Pins[Pins.Length].PersistentGuid("00000000000000000000000000000000");
                Pins[Pins.Length].bHidden(false);
                Pins[Pins.Length].bNotConnectable(false);
                Pins[Pins.Length].bDefaultValueIsReadOnly(false);
                Pins[Pins.Length].bDefaultValueIsIgnored(false);
                Pins[Pins.Length].bAdvancedView(false);
                Pins[Pins.Length].bOrphanedPin(false);
                Pins[Pins.Length].CloseGraphPinText();
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
        class GraphNodeCustomEvent : GraphNode
        {
            public GraphNodeCustomEvent(string customFunctionName, string[] customFunctionArgs)
            {
                Class = "/Script/BlueprintGraph.K2Node_CustomEvent";
                Name = "K2Node_CustomEvent_54";
                NodePosX = "11568";
                NodePosY = "11152";
                CustomFunctionName = "CS_" + customFunctionName;
                AddPin(new GraphPin(this));
                Pins[0].OpenGraphPinText();
                Pins[0].PinId(GenerateRandomHexNumber(32));
                Pins[0].PinName("OutputDelegate");
                Pins[0].Direction(EEdGraphPinDirection.EGPD_Output);
                Pins[0].PinCategory("delegate");
                Pins[0].PinSubCategory("");
                Pins[0].PinSubCategoryObject("None");
                Pins[0].PinSubCategoryMemberReference("(MemberParent=/Script/Engine.BlueprintGeneratedClass'\"/Game/_D/BPs/GameInstanceBP.GameInstanceBP_C\"'");
                Pins[0].MemberName(CustomFunctionName);
                Pins[0].MemberGuid(GenerateRandomHexNumber(32));
                Pins[0].PinValueType("()");
                Pins[0].ContainerType(EPinContainerType.None);
                Pins[0].bIsReference(false);
                Pins[0].bIsConst(false);
                Pins[0].bIsWeakPointer(false);
                Pins[0].bIsUObjectWrapper(false);
                Pins[0].bSerializeAsSinglePrecisionFloat(false);
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
                Pins[1].PinId(GenerateRandomHexNumber(32));
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
                foreach(string s in customFunctionArgs)
                {
                    AddPin(new GraphPin(this));
                    Pins[Pins.Length - 1].OpenGraphPinText();
                    Pins[Pins.Length - 1].PinId(GenerateRandomHexNumber(32));
                    Pins[Pins.Length - 1].PinName(s);
                    Pins[Pins.Length - 1].Direction(EEdGraphPinDirection.EGPD_Output);
                    Pins[Pins.Length - 1].PinCategory("string");
                    Pins[Pins.Length - 1].PinSubCategory("");
                    Pins[Pins.Length - 1].PinSubCategoryObject("None");
                    Pins[Pins.Length - 1].PinSubCategoryMemberReference("");
                    Pins[Pins.Length - 1].PinValueType("");
                    Pins[Pins.Length - 1].ContainerType(EPinContainerType.None);
                    Pins[Pins.Length - 1].bIsReference(false);
                    Pins[Pins.Length - 1].bIsConst(false);
                    Pins[Pins.Length - 1].bIsWeakPointer(false);
                    Pins[Pins.Length - 1].bIsUObjectWrapper(false);
                    Pins[Pins.Length - 1].bSerializeAsSinglePrecisionFloat(false);
                    Pins[Pins.Length - 1].PersistentGuid("00000000000000000000000000000000");
                    Pins[Pins.Length - 1].bHidden(false);
                    Pins[Pins.Length - 1].bNotConnectable(false);
                    Pins[Pins.Length - 1].bDefaultValueIsReadOnly(false);
                    Pins[Pins.Length - 1].bDefaultValueIsIgnored(false);
                    Pins[Pins.Length - 1].bAdvancedView(false);
                    Pins[Pins.Length - 1].bOrphanedPin(false);
                    Pins[Pins.Length - 1].CloseGraphPinText();
                    AddUserDefinedPin(new GraphPin(this));
                    UserDefinedPins[UserDefinedPins.Length - 1].OpenUserDefinedGraphPinText();
                    UserDefinedPins[UserDefinedPins.Length - 1].PinName(s);
                    UserDefinedPins[UserDefinedPins.Length - 1].PinTypeEqualsPinCategory("string");
                    UserDefinedPins[UserDefinedPins.Length - 1].DesiredPinDirection(EEdGraphPinDirection.EGPD_Output);
                }
            }
        }
        class GraphNodePrintString : GraphNodeKismetFunction
        {
            public override string GetBeginObjectClassString()
            {
                string myPins = PinsToString(Pins);
                string myUserDefinedPins = PinsToString(UserDefinedPins);
                return
                   "Begin Object Class=" + Class + " Name=" + Wrap(Name) + "\n" +
                   "   " + "FunctionReference=(MemberParent=/Script/CoreUObject.Class'\"/Script/Engine.KismetSystemLibrary\"',MemberName=\"PrintString\")" + "\n" +
                   "   " + "NodePosX=" + NodePosX + "\n" +
                   "   " + "NodePosY=" + NodePosY + "\n" +
                   "   " + "AdvancedPinDisplay=Hidden" + "\n" +
                   "   " + "EnabledState = DevelopmentOnly" + "\n" +
                   "   " + "NodeGuid=" + NodeGuid + "\n" +
                   "   " + myPins + "\n" +
                   "   " + myUserDefinedPins + "\n" +
                   "End Object";
            }
            public GraphNodePrintString(GraphNode nodeBefore)
            {
                Class = "/Script/BlueprintGraph.K2Node_CallFunction";
                FunctionReference = "(MemberParent=/Script/CoreUObject.Class'\"/Script/Engine.KismetSystemLibrary\"',MemberName=\"PrintString\")";
                Name = "K2Node_CallFunction_21";
                NodePosX = "11888";
                NodePosY = "11168";
                AddPin(new GraphPin(this));
                Pins[0].OpenGraphPinText();
                Pins[0].PinId(GenerateRandomHexNumber(32));
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
                Pins[1].PinId(GenerateRandomHexNumber(32));
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
                Pins[2].PinId(GenerateRandomHexNumber(32));
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
                Pins[3].PinId(GenerateRandomHexNumber(32));
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
                Pins[4].PinId(GenerateRandomHexNumber(32));
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
                Pins[5].PinId(GenerateRandomHexNumber(32));
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
                Pins[6].PinId(GenerateRandomHexNumber(32));
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
                Pins[7].PinId(GenerateRandomHexNumber(32));
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
                Pins[8].PinId(GenerateRandomHexNumber(32));
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
                Pins[9].PinId(GenerateRandomHexNumber(32));
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
        }
        class GraphPin
        {
            //example values in comments
            public GraphNode _ParentNodeRef;
            public string _PinId = GenerateRandomHexNumber(32);
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

            public GraphPin(GraphNode parentNodeRef)
            {
                _ParentNodeRef = parentNodeRef;
            }

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

            return argsWords.ToArray();
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
        static string[] GetUniqueStrings(string[] inputArray)
        {
            List<string> uniqueStrings = new List<string>();
            foreach (string str in inputArray)
            {
                if (!uniqueStrings.Contains(str))
                {
                    uniqueStrings.Add(str);
                }
            }
            return uniqueStrings.ToArray();
        }
        static string ConsoleReadLinesTillEnd(string previousLines)
        {
            previousLines = previousLines + Console.ReadLine();
            if (previousLines.EndsWith("END") != true)
            {
                ConsoleReadLinesTillEnd(previousLines);
            }
            previousLines = previousLines.Substring(0, previousLines.Length - 3);
            return previousLines;
        }
        public static string GenerateRandomHexNumber(int digits = 32)
        {
            Random random = new Random();
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }
        static string Wrap(string str)
        {
            string result = "\"" + str + "\"";
            return result;
        }
        #endregion

    }

}







