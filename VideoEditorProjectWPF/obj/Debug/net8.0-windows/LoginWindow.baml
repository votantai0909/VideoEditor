   M S B A M L   `   `   `  ����P  LVideoEditorProjectWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:VideoEditorProjectWPFVideoEditorProjectWPF  5   	   '    !VideoEditorProjectWPF.LoginWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:VideoEditorProjectWPF     5   	     � Title$  LoginWindow��5   	   $	��450��6   $	��800��6*    � WindowStartupLocation$ CenterScreen=�67    � 
ResizeMode$ NoResize=�6\   ���#F4F4F9$��Segoe UI)�6�   $��16(�6�   .��5	      � .+�5   
   �� $��Center=�6   $	��320��61   $	��Top=�5      $	��500��6,   ���#FFFFFF���#DDDDDD �� RadiusX$ 20��5       �� RadiusY$ 20��6!   $��0,40,0,0q�6.   �� 5   
   $��Login��6   $��Center=�5      $	��Top=�6-   $
��Bold"�5      $��36(�6"   ���#4A90E2$��0,20,0,0q�5      �� 5   
   $��Email��6   $
��Left=�5      $	��Top=�6+   $��
80,100,0,0q�5      ���#555555$��14(�69   �� 5   
   -   5         ��Name$ EmailTextBox��5      $
��Left=�5      $	��Top=�6-   $��
180,95,0,0q�5      $	��300��6&   $��40��62   ���#FAFAFA���#CCCCCC$��5q�5       $��14(�6   $��Center=�5!      $��1q�5"      �� 5%   
   $��Password��6   $
��Left=�5&      $	��Top=�6+   $��
80,160,0,0q�5'      ���#555555$��14(�69   �� 5*   
   -   5        $ PasswordTextBox��5*      $
��Left=�5+      $	��Top=�61   $��180,155,0,0q�5,      $	��300��6+   $��40��67   ���#FAFAFA���#CCCCCC$��5q�5.      $��14(�6"   $��Center=�5/      $��1q�50      �� 53   
   -   5         ��Name$ LoginButton��53      $��Login��6&   $
��Left=�54      $	��Top=�6,   $��180,220,0,0q�55      $	��140��6&   $��45��62   ���#4A90E2
�������$��16(�56   9   $
��Bold"�57      $��0q�6#    � Cursor$
 Hand��58      ��59      �� 5:       �� 
TargetType" ��6   .�5;      �� 	 �� Property	 � ��
 �� Value
 �#4A90E263   �� 5<      	 � ��

 �����63   �� 5=      	 �@J�Opacity$
 1Q�60    �� Triggers 5>      P� 5?       P� Property �@J�IsMouseOver P� Value$
 True��69   . �5@      �� 	 � ��
 �#357ABD6;   �� 5A      	 �@J�Opacity$	
 0.9Q�68   5B      5D      5F      �� 5I   
   -   5        $ 
QuitButton��5I      $
��Quit��6%   $
��Left=�5J      $	��Top=�6,   $��340,220,0,0q�5K      $	��140��6&   $��45��62   ���#FF5A5F
�������$��16(�5L   9   $
��Bold"�5M      $��0q�6#   $
 Hand��5N      ��5O      �� 5P      " ��6   .�5Q      �� 	 � ��
 �#FF5A5F63   �� 5R      	 � ��

 �����63   �� 5S      	 �@J�Opacity$
 1Q�60    5T      P� 5U       �@J�IsMouseOver$
 True��69   . �5V      �� 	 � ��
 �#E048486;   �� 5W      	 �@J�Opacity$	
 0.9Q�68   5X      5Z      5\      5]      5^      