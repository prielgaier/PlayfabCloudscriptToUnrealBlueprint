   M S B A M L   `   `   `  ����A  =CStoBP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:CStoBPCStoBP  5   	       CStoBP.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:CStoBP     5   	   $	��978��5      $
��1181��6'     � WindowStartupLocation$  CenterScreen=�64    � WindowStyle$ ThreeDBorderWindow=�6Y   $��Center=�6z   $��Center=�6�   $
��Auto=�6�   .��5      � ��5	   
   �� 5
       �� EndPoint$ 0.5,1&�6"    �� 
StartPoint$ 0.5,0&�63   .��5      �  � Color$ Black��6   � 5      $ 	#FF0E0E0E��6    � Offset$ 1Q�61   5      .+�5   
   �� -   5         ��Name$ TB_FunctionName��5      $
��Wrap=�6-   $�� Function Name��6A   ���	#FFF55151$��32(�6n   $��367,0,10,146q�6|    J� RenderTransformOrigin$ 0.5,0.5&�6�   $��55��6�   $��Bottom=�6�   ��5      �� 5      $ 0.5,1&�6&   $ 0.5,0&�67   .��5      � $ Black��6#   $ 1Q�61   � 5      $ 	#FF151515��6#   $ 0Q�65   5      5      �� 5   
   -   5         ��Name$ TB_CloudscriptFunction��5      	 �� TextWrapping$
	 Wrap=�62   $��Microsoft Tai Le)�6h   
�������$��15(�6�   
 �� AcceptsReturn$

 True��6�   4  .System.Windows.Automation.AutomationProperties   HelpText$/ )Enter Javascript Playfab Cloudscript Here��6�   $H��BPaste your Javascript PlayFab Cloudscript handler function here...��6  $��10,10,10,227q�6P  $
��True��6f  " �� VerticalScrollBarVisibility$
 Auto=�6�  $ �� HorizontalScrollBarVisibility$
 Auto=�6�  .��6�  ��5      �� 5      $ 0.5,1&�6&   $ 0.5,0&�67   .��5      � $ Black��6#   � 5      $ 	#FF171717��6#   $ 1Q�65   5      .��5   "   5      �� 5   
   -   5        $ TB_FunctionArguments��5      $
��Wrap=�62   $��Function Arguments��6F   $	��259��6`   ���	#FFEC5CCC$��Bottom=�6�   $
��Left=�6�   $��16(�6�   $��	10,0,0,21q�6�   $	��181��6�   ��5       �� 5!      $ 0.5,1&�6&   $ 0.5,0&�67   .��5"      � $ Black��6#   $ 1Q�61   � 5#      $ 	#FF151515��6#   $ 0Q�65   5$      5&      �� 5'   
   -   5         ��Name$ BTN_Convert��5'      $��Convert��6&   $	��115��68   $	��236��6E   $��Bottom=�6f   $��Right=�6�   
���   �$��20(�6�   $��Center=�6�   $��Center=�6�   $��	0,0,10,21q�6   � Cursor$
 Hand��6  ���	#FF0886D3��5(      �� 5)      $ 0.5,1&�6&   $ 0.5,0&�67   .��5*      � $ 	#FF0970D0��6#   $ 1Q�65   � 5+      $ 	#FF00FFE2��6#   $ 0Q�65   5,      5.      �� 5/   
   -   5        $ 	TB_Prefix��5/      $��275,0,0,146q�6%   $
	 Wrap=�6:   ���	#FFF55151$��Bottom=�6e   $��54��6�   $��32(�6�   $��CS��6�   ���	#FFF34F4F$
��Left=�6�   $��87��6�   .��6  ��50      �� 51      $ 0.5,1&�6&   $ 0.5,0&�67   .��52      � $ Black��6#   $ 1Q�61   � 53      $ 	#FF151515��6#   $ 0Q�65   54      .��55   "   56      �� 57   
   -   5         ��Name$ CB_PrintString��57      $��Add print string Node?��6+   $��
0,0,265,64q�6L   $��Bottom=�6`   $��25��6{   ���	#FFF55151���	#FFF34F4F$��18(�6�    d� 	IsChecked$
 TrueY�6	  ���	#FF370000$��Right=�62  $	��204��6N  $ 	0.65,0.55&�6Z  58      59      